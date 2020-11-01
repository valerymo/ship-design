namespace HullDesign1
{
    partial class ConfigDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Host = new System.Windows.Forms.TextBox();
            this.User = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.Database = new System.Windows.Forms.TextBox();
            this.Port = new System.Windows.Forms.TextBox();
            this.MyCancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBoxDBConnectionSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxUseDB = new System.Windows.Forms.CheckBox();
            this.groupBoxDataFiles = new System.Windows.Forms.GroupBox();
            this.textBoxDataFolderPath = new System.Windows.Forms.TextBox();
            this.buttonSelectFolder = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBoxDBConnectionSettings.SuspendLayout();
            this.groupBoxDataFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // Host
            // 
            this.Host.AccessibleName = "";
            this.Host.Location = new System.Drawing.Point(110, 23);
            this.Host.Name = "Host";
            this.Host.Size = new System.Drawing.Size(143, 20);
            this.Host.TabIndex = 1;
            // 
            // User
            // 
            this.User.Location = new System.Drawing.Point(110, 58);
            this.User.Name = "User";
            this.User.Size = new System.Drawing.Size(143, 20);
            this.User.TabIndex = 2;
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(110, 99);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(143, 20);
            this.Password.TabIndex = 3;
            // 
            // Database
            // 
            this.Database.Location = new System.Drawing.Point(110, 137);
            this.Database.Name = "Database";
            this.Database.Size = new System.Drawing.Size(143, 20);
            this.Database.TabIndex = 4;
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(110, 173);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(143, 20);
            this.Port.TabIndex = 5;
            // 
            // MyCancelButton
            // 
            this.MyCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.MyCancelButton.Location = new System.Drawing.Point(232, 410);
            this.MyCancelButton.Name = "MyCancelButton";
            this.MyCancelButton.Size = new System.Drawing.Size(75, 23);
            this.MyCancelButton.TabIndex = 7;
            this.MyCancelButton.Text = "Cancel";
            this.MyCancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "User:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Database:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 177);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Port:";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(42, 410);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 13;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBoxDBConnectionSettings
            // 
            this.groupBoxDBConnectionSettings.Controls.Add(this.label5);
            this.groupBoxDBConnectionSettings.Controls.Add(this.label4);
            this.groupBoxDBConnectionSettings.Controls.Add(this.label3);
            this.groupBoxDBConnectionSettings.Controls.Add(this.label2);
            this.groupBoxDBConnectionSettings.Controls.Add(this.label1);
            this.groupBoxDBConnectionSettings.Controls.Add(this.Port);
            this.groupBoxDBConnectionSettings.Controls.Add(this.Database);
            this.groupBoxDBConnectionSettings.Controls.Add(this.Password);
            this.groupBoxDBConnectionSettings.Controls.Add(this.User);
            this.groupBoxDBConnectionSettings.Controls.Add(this.Host);
            this.groupBoxDBConnectionSettings.Location = new System.Drawing.Point(24, 61);
            this.groupBoxDBConnectionSettings.Name = "groupBoxDBConnectionSettings";
            this.groupBoxDBConnectionSettings.Size = new System.Drawing.Size(283, 206);
            this.groupBoxDBConnectionSettings.TabIndex = 14;
            this.groupBoxDBConnectionSettings.TabStop = false;
            this.groupBoxDBConnectionSettings.Text = "Data Base Connection Settings";
            // 
            // checkBoxUseDB
            // 
            this.checkBoxUseDB.AutoSize = true;
            this.checkBoxUseDB.Location = new System.Drawing.Point(24, 24);
            this.checkBoxUseDB.Name = "checkBoxUseDB";
            this.checkBoxUseDB.Size = new System.Drawing.Size(98, 17);
            this.checkBoxUseDB.TabIndex = 16;
            this.checkBoxUseDB.Text = "Use Data Base";
            this.checkBoxUseDB.UseVisualStyleBackColor = true;
            this.checkBoxUseDB.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBoxDataFiles
            // 
            this.groupBoxDataFiles.Controls.Add(this.textBoxDataFolderPath);
            this.groupBoxDataFiles.Controls.Add(this.buttonSelectFolder);
            this.groupBoxDataFiles.Controls.Add(this.label6);
            this.groupBoxDataFiles.Location = new System.Drawing.Point(24, 286);
            this.groupBoxDataFiles.Name = "groupBoxDataFiles";
            this.groupBoxDataFiles.Size = new System.Drawing.Size(282, 99);
            this.groupBoxDataFiles.TabIndex = 17;
            this.groupBoxDataFiles.TabStop = false;
            this.groupBoxDataFiles.Text = "Data Files";
            // 
            // textBoxDataFolderPath
            // 
            this.textBoxDataFolderPath.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxDataFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDataFolderPath.ForeColor = System.Drawing.Color.Black;
            this.textBoxDataFolderPath.Location = new System.Drawing.Point(17, 38);
            this.textBoxDataFolderPath.Name = "textBoxDataFolderPath";
            this.textBoxDataFolderPath.Size = new System.Drawing.Size(236, 13);
            this.textBoxDataFolderPath.TabIndex = 2;
            //this.textBoxDataFolderPath.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // buttonSelectFolder
            // 
            this.buttonSelectFolder.Location = new System.Drawing.Point(17, 64);
            this.buttonSelectFolder.Name = "buttonSelectFolder";
            this.buttonSelectFolder.Size = new System.Drawing.Size(91, 23);
            this.buttonSelectFolder.TabIndex = 1;
            this.buttonSelectFolder.Text = "Select Folder ...";
            this.buttonSelectFolder.UseVisualStyleBackColor = true;
            this.buttonSelectFolder.Click += new System.EventHandler(this.buttonSelectFolder_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Current Location:";
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.MyCancelButton;
            this.ClientSize = new System.Drawing.Size(325, 444);
            this.Controls.Add(this.groupBoxDataFiles);
            this.Controls.Add(this.checkBoxUseDB);
            this.Controls.Add(this.groupBoxDBConnectionSettings);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.MyCancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Configuration";
            this.Text = "Configuration";
            this.groupBoxDBConnectionSettings.ResumeLayout(false);
            this.groupBoxDBConnectionSettings.PerformLayout();
            this.groupBoxDataFiles.ResumeLayout(false);
            this.groupBoxDataFiles.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button MyCancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox Host;
        public System.Windows.Forms.TextBox User;
        public System.Windows.Forms.TextBox Password;
        public System.Windows.Forms.TextBox Database;
        public System.Windows.Forms.TextBox Port;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBoxDBConnectionSettings;
        private System.Windows.Forms.CheckBox checkBoxUseDB;
        private System.Windows.Forms.GroupBox groupBoxDataFiles;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDataFolderPath;
        private System.Windows.Forms.Button buttonSelectFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}