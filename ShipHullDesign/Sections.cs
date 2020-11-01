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
    public struct point
    {
        public double x;
        public double y;
    }
    
 
    public sealed class Sections
    {
       private static volatile Sections instance;
       private static object syncRoot = new Object();

       private StreamReader m_reader;
       private TextWriter LOG;
       private Dictionary<string, List<point>> m_pointHullSections = new Dictionary<string, List<point>>();
       private Dictionary<string, List<Point3D>> m_point3DHullSections = new Dictionary<string, List<Point3D>>();
       private Dictionary<string, List<point>> m_pointHullSectionsByDraft = new Dictionary<string, List<point>>();
       private Dictionary<string, List<Point3D>> m_point3DHullSectionsByTWL = new Dictionary<string, List<Point3D>>();
       private Dictionary<string, Polyline> m_polylineHullSections = new Dictionary<string, Polyline>();
       private Dictionary<string, Polyline> m_polylineHullSectionsByDraft = new Dictionary<string, Polyline>();
       private Dictionary<string, Polygon> m_polygonHullSections = new Dictionary<string, Polygon>();
       private Dictionary<string, double> m_hullSectionsAreas = new Dictionary<string, double>();
       private Dictionary<string, double> m_hullSectionsAreasByDraft = new Dictionary<string, double>();
       private Polyline m_SectionalAreasCurve = new Polyline();
       private float m_shipDraft = 0;
       
       private double m_DisplacementByDraft = 0;

       private double H; //tmp
       private int m_middleSectionNum = 0;
       private double m_middleSectionHeightOfBoard = 0;
       private double m_middleSectionWidthOfShip = 0;

       private string m_DataFilesPath;
       
       private Sections() 
       {
           //Open Configuration Dialog;
           ConfigDlg dbConfigDialog = new ConfigDlg();
           dbConfigDialog.ShowDialog();

           m_DataFilesPath = Config.Instance.getDataFilesFolderPath();
        
           //initSections_old();
           //const string framesFile = "MaxSurfFrames_OUT.txt";
          
           string filename = Constants.fileLogSections;
           //string logFile = m_DataFilesPath + "log_sections.txt";
           string logFile = m_DataFilesPath + filename;
           //m_reader = new StreamReader(framesFile);
           LOG = new StreamWriter(logFile);

           //initHullSections();
           initHullSections3D();
           initHullSections();
       }

       public static Sections Instance
       {
          get 
          {
             if (instance == null) 
             {
                lock (syncRoot) 
                {
                   if (instance == null)
                       instance = new Sections();
                }
             }

             return instance;
          }
       }


 
        List<List<point>> m_HullSectionsTry = new  List<List<point>>();

        public bool getHullSections(ref List<List<point>> hullSections)
        {
            hullSections = m_HullSectionsTry;
            return true;
        }

        public bool getPointHullSections(ref Dictionary<string, List<point>> hullSections)
        {
            hullSections = m_pointHullSections;
            return true;            
        }

        public bool getPoint3DHullSections(ref Dictionary<string, List<Point3D>> hullSections3D)
        {
            hullSections3D = m_point3DHullSections;
            return true;
        }

        public bool getPoint3DHullSectionsByTWL(ref Dictionary<string, List<Point3D>> hullSections3D)
        {
            hullSections3D = m_point3DHullSectionsByTWL;
            return true;
        }      

        public void getPoints3DBySecNum(ref List<Point3D> sectionPoints3D, int i_SecNum)
        {
            int count = 0;
            foreach (KeyValuePair<string, List<Point3D>> section in m_point3DHullSections)
            {
                if (count == i_SecNum)
                {
                    sectionPoints3D = section.Value;
                    break;
                }
                count++;
            }
        }

        public void getPoints3DBySecNumByTWL(ref List<Point3D> sectionPoints3D, int i_SecNum)
        {
            int count = 0;
            foreach (KeyValuePair<string, List<Point3D>> section in m_point3DHullSectionsByTWL)
            {
                if (count == i_SecNum)
                {
                    sectionPoints3D = section.Value;
                    break;
                }
                count++;
            }
        }


        bool initSections_old()
        {
            m_HullSectionsTry.Clear();
            const string file = "Sections_in.txt";    
           // List<point> Section = new List<point>();

            using (StreamReader r = new StreamReader(file))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    List<point> Section = new List<point>();
                    if (!parseLineGetSection(line, ref Section))
                          return false;
                    m_HullSectionsTry.Add(Section);
                }
            }
            printHullSections();
            return true;
        }

        bool initHullSections_save()
        {
            m_pointHullSections.Clear();
            string line;
            List<point> frame = null;
            string [] str;
            string frameName;
            string framePoints;
            string[] strArray;
            point node;

            //const string file = "MaxSurfFrames_OUT.txt";  
            string filename = Constants.filePointsSections;
            //string file = m_DataFilesPath + "MaxSurfFrames_OUT.txt";
            string file = m_DataFilesPath + filename;        

            //const string file = "Test_AreaCalc.txt";            
            StreamReader r = new StreamReader(file);
            int ScaleX = 1;// 30;
            int ScaleY = 1; // -30;
            int CoordYMove = 0;// 230;
            int CoordXMove = 0; // 100;
            
            while ((line = r.ReadLine()) != null)
            {
                frame = new List<point>();
                str = line.Split(':');
                frameName = str[0];
                framePoints = str[1].Trim();
                framePoints = framePoints.Trim(';');
                framePoints = framePoints.TrimStart('0');
                framePoints = framePoints.TrimStart(',');
                framePoints = framePoints.TrimStart('0');
                framePoints = framePoints.TrimStart(';');
                strArray = framePoints.Split(';');
                foreach (string pair in strArray)
                {
                    str = pair.Split(',');
                    node.x = (Convert.ToDouble(str[0]) * ScaleX) + CoordXMove;
                    node.y = (Convert.ToDouble(str[1]) * ScaleY) + CoordYMove;
                    frame.Add(node);
                }
                m_pointHullSections.Add(frameName, frame);
            }

            foreach (KeyValuePair<string, List<point>> l_section in m_pointHullSections)
            {
                if (l_section.Value.First().y < l_section.Value.Last().y)
                {
                    l_section.Value.Reverse();
                }
            }

            createSectionsFromPoints();

            return true;
        }

//--------------------------------------------------------------------------------
        void initHullSections()
        {
            m_pointHullSections.Clear();

            point node;
            foreach (KeyValuePair<string, List<Point3D>> l_section in m_point3DHullSections)
            {
                List<point> frame = new List<point>(); 
                foreach (Point3D l_point3D in l_section.Value)
                {
                    node.x = l_point3D.X;
                    node.y = l_point3D.Y;
                    frame.Add(node);
                }
               m_pointHullSections.Add(l_section.Key, frame);
            }
            createSectionsFromPoints();

            initMiddleSectionNum();
            initMiddleSectionHeihtOfBoard();
            initMiddleSectionWidthOfShip();
        }

        //--------------------------------------------------------------------------------

        bool initHullSections3D()
        {
            m_point3DHullSections.Clear();
            string line;
            List<Point3D> frame = null;
            string[] str;
            string frameName;
            string framePoints;
            string[] strArray;
            Point3D node = new Point3D();
        
 
            //const string file = "MaxSurfFrames_OUT_3D.txt";
            //const string file = "MaxSurfPatrolBoat_Out.txt_ST.txt";

            string filename = Constants.filePointsST;
            //string file = m_DataFilesPath + "MaxSurfPatrolBoat_Out.txt_ST.txt";
            string file = m_DataFilesPath + filename;
            

            StreamReader r = new StreamReader(file);  
            
            /*
            // DataBase connection -- working! Uncomment when continue implement DB functionality
            string sFileString = DBConnect.Instance.getSectionsFileString("Ship2");
            StringReader r = new StringReader(sFileString);
             */
 
            while ((line = r.ReadLine()) != null)
            {
                frame = new List<Point3D>();
                str = line.Split(':');
                frameName = str[0];
                framePoints = str[1].Trim();
                framePoints = framePoints.Trim(';');
                framePoints = framePoints.TrimStart('0');
                framePoints = framePoints.TrimStart(',');
                framePoints = framePoints.TrimStart('0');
                framePoints = framePoints.TrimStart(',');
                framePoints = framePoints.TrimStart('0');
                framePoints = framePoints.TrimStart(';');
                strArray = framePoints.Split(';');
                foreach (string point_3D in strArray)
                {
                    str = point_3D.Split(',');
                    node.X = Convert.ToDouble(str[0]);
                    node.Y = Convert.ToDouble(str[1]);
                    node.Z = Convert.ToDouble(str[2]);
                    frame.Add(node);
                }
                m_point3DHullSections.Add(frameName, frame);
            }

            foreach (KeyValuePair<string, List<Point3D>> l_section in m_point3DHullSections)
            {
                if (l_section.Value.First().Y < l_section.Value.Last().Y)
                {
                    l_section.Value.Reverse();
                }
            }

            //createSectionsFromPoints3D();
            rebuildSectionsByTheoreticalWaterLiens();

            return true;
        }
        //--------------------------------------------------------------------------------



        public bool drawHullFrames(ref Canvas framesCanvas)
        {
            Polyline l_polyLine;
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                l_polyLine = new Polyline();
                l_polyLine.Stroke = System.Windows.Media.Brushes.Black;

                foreach (point node in frame.Value)
                {
                    l_polyLine.Points.Add(new Point(node.x, node.y));
                }
                l_polyLine.StrokeThickness = 0.5;
                framesCanvas.Children.Add(l_polyLine);

            }

            //PathFigure myPathFigure = new PathFigure();
            //myPathFigure.StartPoint = new Point(40, 10);
            //myPathFigure.Segments.Add(
            //    new BezierSegment(
            //        new Point(80, 150),
            //        new Point(130, 200),
            //        new Point(280, 250),
            //        true /* IsStroked */  ));

            //PathGeometry myPathGeometry = new PathGeometry();
            //myPathGeometry.Figures.Add(myPathFigure);

            //// Display the PathGeometry. 
            //System.Windows.Shapes.Path myPath = new System.Windows.Shapes.Path();
            //myPath.Stroke = Brushes.Red;
            //myPath.StrokeThickness = 3;
            //myPath.Data = myPathGeometry;

            //framesCanvas.Children.Add(myPath);


            return true;
        }

        //--------------------------------------------------------------------------------

        bool parseLineGetSection(string i_line, ref List<point> o_Section)
        {
            point node;
            string [] strArray = i_line.Split(';'); //1.1,2;3.3,5.123;
            string[] str;

            foreach (string pair in strArray)
            {
                try
                {
                    str = pair.Split(',');
                    node.x = Convert.ToDouble(str[0]);
                    node.y = Convert.ToDouble(str[1]);
                    o_Section.Add(node);
                }   
                catch (FormatException) {
                    Console.WriteLine("Unable to convert 'to a Double.");
                    return false;
                }               
                catch (OverflowException) {
                    Console.WriteLine("outside the range of a Double.");
                    return false;
                }
            }
            return true;
        }

        void printHullSections()
        {
            //const string file = "Sections_OUT.txt";
            string filename = Constants.filePointsSections;
            //string file = m_DataFilesPath + "Sections_OUT.txt";
            string file = m_DataFilesPath + filename;  

            int i = 0;
            using (TextWriter wr = new StreamWriter(file))
            {
                foreach (List<point> Section in m_HullSectionsTry)
                {
                    wr.Write("Section "+i+":  ");
                    foreach (point node in Section)
                    {
                        wr.Write(node.x + ",");
                        wr.Write(node.y + ";");
                    }
                    wr.WriteLine();
                    i++;
                }
                // close the stream
                wr.Close();
            }
        }

        //public bool drawHullSections(ref Canvas i_sectionsCanvas)
        // {
        //    Polyline line1 = new Polyline();
        //    Polyline line2 = new Polyline();
        //    Polyline line3 = new Polyline();
   
        //    //Grid myGrid = new Grid();

        //    line1.Stroke = System.Windows.Media.Brushes.Black;
        //    line2.Stroke = System.Windows.Media.Brushes.Green;
        //    line3.Stroke = System.Windows.Media.Brushes.Blue;

        //    line1.Points.Add(new Point(10, 10));
        //    line1.Points.Add(new Point(50, 150));
        //    line1.Points.Add(new Point(100, 200));
        //    line1.Points.Add(new Point(250, 250));

        //    line2.BeginInit();

        //    line2.Points.Add(new Point(20, 10));
        //    line2.Points.Add(new Point(60, 150));
        //    line2.Points.Add(new Point(110, 200));
        //    line2.Points.Add(new Point(260, 250));

        //    line3.Points.Add(new Point(30, 10));
        //    line3.Points.Add(new Point(70, 150));
        //    line3.Points.Add(new Point(120, 200));
        //    line3.Points.Add(new Point(270, 250));

        //    line1.StrokeThickness = 2;
        //    i_sectionsCanvas.Children.Add(line1);

        //    line2.StrokeThickness = 2;
        //    i_sectionsCanvas.Children.Add(line2);

        //    line2.StrokeThickness = 2;
        //    i_sectionsCanvas.Children.Add(line3);

        //    PathFigure myPathFigure = new PathFigure();
        //    myPathFigure.StartPoint = new Point(40, 10);
        //    myPathFigure.Segments.Add(
        //        new BezierSegment(
        //            new Point(80, 150),
        //            new Point(130, 200),
        //            new Point(280, 250),
        //            true /* IsStroked */  ));

        //    PathGeometry myPathGeometry = new PathGeometry();
        //    myPathGeometry.Figures.Add(myPathFigure);

        //    // Display the PathGeometry. 
        //    System.Windows.Shapes.Path myPath = new System.Windows.Shapes.Path();
        //    myPath.Stroke = Brushes.Red;
        //    myPath.StrokeThickness = 3;
        //    myPath.Data = myPathGeometry;

        //    i_sectionsCanvas.Children.Add(myPath);

        //    //this.Content = sectionsGrid;

        //    return true;
        //}

        //--------------------------------------------------------------------------------
        
        //--------------------------------------------------------------------------------
        public bool getSectionsAreas()
        {
            calculateSectionsAreas();

            string filename = Constants.fileAreasSections;
            //string sectionsAreasFile = m_DataFilesPath + "sectionsAreas.txt";
            string sectionsAreasFile = m_DataFilesPath + filename;

            int i = 0;
            using (TextWriter wr = new StreamWriter(sectionsAreasFile))
            {
                foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreas)
                {
                    wr.Write("Section: " + sectionArea.Key + ";  Area: " + sectionArea.Value);
                    wr.WriteLine();
                }
                wr.Close();
            }
            return true;
        }

        public bool getSectionsAreasByDraft()
        {
            calculateSectionsAreasByDraft();

            string filename = Constants.fileAreasSectionsByDraft;
            //string sectionsAreasFile = m_DataFilesPath + "sectionsAreasByDraft.txt";
            string sectionsAreasFile = m_DataFilesPath + filename;

            int i = 0;
            using (TextWriter wr = new StreamWriter(sectionsAreasFile))
            {
                foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreasByDraft)
                {
                    wr.Write("Section: " + sectionArea.Key + ";  Area: " + sectionArea.Value);
                    wr.WriteLine();
                }
                wr.Close();
            }
            return true;
        }

      
        //---------------------------------------------------------------------------------

        bool calculateSectionsAreas()
        {
            double l_sectionArea = 0;

            m_hullSectionsAreas.Clear();
            //foreach (KeyValuePair<string, Polygon> frame in m_polygonHullSections)
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                calculateSplineArea(frame.Value, ref l_sectionArea);
                m_hullSectionsAreas.Add(frame.Key, l_sectionArea);
            }

            return true;
        }

        //--------------------------------------------------------------------------------        

        bool calculateSectionsAreasByDraft()
        {
            double l_sectionArea = 0;

             m_hullSectionsAreasByDraft.Clear();
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSectionsByDraft)
            {
                calculateSplineArea(frame.Value, ref l_sectionArea);
                m_hullSectionsAreasByDraft.Add(frame.Key, l_sectionArea);
            }

             return true;
        }
        //--------------------------------------------------------------------------------

        bool calculateSplineArea(List<point> i_section, ref double o_sectionArea)
        {
            //o_sectionArea = 77;

            PathGeometry myPathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            
            Point l_startPoint = new Point();
            if (i_section.Count != 0)
            {
                l_startPoint.X = i_section[0].x;
                l_startPoint.Y = i_section[0].y;
                pathFigure.StartPoint = l_startPoint;

                int flag_OneSideSections = 1;

                foreach (point node in i_section)
                {
                    pathFigure.Segments.Add(new LineSegment(new Point(node.x, node.y), true));
                }

                if (flag_OneSideSections == 1)
                {
                    //add point on (0, Y)
                    pathFigure.Segments.Add(new LineSegment(new Point(0, l_startPoint.Y), true));
                }
                //close path (add start point)
                pathFigure.Segments.Add(new LineSegment(new Point(l_startPoint.X, l_startPoint.Y), true));

                myPathGeometry.Figures.Add(pathFigure);
                o_sectionArea = myPathGeometry.GetArea();

            }
            return true;
        }
        
        //--------------------------------------------------------------------------------

        bool createSectionsFromPoints ()
        {
            Polyline l_polyLine;
            Polygon l_polygon;

            m_polylineHullSections.Clear();
            m_polygonHullSections.Clear();

            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                l_polyLine = new Polyline();
                l_polygon = new Polygon();
                 foreach (point node in frame.Value)
                {
                    l_polyLine.Points.Add(new Point(node.x, node.y));
                    l_polygon.Points.Add(new Point(node.x, node.y));
                }
                m_polylineHullSections.Add(frame.Key, l_polyLine);
                m_polygonHullSections.Add(frame.Key, l_polygon);
            }
            return true;
        }

   

        //--------------------------------------------------------------------------------

        public bool drawHullSectionsPolylinesInScale(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
        {
            Polyline l_polyLine;
            point scaleNode;

            m_polylineHullSections.Clear();
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                l_polyLine = new Polyline();
                foreach (point node in frame.Value)
                {
                    scaleNode.x = node.x * ScaleX + CoordXMove;
                    scaleNode.y = node.y * ScaleY + CoordYMove;
                    l_polyLine.Points.Add(new Point(scaleNode.x, scaleNode.y));
                 }
                m_polylineHullSections.Add(frame.Key, l_polyLine);
             }

            framesCanvas.Children.Clear();
            drawHullSectionsPolylines(ref framesCanvas);

            return true;
        }

        //--------------------------------------------------------------------------------

        public bool drawHullSectionsPolylinesInScaleByDraft(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
        {
            Polyline l_polyLine;
            point scaleNode;

            m_polylineHullSectionsByDraft.Clear();
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSectionsByDraft)
            {
                l_polyLine = new Polyline();
                foreach (point node in frame.Value)
                {
                    scaleNode.x = node.x * ScaleX + CoordXMove;
                    scaleNode.y = node.y * ScaleY + CoordYMove;
                    l_polyLine.Points.Add(new Point(scaleNode.x, scaleNode.y));
                }
                m_polylineHullSectionsByDraft.Add(frame.Key, l_polyLine);
            }

            framesCanvas.Children.Clear();
            drawHullSectionsPolylinesByDraft(ref framesCanvas);

            return true;
        }
        //--------------------------------------------------------------------------------
        public bool setDraft(float i_shipDraft, float i_rollAngle)
        {

            setHullSEctionsByDraft(i_shipDraft, i_rollAngle);
            getSectionsAreasByDraft();
            setSectionalAreasCurveByDraft();
            calculateDisplacementAndXCByDraft();
            m_shipDraft = i_shipDraft;
            return true;
        }

        //--------------------------------------------------------------------------------

        bool setHullSEctionsByDraft(float i_shipDraft, float i_rollAngle)
        {
            Polyline l_polyLine;
            List<point> l_frame = null;
            point waterlineNode;
            point prevNode;
            bool flagFound;
            double l_xDelta;
            
            if (m_pointHullSections.Count() == 0)
                return false;

            waterlineNode.y = i_shipDraft;
            // waterlineNode.x - Need to be found
            prevNode.x = 0;
            prevNode.y = 0;

            m_polylineHullSectionsByDraft.Clear();
            m_pointHullSectionsByDraft.Clear();
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                l_polyLine = new Polyline();
                l_frame = new List<point>();

                flagFound = false;
                foreach (point node in frame.Value)
                {
                    if ((!flagFound) && (prevNode.x != 0) && (node.x != 0) &&
                        ((prevNode.y - waterlineNode.y) * (node.y - waterlineNode.y) < 0))
                    {
                        l_xDelta = ((waterlineNode.y - node.y) * (prevNode.x - node.x)) / (prevNode.y - node.y);
                        waterlineNode.x = node.x + l_xDelta;
                        l_polyLine.Points.Add(new Point(waterlineNode.x, waterlineNode.y));
                        l_frame.Add(waterlineNode);
                        flagFound = true;
                    }
                    if (node.y <= waterlineNode.y)
                    {
                        l_polyLine.Points.Add(new Point(node.x, node.y));
                        l_frame.Add(node);
                    }
                    prevNode.y = node.y;
                    prevNode.x = node.x;
                 }


                m_polylineHullSectionsByDraft.Add(frame.Key, l_polyLine);
                m_pointHullSectionsByDraft.Add(frame.Key, l_frame);
             }

            return true;
        }

        //--------------------------------------------------------------------------------

        public bool drawHullSectionsPolylines(ref Canvas framesCanvas)
        {
            int count = 0;
            Color l_Color;
            foreach (KeyValuePair<string, Polyline> frame in m_polylineHullSections)
            {
                count++;
                switch (count) // for test only
                {
                    case 1:
                        frame.Value.Stroke = System.Windows.Media.Brushes.Blue;
                        break;
                    case 2:
                        frame.Value.Stroke = System.Windows.Media.Brushes.Red;
                        break;
                    case 3:
                        frame.Value.Stroke = System.Windows.Media.Brushes.Green;
                        break;
                    case 56:
                        frame.Value.Stroke = System.Windows.Media.Brushes.Yellow;
                        break;
                    case 52:
                        frame.Value.Stroke = System.Windows.Media.Brushes.Gray;
                        break;
                    default:
                        frame.Value.Stroke = System.Windows.Media.Brushes.Black;
                        break;
                }

                //frame.Value.Stroke = System.Windows.Media.Brushes.Black;
               frame.Value.StrokeThickness = 0.5;
                framesCanvas.Children.Add(frame.Value);
            }
            return true;
        }

        public bool drawHullSectionsPolylinesByDraft(ref Canvas framesCanvas)
        {
            foreach (KeyValuePair<string, Polyline> frame in m_polylineHullSectionsByDraft)
            {
                frame.Value.Stroke = System.Windows.Media.Brushes.Navy;
                frame.Value.StrokeThickness = 1.5;
                framesCanvas.Children.Add(frame.Value);
            }
            return true;
        }

        public bool intersecSEctionsWithtWaterline(ref Canvas framesCanvas)
        {
            SolidColorBrush myBrush = new SolidColorBrush(Colors.Red);
            Pen thinPen = new Pen(myBrush, 0.001);
            Line waterline = new Line();
            waterline.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            waterline.X1 = 1;
            waterline.X2 = 200;
            waterline.Y1 = 4;
            waterline.Y2 = 4;
            waterline.HorizontalAlignment = HorizontalAlignment.Left;
            waterline.VerticalAlignment = VerticalAlignment.Center;
            waterline.StrokeThickness = 2;
            framesCanvas.Children.Add(waterline);

            System.Windows.Shapes.Path myPath = new System.Windows.Shapes.Path();
    
            foreach (KeyValuePair<string, Polyline> frame in m_polylineHullSections)
            {
                //frame.Value.Stroke = System.Windows.Media.Brushes.Black;
                //frame.Value.StrokeThickness = 0.5;
                //framesCanvas.Children.Add(frame.Value);

                CombinedGeometry cg = new CombinedGeometry();
                cg.GeometryCombineMode = GeometryCombineMode.Intersect;

                cg.Geometry1 = frame.Value.RenderedGeometry.GetWidenedPathGeometry(thinPen);
                cg.Geometry2 = waterline.RenderedGeometry.GetWidenedPathGeometry(thinPen);

                myPath.Data = cg;
               framesCanvas.Children.Add(myPath);

               // var combined = Geometry.Combine(cg.Geometry1, cg.Geometry2,
                //            GeometryCombineMode.Intersect, waterline.TransformToVisual()); 
               // var bounds = combined.GetRenderBounds(thinPen);

            }
            return true;
        }

        //-------------------------------------------------------------------------------
        public bool setSectionalAreasCurveByDraft()
        {
            m_SectionalAreasCurve.Points.Clear();

            int l_delta = 10;
            int l_x = 1;

            foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreasByDraft)
            {
                m_SectionalAreasCurve.Points.Add(new Point(l_x, sectionArea.Value));
                l_x += l_delta;
            }
            return true;
        }

        //------------------------------------
        public bool drawSetSectionalAreasCurveByDraft(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
        {
            m_SectionalAreasCurve.Points.Clear();

            int l_delta = 10;
            int l_x = 0;
            double l_y;

           SortedDictionary<int, double> l_sortedDic = new SortedDictionary<int, double>();

           //int count = 0;
            foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreasByDraft)
            {
                
                string s_sectionNum = sectionArea.Key;
                s_sectionNum = s_sectionNum.TrimStart('S');
                s_sectionNum = s_sectionNum.TrimStart('T');
                int l_sectionNum = int.Parse(s_sectionNum);
                l_sortedDic.Add(l_sectionNum, sectionArea.Value);
                //if (count == 2)
                //{
                //    l_sortedDic.Add(l_sectionNum, sectionArea.Value);
                //    count = 0;
                //}
                //count++;
            }
     
            m_SectionalAreasCurve.Points.Add(new Point(0, 0));
            l_x += l_delta;
            //foreach (KeyValuePair<int, double> sectionArea in l_sortedDic)
            foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreasByDraft)
            {
                l_y = (-20) * sectionArea.Value;
                m_SectionalAreasCurve.Points.Add(new Point(l_x, l_y));
                l_x += l_delta;
            }
            m_SectionalAreasCurve.Points.Add(new Point(l_x, 0));

            m_SectionalAreasCurve.Stroke = System.Windows.Media.Brushes.BurlyWood;
            m_SectionalAreasCurve.StrokeThickness = 3;
            framesCanvas.Children.Clear();
            framesCanvas.Children.Add(m_SectionalAreasCurve);

            // Draw section area line
            l_x = l_delta;
            //foreach (KeyValuePair<int, double> sectionArea in l_sortedDic)
            foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreasByDraft)
            {
                Line l_line = new Line();
                l_line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                l_line.X1 = l_x;
                l_line.X2 = l_x;
                l_line.Y1 = 0;
                l_line.Y2 = sectionArea.Value * (-20);
                l_line.StrokeThickness = 2;
                framesCanvas.Children.Add(l_line);
                l_x += l_delta;
            }
            
            return true;
        }

        //------------------------------------
        public bool drawSetSectionalAreasCurve(ref Canvas framesCanvas)
        {
            m_SectionalAreasCurve.Points.Clear();

            int l_delta = 5;
            int l_x = 0;

            SortedDictionary<int, double> l_sortedDic = new SortedDictionary<int, double>();
            Dictionary<int, double> l_nonSortedDic = new Dictionary<int, double>();

            foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreas)
            {

                string s_sectionNum = sectionArea.Key;
                s_sectionNum = s_sectionNum.TrimStart('S');
                s_sectionNum = s_sectionNum.TrimStart('T');
                int l_sectionNum = int.Parse(s_sectionNum);
                if (sectionArea.Value > 0)
                    l_sortedDic.Add(l_sectionNum, sectionArea.Value);
                    l_nonSortedDic.Add(l_sectionNum, sectionArea.Value); // check non-sorted (maybe TMP)
            }

            foreach (KeyValuePair<int, double> sectionArea in l_sortedDic)
           // foreach (KeyValuePair<int, double> sectionArea in l_nonSortedDic)
            {
                m_SectionalAreasCurve.Points.Add(new Point(l_x, sectionArea.Value * (-10)));
                l_x += l_delta;
            }

            m_SectionalAreasCurve.Stroke = System.Windows.Media.Brushes.Red;
            m_SectionalAreasCurve.StrokeThickness = 10;
            framesCanvas.Children.Clear();
            framesCanvas.Children.Add(m_SectionalAreasCurve);

            return true;
        }

        public void refresh()
        {
            initHullSections();
        }
 
        //------- Calculate Displacement and center of Boyancy by Draft ----

        void calculateDisplacementAndXCByDraft()
        {
            double l_DisplacementByDraft = 0;
            double l_shpacia = 0.75; //1 m
            double l_prevSectionArea = 0; //previouse section area
           

            foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreasByDraft)
            {
                l_DisplacementByDraft += (sectionArea.Value + l_prevSectionArea)/2 * l_shpacia;
                l_prevSectionArea = sectionArea.Value;
            }            
            
            m_DisplacementByDraft = l_DisplacementByDraft;
       
            //Center of Bouancy -- X coordinate

            double l_Moment = 0;
            int count = 1;
            double l_arm;
            foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreasByDraft)
            {
                l_arm = l_shpacia * count;
                l_Moment += ((sectionArea.Value + l_prevSectionArea) / 2 * l_shpacia) * l_arm;
                l_prevSectionArea = sectionArea.Value;
                count++;
            }

            double l_XC = l_Moment / l_DisplacementByDraft;
        
        }

        public bool getHullSectionsByDraft(ref Dictionary<string, List<point>> o_pointHullSectionsByDraft, float i_draft)
        {
            List<point> l_frame = null;
            point waterlineNode;
            point prevNode;
            bool flagFound;
            double l_xDelta;

            if (m_pointHullSections.Count() == 0)
                return false;

            waterlineNode.y = i_draft;
            // waterlineNode.x - Need to be found
            prevNode.x = 0;
            prevNode.y = 0;

            
            o_pointHullSectionsByDraft.Clear();
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                l_frame = new List<point>();

                flagFound = false;
                foreach (point node in frame.Value)
                {
                    if ((!flagFound) && (prevNode.x != 0) && (node.x != 0) &&
                        ((prevNode.y - waterlineNode.y) * (node.y - waterlineNode.y) < 0))
                    {
                        l_xDelta = ((waterlineNode.y - node.y) * (prevNode.x - node.x)) / (prevNode.y - node.y);
                        waterlineNode.x = node.x + l_xDelta;
                        l_frame.Add(waterlineNode);
                        flagFound = true;
                    }
                    if (node.y <= waterlineNode.y)
                    {
                        l_frame.Add(node);
                    }
                    prevNode.y = node.y;
                    prevNode.x = node.x;
                }

                o_pointHullSectionsByDraft.Add(frame.Key, l_frame);
            }

            return true;
       
        }

//----------------------------------------------------
        public void rebuildSectionPointsByWLOrdinates(ref List<Point3D> io_sectionPoints, List<double> i_WLOrdinates)
        {

            List<Point3D> sectionPointsByWL = new List<Point3D>();
            double l_xDelta;
            Point3D prevNode = new Point3D(0, 0, 0);

            foreach (double wl_Y in i_WLOrdinates)
            {
                foreach (Point3D node in io_sectionPoints)
                {
                    if ((prevNode.X != 0) && (node.X != 0) &&
                        ((prevNode.Y - wl_Y) * (node.Y - wl_Y) < 0))
                    {
                        Point3D newNode = new Point3D();
                        l_xDelta = ((wl_Y - node.Y) * (prevNode.X - node.X)) / (prevNode.Y - node.Y);
                        newNode.X = node.X + l_xDelta;
                        newNode.Y = wl_Y;
                        newNode.Z = node.Z;
                        sectionPointsByWL.Add(newNode);
                        break;
                    }
                    else if (node.Y == wl_Y)
                    {
                        sectionPointsByWL.Add(node);
                        break;
                    }
                    prevNode = node;
                }
            }
            io_sectionPoints.Clear();
            io_sectionPoints = sectionPointsByWL;

        }

//--------------------------------
        private double getWLDelta()
        {
            //Take Draft as Design Watreline (Konstru
            double T = 4;
            double l_WLDelta = T / 50;

            return l_WLDelta;
        }

        private bool rebuildSectionsByTheoreticalWaterLiens()
        {
            double l_WLDelta = getWLDelta();
            double l_WaterLine = 0;

            Point3D waterlineNode = new Point3D();
            Point3D prevNode = new Point3D();
            double l_xDelta;

            int H = 7;

            if (m_point3DHullSections.Count() == 0)
                return false;

            // waterlineNode.x - Need to be found
            prevNode.X = 0;
            prevNode.Y = 0;
            prevNode.Z = 0;

            m_point3DHullSectionsByTWL.Clear();

            int size = m_point3DHullSections.Count;
            List<Point3D>[] l_SectionByWL = new List<Point3D>[size];
            for (int j = 0; j < size; j++)
            {
                l_SectionByWL[j] = new List<Point3D>();
            }

            int secNumber = 0;
            string newKey;
            bool fl_found = false;
            foreach (KeyValuePair<string, List<Point3D>> frame in m_point3DHullSections)
            {
                waterlineNode.Z = frame.Value[0].Z;
                for (l_WaterLine = 0; l_WaterLine < H; l_WaterLine += l_WLDelta)
                {
                    waterlineNode.Y = l_WaterLine;
                    foreach (Point3D node in frame.Value)
                    {
                        if ((waterlineNode.Y == 0) && (node.Y == 0))
                        {
                            waterlineNode.X = node.X;
                            l_SectionByWL[secNumber].Add(waterlineNode);
                            fl_found = true;
                            break;
                        }
                        else
                        {
                            if ((prevNode.X != 0) && (node.X != 0) &&
                                ((prevNode.Y - waterlineNode.Y) * (node.Y - waterlineNode.Y) < 0))
                            {
                                l_xDelta = ((waterlineNode.Y - node.Y) * (prevNode.X - node.X)) / (prevNode.Y - node.Y);
                                waterlineNode.X = node.X + l_xDelta;
                                l_SectionByWL[secNumber].Add(waterlineNode);
                                fl_found = true;
                            }
                        }
                        prevNode.Y = node.Y;
                        prevNode.X = node.X;
                    }
                }
                newKey = "" + secNumber + ": ";
                addDeckAndKeelPointsToSection(frame.Value, ref l_SectionByWL[secNumber]);
                //l_SectionByWL[secNumber].Add(frame.Value.First());
                m_point3DHullSectionsByTWL.Add(newKey, l_SectionByWL[secNumber]);
                secNumber++;
            }

            return true;
        }
///-----------------------------
        private void addDeckAndKeelPointsToSection(List<Point3D> i_origSection, ref List<Point3D> io_sectionByWL)
       {
           Point3D deckPoint = new Point3D();
           Point3D keelPoint = new Point3D();

           double up_point = 0;
           double low_point = 100;

           foreach (Point3D node in i_origSection)
           {
               if (node.Y > up_point)
               {
                   up_point = node.Y;
                   deckPoint = node;                   
               }
               if (node.Y < low_point)
               {
                   low_point = node.Y;
                   keelPoint = node;
               }
           }

           Point3D first = i_origSection.First();
           Point3D last = i_origSection.Last();
         
           io_sectionByWL.Insert(0, keelPoint);
           io_sectionByWL.Add(deckPoint);
       }

//-----------------------------------------------
// 27 March 2015
        public bool getSectionsAreasByDraft(float i_shipDraft, ref Dictionary<string, double> o_hullSectionsAreasByDraft)
        {
            //1. 
            // setSections by draft and build:
            // polylineHullSectionsByDraft and pointHullSectionsByDraft
            Polyline l_polyLine;
            List<point> l_frame = null;
            point waterlineNode;
            point prevNode;
            bool flagFound;
            double l_xDelta;
            
            if (m_pointHullSections.Count() == 0)
                return false;

            waterlineNode.y = i_shipDraft;
            // waterlineNode.x - Need to be found
            prevNode.x = 0;
            prevNode.y = 0;

            Dictionary<string, Polyline> polylineHullSectionsByDraft = new Dictionary<string, Polyline>();
            Dictionary<string, List<point>> pointHullSectionsByDraft = new Dictionary<string, List<point>>();
            
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                l_polyLine = new Polyline();
                l_frame = new List<point>();

                flagFound = false;
                foreach (point node in frame.Value)
                {
                    if ((!flagFound) && (prevNode.x != 0) && (node.x != 0) &&
                        ((prevNode.y - waterlineNode.y) * (node.y - waterlineNode.y) < 0))
                    {
                        l_xDelta = ((waterlineNode.y - node.y) * (prevNode.x - node.x)) / (prevNode.y - node.y);
                        waterlineNode.x = node.x + l_xDelta;
                        l_polyLine.Points.Add(new Point(waterlineNode.x, waterlineNode.y));
                        l_frame.Add(waterlineNode);
                        flagFound = true;
                    }
                    if (node.y <= waterlineNode.y)
                    {
                        l_polyLine.Points.Add(new Point(node.x, node.y));
                        l_frame.Add(node);
                    }
                    prevNode.y = node.y;
                    prevNode.x = node.x;
                 }


                polylineHullSectionsByDraft.Add(frame.Key, l_polyLine);
                pointHullSectionsByDraft.Add(frame.Key, l_frame);
             }
           
            //2  calculate Sections Areas By Draft
            double l_sectionArea = 0;
            foreach (KeyValuePair<string, List<point>> frame in pointHullSectionsByDraft)
            {
                calculateSplineArea(frame.Value, ref l_sectionArea);
                o_hullSectionsAreasByDraft.Add(frame.Key, l_sectionArea);
            }
                  
            return true;
        }
//-----------------------------------------------
// 13 June 2015
        private bool initMiddleSectionNum()
        {
            int size = m_point3DHullSections.Count;
            int num = 0;
            if (size % 2 == 0)
            {   //even                
                num = size / 2;
            }
            else
            {   //odd
                num = size / 2 + 1;   
            }

            m_middleSectionNum = num;
            return true;
        }
        public int getMiddleSectionNum()
        {
            return m_middleSectionNum;
        }
        
        public bool getMiddleSectionPoints(ref List<Point3D> midSectionPoints3D)
        {
            getPoints3DBySecNum(ref midSectionPoints3D, m_middleSectionNum);
            
            return true;
        }
        public bool initMiddleSectionHeihtOfBoard()
        {
            List<Point3D> midSectionPoints = new List<Point3D>();
            getMiddleSectionPoints(ref midSectionPoints);

            double pointZ = 0;
            foreach (Point3D point in midSectionPoints)
            {
                if (pointZ < point.Z)
                { 
                    pointZ = point.Z;
                }
            }

            m_middleSectionHeightOfBoard = pointZ;
            return true;
        }

// 06 Sept 2015
// ships with Different
        public bool getSectionsAreasByDraftWihDifferent(float i_bowDraft, float i_sternDraft, ref Dictionary<string, double> o_hullSectionsAreasByDraft)
        {
            //1. 
            // setSections by draft and build:
            // polylineHullSectionsByDraft and pointHullSectionsByDraft
            Polyline l_polyLine;
            List<point> l_frame = null;
            point waterlineNode;
            point prevNode;
            bool flagFound;
            double l_xDelta;

            if (m_pointHullSections.Count() == 0)
                return false;

            //waterlineNode.y = i_sternDraft;
            // waterlineNode.x - Need to be found
            prevNode.x = 0;
            prevNode.y = 0;

            Dictionary<string, Polyline> polylineHullSectionsByDraft = new Dictionary<string, Polyline>();
            Dictionary<string, List<point>> pointHullSectionsByDraft = new Dictionary<string, List<point>>();

            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                l_polyLine = new Polyline();
                l_frame = new List<point>();

                float sectionDraft = 0;
                float sectionCoordinateX = (float) frame.Value.First().x;
                getDraftOnSectionByBowAndSternDraft(i_bowDraft, i_sternDraft, sectionCoordinateX, ref sectionDraft);

                waterlineNode.y = sectionDraft;

                flagFound = false;
                foreach (point node in frame.Value)
                {
                    if ((!flagFound) && (prevNode.x != 0) && (node.x != 0) &&
                        ((prevNode.y - waterlineNode.y) * (node.y - waterlineNode.y) < 0))
                    {
                        l_xDelta = ((waterlineNode.y - node.y) * (prevNode.x - node.x)) / (prevNode.y - node.y);
                        waterlineNode.x = node.x + l_xDelta;
                        l_polyLine.Points.Add(new Point(waterlineNode.x, waterlineNode.y));
                        l_frame.Add(waterlineNode);
                        flagFound = true;
                    }
                    if (node.y <= waterlineNode.y)
                    {
                        l_polyLine.Points.Add(new Point(node.x, node.y));
                        l_frame.Add(node);
                    }
                    prevNode.y = node.y;
                    prevNode.x = node.x;
                }


                polylineHullSectionsByDraft.Add(frame.Key, l_polyLine);
                pointHullSectionsByDraft.Add(frame.Key, l_frame);
            }

            //2  calculate Sections Areas By Draft
            double l_sectionArea = 0;
            foreach (KeyValuePair<string, List<point>> frame in pointHullSectionsByDraft)
            {
                calculateSplineArea(frame.Value, ref l_sectionArea);
                o_hullSectionsAreasByDraft.Add(frame.Key, l_sectionArea);
            }

            return true;
        }

        private bool getDraftOnSectionByBowAndSternDraft(   float i_bowDraft,      //draft of first Section
                                                            float i_sternDraft, //draft of last Section
                                                            float i_sectionCoordinateX,
                                                            ref float o_sectionDraft)
        {
            double xFirst = m_pointHullSections.First().Value.First().x;
            double xLast = m_pointHullSections.Last().Value.First().x;

            double lengthFirtLast = xLast - xFirst;
            double l = i_sectionCoordinateX - xFirst;
            double deltaDraft = i_bowDraft - i_sternDraft;

            o_sectionDraft = (float)(deltaDraft * l / lengthFirtLast) + i_sternDraft;
          
            return true;
        }

        private void initMiddleSectionWidthOfShip()
        {
            int count = 1;
            double maxWidth = 0;
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                if (count == m_middleSectionNum)
                {
                    foreach (point node in frame.Value)
                    {
                        if (maxWidth < Math.Abs(node.x))
                        {
                            maxWidth = Math.Abs(node.x);
                        }
                    }
                    break;
                }
                count++;
            }
            m_middleSectionWidthOfShip = maxWidth;
        }

        public double getWidthOfShipOnMiddleSection()
        {
            return m_middleSectionWidthOfShip;
        }

//------------------------------------------------------------------------------------------

        public bool getRollWLPointsBySectionByDraft(ref Dictionary<string, Point> o_RollWLPointsBySectionByDraft, float i_draft)
        {

            point waterlineNode;
            point prevNode;
            bool flagFound;
            double l_xDelta;

            if (m_pointHullSections.Count() == 0)
                return false;

            waterlineNode.y = i_draft;
            // waterlineNode.x - Need to be found
            prevNode.x = 0;
            prevNode.y = 0;


            o_RollWLPointsBySectionByDraft.Clear();
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                Point intersectionPoint = new Point();

                flagFound = false;
                foreach (point node in frame.Value)
                {
                    if ((!flagFound) && (prevNode.x != 0) && (node.x != 0) &&
                        ((prevNode.y - waterlineNode.y) * (node.y - waterlineNode.y) < 0))
                    {
                        l_xDelta = ((waterlineNode.y - node.y) * (prevNode.x - node.x)) / (prevNode.y - node.y);
                        waterlineNode.x = node.x + l_xDelta;
                        intersectionPoint.X = waterlineNode.x;
                        intersectionPoint.Y = waterlineNode.y;
                        flagFound = true;
                    }
                    prevNode.y = node.y;
                    prevNode.x = node.x;
                }

                o_RollWLPointsBySectionByDraft.Add(frame.Key, intersectionPoint);
            }

            return true;

        }
        
        public bool getMiddleSectionPoints2D(ref List<Point> midSectionPoints2D)
        {
            List<Point3D> midSectionPoints3D = new List<Point3D>();
            getPoints3DBySecNum(ref midSectionPoints3D, m_middleSectionNum);

            foreach (Point3D point3D in midSectionPoints3D)
            {
                Point point = new Point();
                point.X = point3D.X;
                point.Y = point3D.Y;
                midSectionPoints2D.Add(point);
            }

            return true;
        }

//------------------------------------------------------------------------------------------

    } //class Sections
} // namespace HullDesign1
