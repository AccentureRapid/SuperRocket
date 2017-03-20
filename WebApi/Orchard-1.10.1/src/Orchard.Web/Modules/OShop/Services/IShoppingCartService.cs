﻿using Orchard;
using Orchard.ContentManagement;
using OShop.Models;
using OShop.ViewModels;
using System.Collections.Generic;

namespace OShop.Services {
    public interface IShoppingCartService : IDependency {
        IEnumerable<ShoppingCartItemRecord> ListItems();
        void Add(int ItemId, string ItemType = ProductPart.PartItemType, int Quantity = 1);
        void UpdateQuantity(int Id, int Quantity);
        void Remove(int Id);
        void Empty();

        void SetProperty<T>(string Key, T Value);
        T GetProperty<T>(string Key);
        void RemoveProperty(string Key);

        ShoppingCart BuildCart();

        IContent BuildOrder();
    }
}
