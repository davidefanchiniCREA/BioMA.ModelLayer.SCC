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
    partial class SwitchValuesForm : Form
    {
        public SwitchValuesForm()
        {
            InitializeComponent();
        }

        Controller _cntrl;
        internal void SetController(Controller cntrl)
        {
            _cntrl = cntrl;
        }

      
    }
}
