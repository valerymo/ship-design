using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Controls;

namespace HullDesign1
{
    public sealed class Hull3D
    {
        private static volatile Hull3D instance;
        private static object syncRoot = new Object();

        private float m_ScaleX;
        private float m_ScaleY;
        private float m_ScaleZ;

 
       private Hull3D() 
       {
           m_ScaleX = 5;
           m_ScaleY = 5;
           m_ScaleZ = 5;

       }

       public static Hull3D Instance
       {
           get
           {
               if (instance == null)
               {
                   lock (syncRoot)
                   {
                       if (instance == null)
                           instance = new Hull3D();
                   }
               }
               return instance;
           }
       }

       public bool Create3DSections(ref Viewport3D myViewport)
       {
           Dictionary<string, List<point>> l_2DHullSections = new Dictionary<string, List<point>>();

           Sections.Instance.getPointHullSections(ref l_2DHullSections);

           double l_Z = 3;
           int count;
           int i;
           point l_point2DTmp;
                    
          Utilities3D.ClearViewport(ref myViewport);

                          Point3D p1 = new Point3D();
               Point3D p2 = new Point3D();
               Point3D p3 = new Point3D();

            
           foreach (KeyValuePair<string, List<point>> l_section in l_2DHullSections)
           {
               count = l_section.Value.Count;
 
               i = 0;
               point l_prevPoint2D = l_section.Value.First();
               l_section.Value.Skip(1);
               foreach (point l_point2D in l_section.Value)
               {
                   //left side of the ship
                   p1.X = l_prevPoint2D.x / m_ScaleX;
                   if (p1.X < 0) {p1.X = -p1.X;}
                   p1.Y = l_prevPoint2D.y / m_ScaleY -1;           
                   p2.X = (l_prevPoint2D.x + 0.001) / m_ScaleX;
                   if (p2.X < 0) {p2.X = -p2.X;}
                   p2.Y = l_prevPoint2D.y / m_ScaleY -1;                   
                   p3.X = l_point2D.x / m_ScaleX;
                   if (p3.X < 0) { p3.X = -p3.X;}
                   p3.Y = l_point2D.y / m_ScaleY -1;
                   p1.Z = p2.Z = p3.Z = l_Z;
                   Utilities3D.CreateTriangleFace(p1, p2, p3, Colors.Red, true, myViewport);

                   //right side of the ship
                   p1.X = -p1.X;
                   p1.Y = p1.Y;
                   p2.X = -p2.X;
                   p2.Y = p2.Y;
                   p3.X = -p3.X;
                   p3.Y = p3.Y;
                   p1.Z = p2.Z = p3.Z = l_Z;
                    Utilities3D.CreateTriangleFace(p1, p2, p3, Colors.Red, true, myViewport);

                   l_prevPoint2D = l_point2D;
                   l_Z -= 0.01;
               }
               


           }
   
              return true;
       }

       public bool draw3DSections(Viewport3D myViewport)
       {
  
           //Create3DSections(ref myViewport);
           Create3DSections1(ref myViewport);

           return true;
       }

       public bool setScale(float i_3DScale)
       {
           m_ScaleX = i_3DScale;
           m_ScaleY = i_3DScale;
           m_ScaleZ = i_3DScale;
           return true;
       }

       public bool Create3DSections1(ref Viewport3D myViewport)
       {
           Dictionary<string, List<Point3D>> l_3DHullSections = new Dictionary<string, List<Point3D>>();

           Sections.Instance.getPoint3DHullSections(ref l_3DHullSections);

            //int count;
           Utilities3D.ClearViewport(ref myViewport);

           Point3D p1 = new Point3D();
           Point3D p2 = new Point3D();
           Point3D p3 = new Point3D();


           foreach (KeyValuePair<string, List<Point3D>> l_section in l_3DHullSections)
           {
               Point3D l_prevPoint3D = l_section.Value.First();
               l_section.Value.Skip(1);
               foreach (Point3D l_point3D in l_section.Value)
               {
                   //left side of the ship
                   p1.X = l_prevPoint3D.X / m_ScaleX;
                   if (p1.X < 0) { p1.X = -p1.X; }
                   p1.Y = l_prevPoint3D.Y / m_ScaleY - 1;
                   p1.Z = l_prevPoint3D.Z / m_ScaleZ;
                   p2.X = (l_prevPoint3D.X + 0.001) / m_ScaleX;
                   if (p2.X < 0) { p2.X = -p2.X; }
                   p2.Y = l_prevPoint3D.Y / m_ScaleY - 1;
                   //p2.Z = l_prevPoint3D.Z / m_ScaleZ;
                   p3.X = l_point3D.X / m_ScaleX;
                   if (p3.X < 0) { p3.X = -p3.X; }
                   p3.Y = l_point3D.Y / m_ScaleY - 1;
                   p3.Z = p2.Z = p1.Z;
                   Utilities3D.CreateTriangleFace(p1, p2, p3, Colors.Red, true, myViewport);

                   //right side of the ship
                   p1.X = -p1.X;
                   p2.X = -p2.X;
                   p3.X = -p3.X;
                   Utilities3D.CreateTriangleFace(p1, p2, p3, Colors.Red, true, myViewport);

                   l_prevPoint3D = l_point3D;
                 }

           }

           return true;
       }

        //////////// waterlines
       public bool Create3DWaterlines(ref Viewport3D myViewport)
       {
           Dictionary<string, List<Point3D>> l_pointHullWaterlines3D = new Dictionary<string, List<Point3D>>();
           Waterlines.Instance.getPointsHullWaterlines(ref l_pointHullWaterlines3D);

           int count;
           //Utilities3D.ClearViewport(ref myViewport);

           Point3D p1 = new Point3D();
           Point3D p2 = new Point3D();
           Point3D p3 = new Point3D();

           /// wl start 
           foreach (KeyValuePair<string, List<Point3D>> l_waterline3D in l_pointHullWaterlines3D)
           {
               count = l_waterline3D.Value.Count;

               //i = 0;
               Point3D l_prevPoint3D = l_waterline3D.Value.First();

               foreach (Point3D l_point3D in l_waterline3D.Value)
               {
                   p1.X = l_prevPoint3D.X / m_ScaleX;
                   p1.Y = l_prevPoint3D.Y / m_ScaleY - 1;
                   p1.Z = l_prevPoint3D.Z / m_ScaleZ;
                   p2.X = (l_prevPoint3D.X + 0.001) / m_ScaleX;
                   p2.Y = l_prevPoint3D.Y / m_ScaleY - 1;
                   p2.Z = l_prevPoint3D.Z / m_ScaleZ;
                   p3.X = l_point3D.X / m_ScaleX;
                   p3.Y = l_point3D.Y / m_ScaleY - 1;
                   p3.Z = l_point3D.Z / m_ScaleZ;
                   Utilities3D.CreateTriangleFace(p1, p2, p3, Colors.Red, true, myViewport);


                   //right side of the ship
                   p1.X = -p1.X;
                   p2.X = -p2.X;
                   p3.X = -p3.X;
                   
                   Utilities3D.CreateTriangleFace(p1, p2, p3, Colors.Red, true, myViewport);

                   l_prevPoint3D = l_point3D;
               
               }
 
           }
           ///  wl end

           return true;
       }

       public bool draw3DWaterlines(Viewport3D myViewport)
       {

           Create3DWaterlines(ref myViewport);

           return true;
       }

//------------------
       //////////// Edges
       public bool Create3DEdges(ref Viewport3D myViewport)
       {
           Dictionary<string, List<Point3D>> l_pointHullEdges3D = new Dictionary<string, List<Point3D>>();
           Edges.Instance.getPointHullEdges(ref l_pointHullEdges3D);

           int count;
           //Utilities3D.ClearViewport(ref myViewport);

           Point3D p1 = new Point3D();
           Point3D p2 = new Point3D();
           Point3D p3 = new Point3D();

           /// wl start 
           foreach (KeyValuePair<string, List<Point3D>> l_edge3D in l_pointHullEdges3D)
           {
               count = l_edge3D.Value.Count;

               //i = 0;
               Point3D l_prevPoint3D = l_edge3D.Value.First();

               foreach (Point3D l_point3D in l_edge3D.Value)
               {
                   p1.X = l_prevPoint3D.X / m_ScaleX;
                   p1.Y = l_prevPoint3D.Y / m_ScaleY - 1;
                   p1.Z = l_prevPoint3D.Z / m_ScaleZ;
                   p2.X = (l_prevPoint3D.X + 0.001) / m_ScaleX;
                   p2.Y = l_prevPoint3D.Y / m_ScaleY - 1;
                   p2.Z = l_prevPoint3D.Z / m_ScaleZ;
                   p3.X = l_point3D.X / m_ScaleX;
                   p3.Y = l_point3D.Y / m_ScaleY - 1;
                   p3.Z = l_point3D.Z / m_ScaleZ;
                   Utilities3D.CreateTriangleFace(p1, p2, p3, Colors.Red, true, myViewport);


                   //right side of the ship
                   p1.X = -p1.X;
                   p2.X = -p2.X;
                   p3.X = -p3.X;

                   Utilities3D.CreateTriangleFace(p1, p2, p3, Colors.Red, true, myViewport);

                   l_prevPoint3D = l_point3D;

               }

           }
           ///  wl end

           return true;
       }

       public bool draw3DEdges(Viewport3D myViewport)
       {

           Create3DEdges(ref myViewport);

           return true;
       }

       public void drawHull3D(Viewport3D myViewport)
       {
           draw3DSections(myViewport);
           draw3DWaterlines(myViewport);
           draw3DEdges(myViewport);           
       }
 
    } //class Hull3D
}
