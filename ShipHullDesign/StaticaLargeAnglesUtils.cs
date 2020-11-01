using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace HullDesign1
{
    class StaticaLargeAnglesUtils
    {
        private static volatile StaticaLargeAnglesUtils instance;
        private static object syncRoot = new Object();
        private StaticaLargeAnglesUtils()
        {
        }
        public static StaticaLargeAnglesUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new StaticaLargeAnglesUtils();
                    }
                }
                return instance;
            }
        }

/*-------------------------------------------------------------------------------------*/
        private static bool getIntersectPointOfTwoLines(ref Point intersectPoint, Line Line1, Line Line2)
        {

            double dX1 = Line1.X2 - Line1.X1;
            double dY1 = Line1.Y2 - Line1.Y1;
            double dX2 = Line2.X2 - Line2.X1;
            double dY2 = Line2.Y2 - Line2.Y1;
            if (dY1 * dX2 == dY2 * dX1)
            {
                return false;
            }
            intersectPoint.X = ((Line2.Y1 - Line1.Y1) * dX1 * dX2 + dY1 * dX2 * Line1.X1 - dY2 * dX1 * Line2.X1) / (dY1 * dX2 - dY2 * dX1);
            intersectPoint.Y = Line1.Y1 + (dY1 / dX1) * (intersectPoint.X - Line1.X1);

            return true;
        }

        private bool getIntersectWLWithSection(Line rollWL, List<Point> hullSection, ref Point intersectPoint)
        {
            bool found = false;

            int n = 0;
            Point prevP = hullSection.First();

            foreach (Point point in hullSection)
            {
                if (n == 0)
                {
                    n += 1;
                    continue;
                }

                Line line = newLine(prevP, point);

                if (getIntersectPointOfTwoLines(ref intersectPoint, line, rollWL))
                {
                    found = true;
                    if (checkPointInsideLineSegment(prevP, point, intersectPoint))
                    {
                        break; // Intersection point found
                    }

                }
                prevP = point;
            }

            return found;
        }

        public bool getIntersectWLWithSectionOrDeck(Line rollWL, List<Point> hullSection, ref Point intersectPoint)
        {
            bool found = false;

            Point p1 = hullSection[0];
            Point p2 = hullSection[1];
            Line line = newLine(p1, p2);
            if (getIntersectPointOfTwoLines(ref intersectPoint, line, rollWL))
            {
                if (intersectPoint.Y < hullSection[0].Y)
                {
                    found = getIntersectWLWithSection(rollWL, hullSection, ref intersectPoint);
                }
                else
                {
                    found = getIntersectionWLWithDeck(rollWL, hullSection, ref intersectPoint);
                }
            }

            return found;
        }

        private static Line newLine(Point p1, Point p2)
        {
            Line line = new Line();
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;

            return line;
        }

        private static bool checkPointInsideLineSegment(Point p1, Point p2, Point checkPoint)
        {
            bool res = false;

            if ((p1.X > 0 && p2.X > 0 && checkPoint.X > 0)
                || (p1.X < 0 && p2.X < 0 && checkPoint.X < 0)
                )
            {
                if (Math.Abs(p1.X) >= Math.Abs(p2.X))
                {
                    if ((Math.Abs(checkPoint.X) < Math.Abs(p1.X)) && (Math.Abs(checkPoint.X) > Math.Abs(p2.X)))
                    {
                        res = true;
                    }
                }
                else // like Bulb or new forms
                {
                    if ((Math.Abs(checkPoint.X) > Math.Abs(p1.X)) && (Math.Abs(checkPoint.X) < Math.Abs(p2.X)))
                    {
                        res = true;
                    }
                }
            }

            return res;
        }

        private bool getIntersectionWLWithDeck(Line rollWL, List<Point> sectionPoints, ref Point intersectPoint)
        {
            bool found = false;

            Line deckLine = new Line();
            deckLine.X1 = sectionPoints.First().X;
            deckLine.Y1 = sectionPoints.First().Y;
            deckLine.X2 = -deckLine.X1;
            deckLine.Y2 = deckLine.Y1;

            if (getIntersectPointOfTwoLines(ref intersectPoint, deckLine, rollWL))
            {
                found = true;
            }

            return found;
        }


        private static Point getPointAbsMinX(List<Point> intersectPointsList)
        {
            Point min = new Point();
            min.X = 1000;
            min.Y = 1000;

            foreach (Point point in intersectPointsList)
            {
                if (Math.Abs(min.X) > Math.Abs(point.X))
                {
                    min.X = point.X;
                    min.Y = point.Y;
                }
            }
            return min;
        }

        private static Point getPointAbsMaxX(List<Point> intersectPointsList)
        {
            Point max = new Point();
            max.X = 0;
            max.Y = 0;

            foreach (Point point in intersectPointsList)
            {
                if (Math.Abs(max.X) < Math.Abs(point.X))
                {
                    max.X = point.X;
                    max.Y = point.Y;
                }
            }
            return max;
        }

        private static void getIntersectionWLWithSection(string i_side, KeyValuePair<string, Line> rollWL, ref bool intersectPointFound, ref bool firstNode, ref Point prevNode, ref Point intersectPointMiddle, KeyValuePair<string, List<Point>> hullSection, ref Point intersectPoint)
        {
            intersectPoint.Y = intersectPointMiddle.Y;
            foreach (Point currentNode in hullSection.Value)
            {
                if (firstNode)
                {
                    prevNode.Y = currentNode.Y;
                    prevNode.X = currentNode.X;
                    firstNode = false;
                    continue;
                }
                //Get pair of points where intersectPointMiddle.Y will be inside (else - skip)
                //and find intersection coordinate "X" 
                if ((Math.Abs(intersectPoint.Y) > (Math.Abs(currentNode.Y)))
                    && (Math.Abs(intersectPoint.Y) < (Math.Abs(prevNode.Y))))
                {
                    intersectPointFound = true;
                    intersectPoint.X = Math.Abs(currentNode.X) - ((Math.Abs(currentNode.Y) - Math.Abs(intersectPoint.Y))) * ((Math.Abs(currentNode.X) - Math.Abs(prevNode.X))) / ((Math.Abs(currentNode.Y) - Math.Abs(prevNode.Y)));
                    break;
                }
                else
                {
                    prevNode.Y = currentNode.Y;
                    prevNode.X = currentNode.X;
                }
            }
        }

        public void getRollWLPointsBySectionByDraft(ref Dictionary<string, Point> intersectWLPoints, double draftOnBoard, string side)
        {
            intersectWLPoints.Clear();

            Dictionary<string, Point> intersectWLPointsTmp = new Dictionary<string, Point>();
            Sections.Instance.getRollWLPointsBySectionByDraft(ref intersectWLPointsTmp, (float)draftOnBoard);
            //Sections - return points for Left board only, so for "R" side - need change X = -X
            if (side == "R")
            {
                foreach (KeyValuePair<string, Point> point in intersectWLPointsTmp)
                {
                    Point newPoint = new Point();
                    newPoint.X = (-1) * point.Value.X;
                    newPoint.Y = point.Value.Y;
                    intersectWLPoints.Add(point.Key, newPoint);
                }
            }
            else //"L"
            {
                intersectWLPoints = intersectWLPointsTmp;
            }

        }

        public void getIntersectWLAndDeckPointsBySection(Dictionary<string, List<Point>> pointHullSectionsRightBoard, Line line, Point intersectPointMiddle, ref Dictionary<string, Point> intersectWLPoints)
        {
            //will check right side, as suppose roll on right
            foreach (KeyValuePair<string, List<Point>> hullSection in pointHullSectionsRightBoard)
            {
                if (hullSection.Value.First().X > intersectPointMiddle.X)
                {
                    Point point = new Point();
                    point.Y = intersectPointMiddle.Y;
                    point.X = intersectPointMiddle.X;
                    intersectWLPoints.Add(hullSection.Key, point);
                }
            }
        }

        public void getIntersectionsNoDifferent(   Dictionary<string, List<Point>> pointHullSectionsRightBoard,
                                                    Dictionary<string, List<Point>> pointHullSectionsLeftBoard,
                                                    Dictionary<string, Line> rollLinesWLRight,
                                                    Dictionary<string, Line> rollLinesWLLeft,
                                                    ref Dictionary<string, Dictionary<string, Point>> intersectWLSectionDict,
                                                    double widthOnMiddle,
                                                    string i_side)
        {

            Dictionary<string, Line> rollLinesWL = new Dictionary<string, Line>();
            Dictionary<string, List<Point>> pointHullSections = new Dictionary<string, List<Point>>();
            //Get Points of Middle Section
            List<Point> midSectionPoints = new List<Point>();
            List<Point> midSectionPointsLeft = new List<Point>();
            Sections.Instance.getMiddleSectionPoints2D(ref midSectionPointsLeft);
            midSectionPoints = midSectionPointsLeft;

            if (i_side == "L")
            {
                rollLinesWL = rollLinesWLLeft;
                pointHullSections = pointHullSectionsLeftBoard;
            }
            else if (i_side == "R")
            {
                rollLinesWL = rollLinesWLRight;
                pointHullSections = pointHullSectionsRightBoard;

                List<Point> midSectionPointsRight = new List<Point>();
                foreach (Point node in midSectionPoints)
                {
                    Point point = new Point();
                    point.X = (-1) * node.X;
                    point.Y = node.Y;
                    midSectionPointsRight.Add(point);
                }
                midSectionPoints = midSectionPointsRight;
            }
            else
            {
                //MessageBox.Show("error - L/R parameter missing", "getIntersectionsNoDifferent"); 
                return;
            }


            // For each Waterline
            foreach (KeyValuePair<string, Line> rollWL in rollLinesWL)
            {
                Point intersectPointMiddle = new Point();

                StaticaLargeAnglesUtils.Instance.getIntersectWLWithSectionOrDeck(rollWL.Value, midSectionPoints, ref intersectPointMiddle);

                //Found draft on In-water rolled side = intersectPointMiddle.Y
                //IMPORTENT: intersectPointMiddle.Y - will be same for all other Sections, as Rolling without Different.                   

                // Find Intersection or hull Frames/Sections with Rolling WL
                Dictionary<string, Point> intersectWLPoints = new Dictionary<string, Point>();
                if (intersectPointMiddle.Y < midSectionPoints.First().Y) //check if deck not in water
                {
                    getRollWLPointsBySectionByDraft(ref intersectWLPoints, intersectPointMiddle.Y, i_side);
                }
                else
                {
                    getIntersectWLAndDeckPointsBySection(pointHullSectionsRightBoard, rollWL.Value, intersectPointMiddle, ref intersectWLPoints);
                }

                string wlKey = rollWL.Key + i_side;
                intersectWLSectionDict.Add(wlKey, intersectWLPoints);
            }//foreach WL

        }

        public double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        public double getInegralSumOfListDouble(List<double> list)
        {
            double integralSum = 0;
            foreach (double value in list)
            { 
                integralSum += value;
            }
            return integralSum;
        }

        public double getZcByDraft(double draft, List<double> shipDraftsList)
        {
            double zC = 0;
            Dictionary<double, double> zCByDraft = new Dictionary<double, double>();
            Statica.Instance.getZcByDraftsList(shipDraftsList, ref zCByDraft);
            zCByDraft.TryGetValue(draft, out zC);
            if (zC == 0)
            {
                zC = Statica.Instance.getZcByDraft(draft);
            }

            return zC;
        }
//------------------Class end --------------
    }
}
