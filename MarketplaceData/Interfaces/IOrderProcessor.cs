using VetClassLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace VetClassLibrary.Interfaces
{
    public interface IOrderProcessor
    {
        bool CanProcess(Item product);
        Task ProcessAsync(Item product, double quantity);
    }
}
