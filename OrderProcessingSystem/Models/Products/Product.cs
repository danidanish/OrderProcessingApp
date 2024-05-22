using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderProcessingSystem.Models.Products
{
    public class Product
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
    }
}