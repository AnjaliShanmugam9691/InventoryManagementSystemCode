using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryManagementSystem.Models;
using Microsoft.Reporting.WebForms;

namespace InventoryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        TestDBEntities testEntities = new TestDBEntities();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User_Details userDetails)
        {
            if (ModelState.IsValid)
            {
                using (TestDBEntities ctx = new TestDBEntities())
                {
                    var res = ctx.User_Details.Where(a => a.UserName_ID.Equals(userDetails.UserName_ID) && a.Password_VC.Equals(userDetails.Password_VC)).FirstOrDefault();
                    if (res != null)
                    {
                        Session["UserId"] = userDetails.UserName_ID;
                        return RedirectToAction("Dashboard");
                    }
                }
            }
            return View(userDetails);
        }

        public ActionResult Dashboard()
        {
            List<AssetData> asset = new List<AssetData>();
            if(Session["UserId"] != null)
            {
                using (TestDBEntities ctx = new TestDBEntities())
                {
                    asset = (from a in ctx.Asset_Details
                            where a.IsActive_BT == true && a.AssetQuantity_VC > 0
                            select new
                            {
                                Asset_Id = a.Asset_ID,
                                Asset_Name = a.AssetDescription_VC,
                                Asset_Value = a.AssetValue_NB
                            }).Distinct().AsEnumerable().Select(a=> new AssetData
                            {
                                Asset_Id = a.Asset_Id,
                                Asset_Name = a.Asset_Name,
                                Asset_Value = a.Asset_Value
                            }).ToList();                        
                }
                return View(asset);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        
        public ActionResult AssetOrder(int assetId, int assetQty)
        {            
            var userId = Session["UserId"].ToString();
            if(userId != null)
            {
                using (DbContextTransaction tran = testEntities.Database.BeginTransaction())
                {
                    try
                    {
                        var qtyAvailable = AvailableQuantity(assetId);
                        if (qtyAvailable >= assetQty)
                        {
                            Order_Details orderDetails = new Order_Details();
                            orderDetails.UserName_ID = Convert.ToInt32(userId);
                            orderDetails.Asset_ID = assetId;
                            orderDetails.QtyOrdered_NB = assetQty;
                            orderDetails.CreatedOn_DT = DateTime.Now;
                            orderDetails.IsActive_BT = true;                            
                            testEntities.Entry(orderDetails).State = EntityState.Added;
                            testEntities.SaveChanges();

                            var assetDetails = testEntities.Asset_Details.Where(a => a.Asset_ID == assetId).Select(a => a).FirstOrDefault();
                            assetDetails.AssetQuantity_VC = (assetDetails.AssetQuantity_VC - assetQty) <= 0 ? 0 : (assetDetails.AssetQuantity_VC - assetQty);
                            assetDetails.UpdatedOn_DT = DateTime.Now;
                            testEntities.Entry(assetDetails).State = EntityState.Modified;
                            testEntities.SaveChanges();

                            tran.Commit();
                        }
                        else
                        {
                            Order_Details orderDetails = new Order_Details();
                            orderDetails.UserName_ID = Convert.ToInt32(userId);
                            orderDetails.Asset_ID = assetId;
                            orderDetails.CreatedOn_DT = DateTime.Now;
                            orderDetails.IsActive_BT = true;
                            orderDetails.ExpDeliveryDate_DT = DateTime.Now.AddMonths(1);
                            testEntities.Entry(orderDetails).State = EntityState.Added;
                            testEntities.SaveChanges();
                            return Content("NoStock");
                        }
                    }
                    catch
                    {
                        tran.Rollback();
                        return Content("Error");
                    }
                }                    
            }
            
            return Content("Success");
        }

        public int AvailableQuantity(int assetId)
        {
            var qty = testEntities.Asset_Details.Where(a => a.Asset_ID == assetId).Select(a => a.AssetQuantity_VC).FirstOrDefault();
            return (int)qty;
        }

        public ActionResult GetReport()
        {
            List<OrderedItems> ordered = new List<OrderedItems>();
            var userId = Session["UserId"].ToString();
            if(userId != null)
            {
                ordered = (from o in testEntities.Order_Details
                           join a in testEntities.Asset_Details on o.Asset_ID equals a.Asset_ID
                           where o.UserName_ID.ToString() == userId.ToString() && o.IsActive_BT == true 
                           && a.IsActive_BT == true && o.OrderDone_BT != true
                           select new OrderedItems
                           {
                               OrderId = o.Order_ID,
                               AssetId = a.Asset_ID,
                               AssetName = a.AssetDescription_VC,
                               OrderedOn = o.CreatedOn_DT,
                               QtyOrdered = o.QtyOrdered_NB
                           }).ToList();
            }
            return View(ordered);
        }

        public ActionResult GenerateInvoice(int orderId)
        {
            ReportViewer reportViewer = new ReportViewer();
            List<ReportParameter> lstRptParams = new List<ReportParameter>();
            lstRptParams.Add(new ReportParameter("OrderId", orderId.ToString()));
            ViewBag.ReportViewer = InventoryManagementSystem.Utility.Utilities.RptViewer(lstRptParams, "Invoice_Report");
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}