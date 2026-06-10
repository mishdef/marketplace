using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Interfaces;
using VetClassLibrary.Model;
using VetClassLibrary.Model.Storage;

namespace VetClassLibrary.Services
{
    public class StorageService : IStorageService
    {
        private readonly AppDbContext _db;

        public StorageService(AppDbContext db)
        {
            _db = db;
        }

        public double GetQty(int id)
        {
            var storageItem = _db.StorageItems.Find(id);
            if (storageItem != null && !storageItem.Item.IsDeleted)
            {
                return storageItem.Qty;
            }
            else
            {
                throw new Exception("Storage item not found");
            }
        }

        public double GetQty(Item item)
        {
            var storageItem = _db.StorageItems.Find(item.Id);
            if(storageItem != null && !storageItem.Item.IsDeleted)
            {
                return storageItem.Qty;
            }
            else
            {
                throw new Exception("Storage item not found");
            }
        }

        public StorageItem GetStorageItemById(int id)
        {
            var storageItem = _db.StorageItems.Find(id);
            if (storageItem != null && !storageItem.Item.IsDeleted)
            {
                return storageItem;
            }
            else
            {
                throw new Exception("Storage item not found");
            }
        }

        public List<StorageItem> GetStorageItems(int companyId)
        {
            return _db.StorageItems.Include(si => si.Item).Where(si => !si.Item.IsDeleted && si.Item.CompanyId == companyId).ToList();
        }

        public void InitalizeNewItem(Item item)
        {
            _db.StorageItems.Add(new StorageItem { Item = item, Qty = 0 });
            _db.SaveChanges();
        }


        public Item UpdateQty(int id, double qty)
        {
            var storageItem = _db.StorageItems.Find(id);
            if (storageItem != null && !storageItem.Item.IsDeleted)
            {
                storageItem.Qty = qty;
                _db.SaveChanges();
                return storageItem.Item;
            }
            else
            {
                throw new Exception("Storage item not found");
            }
        }

        public StorageItem UpdateQty(Item item, double qty)
        {
            var storageItem = _db.StorageItems.Find(item.Id);
            if (storageItem != null && !storageItem.Item.IsDeleted)
            {
                storageItem.Qty = qty;
                _db.SaveChanges();
                return storageItem;
            }
            else
            {
                throw new Exception("Storage item not found");
            }
        }


    }
}
