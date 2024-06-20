using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CRA.ModelLayer.SCC
{
    public partial class InOutDuplicati : Form
    {
        public InOutDuplicati()
        {
            InitializeComponent();
        }

        private void InOutDuplicati_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("List of variables specified both as inputs and as outputs");
        }

        internal void SetMassage(IEnumerable<InOutPar> dups )
        {
            dgwInputsDups.Rows.Clear();
            foreach (var _inOutPar in dups)
            {
               dgwInputsDups.Rows.Add();
               dgwInputsDups.Rows[dgwInputsDups.Rows.Count - 1].Cells[0].Value = _inOutPar.PropertyName;
               dgwInputsDups.Rows[dgwInputsDups.Rows.Count - 1].Cells[1].Value = _inOutPar.DomainClass;
            }
        }

        private void btnDupsOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDupsCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


    }
}
