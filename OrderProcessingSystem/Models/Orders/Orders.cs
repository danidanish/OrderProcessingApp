using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderProcessingSystem.Models.Orders
{
    public class Orders
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Name { get;set; }
    }
}