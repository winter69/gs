using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GroveCM.Repository
{
    public class PaymentRepository
    {
        private OAuthTokenCredential _authToken;
        private APIContext _api;

        public Payment CreatePayment(GroveCM.Models.Order order, string cancelUrl, string returnUrl)
        {
            var amount = CreateAmount(order);

            var itemList = new ItemList() { items = new List<Item>() };
            itemList.items.Add(new Item()
            {
                name = order.ItemName,
                currency = order.Currency,
                price = order.SubTotal.ToString(),
                quantity = order.Quantity.ToString(),
                sku = order.SKU
            }); 
            
            var payment = new Payment
            {
                transactions = new List<Transaction> 
                                             { 
                                                new Transaction
                                                {
                                                    amount = amount,
                                                    description = "Purchase From Widget Store",
                                                    item_list = itemList
                                                } 
                                             },
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = cancelUrl,
                    return_url = returnUrl
                }
            };

            payment = payment.Create(Api);

            return payment;
        }

        //Execute payment
        public Payment ConfirmPayment(string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution
            {
                payer_id = payerId
            };

            var payment = new Payment { id = paymentId };

            return payment.Execute(Api, paymentExecution);
        }

        private Amount CreateAmount(GroveCM.Models.Order order)
        {
            var details = new Details
            {
                subtotal = order.SubTotal.ToString("0.00", CultureInfo.CurrentCulture),
                shipping = order.Shipping.ToString("0.00", CultureInfo.CurrentCulture),
                tax = order.Tax.ToString("0.00", CultureInfo.CurrentCulture),
                //fee = order.Fees.ToString("0.00", CultureInfo.CurrentCulture)
            };

            var total = order.SubTotal + order.Shipping + order.Tax; //+ order.Fees;

            var amount = new Amount
            {
                currency = "GBP",
                details = details,
                total = total.ToString("0.00", CultureInfo.CurrentCulture)
            };

            return amount;
        }

        private OAuthTokenCredential ApiAccessToken
        {
            get
            {
                // Create or return security Token
                if (this._authToken != null)
                {
                    return this._authToken;
                }

                var clientId = ConfigurationManager.AppSettings["clientId"];
                var secretToken = ConfigurationManager.AppSettings["clientSecret"];
                var config = new Dictionary<string, string> { { "mode", "sandbox" } };

                this._authToken = new OAuthTokenCredential(clientId, secretToken, config);

                return this._authToken;
            }
        }

        private APIContext Api
        {
            get
            {
                // Create or return API Context
                return this._api ?? (this._api = new APIContext(this.ApiAccessToken.GetAccessToken()));
            }
        }
    }
}