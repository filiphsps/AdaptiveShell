using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

namespace Shell.Host {
    partial class TaskBar {
        // <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TaskBar));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.WindowsBtn = new System.Windows.Forms.Button();
            this.SearchBtn = new System.Windows.Forms.Button();
            this.BackBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.WindowsBtn, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.SearchBtn, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.BackBtn, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(300, 50);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // WindowsBtn
            // 
            this.WindowsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.WindowsBtn.BackgroundImage = global::Shell.Host.Properties.Resources.HomeBtn;
            this.WindowsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.WindowsBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.WindowsBtn.FlatAppearance.BorderSize = 0;
            this.WindowsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.WindowsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.WindowsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WindowsBtn.ForeColor = System.Drawing.Color.Transparent;
            this.WindowsBtn.Location = new System.Drawing.Point(135, 10);
            this.WindowsBtn.Margin = new System.Windows.Forms.Padding(5);
            this.WindowsBtn.MaximumSize = new System.Drawing.Size(30, 30);
            this.WindowsBtn.MinimumSize = new System.Drawing.Size(30, 30);
            this.WindowsBtn.Name = "WindowsBtn";
            this.WindowsBtn.Padding = new System.Windows.Forms.Padding(5);
            this.WindowsBtn.Size = new System.Drawing.Size(30, 30);
            this.WindowsBtn.TabIndex = 1;
            this.WindowsBtn.UseVisualStyleBackColor = false;
            this.WindowsBtn.Click += new System.EventHandler(this.OnWindowsClick);
            // 
            // SearchBtn
            // 
            this.SearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SearchBtn.BackgroundImage = global::Shell.Host.Properties.Resources.SearchBtn;
            this.SearchBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SearchBtn.FlatAppearance.BorderSize = 0;
            this.SearchBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.SearchBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.SearchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SearchBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.SearchBtn.Location = new System.Drawing.Point(185, 10);
            this.SearchBtn.Margin = new System.Windows.Forms.Padding(5);
            this.SearchBtn.MaximumSize = new System.Drawing.Size(30, 30);
            this.SearchBtn.MinimumSize = new System.Drawing.Size(30, 30);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(30, 30);
            this.SearchBtn.TabIndex = 3;
            this.SearchBtn.UseVisualStyleBackColor = false;
            this.SearchBtn.Click += new System.EventHandler(this.OnSearchClick);
            // 
            // BackBtn
            // 
            this.BackBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BackBtn.AutoSize = true;
            this.BackBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BackBtn.BackgroundImage")));
            this.BackBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BackBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.BackBtn.FlatAppearance.BorderSize = 0;
            this.BackBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BackBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BackBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.BackBtn.Location = new System.Drawing.Point(85, 10);
            this.BackBtn.Margin = new System.Windows.Forms.Padding(5);
            this.BackBtn.MaximumSize = new System.Drawing.Size(30, 30);
            this.BackBtn.MinimumSize = new System.Drawing.Size(30, 30);
            this.BackBtn.Name = "BackBtn";
            this.BackBtn.Size = new System.Drawing.Size(30, 30);
            this.BackBtn.TabIndex = 2;
            this.BackBtn.UseVisualStyleBackColor = false;
            this.BackBtn.Click += new System.EventHandler(this.OnBackClick);
            // 
            // TaskBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(300, 50);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TaskBar";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private Button WindowsBtn;
        private Button SearchBtn;
        private Button BackBtn;
    }
}