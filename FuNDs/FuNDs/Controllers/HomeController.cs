using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FuNDs.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNet.Identity;

namespace FuNDs.Controllers
{
    public class HomeController : Controller
    {
        private FundRaisersDbContext db = new FundRaisersDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult HowItWorks()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(contactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                MailMessage mail = new MailMessage();
                mail.To.Add("pankaj.sherchan@selu.edu");
                mail.From = new MailAddress("Convergent.origin@gmail.com");
                mail.Subject = "Change Password";



                // mail.Body = "Hello there!" + "<br/>" + "Please click on the link below to reset your password... " + "<br/>" + callbackUrl + "<br/>" + "<br/>" + "<br/>" + "<br/>" + "<br/>" + "If this is not you, please send us an email at convergent.origin@gmail.com " + "<br/>" + "<br/>" + "<br/>" + "Sincerely," + "<br/>" + "Team Origin";
                mail.IsBodyHtml = true;
                //var message = new MailMessage();
                //message.To.Add(new MailAddress("pankaj.sherchan@selu.edu"));  // replace with valid value 
                //message.From = new MailAddress("Convergent.origin@gmail.com");  // replace with valid value
                //message.Subject = "Your email subject";
                mail.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                // message.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                ("Convergent.origin@gmail.com", "fall2015cmps285");// Enter senders User name and password
                smtp.EnableSsl = true;
                Console.WriteLine("Sending email .... ");
                smtp.Send(mail);


                //using (var smtp = new SmtpClient())
                //{
                //    var credential = new NetworkCredential
                //    {
                //    UserName = "Convergent.origin@gmail.com",  // replace with valid value
                //        Password = "fall2015cmps285"  // replace with valid value
                //    };
                //    smtp.Credentials = credential;
                //    smtp.Host = "smtp-mail.outlook.com";
                //    smtp.Port = 587;
                //    smtp.EnableSsl = true;
                //    await smtp.SendMailAsync(mail);
                //    return RedirectToAction("Sent");
                return View("Index");


            }

            else
            {
                return View("Index");
            }
        }
    }
}
