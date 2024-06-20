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
    partial class SwitchValueUserControl : UserControl
    {
        public SwitchValueUserControl()
        {
            InitializeComponent();
        }

        string _switchName;

        internal void SetSwitchName(string p)
        {
            _switchName = p;

        }

        string _switchValue;

        bool _setFromController;

        internal void SetSwitchValue(string p)
        {
            _setFromController = true;
            _switchValue = p;
            labelSwitchValue.Text = p;

            if (_cntrl != null && !string.IsNullOrEmpty(_switchName)) {


                //inputs
                this.checkedListBoxInputs.Items.Clear();
                IEnumerable<string> inputpresentValues=_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Inputs;
                string[] inputallValues = _cntrl.Inputs.Select(a => a.Key).ToArray();
                foreach (string s in inputallValues) {
                    if (inputpresentValues.Contains(s))
                    {
                        this.checkedListBoxInputs.Items.Add(s, CheckState.Checked);
                    }
                    else {
                        this.checkedListBoxInputs.Items.Add(s, CheckState.Unchecked);
                    }
                }


                //outputs
                this.checkedListBoxOutputs.Items.Clear();
                IEnumerable<string> outputpresentValues = _cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Outputs;
                string[] outputallValues = _cntrl.Outputs.Select(a => a.Key).ToArray();
                foreach (string s in outputallValues)
                {
                    if (outputpresentValues.Contains(s))
                    {
                        this.checkedListBoxOutputs.Items.Add(s, CheckState.Checked);
                    }
                    else
                    {
                        this.checkedListBoxOutputs.Items.Add(s, CheckState.Unchecked);
                    }
                }

                //parameters
                this.checkedListBoxParameters.Items.Clear();
                IEnumerable<string> parampresentValues = _cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Parameters;
                string[] paramallValues = _cntrl.Parameters.Select(a => a.Key).ToArray();
                foreach (string s in paramallValues)
                {
                    if (parampresentValues.Contains(s))
                    {
                        this.checkedListBoxParameters.Items.Add(s, CheckState.Checked);
                    }
                    else
                    {
                        this.checkedListBoxParameters.Items.Add(s, CheckState.Unchecked);
                    }
                }

                //this.checkedListBoxOutputs.Items.AddRange(_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Outputs.Select(a => a.Key));
               // this.checkedListBoxParameters.Items.AddRange(_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Parameters.Select(a => a.Key));
                
            }

            _setFromController = false;
        }

        
        Controller _cntrl;
        internal void SetController(Controller cntrl)
        {
            _cntrl = cntrl;
        }



        private void checkedListBoxInputs_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_setFromController)
            {
                string s = checkedListBoxInputs.Items.OfType<string>().ElementAt(e.Index);
                if (e.NewValue == CheckState.Checked)
                {
                    if (!_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Inputs.Contains((string)s))
                    {
                        _cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Inputs.Add((string)s);
                    }
                }
                else
                {
                    if (_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Inputs.Contains((string)s))
                    {
                        _cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Inputs.Remove((string)s);
                    }

                }
            }
        }

        private void checkedListBoxOutputs_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_setFromController)
            {
                string s = checkedListBoxOutputs.Items.OfType<string>().ElementAt(e.Index);
                if (e.NewValue == CheckState.Checked)
                {
                    if (!_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Outputs.Contains((string)s))
                    {
                        _cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Outputs.Add((string)s);
                    }
                }
                else
                {
                    if (_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Outputs.Contains((string)s))
                    {
                        _cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Outputs.Remove((string)s);
                    }

                }
            }
        }

        private void checkedListBoxParameters_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_setFromController)
            {
                string s = checkedListBoxParameters.Items.OfType<string>().ElementAt(e.Index);
                if (e.NewValue == CheckState.Checked)
                {
                    if (!_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Parameters.Contains((string)s))
                    {
                        _cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Parameters.Add((string)s);
                    }
                }
                else
                {
                    if (_cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Parameters.Contains((string)s))
                    {
                        _cntrl.Switches[_switchName].SwitchValueRelatedModelingOptions4SwitchValues[_switchValue].Parameters.Remove((string)s);
                    }

                }
            }
        }

    }

     
}
