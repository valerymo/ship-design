using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Documents;

namespace HullDesign1
{
    public sealed class HullDraft
    {
       private static volatile HullDraft instance;
       private static object syncRoot = new Object();

       private Dictionary<string, List<point>> m_pointImportedHullSections = new Dictionary<string, List<point>>();
       private Dictionary<string, List<point>> m_pointHullSectionsByWL = new Dictionary<string, List<point>>();
       private Dictionary<string, Polyline> m_polylineHullSectionsByWL = new Dictionary<string, Polyline>();
       private Dictionary<string, List<Point3D>> m_pointImportedHullWaterlines3D = new Dictionary<string, List<Point3D>>();
       private Dictionary<string, Polyline> m_polylineHullWaterlines = new Dictionary<string, Polyline>();

       private double L;
       private double B;
       private double T;
       private double H;


       public static HullDraft Instance
       {
           get
           {
               if (instance == null)
               {
                   lock (syncRoot)
                   {
                       if (instance == null)
                           instance = new HullDraft();
                   }
               }
               return instance;
           }
       }

       private HullDraft()
       {
           //initData();
       }

       public void initData()
       {
           Sections.Instance.getPointHullSections(ref m_pointImportedHullSections);
           Waterlines.Instance.getPointsHullWaterlines(ref m_pointImportedHullWaterlines3D);

           L = 75;
           B = 12.2;
           T = 4;
           H = 7;

           rebuildSectionsByTheoreticalWaterLiens();
           setPolylineHullSectionsByWL();
       }

 

       private void checkUpdateImportedSections()
       {

       }

       private double getWLDelta()
       {
           //Take Draft as Design Watreline (Konstru
           double l_WLDelta = T / 50;

           return l_WLDelta;
       }

       private bool rebuildSectionsByTheoreticalWaterLiens()
       {
           //checkUpdateImportedSections();
           double l_WLDelta = getWLDelta();
           double l_WaterLine = 0;

           point waterlineNode;
           point prevNode;
           double l_xDelta;

           if (m_pointImportedHullSections.Count() == 0)
               return false;

           // waterlineNode.x - Need to be found
           prevNode.x = 0;
           prevNode.y = 0;

           m_pointHullSectionsByWL.Clear();

           int size = m_pointImportedHullSections.Count;
           List<point>[] l_SectionByWL = new List<point>[size];
           for (int j = 0; j < size; j++)
           {
               l_SectionByWL[j] = new List<point>();
           }

           int secNumber = 0;
           string newKey;
           bool fl_found = false;
           foreach (KeyValuePair<string, List<point>> frame in m_pointImportedHullSections)
           {
               for (l_WaterLine = 0; l_WaterLine < H; l_WaterLine += l_WLDelta)
               {
                   waterlineNode.y = l_WaterLine;
                   foreach (point node in frame.Value)
                   {                      
                       if ((waterlineNode.y == 0) && (node.y == 0))
                       {
                           waterlineNode.x = node.x;
                           l_SectionByWL[secNumber].Add(waterlineNode);
                           fl_found = true;
                           break;
                       }
                       else
                       {
                           if ((prevNode.x != 0) && (node.x != 0) &&
                               ((prevNode.y - waterlineNode.y) * (node.y - waterlineNode.y) < 0))
                           {
                               l_xDelta = ((waterlineNode.y - node.y) * (prevNode.x - node.x)) / (prevNode.y - node.y);
                               waterlineNode.x = node.x + l_xDelta;
                               l_SectionByWL[secNumber].Add(waterlineNode);
                               fl_found = true;
                               //break;
                           }
                       }
                       prevNode.y = node.y;
                       prevNode.x = node.x;
                   }
               }
              // if (fl_found)
              // {
                   newKey = "" + secNumber + ": ";
                   addDeckAndKeelPointsToSection(frame.Value, ref l_SectionByWL[secNumber]);
                   m_pointHullSectionsByWL.Add(newKey, l_SectionByWL[secNumber]);
              // }
               
               secNumber++;
           }

           return true;
       }

       private void addDeckAndKeelPointsToSection(List<point> i_origSection, ref List<point> io_sectionByWL)
       {
           point deckPoint = new point();
           point keelPoint = new point();

           double up_point = 0;
           double low_point = 100;

           foreach (point node in i_origSection)
           {
               if (node.y > up_point)
               {
                   up_point = node.y;
                   deckPoint.x = node.x;
                   deckPoint.y = node.y;
               }
               if (node.y < low_point)
               {
                   low_point = node.y;
                   keelPoint.x = node.x;
                   keelPoint.y = node.y;
               }
           }
         
           io_sectionByWL.Insert(0, keelPoint);
           io_sectionByWL.Add(deckPoint);
       }

       private void setPolylineHullSectionsByWL()
       {
           Polyline l_polyLine;

           foreach (KeyValuePair<string, List<point>> section in m_pointHullSectionsByWL)
           {
               l_polyLine = new Polyline();

               foreach (point node in section.Value)
               {
                   l_polyLine.Points.Add(new Point(node.x, node.y));
               }

               m_polylineHullSectionsByWL.Add(section.Key, l_polyLine);
           }
       }

       public bool drawHullSectionsPolylines(ref Canvas framesCanvas)
       {
           foreach (KeyValuePair<string, Polyline> frame in m_polylineHullSectionsByWL)
           {
               frame.Value.Stroke = System.Windows.Media.Brushes.Black;
               frame.Value.StrokeThickness = 0.5;
               framesCanvas.Children.Add(frame.Value);
           }
           return true;
       }


       public bool drawHullSectionsPolylinesInScale(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
       {
           Polyline l_polyLine;
           point scaleNode;

           m_polylineHullSectionsByWL.Clear();
           foreach (KeyValuePair<string, List<point>> frame in m_pointHullSectionsByWL)
           //foreach (KeyValuePair<string, List<point>> frame in m_pointImportedHullSections)  
           {
               l_polyLine = new Polyline();
               foreach (point node in frame.Value)
               {
                   scaleNode.x = node.x * ScaleX + CoordXMove;
                   scaleNode.y = node.y * ScaleY + CoordYMove;
                   l_polyLine.Points.Add(new Point(scaleNode.x, scaleNode.y));
               }
               m_polylineHullSectionsByWL.Add(frame.Key, l_polyLine);
           }

           framesCanvas.Children.Clear();
           drawHullSectionsPolylines(ref framesCanvas);

           drawDraftLines(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove);

           return true;
       }

       private void drawDraftLines(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
       {
           double l_WLDelta = getWLDelta()*4;
           double l_WaterLine = 0;

           int xCoord = 8;
           
           for (l_WaterLine = 0; l_WaterLine < H; l_WaterLine += l_WLDelta)
           {
               Line wl = new Line ();
               wl.X1 = xCoord* ScaleX + CoordXMove;
               wl.X2 = -xCoord* ScaleX + CoordXMove;
               wl.Y1 = wl.Y2 = l_WaterLine * ScaleY + CoordYMove;

               wl.Stroke = System.Windows.Media.Brushes.Red;
               wl.StrokeThickness = 0.5;
               framesCanvas.Children.Add(wl);
           }

           Line DP = new Line();
           DP.X1 = 0 + CoordXMove;
           DP.X2 = 0 + CoordXMove;
           DP.Y1 = 0 + CoordYMove;
           DP.Y2 = H * ScaleY + CoordYMove; ;
           DP.Stroke = System.Windows.Media.Brushes.Red;
           DP.StrokeThickness = 0.5;
           framesCanvas.Children.Add(DP);          
       }

       public bool getPointHullSections(ref Dictionary<string, List<point>> hullSections)
       {
           hullSections = m_pointHullSectionsByWL;
           return true;
       }

       public bool drawHullWaterlinesPolylinesInScale(ref Canvas Canvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
       {
           Polyline l_polyLine;
           point scaleNode;

           m_polylineHullWaterlines.Clear();
           foreach (KeyValuePair<string, List<Point3D>> frame in m_pointImportedHullWaterlines3D)  
           {
               l_polyLine = new Polyline();
               foreach (Point3D node in frame.Value)
               {
                   scaleNode.x = node.Z * ScaleX/2 + 0;// CoordXMove;
                   scaleNode.y = node.X * ScaleY/2 + 0;
                   l_polyLine.Points.Add(new Point(scaleNode.x, scaleNode.y));
               }
               m_polylineHullWaterlines.Add(frame.Key, l_polyLine);
           }

           Canvas.Children.Clear();
           drawHullWaterlinesPolylines(ref Canvas);

           //drawDraftLines(ref Canvas, ScaleX, ScaleY, CoordXMove, CoordYMove);          

           return true;
       }

       public bool drawHullWaterlinesPolylines(ref Canvas l_Canvas)
       {
           foreach (KeyValuePair<string, Polyline> waterline in m_polylineHullWaterlines)
           {
               waterline.Value.Stroke = System.Windows.Media.Brushes.Black;
               waterline.Value.StrokeThickness = 0.5;
               l_Canvas.Children.Add(waterline.Value);
           }
           return true;
       }

       public void drawHullDraftInitial(ref Canvas canvas1, ref Canvas canvas2)
       {
           initData();
           drawHullSectionsPolylinesInScale(ref canvas1,50, -50, 300, 230);
           drawHullWaterlinesPolylinesInScale(ref canvas2, 50, -50, 300, 230);
       }

       public void getSectionPointsBySecNum(ref List<point> sectionPoints, int i_SecNum)
       {
           int count = 1;
           foreach (KeyValuePair<string, List<point>> frame in m_pointImportedHullSections)
           {
               if (count == i_SecNum)
               {
                   sectionPoints = frame.Value;
                   break;
               }
               count++;
           }
       }


    }
}
