﻿namespace IDTechConfigReader
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.picBoxConfigWait1 = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.picBoxConfigWait2 = new System.Windows.Forms.PictureBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.picBoxConfigWait3 = new System.Windows.Forms.PictureBox();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.picBoxConfigWait4 = new System.Windows.Forms.PictureBox();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxConfigWait1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxConfigWait2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxConfigWait3)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxConfigWait4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 53);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(700, 607);
            this.tabControl1.TabIndex = 14;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.picBoxConfigWait1);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(692, 581);
            this.tabPage1.TabIndex = 7;
            this.tabPage1.Text = "Configuration";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // picBoxConfigWait1
            // 
            this.picBoxConfigWait1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxConfigWait1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picBoxConfigWait1.Image = ((System.Drawing.Image)(resources.GetObject("picBoxConfigWait1.Image")));
            this.picBoxConfigWait1.Location = new System.Drawing.Point(20, 14);
            this.picBoxConfigWait1.Name = "picBoxConfigWait1";
            this.picBoxConfigWait1.Size = new System.Drawing.Size(652, 553);
            this.picBoxConfigWait1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picBoxConfigWait1.TabIndex = 3;
            this.picBoxConfigWait1.TabStop = false;
            this.picBoxConfigWait1.Visible = false;
            this.picBoxConfigWait1.WaitOnLoad = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(183, 124);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 29);
            this.button3.TabIndex = 4;
            this.button3.Text = "Factory";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Reset Configuration:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(295, 50);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 29);
            this.button2.TabIndex = 2;
            this.button2.Text = "Device";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(183, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 29);
            this.button1.TabIndex = 1;
            this.button1.Text = "File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Load Configuration From:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.picBoxConfigWait2);
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(692, 581);
            this.tabPage2.TabIndex = 4;
            this.tabPage2.Text = "Terminal Data";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // picBoxConfigWait2
            // 
            this.picBoxConfigWait2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxConfigWait2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picBoxConfigWait2.Image = ((System.Drawing.Image)(resources.GetObject("picBoxConfigWait2.Image")));
            this.picBoxConfigWait2.Location = new System.Drawing.Point(20, 14);
            this.picBoxConfigWait2.Name = "picBoxConfigWait2";
            this.picBoxConfigWait2.Size = new System.Drawing.Size(660, 553);
            this.picBoxConfigWait2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picBoxConfigWait2.TabIndex = 4;
            this.picBoxConfigWait2.TabStop = false;
            this.picBoxConfigWait2.Visible = false;
            this.picBoxConfigWait2.WaitOnLoad = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(20, 14);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(652, 553);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "TAG";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "VALUE";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.picBoxConfigWait3);
            this.tabPage3.Controls.Add(this.listView2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(692, 581);
            this.tabPage3.TabIndex = 5;
            this.tabPage3.Text = "AID List";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // picBoxConfigWait3
            // 
            this.picBoxConfigWait3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxConfigWait3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picBoxConfigWait3.Image = ((System.Drawing.Image)(resources.GetObject("picBoxConfigWait3.Image")));
            this.picBoxConfigWait3.Location = new System.Drawing.Point(20, 14);
            this.picBoxConfigWait3.Name = "picBoxConfigWait3";
            this.picBoxConfigWait3.Size = new System.Drawing.Size(653, 554);
            this.picBoxConfigWait3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picBoxConfigWait3.TabIndex = 4;
            this.picBoxConfigWait3.TabStop = false;
            this.picBoxConfigWait3.Visible = false;
            this.picBoxConfigWait3.WaitOnLoad = true;
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(20, 14);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(652, 553);
            this.listView2.TabIndex = 2;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "AID";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "VALUES";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.picBoxConfigWait4);
            this.tabPage4.Controls.Add(this.listView3);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(692, 581);
            this.tabPage4.TabIndex = 6;
            this.tabPage4.Text = "CAPK";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // picBoxConfigWait4
            // 
            this.picBoxConfigWait4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxConfigWait4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picBoxConfigWait4.Image = ((System.Drawing.Image)(resources.GetObject("picBoxConfigWait4.Image")));
            this.picBoxConfigWait4.Location = new System.Drawing.Point(20, 14);
            this.picBoxConfigWait4.Name = "picBoxConfigWait4";
            this.picBoxConfigWait4.Size = new System.Drawing.Size(652, 553);
            this.picBoxConfigWait4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picBoxConfigWait4.TabIndex = 5;
            this.picBoxConfigWait4.TabStop = false;
            this.picBoxConfigWait4.Visible = false;
            this.picBoxConfigWait4.WaitOnLoad = true;
            // 
            // listView3
            // 
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader9});
            this.listView3.FullRowSelect = true;
            this.listView3.GridLines = true;
            this.listView3.Location = new System.Drawing.Point(20, 14);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(652, 553);
            this.listView3.TabIndex = 3;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "CAPK";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "MODULUS";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(696, 44);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 680);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "IDTech Configuration Reader";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxConfigWait1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxConfigWait2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxConfigWait3)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxConfigWait4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPage1;
        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox picBoxConfigWait1;
        private System.Windows.Forms.PictureBox picBoxConfigWait2;
        private System.Windows.Forms.PictureBox picBoxConfigWait3;
        private System.Windows.Forms.PictureBox picBoxConfigWait4;
        private System.Windows.Forms.ColumnHeader columnHeader9;
    }
}

