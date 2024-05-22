using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderProcessingSystem.Models.Products
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}