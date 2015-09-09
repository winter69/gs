using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GroveCM.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Username { get; set; } //

        public string OrderNumber { get; set; } //

        public string PaymentReference { get; set; } //

        public decimal SubTotal { get; set; } //

        public decimal Shipping { get; set; } //

        public decimal Tax { get; set; } //

        public decimal Fees { get; set; } //

        public string ItemName { get; set; }

        public string Currency { get; set; }

        public int Quantity { get; set; }

        public string SKU { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public decimal Total { get; set; }
    }
}