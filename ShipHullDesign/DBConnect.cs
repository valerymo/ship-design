using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Data.SqlClient;

namespace HullDesign1
{
    class DBConnect
    {
        private SqlConnection connection;
        private static volatile DBConnect instance;
        private static object syncRoot = new Object();

        private string server;
        private string database;
        private string uid;
        private string password;
        private string port;

        //Constructor
        private DBConnect()
        {
            Initialize();
        }

        public static DBConnect Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DBConnect();
                    }
                }
                return instance;
            }
        }

        //Initialize values
        private void Initialize()
        {
            //DBConnectionConfig();
       
            //server = "10.0.0.2";
            //database = "FLEETDB1";
            //uid = "FLEET";
            //password = "FLEET";
            //port = "3306";


            DBConnectionDetails dbConnectDetails = new DBConnectionDetails();
            getDBConnectionDetailsFromRegistry(ref dbConnectDetails);

            server = dbConnectDetails.Host;
            uid = dbConnectDetails.User;
            password = dbConnectDetails.Password;
            database = dbConnectDetails.Database;
            port = dbConnectDetails.Port;

            string connectionString;
            connectionString = "server=" + server + ";" + "database=" +
            database + ";" + "Port=" + port + ";" + "User ID=" + uid + ";" + "Pwd=" + password + ";";

            connection = new SqlConnection(connectionString);
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                    default:
                        MessageBox.Show("Anknown connection error");
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //-----------------------------------------------------------------
        //Count
        public int Count()
        {
            string query = "SELECT Count(*) FROM ShipsHullData";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                SqlCommand cmd = new SqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();
            }

            return Count;

        }
        //-----------------------------------------------------------------

        //Count
        public string getSectionsFileString(string i_sShipName)
        {
            string query = String.Format("select hull_file from ShipsHullData where SHIP_NAME = '{0}'", i_sShipName);
            string sSectionsFileString = "";

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                SqlCommand cmd = new SqlCommand(query, connection);

                //ExecuteScalar will return one value
                sSectionsFileString = (string)cmd.ExecuteScalar();

                //close Connection
                this.CloseConnection();
            }

            return sSectionsFileString;

        }
        //-----------------------------------------------------------------

        private void DBConnectionConfig()
        {

            // construct a new customer dialog
            ConfigDlg dbConfigDialog = new ConfigDlg();
            // show the modal dialog until the AcceptButton (OK) or CancelButton(Cancel) is pressed
 
            DBConnectionDetails dbConnectDetails = new DBConnectionDetails();
            getDBConnectionDetailsFromRegistry(ref dbConnectDetails);

            dbConfigDialog.Host.Text = dbConnectDetails.Host;
            dbConfigDialog.User.Text = dbConnectDetails.User;
            dbConfigDialog.Password.Text = dbConnectDetails.Password;
            dbConfigDialog.Database.Text = dbConnectDetails.Database;
            dbConfigDialog.Port.Text = dbConnectDetails.Port;

            if (dbConfigDialog.ShowDialog() == DialogResult.OK)
            {
                // write Dialog data to Registry
                
                dbConnectDetails.Host = dbConfigDialog.Host.Text;
                dbConnectDetails.User = dbConfigDialog.User.Text;
                dbConnectDetails.Password = dbConfigDialog.Password.Text;
                dbConnectDetails.Database = dbConfigDialog.Database.Text;
                dbConnectDetails.Port = dbConfigDialog.Port.Text;

                setDBConnectionDetailsToRegistry(dbConnectDetails);
            }


        }

        //-----------------------------------------------------------------

        private struct DBConnectionDetails
        {
            public string Host, User, Password, Database, Port;
            public DBConnectionDetails (string Host, string User, string Password, 
                                        string Database, string Port)
            {
                this.Host = Host;
                this.User = User;
                this.Password = Password;
                this.Database = Database;
                this.Port = Port;
            }
        }

        private bool getDBConnectionDetailsFromRegistry( ref DBConnectionDetails o_dbConnectDetails)
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey1 = "Software";
            const string subkey2 = "ShipHullDesign";
            const string subkey3 = "DatabaseConnection";
            const string keyName = userRoot + "\\" + subkey1 + "\\" + subkey2 + "\\" + subkey3 ;

            o_dbConnectDetails.Host = (string)Registry.GetValue(keyName, "Host","");
            o_dbConnectDetails.User = (string)Registry.GetValue(keyName, "User", "");
            o_dbConnectDetails.Password = (string)Registry.GetValue(keyName, "Password", "");
            o_dbConnectDetails.Database = (string)Registry.GetValue(keyName, "Database", "");
            o_dbConnectDetails.Port = (string)Registry.GetValue(keyName, "Port", "");

            if ((o_dbConnectDetails.Host == "")
                || (o_dbConnectDetails.User == "")
                || (o_dbConnectDetails.Password == "")
                || (o_dbConnectDetails.Database == "")
                || (o_dbConnectDetails.Port == ""))
            {
                return false;
            }
            
            return true;
        }

        private bool setDBConnectionDetailsToRegistry(DBConnectionDetails dbConnectDetails)
        {
                const string userRoot = "HKEY_CURRENT_USER";
                const string subkey1 = "Software";
                const string subkey2 = "ShipHullDesign";
                const string subkey3 = "DatabaseConnection";
                const string keyName = userRoot + "\\" + subkey1 + "\\" + subkey2 + "\\" + subkey3;

                Registry.SetValue(keyName, "Host", dbConnectDetails.Host);
                Registry.SetValue(keyName, "User", dbConnectDetails.User);
                Registry.SetValue(keyName, "Password", dbConnectDetails.Password);
                Registry.SetValue(keyName, "Database", dbConnectDetails.Database);
                Registry.SetValue(keyName, "Port", dbConnectDetails.Port);
 
                return true;
        }

        //-----------------------------------------------------------------



        //-----------------------------------------------------------------

    }
}
