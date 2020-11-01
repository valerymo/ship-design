using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace HullDesign1
{
    public sealed class Config
    {
        private static volatile Config instance;
        private static object syncRoot = new Object();

        private string m_dataFilesRegPath;
        private string m_folderPath;

        private Config() 
        {
            defineRegistryPath();
            getRegistryData();
        }

        public static Config Instance
        {
           get 
           {
             if (instance == null) 
             {
                lock (syncRoot) 
                {
                   if (instance == null)
                       instance = new Config();
                }
             }
             return instance;
           }
        }
        
        private void defineRegistryPath()
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey1 = "Software";
            const string subkey2 = "ShipHullDesign";
            //const string subkey3 = "DatabaseConnection";
            const string subkey4 = "DataFilesDefinitions";

            m_dataFilesRegPath = userRoot + "\\" + subkey1 + "\\" + subkey2 + "\\" + subkey4;
        }

        private void getRegistryData()
        {
            m_folderPath = (string)Registry.GetValue(m_dataFilesRegPath, "Data Files Folder", "");
        }

        public string getDataFilesFolderPath()
        {
            return m_folderPath;
        }

     }
}
