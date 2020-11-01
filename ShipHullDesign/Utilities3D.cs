using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using _3DTools;

namespace HullDesign1
{
    public class Utilities3D
    {

        public static void CreateTriangleFace(
                            Point3D p0, Point3D p1, Point3D p2,
                            Color color, bool isWireframe,
                            Viewport3D viewport)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = color;
            Material material = new DiffuseMaterial(brush);
            GeometryModel3D geometry = new GeometryModel3D(mesh, material);
            ModelUIElement3D model = new ModelUIElement3D();
            model.Model = geometry;
            viewport.Children.Add(model);

            if (isWireframe == true)
            {
                ScreenSpaceLines3D ssl = new ScreenSpaceLines3D();
                ssl.Points.Add(p0);
                ssl.Points.Add(p1);
                ssl.Points.Add(p1);
                ssl.Points.Add(p2);
                ssl.Points.Add(p2);
                ssl.Points.Add(p0);
                ssl.Color = Colors.Navy;
                ssl.Thickness = 1;
                viewport.Children.Add(ssl);
            }
        }


        public static bool  ClearViewport(ref Viewport3D i_viewport)
        {
            //if (i_viewport.Children.Count > 4)
            //{
            ScreenSpaceLines3D ssl;
            for (int i = i_viewport.Children.Count - 1; i >= 0; i--)
            {
                if (i_viewport.Children[i].GetType() == typeof(ScreenSpaceLines3D))
                {
                    ssl = (ScreenSpaceLines3D)i_viewport.Children[i];
                    //ssl.Color = Colors.Transparent;
                    ssl.Color = Colors.White;
                }
            }

            //    for (int j = 0; j < i_viewport.Children.Count; j++)
            //    {
            //        if (i_viewport.Children[j].GetType() == typeof(ModelUIElement3D))
            //        {
            //            ModelUIElement3D oldModel = (ModelUIElement3D)i_viewport.Children[j];
            //            i_viewport.Children.Remove(oldModel);
            //        }
            //        if (i_viewport.Children[j].GetType() == typeof(ScreenSpaceLines3D))
            //        {
            //            ScreenSpaceLines3D sslTmp = (ScreenSpaceLines3D)i_viewport.Children[j];
            //            i_viewport.Children.Remove(sslTmp);
            //        }
            //    }

            //    Viewport3D viewport = new Viewport3D();

            //    MeshGeometry3D mesh = new MeshGeometry3D();
            //    SolidColorBrush brush = new SolidColorBrush();
            //    Material material = new DiffuseMaterial(brush);
            //    GeometryModel3D geometry = new GeometryModel3D(mesh, material);
            //    ModelUIElement3D model = new ModelUIElement3D();
            //    model.Model = geometry;
            //    //DirectionalLight drl = new DirectionalLight();
            //    //drl.Color = Colors.White;
            //    //drl.Direction = new Vector3D(-1, -1, -1);          
            //    viewport.Children.Add(model);

            //    PerspectiveCamera myPCamera = new PerspectiveCamera();
            //    myPCamera.Position = new Point3D(2, 2, 10);
            //    myPCamera.LookDirection = new Vector3D(-2, -2, -10);
            //    myPCamera.UpDirection = new Vector3D(0, 1, 0);
            //    viewport.Camera = myPCamera;

            //    i_viewport = viewport;
            //}
 
             return true;
        }



        //
        public static Matrix3D SetViewMatrix(Point3D cameraPosition, 
            Vector3D lookDirection, Vector3D upDirection)
        {
            // Normalize vectors:
            lookDirection.Normalize();
            upDirection.Normalize();
            // Define vectors, XScale, YScale, and ZScale:

            double denom = Math.Sqrt(1-Math.Pow(Vector3D.DotProduct(lookDirection, upDirection), 2));
            Vector3D XScale = Vector3D.CrossProduct(lookDirection, upDirection) / denom;
            Vector3D YScale = upDirection/denom - ((Vector3D.DotProduct(upDirection, lookDirection)) * lookDirection) / denom;
            Vector3D ZScale = lookDirection;
            
            // Construct M matrix:
            Matrix3D M = new Matrix3D();
            M.M11 = XScale.X;
            M.M21 = XScale.Y;
            M.M31 = XScale.Z;
            M.M12 = YScale.X;
            M.M22 = YScale.Y;
            M.M32 = YScale.Z;
            M.M13 = ZScale.X;
            M.M23 = ZScale.Y;
            M.M33 = ZScale.Z;

            // Translate the camera position to the origin:
            Matrix3D translateMatrix = new Matrix3D();
            translateMatrix.Translate(new Vector3D(-cameraPosition.X, -cameraPosition.Y, -cameraPosition.Z));
            // Define reflect matrix about the Z axis:
            Matrix3D reflectMatrix = new Matrix3D();
            reflectMatrix.M33 = -1;
            // Construct the View matrix:
            Matrix3D viewMatrix = translateMatrix * M * reflectMatrix;
            
            return viewMatrix;
        }
        //

     } //public class Utilities3D
}
