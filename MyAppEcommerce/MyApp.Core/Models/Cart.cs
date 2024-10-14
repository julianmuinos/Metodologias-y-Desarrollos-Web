using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApp.Core.Models
{
    public class Cart
    {
        public int Id { get; }
        public User User { get; }
        public DateTime Date { get; }
        private List<Item> _items = new List<Item>();
        public decimal Total
        {
            get
            {
                decimal _total = 0;
                _items.ForEach(item => _total += item.Total);
                return _total;
            }
        }

        public Cart(int pId, User pUser, DateTime pDate)
        {
            Id = pId;
            User = pUser;
            Date = pDate;
        }
        public Cart(User pUser)
        {
            User = pUser;
        }

        public Cart() { }
        

        public List<Item> GetItems()
        {
            List<Item> _aux = new List<Item>();
            _items.ForEach(item => { _aux.Add(new Item(item.Product, item.Quantity)); });
            return _aux;
        }
        public void AddItem(Product pProduct, uint pQuantity = 1)
        {
            Item _item = _items.FirstOrDefault(item => item.Product.Id == pProduct.Id);
            if (_item != null) _item.Add(pQuantity);
            else _items.Add(new Item(pProduct, pQuantity));
        }
        public void SubtractItem(Product pProduct, uint pQuantity = 1)
        {
            Item _item = _items.FirstOrDefault(item => item.Product.Id == pProduct.Id);
            if (_item != null)
            {
                _item.Subtract(pQuantity);
                if (_item.Quantity == 0) _items.Remove(_item);
            }
        }
    }
}