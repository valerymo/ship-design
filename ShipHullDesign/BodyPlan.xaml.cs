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
using System.Windows.Shapes;

namespace HullDesign1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class BodyPlan : Window
    {
        public BodyPlan()
        {
            InitializeComponent();

            //drawSection();
            drawCurve();
           //drawSpline();
        }

        void drawSection()
        {
            Line myLine;
            
            StackPanel myStackPanel = new StackPanel();
            
            myLine = new Line();
            //myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            myLine.Stroke = System.Windows.Media.Brushes.Black;
            myLine.X1 = 1;
            myLine.X2 = 50;
            myLine.Y1 = 1;
            myLine.Y2 = 50;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            myStackPanel.Children.Add(myLine);

            this.Content = myStackPanel;
        }
        void drawCurve()
        {
            Polyline line1 = new Polyline();
            Polyline line2 = new Polyline();
            Polyline line3 = new Polyline();
            //StackPanel myStackPanel = new StackPanel();
            Grid myGrid = new Grid();

            line1.Stroke = System.Windows.Media.Brushes.Black;
            line2.Stroke = System.Windows.Media.Brushes.Green;
            line3.Stroke = System.Windows.Media.Brushes.Blue;

            line1.Points.Add(new Point(10, 10));
            line1.Points.Add(new Point(50, 150));
            line1.Points.Add(new Point(100, 200));
            line1.Points.Add(new Point(250, 250));

            line2.BeginInit();

            line2.Points.Add(new Point(20, 10));
            line2.Points.Add(new Point(60, 150));
            line2.Points.Add(new Point(110, 200));
            line2.Points.Add(new Point(260, 250));

            line3.Points.Add(new Point(30, 10));
            line3.Points.Add(new Point(70, 150));
            line3.Points.Add(new Point(120, 200));
            line3.Points.Add(new Point(270, 250));

            line1.StrokeThickness = 2;
            myGrid.Children.Add(line1);

            line2.StrokeThickness = 2;
            myGrid.Children.Add(line2);

            line2.StrokeThickness = 2;
            myGrid.Children.Add(line3);

            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(40, 10);
            myPathFigure.Segments.Add(
                new BezierSegment(
                    new Point(80, 150),
                    new Point(130, 200),
                    new Point(280, 250),
                    true /* IsStroked */  ));

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures.Add(myPathFigure);

            // Display the PathGeometry. 
            Path myPath = new Path();
            myPath.Stroke = Brushes.Red;
            myPath.StrokeThickness = 3;
            myPath.Data = myPathGeometry;

            myGrid.Children.Add(myPath);

            this.Content = myGrid;   
        }

        void drawSpline()
        {
            PathFigure myPathFigure1 = new PathFigure();
            myPathFigure1.StartPoint = new Point(10, 10);
            myPathFigure1.Segments.Add(
                new BezierSegment(
                    new Point(50, 150),
                    new Point(100, 200),
                    new Point(250, 250),
                    true /* IsStroked */  ));

            PathFigure myPathFigure2 = new PathFigure();
            myPathFigure2.StartPoint = new Point(20, 10);
            myPathFigure2.Segments.Add(
                new BezierSegment(
                    new Point(60, 150),
                    new Point(110, 200),
                    new Point(260, 250),
                    true /* IsStroked */  ));
            PathFigure myPathFigure3 = new PathFigure();
            myPathFigure3.StartPoint = new Point(30, 10);
            myPathFigure3.Segments.Add(
                new BezierSegment(
                    new Point(70, 150),
                    new Point(120, 200),
                    new Point(270, 250),
                    true /* IsStroked */  ));

            /// Create a PathGeometry to contain the figure.
            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures.Add(myPathFigure1);
            myPathGeometry.Figures.Add(myPathFigure2);
            myPathGeometry.Figures.Add(myPathFigure3);

            // Display the PathGeometry. 
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 2;
            myPath.Data = myPathGeometry;

            this.Content = myPath;


        }

        public bool drawFrames(ref Grid i_Grid)
        {

            return true;
        }
    
    }
}
