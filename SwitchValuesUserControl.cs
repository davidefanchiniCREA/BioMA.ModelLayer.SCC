using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CRA.ModelLayer.SCC
{
    partial class SwitchValuesUserControl : UserControl
    {
        public SwitchValuesUserControl()
        {
            InitializeComponent();
        }

        string _switchName;

        internal void SetSwitchName(string p)
        {
            _switchName = p;
            labelSwitchName.Text = p;
        }

        Controller _cntrl;
        internal void SetController(Controller cntrl)
        {
            _cntrl = cntrl;
            if (!string.IsNullOrEmpty(_switchName))
            {
                foreach (var sv in _cntrl.Switches[_switchName].AcceptableSwitchValues)
                {

                    SwitchValueUserControl svuc = new SwitchValueUserControl();
                    svuc.SetController(_cntrl);
                    svuc.SetSwitchName(_switchName);
                    svuc.SetSwitchValue(sv);
                    flowLayoutPanel1.Controls.Add(svuc);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ((Form)this.Parent).Close();
        }
    }
}
