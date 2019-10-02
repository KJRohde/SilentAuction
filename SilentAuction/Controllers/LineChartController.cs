using Newtonsoft.Json;
using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static SilentAuction.Models.LineChart;

namespace SilentAuction.Controllers
{
    public class LineChartController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: LineChart
        public ActionResult Index(int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            var dataSet = context.Data.Where(d => d.AuctionId == id);
            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (Data point in dataSet)
            {
                dataPoints.Add(new DataPoint(point.Time, point.Money));
            }

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View();
        }
    }
}