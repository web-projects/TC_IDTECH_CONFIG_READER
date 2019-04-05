namespace IDTechConfigReader
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.ApplicationtabPage = new System.Windows.Forms.TabPage();
            this.ConfigurationtabPage = new System.Windows.Forms.TabPage();
            this.ConfigurationPanel2 = new System.Windows.Forms.Panel();
            this.ConfigurationExpandButton = new System.Windows.Forms.Button();
            this.tabControlConfiguration = new System.Windows.Forms.TabControl();
            this.ConfigurationTerminalDatatabPage = new System.Windows.Forms.TabPage();
            this.ConfigurationTerminalDatapicBoxWait = new System.Windows.Forms.PictureBox();
            this.ConfigurationTerminalDatalistView = new System.Windows.Forms.ListView();
            this.ConfigurationTerminalDatacolumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationTerminalDatacolumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationAIDStabPage = new System.Windows.Forms.TabPage();
            this.ConfigurationAIDSpicBoxWait = new System.Windows.Forms.PictureBox();
            this.ConfigurationAIDSlistView = new System.Windows.Forms.ListView();
            this.ConfigurationAIDStabPageColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationAIDStabPageColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationCAPKStabPage = new System.Windows.Forms.TabPage();
            this.ConfigurationCAPKSpicBoxWait = new System.Windows.Forms.PictureBox();
            this.ConfigurationCAPKSlistView = new System.Windows.Forms.ListView();
            this.ConfigurationCAPKStabPageColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationCAPKStabPageColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationGROUPStabPage = new System.Windows.Forms.TabPage();
            this.ConfigurationGROUPSpicBoxWait = new System.Windows.Forms.PictureBox();
            this.ConfigurationGROUPSConfigurationPanel1label1 = new System.Windows.Forms.Label();
            this.ConfigurationGROUPStabPagecomboBox1 = new System.Windows.Forms.ComboBox();
            this.ConfigurationGROUPSlistView = new System.Windows.Forms.ListView();
            this.ConfigurationGROUPStabPageColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationGROUPStabPageColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationGROUPStabPageColumnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConfigurationPanel1 = new System.Windows.Forms.Panel();
            this.ConfigurationPanel1pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ConfigurationPanel1label2 = new System.Windows.Forms.Label();
            this.ConfigurationPanel1btnDeviceMode = new System.Windows.Forms.Button();
            this.ConfigurationPanel1btnEMVMode = new System.Windows.Forms.Button();
            this.ConfigurationPanel1label1 = new System.Windows.Forms.Label();
            this.ConfigurationPanel1btnFactoryReset = new System.Windows.Forms.Button();
            this.ConfigurationCollapseButton = new System.Windows.Forms.Button();
            this.ConfigurationSplitter1 = new System.Windows.Forms.Splitter();
            this.ConfigurationGroupBox1 = new System.Windows.Forms.GroupBox();
            this.radioLoadFromDevice = new System.Windows.Forms.RadioButton();
            this.radioLoadFromFile = new System.Windows.Forms.RadioButton();
            this.FirmwaretabPage = new System.Windows.Forms.TabPage();
            this.lblFirmwareVersion = new System.Windows.Forms.Label();
            this.btnFirmwareUpdate = new System.Windows.Forms.Button();
            this.FirmwareConfigurationPanel1label1 = new System.Windows.Forms.Label();
            this.FirmwareprogressBar1 = new System.Windows.Forms.ProgressBar();
            this.ApplicationpicBoxWait = new System.Windows.Forms.PictureBox();
            this.FirmwareopenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControlMain.SuspendLayout();
            this.ConfigurationtabPage.SuspendLayout();
            this.ConfigurationPanel2.SuspendLayout();
            this.tabControlConfiguration.SuspendLayout();
            this.ConfigurationTerminalDatatabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationTerminalDatapicBoxWait)).BeginInit();
            this.ConfigurationAIDStabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationAIDSpicBoxWait)).BeginInit();
            this.ConfigurationCAPKStabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationCAPKSpicBoxWait)).BeginInit();
            this.ConfigurationGROUPStabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationGROUPSpicBoxWait)).BeginInit();
            this.ConfigurationPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationPanel1pictureBox1)).BeginInit();
            this.ConfigurationGroupBox1.SuspendLayout();
            this.FirmwaretabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ApplicationpicBoxWait)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.ApplicationtabPage);
            this.tabControlMain.Controls.Add(this.ConfigurationtabPage);
            this.tabControlMain.Controls.Add(this.FirmwaretabPage);
            this.tabControlMain.Location = new System.Drawing.Point(12, 53);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(830, 607);
            this.tabControlMain.TabIndex = 0;
            this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // ApplicationtabPage
            // 
            this.ApplicationtabPage.Location = new System.Drawing.Point(4, 22);
            this.ApplicationtabPage.Name = "ApplicationtabPage";
            this.ApplicationtabPage.Size = new System.Drawing.Size(822, 581);
            this.ApplicationtabPage.TabIndex = 1;
            this.ApplicationtabPage.Text = "Application";
            this.ApplicationtabPage.UseVisualStyleBackColor = true;
            // 
            // ConfigurationtabPage
            // 
            this.ConfigurationtabPage.Controls.Add(this.ConfigurationPanel2);
            this.ConfigurationtabPage.Controls.Add(this.ConfigurationPanel1);
            this.ConfigurationtabPage.Location = new System.Drawing.Point(4, 22);
            this.ConfigurationtabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationtabPage.Name = "ConfigurationtabPage";
            this.ConfigurationtabPage.Padding = new System.Windows.Forms.Padding(2);
            this.ConfigurationtabPage.Size = new System.Drawing.Size(822, 581);
            this.ConfigurationtabPage.TabIndex = 1;
            this.ConfigurationtabPage.Text = "Configuration";
            this.ConfigurationtabPage.UseVisualStyleBackColor = true;
            // 
            // ConfigurationPanel2
            // 
            this.ConfigurationPanel2.BackColor = System.Drawing.Color.Transparent;
            this.ConfigurationPanel2.Controls.Add(this.ConfigurationExpandButton);
            this.ConfigurationPanel2.Controls.Add(this.tabControlConfiguration);
            this.ConfigurationPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConfigurationPanel2.Location = new System.Drawing.Point(352, 2);
            this.ConfigurationPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationPanel2.Name = "ConfigurationPanel2";
            this.ConfigurationPanel2.Size = new System.Drawing.Size(468, 577);
            this.ConfigurationPanel2.TabIndex = 1;
            this.ConfigurationPanel2.Visible = false;
            // 
            // ConfigurationExpandButton
            // 
            this.ConfigurationExpandButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ConfigurationExpandButton.Location = new System.Drawing.Point(445, 0);
            this.ConfigurationExpandButton.Margin = new System.Windows.Forms.Padding(0);
            this.ConfigurationExpandButton.Name = "ConfigurationExpandButton";
            this.ConfigurationExpandButton.Size = new System.Drawing.Size(27, 579);
            this.ConfigurationExpandButton.TabIndex = 1;
            this.ConfigurationExpandButton.Text = ">>";
            this.ConfigurationExpandButton.UseVisualStyleBackColor = true;
            this.ConfigurationExpandButton.Visible = false;
            this.ConfigurationExpandButton.Click += new System.EventHandler(this.OnExpandConfigurationView);
            // 
            // tabControlConfiguration
            // 
            this.tabControlConfiguration.Controls.Add(this.ConfigurationTerminalDatatabPage);
            this.tabControlConfiguration.Controls.Add(this.ConfigurationAIDStabPage);
            this.tabControlConfiguration.Controls.Add(this.ConfigurationCAPKStabPage);
            this.tabControlConfiguration.Controls.Add(this.ConfigurationGROUPStabPage);
            this.tabControlConfiguration.Location = new System.Drawing.Point(20, 15);
            this.tabControlConfiguration.Margin = new System.Windows.Forms.Padding(2);
            this.tabControlConfiguration.Name = "tabControlConfiguration";
            this.tabControlConfiguration.SelectedIndex = 0;
            this.tabControlConfiguration.Size = new System.Drawing.Size(379, 522);
            this.tabControlConfiguration.TabIndex = 0;
            this.tabControlConfiguration.SelectedIndexChanged += new System.EventHandler(this.OnConfigurationListItemSelectedIndexChanged);
            this.tabControlConfiguration.VisibleChanged += new System.EventHandler(this.OnConfigurationTabControlVisibilityChanged);
            // 
            // ConfigurationTerminalDatatabPage
            // 
            this.ConfigurationTerminalDatatabPage.Controls.Add(this.ConfigurationTerminalDatapicBoxWait);
            this.ConfigurationTerminalDatatabPage.Controls.Add(this.ConfigurationTerminalDatalistView);
            this.ConfigurationTerminalDatatabPage.Location = new System.Drawing.Point(4, 22);
            this.ConfigurationTerminalDatatabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationTerminalDatatabPage.Name = "ConfigurationTerminalDatatabPage";
            this.ConfigurationTerminalDatatabPage.Size = new System.Drawing.Size(371, 496);
            this.ConfigurationTerminalDatatabPage.TabIndex = 3;
            this.ConfigurationTerminalDatatabPage.Text = "Terminal Data";
            this.ConfigurationTerminalDatatabPage.UseVisualStyleBackColor = true;
            // 
            // ConfigurationTerminalDatapicBoxWait
            // 
            this.ConfigurationTerminalDatapicBoxWait.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigurationTerminalDatapicBoxWait.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ConfigurationTerminalDatapicBoxWait.Image = ((System.Drawing.Image)(resources.GetObject("ConfigurationTerminalDatapicBoxWait.Image")));
            this.ConfigurationTerminalDatapicBoxWait.Location = new System.Drawing.Point(2, 0);
            this.ConfigurationTerminalDatapicBoxWait.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationTerminalDatapicBoxWait.Name = "ConfigurationTerminalDatapicBoxWait";
            this.ConfigurationTerminalDatapicBoxWait.Size = new System.Drawing.Size(367, 496);
            this.ConfigurationTerminalDatapicBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ConfigurationTerminalDatapicBoxWait.TabIndex = 4;
            this.ConfigurationTerminalDatapicBoxWait.TabStop = false;
            this.ConfigurationTerminalDatapicBoxWait.Visible = false;
            this.ConfigurationTerminalDatapicBoxWait.WaitOnLoad = true;
            // 
            // ConfigurationTerminalDatalistView
            // 
            this.ConfigurationTerminalDatalistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ConfigurationTerminalDatacolumnHeader1,
            this.ConfigurationTerminalDatacolumnHeader2});
            this.ConfigurationTerminalDatalistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConfigurationTerminalDatalistView.FullRowSelect = true;
            this.ConfigurationTerminalDatalistView.GridLines = true;
            this.ConfigurationTerminalDatalistView.Location = new System.Drawing.Point(0, 0);
            this.ConfigurationTerminalDatalistView.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationTerminalDatalistView.Name = "ConfigurationTerminalDatalistView";
            this.ConfigurationTerminalDatalistView.Size = new System.Drawing.Size(371, 496);
            this.ConfigurationTerminalDatalistView.TabIndex = 1;
            this.ConfigurationTerminalDatalistView.UseCompatibleStateImageBehavior = false;
            this.ConfigurationTerminalDatalistView.View = System.Windows.Forms.View.Details;
            // 
            // ConfigurationTerminalDatacolumnHeader1
            // 
            this.ConfigurationTerminalDatacolumnHeader1.Text = "TAG";
            // 
            // ConfigurationTerminalDatacolumnHeader2
            // 
            this.ConfigurationTerminalDatacolumnHeader2.Text = "VALUE";
            // 
            // ConfigurationAIDStabPage
            // 
            this.ConfigurationAIDStabPage.Controls.Add(this.ConfigurationAIDSpicBoxWait);
            this.ConfigurationAIDStabPage.Controls.Add(this.ConfigurationAIDSlistView);
            this.ConfigurationAIDStabPage.Location = new System.Drawing.Point(4, 22);
            this.ConfigurationAIDStabPage.Margin = new System.Windows.Forms.Padding(1);
            this.ConfigurationAIDStabPage.Name = "ConfigurationAIDStabPage";
            this.ConfigurationAIDStabPage.Size = new System.Drawing.Size(371, 496);
            this.ConfigurationAIDStabPage.TabIndex = 0;
            this.ConfigurationAIDStabPage.Text = "AIDS";
            this.ConfigurationAIDStabPage.UseVisualStyleBackColor = true;
            // 
            // ConfigurationAIDSpicBoxWait
            // 
            this.ConfigurationAIDSpicBoxWait.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigurationAIDSpicBoxWait.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ConfigurationAIDSpicBoxWait.Image = ((System.Drawing.Image)(resources.GetObject("ConfigurationAIDSpicBoxWait.Image")));
            this.ConfigurationAIDSpicBoxWait.Location = new System.Drawing.Point(2, 2);
            this.ConfigurationAIDSpicBoxWait.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationAIDSpicBoxWait.Name = "ConfigurationAIDSpicBoxWait";
            this.ConfigurationAIDSpicBoxWait.Size = new System.Drawing.Size(367, 494);
            this.ConfigurationAIDSpicBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ConfigurationAIDSpicBoxWait.TabIndex = 4;
            this.ConfigurationAIDSpicBoxWait.TabStop = false;
            this.ConfigurationAIDSpicBoxWait.Visible = false;
            this.ConfigurationAIDSpicBoxWait.WaitOnLoad = true;
            // 
            // ConfigurationAIDSlistView
            // 
            this.ConfigurationAIDSlistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ConfigurationAIDStabPageColumnHeader1,
            this.ConfigurationAIDStabPageColumnHeader2});
            this.ConfigurationAIDSlistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConfigurationAIDSlistView.GridLines = true;
            this.ConfigurationAIDSlistView.Location = new System.Drawing.Point(0, 0);
            this.ConfigurationAIDSlistView.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationAIDSlistView.Name = "ConfigurationAIDSlistView";
            this.ConfigurationAIDSlistView.Size = new System.Drawing.Size(371, 496);
            this.ConfigurationAIDSlistView.TabIndex = 0;
            this.ConfigurationAIDSlistView.UseCompatibleStateImageBehavior = false;
            this.ConfigurationAIDSlistView.View = System.Windows.Forms.View.Details;
            // 
            // ConfigurationAIDStabPageColumnHeader1
            // 
            this.ConfigurationAIDStabPageColumnHeader1.Text = "TAG";
            // 
            // ConfigurationAIDStabPageColumnHeader2
            // 
            this.ConfigurationAIDStabPageColumnHeader2.Text = "VALUE";
            // 
            // ConfigurationCAPKStabPage
            // 
            this.ConfigurationCAPKStabPage.Controls.Add(this.ConfigurationCAPKSpicBoxWait);
            this.ConfigurationCAPKStabPage.Controls.Add(this.ConfigurationCAPKSlistView);
            this.ConfigurationCAPKStabPage.Location = new System.Drawing.Point(4, 22);
            this.ConfigurationCAPKStabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationCAPKStabPage.Name = "ConfigurationCAPKStabPage";
            this.ConfigurationCAPKStabPage.Padding = new System.Windows.Forms.Padding(2);
            this.ConfigurationCAPKStabPage.Size = new System.Drawing.Size(371, 496);
            this.ConfigurationCAPKStabPage.TabIndex = 1;
            this.ConfigurationCAPKStabPage.Text = "CAPKS";
            this.ConfigurationCAPKStabPage.UseVisualStyleBackColor = true;
            // 
            // ConfigurationCAPKSpicBoxWait
            // 
            this.ConfigurationCAPKSpicBoxWait.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigurationCAPKSpicBoxWait.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ConfigurationCAPKSpicBoxWait.Image = ((System.Drawing.Image)(resources.GetObject("ConfigurationCAPKSpicBoxWait.Image")));
            this.ConfigurationCAPKSpicBoxWait.Location = new System.Drawing.Point(2, 2);
            this.ConfigurationCAPKSpicBoxWait.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationCAPKSpicBoxWait.Name = "ConfigurationCAPKSpicBoxWait";
            this.ConfigurationCAPKSpicBoxWait.Size = new System.Drawing.Size(367, 494);
            this.ConfigurationCAPKSpicBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ConfigurationCAPKSpicBoxWait.TabIndex = 4;
            this.ConfigurationCAPKSpicBoxWait.TabStop = false;
            this.ConfigurationCAPKSpicBoxWait.Visible = false;
            this.ConfigurationCAPKSpicBoxWait.WaitOnLoad = true;
            // 
            // ConfigurationCAPKSlistView
            // 
            this.ConfigurationCAPKSlistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ConfigurationCAPKStabPageColumnHeader1,
            this.ConfigurationCAPKStabPageColumnHeader2});
            this.ConfigurationCAPKSlistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConfigurationCAPKSlistView.GridLines = true;
            this.ConfigurationCAPKSlistView.Location = new System.Drawing.Point(2, 2);
            this.ConfigurationCAPKSlistView.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationCAPKSlistView.Name = "ConfigurationCAPKSlistView";
            this.ConfigurationCAPKSlistView.Size = new System.Drawing.Size(367, 492);
            this.ConfigurationCAPKSlistView.TabIndex = 1;
            this.ConfigurationCAPKSlistView.UseCompatibleStateImageBehavior = false;
            this.ConfigurationCAPKSlistView.View = System.Windows.Forms.View.Details;
            // 
            // ConfigurationCAPKStabPageColumnHeader1
            // 
            this.ConfigurationCAPKStabPageColumnHeader1.Text = "TAG";
            // 
            // ConfigurationCAPKStabPageColumnHeader2
            // 
            this.ConfigurationCAPKStabPageColumnHeader2.Text = "VALUE";
            // 
            // ConfigurationGROUPStabPage
            // 
            this.ConfigurationGROUPStabPage.Controls.Add(this.ConfigurationGROUPSpicBoxWait);
            this.ConfigurationGROUPStabPage.Controls.Add(this.ConfigurationGROUPSConfigurationPanel1label1);
            this.ConfigurationGROUPStabPage.Controls.Add(this.ConfigurationGROUPStabPagecomboBox1);
            this.ConfigurationGROUPStabPage.Controls.Add(this.ConfigurationGROUPSlistView);
            this.ConfigurationGROUPStabPage.Location = new System.Drawing.Point(4, 22);
            this.ConfigurationGROUPStabPage.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationGROUPStabPage.Name = "ConfigurationGROUPStabPage";
            this.ConfigurationGROUPStabPage.Size = new System.Drawing.Size(371, 496);
            this.ConfigurationGROUPStabPage.TabIndex = 2;
            this.ConfigurationGROUPStabPage.Text = "GROUPS";
            this.ConfigurationGROUPStabPage.UseVisualStyleBackColor = true;
            // 
            // ConfigurationGROUPSpicBoxWait
            // 
            this.ConfigurationGROUPSpicBoxWait.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigurationGROUPSpicBoxWait.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ConfigurationGROUPSpicBoxWait.Image = ((System.Drawing.Image)(resources.GetObject("ConfigurationGROUPSpicBoxWait.Image")));
            this.ConfigurationGROUPSpicBoxWait.Location = new System.Drawing.Point(2, 2);
            this.ConfigurationGROUPSpicBoxWait.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationGROUPSpicBoxWait.Name = "ConfigurationGROUPSpicBoxWait";
            this.ConfigurationGROUPSpicBoxWait.Size = new System.Drawing.Size(367, 494);
            this.ConfigurationGROUPSpicBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ConfigurationGROUPSpicBoxWait.TabIndex = 4;
            this.ConfigurationGROUPSpicBoxWait.TabStop = false;
            this.ConfigurationGROUPSpicBoxWait.Visible = false;
            this.ConfigurationGROUPSpicBoxWait.WaitOnLoad = true;
            // 
            // ConfigurationGROUPSConfigurationPanel1label1
            // 
            this.ConfigurationGROUPSConfigurationPanel1label1.Location = new System.Drawing.Point(20, 17);
            this.ConfigurationGROUPSConfigurationPanel1label1.Name = "ConfigurationGROUPSConfigurationPanel1label1";
            this.ConfigurationGROUPSConfigurationPanel1label1.Size = new System.Drawing.Size(100, 23);
            this.ConfigurationGROUPSConfigurationPanel1label1.TabIndex = 12;
            this.ConfigurationGROUPSConfigurationPanel1label1.Text = "Config Group:";
            // 
            // ConfigurationGROUPStabPagecomboBox1
            // 
            this.ConfigurationGROUPStabPagecomboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ConfigurationGROUPStabPagecomboBox1.FormattingEnabled = true;
            this.ConfigurationGROUPStabPagecomboBox1.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.ConfigurationGROUPStabPagecomboBox1.Location = new System.Drawing.Point(137, 17);
            this.ConfigurationGROUPStabPagecomboBox1.Name = "ConfigurationGROUPStabPagecomboBox1";
            this.ConfigurationGROUPStabPagecomboBox1.Size = new System.Drawing.Size(155, 21);
            this.ConfigurationGROUPStabPagecomboBox1.TabIndex = 11;
            this.ConfigurationGROUPStabPagecomboBox1.SelectedIndexChanged += new System.EventHandler(this.OnConfigGroupSelectionChanged);
            // 
            // ConfigurationGROUPSlistView
            // 
            this.ConfigurationGROUPSlistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ConfigurationGROUPStabPageColumnHeader1,
            this.ConfigurationGROUPStabPageColumnHeader2,
            this.ConfigurationGROUPStabPageColumnHeader3});
            this.ConfigurationGROUPSlistView.GridLines = true;
            this.ConfigurationGROUPSlistView.Location = new System.Drawing.Point(0, 54);
            this.ConfigurationGROUPSlistView.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationGROUPSlistView.Name = "ConfigurationGROUPSlistView";
            this.ConfigurationGROUPSlistView.Size = new System.Drawing.Size(371, 442);
            this.ConfigurationGROUPSlistView.TabIndex = 1;
            this.ConfigurationGROUPSlistView.UseCompatibleStateImageBehavior = false;
            this.ConfigurationGROUPSlistView.View = System.Windows.Forms.View.Details;
            // 
            // ConfigurationGROUPStabPageColumnHeader1
            // 
            this.ConfigurationGROUPStabPageColumnHeader1.Text = "AID";
            // 
            // ConfigurationGROUPStabPageColumnHeader2
            // 
            this.ConfigurationGROUPStabPageColumnHeader2.Text = "TAG";
            // 
            // ConfigurationGROUPStabPageColumnHeader3
            // 
            this.ConfigurationGROUPStabPageColumnHeader3.Text = "VALUE";
            // 
            // ConfigurationPanel1
            // 
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationPanel1pictureBox1);
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationPanel1label2);
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationPanel1btnDeviceMode);
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationPanel1btnEMVMode);
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationPanel1label1);
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationPanel1btnFactoryReset);
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationCollapseButton);
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationSplitter1);
            this.ConfigurationPanel1.Controls.Add(this.ConfigurationGroupBox1);
            this.ConfigurationPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.ConfigurationPanel1.Location = new System.Drawing.Point(2, 2);
            this.ConfigurationPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationPanel1.Name = "ConfigurationPanel1";
            this.ConfigurationPanel1.Size = new System.Drawing.Size(350, 577);
            this.ConfigurationPanel1.TabIndex = 0;
            // 
            // ConfigurationPanel1pictureBox1
            // 
            this.ConfigurationPanel1pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("ConfigurationPanel1pictureBox1.Image")));
            this.ConfigurationPanel1pictureBox1.Location = new System.Drawing.Point(0, 3);
            this.ConfigurationPanel1pictureBox1.Name = "ConfigurationPanel1pictureBox1";
            this.ConfigurationPanel1pictureBox1.Size = new System.Drawing.Size(319, 571);
            this.ConfigurationPanel1pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ConfigurationPanel1pictureBox1.TabIndex = 19;
            this.ConfigurationPanel1pictureBox1.TabStop = false;
            this.ConfigurationPanel1pictureBox1.Visible = false;
            // 
            // ConfigurationPanel1label2
            // 
            this.ConfigurationPanel1label2.AutoSize = true;
            this.ConfigurationPanel1label2.Location = new System.Drawing.Point(29, 194);
            this.ConfigurationPanel1label2.Name = "ConfigurationPanel1label2";
            this.ConfigurationPanel1label2.Size = new System.Drawing.Size(74, 13);
            this.ConfigurationPanel1label2.TabIndex = 17;
            this.ConfigurationPanel1label2.Text = "Device Mode:";
            // 
            // ConfigurationPanel1btnDeviceMode
            // 
            this.ConfigurationPanel1btnDeviceMode.Location = new System.Drawing.Point(172, 194);
            this.ConfigurationPanel1btnDeviceMode.Name = "ConfigurationPanel1btnDeviceMode";
            this.ConfigurationPanel1btnDeviceMode.Size = new System.Drawing.Size(75, 23);
            this.ConfigurationPanel1btnDeviceMode.TabIndex = 18;
            this.ConfigurationPanel1btnDeviceMode.Text = "MODE";
            this.ConfigurationPanel1btnDeviceMode.Click += new System.EventHandler(this.OnSetDeviceMode);
            // 
            // ConfigurationPanel1btnEMVMode
            // 
            this.ConfigurationPanel1btnEMVMode.Location = new System.Drawing.Point(32, 248);
            this.ConfigurationPanel1btnEMVMode.Name = "ConfigurationPanel1btnEMVMode";
            this.ConfigurationPanel1btnEMVMode.Size = new System.Drawing.Size(117, 23);
            this.ConfigurationPanel1btnEMVMode.TabIndex = 16;
            this.ConfigurationPanel1btnEMVMode.Text = "EMV Mode Disable";
            this.ConfigurationPanel1btnEMVMode.Click += new System.EventHandler(this.OnEMVModeDisable);
            // 
            // ConfigurationPanel1label1
            // 
            this.ConfigurationPanel1label1.AutoSize = true;
            this.ConfigurationPanel1label1.Location = new System.Drawing.Point(29, 131);
            this.ConfigurationPanel1label1.Name = "ConfigurationPanel1label1";
            this.ConfigurationPanel1label1.Size = new System.Drawing.Size(103, 13);
            this.ConfigurationPanel1label1.TabIndex = 6;
            this.ConfigurationPanel1label1.Text = "Reset Configuration:";
            // 
            // ConfigurationPanel1btnFactoryReset
            // 
            this.ConfigurationPanel1btnFactoryReset.Location = new System.Drawing.Point(172, 123);
            this.ConfigurationPanel1btnFactoryReset.Name = "ConfigurationPanel1btnFactoryReset";
            this.ConfigurationPanel1btnFactoryReset.Size = new System.Drawing.Size(75, 29);
            this.ConfigurationPanel1btnFactoryReset.TabIndex = 7;
            this.ConfigurationPanel1btnFactoryReset.Text = "Factory";
            this.ConfigurationPanel1btnFactoryReset.UseVisualStyleBackColor = true;
            this.ConfigurationPanel1btnFactoryReset.Click += new System.EventHandler(this.OnLoadFactory);
            // 
            // ConfigurationCollapseButton
            // 
            this.ConfigurationCollapseButton.Location = new System.Drawing.Point(320, -2);
            this.ConfigurationCollapseButton.Margin = new System.Windows.Forms.Padding(0);
            this.ConfigurationCollapseButton.Name = "ConfigurationCollapseButton";
            this.ConfigurationCollapseButton.Size = new System.Drawing.Size(27, 579);
            this.ConfigurationCollapseButton.TabIndex = 5;
            this.ConfigurationCollapseButton.Text = "<<";
            this.ConfigurationCollapseButton.UseVisualStyleBackColor = true;
            this.ConfigurationCollapseButton.Visible = false;
            this.ConfigurationCollapseButton.Click += new System.EventHandler(this.OnCollapseConfigurationView);
            // 
            // ConfigurationSplitter1
            // 
            this.ConfigurationSplitter1.Location = new System.Drawing.Point(0, 0);
            this.ConfigurationSplitter1.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationSplitter1.Name = "ConfigurationSplitter1";
            this.ConfigurationSplitter1.Size = new System.Drawing.Size(2, 577);
            this.ConfigurationSplitter1.TabIndex = 4;
            this.ConfigurationSplitter1.TabStop = false;
            // 
            // ConfigurationGroupBox1
            // 
            this.ConfigurationGroupBox1.Controls.Add(this.radioLoadFromDevice);
            this.ConfigurationGroupBox1.Controls.Add(this.radioLoadFromFile);
            this.ConfigurationGroupBox1.Location = new System.Drawing.Point(32, 26);
            this.ConfigurationGroupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.ConfigurationGroupBox1.Name = "ConfigurationGroupBox1";
            this.ConfigurationGroupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.ConfigurationGroupBox1.Size = new System.Drawing.Size(262, 63);
            this.ConfigurationGroupBox1.TabIndex = 3;
            this.ConfigurationGroupBox1.TabStop = false;
            this.ConfigurationGroupBox1.Text = "LOAD FROM: ";
            // 
            // radioLoadFromDevice
            // 
            this.radioLoadFromDevice.AutoSize = true;
            this.radioLoadFromDevice.Location = new System.Drawing.Point(77, 28);
            this.radioLoadFromDevice.Margin = new System.Windows.Forms.Padding(2);
            this.radioLoadFromDevice.Name = "radioLoadFromDevice";
            this.radioLoadFromDevice.Size = new System.Drawing.Size(64, 17);
            this.radioLoadFromDevice.TabIndex = 2;
            this.radioLoadFromDevice.TabStop = true;
            this.radioLoadFromDevice.Text = "DEVICE";
            this.radioLoadFromDevice.UseVisualStyleBackColor = true;
            this.radioLoadFromDevice.CheckedChanged += new System.EventHandler(this.OnLoadFromDevice);
            // 
            // radioLoadFromFile
            // 
            this.radioLoadFromFile.AutoSize = true;
            this.radioLoadFromFile.Location = new System.Drawing.Point(19, 28);
            this.radioLoadFromFile.Margin = new System.Windows.Forms.Padding(2);
            this.radioLoadFromFile.Name = "radioLoadFromFile";
            this.radioLoadFromFile.Size = new System.Drawing.Size(47, 17);
            this.radioLoadFromFile.TabIndex = 1;
            this.radioLoadFromFile.TabStop = true;
            this.radioLoadFromFile.Text = "FILE";
            this.radioLoadFromFile.UseVisualStyleBackColor = true;
            this.radioLoadFromFile.CheckedChanged += new System.EventHandler(this.OnLoadFromFile);
            // 
            // FirmwaretabPage
            // 
            this.FirmwaretabPage.Controls.Add(this.lblFirmwareVersion);
            this.FirmwaretabPage.Controls.Add(this.btnFirmwareUpdate);
            this.FirmwaretabPage.Controls.Add(this.FirmwareConfigurationPanel1label1);
            this.FirmwaretabPage.Controls.Add(this.FirmwareprogressBar1);
            this.FirmwaretabPage.Location = new System.Drawing.Point(4, 22);
            this.FirmwaretabPage.Name = "FirmwaretabPage";
            this.FirmwaretabPage.Size = new System.Drawing.Size(822, 581);
            this.FirmwaretabPage.TabIndex = 2;
            this.FirmwaretabPage.Text = "Firmware";
            this.FirmwaretabPage.UseVisualStyleBackColor = true;
            // 
            // lblFirmwareVersion
            // 
            this.lblFirmwareVersion.AutoSize = true;
            this.lblFirmwareVersion.Location = new System.Drawing.Point(128, 55);
            this.lblFirmwareVersion.Name = "lblFirmwareVersion";
            this.lblFirmwareVersion.Size = new System.Drawing.Size(65, 13);
            this.lblFirmwareVersion.TabIndex = 12;
            this.lblFirmwareVersion.Text = "UNKNOWN";
            // 
            // btnFirmwareUpdate
            // 
            this.btnFirmwareUpdate.Enabled = false;
            this.btnFirmwareUpdate.Location = new System.Drawing.Point(26, 79);
            this.btnFirmwareUpdate.Name = "btnFirmwareUpdate";
            this.btnFirmwareUpdate.Size = new System.Drawing.Size(75, 29);
            this.btnFirmwareUpdate.TabIndex = 10;
            this.btnFirmwareUpdate.Text = "Update";
            this.btnFirmwareUpdate.UseVisualStyleBackColor = true;
            this.btnFirmwareUpdate.Click += new System.EventHandler(this.OnFirmwareUpdate);
            // 
            // FirmwareConfigurationPanel1label1
            // 
            this.FirmwareConfigurationPanel1label1.Location = new System.Drawing.Point(23, 53);
            this.FirmwareConfigurationPanel1label1.Name = "FirmwareConfigurationPanel1label1";
            this.FirmwareConfigurationPanel1label1.Size = new System.Drawing.Size(100, 23);
            this.FirmwareConfigurationPanel1label1.TabIndex = 14;
            this.FirmwareConfigurationPanel1label1.Text = "Firmware Version:";
            // 
            // FirmwareprogressBar1
            // 
            this.FirmwareprogressBar1.Location = new System.Drawing.Point(26, 203);
            this.FirmwareprogressBar1.Name = "FirmwareprogressBar1";
            this.FirmwareprogressBar1.Size = new System.Drawing.Size(627, 29);
            this.FirmwareprogressBar1.TabIndex = 13;
            this.FirmwareprogressBar1.Visible = false;
            // 
            // ApplicationpicBoxWait
            // 
            this.ApplicationpicBoxWait.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplicationpicBoxWait.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ApplicationpicBoxWait.Location = new System.Drawing.Point(2, 2);
            this.ApplicationpicBoxWait.Margin = new System.Windows.Forms.Padding(2);
            this.ApplicationpicBoxWait.Name = "ApplicationpicBoxWait";
            this.ApplicationpicBoxWait.Size = new System.Drawing.Size(367, 494);
            this.ApplicationpicBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ApplicationpicBoxWait.TabIndex = 4;
            this.ApplicationpicBoxWait.TabStop = false;
            this.ApplicationpicBoxWait.Visible = false;
            this.ApplicationpicBoxWait.WaitOnLoad = true;
            // 
            // FirmwareopenFileDialog1
            // 
            this.FirmwareopenFileDialog1.FileName = "FirmwareopenFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 680);
            this.Controls.Add(this.tabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "IDTech Configuration Reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.tabControlMain.ResumeLayout(false);
            this.ConfigurationtabPage.ResumeLayout(false);
            this.ConfigurationPanel2.ResumeLayout(false);
            this.tabControlConfiguration.ResumeLayout(false);
            this.ConfigurationTerminalDatatabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationTerminalDatapicBoxWait)).EndInit();
            this.ConfigurationAIDStabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationAIDSpicBoxWait)).EndInit();
            this.ConfigurationCAPKStabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationCAPKSpicBoxWait)).EndInit();
            this.ConfigurationGROUPStabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationGROUPSpicBoxWait)).EndInit();
            this.ConfigurationPanel1.ResumeLayout(false);
            this.ConfigurationPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConfigurationPanel1pictureBox1)).EndInit();
            this.ConfigurationGroupBox1.ResumeLayout(false);
            this.ConfigurationGroupBox1.PerformLayout();
            this.FirmwaretabPage.ResumeLayout(false);
            this.FirmwaretabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ApplicationpicBoxWait)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        // TAB: APPLICATION
        private System.Windows.Forms.TabPage ApplicationtabPage;
        private System.Windows.Forms.PictureBox ApplicationpicBoxWait;
        // TAB: CONFIGURATION
        private System.Windows.Forms.TabControl tabControlConfiguration;
        private System.Windows.Forms.TabPage ConfigurationtabPage;
        private System.Windows.Forms.Panel ConfigurationPanel1;
        private System.Windows.Forms.PictureBox ConfigurationPanel1pictureBox1;
        private System.Windows.Forms.Panel ConfigurationPanel2;
        private System.Windows.Forms.RadioButton radioLoadFromDevice;
        private System.Windows.Forms.RadioButton radioLoadFromFile;
        private System.Windows.Forms.GroupBox ConfigurationGroupBox1;
        private System.Windows.Forms.Label ConfigurationPanel1label1;
        private System.Windows.Forms.Label ConfigurationPanel1label2;
        private System.Windows.Forms.Button ConfigurationPanel1btnFactoryReset;
        private System.Windows.Forms.Button ConfigurationPanel1btnDeviceMode;
        private System.Windows.Forms.Button ConfigurationPanel1btnEMVMode;
        private System.Windows.Forms.Splitter ConfigurationSplitter1;
        private System.Windows.Forms.Button ConfigurationCollapseButton;
        private System.Windows.Forms.Button ConfigurationExpandButton;
        // SUB-TAB: TERMINAL DATA
        private System.Windows.Forms.TabPage ConfigurationTerminalDatatabPage;
        private System.Windows.Forms.ListView ConfigurationTerminalDatalistView;
        private System.Windows.Forms.ColumnHeader ConfigurationTerminalDatacolumnHeader1;
        private System.Windows.Forms.ColumnHeader ConfigurationTerminalDatacolumnHeader2;
        private System.Windows.Forms.PictureBox ConfigurationTerminalDatapicBoxWait;
        // SUB-TAB: AID LIST
        private System.Windows.Forms.TabPage ConfigurationAIDStabPage;
        private System.Windows.Forms.ListView ConfigurationAIDSlistView;
        private System.Windows.Forms.ColumnHeader ConfigurationAIDStabPageColumnHeader1;
        private System.Windows.Forms.ColumnHeader ConfigurationAIDStabPageColumnHeader2;
        private System.Windows.Forms.PictureBox ConfigurationAIDSpicBoxWait;
        // SUB-TAB: CAPK LIST
        private System.Windows.Forms.TabPage ConfigurationCAPKStabPage;
        private System.Windows.Forms.ListView ConfigurationCAPKSlistView;
        private System.Windows.Forms.ColumnHeader ConfigurationCAPKStabPageColumnHeader1;
        private System.Windows.Forms.ColumnHeader ConfigurationCAPKStabPageColumnHeader2;
        private System.Windows.Forms.PictureBox ConfigurationCAPKSpicBoxWait;
        // SUB-TAB: GROUPS
        private System.Windows.Forms.TabPage ConfigurationGROUPStabPage;
        private System.Windows.Forms.Label ConfigurationGROUPSConfigurationPanel1label1;
        private System.Windows.Forms.PictureBox ConfigurationGROUPSpicBoxWait;
        private System.Windows.Forms.ListView ConfigurationGROUPSlistView;
        private System.Windows.Forms.ColumnHeader ConfigurationGROUPStabPageColumnHeader1;
        private System.Windows.Forms.ColumnHeader ConfigurationGROUPStabPageColumnHeader2;
        private System.Windows.Forms.ColumnHeader ConfigurationGROUPStabPageColumnHeader3;
        private System.Windows.Forms.ComboBox ConfigurationGROUPStabPagecomboBox1;
        // TAB: FIRMWARE
        private System.Windows.Forms.TabPage FirmwaretabPage;
        private System.Windows.Forms.Button btnFirmwareUpdate;
        private System.Windows.Forms.Label lblFirmwareVersion;
        private System.Windows.Forms.Label FirmwareConfigurationPanel1label1;
        private System.Windows.Forms.OpenFileDialog FirmwareopenFileDialog1;
        private System.Windows.Forms.ProgressBar FirmwareprogressBar1;
    }
}

