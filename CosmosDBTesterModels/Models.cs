using System;
using System.Collections.Generic;

namespace CosmosDBTesterModels
{
    public class Product
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public string Image { get; set; }
    }


    public class OrderItem
    {
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public DateTime DatePlaced { get; set; }
        public IEnumerable<OrderItem> Items { get; set; }
        public bool Shipped { get; set; }

        public string ShippingState { get; set; }
    }

    public class Customer
    {
        public int CustomerID { get; set; }

        public double OrderTotal { get; set; }

        public Bogus.Person Person { get; set; }
    }
}
