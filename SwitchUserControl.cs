using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CRA.ModelLayer.Strategy;


namespace CRA.ModelLayer.SCC
{
    partial class SwitchUserControl : UserControl
    {
        public SwitchUserControl()
        {
            InitializeComponent();
            labelStatus.Text = "Changed";
            button1.Enabled = false;
        }

        Controller _cntrl;
        internal void SetController(Controller cntrl)
        {
            _cntrl = cntrl;
        }

        string _switchname;
        internal void SetSwitchName(string switchname)
        {
            _switchname = switchname;
            textBox1.Text = _switchname;

            if (!_cntrl.Switches.ContainsKey(switchname)) { _cntrl.Switches.Add(switchname, new BuildingSwitch(switchname, "", new Dictionary<string, ModellingOptions>(),_cntrl.GeneralModelingOptions)); }

            if (_cntrl.Switches.ContainsKey(_switchname))
            {
                textBox2.Text = _cntrl.Switches[_switchname].SwitchDescription;

                foreach (string s in _cntrl.Switches[_switchname].AcceptableSwitchValues) {

                    if (!string.IsNullOrEmpty(s)) dataGridView1.Rows.Add(new object[]{s});
                }
            }

            labelStatus.Text = "Saved";
            button1.Enabled = true;
        }
        internal string GetSwitchName
        {
            get { return _switchname; }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (_cntrl.Switches.ContainsKey(_switchname))
            {
                Dictionary<string, ModellingOptions> d = new Dictionary<string, ModellingOptions>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string s = (string)row.Cells["ValueId"].Value;
                    if (!string.IsNullOrEmpty(s) && !d.ContainsKey(s)) d.Add(s, null);
                }
                if (_cntrl.Switches[_switchname].SwitchValueRelatedModelingOptions4SwitchValues == null )
                {
                    _cntrl.Switches[_switchname] = new BuildingSwitch(_switchname, textBox2.Text, d, _cntrl.GeneralModelingOptions);
                }
                else {
                    Dictionary<string, SwitchValueRelatedModelingOptions> alreadypresentValuesModelingOptions = _cntrl.Switches[_switchname].SwitchValueRelatedModelingOptions4SwitchValues;

                    //if it is a newvalue, add the entry into the dictionary
                    foreach (string k in d.Keys) {

                        if (!alreadypresentValuesModelingOptions.ContainsKey(k)) {
                            alreadypresentValuesModelingOptions.Add(k, _cntrl.GeneralModelingOptions);
                        }
                    }

                    _cntrl.Switches[_switchname] = new BuildingSwitch(_switchname, textBox2.Text, d, alreadypresentValuesModelingOptions);
                }

               
            }
            labelStatus.Text = "Saved";
            button1.Enabled = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

            labelStatus.Text = "Changed";
            button1.Enabled = false;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
            labelStatus.Text = "Changed";
            button1.Enabled = false;
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            labelStatus.Text = "Changed";
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var suc = new SwitchValuesUserControl();
            

            SwitchValuesForm f = new SwitchValuesForm();
            
            f.SetController(_cntrl);
           
            if (_switchname != null)
            {
                suc.SetSwitchName(_switchname);
                suc.SetController(_cntrl);
            }
            f.Controls.Add(suc);
            f.ShowDialog();
        }
    }
}
