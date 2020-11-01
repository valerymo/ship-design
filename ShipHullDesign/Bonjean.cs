using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media.Media3D;

namespace HullDesign1
{
    public sealed class Bonjean
    {

       private static volatile Bonjean instance;
       private static object syncRoot = new Object();
       private Dictionary<string, List<point>> m_hullSections = new Dictionary<string, List<point>>();
       private Dictionary<string, List<Point3D>> m_hullSections3D = new Dictionary<string, List<Point3D>>();
       private List<double> m_shipDraftList = new List<double> { 0, 1, 2, 3, 4 };
       private Dictionary<string, Polyline> m_polylineSectionAreasByDraft = new Dictionary<string, Polyline>();
       private Dictionary<string, double> m_hullSectionsAreasByDraft = new Dictionary<string, double>();
       private System.Windows.Controls.DataGrid m_sectionAreasTableDataGrid = new System.Windows.Controls.DataGrid();
       private Grid m_sectionAreasGrid = new Grid();


        //main dimentions - tmp, need to be taken from main
       private double L;
       private double B;
       private double T;
       private double H;

       private double m_bowDraft;
       private double m_sternDraft;
       private double m_shpacia;
             
       
       public static Bonjean Instance
       {
           get
           {
               if (instance == null)
               {
                   lock (syncRoot)
                   {
                       if (instance == null)
                           instance = new Bonjean();
                   }
               }
               return instance;
           }
       }

       private Bonjean()
       {
           //initData();
     
       }

       private double getWLDelta()
       {
           //Take Draft as Design Watreline (Konstru
           double l_WLDelta =T / 10;

           return l_WLDelta;
       }

      

       public void initData()
       {
           //  main dimantions -- tmp; need to be taken from main
           L = 75;
           B = 12.2;
           T = 4;
           //H = 7;

           Sections.Instance.getPointHullSections(ref m_hullSections);
           Sections.Instance.getPoint3DHullSections(ref m_hullSections3D);

           initPolylineSectionAreasByDraft();

           //foreach (KeyValuePair<string, List<point>> frame in m_hullSections)
           //{
           //    m_polylineSectionAreasByDraft.Add(frame.Key, new Polyline());
           //}

           //setSectionAreasGrid();
           m_shpacia = checkShpacia();
       }


       public void drawBonjeanWithGrid(ref Canvas canvas, ref System.Windows.Controls.Grid dataGrid1 /*, ref Grid sectionAreasGrid*/)
       {
           initData();
           drawGrid(ref canvas, 50, -50, 250, 230);
           //orig CoordXMove = 300

           drawBonjean(ref canvas, 50, -50, 250, 230);

           //m_sectionAreasTableDataGrid = dataGrid1;
           //setSectionAreasTableGrid();

           //m_sectionAreasGrid = sectionAreasGrid;
           //setSectionAreasGrid();

       }

       private void drawGrid(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
        {
           
            double l_WLDelta = getWLDelta();
            double l_WaterLine = 0;

            double xCoord = 6;
            double fixXCoord = 1;

            for (l_WaterLine = 0; l_WaterLine < T; l_WaterLine += l_WLDelta)
            {
                Line wl = new Line();
                wl.X1 = xCoord * ScaleX + CoordXMove;
                wl.X2 = (-xCoord + fixXCoord) * ScaleX + CoordXMove;
                wl.Y1 = wl.Y2 = l_WaterLine * ScaleY + CoordYMove;

                wl.Stroke = System.Windows.Media.Brushes.Red;
                wl.StrokeThickness = 0.5;
                framesCanvas.Children.Add(wl);
            }

            drawHullSectionsGrid(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove);
        }

       private void setSectionAreasTableGrid()
       {
           DataGridTextColumn column1 = new DataGridTextColumn();
           column1.Header = "Num";
           m_sectionAreasTableDataGrid.Columns.Add(column1);

           DataGridTextColumn column2 = new DataGridTextColumn();
           column2.Header = "S m2";
           m_sectionAreasTableDataGrid.Columns.Add(column2);

           for (int i = 1; i < 21; i++)
           {
               DataGridViewRow row = new DataGridViewRow();
               m_sectionAreasTableDataGrid.Items.Add(row);
           }
       }


       private void drawHullSectionsGrid(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
        {

            double l_deltaX = getSectionsDeltaX(framesCanvas);
            double X1 = 0;
            double X2= 0;
            double Y1, Y2;
            Y1 = 0 + CoordYMove;
            Y2 = T * ScaleY + CoordYMove;

            foreach (KeyValuePair<string, List<point>> frame in m_hullSections)
            {
                
                Line sectionGridLine = new Line();
                sectionGridLine.Y1 = Y1;
                sectionGridLine.Y2 = Y2;
                //double sectionX = frame.Value.First().x;
                X1 += l_deltaX;
                X2 = X1;
                sectionGridLine.X1 = sectionGridLine.X2 = X1;
    
                sectionGridLine.Stroke = System.Windows.Media.Brushes.Red;
                sectionGridLine.StrokeThickness = 0.5;
                framesCanvas.Children.Add(sectionGridLine);
            }
       }

       private double getSectionsDeltaX(Canvas framesCanvas)
       {
            double sectionsDeltaX = 10;
            double canvasWidth = framesCanvas.Width; //545

            int numOfSections = m_hullSections.Count;

            sectionsDeltaX = canvasWidth / (numOfSections - 1); 
          
            return sectionsDeltaX;
       }

       private void drawBonjean(ref Canvas framesCanvas, int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
       {
           double l_deltaX = getSectionsDeltaX(framesCanvas);
           
           foreach (float draft in m_shipDraftList)
           {
               Dictionary<string, double> hullSectionsAreas = new Dictionary<string, double>();
               Sections.Instance.getSectionsAreasByDraft(draft, ref hullSectionsAreas);
               double gridX = 0;
               foreach (KeyValuePair<string, double> sectionArea in hullSectionsAreas)
               {
                   gridX += l_deltaX;
                   foreach (KeyValuePair<string, Polyline> node in m_polylineSectionAreasByDraft)
                   {
                       if (node.Key == sectionArea.Key)
                       {
                           double polyLineX = sectionArea.Value * ScaleX / 10 + gridX;
                           double polyLineY = (double)draft * ScaleY + CoordYMove;
                           node.Value.Points.Add(new Point (polyLineX, polyLineY));
                           break;
                       }
                   }

               }
           }

           drawHullSectionsPolylinesByDraft(ref framesCanvas);

       }


       public bool drawHullSectionsPolylinesByDraft(ref Canvas framesCanvas)
       {
           foreach (KeyValuePair<string, Polyline> frame in m_polylineSectionAreasByDraft)
           {
               frame.Value.Stroke = System.Windows.Media.Brushes.Navy;
               frame.Value.StrokeThickness = 1.3;
               framesCanvas.Children.Add(frame.Value);
           }
           return true;
       }

       //void setDraftsSliders()
       //{
       //    Slider hsliderDraftBow = new Slider();
       //    hsliderDraftBow.Name = "sliderDraftBow";
       //    hsliderDraftBow.Minimum = 0;
       //    hsliderDraftBow.Maximum = T;
       //    //.....


       //}

        public void setBowDraft (double draft)
        {
            m_bowDraft = draft;
        }
        public void setSternDraft(double draft)
        {
            m_sternDraft = draft;
        }

        public double getBowDraft()
        {
            return m_bowDraft;
        }
        public double getSternDraft()
        {
            return m_sternDraft;
        }

        public void drawBonjeanWLDifferentShipSide(ref Canvas framesCanvas)
        {

        }

        public void drawBonjeanWLDifferent(ref Canvas framesCanvas, double firstSectionY, double lastSectionY,
                                                        int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
        {
                     
            if (m_hullSections3D.Count > 0)
            {
                double firstSectionX = m_hullSections3D.First().Value.First().Z;
                double lastSectionX = m_hullSections3D.Last().Value.First().Z;
                Line watelineDiff = new Line();
                //watelineDiff.X1 = firstSectionX * ScaleX/6;
                watelineDiff.X1 = framesCanvas.Width;
                watelineDiff.Y1 = firstSectionY * ScaleY + CoordYMove;

                //watelineDiff.X2 = lastSectionX * ScaleX/6;
                watelineDiff.X2 = 0;
                watelineDiff.Y2 = lastSectionY * ScaleY + CoordYMove;

                watelineDiff.Stroke = System.Windows.Media.Brushes.Red;
                watelineDiff.StrokeThickness = 2;
                framesCanvas.Children.Add(watelineDiff);
            }
        }

        public void redrowBonjean(ref Canvas framesCanvas, double firstSectionY, double lastSectionY,
                                       int ScaleX, int ScaleY, int CoordXMove, int CoordYMove)
        {
            framesCanvas.Children.Clear();
           
            drawGrid(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove);
            drawBonjean(ref framesCanvas, ScaleX, ScaleY, CoordXMove, CoordYMove);
            drawBonjeanWLDifferent(ref framesCanvas, firstSectionY, lastSectionY, ScaleX, ScaleY, CoordXMove, CoordYMove);

            populateSectionAreasTableGrid();
        }

        public void redrowBonjeanWithDifferent(ref Canvas framesCanvas, double firstSectionY, double lastSectionY)
        {
            framesCanvas.Children.Clear();
            initPolylineSectionAreasByDraft();

            drawGrid(ref framesCanvas, 50, -50, 250, 230);
            drawBonjean(ref framesCanvas, 50, -50, 250, 230);

            drawBonjeanWLDifferent(ref framesCanvas, firstSectionY, lastSectionY, 50, -50, 250, 230);

        }


        public void initSectionAreasByDraft (ref System.Windows.Data.CollectionViewSource bonjeanViewSource)
        {
            m_hullSectionsAreasByDraft.Clear();    
            Sections.Instance.getSectionsAreasByDraftWihDifferent((float)m_bowDraft, (float)m_sternDraft, ref m_hullSectionsAreasByDraft);

            //loadHullSectionsAreasByDraft(ref bonjeanViewSource);
        }


        private void initPolylineSectionAreasByDraft()
        {
            m_polylineSectionAreasByDraft.Clear();

            foreach (KeyValuePair<string, List<point>> frame in m_hullSections)
            {
                m_polylineSectionAreasByDraft.Add(frame.Key, new Polyline());
            }
        }

        private void populateSectionAreasTableGrid()
        {
            System.Windows.Forms.DataGrid myGrid = new System.Windows.Forms.DataGrid();
            System.Data.DataSet myDataSet = new System.Data.DataSet("myDataSet");

            foreach (KeyValuePair<string, double> node  in m_hullSectionsAreasByDraft)
            {
                System.Data.DataRow row = myDataSet.Tables[0].NewRow();
                row[1] = "1";
                row[2] = "50";
                myDataSet.Tables[0].Rows.Add(row);                           
            }
            myGrid.DataSource = myDataSet;
        }

        //public void loadHullSectionsAreasByDraft(ref System.Windows.Data.CollectionViewSource bonjeanViewSource)
        public Dictionary<string, double> getHullSectionsAreasByDraft()
        {
            return m_hullSectionsAreasByDraft;                       
        }

//-------------------------------------------------------------------------------------------------------------------

        public void calculateDisplacementAndBouancyXcByCurrentDraft(ref double o_DisplacementByDraft, ref double o_Xc)
        {
            checkShpacia();

            double l_DisplacementByDraft = 0;
            double l_prevSectionArea = 0; //previouse section area

            double l_Moment = 0;
            int count = 1;
            double l_armFromMiddle = 0;

            int middleSectionNum = Sections.Instance.getMiddleSectionNum();
            double middleSectionCoordX = middleSectionNum * m_shpacia;


            foreach (KeyValuePair<string, double> sectionArea in m_hullSectionsAreasByDraft)
            {
                //Displacement calculation
                l_DisplacementByDraft += (sectionArea.Value + l_prevSectionArea) / 2 * m_shpacia;

                //Moment calculation
                l_armFromMiddle = middleSectionCoordX - (m_shpacia * count);

                l_Moment += ((sectionArea.Value + l_prevSectionArea) / 2 * m_shpacia) * l_armFromMiddle;
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

//-------------------------------------------------------------------------------------------------------------------

        private double checkShpacia()
        {
            double shpacia = 1;

            double prevSectionX = 0;
            double currentSectionX =0;
            int count = 0;
            foreach (KeyValuePair<string, List<Point3D>> l_section in m_hullSections3D)
            {
                if (count == 0)
                {
                    currentSectionX = l_section.Value.First().Z;
                    count++;
                }
                else
                {
                    prevSectionX = currentSectionX;
                    currentSectionX = l_section.Value.First().Z;
                    shpacia = (-1) * (currentSectionX - prevSectionX);
                    break;
                }
            }
            
            return shpacia;
        }

        public double getShapcia()
        {
            return m_shpacia;
        }

    }
}
