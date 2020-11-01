using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
 
    public sealed class Edges
    {
       private static volatile Edges instance;
       private static object syncRoot = new Object();

       private TextWriter LOG;
       private Dictionary<string, List<Point3D>> m_point3DHullEdges = new Dictionary<string, List<Point3D>>();
       private Dictionary<string, Polyline> m_polylineHullEdges = new Dictionary<string, Polyline>();
       private List<Point3D> m_pointsForshteven = new List<Point3D>();
       private List<Point3D> m_pointsForshtevenByTWL = new List<Point3D>();
       private Polyline m_polylForshtevenEdge = new Polyline();

       private Edges() 
       {
           const string logFile = "log_Edges.txt";
           LOG = new StreamWriter(logFile);

           initHullEdges();
           initForshtevenEdge();
       }

       public static Edges Instance
       {
          get 
          {
             if (instance == null) 
             {
                lock (syncRoot) 
                {
                   if (instance == null)
                       instance = new Edges();
                }
             }

             return instance;
          }
       }
       
       //--------------------------------------------------------------------------------
       bool initHullEdges()
       {
           m_point3DHullEdges.Clear();
           string line;
           List<Point3D> edge = null;
           string[] str;
           string wlName;
           string edgePoints;
           string[] strArray;
           Point3D node = new Point3D();

           const string file = "MaxSurfEdges_OUT_3D.txt";
           StreamReader r = new StreamReader(file);
           int ScaleX = 1;// 30;
           int ScaleY = 1; // -30;
           int ScaleZ = 1; // -30;
           int CoordYMove = 0;// 230;
           int CoordXMove = 0; // 100;
           int CoordZMove = 0;

           while ((line = r.ReadLine()) != null)
           {
               edge = new List<Point3D>();
               str = line.Split(':');
               wlName = str[0];
               edgePoints = str[1].Trim();
               edgePoints = edgePoints.Trim(';');
               edgePoints = edgePoints.TrimStart('0');
               edgePoints = edgePoints.TrimStart(',');
               edgePoints = edgePoints.TrimStart('0');
               edgePoints = edgePoints.TrimStart(',');
               edgePoints = edgePoints.TrimStart('0');
               edgePoints = edgePoints.TrimStart(';');
               strArray = edgePoints.Split(';');
               foreach (string point_3D in strArray)
               {
 
                   str = point_3D.Split(',');
                   node.X = (Convert.ToDouble(str[0]) * ScaleX) + CoordXMove;
                   node.Y = (Convert.ToDouble(str[1]) * ScaleY) + CoordYMove;
                   node.Z = (Convert.ToDouble(str[2]) * ScaleZ) + CoordZMove;
                   if ((node.X == 0) && (node.Y == 0) && (node.Z == 0))
                        { continue;}
                   edge.Add(node);
               }
               m_point3DHullEdges.Add(wlName, edge);
           }

           foreach (KeyValuePair<string, List<Point3D>> l_section in m_point3DHullEdges)
           {
               if (l_section.Value.First().Y < l_section.Value.Last().Y)
               {
                   l_section.Value.Reverse();
               }
           }

           createEdgesFromPoints();

           return true;
       }
       
       //--------------------------------------------------------------------------------

       bool createEdgesFromPoints()
       {
           Polyline l_polyLine;

           m_polylineHullEdges.Clear();
           foreach (KeyValuePair<string, List<Point3D>> l_edge in m_point3DHullEdges)
           {
               l_polyLine = new Polyline();
               foreach (Point3D node in l_edge.Value)
               {
                   l_polyLine.Points.Add(new Point(node.X, node.Z));
               }
               m_polylineHullEdges.Add(l_edge.Key, l_polyLine);
           }
           return true;
       }

       bool createForshtevenFromPoints()
       {
           Polyline polyLine =  new Polyline();

           foreach (Point3D node in m_pointsForshteven)
           {
               polyLine.Points.Add(new Point(node.Y, node.Z));              
           }

           m_polylForshtevenEdge.Points.Clear();
           m_polylForshtevenEdge = polyLine;

           return true;
       }


       //--------------------------------------------------------------------------------

       public bool getPointHullEdges(ref Dictionary<string, List<Point3D>> hullSections)
       {
           hullSections = m_point3DHullEdges;
           return true;
       }

       //--------------------------------------------------------------------------------

        public bool getForshtevenPoints(ref List<Point3D> o_pointsForshteven)
       {
           o_pointsForshteven = m_pointsForshteven;
           return true;
       }

        //--------------------------------------------------------------------------------

        public bool getForshtevenPointsByTWL(ref List<Point3D> o_pointsForshteven)
        {
            o_pointsForshteven = m_pointsForshtevenByTWL;
            return true;
        }

       //--------------------------------------------------------------------------------

       bool initForshtevenEdge()
       {
           m_pointsForshteven.Clear();
           string line;
           List<Point3D> edge = new List<Point3D>();
           string[] str;
           string[] strArray;
           Point3D node = new Point3D();

           const string file = "Forshteven.TXT";
           StreamReader r = new StreamReader(file);
           int ScaleX = 1;
           int ScaleY = 1; 
           int ScaleZ = 1; 
           int CoordYMove = 0;
           int CoordXMove = 0; 
           int CoordZMove = 0;

           while ((line = r.ReadLine()) != null)
           {
               strArray = line.Split(';');
               foreach (string point_3D in strArray)
               {

                   str = point_3D.Split(',');
                   node.X = (Convert.ToDouble(str[0]) * ScaleX) + CoordXMove;
                   node.Y = (Convert.ToDouble(str[1]) * ScaleY) + CoordYMove;
                   node.Z = (Convert.ToDouble(str[2]) * ScaleZ) + CoordZMove;
                   if ((node.X == 0) && (node.Y == 0) && (node.Z == 0))
                   { continue; }
                   edge.Add(node);
               }
            }
          
           m_pointsForshteven = edge;

           if (m_pointsForshteven.First().Y < m_pointsForshteven.Last().Y)
           {
               m_pointsForshteven.Reverse();
           }

           createForshtevenFromPoints();

           return true;
       }

        //--------------------------------------------------------------------------------
       private bool rebuildForshtevenByTheoreticalWaterLines()
       {

           double T = 4;
           double l_WLDelta = T / 50; // improve it to take T from single point

           double l_WaterLine = 0;

           Point3D waterlineNode = new Point3D();
           Point3D prevNode = new Point3D();
           double l_zDelta;

           int H = 7;

           if (m_pointsForshteven.Count() == 0)
               return false;

           // waterlineNode.x - Need to be found
           prevNode.X = 0;
           prevNode.Y = 0;
           prevNode.Z = 0;

           m_pointsForshtevenByTWL.Clear();

           List<Point3D> l_pointsForshtByTWL = new List<Point3D>();
 
           bool fl_found = false;

           waterlineNode.X = 0;

            for (l_WaterLine = 0; l_WaterLine < H; l_WaterLine += l_WLDelta)
            {
                waterlineNode.Y = l_WaterLine;
                foreach (Point3D node in m_pointsForshteven)
                {
                    if ((waterlineNode.Y == 0) && (node.Y == 0))
                    {
                        waterlineNode.Z = node.Z;
                        l_pointsForshtByTWL.Add(waterlineNode);
                        fl_found = true;
                        break;
                    }
                    else
                    {
                        if ((prevNode.Z != 0) && (node.Z != 0) &&
                            ((prevNode.Y - waterlineNode.Y) * (node.Y - waterlineNode.Y) < 0))
                        {
                            l_zDelta = ((waterlineNode.Y - node.Y) * (prevNode.Z - node.Z)) / (prevNode.Y - node.Y);
                            waterlineNode.Z = node.Z + l_zDelta;
                            l_pointsForshtByTWL.Add(waterlineNode);
                            fl_found = true;
                        }
                    }
                    prevNode.Y = node.Y;
                    prevNode.Z = node.Z;
                }
            }

            m_pointsForshtevenByTWL = l_pointsForshtByTWL;         

           return true;
       }


//----------------------------------------------------------------------------------------
    } 

}

