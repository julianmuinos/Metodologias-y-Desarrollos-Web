using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyApp.Core.Models
{
    public class Item
    {
        public Product Product { get; }
        public uint Quantity { get; set; }
        public decimal Total
        {
            get { return Product.Price * Quantity; }
        }
        public Item(Product pProduct)
        {
            Product = pProduct;
            Quantity = 1;
        }
        public Item(Product pProduct, uint pQuantity)
        {
            Product = pProduct;
            Quantity = pQuantity;
        }
        public uint Add(uint pQuantity = 1)
        {
            return Quantity += pQuantity;
        }
        public uint Subtract(uint pQuantity = 1)
        {
            return Quantity -= pQuantity;
        }
        public uint Update(uint pQuantity)
        {
            return Quantity = pQuantity;
        }
    }
}