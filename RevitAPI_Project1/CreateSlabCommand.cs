using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_Project1
{
    [Transaction(TransactionMode.Manual)]
    public class CreateSlabCommand :IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;

            Level level = new FilteredElementCollector(Doc).OfClass(typeof(Level)).Cast<Level>()
                                                           .FirstOrDefault(l => l.Name.Equals("Level 2"));

            FloorType SlabType = new FilteredElementCollector(Doc)
                                        .OfClass(typeof(FloorType)).Cast<FloorType>()
                                        .FirstOrDefault(f => f.Name.Contains("Generic 300mm"));

            XYZ originalstart = new XYZ(0, 0, 0);
            try
            {
                using (Transaction transaction = new Transaction(Doc, "Add Cloumns"))
                {
                    
                    var noOFHzGrid = CreateGridCommand.NumberOfHorizontalGrid-1;
                    var horizontal = noOFHzGrid * CreateGridCommand.DistanceBetweenVerticalGrid;

                    var noOFvcGrid = CreateGridCommand.NumberOfVerticalGrid-1;
                    var vertical = noOFvcGrid * CreateGridCommand.DistanceBetweenHorizontalGrid;

                    Line line1 = Line.CreateBound(originalstart + new XYZ(0,0, 300),originalstart+new XYZ(horizontal,0, 300));
                    Line line4 = Line.CreateBound(originalstart + new XYZ(horizontal, 0, 300),originalstart+new XYZ(horizontal,vertical, 300));
                    Line line2 = Line.CreateBound(originalstart + new XYZ(horizontal, vertical, 300), originalstart + new XYZ(0, vertical, 300));
                    Line line3 = Line.CreateBound(originalstart + new XYZ(0, vertical, 300), originalstart+ new XYZ(0, 0, 300));




                    CurveLoop curvloop = CurveLoop.Create(new List<Curve>()
                    {
                        line1, line4, line2, line3
                    });
                    try
                    {
                    Floor.Create(Doc,new List<CurveLoop>() { curvloop}, SlabType.Id, level.Id);

                    }
                    catch
                    {
                        TaskDialog.Show("Not found", "Floor type not found , please load the family for type (Generic 300mm)!");
                        return Result.Failed;
                    }
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
