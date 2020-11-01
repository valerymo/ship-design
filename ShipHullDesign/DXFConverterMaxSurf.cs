using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HullDesign1
{
    class DXFConverterMaxSurf
    {
        private StreamReader m_reader;
        private TextWriter LOG;
        private string m_outFileName;
        

        Dictionary<string, List<point>> m_HullFrames = new Dictionary<string, List<point>>();
        Dictionary<string, List<Point3D>> m_HullWaterlines3D = new Dictionary<string, List<Point3D>>();
        Dictionary<string, List<Point3D>> m_HullSections3D = new Dictionary<string, List<Point3D>>();
        Dictionary<string, List<Point3D>> m_HullEdges3D = new Dictionary<string, List<Point3D>>();     
        Dictionary<string, List<point>> m_HullButtocks = new Dictionary<string, List<point>>();

        public DXFConverterMaxSurf(string dxfFile, string outputFile)
        {
            //const string dxfFile = "Patrol_boat_bodyPlan_MaxS.dxf";
            //const string dxfFile = "Balker_MaxS.dxf";
            const string logFile = "DXFConverter_log.txt";
            m_reader = new StreamReader(dxfFile);
            m_outFileName = outputFile;
            LOG = new StreamWriter(logFile);
        }

        public bool readGroup(ref int l_code, ref string l_strValue, ref double l_numValue)
        {        
                try
                {
                    string line_code;
                    string line_value;

                    if ((line_code = m_reader.ReadLine()) == null ||
                        (line_value = m_reader.ReadLine()) == null)
                    {
                        return true;
                    }

                    l_code = Convert.ToInt32(line_code);
                    if (((l_code >= 0) && (l_code <= 9)) || (l_code == 99))
                    {
                        l_strValue = line_value;
                        l_numValue = -33;
                    }
                    else if (((l_code >= 10) && (l_code <= 79)) ||
                            ((l_code >= 210) && (l_code <= 239)))
                    {
                        l_strValue = null;
                        l_numValue = Convert.ToInt32(line_value);
                    }
                }
                catch
                {
                    LOG.Write("Exception in readGroup");
                }
                LOG.Close();
                return true;
            }

 
        public bool getFramesPoints()
        {
            
            try
            {
                string line_code;
                string line_value;
                bool l_isFrame = false;

                List<point> frame = null;
                //point frame_point;

                string l_FrameNum = "";
                //bool l_newFrame = true;
                int l_code;
  
                /// reading by two lines
                while ((line_code = m_reader.ReadLine()) != null &&
                    (line_value = m_reader.ReadLine()) != null)
                { 
                    l_code = Convert.ToInt32(line_code);
                    if (l_code == 8) 
                    {
                       if (line_value.StartsWith("ST"))
                       {
                           if (line_value != l_FrameNum) //new frame
                           {
                               l_FrameNum = line_value;
                               frame = new List<point>();
                               if (!m_HullFrames.ContainsKey(line_value))
                               {
                                  m_HullFrames.Add(line_value.TrimEnd(), frame);
                               }
                           }
                               
                           l_isFrame = true;                          
                       }
                       else
                       {
                            l_isFrame = false;
                       }
                         
                    }
                    else if ((l_code == 10) && l_isFrame)            
                    {
                        point frame_point = new point();
                        frame_point.x = Convert.ToDouble(line_value.Trim());
                        line_code = m_reader.ReadLine();
                        line_value = m_reader.ReadLine();
                        l_code = Convert.ToInt32(line_code);
                        if ((l_code == 20) && l_isFrame)
                        {
                                frame_point.y = Convert.ToDouble(line_value.Trim());
                        }
                        frame.Add(frame_point);
                    }
          }

                //print m_HullFrames
                printHullFrames();
 
            }
            catch
            {
                LOG.Write("Exception in getFramesPoints");
                LOG.Close();
            }
            return true;
        }

        void printHullFrames()
        {
            //const string file = "MaxSurfFrames_OUT1.txt";
            string file = m_outFileName;
            int i = 0;
            try
            {
                using (TextWriter wr = new StreamWriter(file))
                {
                    foreach (KeyValuePair<string, List<point>> frame in m_HullFrames)
                    {
                        wr.Write(frame.Key + ": ");
                        foreach (point node in frame.Value)
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
            catch
            {
                LOG.Write("Exception in printHullFrames");
                LOG.Close();
            }
        }

        //------------------------------------//
        public bool getWaterlines3DPoints()
        {
            try
            {
                string line_code;
                string line_value;
                bool l_isWL = false;

                List<Point3D> l_waterline = null;
 
                string l_WLNum = "";
                int l_code;

                /// reading by two lines
                while ((line_code = m_reader.ReadLine()) != null &&
                    (line_value = m_reader.ReadLine()) != null)
                {
                    l_code = Convert.ToInt32(line_code);
                    if (l_code == 8)
                    {
                        if (line_value.StartsWith("WL"))
                        {
                            if (line_value != l_WLNum) //new frame
                            {
                                l_WLNum = line_value;
                                l_waterline = new List<Point3D>();
                                if (!m_HullWaterlines3D.ContainsKey(line_value))
                                {
                                    m_HullWaterlines3D.Add(line_value.TrimEnd(), l_waterline);
                                }
                            }

                            l_isWL = true;
                        }
                        else
                        {
                            l_isWL = false;
                        }

                    }
                    else if ((l_code == 10) && l_isWL)
                    {
                        Point3D l_WLPoint = new Point3D();
                        l_WLPoint.Z = Convert.ToDouble(line_value.Trim());
                        line_code = m_reader.ReadLine();
                        line_value = m_reader.ReadLine();
                        l_code = Convert.ToInt32(line_code);
                        if ((l_code == 20) && l_isWL)
                        {
                            l_WLPoint.X = Convert.ToDouble(line_value.Trim());
                        }
                        line_code = m_reader.ReadLine();
                        line_value = m_reader.ReadLine();
                        l_code = Convert.ToInt32(line_code);
                        if ((l_code == 30) && l_isWL)
                        {
                            l_WLPoint.Y = Convert.ToDouble(line_value.Trim());
                        }
                        l_waterline.Add(l_WLPoint);
                    }
                }

                printWaterlines3D();

            }
            catch
            {
                LOG.Write("Exception in getWaterlines3DPoints");
                LOG.Close();
            }
            return true;
        }

        void printWaterlines3D()
        {
            string file = m_outFileName;
            file.TrimEnd();
            file.TrimEnd();
            file.TrimEnd();
            file += "_WL.txt";

            int i = 0;
            try
            {
                using (TextWriter wr = new StreamWriter(file))
                {
                    foreach (KeyValuePair<string, List<Point3D>> frame in m_HullWaterlines3D)
                    {
                        wr.Write(frame.Key + ": ");
                        foreach (Point3D node in frame.Value)
                        {
                            wr.Write(node.X + ",");
                            wr.Write(node.Y + ",");
                            wr.Write(node.Z + ";");
                        }
                        wr.WriteLine();
                        i++;
                    }
                    // close the stream
                    wr.Close();
                }
            }
            catch
            {
                LOG.Write("Exception in printWaterlines3D");
                LOG.Close();
            }
        }

        //------------------------------------//
        public bool getSections3DPoints()
        {
            try
            {
                string line_code;
                string line_value;
                bool l_isSection = false;

                List<Point3D> l_section = null;

                string l_SectionNum = "";
                int l_code;

                //reset stream
                m_reader.BaseStream.Position = 0;
                m_reader.DiscardBufferedData(); 
                /// reading by two lines
                while ((line_code = m_reader.ReadLine()) != null &&
                    (line_value = m_reader.ReadLine()) != null)
                {
                    l_code = Convert.ToInt32(line_code);
                    if (l_code == 8)
                    {
                        if (line_value.StartsWith("ST"))
                        {
                            if (line_value != l_SectionNum) //new frame
                            {
                                l_SectionNum = line_value;
                                l_section = new List<Point3D>();
                                if (!m_HullSections3D.ContainsKey(line_value))
                                {
                                    m_HullSections3D.Add(line_value.TrimEnd(), l_section);
                                }
                            }

                            l_isSection = true;
                        }
                        else
                        {
                            l_isSection = false;
                        }

                    }
                    else if ((l_code == 10) && l_isSection)
                    {
                        Point3D l_WLPoint = new Point3D();
                        l_WLPoint.Z = Convert.ToDouble(line_value.Trim());
                        line_code = m_reader.ReadLine();
                        line_value = m_reader.ReadLine();
                        l_code = Convert.ToInt32(line_code);
                        if ((l_code == 20) && l_isSection)
                        {
                            l_WLPoint.X = Convert.ToDouble(line_value.Trim());
                        }
                        line_code = m_reader.ReadLine();
                        line_value = m_reader.ReadLine();
                        l_code = Convert.ToInt32(line_code);
                        if ((l_code == 30) && l_isSection)
                        {
                            l_WLPoint.Y = Convert.ToDouble(line_value.Trim());
                        }
                        l_section.Add(l_WLPoint);
                    }
                }

                printSections3D();

            }
            catch
            {
                LOG.Write("Exception in getSections3DPoints");
                LOG.Close();
            }
            return true;
        }

        void printSections3D()
        {
            string file = m_outFileName;
            file.TrimEnd();
            file.TrimEnd();
            file.TrimEnd();
            file += "_ST.txt";

            int i = 0;
            try
            {
                using (TextWriter wr = new StreamWriter(file))
                {
                    foreach (KeyValuePair<string, List<Point3D>> frame in m_HullSections3D)
                    {
                        wr.Write(frame.Key + ": ");
                        foreach (Point3D node in frame.Value)
                        {
                            wr.Write(node.X + ",");
                            wr.Write(node.Y + ",");
                            wr.Write(node.Z + ";");
                        }
                        wr.WriteLine();
                        i++;
                    }
                    // close the stream
                    wr.Close();
                }
            }
            catch
            {
                LOG.Write("Exception in printSections3D");
                LOG.Close();
            }
        }


        //-------------------Edges begin  --------------------------------------------------------
        
        public bool getEdges3DPoints()
        {
            try
            {
                string line_code;
                string line_value;
                bool l_isEDGES = false;

                List<Point3D> l_edge = null;

                string l_EdgeNum = "";
                int l_code;

                //-----------------------------------
                m_reader.BaseStream.Position = 0;
                m_reader.DiscardBufferedData(); 
                /// reading by two lines
                while ((line_code = m_reader.ReadLine()) != null &&
                    (line_value = m_reader.ReadLine()) != null)
                {
                    l_code = Convert.ToInt32(line_code);
                    if (l_code == 8)
                    {
                        if (line_value.StartsWith("EDGES"))
                        {
                            if (line_value != l_EdgeNum) //new frame
                            {
                                l_EdgeNum = line_value;
                                l_edge = new List<Point3D>();
                                if (!m_HullEdges3D.ContainsKey(line_value))
                                {
                                    m_HullEdges3D.Add(line_value.TrimEnd(), l_edge);
                                }
                            }

                            l_isEDGES = true;
                        }
                        else
                        {
                            l_isEDGES = false;
                        }

                    }
                    else if ((l_code == 10) && l_isEDGES)
                    {
                        Point3D l_EDGESPoint = new Point3D();
                        l_EDGESPoint.Z = Convert.ToDouble(line_value.Trim());
                        line_code = m_reader.ReadLine();
                        line_value = m_reader.ReadLine();
                        l_code = Convert.ToInt32(line_code);
                        if ((l_code == 20) && l_isEDGES)
                        {
                            l_EDGESPoint.X = Convert.ToDouble(line_value.Trim());
                        }
                        line_code = m_reader.ReadLine();
                        line_value = m_reader.ReadLine();
                        l_code = Convert.ToInt32(line_code);
                        if ((l_code == 30) && l_isEDGES)
                        {
                            l_EDGESPoint.Y = Convert.ToDouble(line_value.Trim());
                        }
                        l_edge.Add(l_EDGESPoint);
                    }
                }

                printEdges3D();

            }
            catch
            {
                LOG.Write("Exception in getEdges3DPoints");
                LOG.Close();
            }
            return true;
        }

        void printEdges3D()
        {
            string file = m_outFileName;
            file.TrimEnd();
            file.TrimEnd();
            file.TrimEnd();
            file += "_EDGES.txt";

            int i = 0;
            try
            {
                using (TextWriter wr = new StreamWriter(file))
                {
                    foreach (KeyValuePair<string, List<Point3D>> frame in m_HullEdges3D)
                    {
                        wr.Write(frame.Key + ": ");
                        foreach (Point3D node in frame.Value)
                        {
                            wr.Write(node.X + ",");
                            wr.Write(node.Y + ",");
                            wr.Write(node.Z + ";");
                        }
                        wr.WriteLine();
                        i++;
                    }
                    // close the stream
                    wr.Close();
                }
            }
            catch
            {
                LOG.Write("Exception in printEdges3D");
                LOG.Close();
            }
        }

        //-------------------------- Edges end

    } // class DXFConverterMaxSurf 
} // namespace HullDesign1
