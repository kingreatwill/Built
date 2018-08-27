@using Stimulsoft.Report.Mvc;
@{
    ViewBag.Title = "设计";
}
@model StockingCloud.PrintingUI.Models.VwSecuritySessionResponse
@Html.Stimulsoft().StiMvcDesigner(new StiMvcDesignerOptions()
{
    Actions =
{
GetReport = "GetReport",
PreviewReport = "PreviewReport",
DesignerEvent = "DesignerEvent",
SaveReport ="SaveReport"
},
    Localization = "~/Localization/zh-CHS.xml",
    Toolbar = new StiMvcDesignerOptions.ToolbarOptions
    {
        ShowLayoutButton = true,
        ShowAboutButton = false,
        ShowInsertButton = true,
        ShowPageButton = true,
        ShowPreviewButton = true,
        ShowSaveButton = true,
        ShowSetupToolboxButton = true
    },
    FileMenu = new StiMvcDesignerOptions.FileMenuOptions
    {
        Visible = false
        //Visible = (Model.PlatformType == (int)StockingCloud.PrintingUI.Enums.Enums.PlatformType.Lingcb),
    },
    Theme = Stimulsoft.Report.Web.StiDesignerTheme.Office2013DarkGrayBlue,
    Dictionary = new StiMvcDesignerOptions.DictionaryOptions
    {
        PermissionDataColumns = Stimulsoft.Report.Web.StiDesignerPermissions.View,
        PermissionVariables = Stimulsoft.Report.Web.StiDesignerPermissions.View,
        PermissionBusinessObjects = Stimulsoft.Report.Web.StiDesignerPermissions.None,
        PermissionDataConnections = Stimulsoft.Report.Web.StiDesignerPermissions.View,
        PermissionDataRelations = Stimulsoft.Report.Web.StiDesignerPermissions.View,
        PermissionDataSources = Stimulsoft.Report.Web.StiDesignerPermissions.View,
        PermissionResources = Stimulsoft.Report.Web.StiDesignerPermissions.View,
        PermissionSqlParameters = Stimulsoft.Report.Web.StiDesignerPermissions.View,
    },
    Server = new StiMvcDesignerOptions.ServerOptions
    {
        RequestTimeout = 120
    }
})



@using Stimulsoft.Report.Mvc;
@using Stimulsoft.Report.Web;
@using System.Web.UI.WebControls;
@{
    ViewBag.Title = "预览";
}
@model StockingCloud.PrintingUI.Models.VwSecuritySessionResponse
@Html.Stimulsoft().StiMvcViewer(new StiMvcViewerOptions()
{
    Actions =
{
GetReport = "GetReport",
ViewerEvent = "ViewerEvent",
DesignReport = "Design"
},
    Localization = "~/Localization/zh-CHS.xml",
    Appearance =
{
BackgroundColor = System.Drawing.Color.FromArgb(0xe8, 0xe8, 0xe8)
},
    Toolbar =
{
DisplayMode = StiToolbarDisplayMode.Simple,
ShowDesignButton = false,
ShowAboutButton=false,
},
    Exports = new StiMvcViewerOptions.ExportOptions
    {
        ShowExportDialog = true,
        ShowExportToCsv = false,
        ShowExportToDbf = false,
        ShowExportToDif = false,
        ShowExportToDocument = false,
        ShowExportToExcel = true,
        ShowExportToExcel2007 = false,
        ShowExportToExcelXml = false,
        ShowExportToHtml = false,
        ShowExportToHtml5 = false,
        ShowExportToImageBmp = false,
        ShowExportToImageGif = false,
        ShowExportToImageJpeg = true,
        ShowExportToImageMetafile = false,
        ShowExportToImagePcx = false,
        ShowExportToImagePng = true,
        ShowExportToImageSvg = true,
        ShowExportToImageSvgz = false,
        ShowExportToImageTiff = false,
        ShowExportToMht = false,
        ShowExportToOpenDocumentCalc = false,
        ShowExportToOpenDocumentWriter = false,
        ShowExportToPdf = true,
        ShowExportToPowerPoint = true,
        ShowExportToRtf = false,
        ShowExportToSylk = false,
        ShowExportToText = false,
        ShowExportToWord2007 = true,
        ShowExportToXml = false,
        ShowExportToXps = false
    },
    Server = new StiMvcViewerOptions.ServerOptions
    {
        RequestTimeout = 120
    }
})