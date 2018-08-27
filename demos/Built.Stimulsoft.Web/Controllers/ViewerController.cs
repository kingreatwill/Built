using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using System.Data;
using Stimulsoft.Report.Web;
using Stimulsoft.Report.Export;

namespace Built.Stimulsoft.Web.Controllers
{
    public class ViewerController : Controller
    {
        static ViewerController()
        {
            //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnO...";
            //Stimulsoft.Base.StiLicense.LoadFromFile("license.key");
            //Stimulsoft.Base.StiLicense.LoadFromStream(stream);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetReport()
        {
            StiReport report = new StiReport();
            report.LoadDocument(StiNetCoreHelper.MapPath(this, "Reports/SimpleList.mdc"));

            return StiNetCoreViewer.GetReportResult(this, report);
        }

        public IActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }

        public IActionResult PrintReport()
        {
            StiReport report = StiNetCoreViewer.GetReportObject(this);

            // Some actions with report when printing

            return StiNetCoreViewer.PrintReportResult(this, report);
        }

        public IActionResult ExportReport()
        {
            StiReport report = StiNetCoreViewer.GetReportObject(this);
            StiRequestParams parameters = StiNetCoreViewer.GetRequestParams(this);

            // Some actions with report when exporting
            report.ReportName = "MyReportName";
            report.ReportAlias = report.ReportName;

            if (parameters.ExportFormat == StiExportFormat.Pdf)
            {
                // Change some export settings when exporting to PDF
                StiPdfExportSettings settings = (StiPdfExportSettings)StiNetCoreViewer.GetExportSettings(this);
                settings.CreatorString = "My Company";

                return StiNetCoreViewer.ExportReportResult(this, report, settings);
            }

            return StiNetCoreViewer.ExportReportResult(this, report);
        }

        public IActionResult EmailReport()
        {
            StiEmailOptions options = StiNetCoreViewer.GetEmailOptions(this);

            options.AddressFrom = "admin@test.com";
            //options.AddressTo = "manager@test.com";
            //options.Subject = "Quarterly Report";
            //options.Body = "Quarterly report on arrival of the goods.";

            options.Host = "smtp.test.com";
            //options.Port = 465;
            options.UserName = "admin@test.com";
            options.Password = "************";

            return StiNetCoreViewer.EmailReportResult(this, options);
        }
    }
}