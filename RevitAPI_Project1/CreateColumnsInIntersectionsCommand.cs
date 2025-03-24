using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_Project1
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class CreateColumnsInIntersectionsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;

            Level level = new FilteredElementCollector(Doc).OfClass(typeof(Level)).Cast<Level>()
                                                           .FirstOrDefault(l => l.Name.Equals("Level 1"));

            FamilySymbol columnType = new FilteredElementCollector(Doc)
                                        .OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>()
                                        .FirstOrDefault(f => f.Name.Contains("450 x 600mm"));

            XYZ originalstart = new XYZ(0,0,0);
            try
            {
                using(Transaction transaction = new Transaction(Doc,"Add Cloumns"))
                {
                    transaction.Start();
                    try
                    {

                    if (!columnType.IsActive)
                        columnType.Activate();
                    }
                    catch
                    {
                        TaskDialog.Show("Not found", "Coulmn type not found , please load the family for type (450 x 600mm)!");
                        return Result.Failed;
                    }

                    for (int i = 0; i < CreateGridCommand.horizontalGrids.Count; i++)
                    {
                        for (int j = 0; j < CreateGridCommand.verticalGrids.Count; j++)
                        {
                            XYZ intersectionPoint = originalstart + new XYZ(j * CreateGridCommand.DistanceBetweenVerticalGrid, i * CreateGridCommand.DistanceBetweenHorizontalGrid, 0);
                            Doc.Create.NewFamilyInstance(intersectionPoint, columnType, level, StructuralType.Column);
                        }
                    }
                    transaction.Commit();
                }

                return Result.Succeeded;
            }catch(Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
