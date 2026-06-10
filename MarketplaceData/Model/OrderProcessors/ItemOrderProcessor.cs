using VetClassLibrary.Interfaces;
using VetClassLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace VetClassLibrary.Model.OrderProcessors
{
    public class ItemOrderProcessor : IOrderProcessor
    {
        private readonly IStorageService _storageService;

        public ItemOrderProcessor(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public bool CanProcess(Item product)
        {
            return product is Item;
        }

        public async Task ProcessAsync(Item product, double quantity)
        {
            if (product is Item good)
            {
                double qty = _storageService.GetQty(product);
                if (qty > quantity)
                {
                    _storageService.UpdateQty(good, qty - quantity);
                }
                else
                {
                    throw new Exception($"Not enough \"{good.Name}\" in storage");
                }
            }
        }
    }
}
