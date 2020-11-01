using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Drawing;

using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Controls;

using System.Windows.Media;

namespace HullDesign1
{
    public sealed class Statica
    {
        // -------- Singleton definition
        private static volatile Statica instance;
        private static object syncRoot = new Object();


        private Statica()
        {
        }

        // -------- Singleton definition
        public static Statica Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Statica();
                    }
                }
                return instance;
            }
        }




        // Hydrostatics Curves
        private List<point> m_pointsXc = new List<point>();  // x - LCB, Longitudinal Centre of Buoyancy 
        private Polyline m_polyLineXc = new Polyline();
        private List<point> m_pointsXf = new List<point>();  // LCF, Longitudinal Centre of Flotation
        private List<point> m_pointsZc = new List<point>();  // Z-coordinate of Centre of Buoyancy 
        private List<point> m_pointsZm = new List<point>();  // Z-coordinate of Metacentre
        private List<point> m_pointsV = new List<point>();   // Displacement, m3
        private Polyline m_polyLineV = new Polyline();
        private List<point> m_pointsS = new List<point>();   // Waterline area, m2
        private List<point> m_pointsR0 = new List<point>();  // R0 = Iif/V
        private List<point> m_points_r0 = new List<point>(); // r0 = Ix/V

        //Ix - Момент инерции площади ватерлинии относительно центральной продольной оси, m4
        //Ix - Moment of inertia of the waterplane area about the centreline, м4
        private List<point> m_pointsIx = new List<point>();
        

        //Iy - Момент инерции площади ватерлинии относительно центральной поперечной оси
        //Iy - Moment of inertia of the waterplane area about the centreline
        private List<point> m_pointsIyf = new List<point>(); // Iy Moment of inertia of the waterplane area about a transverse axis

        //Coefficients of Hull Form
        private List<point> m_pointsAlfa = new List<point>();
        private List<point> m_pointsBeta = new List<point>();
        private List<point> m_pointsDelta = new List<point>();

        //Getters        
        public void getPointsXc(ref List<point> o_pointsXc) { o_pointsXc = m_pointsXc; }
        public void getPointsXf(ref List<point> o_pointsXf) { o_pointsXf = m_pointsXf; }
        public void getPointsZc(ref List<point> o_pointsZc) { o_pointsZc = m_pointsZc; }
        public void getPointsZm(ref List<point> o_pointsZm) { o_pointsZm = m_pointsZm; }
        public void getPointsV(ref List<point> o_pointsV) { o_pointsV = m_pointsV; }
        public void getPointsR0(ref List<point> o_pointsR0) { o_pointsR0 = m_pointsR0; }
        public void getPoints_r0(ref List<point> o_points_r0) { o_points_r0 = m_points_r0; }
        public void getPointsIx(ref List<point> o_pointsIx) { o_pointsIx = m_pointsIx; }
        public void getPointsIyf(ref List<point> o_pointsIyf) { o_pointsIyf = m_pointsIyf; }
        public void getPointsAlfa(ref List<point> o_pointsAlfa) { o_pointsAlfa = m_pointsAlfa; }
        public void getPointsBeta(ref List<point> o_pointsBeta) { o_pointsBeta = m_pointsBeta; }
        public void getPointsDelta(ref List<point> o_pointsDelta) { o_pointsDelta = m_pointsDelta; }


        public Dictionary<string, double> m_hullSectionsAreasByDraft = new Dictionary<string, double>();

        double m_shpacia = 0.75;
 
        // --------  Calculate Hydrostatic Curves points  -------------------------

        public void CalculateHydrostaticCurves()
        {
            //list of drafts (per waterline) -- STUB
            List<double>shipDraft = new List<double> {0,1,2,3,4,5,6};

            computePointsXcAndV(ref shipDraft);
        }

        //computePointsXc -- Xc - LCB, Longitudinal Centre of Buoyancy 
        // Вычисление абсциссы центра величины
        private bool computePointsXcAndV(ref List<double> shipDraftList)
        {
            //List<point> l_pointsXc = new List<point>();
            
            // for each Drafts from the list of Drafts
            //      get the list of Sections Areas 
            //          and calculate Xc - LCB, Longitudinal Centre of Buoyancy 

            m_pointsXc.Clear();
            m_polyLineXc.Points.Clear();

            int countTmp = 0;

            Dictionary<string, double> hullSectionsAreas = new Dictionary<string, double>();
            foreach (float l_Draft in shipDraftList)
            {
                hullSectionsAreas.Clear();
                //get Sections Areas
                Sections.Instance.getSectionsAreasByDraft(l_Draft, ref hullSectionsAreas);
                double l_xc = 0;
                double l_DisplacementByDraft = 0;
                calculateDisplacementAndBouancyXcByDraft (ref hullSectionsAreas, ref l_DisplacementByDraft, ref l_xc);
                
                point Xc;
                Xc.x = l_xc;
                Xc.y = l_Draft;
                if (countTmp != 0) // skip 0 waterline -- stub, TBD
                {
                    m_pointsXc.Add(Xc);
                    m_polyLineXc.Points.Add(new Point(Xc.x, Xc.y));
                }
                countTmp++;
                m_polyLineV.Points.Add(new Point(l_DisplacementByDraft, l_Draft));
                point V;
                V.x = l_DisplacementByDraft;
                V.y = l_Draft;
                m_pointsV.Add(V);

            }

            return true;
        }

        private void computePointsXf()
        {
            //Dictionary<string, List<Point3D>> l_hullWaterlines = new Dictionary<string, List<Point3D>>();

            //Waterlines.Instance.getPointsHullWaterlines(ref l_hullWaterlines);

            List<point> l_pointsXf = new List<point>();
            //....
            m_pointsXf = l_pointsXf;
        }

        private void computePointsZc()
        {
            List<point> l_pointsZc = new List<point>();
            //....
            m_pointsZc = l_pointsZc;
        }

        private void computePointsZm()
        {
            List<point> l_pointsZm = new List<point>();
            //....
            m_pointsZm = l_pointsZm;
        }

        private void computePointsV()
        {
            List<point> l_pointsV = new List<point>();
            //....
            m_pointsV = l_pointsV;
        }

        private void computePointsR0()
        {
            List<point> l_pointsR0 = new List<point>();
            //....
            m_pointsR0 = l_pointsR0;
        }

        private void computePointsr0()
        {
            List<point> l_points_r0 = new List<point>();
            //....
            m_points_r0 = l_points_r0;
        }

        private void computePointsIx()
        {
            List<point> l_pointsIx = new List<point>();
            //....
            m_pointsIx = l_pointsIx;
        }

        private void computePointsIyf()
        {
            List<point> l_pointsIyf = new List<point>();
            //....
            m_pointsIyf = l_pointsIyf;
        }

        private void computePointsAlfa()
        {
            List<point> l_pointsAlfa = new List<point>();
            //....
            m_pointsAlfa = l_pointsAlfa;
        }

        private void computePointsBeta()
        {
            List<point> l_pointsBeta = new List<point>();
            //....
            m_pointsBeta = l_pointsBeta;
        }

        private void computePointsDelta()
        {
            List<point> l_pointsDelta = new List<point>();
            //....
            m_pointsDelta = l_pointsDelta;
        }


        private bool calculateXcForDraft(ref double i_xc)
        {
            double l_Xc = 1;
            i_xc = l_Xc;
            return true;
        }

//----------------------------
        //27 Mar 2015
        void calculateDisplacementAndBouancyXcByDraft(ref Dictionary<string, double> i_hullSectionsAreasByDraft,
                                               ref double o_DisplacementByDraft,
                                               ref double o_Xc)
        {
            double l_DisplacementByDraft = 0;
            double l_shpacia = 0.75; //1 m
            double l_prevSectionArea = 0; //previouse section area

            double l_Moment = 0;
            int count = 1;
            double l_armFromMiddle = 0;

            int middleSectionNum = Sections.Instance.getMiddleSectionNum();
            double middleSectionCoordX = middleSectionNum * l_shpacia;


            foreach (KeyValuePair<string, double> sectionArea in i_hullSectionsAreasByDraft)
            {
                //Displacement calculation
                l_DisplacementByDraft += (sectionArea.Value + l_prevSectionArea) / 2 * l_shpacia;

                //Moment calculation
                l_armFromMiddle = middleSectionCoordX - (l_shpacia * count); 

                l_Moment += ((sectionArea.Value + l_prevSectionArea) / 2 * l_shpacia) * l_armFromMiddle;
                count++;
                
                //next
                l_prevSectionArea = sectionArea.Value;
            }

            o_DisplacementByDraft = l_DisplacementByDraft;

            //Center of Bouancy -- X coordinate
            if (o_DisplacementByDraft == 0)
            {
                o_Xc = 0;
            }
            else
            {
                o_Xc = l_Moment / o_DisplacementByDraft;
            }

        }

        public bool drawXcPolyline(ref Canvas framesCanvas)
        {
            m_polyLineXc.Stroke = System.Windows.Media.Brushes.Black;
            m_polyLineXc.StrokeThickness = 0.5;
            framesCanvas.Children.Add(m_polyLineXc);
            
            return true;
        }
        public bool drawXcPolyline(ref Canvas framesCanvas, ref Polyline i_polyline)
        {
            i_polyline.Stroke = System.Windows.Media.Brushes.Black;
            i_polyline.StrokeThickness = 0.5;
            framesCanvas.Children.Add(i_polyline);

            return true;
        }
        public bool drawHydrostaticPolyline (   ref Canvas framesCanvas, 
                                                ref Polyline i_polyline,
                                                System.Windows.Media.Brush i_colour)
        {
            //i_polyline.Stroke = System.Windows.Media.Brushes.Black;
            i_polyline.Stroke = i_colour;
            i_polyline.StrokeThickness = 0.5;
            framesCanvas.Children.Add(i_polyline);

            return true;
        }

        public bool drawHydrostaticPolylines(ref Canvas framesCanvas)
        {
            drawXcPolyline(ref framesCanvas);
            return true;
        }


        private void drawDraftLines(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> shipDraftList)
        {
            int xCoord = 8;
            int H = 7; //STUB

            foreach (double l_WaterLine in shipDraftList)
            {
                Line wl = new Line();
                wl.X1 = xCoord * ScaleX + CoordXMove;
                wl.X2 = -xCoord * ScaleX + CoordXMove;
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

        public bool drawXcPolylineInScale(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, List<double> shipDraftList)
        {
            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            foreach (Point node in m_polyLineXc.Points)
            {
                //scaleNode.X = node.X * ScaleX + CoordXMove;
                scaleNode.X = node.X * ScaleX / 5 + CoordXMove;
                scaleNode.Y = node.Y * ScaleY + CoordYMove;
                l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));
            }

            //framesCanvas.Children.Clear();
            l_polyLine.Stroke = System.Windows.Media.Brushes.Black;
            l_polyLine.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLine);

            //drawDraftLines(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);

            return true;
        }

        public bool drawVPolylineInScale(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, List<double> shipDraftList)
        {
            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            foreach (Point node in m_polyLineV.Points)
            {
                scaleNode.X = node.X * ScaleX/200 + CoordXMove;
                scaleNode.Y = node.Y * ScaleY + CoordYMove;
                l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));
            }

            //framesCanvas.Children.Clear();
            l_polyLine.Stroke = System.Windows.Media.Brushes.Blue;
            l_polyLine.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLine);

            return true;
        }

        //public bool drawHydrostaticCurvesPolylinesInScale (ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> shipDraftList)
         public bool drawHydrostaticCurvesPolylinesInScale (ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
        {
            List<double> shipDraftList = new List<double>();
            double T = 6;
            double l_WLDelta = (T / 10);

            for (double draft = 0; draft <= T; )
            {
                shipDraftList.Add(draft);
                draft += l_WLDelta;
            }


            framesCanvas.Children.Clear();
            //draw Xc
            drawXcPolylineInScale(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, shipDraftList);
            drawVPolylineInScale(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, shipDraftList);
            //draw Xf
            //draw Moment Inertia X of Waterlines
            drawMomentInertiaXWaterlinesPolylineInScale(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            drawStaticMomentXfWaterLinesInScaleByDraft(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            drawMomentInertiaYfWaterlinesPolylineInScale(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            drawWaterlineAreasPolylineInScale(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            //drawWaterlineAreasPolylineInScaleByDraft(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            drawBigMetacentricRadius(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            drawSmallMetacentricRadius(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            drawZm(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            //draw ....
            drawDraftLines(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove, ref shipDraftList);
            return true;
        }

        private bool computeWaterLinesAreas(ref List<double> shipDraftList)
        {
            

            return true;
        }

//----------------------------------------------------------------------

        private bool computeMomentInertiaXSingleWaterline(Polyline i_waterLine, ref double o_momentInertiaX)
        {
            double wlArea = 0;
            //Waterlines.Instance.getSingleWaterlineArea(i_waterLine, ref wlArea);
            double l_shpacia = 0.75;
            double momentInertiaX = 0;
            double momentInertiaXWL = 0;

            //compute moment Inertia  ... continue -- today date: 13 June
           //Ix = 2/3 Integral y3*dx
            int count = 0;
            Point pevPoint = new Point();
            foreach (Point point in i_waterLine.Points)
            {
                momentInertiaX = (-1) * (Math.Pow(point.Y, 3) * l_shpacia);
                momentInertiaXWL += momentInertiaX;
            }

            o_momentInertiaX = 0.75 * momentInertiaXWL;
            return true;
        }

        private bool computeMomentInertiaXWaterlines(ref List<double> i_shipDraftList, ref Dictionary<double, double> o_momentInertiaXWaterline)
        {
            Polyline waterLine = new Polyline();

            foreach (double l_Draft in i_shipDraftList)
            {
                double momentInertiaX = 0;
                Polyline waterline = new Polyline();
                Waterlines.Instance.getWaterlineByDraftCreateFromSections((float)l_Draft, ref waterline);
                computeMomentInertiaXSingleWaterline(waterline, ref momentInertiaX);
                o_momentInertiaXWaterline[l_Draft] = momentInertiaX;
            }            
              
            return true;
        }


        private bool drawMomentInertiaXWaterlinesPolylineInScale(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> i_shipDraftList)      
        {
            Dictionary<double, double> momentInertiaXWaterlinesByDraft = new Dictionary<double, double>();
            computeMomentInertiaXWaterlines(ref i_shipDraftList, ref momentInertiaXWaterlinesByDraft);

            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            double momentInertia = 0;

            foreach (double l_Draft in i_shipDraftList)
            {
                if (l_Draft == 0)
                    continue;
                if (momentInertiaXWaterlinesByDraft.ContainsKey(l_Draft))
                {
                    momentInertia = momentInertiaXWaterlinesByDraft[l_Draft];
                    scaleNode.X = momentInertia * ScaleX / 1000 + CoordXMove;
                    scaleNode.Y = l_Draft * ScaleY + CoordYMove;
                    l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));
                }
            }

            //framesCanvas.Children.Clear();
            l_polyLine.Stroke = System.Windows.Media.Brushes.OrangeRed;
            l_polyLine.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLine);

            return true;
        }

//----------------------------------------
        public bool computeStaticMomentOfWaterlineAreaFromMidelAndXf(Polyline i_waterLine, ref double o_StaticMoment, ref double o_Xf)
        {
            double l_shpacia = 0.75;
            double staticMoment = 0;

            int middleSectionNum = Sections.Instance.getMiddleSectionNum();
            double middleSectionCoordX = middleSectionNum * l_shpacia;

            int count = 0;
            double sectionCoordXFromMiddel = 0;

            foreach (Point point in i_waterLine.Points)
            {
                sectionCoordXFromMiddel = middleSectionCoordX - point.X; 
               // if (sectionCoordXFromMiddel < 0)
                //    sectionCoordXFromMiddel = (-1) * sectionCoordXFromMiddel;

                staticMoment = ((-1) * point.Y * l_shpacia) * sectionCoordXFromMiddel;

                o_StaticMoment += staticMoment;
            }

            o_StaticMoment = 2 * o_StaticMoment;

            double waterlineArea  = 0;
            Waterlines.Instance.getWaterlineAreaByPolyline(i_waterLine, ref waterlineArea);

            o_Xf = o_StaticMoment / waterlineArea;

            return true;            
  
        }

//------
        private bool computeStaticMomentOfWaterlinesByDraft(ref List<double> i_shipDraftList, 
                                                     ref Dictionary<double, double> o_staticMomentWaterlines,
                                                     ref Dictionary<double, double> o_XftWaterlines)
        {
            Polyline waterLine = new Polyline();

            foreach (double l_Draft in i_shipDraftList)
            {
                double staticMoment = 0;
                double xF = 0;

                Polyline waterline = new Polyline();
                Waterlines.Instance.getWaterlineByDraftCreateFromSections((float)l_Draft, ref waterline);
                computeStaticMomentOfWaterlineAreaFromMidelAndXf(waterline, ref staticMoment, ref xF);
                o_staticMomentWaterlines[l_Draft] = staticMoment;
                o_XftWaterlines[l_Draft] = xF;
            }

            return true;
        }
//-------
        private bool drawStaticMomentXfWaterLinesInScaleByDraft(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> i_shipDraftList)
        {
            Dictionary<double, double> staticMomentWaterlinesByDraft = new Dictionary<double, double>();
            Dictionary<double, double> xFWaterlinesByDraft = new Dictionary<double, double>();
            computeStaticMomentOfWaterlinesByDraft(ref i_shipDraftList, ref staticMomentWaterlinesByDraft, ref xFWaterlinesByDraft);

            Polyline l_polyLineStaticMoment = new Polyline();
            Point scaleNodeStaticMoment = new Point();

            Polyline l_polyLineXf = new Polyline();
            Point scaleNodeXf = new Point();


            double staticMoment = 0;
            double xF = 0;

            foreach (double l_Draft in i_shipDraftList)
            {
                if (l_Draft == 0)
                    continue;
                if (staticMomentWaterlinesByDraft.ContainsKey(l_Draft))
                {
                    staticMoment = staticMomentWaterlinesByDraft[l_Draft];
                    scaleNodeStaticMoment.X = staticMoment * ScaleX / 800 + CoordXMove;
                    scaleNodeStaticMoment.Y = l_Draft * ScaleY + CoordYMove;
                    l_polyLineStaticMoment.Points.Add(new Point(scaleNodeStaticMoment.X, scaleNodeStaticMoment.Y));
                }

                if (xFWaterlinesByDraft.ContainsKey(l_Draft))
                {
                    staticMoment = xFWaterlinesByDraft[l_Draft];
                    scaleNodeXf.X = staticMoment * ScaleX /5 + CoordXMove;
                    scaleNodeXf.Y = l_Draft * ScaleY + CoordYMove;
                    l_polyLineXf.Points.Add(new Point(scaleNodeXf.X, scaleNodeXf.Y));
                }            
            }


            //framesCanvas.Children.Clear();
            l_polyLineStaticMoment.Stroke = System.Windows.Media.Brushes.Olive;
            l_polyLineStaticMoment.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLineStaticMoment);

            l_polyLineXf.Stroke = System.Windows.Media.Brushes.Navy;
            l_polyLineXf.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLineXf);


            return true;
        }
//------------------------------------------------------------------------
        private bool computeMomentInertiaYfSingleWaterline(Polyline i_waterLine, ref double o_momentInertiaYf)
        {
            double wlArea = 0;
            double l_shpacia = 0.75;
            double momentInertiaYf = 0;
            double momentInertiaYfIntgrl = 0;
            double staticMoment = 0;
            double xF = 0;

            Waterlines.Instance.getWaterlineAreaByPolyline(i_waterLine, ref wlArea);
            computeStaticMomentOfWaterlineAreaFromMidelAndXf(i_waterLine, ref staticMoment, ref xF);

            //compute moment Inertia  date: 20 June 2015
            //Iyf = 1/2 Integral *x2*y*dx
            foreach (Point point in i_waterLine.Points)
            {
                momentInertiaYf = (-1) * (Math.Pow(point.X, 2) * point.Y * l_shpacia);
                momentInertiaYfIntgrl += momentInertiaYf;
            }

            o_momentInertiaYf = 0.5 * momentInertiaYfIntgrl;
            o_momentInertiaYf = o_momentInertiaYf - (Math.Pow(xF, 2)) * wlArea;

            return true;
        }

        private bool computeMomentInertiaYfWaterlines(ref List<double> i_shipDraftList, ref Dictionary<double, double> o_dMomentInertiaYf)
        {
            Polyline waterLine = new Polyline();

            foreach (double l_Draft in i_shipDraftList)
            {
                double momentInertiaYf = 0;
                Polyline waterline = new Polyline();
                Waterlines.Instance.getWaterlineByDraftCreateFromSections((float)l_Draft, ref waterline);
                computeMomentInertiaYfSingleWaterline(waterline, ref momentInertiaYf);
                o_dMomentInertiaYf[l_Draft] = momentInertiaYf;
            }

            return true;
        }


        private bool drawMomentInertiaYfWaterlinesPolylineInScale(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> i_shipDraftList)
        {
            Dictionary<double, double> momentInertiaYfWaterlinesByDraft = new Dictionary<double, double>();
            computeMomentInertiaYfWaterlines(ref i_shipDraftList, ref momentInertiaYfWaterlinesByDraft);

            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            double momentInertia = 0;

            foreach (double l_Draft in i_shipDraftList)
            {
                if (l_Draft == 0)
                    continue;
                if (momentInertiaYfWaterlinesByDraft.ContainsKey(l_Draft))
                {
                    momentInertia = momentInertiaYfWaterlinesByDraft[l_Draft];
                    scaleNode.X = momentInertia * ScaleX /10000 + CoordXMove;
                    scaleNode.Y = l_Draft * ScaleY + CoordYMove;
                    l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));
                }
            }

            //framesCanvas.Children.Clear();
            l_polyLine.Stroke = System.Windows.Media.Brushes.Orange;
            l_polyLine.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLine);

            return true;
        }


//------------------------------------------------------------------------

        private bool getWaterlineAreas(List<double> i_shipDraftList, ref Dictionary<double, double> o_waterlineAreas)
        {
            Polyline waterLine = new Polyline();

            foreach (double l_Draft in i_shipDraftList)
            {
                double waterlineArea = 0;
                Polyline waterline = new Polyline();
                Waterlines.Instance.getWaterlineByDraftCreateFromSections((float)l_Draft, ref waterline);
                Waterlines.Instance.getWaterlineAreaByPolyline(waterline, ref waterlineArea);
                o_waterlineAreas[l_Draft] = waterlineArea;
            }
            return true;
        }

        private bool drawWaterlineAreasPolylineInScaleByDraft(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> i_shipDraftList)
        {

            Dictionary<double, double> waterlineAreas = new Dictionary<double, double>();
            getWaterlineAreas(i_shipDraftList, ref waterlineAreas);

            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            double wlArea = 0;
            foreach (var node in waterlineAreas)
            {
                    wlArea = node.Value;
                    scaleNode.X = 2 * wlArea * ScaleX / 100 + CoordXMove;
                    scaleNode.Y = node.Key * ScaleY + CoordYMove;
                    l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));
            }

            l_polyLine.Stroke = System.Windows.Media.Brushes.PowderBlue;
            l_polyLine.StrokeThickness = 3;
            framesCanvas.Children.Add(l_polyLine);

            return true;
        }

       
        private bool drawWaterlineAreasPolylineInScale(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> i_shipDraftList)
        {

            Dictionary<double, double> waterlineAreas = new Dictionary<double, double>();
            getWaterlineAreas(ref waterlineAreas);

            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            double wlArea = 0;

            foreach (var node in waterlineAreas)
            {
                wlArea = node.Value;
                scaleNode.X = wlArea * ScaleX / 100 + CoordXMove;
                scaleNode.Y = node.Key * ScaleY + CoordYMove;
                l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));
 
            }

            //framesCanvas.Children.Clear();
            l_polyLine.Stroke = System.Windows.Media.Brushes.BlueViolet;
            l_polyLine.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLine);

            return true;
        }

//------------------------------------------------------------------------

        private bool getWaterlineAreas(ref Dictionary<double, double> o_waterlineAreas)
        { 
            Dictionary<string, List<Point3D>> pointImportedHullWaterlines3D = new Dictionary<string, List<Point3D>>();
            Waterlines.Instance.getPointsHullWaterlines(ref pointImportedHullWaterlines3D);

            Polyline l_polyLine;            
            foreach (KeyValuePair<string, List<Point3D>> waterline in pointImportedHullWaterlines3D)
            {
                l_polyLine = new Polyline();
                foreach (Point3D node in waterline.Value)
                {
                    //coord fix: x=z; y=x
                    l_polyLine.Points.Add(new Point(node.Z, node.X));
                }
                double waterlineArea = 0;
                double draft = waterline.Value[1].Y;
                Waterlines.Instance.getWaterlineAreaByPolyline(l_polyLine, ref waterlineArea);
                o_waterlineAreas[draft] = waterlineArea;                
            }
            return true;
        }

//------------------------------------------
        private bool getBigMetacentricRadiusByDraft(double i_Draft, ref double o_bigMetacentrRadius)
        {
            //Get Displacement for draft = i_Draft 
            Dictionary<string, double> hullSectionsAreas = new Dictionary<string, double>();
            Sections.Instance.getSectionsAreasByDraft((float)i_Draft, ref hullSectionsAreas);
            double Xc = 0;
            double displacementV = 0;
            calculateDisplacementAndBouancyXcByDraft(ref hullSectionsAreas, ref displacementV, ref Xc);

            if (displacementV == 0)
            {
                return false;
            } 

            // Get MomentInertiaYf for draft = i_Draft 
            Polyline waterline = new Polyline();
            double momentInertiaYf = 0;
            Waterlines.Instance.getWaterlineByDraftCreateFromSections((float)i_Draft, ref waterline);
            computeMomentInertiaYfSingleWaterline(waterline, ref momentInertiaYf);

            //Big M<etacentric Radius
            o_bigMetacentrRadius = momentInertiaYf / displacementV;
            

            return true; 
        }

        private bool getBigMetacentricRadius(List<double> i_shipDraftList, ref Dictionary<double, double> o_bigMetacentrRadius)
        {
            foreach (double l_Draft in i_shipDraftList)
            {
                if (l_Draft == 0)
                    continue;
                double bigMetacentrRadius = 0;
                getBigMetacentricRadiusByDraft(l_Draft, ref bigMetacentrRadius);
                o_bigMetacentrRadius[l_Draft] = bigMetacentrRadius;
            }
            return true;
        }

        private bool drawBigMetacentricRadius(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> i_shipDraftList)
        {

            Dictionary<double, double> bigMetacentrRadius = new Dictionary<double, double>();
            getBigMetacentricRadius(i_shipDraftList, ref bigMetacentrRadius);

            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            foreach (var node in bigMetacentrRadius)
            {
                scaleNode.X = node.Value * ScaleX /100 + CoordXMove;
                scaleNode.Y = node.Key * ScaleY + CoordYMove;
                l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));

            }

            //framesCanvas.Children.Clear();
            l_polyLine.Stroke = System.Windows.Media.Brushes.Chartreuse;
            l_polyLine.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLine);

            return true;
        }

//-------------------------------------------------------------------------------------------------
       
        private bool getSmallMetacentricRadiusByDraft(double i_Draft, ref double o_smallMetacentrRadius)
        {
            //Get Displacement for draft = i_Draft 
            Dictionary<string, double> hullSectionsAreas = new Dictionary<string, double>();
            Sections.Instance.getSectionsAreasByDraft((float)i_Draft, ref hullSectionsAreas);
            double Xc = 0;
            double displacementV = 0;
            calculateDisplacementAndBouancyXcByDraft(ref hullSectionsAreas, ref displacementV, ref Xc);

            if (displacementV == 0)
            {
                return false;
            }

            // Get MomentInertiaYf for draft = i_Draft 
            Polyline waterline = new Polyline();
            double momentInertiaX = 0;
            Waterlines.Instance.getWaterlineByDraftCreateFromSections((float)i_Draft, ref waterline);
            computeMomentInertiaXSingleWaterline(waterline, ref momentInertiaX);

            //Big M<etacentric Radius
            o_smallMetacentrRadius = momentInertiaX / displacementV;


            return true;
        }

        private bool getSmallMetacentricRadius(List<double> i_shipDraftList, ref Dictionary<double, double> o_smallMetacentrRadius)
        {
            foreach (double l_Draft in i_shipDraftList)
            {
                if (l_Draft == 0)
                    continue;
                double smallMetacentrRadius = 0;
                getSmallMetacentricRadiusByDraft(l_Draft, ref smallMetacentrRadius);
                o_smallMetacentrRadius[l_Draft] = smallMetacentrRadius;
            }
            return true;
        }

        private bool drawSmallMetacentricRadius(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> i_shipDraftList)
        {

            Dictionary<double, double> smallMetacentrRadius = new Dictionary<double, double>();
            getSmallMetacentricRadius(i_shipDraftList, ref smallMetacentrRadius);

            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            foreach (var node in smallMetacentrRadius)
            {
                scaleNode.X = node.Value * ScaleX / 5 + CoordXMove;
                scaleNode.Y = node.Key * ScaleY + CoordYMove;
                l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));

            }

            //framesCanvas.Children.Clear();
            l_polyLine.Stroke = System.Windows.Media.Brushes.Chartreuse;
            l_polyLine.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLine);

            return true;
        }
//-------------------------------------------------------------------------------
        public bool getZcByDraftsList(List<double> i_shipDraftList, ref Dictionary<double, double> o_dZc)
        {
            Dictionary<string, double> hullSectionsAreas = new Dictionary<string, double>();

            double Xc = 0;
            double displacementV = 0;
            double prevDraft = 0;
            double prevDisplacement = 0;
            double deltaDisplacement = 0;
            double deltaDraft = 0;
            double integrationSum = 0;
            foreach (double l_Draft in i_shipDraftList)
            {           
                hullSectionsAreas.Clear();              
                Sections.Instance.getSectionsAreasByDraft((float)l_Draft, ref hullSectionsAreas);
                calculateDisplacementAndBouancyXcByDraft(ref hullSectionsAreas, ref displacementV, ref Xc);
                deltaDisplacement = displacementV - prevDisplacement;
                prevDisplacement = displacementV;
                deltaDraft = l_Draft - prevDraft;
                prevDraft = l_Draft;
                if (displacementV == 0)
                    continue;
                integrationSum +=  displacementV * deltaDraft;
                double Zc = l_Draft - (integrationSum / displacementV);
                o_dZc[l_Draft] = Zc;
             }

            return true;
        }

        //-------------------------------------------------------------------------------
        public double getZcByDraft(double iDraft)
        {

            double deltaDraft = 0.1;
            double halfDelta = deltaDraft / 2;
            double Zc = 0;
            double displacement = getDisplacementByDraft(iDraft);
            double wlArea = 0;
            double integratedWLMoment = 0;

            for (double draft = deltaDraft; draft <= iDraft; draft += deltaDraft)
            {
                wlArea = getWaterlineAreaByDraft(draft);
                integratedWLMoment += wlArea  * (draft - halfDelta);
            }
            Zc = integratedWLMoment * deltaDraft / displacement;         
            return Zc;
        }

        private double getWaterlineAreaByDraft(double draft)
        {
            double wlArea = 0;
            Polyline waterline = new Polyline();
            Waterlines.Instance.getWaterlineByDraftCreateFromSections((float)draft, ref waterline);
            Waterlines.Instance.getWaterlineAreaByPolyline(waterline, ref wlArea);
            return wlArea;
        }

        private double getDisplacementByDraft(double draft)
        {
            double displacementByDraft = 0;
            double prevSectionArea = 0;
            Dictionary<string, double> hullSectionsAreas = new Dictionary<string, double>();

            Sections.Instance.getSectionsAreasByDraft((float)draft, ref hullSectionsAreas);        
            foreach (KeyValuePair<string, double> sectionArea in hullSectionsAreas)
            {
                displacementByDraft += (sectionArea.Value + prevSectionArea) / 2 * m_shpacia;
                prevSectionArea = sectionArea.Value;
            }

            return displacementByDraft;
        }


        private bool getZm(List<double> i_shipDraftList,  Dictionary<double, double> i_dZc, 
                                                          Dictionary<double, double> i_dSmallMetacentRadius,
                                                          ref Dictionary<double, double> i_dZm)
        {
            Dictionary<double, double> l_dZm = new Dictionary<double, double>();
            foreach (double l_Draft in i_shipDraftList)
            {
                if (l_Draft == 0)
                    continue;
                l_dZm[l_Draft] = i_dSmallMetacentRadius[l_Draft] - i_dZc [l_Draft];
            }

            i_dZm = l_dZm;
            return true;
        }

        private bool drawZm(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove, ref List<double> i_shipDraftList)
        {

            Dictionary<double, double> dZcByDraft = new Dictionary<double, double>();
            Dictionary<double, double> dSmallMetacentRadius = new Dictionary<double, double>();
            Dictionary<double, double> l_dZmByDraft = new Dictionary<double, double>();

            getZcByDraftsList(i_shipDraftList, ref dZcByDraft);
            getSmallMetacentricRadius(i_shipDraftList, ref dSmallMetacentRadius);
            getZm(i_shipDraftList, dZcByDraft, dSmallMetacentRadius, ref l_dZmByDraft);

            Polyline l_polyLine = new Polyline();
            Point scaleNode = new Point();

            foreach (var node in l_dZmByDraft)
            {
                scaleNode.X = node.Value * ScaleX / 5 + CoordXMove;
                scaleNode.Y = node.Key * ScaleY + CoordYMove;
                l_polyLine.Points.Add(new Point(scaleNode.X, scaleNode.Y));

            }

            //framesCanvas.Children.Clear();
            l_polyLine.Stroke = System.Windows.Media.Brushes.SeaGreen;
            l_polyLine.StrokeThickness = 1;
            framesCanvas.Children.Add(l_polyLine);

            return true;
        }

        public double getShpacia()
        {
            return m_shpacia;
        }

    } //class Statica
}
