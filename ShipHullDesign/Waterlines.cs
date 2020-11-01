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
    public sealed class Waterlines
    {
       private static volatile Waterlines instance;
       private static object syncRoot = new Object();

       private string m_DataFilesPath;
       private TextWriter LOG;
       private Dictionary<string, List<Point3D>> m_point3DHullWaterlines = new Dictionary<string, List<Point3D>>();
       private Dictionary<string, Polyline> m_polylineHullWaterlines = new Dictionary<string, Polyline>();
       private Dictionary<string, double> m_waterlineAreas = new Dictionary<string, double>();

       private Waterlines() 
       {
           m_DataFilesPath = Config.Instance.getDataFilesFolderPath();

           string filename = Constants.fileLogWaterlines;
           //string logFile = m_DataFilesPath + "log_waterlines.txt";
           string logFile = m_DataFilesPath + filename;
           LOG = new StreamWriter(logFile);

           initHullWaterlines();
           calculateWaterlineAreas();
           printWaterlinesAreas();
       }

       public static Waterlines Instance
       {
          get 
          {
             if (instance == null) 
             {
                lock (syncRoot) 
                {
                   if (instance == null)
                       instance = new Waterlines();
                }
             }

             return instance;
          }
       }
       
       //--------------------------------------------------------------------------------
       bool initHullWaterlines()
       {
           m_point3DHullWaterlines.Clear();
           string line;
           List<Point3D> waterline = null;
           string[] str;
           string wlName;
           string wlPoints;
           string[] strArray;
           Point3D node = new Point3D();

           string filename = Constants.filePointsWaterlines3D;
           //string file = m_DataFilesPath + "MaxSurfWaterlines_OUT_3D.txt";
           string file = m_DataFilesPath + filename;
           StreamReader r = new StreamReader(file);
           int ScaleX = 1;// 30;
           int ScaleY = 1; // -30;
           int ScaleZ = 1; // -30;
           int CoordYMove = 0;// 230;
           int CoordXMove = 0; // 100;
           int CoordZMove = 0;

           while ((line = r.ReadLine()) != null)
           {
               waterline = new List<Point3D>();
               str = line.Split(':');
               wlName = str[0];
               wlPoints = str[1].Trim();
               wlPoints = wlPoints.Trim(';');
               wlPoints = wlPoints.TrimStart('0');
               wlPoints = wlPoints.TrimStart(',');
               wlPoints = wlPoints.TrimStart('0');
               wlPoints = wlPoints.TrimStart(',');
               wlPoints = wlPoints.TrimStart('0');
               wlPoints = wlPoints.TrimStart(';');
               strArray = wlPoints.Split(';');
               foreach (string point_3D in strArray)
               {
                   str = point_3D.Split(',');
                   node.X = (Convert.ToDouble(str[0]) * ScaleX) + CoordXMove;
                   node.Y = (Convert.ToDouble(str[1]) * ScaleY) + CoordYMove;
                   node.Z = (Convert.ToDouble(str[2]) * ScaleZ) + CoordZMove;
                   waterline.Add(node);
               }
               m_point3DHullWaterlines.Add(wlName, waterline);
           }

           foreach (KeyValuePair<string, List<Point3D>> l_section in m_point3DHullWaterlines)
           {
               if (l_section.Value.First().Y < l_section.Value.Last().Y)
               {
                   l_section.Value.Reverse();
               }
           }

           createWaterlinesFromPoints();

           return true;
       }
       
       //--------------------------------------------------------------------------------

       void createWaterlinesFromPoints()
       {
           Polyline l_polyLine;

           m_polylineHullWaterlines.Clear();
           foreach (KeyValuePair<string, List<Point3D>> frame in m_point3DHullWaterlines)
           {
               l_polyLine = new Polyline();
               foreach (Point3D node in frame.Value)
               {
                   l_polyLine.Points.Add(new Point(node.X, node.Z));
               }
               m_polylineHullWaterlines.Add(frame.Key, l_polyLine);
           }
       }

       //--------------------------------------------------------------------------------

       public void getPointsHullWaterlines(ref Dictionary<string, List<Point3D>> o_hullWaterlines)
       {
           o_hullWaterlines = m_point3DHullWaterlines;
       }

       //--------------------------------------------------------------------------------

       public void getWaterlineAreas(ref Dictionary<string, double> o_waterlineAreas)
       {
           o_waterlineAreas = m_waterlineAreas;
       }

       //---------------------------------------------------------------------------------
        void calculateWaterlineAreas()
        {
            double l_waterlineArea = 0;

            m_waterlineAreas.Clear();
            foreach (KeyValuePair<string, List<Point3D>> waterline in m_point3DHullWaterlines)
            {
                calculateSplineArea(waterline.Value, ref l_waterlineArea);
                m_waterlineAreas.Add(waterline.Key, l_waterlineArea);
            }
        }


        public void getWaterlineAreaByPolyline(Polyline i_waterline, ref double o_wlArea)
        {
            double l_waterlineArea = 0;
            List<Point3D> waterline = new List<Point3D>();
            //convert Polyline  to List<Point3D>
            foreach (Point point in i_waterline.Points)
            {
                Point3D point3D = new Point3D();
                point3D.X = point.X;
                point3D.Z = point.Y;
                waterline.Add(point3D);
            }

            calculateSplineArea(waterline, ref l_waterlineArea);
            o_wlArea = l_waterlineArea;
        }

        //--------------------------------------------------------------------------------
        public void calculateSplineArea(List<Point3D> i_waterline, ref double o_waterlineArea)
        {
            PathGeometry myPathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point l_startPoint = new Point();

            int flag_OneSide = 1;

            Point3D currentNode = new Point3D();

            if (i_waterline.Count != 0)
            {
                //ermovinig 0,0,0 if it's a first elem
                if ((i_waterline[0].X == 0) && (i_waterline[0].Y == 0) && (i_waterline[0].Z == 0))
                {
                    i_waterline.Skip(1);
                }
                l_startPoint.X = i_waterline[0].Z;
                l_startPoint.Y = i_waterline[0].X;
                pathFigure.StartPoint = l_startPoint;

                //Starting from Stern
                foreach (Point3D node in i_waterline)
                {
                    pathFigure.Segments.Add(new LineSegment(new Point(node.Z, node.X), true));
                    currentNode = node;
                }

               if ( flag_OneSide == 1)
                {
                    //add point on (0, strart_point.Y)
                    pathFigure.Segments.Add(new LineSegment(new Point(l_startPoint.X, 0), true));
                }
 
                 //close path - add start point
                pathFigure.Segments.Add(new LineSegment(new Point(l_startPoint.X, l_startPoint.Y), true));

                myPathGeometry.Figures.Add(pathFigure);
                o_waterlineArea = myPathGeometry.GetArea();
            }
        }

        //--------------------------------------------------------------------------------
        public void printWaterlinesAreas()
        {
            string filename = Constants.fileAreasWaterlines;
            //string wlAreasFile = m_DataFilesPath + "waterlinesAreas.txt";
            string wlAreasFile = m_DataFilesPath + filename;

            int i = 0;
            using (TextWriter wr = new StreamWriter(wlAreasFile))
            {
                foreach (KeyValuePair<string, double> waterlineArea in m_waterlineAreas)
                {
                    wr.Write(waterlineArea.Key + " area: " + waterlineArea.Value);
                    wr.WriteLine();
                }
                wr.Close();
            }
        }

        //--------------------------------------------------------------------------------
        public void getWaterlineByDraftCreateFromSections(float i_Draft, ref Polyline o_waterLine)
        {
            Polyline l_polyLine;
            float l_draft = i_Draft;
            double l_shpacia = 0.75; //1 m
            int count = 1;
            double ZCoord;
            double XCoord = 0;
            
            Dictionary<string, List<point>> l_pointHullSectionsByDraft = new Dictionary<string, List<point>>();
            //Get Sections by Draft
            Sections.Instance.getHullSectionsByDraft(ref l_pointHullSectionsByDraft, l_draft);
            
            // Extract intersect points of sections with waterline  - 
            // X coordinate for 0 node from each section: section[0].X        
            List<Point3D> l_waterlineByDraft = new List<Point3D>();
            //int frameNodeCount;

            foreach (KeyValuePair<string, List<point>> frame in l_pointHullSectionsByDraft)
            {
                ZCoord = l_shpacia * count;
                if (frame.Value.Count() > 0)
                {
                    XCoord = frame.Value[0].x;
                    Point3D newPoint = new Point3D(XCoord, l_draft, ZCoord);
                    l_waterlineByDraft.Add(newPoint);
                }
                count++;
            }

            l_polyLine = new Polyline();
            foreach (Point3D node in l_waterlineByDraft)
            {
                l_polyLine.Points.Add(new Point(node.Z, node.X));
            }
            
            o_waterLine = l_polyLine;

        }

    }
}
