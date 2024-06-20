using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CRA.ModelLayer.SCC
{
    partial class FormSCC
    {
        /// <summary>Agiunge alla griglia Input o Output una nuova riga in base al selettore radio button </summary>
        /// <param name="variable"></param>
        /// <param name="domainClass"></param>
        private void DgwInOutAddRow(string variable, string domainClass)
        {
            if (rdbInput.Checked)
            {
                #region non posso aggiungere agli Input lo stesso Input

                if (dgwInputs.ToListOfArrayStrings().AlreadySet(variable, domainClass))
                {
                    var alert = string.Format("The variable \"{0}\" is already set as input", variable);
                    var caption = string.Format("Input already set");
                    MessageBox.Show(alert, caption, MessageBoxButtons.OK);
                    return;
                }

                #endregion
                #region l'utente può scegliere se aggiungere un input avendolo già definito come output

                if (dgwOutputs.ToListOfArrayStrings().AlreadySet(variable, domainClass))
                {
                    var alert =
                        string.Format(
                            "You have specified the variable \"{0}\" both as input and as output.\nIn general, this is poor specification practice for a strategy, and it can be considered acceptable for auxiliary variables.\nContinue?",
                            variable);
                    var caption = string.Format("Add as Input but already set as Output");
                    if (
                        MessageBox.Show(alert, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1).Equals(DialogResult.Cancel)) return;
                }

                #endregion
                dgwInputs.Rows.Add();
                dgwInputs.Rows[dgwInputs.Rows.Count - 1].Cells[0].Value = variable;
                dgwInputs.Rows[dgwInputs.Rows.Count - 1].Cells[1].Value = domainClass;
            }
            else
            {
                #region non posso aggiungere agli Outputs lo stesso Output

                if (dgwOutputs.ToListOfArrayStrings().AlreadySet(variable, domainClass))
                {
                    var alert = string.Format("The variable \"{0}\" is already set as output", variable);
                    var caption = string.Format("Output already set");
                    MessageBox.Show(alert, caption, MessageBoxButtons.OK);
                    return;
                }

                #endregion
                #region l'utente può scegliere se aggiungere un onput avendolo già definito come input

                if (dgwInputs.ToListOfArrayStrings().AlreadySet(variable, domainClass))
                {
                    var alert =
                        string.Format(
                            "You have specified the variable \"{0}\" both as input and as output.\nIn general, this is poor specification practice for a strategy, and it can be considered acceptable for auxiliary variables.\nContinue?",
                            variable);
                    var caption = string.Format("Add as Output but already set as Input");
                    if (
                        MessageBox.Show(alert, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1).Equals(DialogResult.Cancel)) return;
                }

                #endregion
                dgwOutputs.Rows.Add();
                dgwOutputs.Rows[dgwOutputs.Rows.Count - 1].Cells[0].Value = variable;
                dgwOutputs.Rows[dgwOutputs.Rows.Count - 1].Cells[1].Value = domainClass;
            }
        }

        #region eventi sulla lstVariables
        /// <summary> </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstVariables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string domainClass = cmbDomainClass.SelectedItem.ToString();
            string variable = lstVariables.SelectedItemToVariable();
            DgwInOutAddRow(variable, domainClass);
        }
        #endregion
    }

    #region metodi che estendono i metodi sulle DataGridView di Input e Output
    /// <summary>Estensione sulle DataGridView di Input e Output"</summary>
    public static class DgwInOutExtension
    {
        /// <summary>Estensione sulle DataGridView di Input e Output che converte la row in una lista di array[0] = "variabile"  e array[1] = "DomainClass"</summary>
        /// <param name="dgwInOut">DataGridView Input / Output</param>
        /// <returns>Lista di array di stringhe [Input],[DomainClass]</returns>
        public static IEnumerable<string[]> ToListOfArrayStrings(this DataGridView dgwInOut)
        {
             return dgwInOut.Rows.OfType<DataGridViewRow>().Select(a => new[] { (string)a.Cells[0].Value, (string)a.Cells[1].Value }).ToList();
        }
        /// <summary>Estensione sulle DataGridView di Input e Output che converte la row in una lista di array[0] = "variabile"  e array[1] = "DomainClass"</summary>
        /// <param name="dgwInOut">DataGridView Input / Output</param>
        /// <returns>Lista di array di classe InOutPar</returns>
        public static IEnumerable<InOutPar> ToListOfInOutPar(this DataGridView dgwInOut)
        {
            return dgwInOut.Rows.OfType<DataGridViewRow>().Select(a => new InOutPar() { PropertyName = (string)a.Cells[0].Value, DomainClass = (string)a.Cells[1].Value }).ToList();
        }

        /// <summary>Estensione sulle lista di array di stringhe [variable],[DomainClass] che verifica se è già presente un Input / Output</summary>
        /// <param name="listOfArrayStrings">Lista di array di stringhe [variable],[DomainClass]</param>
        /// <param name="variable">Primo elemento (variable) della griglia di Input / Output</param>
        /// <param name="domainClass">Secondo elemento (DomainClass) della griglia di Input / Output</param>
        /// <returns>true se l'elemento è presente, false altrimenti</returns>
        public static bool AlreadySet(this IEnumerable<string[]> listOfArrayStrings, string variable, string domainClass)
        {
             return (listOfArrayStrings.FirstOrDefault(a => a[0].Equals(variable) && a[1].Equals(domainClass)) != null);
        }
        
    }
    #endregion


    #region metodi che estendono i metodi sulle listview

    /// <summary>Estensione sulle listview della DomainClass"</summary>
    public static class VariableExtension
    {
        /// <summary>separatore definito per i 2 field che compongono la "lstVariables"</summary>
        private const char X = ' ';

        /// <summary>Estensione sulle DataGridView di Input e Output che converte la row in una lista di array[0] = "variabile"  e array[1] = "DomainClass"</summary>
        /// <param name="lstVariables">LisBox di liste di "variable"</param>
        /// <returns>Lista di array di stringhe [Input],[DomainClass]</returns>
        public static string SelectedItemToVariable(this ListBox lstVariables)
        {
            string variable = lstVariables.SelectedItem.ToString();
            int pos = variable.LastIndexOf(X) +1;
            return variable.Substring(pos, variable.Length - pos);
        }
    }

    #endregion


}
