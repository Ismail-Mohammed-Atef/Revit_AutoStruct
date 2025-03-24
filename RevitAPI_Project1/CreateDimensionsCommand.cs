using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_Project1
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreateDimensionsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;
            try
            {

                using (Transaction dimensionTransaction = new Transaction(Doc, "Create Dimensions"))
                {
                    dimensionTransaction.Start();

                    CreateDimensions(Doc, CreateGridCommand.horizontalGrids, isHorizontal: true, CreateGridCommand.DistanceBetweenHorizontalGrid);
                    CreateDimensions(Doc, CreateGridCommand.verticalGrids, isHorizontal: false, CreateGridCommand.DistanceBetweenVerticalGrid);

                    dimensionTransaction.Commit();
                }



                return Result.Succeeded;
            }catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        private void CreateDimensions(Document doc, List<Grid> grids, bool isHorizontal, double gridSpacing)
        {
            // Collect references
            List<Reference> references = new List<Reference>();
            foreach (var grid in grids)
            {
                if (grid.Curve == null)
                {
                    continue; // Skip invalid grids
                }
                references.Add(new Reference(grid));
            }

            if (references.Count < 2)
                return; // Need at least two grids to create dimensions

            // Define offsets for dimension placement
            double totalDimensionOffset = gridSpacing * 0.5; // Offset for total dimension
            double individualDimensionOffset = gridSpacing * 0.2; // Offset for individual dimensions

            // Total dimension offset direction (opposite to individual dimensions)
            XYZ totalOffsetDirection = isHorizontal ? new XYZ(-totalDimensionOffset, 0, 0) : new XYZ(0, -totalDimensionOffset, 0);
            XYZ totalOffsetDirectionEnd = isHorizontal ? new XYZ(totalDimensionOffset, 0, 0) : new XYZ(0, totalDimensionOffset, 0);

            // Individual dimension offset direction (opposite to total dimension)
            XYZ individualOffsetDirection = isHorizontal ? new XYZ(-individualDimensionOffset, 0, 0) : new XYZ(0, -individualDimensionOffset, 0);
            XYZ individualOffsetDirectionEnd = isHorizontal ? new XYZ(individualDimensionOffset, 0, 0) : new XYZ(0, individualDimensionOffset, 0);

            // Total Dimension (First to Last)
            Line totalDimensionLine = Line.CreateBound(
                grids[0].Curve.GetEndPoint(0) + totalOffsetDirection,
                grids[grids.Count - 1].Curve.GetEndPoint(0) + totalOffsetDirection
            );
            Line totalDimensionLineEnd = Line.CreateBound(
                grids[0].Curve.GetEndPoint(1) + totalOffsetDirectionEnd,
                grids[grids.Count - 1].Curve.GetEndPoint(1) + totalOffsetDirectionEnd
            );

            ReferenceArray totalReferenceArray = new ReferenceArray();
            totalReferenceArray.Append(references[0]);
            totalReferenceArray.Append(references[grids.Count - 1]);

            Dimension totalDimension = doc.Create.NewDimension(doc.ActiveView, totalDimensionLine, totalReferenceArray);
            Dimension totalDimensionEnd = doc.Create.NewDimension(doc.ActiveView, totalDimensionLineEnd, totalReferenceArray);

            // Individual Dimensions
            for (int i = 0; i < references.Count - 1; i++)
            {
                Line individualDimensionLine = Line.CreateBound(
                    grids[i].Curve.GetEndPoint(0) + individualOffsetDirection,
                    grids[i + 1].Curve.GetEndPoint(0) + individualOffsetDirection
                );
                Line individualDimensionLineEnd = Line.CreateBound(
                    grids[i].Curve.GetEndPoint(1) + individualOffsetDirectionEnd,
                    grids[i + 1].Curve.GetEndPoint(1) + individualOffsetDirectionEnd
                );

                ReferenceArray individualRefs = new ReferenceArray();
                individualRefs.Append(references[i]);
                individualRefs.Append(references[i + 1]);

                Dimension individualDimension = doc.Create.NewDimension(doc.ActiveView, individualDimensionLine, individualRefs);
                Dimension individualDimensionEnd = doc.Create.NewDimension(doc.ActiveView, individualDimensionLineEnd, individualRefs);

            }
        }
    }
}
