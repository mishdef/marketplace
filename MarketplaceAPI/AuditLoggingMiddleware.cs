using System.Diagnostics;
using System.Text;

namespace MarketplaceAPI
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyMemoryStream = new MemoryStream();
            context.Response.Body = responseBodyMemoryStream;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                if (context.Response.StatusCode >= 400)
                {
                    var requestBody = IsTextPayload(context.Request.ContentType)
                        ? await ReadRequestBodyAsync(context.Request)
                        : "[Binary or unsupported payload]";

                    var responseBody = IsTextPayload(context.Response.ContentType)
                        ? await ReadResponseBodyAsync(context.Response)
                        : "[Binary or unsupported payload]";

                    _logger.LogError("HTTP ERROR {StatusCode} | {Method} {Path} | Elapsed: {Elapsed}ms\nRequest Body: {ReqBody}\nResponse Body: {ResBody}",
                        context.Response.StatusCode,
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.ElapsedMilliseconds,
                        requestBody,
                        responseBody);
                }

                responseBodyMemoryStream.Position = 0;
                await responseBodyMemoryStream.CopyToAsync(originalResponseBodyStream);
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            return string.IsNullOrWhiteSpace(body) ? "[Empty]" : body;
        }

        private async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            response.Body.Position = 0;
            using var reader = new StreamReader(response.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            response.Body.Position = 0;

            return string.IsNullOrWhiteSpace(body) ? "[Empty]" : body;
        }
        
        private bool IsTextPayload(string? contentType)
        {
            if (string.IsNullOrEmpty(contentType)) return false;

            var ct = contentType.ToLower();
            return ct.Contains("application/json") ||
                   ct.Contains("application/xml") ||
                   ct.Contains("text/");
        }
    }
}
