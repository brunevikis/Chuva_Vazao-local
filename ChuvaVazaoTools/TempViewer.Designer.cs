namespace ChuvaVazaoTools {
    partial class TempViewer {
        /// <summary>
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.btn_ScaleUp = new System.Windows.Forms.Button();
            this.btn_InterUp = new System.Windows.Forms.Button();
            this.btn_ScaleDown = new System.Windows.Forms.Button();
            this.btn_InterDown = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_defaulChart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
            chartArea2.AxisX.IsLabelAutoFit = false;
            chartArea2.AxisX.LabelStyle.Angle = -45;
            chartArea2.AxisX.MajorGrid.Interval = 0D;
            chartArea2.AxisX.MinorGrid.Enabled = true;
            chartArea2.AxisY.Maximum = 35D;
            chartArea2.AxisY.Minimum = 0D;
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(675, 347);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(3, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(131, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.radioButton3);
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Location = new System.Drawing.Point(693, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(137, 123);
            this.panel1.TabIndex = 2;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(35, 76);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(41, 17);
            this.radioButton3.TabIndex = 4;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Var";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(35, 53);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(84, 17);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Média 9-18h";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(35, 30);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(59, 17);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Horário";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // btn_ScaleUp
            // 
            this.btn_ScaleUp.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btn_ScaleUp.Location = new System.Drawing.Point(728, 179);
            this.btn_ScaleUp.Name = "btn_ScaleUp";
            this.btn_ScaleUp.Size = new System.Drawing.Size(37, 23);
            this.btn_ScaleUp.TabIndex = 4;
            this.btn_ScaleUp.Text = "é";
            this.btn_ScaleUp.UseVisualStyleBackColor = true;
            this.btn_ScaleUp.Click += new System.EventHandler(this.btn_ScaleUp_Click);
            // 
            // btn_InterUp
            // 
            this.btn_InterUp.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btn_InterUp.Location = new System.Drawing.Point(728, 231);
            this.btn_InterUp.Name = "btn_InterUp";
            this.btn_InterUp.Size = new System.Drawing.Size(37, 23);
            this.btn_InterUp.TabIndex = 5;
            this.btn_InterUp.Text = "é";
            this.btn_InterUp.UseVisualStyleBackColor = true;
            this.btn_InterUp.Click += new System.EventHandler(this.btn_InterUp_Click);
            // 
            // btn_ScaleDown
            // 
            this.btn_ScaleDown.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btn_ScaleDown.Location = new System.Drawing.Point(775, 179);
            this.btn_ScaleDown.Name = "btn_ScaleDown";
            this.btn_ScaleDown.Size = new System.Drawing.Size(37, 23);
            this.btn_ScaleDown.TabIndex = 6;
            this.btn_ScaleDown.Text = "ê";
            this.btn_ScaleDown.UseVisualStyleBackColor = true;
            this.btn_ScaleDown.Click += new System.EventHandler(this.btn_ScaleDown_Click);
            // 
            // btn_InterDown
            // 
            this.btn_InterDown.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btn_InterDown.Location = new System.Drawing.Point(775, 231);
            this.btn_InterDown.Name = "btn_InterDown";
            this.btn_InterDown.Size = new System.Drawing.Size(37, 23);
            this.btn_InterDown.TabIndex = 7;
            this.btn_InterDown.Text = "ê";
            this.btn_InterDown.UseVisualStyleBackColor = true;
            this.btn_InterDown.Click += new System.EventHandler(this.btn_InterDown_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(729, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Alterar Escala";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(729, 215);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Alterar Intervalo";
            // 
            // btn_defaulChart
            // 
            this.btn_defaulChart.Location = new System.Drawing.Point(728, 272);
            this.btn_defaulChart.Name = "btn_defaulChart";
            this.btn_defaulChart.Size = new System.Drawing.Size(84, 23);
            this.btn_defaulChart.TabIndex = 10;
            this.btn_defaulChart.Text = "Restaurar";
            this.btn_defaulChart.UseVisualStyleBackColor = true;
            this.btn_defaulChart.Click += new System.EventHandler(this.btn_defaulChart_Click);
            // 
            // TempViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 371);
            this.Controls.Add(this.btn_defaulChart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_InterDown);
            this.Controls.Add(this.btn_ScaleDown);
            this.Controls.Add(this.btn_InterUp);
            this.Controls.Add(this.btn_ScaleUp);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chart1);
            this.Name = "TempViewer";
            this.Text = "TempViewer";
            this.Load += new System.EventHandler(this.TempViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Button btn_ScaleUp;
        private System.Windows.Forms.Button btn_InterUp;
        private System.Windows.Forms.Button btn_ScaleDown;
        private System.Windows.Forms.Button btn_InterDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_defaulChart;
    }
}