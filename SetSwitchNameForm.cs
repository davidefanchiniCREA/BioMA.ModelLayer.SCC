using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CRA.ModelLayer.SCC
{
    partial class SetSwitchNameForm : Form
    {
        public SetSwitchNameForm()
        {
            InitializeComponent();
        }


        Controller _cntrl;
        internal void SetController(Controller cntrl)
        {
            _cntrl = cntrl;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)) { MessageBox.Show("Please enter the switch name"); return; }
            _cntrl.TmpSwitchName = textBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _cntrl.TmpSwitchName = null;
            this.Close();
        }

       
    }
}
