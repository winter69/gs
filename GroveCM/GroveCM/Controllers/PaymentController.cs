using GroveCM.Models;
using GroveCM.Repository;
using PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroveCM.Controllers
{
    public class PaymentController : Controller
    {
        // Use dependency injection rather
        private GroveCMContext db = new GroveCMContext();
        private PaymentRepository paymentRepository = new PaymentRepository();
        private OrderRepository orderRepository = new OrderRepository();

        public ActionResult Index2()
        {
            GroveCM.Models.Order order = orderRepository.GetNewOrder();

            return View(order);
        }

        public ActionResult Purchase(GroveCM.Models.Order order)
        {
            order.OrderDate = System.DateTime.Now;
            //Save order details to database
            if (ModelState.IsValid)
            {
                orderRepository.SaveOrder(order);
            }


            var baseUrl = string.Concat(this.Request.Url.Scheme, "://", this.Request.Url.Authority);

            try
            {
                var guid = Convert.ToString((new Random()).Next(100000));
                var cancelUrl = this.Url.Action("PurchaseUnsucessful", "Payment");
                var returnUrl = this.Url.Action("PurchaseComplete", "Payment", 
                    new { orderNumber = order.OrderNumber.ToLower(), guid = guid });

                var payment = paymentRepository.CreatePayment(order, string.Concat(baseUrl, cancelUrl), string.Concat(baseUrl, returnUrl));

                if (!payment.WasCreated())
                {
                    return this.RedirectToAction("PurchaseUnsucessful");
                }
                
                // Save order with payment.id somewhere on it
                TempData["PaymentId"] = payment.id;

                return this.Redirect(payment.GetApprovalUrl());
            }

            catch (PayPalException ex)
            {
                this.ViewBag.Error = ((ConnectionException)ex.InnerException).Response;
            }
            catch (Exception ex)
            {
                this.ViewBag.Error = ex.Message;
            }

            return this.RedirectToAction("purchaseunsucessful");
        }

        public ActionResult PurchaseUnsucessful()
        {
            return this.View();
        }

        public ActionResult PurchaseComplete(string orderNumber, string payerId)
        {
            try
            {
                GroveCM.Models.Order order = db.Orders.Where(a => a.OrderNumber == orderNumber).FirstOrDefault(); ;

                this.paymentRepository.ConfirmPayment(payerId, TempData["PaymentId"].ToString());

                return this.View("purchasecomplete", order);
            }
            catch (PayPalException ex)
            {
                this.ViewBag.Error = ((ConnectionException)ex.InnerException).Response;
            }
            catch (Exception ex)
            {
                this.ViewBag.Error = ex.Message;
            }

            return this.RedirectToAction("purchaseunsucessful");
        }
    }
    
    public static class PaymentExtensions
    {
        public static string GetApprovalUrl(this Payment payment)
        {
            var x = payment.links.First(link => link.rel.ToLowerInvariant() == "approval_url".ToLowerInvariant()).href;
            return payment.links.First(link => link.rel.ToLowerInvariant() == "approval_url".ToLowerInvariant()).href;
        }

        public static bool WasSucessful(this Payment payment)
        {
            return payment.state.ToLowerInvariant() == "approved";
        }

        public static bool WasCreated(this Payment payment)
        {
            return payment.state.ToLowerInvariant() == "created";
        }
    }
}