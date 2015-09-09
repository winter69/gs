using GroveCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GroveCM.Repository
{
    public class OrderRepository
    {
        private GroveCMContext db = new GroveCMContext();

        public Order GetNewOrder()
        {
            Order order = new Order
            {
                OrderNumber = Guid.NewGuid().ToString(),
                PaymentReference = Guid.NewGuid().ToString(),
                SubTotal = 250,
                Shipping = 10,
                Tax = 25,
                ItemName = "Super Widget",
                Currency = "GBP",
                Quantity = 1,
                OrderDate = DateTime.Now,
                Username = "GrahamSandwell",
                SKU = "SKU1"
            };

            return order;
        }

        public bool SaveOrder(Order order)
        {
            order.OrderDate = System.DateTime.Now;
            //Save order details to database
                db.Orders.Add(order);
                db.SaveChanges();

            return true;
        }

    }
}