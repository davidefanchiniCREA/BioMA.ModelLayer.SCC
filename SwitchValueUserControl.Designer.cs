namespace CRA.ModelLayer.SCC
{
    partial class SwitchValueUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.labelSwitchValue = new System.Windows.Forms.Label();
            this.checkedListBoxInputs = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxOutputs = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxParameters = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(108, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Switch value:";
            // 
            // labelSwitchValue
            // 
            this.labelSwitchValue.AutoSize = true;
            this.labelSwitchValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSwitchValue.Location = new System.Drawing.Point(190, 4);
            this.labelSwitchValue.Name = "labelSwitchValue";
            this.labelSwitchValue.Size = new System.Drawing.Size(41, 13);
            this.labelSwitchValue.TabIndex = 1;
            this.labelSwitchValue.Text = "label2";
            // 
            // checkedListBoxInputs
            // 
            this.checkedListBoxInputs.CheckOnClick = true;
            this.checkedListBoxInputs.FormattingEnabled = true;
            this.checkedListBoxInputs.Location = new System.Drawing.Point(7, 36);
            this.checkedListBoxInputs.Name = "checkedListBoxInputs";
            this.checkedListBoxInputs.Size = new System.Drawing.Size(171, 199);
            this.checkedListBoxInputs.TabIndex = 2;
            this.checkedListBoxInputs.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxInputs_ItemCheck);
            // 
            // checkedListBoxOutputs
            // 
            this.checkedListBoxOutputs.CheckOnClick = true;
            this.checkedListBoxOutputs.FormattingEnabled = true;
            this.checkedListBoxOutputs.Location = new System.Drawing.Point(203, 36);
            this.checkedListBoxOutputs.Name = "checkedListBoxOutputs";
            this.checkedListBoxOutputs.Size = new System.Drawing.Size(171, 199);
            this.checkedListBoxOutputs.TabIndex = 3;
            this.checkedListBoxOutputs.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxOutputs_ItemCheck);
            // 
            // checkedListBoxParameters
            // 
            this.checkedListBoxParameters.CheckOnClick = true;
            this.checkedListBoxParameters.FormattingEnabled = true;
            this.checkedListBoxParameters.Location = new System.Drawing.Point(400, 36);
            this.checkedListBoxParameters.Name = "checkedListBoxParameters";
            this.checkedListBoxParameters.Size = new System.Drawing.Size(171, 199);
            this.checkedListBoxParameters.TabIndex = 4;
            this.checkedListBoxParameters.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxParameters_ItemCheck);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Inputs";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(461, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Parameters";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(265, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Outputs";
            // 
            // SwitchValueUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkedListBoxParameters);
            this.Controls.Add(this.checkedListBoxOutputs);
            this.Controls.Add(this.checkedListBoxInputs);
            this.Controls.Add(this.labelSwitchValue);
            this.Controls.Add(this.label1);
            this.Name = "SwitchValueUserControl";
            this.Size = new System.Drawing.Size(592, 243);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSwitchValue;
        private System.Windows.Forms.CheckedListBox checkedListBoxInputs;
        private System.Windows.Forms.CheckedListBox checkedListBoxOutputs;
        private System.Windows.Forms.CheckedListBox checkedListBoxParameters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
