using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FuNDs.Models;
using Microsoft.AspNet.Identity;

using System.Web.Services;

namespace FuNDs.Controllers
{
    public class CampaignsController : Controller
    {
        private FundRaisersDbContext db = new FundRaisersDbContext();

        public ActionResult Index()
        {
            int x = Convert.ToInt32(Session["userId"]);


            var user = db.FundRaisers.FirstOrDefault(s => s.FundRaisersId == x);

            if (user != null)
            {
                List<Campaign> campaignlist = new List<Campaign>();
                foreach (var campaign in user.Campaigns)
                {
                    campaignlist.Add(campaign);
                }

                return View(campaignlist.ToList());
            }
            else
            {
                ModelState.AddModelError("doesUserExist", "Email Not verified. Please check your email confirmation");
                return View("Index", "Home");
            }

        }



        // GET: Campaigns
        //public ActionResult Index()
        //{
        //    int x = Convert.ToInt32(Session["userId"]);


        //    var user = db.FundRaisers.FirstOrDefault(s => s.FundRaisersId == x);

        //    if (user != null)
        //    {
        //        List<Campaign> campaignlist = new List<Campaign>();
        //        foreach (var campaign in user.Campaigns)
        //        {
        //            campaignlist.Add(campaign);
        //        }

        //        return View(campaignlist.ToList());
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("doesUserExist", "Email Not verified. Please check your email confirmation");
        //        return View("Index", "Home");
        //    }

        //}

        public ActionResult AllCampaigns(string sortOrder, string searchString)
        {
            // public ViewResult Index(string sortOrder, string searchString)

            if (!String.IsNullOrEmpty(searchString))
            {

                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                // ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

                var campaigns = from s in db.Campaigns
                                select s;

                if (!String.IsNullOrEmpty(searchString))
                {
                    campaigns = campaigns.Where(s => s.CampaignTitle.ToUpper().Contains(searchString.ToUpper()));
                                          
                }

                // campaigns = campaigns.Where(s => s.CampaignTitle.Contains(searchString));

                switch (sortOrder)
                {
                    case "Name_desc":
                        campaigns = campaigns.OrderByDescending(s => s.CampaignTitle);
                        break;
                    case "Date":
                        campaigns = campaigns.OrderBy(s => s.StartingDate);
                        break;
                    case "Date_desc":
                       campaigns = campaigns.OrderByDescending(s => s.StartingDate);
                       break;
                    default:
                        campaigns = campaigns.OrderBy(s => s.CampaignTitle);
                        break;
                }
                return View("AllCampaigns", campaigns.ToList());
            }
            //  return View("SearchResults", projects.ToList());
            else
            {

                //foreach (var campaign in user.Campaigns)
                //{
                //    campaignlist.Add(campaign);
                //}

                //return View(campaignlist.ToList());

                List<Campaign> allCampaigns = new List<Campaign>();

                allCampaigns = db.Campaigns.ToList();
                return View(allCampaigns);
            }
        }

        //public ActionResult AllCampaigns(string searchString) {


      
        //    var campaigns = from m in db.Campaigns
        //                 select m;

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        campaigns = campaigns.Where(s => s.CampaignTitle.Contains(searchString));
        //    }

        //    return View(campaigns);
        //}
            
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // GET: Campaigns/Create
        public ActionResult Create()
        {
            ViewBag.FundRaisersId = new SelectList(db.FundRaisers, "ID", "FirstName");
            return View();
        }

        // POST: Campaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CampaignId,CampaignTitle,CampaignDescription,CampaignAmount,startingDate,endingDate,FundRaisersId")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                campaign.FundRaisersId = Convert.ToInt32(Session["userId"]);
                db.Campaigns.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FundRaisersId = new SelectList(db.FundRaisers, "ID", "FirstName", campaign.FundRaisersId);
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            ViewBag.FundRaisersId = new SelectList(db.FundRaisers, "ID", "FirstName", campaign.FundRaisersId);
            return View(campaign);
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CampaignId,CampaignTitle,CampaignDescription,CampaignAmount,startingDate,endingDate,FundRaisersId")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FundRaisersId = new SelectList(db.FundRaisers, "ID", "FirstName", campaign.FundRaisersId);
            return View(campaign);
        }

        // GET: Campaigns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // POST: Campaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            db.Campaigns.Remove(campaign);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
