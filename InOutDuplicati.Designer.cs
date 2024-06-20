namespace CRA.ModelLayer.SCC
{
    partial class InOutDuplicati
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgwInputsDups = new System.Windows.Forms.DataGridView();
            this.Inputs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InputDomainClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDupsOk = new System.Windows.Forms.Button();
            this.btnDupsCancel = new System.Windows.Forms.Button();
            this.lblDups = new System.Windows.Forms.Label();
            this.lblDupsFooter = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgwInputsDups)).BeginInit();
            this.SuspendLayout();
            // 
            // dgwInputsDups
            // 
            this.dgwInputsDups.AllowUserToAddRows = false;
            this.dgwInputsDups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgwInputsDups.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgwInputsDups.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgwInputsDups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwInputsDups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Inputs,
            this.InputDomainClass});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgwInputsDups.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgwInputsDups.Location = new System.Drawing.Point(12, 29);
            this.dgwInputsDups.Name = "dgwInputsDups";
            this.dgwInputsDups.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgwInputsDups.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgwInputsDups.Size = new System.Drawing.Size(751, 274);
            this.dgwInputsDups.TabIndex = 8;
            // 
            // Inputs
            // 
            this.Inputs.Frozen = true;
            this.Inputs.HeaderText = "Inputs";
            this.Inputs.MaxInputLength = 200;
            this.Inputs.Name = "Inputs";
            this.Inputs.ReadOnly = true;
            this.Inputs.ToolTipText = "The inputs selected for the strategy";
            this.Inputs.Width = 200;
            // 
            // InputDomainClass
            // 
            this.InputDomainClass.FillWeight = 500F;
            this.InputDomainClass.HeaderText = "Domain Class";
            this.InputDomainClass.MaxInputLength = 500;
            this.InputDomainClass.MinimumWidth = 50;
            this.InputDomainClass.Name = "InputDomainClass";
            this.InputDomainClass.ReadOnly = true;
            this.InputDomainClass.ToolTipText = "The domain class where the variables is defined";
            this.InputDomainClass.Width = 500;
            // 
            // btnDupsOk
            // 
            this.btnDupsOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDupsOk.Location = new System.Drawing.Point(12, 356);
            this.btnDupsOk.Name = "btnDupsOk";
            this.btnDupsOk.Size = new System.Drawing.Size(150, 23);
            this.btnDupsOk.TabIndex = 9;
            this.btnDupsOk.Text = "OK";
            this.btnDupsOk.UseVisualStyleBackColor = true;
            this.btnDupsOk.Click += new System.EventHandler(this.btnDupsOk_Click);
            // 
            // btnDupsCancel
            // 
            this.btnDupsCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDupsCancel.Location = new System.Drawing.Point(168, 356);
            this.btnDupsCancel.Name = "btnDupsCancel";
            this.btnDupsCancel.Size = new System.Drawing.Size(150, 23);
            this.btnDupsCancel.TabIndex = 10;
            this.btnDupsCancel.Text = "Cancel";
            this.btnDupsCancel.UseVisualStyleBackColor = true;
            this.btnDupsCancel.Click += new System.EventHandler(this.btnDupsCancel_Click);
            // 
            // lblDups
            // 
            this.lblDups.AutoSize = true;
            this.lblDups.Location = new System.Drawing.Point(12, 13);
            this.lblDups.Name = "lblDups";
            this.lblDups.Size = new System.Drawing.Size(385, 13);
            this.lblDups.TabIndex = 11;
            this.lblDups.Text = "This is the list of variables that you have specified both as inputs and as outpu" +
    "ts.";
            // 
            // lblDupsFooter
            // 
            this.lblDupsFooter.AutoSize = true;
            this.lblDupsFooter.Location = new System.Drawing.Point(12, 306);
            this.lblDupsFooter.Name = "lblDupsFooter";
            this.lblDupsFooter.Size = new System.Drawing.Size(581, 39);
            this.lblDupsFooter.TabIndex = 12;
            this.lblDupsFooter.Text = "In general, this is poor specification practice for a strategy, and it can be con" +
    "sidered acceptable only for auxiliary variables.\r\n \r\nWould you like to continue?" +
    "\r\n";
            // 
            // InOutDuplicati
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 389);
            this.Controls.Add(this.lblDupsFooter);
            this.Controls.Add(this.lblDups);
            this.Controls.Add(this.btnDupsCancel);
            this.Controls.Add(this.btnDupsOk);
            this.Controls.Add(this.dgwInputsDups);
            this.Name = "InOutDuplicati";
            this.Load += new System.EventHandler(this.InOutDuplicati_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgwInputsDups)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgwInputsDups;
        private System.Windows.Forms.DataGridViewTextBoxColumn Inputs;
        private System.Windows.Forms.DataGridViewTextBoxColumn InputDomainClass;
        private System.Windows.Forms.Button btnDupsOk;
        private System.Windows.Forms.Button btnDupsCancel;
        private System.Windows.Forms.Label lblDups;
        private System.Windows.Forms.Label lblDupsFooter;
    }
}