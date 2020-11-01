using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace HullDesign1
{
    class StaticaLargeAnglesOverturnMoment
    {
        private static volatile StaticaLargeAnglesOverturnMoment instance;
        private static object syncRoot = new Object();
        private StaticaLargeAnglesOverturnMoment()
        {
        }
        public static StaticaLargeAnglesOverturnMoment Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new StaticaLargeAnglesOverturnMoment();
                    }
                }
                return instance;
            }
        }

/*------------------------------- members ---------------------------------------------*/
        double m_pitchingAmplitudeAngle;
        Dictionary<int, double> m_basicStaticStabililtyArms = new Dictionary<int, double>();
        Dictionary<int, double> m_extendedStaticStabililtyArms = new Dictionary<int, double>();
/*-------------------------------------------------------------------------------------*/


        private void init(ref Dictionary<int, double> basicStaticStabililtyArms)
        {
            m_pitchingAmplitudeAngle = 30;
            m_basicStaticStabililtyArms = basicStaticStabililtyArms;

        }

        private void extendStaticStabilityDiagram(double pitchingAmplitudeAngle, Dictionary<int, double> basicStaticStabililtyArms)
        {
            m_extendedStaticStabililtyArms.Clear();
            foreach (KeyValuePair<int, double> node in basicStaticStabililtyArms)
            {
                if (node.Key > pitchingAmplitudeAngle)
                    break;

                m_extendedStaticStabililtyArms.Add(-node.Key, -node.Value);

            }
            
        }

        public void OverturnMoment(double pitchingAmplitudeAngle, ref Dictionary<int, double> basicStaticStabililtyArms, ref Canvas canvas)
        {
            extendStaticStabilityDiagram(pitchingAmplitudeAngle, basicStaticStabililtyArms);
            drawExtendedStaticStabililtyArms(ref canvas);
        }

        private void drawExtendedStaticStabililtyArms(ref Canvas canvas)
        {
            double scaleX = 4;
            double scaleY = -0.3;
            double move_X = -10;
            double move_Y = 200;
            System.Windows.Media.SolidColorBrush color = System.Windows.Media.Brushes.MediumBlue;
            double thick = 2;
            System.Windows.Media.SolidColorBrush colorCoord = System.Windows.Media.Brushes.Black;
            double thickCoord = 1;

            //StaticaLargeAngles.Instance.drawStabililtyArms(m_basicStaticStabililtyArms, canvas, 
            //                                                0, scaleY, 0, 0,
            //                                               color, colorCoord, thick, thickCoord);
            StaticaLargeAngles.Instance.drawStabililtyArms(m_extendedStaticStabililtyArms, canvas,
                                                            scaleX, scaleY, move_X, move_Y,
                                                            color, colorCoord, thick, thickCoord);
        }
    }
}
