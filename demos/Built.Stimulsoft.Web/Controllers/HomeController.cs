using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Built.Stimulsoft.Web.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System.Data;

namespace Built.Stimulsoft.Web.Controllers
{
    /// <summary>
    /// https://github.com/stimulsoft/Samples-NET.Core-MVC-CSharp
    /// </summary>
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private StiReport GetReport()
        {
            string reportPath = StiNetCoreHelper.MapPath(this, "Reports/TwoSimpleLists.mrt");
            var report = new StiReport();
            report.Load(reportPath);

            string dataPath = StiNetCoreHelper.MapPath(this, "Reports/Data/Demo.xml");
            var data = new DataSet("Demo");
            data.ReadXml(dataPath);
            report.RegData(data);

            return report;
        }

        public IActionResult PrintPdf()
        {
            StiReport report = this.GetReport();
            return StiNetCoreReportResponse.PrintAsPdf(report);
        }

        public IActionResult PrintHtml()
        {
            StiReport report = this.GetReport();
            return StiNetCoreReportResponse.PrintAsHtml(report);
        }

        public IActionResult ExportPdf()
        {
            StiReport report = this.GetReport();
            return StiNetCoreReportResponse.ResponseAsPdf(report);
        }

        public IActionResult ExportHtml()
        {
            StiReport report = this.GetReport();
            return StiNetCoreReportResponse.ResponseAsHtml(report);
        }

        public ActionResult ExportPng()
        {
            StiReport report = this.GetReport();
            return StiNetCoreReportResponse.ResponseAsPng(report);
        }

        public ActionResult ExportBmp()
        {
            StiReport report = this.GetReport();
            return StiNetCoreReportResponse.ResponseAsBmp(report);
        }

        public IActionResult ExportXls()
        {
            StiReport report = this.GetReport();
            return StiNetCoreReportResponse.ResponseAsXls(report);
        }
    }
}