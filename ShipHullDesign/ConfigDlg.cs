using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HullDesign1
{
    public partial class ConfigDlg : Form
    {
        private bool m_useDB = false;
        private string m_Host, m_User, m_Password, m_Database, m_Port;
        private string m_shipHullDesignRegPath;
        private string m_dbConnectionRegPath;
        private string m_dataFilesRegPath;
        private string m_folderPath;
        
        public ConfigDlg()
        {
            InitializeComponent();

            defineRegistryPath();
            getRegistryDBConnectionDetails();
            getRegistryUseDB();
            getRegistryDataFilesFolderPath();
            updateDialogSettings();
            groupStatusUpdate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupStatusUpdate();
            if (checkBoxUseDB.Checked)
            {
                m_useDB = true;
            }
            else
            {
                m_useDB = false;
            }
        }

         private void groupStatusUpdate()
         {
             if (checkBoxUseDB.Checked)
             {
                 groupBoxDataFiles.Enabled = false;
                 groupBoxDBConnectionSettings.Enabled = true;
              }
             else
             {
                 groupBoxDataFiles.Enabled = true;
                 groupBoxDBConnectionSettings.Enabled = false;
              }
         }

         private void buttonOK_Click(object sender, EventArgs e)
         {
            setRegistryDBConnectionDetails();
            setRegistryUseDB();
            setRegistryDataFilesFolderPath();
         }


 ///  Registry get / set

         private void getRegistryDBConnectionDetails()
         {
             m_Host = (string)Registry.GetValue(m_dbConnectionRegPath, "Host", "");
             m_User = (string)Registry.GetValue(m_dbConnectionRegPath, "User", "");
             m_Password = (string)Registry.GetValue(m_dbConnectionRegPath, "Password", "");
             m_Database = (string)Registry.GetValue(m_dbConnectionRegPath, "Database", "");
             m_Port = (string)Registry.GetValue(m_dbConnectionRegPath, "Port", "");
         }

         private void setRegistryDBConnectionDetails()
         {
 
             //get Data from Dialog:
             m_Host = this.Host.Text;
             m_User = this.User.Text;
             m_Password = this.Password.Text;
             m_Database = this.Database.Text;
             m_Port = this.Port.Text;

             //update registry with DB connection data
             Registry.SetValue(m_dbConnectionRegPath, "Host", m_Host);
             Registry.SetValue(m_dbConnectionRegPath, "User", m_User);
             Registry.SetValue(m_dbConnectionRegPath, "Password", m_Password);
             Registry.SetValue(m_dbConnectionRegPath, "Database", m_Database);
             Registry.SetValue(m_dbConnectionRegPath, "Port", m_Port);

         }


         private void updateDialogSettings()
         {
             this.Host.Text = m_Host;
             this.User.Text = m_User;
             this.Password.Text = m_Password;
             this.Database.Text = m_Database;
             this.Port.Text = m_Port;

             if (m_useDB)
             {
                 this.checkBoxUseDB.Checked = true;
             }
             else
             {
                 this.checkBoxUseDB.Checked = false;
             }

             this.textBoxDataFolderPath.Text = m_folderPath;
         }

         private void defineRegistryPath()
         {
             const string userRoot = "HKEY_CURRENT_USER";
             const string subkey1 = "Software";
             const string subkey2 = "ShipHullDesign";
             const string subkey3 = "DatabaseConnection";
             const string subkey4 = "DataFilesDefinitions";
             
             m_shipHullDesignRegPath = userRoot + "\\" + subkey1 + "\\" + subkey2;
             m_dbConnectionRegPath = userRoot + "\\" + subkey1 + "\\" + subkey2 + "\\" + subkey3;
             m_dataFilesRegPath = userRoot + "\\" + subkey1 + "\\" + subkey2 + "\\" + subkey4;
         }

         private void getRegistryUseDB()
         {
             string useDB = (string)Registry.GetValue(m_shipHullDesignRegPath, "Use Database", "");
             if ((useDB == "") || (useDB == "No"))
             {
                 m_useDB = false;
             }
             else
             {
                 m_useDB = true;
             }
         }

         private void setRegistryUseDB()
         {
             string useDB;
             if (m_useDB == false)
             {
                 useDB = "No";
             }
             else
             {
                 useDB = "Yes";
             }

             Registry.SetValue(m_shipHullDesignRegPath, "Use Database", useDB);
         }

         private void buttonSelectFolder_Click(object sender, EventArgs e)
         {
             this.folderBrowserDialog1.ShowDialog();
             m_folderPath = this.folderBrowserDialog1.SelectedPath;
             m_folderPath += "\\";
             this.textBoxDataFolderPath.Text = m_folderPath;
         }


         private void setRegistryDataFilesFolderPath()
         {
             Registry.SetValue(m_dataFilesRegPath, "Data Files Folder", m_folderPath);
         }

         private void getRegistryDataFilesFolderPath()
         {
             m_folderPath = (string)Registry.GetValue(m_dataFilesRegPath, "Data Files Folder", "");
         }


    }
}
