using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Model;
using VetClassLibrary.Model.Storage;

namespace VetClassLibrary.Interfaces
{
    public interface IStorageService
    {
        Item UpdateQty(int id, double qty);
        StorageItem UpdateQty(Item item, double qty);
        StorageItem GetStorageItemById(int id);
        List<StorageItem> GetStorageItems();
        double GetQty(int id);
        double GetQty(Item item);
        void InitalizeNewItem(Item item);
    }
}
