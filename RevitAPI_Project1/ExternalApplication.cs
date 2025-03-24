using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RevitAPI_Project1.Helpers;

namespace RevitAPI_Project1
{
    [Transaction(TransactionMode.Manual)]
    public class ExternalApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {

            application.CreateRibbonTab("IGI Plug-Ins");
            RibbonPanel RibbonPanel1 = application.CreateRibbonPanel("IGI Plug-Ins", "Structural Elements Command");
            String AssemblyPath = Assembly.GetExecutingAssembly().Location;

            #region Create Grid Application

            PushButtonData createGridButton = new PushButtonData("Grid", "  Create Grids  ", AssemblyPath, "RevitAPI_Project1.CreateGridCommand");
            createGridButton.Image = ButtonHelper.GetImageFromResources(Properties.Resources.Grid16);
            createGridButton.LargeImage = ButtonHelper.GetImageFromResources(Properties.Resources.Grid32);
            createGridButton.ToolTip = "This command creates grids in your Revit project.";

            #endregion

            #region Create Columns Application

            PushButtonData createColumnsButton = new PushButtonData("Columns", "  Create Columns  ", AssemblyPath, "RevitAPI_Project1.CreateColumnsInIntersectionsCommand");
            createColumnsButton.Image = ButtonHelper.GetImageFromResources(Properties.Resources.Columns16);
            createColumnsButton.LargeImage = ButtonHelper.GetImageFromResources(Properties.Resources.Columns32);
            createColumnsButton.ToolTip = "This command creates Columns in your Revit project at Grids Intersections.";

            #endregion

            #region Create Foundation

            PushButtonData createFoundationButton = new PushButtonData("Footing", "  Create Isolated Footing  ", AssemblyPath, "RevitAPI_Project1.CreateFoundationICommand");
            createFoundationButton.Image = ButtonHelper.GetImageFromResources(Properties.Resources.Foundation16);
            createFoundationButton.LargeImage = ButtonHelper.GetImageFromResources(Properties.Resources.Foundation32);
            createFoundationButton.ToolTip = "This command creates Isolated Footings in your Revit project at Columns.";

            #endregion

            #region Create Dimensions 

            PushButtonData createDimensionsButton = new PushButtonData("Dimesnion", "  Create Dimensions  ", AssemblyPath, "RevitAPI_Project1.CreateDimensionsCommand");
            createDimensionsButton.Image  = ButtonHelper.GetImageFromResources(Properties.Resources.Dimensions16);
            createDimensionsButton.LargeImage = ButtonHelper.GetImageFromResources(Properties.Resources.Dimensions32);
            createDimensionsButton.ToolTip = "This command creates Dimesions Between axis Horizontal and Vertical.";

            #endregion

            #region Create Floor
            PushButtonData CreateFloorButton = new PushButtonData("Floors", "  Create Floors  ", AssemblyPath, "RevitAPI_Project1.CreateSlabCommand");
            CreateFloorButton.Image = ButtonHelper.GetImageFromResources(Properties.Resources.Columns16);
            CreateFloorButton.LargeImage = ButtonHelper.GetImageFromResources(Properties.Resources.Columns32);
            CreateFloorButton.ToolTip = "This command creates floors in your Revit project at Grids Intersections.";

            #endregion

            RibbonPanel1.AddItem(createGridButton);
            RibbonPanel1.AddItem(createColumnsButton);
            RibbonPanel1.AddItem(createFoundationButton);
            RibbonPanel1.AddItem(createDimensionsButton);
            RibbonPanel1.AddItem(CreateFloorButton);

            return Result.Succeeded;
        }
    }
}
