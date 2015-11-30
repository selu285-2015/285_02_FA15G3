using FuNDs.Models;
using log4net;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Configuration;
using System.Web.Mvc;

namespace FuNDs.Controllers
{
    public class PaypalController : Controller
    {
        private FundRaisersDbContext db = new FundRaisersDbContext();

        // GET: Paypal1
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult PaymentWithPaypal(string DonationAmount)
        {

          //  public ActionResult PaymentWithPaypal(int id) { 
     // Donor donor = db.Donors.FirstOrDefault(s => s.DonorId ==);
          //  double amount = donor.DonationAmount;
          //  string amount1 = amount + "";
            //getting the apiContext as earlier
            APIContext apiContext = Configuration.GetAPIContext();
          

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class

                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    // So we have provided URL of this controller only
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/Paypal/PaymentWithPayPal?";

                    //guid we are generating for storing the paymentID received in session
                    //after calling the create function and it is used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, DonationAmount);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("Failure");
                    }
                }
            }
            
           catch (Exception ex)
            {
               // Logger.log("Error" + ex.Message);
              // return View("Failure");
            }   
            var txToken = "8R796233JB671434K";
            var authToken = "HlTvz4_zZRExCKYvBHDr8AsNgspBPJIJQ2D97trUfzTk7_qTeY2_6QGlK38";
            var query = string.Format("cmd=_notify-synch&tx={0}&at={1}",
                              txToken, authToken);

            return RedirectToAction("Success"); 
          //  RedirectToAction ("Success");
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        // this is new


        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string DonationAmount)
        {

            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(new Item()
            {
                name = "Item Name",
                currency = "USD",
                price = DonationAmount,
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            // similar as we did for credit card, do here and create details object
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = DonationAmount
            };

            // similar as we did for credit card, do here and create amount object
            var amount = new Amount()
            {
                currency = "USD",
                total = DonationAmount, // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();
            var invoice = Convert.ToString((new Random()).Next(100000));
            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = invoice,
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
        }

        // this is end

        //private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        //{

        //    //similar to credit card create itemlist and add item objects to it
        //    var itemList = new ItemList() { items = new List<Item>() };

        //    itemList.items.Add(new Item()
        //    {
        //        name = "Item Name",
        //        currency = "USD",
        //        price = "5",
        //        quantity = "1",
        //        sku = "sku"
        //    });

        //    var payer = new Payer() { payment_method = "paypal" };

        //    // Configure Redirect Urls here with RedirectUrls object
        //    var redirUrls = new RedirectUrls()
        //    {
        //        cancel_url = redirectUrl,
        //        return_url = redirectUrl
        //    };

        //    // similar as we did for credit card, do here and create details object
        //    var details = new Details()
        //    {
        //        tax = "1",
        //        shipping = "1",
        //        subtotal = "5"
        //    };

        //    // similar as we did for credit card, do here and create amount object
        //    var amount = new Amount()
        //    {
        //        currency = "USD",
        //        total = "7", // Total must be equal to sum of shipping, tax and subtotal.
        //        details = details
        //    };

        //    var transactionList = new List<Transaction>();

        //    transactionList.Add(new Transaction()
        //    {
        //        description = "Transaction description.",
        //        invoice_number = "your invoice number",
        //        amount = amount,
        //        item_list = itemList
        //    });

        //    this.payment = new Payment()
        //    {
        //        intent = "sale",
        //        payer = payer,
        //        transactions = transactionList,
        //        redirect_urls = redirUrls
        //    };

        //    // Create a payment using a APIContext
        //    return this.payment.Create(apiContext);
        //}

      


        
        public static class Configuration
        {
            //these variables will store the clientID and clientSecret
            //by reading them from the web.config
            public readonly static string ClientId;
            public readonly static string ClientSecret;

            static Configuration()
            {
                var config = GetConfig();
                ClientId = config["clientId"];
                ClientSecret = config["clientSecret"];
            }

            // getting properties from the web.config
            public static Dictionary<string, string> GetConfig()
            {
                return PayPal.Manager.ConfigManager.Instance.GetProperties();
            }

            private static string GetAccessToken()
            {
                // getting accesstocken from paypal                
                string accessToken = new OAuthTokenCredential
            (ClientId, ClientSecret, GetConfig()).GetAccessToken();

                return accessToken;
            }

            public static APIContext GetAPIContext()
            {
                // return apicontext object by invoking it with the accesstoken
                APIContext apiContext = new APIContext(GetAccessToken());
                apiContext.Config = GetConfig();
                return apiContext;
            }

          


            }
   

    

        public ActionResult Success() {


            return View();

        }

    }
}