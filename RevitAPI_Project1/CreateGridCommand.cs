using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace RevitAPI_Project1
{
    [Transaction(TransactionMode.Manual)]
    public class CreateGridCommand : IExternalCommand
    {
        public static List<Grid> horizontalGrids { get; set; } = new List<Grid>();
        public static List<Grid> verticalGrids { get; set; } = new List<Grid>();
        public static double DistanceBetweenHorizontalGrid = 4000/ 304.8;
        public static double DistanceBetweenVerticalGrid = 6000 / 304.8;
        public static int NumberOfHorizontalGrid = 5;
        public static int NumberOfVerticalGrid = 5;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;

            try
            {
                

               
                XYZ originalstart = new XYZ(0, 0, 0);
               

                using (Transaction transaction = new Transaction(Doc, "Create Grid"))
                {
                    transaction.Start();

                   
                    #region Create Grid horizontal (in Y Direction)
                    for (int i = 0; i < NumberOfHorizontalGrid; i++)
                    {
                        XYZ startPoint = originalstart + new XYZ(-20, i * DistanceBetweenHorizontalGrid, 0);
                        XYZ endPoint = startPoint + new XYZ(((NumberOfVerticalGrid - 1) * DistanceBetweenVerticalGrid)+40, 0, 0);

                        Line GL = Line.CreateBound(startPoint, endPoint);
                        Grid grid = Grid.Create(Doc, GL);
                        horizontalGrids.Add(grid);
                    }
                    #endregion

                    #region Create Grid Vertical (in X Direction)
                    for (int i = 0; i < NumberOfVerticalGrid; i++)
                    {
                        XYZ startPoint = originalstart + new XYZ(i * DistanceBetweenVerticalGrid, -20, 0);
                        XYZ endPoint = startPoint + new XYZ(0, ((NumberOfHorizontalGrid - 1) * DistanceBetweenHorizontalGrid)+40, 0);

                        Line GL = Line.CreateBound(startPoint, endPoint);

                        Grid grid = Grid.Create(Doc, GL);
                        verticalGrids.Add(grid);
                    }
                    #endregion

                    transaction.Commit();
                }

                

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}