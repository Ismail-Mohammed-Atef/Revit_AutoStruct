using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace RevitAPI_Project1
{
    [Transaction(TransactionMode.Manual)]
    internal class CreateFoundationICommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;

            FamilySymbol foundationType = new FilteredElementCollector(Doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>()
                .FirstOrDefault(f => f.Name.Equals("1800 x 1200 x 450mm")); ;

            Level level = new FilteredElementCollector(Doc).OfClass(typeof(Level)).Cast<Level>().FirstOrDefault(l => l.Name.Equals("Level 1"));

            XYZ originalstart = new XYZ();

            try
            {
                using (Transaction transaction = new Transaction(Doc, "Create Foundation"))
                {
                    transaction.Start();
                    try
                    {

                        if (!foundationType.IsActive)
                        {
                            foundationType.Activate();
                        }
                    }
                    catch
                    {
                        TaskDialog.Show("Not found", "Foundation type not found , please load the family for type (1800 x 1200 x 450mm)!");
                        return Result.Failed;
                    }

                    for (int i = 0; i < CreateGridCommand.horizontalGrids.Count; i++)
                    {
                        for (int j = 0; j < CreateGridCommand.verticalGrids.Count; j++)
                        {
                            XYZ intersectionPoint = originalstart + new XYZ(j * CreateGridCommand.DistanceBetweenVerticalGrid, i * CreateGridCommand.DistanceBetweenHorizontalGrid, 0);
                            Doc.Create.NewFamilyInstance(intersectionPoint, foundationType, level, StructuralType.Footing);
                        }
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
