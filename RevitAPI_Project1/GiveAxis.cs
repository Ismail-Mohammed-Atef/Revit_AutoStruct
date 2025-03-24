using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Reflection;

namespace RevitAPI_Project1
{
    [Transaction(TransactionMode.Manual)]
    public class GiveAxis : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the active document
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            try
            {


                // Get all grids in the document
                // Start a transaction to create dimensions
                using (Transaction trans = new Transaction(doc, "Automate Dimensions"))
                {
                    trans.Start();


                    Reference reference = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                    Grid ele = doc.GetElement(reference) as Grid;

                    XYZ point = ele.Curve.GetEndPoint(0);

                    ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_Grids);
                    ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);

                    LogicalOrFilter filter = new LogicalOrFilter(filter1, filter2);

                    ReferenceIntersector referenceIntersector1 = new ReferenceIntersector(filter1, FindReferenceTarget.Element, (View3D)doc.ActiveView);
                    ReferenceIntersector referenceIntersector2 = new ReferenceIntersector(filter2, FindReferenceTarget.Face, (View3D)doc.ActiveView);

                    List<ReferenceWithContext> referenceWithContext = referenceIntersector1.Find(point, new XYZ(0, 0, 1)).ToList();
                    foreach (ReferenceWithContext context in referenceWithContext)
                    {

                    Reference referencee = context.GetReference();
                    XYZ topPoint = reference.GlobalPoint;
                    // Reference reference1=   doc.GetElement(topPoint);
                    //doc.Create.NewDimension(uiDoc.ActiveView,)
                    }



                    trans.Commit();
                }

                return Result.Succeeded;
            }catch (Exception ex)
            {
                return Result.Failed;
            }
        }

        // Helper method to find the closest face of an element to a grid
        //private Face GetClosestFaceToGrid(FamilyInstance element, Grid grid)
        //{
        //    Options options = new Options();

        //    options.ComputeReferences = true;
        //    GeometryElement geomElem = element.get_Geometry(options);

        //    if (geomElem == null)
        //    {
        //        TaskDialog.Show("Error", "No geometry found for the element.");
        //        return null;
        //    }

        //    // Get the grid curve and its direction
        //    Curve gridCurve = grid.Curve;
        //    XYZ gridDirection = (gridCurve.GetEndPoint(1) - gridCurve.GetEndPoint(0)).Normalize();

        //    Face closestFace = null;
        //    double minDistance = double.MaxValue;

        //    // Iterate through all geometry objects
        ////    foreach (GeometryObject geomObj in geomElem)
        ////    {
        ////        if (geomObj is GeometryInstance geometryInstance)
        ////        {
        ////            // Get the transformed geometry of the instance
        ////            GeometryElement instanceGeometry = geometryInstance.GetInstanceGeometry();
        ////            if (instanceGeometry == null) continue;

        ////            foreach (GeometryObject instObj in instanceGeometry)
        ////            {
        ////                if (instObj is Solid solid && solid.Faces.Size > 0)
        ////                {
        ////                    // Iterate through all faces of the solid
        ////                    foreach (Face face in solid.Faces)
        ////                    {
        ////                        if (face is PlanarFace planarFace)
        ////                        {
        ////                            // Get the face normal
        ////                            XYZ faceNormal = planarFace.FaceNormal;

        ////                            // Check if the face is aligned with the grid direction
        ////                            if (faceNormal.IsAlmostEqualTo(gridDirection) || faceNormal.IsAlmostEqualTo(-gridDirection))
        ////                            {
        ////                                // Calculate the centroid of the face
        ////                                //XYZ faceCentroid = GetFaceCentroid(planarFace);

        ////                                // Calculate the distance from the face centroid to the grid curve
        ////                                //double distance = DistanceFromPointToCurve(faceCentroid, gridCurve);

        ////                                // Update the closest face if this face is closer
        ////                                if (distance < minDistance)
        ////                                {
        ////                                    minDistance = distance;
        ////                                    closestFace = face;
        ////                                }
        ////                            }
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////        }
        ////    }

        //    return closestFace;
        //}

        
    }
}