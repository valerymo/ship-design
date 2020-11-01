using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;


namespace HullDesign1
{
    public partial class StaticaLargeAngles
    {
        // -------- Singleton definition
        private static volatile StaticaLargeAngles instance;
        private static object syncRoot = new Object();

        private double H;
        private double m_draft;
        private double L;

        public double Draft
        {
            get { return m_draft; }
            set { m_draft = value; }
        }

        Canvas m_canvasRollingWLs = new Canvas();
        Canvas m_canvasArmsOfStaticStability = new Canvas();
        Canvas m_canvasArmsOfDynamicStability = new Canvas();

        private Dictionary<string, List<point>> m_pointHullSections = new Dictionary<string, List<point>>();
        private Dictionary<string, List<Point>> m_pointHullSectionsLeftBoard = new Dictionary<string, List<Point>>();
        private Dictionary<string, List<Point>> m_pointHullSectionsRightBoard = new Dictionary<string, List<Point>>();
        private Dictionary<string, Polyline> m_polylineHullSectionsLeftBoardInScale = new Dictionary<string, Polyline>();
        private Dictionary<string, Polyline> m_polylineHullSectionsRightBoardInScale = new Dictionary<string, Polyline>();

        private Dictionary<string, Line> m_rollLinesWLRight = new Dictionary<string, Line>();
        private Dictionary<string, Line> m_rollLinesWLLeft = new Dictionary<string, Line>();
        Dictionary<string, Dictionary<string, Tuple<double, double>>> m_intersectWLSectionAB = new Dictionary<string, Dictionary<string, Tuple<double, double>>>();

        private Dictionary<int, double> m_metacentricRadiuses = new Dictionary<int, double>();
        private Dictionary<int, double> m_staticStabililtyArms = new Dictionary<int, double>();
        private Dictionary<int, double> m_formStabililtyArms = new Dictionary<int, double>();
        private Dictionary<int, double> m_dynamicStabililtyArms = new Dictionary<int, double>();

        double m_ScaleX = 1;
        double m_ScaleY = 1;
        double m_moveX = 0;
        double m_moveY = 0;


        bool m_shipWithoutRolle = true;

        Line m_lineDP = new Line();
        Line m_lineDraft = new Line();

        private StaticaLargeAngles()
        {
        }

        // -------- Singleton definition
        public static StaticaLargeAngles Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new StaticaLargeAngles();
                    }
                }
                return instance;
            }
        }

        private double m_ZG = 2; //stub
        private double m_ZGMIN = 0;
        private double m_ZGMAX = 0;
        private int m_ZGChangeStep = 0;

        private int m_rollAngleMIN = 0;
        private int m_rollAngleMAX = 0;
        private int m_rollAngleChangeStep = 0;

        public int RollAngleMIN
        {
            get { return m_rollAngleMIN; }
            set { m_rollAngleMIN = value; }
        }
        public int RollAngleMAX
        {
            get { return m_rollAngleMAX; }
            set { m_rollAngleMAX = value; }
        }
        public int RollAngleStep
        {
            get { return m_rollAngleChangeStep; }
            set { m_rollAngleChangeStep = value; }
        }

        private double m_dynamicMoment = 0;
        private double m_overturningMoment = 0;

        private Dictionary<string, List<Point3D>> m_hullSections3D = new Dictionary<string, List<Point3D>>();
        private List<double> m_shipDraftList = new List<double> { 0, 1, 2, 3, 4 };
        private Dictionary<double, Polyline> m_stabilityDiagramByDraft = new Dictionary<double, Polyline>();
        private Dictionary<double, Polyline> m_dynamicStabilityDiagramByDraft = new Dictionary<double, Polyline>();


        //------------------------------------------------------------------
        private void init(  ref Canvas canvasRollingWLs,
                            ref Canvas canvasArmsOfStaticStability,
                            ref Canvas canvasArmsOfDynamicStability)
        {
            m_canvasRollingWLs = canvasRollingWLs;
            m_canvasArmsOfStaticStability = canvasArmsOfStaticStability;
            m_canvasArmsOfDynamicStability = canvasArmsOfDynamicStability;
                        
            Sections.Instance.getPointHullSections(ref m_pointHullSections);
            splitHullSectionsToRightAndLeftSidePointLists();
            H = 7; // stub
            m_draft = 4; // stub
            L = 75; //stub

            //Stubs:
            //m_rollAngle = 0;
            setScales();
        }

        private void setScales()
        {
            m_ScaleX = 20;
            m_ScaleY = -20;
            m_moveX = 150;
            m_moveY = 200;

        }

        //------------------------------------------------------------------

        private void drawSectionsAndRollWL()
        {
            m_canvasRollingWLs.Children.Clear();
            drawHullSectionsPolylinesInScale();
            drawDP();
            drawRollingWLs();
        }

        //------------------------------------------------------------------

        public void processingDynamicStability(ref Canvas canvasRollingWLs,
                                         ref Canvas canvasArmsOfStaticStability,
                                         ref Canvas canvasArmsOfDynamicStability)
        {
            canvasArmsOfDynamicStability.Children.Clear();
            canvasArmsOfStaticStability.Children.Clear();
            
            setRollingWLs();
            drawSectionsAndRollWL();
            calculateIntersectsOfSectionsWithRollingWaterlines();
            calculateMetacentricRadiuses();
            calculateStabililtyArms();

            drawStaticStabililtyDiagram(canvasArmsOfStaticStability);
            drawDynamicStabililtyDiagram(canvasArmsOfDynamicStability);

            double pitchingAmplitudeAngle = 30;
            StaticaLargeAnglesOverturnMoment.Instance.OverturnMoment(pitchingAmplitudeAngle, ref m_staticStabililtyArms, ref canvasArmsOfStaticStability);
        }

        private void drawStaticStabililtyDiagram(Canvas canvas)
        {
            double scaleX = 4;
            double scaleY = -200;
            double move_X = -10;
            double move_Y = 200;
            System.Windows.Media.SolidColorBrush color = System.Windows.Media.Brushes.Navy;
            double thick = 2;
            System.Windows.Media.SolidColorBrush colorCoord = System.Windows.Media.Brushes.Black;
            double thickCoord = 1;

            drawStabililtyArms(m_staticStabililtyArms, canvas,
                                scaleX, scaleY, move_X, move_Y, 
                                color, colorCoord, thick, thickCoord);
        }

        private void drawDynamicStabililtyDiagram(Canvas canvas)
        {
            double scaleX = 4;
            double scaleY = -2;
            double move_X = -10;
            double move_Y = -20;
            canvas.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            System.Windows.Media.SolidColorBrush color = System.Windows.Media.Brushes.Green;
            double thick = 2;
            System.Windows.Media.SolidColorBrush colorCoord = System.Windows.Media.Brushes.Black;
            double thickCoord = 1;

            drawStabililtyArms(m_dynamicStabililtyArms, canvas,
                                scaleX, scaleY, move_X, move_Y, 
                                color, colorCoord, thick, thickCoord);
        }


        public void initialProcessingDynamicStability(ref Canvas canvasRollingWLs,
                                        ref Canvas canvasArmsOfStaticStability,
                                        ref Canvas canvasArmsOfDynamicStability)
        {
            m_rollAngleMIN = 0;
            m_rollAngleMAX = 80;
            m_rollAngleChangeStep = 1;

            init(ref canvasRollingWLs, ref canvasArmsOfStaticStability,
                     ref canvasArmsOfDynamicStability);
 
            processingDynamicStability(ref canvasRollingWLs,
                                         ref canvasArmsOfStaticStability,
                                         ref canvasArmsOfDynamicStability);
        }

        //------------------------------------------------------------------

        private void drawHullSectionsPolylinesInScale()
        {

            m_polylineHullSectionsLeftBoardInScale.Clear();
            m_polylineHullSectionsRightBoardInScale.Clear();

            point scaleNode;

            Canvas framesCanvas = m_canvasRollingWLs;

            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                Polyline l_polyLineLeft = new Polyline();
                Polyline l_polyLineRight = new Polyline();
                foreach (point node in frame.Value)
                {
                    scaleNode.x = node.x * m_ScaleX + m_moveX;
                    scaleNode.y = node.y * m_ScaleY + m_moveY;
                    l_polyLineLeft.Points.Add(new Point(scaleNode.x, scaleNode.y));

                    scaleNode.x = (-1) * node.x * m_ScaleX + m_moveX;
                    l_polyLineRight.Points.Add(new Point(scaleNode.x, scaleNode.y));

                }
                m_polylineHullSectionsLeftBoardInScale.Add(frame.Key, l_polyLineLeft);
                m_polylineHullSectionsRightBoardInScale.Add(frame.Key, l_polyLineRight);
            }

            framesCanvas.Children.Clear();

            //draw
            //Port -- Left side
            foreach (KeyValuePair<string, Polyline> frame in m_polylineHullSectionsLeftBoardInScale)
            {
                frame.Value.Stroke = System.Windows.Media.Brushes.Gray;
                frame.Value.StrokeThickness = 0.5;
                framesCanvas.Children.Add(frame.Value);
            }
            //Starboard -- Right side
            foreach (KeyValuePair<string, Polyline> frame in m_polylineHullSectionsRightBoardInScale)
            {
                frame.Value.Stroke = System.Windows.Media.Brushes.Gray;
                frame.Value.StrokeThickness = 0.5;
                framesCanvas.Children.Add(frame.Value);
            }

        }
        //-----------------------------------------------------------------
        private void drawDP()
        {
            Canvas framesCanvas = m_canvasRollingWLs;
            
            Line DP = new Line();
            DP.X1 = 0 + m_moveX;
            DP.X2 = 0 + m_moveX;
            DP.Y1 = 0 + m_moveY;
            DP.Y2 = H * m_ScaleY + m_moveY; ;
            DP.Stroke = System.Windows.Media.Brushes.Red;
            DP.StrokeThickness = 0.5;
            framesCanvas.Children.Add(DP);

            m_lineDP = DP; // set DB
        }

        public void drawStabililtyArms(Dictionary<int, double> stabililtyArms, Canvas canvas, 
                                        double scaleX, double scaleY, double moveX, double moveY,
                                        System.Windows.Media.SolidColorBrush color,
                                        System.Windows.Media.SolidColorBrush colorCoord,
                                        double thick,
                                        double thickCoord)
        {
            //double scaleX = 4;
            //double scaleY = -0.15;
            //double scaleY = -20;
            //double moveX = -4;
            //double moveY = -20;
            Canvas framesCanvas = canvas;
            
            Polyline polyline = new Polyline();
            Ellipse e = new Ellipse();

            //double initialArm = getInitialStaticStabililtyArm();
            polyline.Points.Add(new Point(0 + moveX,  moveY));
            foreach (KeyValuePair<int, double> node in stabililtyArms)
            {
                  polyline.Points.Add(new Point(node.Key * scaleX + moveX,
                                              node.Value * scaleY + moveY));
            }
            polyline.Stroke = color;
            polyline.StrokeThickness = thick;

            framesCanvas.Children.Add(polyline);
            //Canvas.SetRight(polyline, 1);
            //Canvas.SetTop(polyline, 1);

            drawStabililtyArmsCoordinatesLines(scaleX, scaleY, moveX, moveY,
                                                stabililtyArms.First(), 
                                                stabililtyArms.Last(),
                                                framesCanvas,
                                                colorCoord,
                                                thickCoord);
        }


        private void drawStabililtyArmsCoordinatesLines(                                            
                                            double scaleX, double scaleY,
                                            double moveX, double moveY,
                                            KeyValuePair<int, double> firstPoint,
                                            KeyValuePair<int, double> lastPoint,
                                            Canvas canvas,
                                            System.Windows.Media.SolidColorBrush color,
                                            double thick)
        {
            Canvas framesCanvas = canvas;

            int addX = 20;
            int addY = -150;
 
            Line lineX = new Line();
            lineX.Y1 = 0 + moveY;
            lineX.Y2 = lineX.Y1;
            lineX.X1 = firstPoint.Key + moveX;
            lineX.X2 = lastPoint.Key * scaleX + moveX + addX;
            lineX.Stroke = color;
            lineX.StrokeThickness = thick;
            framesCanvas.Children.Add(lineX);

            Line lineY = new Line();
            lineY.X1 = firstPoint.Key + moveX;
            lineY.X2 = lineY.X1;
            lineY.Y1 = 0 + moveY;
            lineY.Y2 = lineY.Y1  + addY;
            lineY.Stroke = color;
            lineY.StrokeThickness = thick;
            framesCanvas.Children.Add(lineY);
        }

        //------------------------------------------------------------------

        private void setRollingWLs()
        {
            m_rollLinesWLLeft.Clear();
            m_rollLinesWLRight.Clear();

            //get Ship beam (width) on middle section
            double widthOnMiddle = Sections.Instance.getWidthOfShipOnMiddleSection();

            for (double rollAngle = m_rollAngleMIN; rollAngle <= m_rollAngleMAX; rollAngle += m_rollAngleChangeStep)
            {
                string sRollAngle = rollAngle.ToString("F0");
                double radianAngle = (rollAngle / 180) * Math.PI;

                Line draftLineRight = new Line();
                draftLineRight.X1 = 0;
                draftLineRight.Y1 = m_draft;
                draftLineRight.X2 = widthOnMiddle * Math.Cos(radianAngle) + draftLineRight.X1;
                draftLineRight.Y2 = widthOnMiddle * Math.Sin(radianAngle) + draftLineRight.Y1;
                m_rollLinesWLRight.Add(sRollAngle, draftLineRight);

                Line draftLineLeft = new Line();
                draftLineLeft.X1 = 0;
                draftLineLeft.Y1 = m_draft;
                draftLineLeft.X2 = widthOnMiddle * Math.Cos(radianAngle + Math.PI) + draftLineLeft.X1;
                draftLineLeft.Y2 = widthOnMiddle * Math.Sin(radianAngle + Math.PI) + draftLineLeft.Y1;
                m_rollLinesWLLeft.Add(sRollAngle, draftLineLeft);

            }



        }
        //------------------------------------------------------------------
        private void drawRollingWLs()
        {
            Canvas framesCanvas= m_canvasRollingWLs;
            //framesCanvas.Children.Clear();
            //Starboard -- Right side

            foreach (Line l_line in m_rollLinesWLRight.Values)
            {
                Line currentLine = new Line();
                currentLine.X1 = l_line.X1 * m_ScaleX + m_moveX;
                currentLine.Y1 = l_line.Y1 * m_ScaleY + m_moveY;
                currentLine.X2 = l_line.X2 * m_ScaleX + m_moveX;
                currentLine.Y2 = l_line.Y2 * m_ScaleY + m_moveY;



                currentLine.Stroke = System.Windows.Media.Brushes.Navy;
                currentLine.StrokeThickness = 0.5;
                framesCanvas.Children.Add(currentLine);
            }
            
            foreach (Line l_line in m_rollLinesWLLeft.Values)
            {
                Line currentLine = new Line();
                currentLine.X1 = l_line.X1 * m_ScaleX + m_moveX;
                currentLine.Y1 = l_line.Y1 * m_ScaleY + m_moveY;
                currentLine.X2 = l_line.X2 * m_ScaleX + m_moveX;
                currentLine.Y2 = l_line.Y2 * m_ScaleY + m_moveY;



                currentLine.Stroke = System.Windows.Media.Brushes.Navy;
                currentLine.StrokeThickness = 0.5;
                framesCanvas.Children.Add(currentLine);
            }

        }
        //--------------------------------------------------------------------
        private void splitHullSectionsToRightAndLeftSidePointLists()
        {
            foreach (KeyValuePair<string, List<point>> frame in m_pointHullSections)
            {
                List<Point> l_pointsListLeft = new List<Point>();
                List<Point> l_pointsListRight = new List<Point>();

                foreach (point node in frame.Value)
                {
                    l_pointsListLeft.Add(new Point(node.x, node.y));
                    l_pointsListRight.Add(new Point((-1) * node.x, node.y));
                }
                m_pointHullSectionsLeftBoard.Add(frame.Key, l_pointsListLeft);
                m_pointHullSectionsRightBoard.Add(frame.Key, l_pointsListRight);
            }
        }
        //------------------------------------------------------------------

        private void calculateIntersectsOfSectionsWithRollingWaterlines()
        {
            /*            
             * will use nested Dictinary data structure as following to save the intersections points per Rollinf WL/Angle 
             * and Section Dictionary <WL ID, Dictinary<Section ID, pair<Point, Point>> m_intersectAllWLsSections
             *              
             * Loop1:
             * For each WL
             * Get WL Line equation Parameters (A1, B1, C1)   A1x + B1y + C1 = 0
             * Dictinary<Section ID, pair<Point, Point> intersectWLSectionDict = new Dictinary<Section ID, pair<Point, Point>();
             *  
             *  //Loop2:
             *  For each Section
             *      //Right Side
             *      Find two Points on Sections closed to Intersection - Right side
             *         Interpolation of Polilyne as Line between these points (some accuracy lost for easier calculations ...)
             *      Get Section Line segment equation Parameters (A2, B2, C2)   A2x + B2y + C2 = 0
             *      Check if following two line are intersect
             *              A1x + B1y + C1 = 0
             *              A2x + B2y + C2 = 0
             *      IF Lines have intersection point
             *              Find intersection point
             *              Get ai: distence from DP (X0, Y0) to intersection point (X1, Y1)   
             *      
             *      //Left side
             *      Find two Points on Sections closed to Intersection - Left side
             *         Interpolation of Polilyne as Line between these points (some accuracy lost for easier calculations ...)
             *      Get Section Line segment equation Parameters (A2, B2, C2)   A2x + B2y + C2 = 0
             *      Check if following two line are intersect
             *              A1x + B1y + C1 = 0
             *              A2x + B2y + C2 = 0
             *      IF Lines have intersection point
             *              Find intersection point
             *              Get bi: distence from DP (X0, Y0) to intersection point (X2, Y2)
             *       
             *      //If (left side and right side have intersection points
             *      IF (ai != null and bi != null)
             *          Save "ai" and "bi" as pair n data structure: m_intersectAi<Section ID , pair<ai, bi>>  : 
             *              intersectWLSectionDict.add (SectionID, pair<ai, bi>
             *   //End of Loop2 -- loop by Sections for current WL
             *   
             *   Add Dictionary of intersections for current WL for all Sections to Dictionary of Rolling WLs
             *   m_intersectAllWLsSections.Add (WL ID, intersectWLSectionDict);
             *   
             * //End of Loop2
             * Here we will havd m_intersectAllWLsSections  Dictionary that contains values
             * of distances to all intersections points for All Rolling WLs for All Sections
             * 
            */
            m_intersectWLSectionAB.Clear();
            double widthOnMiddle = Sections.Instance.getWidthOfShipOnMiddleSection();

            Dictionary<string, Dictionary<string, Point>> intersectionsNoDifferentRight = new Dictionary<string, Dictionary<string, Point>>();
            Dictionary<string, Dictionary<string, Point>> intersectionsNoDifferentLeft = new Dictionary<string, Dictionary<string, Point>>();

            StaticaLargeAnglesUtils.Instance.getIntersectionsNoDifferent(m_pointHullSectionsRightBoard, m_pointHullSectionsLeftBoard,
                                        m_rollLinesWLRight,m_rollLinesWLLeft,
                                        ref intersectionsNoDifferentRight, widthOnMiddle, "R");
            StaticaLargeAnglesUtils.Instance.getIntersectionsNoDifferent(m_pointHullSectionsRightBoard, m_pointHullSectionsLeftBoard,
                                        m_rollLinesWLRight, m_rollLinesWLLeft,
                                        ref intersectionsNoDifferentLeft, widthOnMiddle, "L");

            Dictionary<string, Point> leftIntSecWL;
            foreach (KeyValuePair<string, Dictionary<string, Point>> rightIntSecWL in intersectionsNoDifferentRight)
            {
               String key = rightIntSecWL.Key;
               key = key.Remove(key.Length-1);
               String key1 = key;
               key += 'L';

               if (!intersectionsNoDifferentLeft.ContainsKey(key))
               {
                    string text = "At left side - missing WL Key: " + rightIntSecWL.Key;
                    Console.WriteLine(text);
                    continue;   
               }
               leftIntSecWL = intersectionsNoDifferentLeft[key];

               Dictionary<string, Tuple<double, double>> intersectDist = new Dictionary<string, Tuple<double, double>>();
               foreach (KeyValuePair<string, Point> frameR in rightIntSecWL.Value)
               {
                   Point intersectPointR = frameR.Value;
                   Point intersectPointL;
                   if (!leftIntSecWL.ContainsKey(frameR.Key))
                   {
                        string text = "At left side - missing Intersect point: " + frameR.Key;
                        Console.WriteLine(text);
                        continue;                      
                   }
                   intersectPointL = leftIntSecWL[frameR.Key];
                   
                   double distIntersectRightFrom0 = Math.Sqrt(Math.Pow((intersectPointR.Y - m_draft), 2) + Math.Pow(intersectPointR.X, 2));
                   double distIntersectLeftFrom0 = Math.Sqrt(Math.Pow((intersectPointL.Y - m_draft), 2) + Math.Pow(intersectPointL.X, 2));
                   
                   Tuple<double,double> distABPair = new Tuple<double,double>(distIntersectRightFrom0, distIntersectLeftFrom0);
                   intersectDist.Add(frameR.Key,distABPair);
               }
                m_intersectWLSectionAB.Add(rightIntSecWL.Key, intersectDist);
            }
        }



        private void calculateMetacentricRadiuses()
        {
            m_metacentricRadiuses.Clear();
            double displacement = 0;
            double integralSum = 0;
            double y3R = 0;
            double y3L = 0;
            double shpacia = Statica.Instance.getShpacia();
            //shpacia = 1.2;

            calculateDisplacementByDraftNoRoll(m_draft, ref displacement);

            foreach (KeyValuePair<string, Dictionary<string, Tuple<double, double>>> wl in  m_intersectWLSectionAB)
            {
                int rollAngle = int.Parse(wl.Key.Remove(wl.Key.Length - 1));
                integralSum = 0; //!!!
                foreach (KeyValuePair<string, Tuple<double, double>> section in wl.Value)
                {
                    y3R = Math.Pow(section.Value.Item1, 3);
                    y3L = Math.Pow(section.Value.Item2, 3);
                    integralSum += (y3R - y3L);
                }
                //Moment Inercii X --  Ix
                double Ix = integralSum * shpacia / 3;
                double momentSWl = getRelativeMomentInerWl(wl);
                double Ixq = Ix - momentSWl;
                double metRadius = Ixq / displacement;
                m_metacentricRadiuses.Add(rollAngle, metRadius);

            }
 
        }

        private double getRelativeMomentInerWl(KeyValuePair<string, Dictionary<string, Tuple<double, double>>> wl)
        {
            if (wl.Key.Equals("0R"))
            {
                return 0;
            }
            double wlArea = getRollingWaterlineArea(wl);
            double yF = getYF(wl.Key);
            double relativeMomentInerWl = wlArea * Math.Pow(yF, 2);
            return relativeMomentInerWl;
        }

        private double getYF(string insRollAngle)
        {
            double yF = 0;
            string sRollAngle  = insRollAngle.Remove(insRollAngle.Length - 1);
            double rollAngle = (double)int.Parse(sRollAngle);
            double radianAngle = (rollAngle / 180) * Math.PI;

            int middleSectionNum = Sections.Instance.getMiddleSectionNum();
            double widthOnMiddle = Sections.Instance.getWidthOfShipOnMiddleSection();
            Line rightLine = new Line ();           
            m_rollLinesWLRight.TryGetValue(sRollAngle, out rightLine);
            double widthRight = (rightLine.Y2 - rightLine.Y1) / Math.Sin(radianAngle);
            yF = Math.Abs(widthRight - widthOnMiddle);

            return yF;
        }

        
        void calculateDisplacementByDraftNoRoll(double i_draft, ref double o_DisplacementByDraft)
        {

            Dictionary<string, double> hullSectionsAreas = new Dictionary<string, double>();
            Sections.Instance.getSectionsAreasByDraft((float)i_draft, ref hullSectionsAreas);


            double l_DisplacementByDraft = 0;
            double l_shpacia = Statica.Instance.getShpacia();
           // l_shpacia = 1.3;
            double l_prevSectionArea = 0; //previouse section area

            foreach (KeyValuePair<string, double> sectionArea in hullSectionsAreas)
            {
                l_DisplacementByDraft += (sectionArea.Value + l_prevSectionArea) / 2 * l_shpacia;
                l_prevSectionArea = sectionArea.Value;
            }

            o_DisplacementByDraft = l_DisplacementByDraft;

        }

        void calculateStabililtyArms()
        {
            m_formStabililtyArms.Clear();
            m_staticStabililtyArms.Clear();
            m_dynamicStabililtyArms.Clear();

            List<double> rSinList = new List<double>();
            List<double> rCosList = new List<double>();
            List<double> yCList = new List<double>();
            List<double> zCDifList = new List<double>();

            List<double> statStabililtyArmList = new List<double>();

            double zC = StaticaLargeAnglesUtils.Instance.getZcByDraft(m_draft, m_shipDraftList);
            double a0 = m_ZG - zC;
            
            double rollAngleChangeStepRad = StaticaLargeAnglesUtils.Instance.ConvertDegreesToRadians(m_rollAngleChangeStep);

            foreach (KeyValuePair<int, double> metacentRadius in m_metacentricRadiuses)
            {
                //metacentRadius.Key - is angle in degrees
                double angleRad = StaticaLargeAnglesUtils.Instance.ConvertDegreesToRadians(metacentRadius.Key);
                double radius = metacentRadius.Value;

                rCosList.Add(radius * Math.Cos(angleRad));
                rSinList.Add(radius * Math.Sin(angleRad));

                double sum1 = StaticaLargeAnglesUtils.Instance.getInegralSumOfListDouble(rCosList);
                double yC = rollAngleChangeStepRad * sum1 / 2;
                yCList.Add(yC);
                
                double sum2 = StaticaLargeAnglesUtils.Instance.getInegralSumOfListDouble(rSinList);
                double zCDif = rollAngleChangeStepRad * sum2 / 2;
                zCDifList.Add(zCDif);

                double formStabililtyArm = yC * Math.Cos(angleRad) + zCDif * Math.Sin(angleRad);
                m_formStabililtyArms.Add(metacentRadius.Key, formStabililtyArm);

                double statStabililtyArm = formStabililtyArm - a0 * Math.Sin(angleRad);
                m_staticStabililtyArms.Add(metacentRadius.Key, statStabililtyArm);
                statStabililtyArmList.Add(statStabililtyArm);

                double sum3 = StaticaLargeAnglesUtils.Instance.getInegralSumOfListDouble(statStabililtyArmList);
                double dynStabililtyArms = m_rollAngleChangeStep * sum3 / 2;
                m_dynamicStabililtyArms.Add(metacentRadius.Key, dynStabililtyArms);
            }
        }



        public double getRollingWaterlineArea(KeyValuePair<string, Dictionary<string, Tuple<double, double>>> wl)
        {
            double wlArea = 0;

            int rollAngle = int.Parse(wl.Key.Remove(wl.Key.Length - 1));
            double radianAngle = ((double)rollAngle / 180) * Math.PI;
            double sin = Math.Sin(radianAngle);

            double yR = 0;
            double yL = 0;
            double width = 0;

            double shpacia = Statica.Instance.getShpacia();
            shpacia = 1.3; //test

            foreach (KeyValuePair<string, Tuple<double, double>> section in wl.Value)
            {
                yR = section.Value.Item1;
                yL = section.Value.Item2;
                width = (yR - yL) / sin;
                wlArea += width * shpacia; // approximately / could be improved
            }        

            return wlArea;
        }



    } //class
} // namespace
