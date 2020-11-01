using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HullDesign1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>   
    public partial class MainWindow : Window
    {
        private string m_sectionsFileName;
        int m_ScaleX = 30;
        int m_ScaleY = -30;
        int m_CoordXMove = 100;
        int m_CoordYMove = 230;

        int ROUND = 2;

        TextBox bowDraftTB = new TextBox();


        public MainWindow()
        {
            InitializeComponent();
            this.MouseWheel += new MouseWheelEventHandler(MainWindow_MouseWheel);

            //Open Configuration Dialog; !!!!???? -- moved to section -- need tb checked
            //ConfigDlg dbConfigDialog = new ConfigDlg();
            //dbConfigDialog.ShowDialog();

            Hull3D.Instance.drawHull3D(myViewport);
            HullDraft.Instance.drawHullDraftInitial(ref canvasHullDesign, ref canvas4);
            ShipHullSurface.Instance.drawShipHullSurface(myViewport1);
            Bonjean.Instance.drawBonjeanWithGrid(ref canvasBonjeanCurves, ref BonjeanWindowGrid);
            StaticaLargeAngles.Instance.initialProcessingDynamicStability(ref canvasRollingWLs,
                                                             ref canvasArmsOfStaticStability,
                                                             ref canvasArmsOfDynamicStability);

            //for test:
            Sections.Instance.getSectionsAreas();

 
            ////Test connection (tmp)
            //int tmpCount = DBConnect.Instance.Count();
            //int tmpCount1 = DBConnect.Instance.Count();
            //string sFileString = DBConnect.Instance.getSectionsFileString("Ship2");
            //string tmp = "";

            Statica.Instance.CalculateHydrostaticCurves();
            Statica.Instance.drawHydrostaticPolylines(ref canvasHydrostaticCurves);
  
        }

        public static int tryMe()
        {
            return 1;
        }


        void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                m_ScaleX += 5;
                m_ScaleY -= 5;
            }
            else
            {
                if (m_ScaleY < 0)
                {
                    m_ScaleX -= 5;
                    m_ScaleY += 5;
                }
            }

            HullDraft.Instance.drawHullSectionsPolylinesInScale(ref canvasHullDesign,
                     m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);

            HullDraft.Instance.drawHullWaterlinesPolylinesInScale(ref canvas4,
                     m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);           
 

            Sections.Instance.drawHullSectionsPolylinesInScale(ref canvas1,
                                  m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);

            Sections.Instance.drawHullSectionsPolylinesInScaleByDraft(ref canvas2,
                       m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);


            //List<double> shipDrafts = new List<double> { 0, 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6 }; // stub
            //Statica.Instance.drawHydrostaticCurvesPolylinesInScale(ref canvasHydrostaticCurves,
            //           m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove, ref shipDrafts);
            Statica.Instance.drawHydrostaticCurvesPolylinesInScale(ref canvasHydrostaticCurves,m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource bonjeanViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("bonjeanViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // bonjeanViewSource.Source = [generic data source]
            Dictionary<string, double> dataSource = new Dictionary<string, double>();
            bonjeanViewSource.Source = dataSource;


        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            BodyPlan dlg = new BodyPlan();
            dlg.Show();
        }

        // private void button1_Click(object sender, RoutedEventArgs e)
        //{
  
        //    List<List<point>> hullSections = new List<List<point>>();

        //    Sections.Instance.getHullSections(ref hullSections);

        //    DXFConverterMaxSurf dxfMaxSurfConverter = new DXFConverterMaxSurf();

        //    dxfMaxSurfConverter.getFramesPoints();

        //}

        private void tabControl1_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        //click Button in tabItem "Sections"
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            //Sections.Instance.drawHullSections(ref canvas1);
            //Sections.Instance.drawHullFrames(ref canvas1);
            
            //Sections.Instance.drawHullSectionsPolylines(ref canvas1);
            Sections.Instance.drawHullSectionsPolylinesInScale(ref canvas1,
                                            m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);

             Sections.Instance.getSectionsAreas();

            Sections.Instance.setDraft(6, 0); // Draft, Angle
            //Sections.Instance.getSectionsAreasByDraft();
            Sections.Instance.drawHullSectionsPolylinesInScaleByDraft(ref canvas2,
                                           m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);

            //Sections.Instance.intersecSEctionsWithtWaterline(ref canvas3);
            //Sections.Instance.drawSetSectionalAreasCurve(ref canvas3);
         
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //List<List<point>> hullSections = new List<List<point>>();

            //Sections.Instance.getHullSections(ref hullSections);

            //************** get Input DXF file **************
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".dxf";
            dlg.Filter = "DXF files (.dxf)|*.dxf";
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            string l_dxfFilename;
            if (result == true)
            {
                //input file name 
                l_dxfFilename = dlg.FileName;

                //get Outputfile name
                Microsoft.Win32.OpenFileDialog dlg1 = new Microsoft.Win32.OpenFileDialog();
                // Set filter for file extension and default file extension 
                dlg1.DefaultExt = ".txt";
                dlg1.Filter = "Output file name (.txt)|*.txt";
                Nullable<bool> result_out = dlg1.ShowDialog();
                if (result_out == true)
                {
                    m_sectionsFileName = dlg1.FileName;
                    DXFConverterMaxSurf dxfMaxSurfConverter = new DXFConverterMaxSurf(l_dxfFilename, m_sectionsFileName);
                    dxfMaxSurfConverter.getFramesPoints();
                }

                Sections.Instance.refresh();
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            dlg.DefaultExt = ".hulls";
            dlg.Filter = "Ship Hull files (.hulls)|*.hulls";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                //FileNameTextBox.Text = filename;
            }


        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Sections.Instance.setDraft((float) e.NewValue, 0);

            Sections.Instance.drawHullSectionsPolylinesInScaleByDraft(ref canvas2,
            m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);

            Sections.Instance.drawSetSectionalAreasCurveByDraft(ref canvas3,
            m_ScaleX, m_ScaleY, m_CoordXMove, m_CoordYMove);

            //Sections.Instance.drawSetSectionalAreasCurve(ref canvas3);
        }

        private void dxfConvert_Click(object sender, RoutedEventArgs e)
        {
            //************** get Input DXF file **************
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".dxf";
            dlg.Filter = "DXF files (.dxf)|*.dxf";
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            string l_dxfFilename;
            if (result == true)
            {
                //input file name 
                l_dxfFilename = dlg.FileName;

                //get Outputfile name
                Microsoft.Win32.OpenFileDialog dlg1 = new Microsoft.Win32.OpenFileDialog();
                // Set filter for file extension and default file extension 
                dlg1.DefaultExt = ".txt";
                dlg1.Filter = "Output file name (.txt)|*.txt";
                Nullable<bool> result_out = dlg1.ShowDialog();
                if (result_out == true)
                {
                    DXFConverterMaxSurf dxfMaxSurfConverter = new DXFConverterMaxSurf(l_dxfFilename, dlg1.FileName);
                    dxfMaxSurfConverter.getWaterlines3DPoints();
                    dxfMaxSurfConverter.getSections3DPoints();
                    dxfMaxSurfConverter.getEdges3DPoints();
                }

                Sections.Instance.refresh();
                //Waterlines.Instance.refresh();
                //Buttocks.Instance.refresh();

            }
 
        }

 

        private void slider_bowDraftChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Bonjean.Instance.setBowDraft(e.NewValue);

            double bowDraft = Bonjean.Instance.getBowDraft();
            double sternDraft = Bonjean.Instance.getSternDraft();

            Bonjean.Instance.redrowBonjeanWithDifferent(ref canvasBonjeanCurves, bowDraft, sternDraft);

            if (textBoxBowDraft != null)
            {
                bowDraft = Math.Round(bowDraft, ROUND);
                textBoxBowDraft.Text = bowDraft.ToString();
            }

            if (textBoxSternDraft != null)
            {
                sternDraft = Math.Round(sternDraft, ROUND);
                textBoxSternDraft.Text = sternDraft.ToString();
            }                    

        }

        private void slider_sternDraftChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Bonjean.Instance.setSternDraft(e.NewValue);

            double bowDraft = Bonjean.Instance.getBowDraft();
            double sternDraft = Bonjean.Instance.getSternDraft();

            Bonjean.Instance.redrowBonjeanWithDifferent(ref canvasBonjeanCurves, bowDraft, sternDraft);

            if (textBoxBowDraft != null)
            {
                bowDraft = Math.Round(bowDraft, ROUND);
                textBoxBowDraft.Text = bowDraft.ToString();
            }

            if (textBoxSternDraft != null)
            {
                sternDraft = Math.Round(sternDraft, ROUND);
                textBoxSternDraft.Text = sternDraft.ToString();
            }

                 
         }

        private void textBoxBowDraft_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void sectionsAreasUpdate_button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource bonjeanViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("bonjeanViewSource")));
            Bonjean.Instance.initSectionAreasByDraft(ref bonjeanViewSource);

            var groupDescription1 = new PropertyGroupDescription("1");
            var groupDescription2 = new PropertyGroupDescription("2");
            bonjeanViewSource.GroupDescriptions.Add(groupDescription1);
            bonjeanViewSource.GroupDescriptions.Add(groupDescription2);

            Dictionary<string, double> dataSource = new Dictionary<string, double>();
            dataSource = Bonjean.Instance.getHullSectionsAreasByDraft();
            bonjeanViewSource.Source = dataSource;

            double bowDraft = Bonjean.Instance.getBowDraft();
            double sternDraft = Bonjean.Instance.getSternDraft();

            bowDraft = Math.Round(bowDraft, ROUND);
            textBoxBowDraft.Text = bowDraft.ToString();

            sternDraft = Math.Round(sternDraft, ROUND);
            textBoxSternDraft.Text = sternDraft.ToString();

            double Displacement = 0;
            double Xc = 0;
            Bonjean.Instance.calculateDisplacementAndBouancyXcByCurrentDraft(ref Displacement, ref Xc);
            textBoxDisplacement.Text = (Math.Round(Displacement, ROUND)).ToString();
            textBoxXc.Text = (Math.Round(Xc, ROUND)).ToString();
                      
        }

        //click Button in tabItem "StaticaLargeAngles"
        private void button_staticaLargeAngles_update_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StaticaLargeAngles.Instance.Draft = Convert.ToDouble(textBoxDraft.Text);
                StaticaLargeAngles.Instance.RollAngleMAX = Convert.ToInt16(textBoxMaxAngle.Text);
                StaticaLargeAngles.Instance.RollAngleStep = Convert.ToInt16(textBoxAngleStep.Text);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message); 
            }
            StaticaLargeAngles.Instance.processingDynamicStability(ref canvasRollingWLs,
                                                           ref canvasArmsOfStaticStability,
                                                           ref canvasArmsOfDynamicStability);
       }

 
 /*
        private void dynamicStabilityProperties_button_Click(object sender, RoutedEventArgs e)
        {
            DynamicStability.PropertiesOfDynamicStability prop = DynamicStability.Instance.getProperties();
            Console.WriteLine("Properties details - {0}", prop);
        }
        */
    }

}
