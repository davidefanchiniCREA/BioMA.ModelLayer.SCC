using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CRA.ModelLayer.Core;
using CRA.ModelLayer.Strategy;

using System.CodeDom;
using Microsoft.CSharp;

[assembly: InternalsVisibleTo("CRA.ModelLayer.SCC.BatchRun")]
namespace CRA.ModelLayer.SCC
{
    partial class FormSCC : Form
    {
        public FormSCC()
        {
            if (File.Exists("Lutil.lsf"))
            {
                MLLicense lic = new MLLicense();
                lic.ShowDialog();
            }
          
            InitializeComponent();
            _cntrl = new Controller();
            _cntrl.MessageToTheView += ShowMessage;

            //#LE#: controller per le associate
            _cntrlass = new AssStrategies();

            //fill the parameters ValueType column with the available VarInfoValueTypes
            ((DataGridViewComboBoxColumn)dgwParameters.Columns["ValueType"]).Items.AddRange(VarInfoValueTypes.Values.Select(a => a.Name).ToArray());
        }

        private void ShowMessage(string msg)
        {
            MessageBox.Show(msg);
        }

        private void FormSCC_Load(object sender, EventArgs e)
        {
            string codd = String.Empty;
            try
            {
                //codd = ApplicationDeployment.CurrentDeployment.DataDirectory + "/";
            }
            catch { }
            // Load & synchronize Domains
            _dsDomain.ReadXml(codd+"ListOfDomains.xml");
            for (int ind = 0; ind < _dsDomain.Tables[0].Rows.Count; ind++)
            {
                cmbDomain.Items.Add(_dsDomain.Tables[0].Rows[ind][0]);
            }
            _cntrl.Domains = _dsDomain;
            // Load & synchronize ModelTypes
            _dsModelType = new DataSet("ModelType");
            _dsModelType.ReadXml(codd+"ListOfModelTypes.xml");
            for (int ind = 0; ind < _dsModelType.Tables[0].Rows.Count; ind++)
            {
                cmbModelType.Items.Add(_dsModelType.Tables[0].Rows[ind][0]);
            }
            _cntrl.ModelTypes = _dsModelType;
        }

        #region Fields

        Controller _cntrl;
        DataSet _dsDomain = new DataSet();
        DataSet _dsModelType = new DataSet();
        
        /// <summary>controller per le associate</summary>
        private AssStrategies _cntrlass;
        #endregion

        #region Events synchronizing controller properties

        private void cmbDomain_TextChanged(object sender, EventArgs e)
        {
            _cntrl.Domain = cmbDomain.Text;
        }

        private void cmbModelType_TextChanged(object sender, EventArgs e)
        {
            _cntrl.ModelType = cmbModelType.Text;
        }

        private void txtNamespace_Leave(object sender, EventArgs e)
        {
            if (txtNamespace.Text != String.Empty)
                _cntrl.NameSpace = txtNamespace.Text;
        }

        private void txtStrategyName_Leave(object sender, EventArgs e)
        {
            if (txtStrategyName.Text != String.Empty)
                _cntrl.StrategyName = txtStrategyName.Text;
        }

        private void txtStrategyDescription_Leave(object sender, EventArgs e)
        {
            if (txtStrategyDescription.Text != String.Empty)
                _cntrl.StrategyDescription = txtStrategyDescription.Text;
        }

        private void dgwParameters_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgwParameters.RowCount > 1)
            {
                _cntrl.Parameters.Clear();
                for (int rowPar = 0; rowPar < dgwParameters.RowCount - 1; rowPar++)
                {
                    object name = dgwParameters.Rows[rowPar].Cells["NameVar"].Value;
                    object Description = dgwParameters.Rows[rowPar].Cells["Description"].Value;
                    object MaxValue = (dgwParameters.Rows[rowPar].Cells["MaxValue"].Value);
                    object MinValue = (dgwParameters.Rows[rowPar].Cells["MinValue"].Value);
                    object DefaultValue = (dgwParameters.Rows[rowPar].Cells["DefaultValue"].Value);
                    object Units = dgwParameters.Rows[rowPar].Cells["Units"].Value;
                    object ValueType = dgwParameters.Rows[rowPar].Cells["ValueType"].EditedFormattedValue;

                    //2015-09-11: la lista è popolata con il primo record vuoto per l'inserimento
                    //quindi è necessario se non valorizzata, fare un bello skip
                    if ( name == null) break;
                    VarInfo v = new VarInfo();
                    v.Name = name.ToString().Trim();

                    if (!(name is System.DBNull || Description is System.DBNull || MaxValue is System.DBNull || MinValue is System.DBNull || DefaultValue is System.DBNull || Units is System.DBNull || ValueType is System.DBNull
                        || name == null || Description == null || MaxValue == null || MinValue == null || DefaultValue == null || Units == null || ValueType == null))
                    {

                        v.Description = Description.ToString().Replace("\"", "'");
                        v.MaxValue = double.Parse(MaxValue.ToString());
                        v.MinValue = double.Parse(MinValue.ToString());
                        v.DefaultValue = double.Parse(DefaultValue.ToString());
                        v.Units = Units.ToString();

                        if (v.MaxValue < v.MinValue) { MessageBox.Show("Error: Min value of parameter '" + v.Name + "' greater than the max value"); continue; }
                        if (v.MaxValue < v.DefaultValue) { MessageBox.Show("Error: Default value of parameter '" + v.Name + "' greater than the max value"); continue; }
                        if (v.DefaultValue < v.MinValue) { MessageBox.Show("Error: Min value of parameter '" + v.Name + "' greater than the default value"); continue; }

                        try
                        {
                            v.ValueType = VarInfoValueTypes.GetInstanceForName(ValueType.ToString());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error setting ValueType field. Value not valid. Valid values are: " + VarInfoValueTypes.Values.Select(a => a.Name).Aggregate((a, b) => a = a + "," + b));
                            continue;
                        }

                    }

                    if (!_cntrl.Parameters.ContainsKey(v.Name.Trim())) _cntrl.Parameters.Add(v.Name.Trim(), v);
                }
            }
        }



        private void dgwOutputs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgwOutputs.Rows.Count > 0)
            {
                _cntrl.Outputs.Clear();
                for (int inx = 0; inx < dgwOutputs.Rows.Count; inx++)
                {

                    object output = dgwOutputs.Rows[inx].Cells["Outputs"].Value;
                    object outputDomainClass = dgwOutputs.Rows[inx].Cells["OutputDomainClass"].Value;
                    if (output != null && outputDomainClass != null)
                    {
                        if (!_cntrl.Outputs.ContainsKey(output.ToString().Trim())) _cntrl.Outputs.Add(output.ToString().Trim(), outputDomainClass.ToString().Trim());
                    }
                }
            }
        }

        private void dgwOutputs_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {

        }

        private void dgwOutputs_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (dgwOutputs.Rows.Count > 0)
            {
                _cntrl.Outputs.Clear();
                for (int inx = 0; inx < dgwOutputs.Rows.Count; inx++)
                {

                    object output = dgwOutputs.Rows[inx].Cells["Outputs"].Value;
                    object outputDomainClass = dgwOutputs.Rows[inx].Cells["OutputDomainClass"].Value;
                    if (output != null && outputDomainClass != null)
                    {
                        if (!_cntrl.Outputs.ContainsKey(output.ToString().Trim())) _cntrl.Outputs.Add(output.ToString().Trim(), outputDomainClass.ToString().Trim());
                    }
                }
            }
        }


        private void dgwInputs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgwInputs.Rows.Count > 0)
            {
                _cntrl.Inputs.Clear();
                for (int inx = 0; inx < dgwInputs.Rows.Count; inx++)
                {

                    object input = dgwInputs.Rows[inx].Cells["Inputs"].Value;
                    object inputDomainClass = dgwInputs.Rows[inx].Cells["InputDomainClass"].Value;
                    if (input != null && inputDomainClass != null)
                    {
                        if (!_cntrl.Inputs.ContainsKey(input.ToString().Trim())) _cntrl.Inputs.Add(input.ToString().Trim(), inputDomainClass.ToString().Trim());
                    }
                }
            }
        }

        private void dgwInputs_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {

        }

        private void dgwInputs_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (dgwInputs.Rows.Count > 0)
            {
                _cntrl.Inputs.Clear();
                for (int inx = 0; inx < dgwInputs.Rows.Count; inx++)
                {

                    object input = dgwInputs.Rows[inx].Cells["Inputs"].Value;
                    object inputDomainClass = dgwInputs.Rows[inx].Cells["InputDomainClass"].Value;
                    if (input != null && inputDomainClass != null)
                    {
                        if (!_cntrl.Inputs.ContainsKey(input.ToString().Trim())) _cntrl.Inputs.Add(input.ToString().Trim(), inputDomainClass.ToString().Trim());
                    }
                }
            }
        }

        private void chkIsContext_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsContext.Checked)
            {
                _cntrl.IsContext = "true";
            }
            else
            {
                _cntrl.IsContext = "false";
            }
        }

        private void txtTimeStep_Leave(object sender, EventArgs e)
        {
            if (txtTimeStep.Text != String.Empty)
                foreach (string str in txtTimeStep.Lines)
                {
                    // TODO: add check that it can be parsed to integer
                    if (str != String.Empty)
                        _cntrl.TimeSteps.Add(str);
                }
        }
      
        private void lblDataInterfacesDLL_Leave(object sender, EventArgs e)
        {
            _cntrl.DataInterfacesDLL = lblDataInterfacesDLL.Text;
        }

        private void lblStrategiesDLL_Leave(object sender, EventArgs e)
        {
            _cntrl.StrategiesDLL = lblStrategiesDLL.Text;
        }

        private void rdbSimple_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbSimple.Checked) _cntrl.SimpleStrategy = "checked";
            else _cntrl.SimpleStrategy = "";
            
        }


        private void rdbCompositeContext_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbCompositeContext.Checked)
            {
                _cntrl.CompositeStrategy = "checked";
                this.tabControl1.TabPages.Insert(1, this.tabAssociatedeStrategies);
                var altezzaI = this.dgwInputs.Size;
                altezzaI.Height = 170;
                this.dgwInputs.Size = altezzaI;
                var altezzaO = this.dgwOutputs.Size;
                altezzaO.Height = 170;
                this.dgwOutputs.Size = altezzaO;
                this.dgwInputsAssociated.Visible = true;
                this.dgwOutputsAssociated.Visible = true;
                //this.dgwParametersAssociated.Visible = true;
                this.label20.Visible = true;
                this.label21.Visible = true;
                //2016-06-05: si passa alla visione composite strategy
                //quindi nascondo il pulsante per vedere i tutti i parametri di tutte le strategy
                //ripulisco la tabella dei parametri associati
                this.StrategiesFullParams.Visible = false;
                this.dgwParametersAssociated.Rows.Clear();
            }
            else {
                _cntrl.CompositeStrategy = "";
                this.tabControl1.Controls.Remove(this.tabAssociatedeStrategies);
                var altezza = this.dgwInputs.Size;
                altezza.Height = 385;
                this.dgwInputs.Size = altezza;
                this.dgwOutputs.Size = altezza;
                this.dgwInputsAssociated.Visible = false;
                this.dgwOutputsAssociated.Visible = false;
                //this.dgwParametersAssociated.Visible = false;
                this.label20.Visible = false;
                this.label21.Visible = false;
                //2016-06-05: si passa alla visione simple strategy
                //ripulisco l'elenco delle strategy
                //quindi visualizzo il pulsante per vedere i tutti i parametri di tutte le strategy
                //ripulisco le tabelle composite
                this.StrategiesFullParams.Visible = true;
                // davide - prova
                // commento questa perché ciò che avevo selezionato come Associated deve permanere - begin
                //lstAssociatedStrategies.Items.Clear();
                //if (!string.IsNullOrWhiteSpace(lblStrategiesDLL.Text))
                //    ReflectAssemblyStrategies(lblStrategiesDLL.Text);
                // commento questa perché ciò che avevo selezionato come Associated deve permanere - end
            }
        }
        
        private void txtAuthor_Leave(object sender, EventArgs e)
        {
            _cntrl.Author = txtAuthor.Text;
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            _cntrl.Email = txtEmail.Text;
        }

        private void txtInstitution_Leave(object sender, EventArgs e)
        {
            _cntrl.Institution = txtInstitution.Text;
        }

        private void dateTimePickerFirstRelease_Leave(object sender, EventArgs e)
        {
            _cntrl.DateFirstRelease = dateTimePickerFirstRelease.Text;
        }

        private void txtAuthor2_Leave(object sender, EventArgs e)
        {
            _cntrl.AuthorRevision = txtAuthor2.Text;
        }

        private void txtEmail2_Leave(object sender, EventArgs e)
        {
            _cntrl.EmailRevision = txtEmail2.Text;
        }

        private void dateTimePickerRelease2_Leave(object sender, EventArgs e)
        {
            _cntrl.DateRevision = dateTimePickerRelease2.Text;
        }

        #endregion

        #region Events for tab Global

        private void btnDataInterfaces_Click(object sender, EventArgs e)
        {
            // Clean combo Domain classes
            cmbDomainClass.Items.Clear();

            // Extract 
            openFileDialog1.Filter = "Domain classes and interfaces assemblies (*.dll)|*.dll";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fname = openFileDialog1.FileName;
                ReflectAssemblyDataInterfaces(fname);
            }
        }

        private void ReflectAssemblyDataInterfaces(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName)) return;

            lblDataInterfacesDLL.Text = assemblyName;

            // Discover the higher lever interface inheriting from IStrategy         
            Assembly asse = Assembly.LoadFrom(assemblyName);

            _cntrl.LoadedAssemblies = new List<string>();
            _cntrl.LoadedAssemblies.Add(asse.FullName);
            foreach (AssemblyName assed in asse.GetReferencedAssemblies())
            {
                _cntrl.LoadedAssemblies.Add(assed.FullName);
            }

            Type[] types = asse.GetTypes();

            Type higherLevelStrategyInterface = null;
            bool cannotFindAnyHierarchyBetweenInterfaces = true;
            foreach (Type ty in types)
            {
                //if it is an interface and extends one of the 2 IStrategy
                if (ty.IsInterface &&
                    (typeof (CRA.ModelLayer.Strategy.IStrategy).IsAssignableFrom(ty) 
                    //|| typeof (CRA.Core.Preconditions.IStrategy).IsAssignableFrom(ty)
                     ))
                {
                    if (higherLevelStrategyInterface == null) higherLevelStrategyInterface = ty;
                    else
                    {
                        if (!higherLevelStrategyInterface.IsAssignableFrom(higherLevelStrategyInterface))
                        {
                            higherLevelStrategyInterface = ty; //to get the higher one
                            cannotFindAnyHierarchyBetweenInterfaces = false;
                        }
                    }
                }
            }
            if (higherLevelStrategyInterface != null)
            {

                string nameInterface = higherLevelStrategyInterface.ToString();

                // Get namespace
                int pos = nameInterface.IndexOf("Interfaces");
                if (pos > -1)
                    if (string.IsNullOrEmpty(txtNamespace.Text))
                        txtNamespace.Text =
                            nameInterface.Substring(0, nameInterface.Length - (nameInterface.Length - pos)) +
                            "Strategies";

                txtNamespace.Focus();

                lblIStrategyInterface.Text = higherLevelStrategyInterface.Name;

                // Discover method))
                if (higherLevelStrategyInterface.FullName == nameInterface)
                {
                    MemberInfo[] ms = higherLevelStrategyInterface.GetMembers();
                    foreach (MemberInfo m in ms)
                    {
                        if (m.MemberType == MemberTypes.Method && m.ToString().Contains("Void"))

                        {
                            if (!m.Name.Contains("Conditions") && !m.Name.Equals("SetParametersDefaultValue"))
                            {
                                _cntrl.ModelMethodName = m.Name;

                                string modelCall = m.ToString();

                                int openP = modelCall.IndexOf('(');
                                int closeP = modelCall.IndexOf(')');
                                string typesParameters = modelCall.Substring(openP + 1,
                                    modelCall.Length - openP -
                                    (modelCall.Length - closeP + 1));
                                string[] singleDomainClasses = typesParameters.Split(',');

                                _cntrl.DomainClassTypeAndInstances = new Dictionary<string, Type>();
                                for (int inx = 0; inx < (singleDomainClasses.Count()); inx++)
                                {
                                    string[] dom = singleDomainClasses[inx].Split('.');
                                    string instance = dom[dom.Count() - 1].ToLower();
                                    if (_cntrl.DomainClassTypeAndInstances.Keys.Contains(instance))
                                        instance = instance + "1";

                                    //try load the type of the domain class
                                    Type methodArgumentType = asse.GetType(singleDomainClasses[inx]);

                                    //if the type of the domain class is null try to load it from a referenced assembly (e.g. cra.agrmanagement)
                                    if (methodArgumentType == null)
                                    {
                                        foreach (AssemblyName assed in asse.GetReferencedAssemblies())
                                        {
                                            try
                                            {
                                                methodArgumentType =
                                                    Assembly.LoadFile(Path.GetDirectoryName(asse.Location) + "\\" +
                                                                      assed.Name + ".dll")
                                                        .GetType(singleDomainClasses[inx]);
                                            }
                                            catch (FileNotFoundException)
                                            {
                                            }

                                            if (methodArgumentType != null) break;
                                        }
                                    }

                                    //if the type of the domain class is not null, add it to the DomainClassTypeAndInstances structure
                                    if (methodArgumentType != null)
                                        _cntrl.DomainClassTypeAndInstances.Add(instance, methodArgumentType);
                                }
                                string newModelCall = modelCall.Substring(0, openP + 1) +
                                                      _cntrl.DomainClassTypeAndInstances.Values.Select(a => a.Name)
                                                          .Aggregate((a, b) => a = a + "," + b) +
                                                      modelCall.Substring(closeP);

                                lblStrategyComponentInterface.Text = newModelCall;
                            }
                        }
                    }
                }

                cmbDomainClass.Items.Clear();
                //#LE#:2017-03-14: fix 1.2.0.17. vanno ripristinate le condizioni sulla combo e lista nel tab i/o
                cmbDomainClass.Text = "";
                lstVariables.Items.Clear();

                foreach (var pair in _cntrl.DomainClassTypeAndInstances)
                {
                    if ( //pair.Value.IsClass &&
                        pair.Value.IsPublic)
                    {
                        // TODO: delete reference to old preconditions but leave IDomainClass
                        if (
                            //typeof (CRA.Core.Preconditions.IDomainClass).IsAssignableFrom(pair.Value) ||
                            typeof (CRA.ModelLayer.Core.IDomainClass).IsAssignableFrom(pair.Value))
                        {
                            cmbDomainClass.Items.Add(Controller.GetCmbDomainClassFromDomainClassTypeAndId(pair.Value,
                                pair.Key));
                        }
                    }
                }
            }
        }

        //TODO:#LE#: 2017-02-27: secondo me non serve e non è detto che funzioni

        //#LE#: mutuata da una analoga in CodeParameters
        //static string GetFullName(Type t)
        //{
        //    if (!t.IsGenericType)
        //        return t.Name;
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
        //    sb.Append(t.GetGenericArguments().Aggregate("<",

        //        delegate(string aggregate, Type type)
        //        {
        //            return aggregate + (aggregate == "<" ? "" : ",") + GetFullName(type);
        //        }
        //        ));
        //    sb.Append(">");

        //    return sb.ToString();
        //}

        private void cmbDomainClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstVariables.Items.Clear();
            try
            {
                string domainClassTypeFullName =Controller.GetDomainClassTypeFromCmbDomainClass( cmbDomainClass.SelectedItem.ToString());
                string fname = lblDataInterfacesDLL.Text;
                Assembly asse = Assembly.LoadFile(fname);
                Type theType = asse.GetType(domainClassTypeFullName);

                //try in the referenced assemblies
                if (theType == null)
                {

                    foreach (AssemblyName assed in asse.GetReferencedAssemblies())
                    {
                        try
                        {
                            theType =
                                Assembly.LoadFile(Path.GetDirectoryName(asse.Location) + "\\" + assed.Name + ".dll")
                                    .GetType(domainClassTypeFullName);
                        }
                        catch
                        {

                        }

                        if (theType != null) break;
                    }
                }

                if (theType != null)
                {
                    MemberInfo[] mbrInfoArray = theType.GetProperties()
                        //2016-05-17: RICHIESTA: la stringa Description e URL non dovrebbero comparire nella listbox del tab Inputs-Outputs
                        //RISPOSTA: non devono essere proprietà virtuali
                        .Where(p => p.GetMethod.IsVirtual == false)
                        .OrderBy(a => a.Name).ToArray();

                    foreach (MemberInfo m in mbrInfoArray)
                    {
                        //#LE#:2017-02-15: in errore per t.IsGenericType
                        if (m.MemberType == MemberTypes.Property)
                        {
                            // funzione di utilita per recuperare il tipo ritornato dalla proprietà
                            var aatt = ((PropertyInfo)m);
                            String temp = GetFriendlyTypeName(aatt.PropertyType);
                            //
                            StringBuilder sb = new StringBuilder();
                            // trovo l'ultimo punto di System.Collection.blabla in modo da avere solo dictionary
                            sb.Append(temp.Substring(temp.LastIndexOf(".") + 1));
                            sb.Append(" ");
                            sb.Append(aatt.Name);
                            //
                            lstVariables.Items.Add(sb);                            
                        }
                    }
                }

                //2016-05-12: se semplici quando carico una strategy nel menu e la interface 
                #region #LE#: cerco i parametri per l'elemento selezionato
                //// per ogni in UNIMI.CropML.Interfaces
                foreach (var pair in _cntrl.DomainClassTypeAndInstances) // su tutte le domain classes
                {
                    // se la domain class ha nel namespace (UNIMI.CropML.Interfaces.Exogeus e UNIMI.CropML.Interfaces.ExogeusVarInfo) la classe che mi interessa (Exogeus)
                    if (pair.Key.ToLower().Equals(theType.Name.ToLower()))
                    {
                        // se la classe che mi interessa implementa l'interfaccia IVarInfoClass
                        foreach (var _type in asse.GetTypes())
                        {
                            if (_type.FullName.ToLower().Contains(pair.Key.ToLower()) &&
                                !_type.FullName.ToLower().Equals(pair.Value.ToString().ToLower()))
                            {
                                // beccata la classe, verifico che implementi IVarInfoClass
                                var aaaaa = _type.GetInterfaces();

                                bool passa = false;
                                foreach (Type aaType in aaaaa)
                                {
                                    if (aaType == typeof(IVarInfoClass)) // controllo che sia IVarInfoClass
                                    {
                                        passa = true;
                                    }
                                }

                                // nella selezione degli input NON deve calcolare i parametri
                                passa = false;
                                if (passa)
                                {
                                    // eseguo il metodo per richiamare i parametri
                                    var ele = asse.CreateInstance(_type.ToString());
                                    foreach (var g in _type.GetProperties())
                                    {
                                        if ((g.GetValue(ele, null)).GetType() == typeof(VarInfo))
                                        {
                                            // se ritorna VarInfo
                                            var _p = (VarInfo)g.GetValue(ele, null);

                                            //dgwParameters.CancelEdit();                                                
                                            dgwParameters.Rows[0].Cells[0].Value = _p.Name;
                                            dgwParameters.Rows[0].Cells[1].Value = _p.Description.Replace("\"", "'");
                                            dgwParameters.Rows[0].Cells[2].Value = _p.MinValue.ToString();
                                            dgwParameters.Rows[0].Cells[3].Value = _p.MaxValue.ToString();
                                            dgwParameters.Rows[0].Cells[4].Value = _p.DefaultValue.ToString();
                                            dgwParameters.Rows[0].Cells[5].Value = _p.Units;
                                            var ee = ((DataGridViewComboBoxCell)dgwParameters.Rows[0].Cells[6]).Items;
                                            int idx = ee.IndexOf(_p.ValueType.Name);
                                            ((DataGridViewComboBoxCell)dgwParameters.Rows[0].Cells[6]).Value = ee[idx];
                                            dgwParameters.CommitEdit(DataGridViewDataErrorContexts.Commit);
                                            dgwParameters.Rows.Insert(0, 1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            catch(Exception ex)
            {
                var alert = string.Format("Either the interfaces assembly or the domain class are missing; the file selected is not a CRA model/interface assembly.");
                var caption = string.Format("Reflection Data DLL");
                MessageBox.Show(alert, caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>Funzione di utilità per gestire i tipi non di base (ES: dictionary)/// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetFriendlyTypeName(Type type)
        {
            using (var p = new CSharpCodeProvider())
            {
                var r = new CodeTypeReference(type);
                return p.GetTypeOutput(r);
            }
        }

        /// <summary>esegue la reflaction sulla DLL strategy per estrarre input/output. I parametri sono stati spostati</summary>
        /// <param name="assemblyName"></param>
        private void ReflectAssemblyStrategies(string assemblyName)
        {
            //check
            if (assemblyName == null || assemblyName.Equals(String.Empty)) return;
            //
            cmbAssociatedStrategies.Items.Clear();

            //gestione associate                
            _cntrlass.WhenStrategyClassAddedOrRemoved(
                lblStrategiesDLL.Text, cmbAssociatedStrategies.Text,
                AssStrategies.Actions.STRATEGY_REMOVEALL,
                dgwInputs, dgwOutputs, dgwParameters,
                dgwInputsAssociated, dgwOutputsAssociated, dgwParametersAssociated);

            lblStrategiesDLL.Text = assemblyName;
            Assembly a = Assembly.LoadFrom(assemblyName);
            Type[] types = a.GetTypes().OrderBy(ad=>ad.FullName).ToArray();
            
            foreach (Type t in types)
            {
                try
                {
                    // Discover classes 
                    if (t.IsClass)
                    {
                        //TODO: togliere old IStrategy
                        if (
                            //typeof(CRA.Core.Preconditions.IStrategy).IsAssignableFrom(t) || 
                            typeof(IStrategy).IsAssignableFrom(t))
                        {
                            cmbAssociatedStrategies.Items.Add(t.FullName);
                        }
                    }
                }
                catch
                { 
                    // No action 
                }
            }
        }

        /// <summary>Salva il file di configurazione</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDefinition_Click(object sender, EventArgs e)
        {
            try
            {
                #region 2016-05-19: controllo se ci sono inputs/outputs uguali
                var dups = (dgwInputs.ToListOfInOutPar().Intersect(dgwOutputs.ToListOfInOutPar(), new InputOutputComparer())).ToList();
                if (dups.Any())
                {
                    var f = new InOutDuplicati();
                    f.SetMassage(dups);
                    if (f.ShowDialog() != DialogResult.OK) return;
                }
                #endregion

                if (_cntrl.FillDataSetDefinition() && !string.IsNullOrWhiteSpace(saveFileDialog1.FileName))
                {
                    _cntrl.SaveDataSetDefinition(saveFileDialog1.FileName);
                }
                else btnSaveAsDefinition_Click(sender, e);
            }
            catch (Exception eee) {this.ShowMessage(eee.Message);}
        }

        private void btnStrategiesDLL_Click(object sender, EventArgs e)
        {          
            // Extract 
            openFileDialog1.Filter = "Strategies assemblies (*.dll)|*.dll";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fname = openFileDialog1.FileName;
                ReflectAssemblyStrategies(fname);
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBox aboutScc = new AboutBox();
            aboutScc.Show();
        }

        private void RefreshView()
        {
            // Global
            if (_cntrl.Definition.Tables["Global"].Rows[0]["SimpleStrategy"].ToString() == "checked") rdbSimple.Checked = true;
            if (_cntrl.Definition.Tables["Global"].Rows[0]["CompositeStrategy"].ToString() == "checked") rdbCompositeContext.Checked = true;
            lblDataInterfacesDLL.Text = _cntrl.Definition.Tables["Global"].Rows[0]["DataInterfacesDLL"].ToString();
            lblStrategiesDLL.Text = _cntrl.Definition.Tables["Global"].Rows[0]["StrategiesDLL"].ToString();
            // General

            string val = _cntrl.Definition.Tables["General"].Rows[0]["Domain"].ToString();

            if (cmbDomain.Items.Contains(val))
            {
                int i = 0;
                foreach (var s in cmbDomain.Items)
                {
                    if (s!=null && ((string)s).Equals(val))
                    {
                        cmbDomain.SelectedIndex = i;
                    }
                    i++;
                }
            }


            string val2 = _cntrl.Definition.Tables["General"].Rows[0]["ModelType"].ToString();

            if (cmbModelType.Items.Contains(val2))
            {
                int i = 0;
                foreach (var s in cmbModelType.Items)
                {
                    if (s != null && ((string)s).Equals(val2))
                    {
                        cmbModelType.SelectedIndex = i;
                    }
                    i++;
                }
            }


         
            txtStrategyName.Text = _cntrl.Definition.Tables["General"].Rows[0]["StrategyName"].ToString();
            txtNamespace.Text = _cntrl.Definition.Tables["General"].Rows[0]["Namespace"].ToString();
            txtStrategyDescription.Text = _cntrl.Definition.Tables["General"].Rows[0]["Strategydescription"].ToString();
            string[] _timeSteps =
                _cntrl.Definition.Tables["General"].Rows[0]["TimeSteps"].ToString().Split(',');
            txtTimeStep.Lines = _timeSteps;
            if (_cntrl.Definition.Tables["General"].Rows[0]["IsContext"].ToString() == "checked")
                chkIsContext.Checked = true;
            txtAuthor.Text = _cntrl.Definition.Tables["General"].Rows[0]["AuthorFirstRelease"].ToString();
            txtInstitution.Text = _cntrl.Definition.Tables["General"].Rows[0]["Institution"].ToString();
            txtEmail.Text = _cntrl.Definition.Tables["General"].Rows[0]["Email"].ToString();
            dateTimePickerFirstRelease.Text = _cntrl.Definition.Tables["General"].Rows[0]["DateFirstRelease"].ToString();
            txtAuthor2.Text = _cntrl.Definition.Tables["General"].Rows[0]["AuthorRevision"].ToString();
            txtEmail2.Text = _cntrl.Definition.Tables["General"].Rows[0]["EmailAuthorRevision"].ToString();
            dateTimePickerRelease2.Text = _cntrl.Definition.Tables["General"].Rows[0]["DateRevision"].ToString();
            
            // Assoc strat
            _cntrl.AssociatedStrategies = new Dictionary<string, string>();
            _cntrl.OrderedAssociatedStrategies = new List<string>();
            lstAssociatedStrategies.Items.Clear();
            for (int inx = 0; inx < _cntrl.Definition.Tables["AssociatedStrategies"].Rows.Count; inx++)
            {
                object o =  _cntrl.Definition.Tables["AssociatedStrategies"].Rows[inx]["StrategyFullName"]                 ;
                lstAssociatedStrategies.Items.Add((string)o);
                if (!_cntrl.AssociatedStrategies.ContainsKey((string)o))
                {
                    _cntrl.AssociatedStrategies.Add((string)o, ((string)o).Split(".".ToCharArray()).Last().ToLower());
                    _cntrl.OrderedAssociatedStrategies.Add((string)o);
                }
            }

            

            //TODO: forse sopra nel ciclo, ma se faccio come se ci fosse un reorder, ma non ho controllo sugli elementi aggiunti
            //#LE#:gestione associate                
            _cntrlass.WhenStrategyClassAddedOrRemoved(
                lblStrategiesDLL.Text, cmbAssociatedStrategies.Text,
                AssStrategies.Actions.STRATEGY_REORDER,
                dgwInputs, dgwOutputs, dgwParameters,
                dgwInputsAssociated, dgwOutputsAssociated, dgwParametersAssociated);

            //Switches
            if (_cntrl.Switches==null) _cntrl.Switches = new Dictionary<string, BuildingSwitch>();
            flowLayoutPanel1.Controls.Clear();
            for (int inx = 0; inx < _cntrl.Definition.Tables["Switches"].Rows.Count; inx++)
            {
                string swname = (string)_cntrl.Definition.Tables["Switches"].Rows[inx]["SwitchName"];
                string swdesc = (string)_cntrl.Definition.Tables["Switches"].Rows[inx]["SwitchDescription"];
                string swvalues = (string)_cntrl.Definition.Tables["Switches"].Rows[inx]["SwitchValues"];
                string[] swvals = swvalues.Split(Controller.SEPARATOR.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, ModellingOptions> d = new Dictionary<string, ModellingOptions>();
                foreach (string s in swvals) {
                    d.Add(s, null);
                }

                if (!_cntrl.Switches.ContainsKey((string)swname)) _cntrl.Switches.Add((string)swname, new BuildingSwitch(swname, swdesc, d,_cntrl.GeneralModelingOptions));

                var suc=new SwitchUserControl();
                suc.SetController(_cntrl);
                suc.SetSwitchName((string)swname);
                flowLayoutPanel1.Controls.Add(suc);
               
            }

            // Inputs
            dgwInputs.Rows.Clear();
            for (int inx = 0; inx < _cntrl.Definition.Tables["Inputs"].Rows.Count; inx++)
            {
                object[] o = { _cntrl.Definition.Tables["Inputs"].Rows[inx]["Variable"], _cntrl.Definition.Tables["Inputs"].Rows[inx]["DomainClass"] };
                dgwInputs.Rows.Add(o);
            }
            dgwInputs_CellValueChanged(null, null);

            // Outputs
            dgwOutputs.Rows.Clear();
            for (int inx = 0; inx < _cntrl.Definition.Tables["Outputs"].Rows.Count; inx++)
            {
                object[] o = { _cntrl.Definition.Tables["Outputs"].Rows[inx]["Variable"], _cntrl.Definition.Tables["Outputs"].Rows[inx]["DomainClass"] }
                ;
                dgwOutputs.Rows.Add(o);

            }
            dgwOutputs_CellValueChanged(null, null);

            // Parameters
            dgwParameters.Rows.Clear();

            for (int inx = 0; inx < _cntrl.Definition.Tables["Parameters"].Rows.Count; inx++)
            {
                object[] o = {
                                 _cntrl.Definition.Tables["Parameters"].Rows[inx]["VarName"],
                                 _cntrl.Definition.Tables["Parameters"].Rows[inx]["Description"],
                                 _cntrl.Definition.Tables["Parameters"].Rows[inx]["MinValue"],
                                 _cntrl.Definition.Tables["Parameters"].Rows[inx]["MaxValue"],
                                 _cntrl.Definition.Tables["Parameters"].Rows[inx]["DefaultValue"],
                                 _cntrl.Definition.Tables["Parameters"].Rows[inx]["Units"],
                                 _cntrl.Definition.Tables["Parameters"].Rows[inx]["ValueType"],
                             };
                dgwParameters.Rows.Add(o);
            }
            dgwParameters_CellValueChanged(null, null);
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            CodeParts.CodeUtilities.Warnings = String.Empty;

            if (_cntrl.DomainClassTypeAndInstances == null || string.IsNullOrEmpty(_cntrl.StrategyName) || string.IsNullOrEmpty(_cntrl.NameSpace))
            {
                MessageBox.Show("Plase set all the required fields: strategy name, strategy namespace, interfaces dll");
                return;
            }

            if (_cntrl.GenerateCode("StrategyTemplate.txt"))
            {
                saveFileDialog2.Filter = "C# code files (*.cs)|*.cs";
                saveFileDialog2.FileName = txtStrategyName.Text;
                if (DialogResult.OK == saveFileDialog2.ShowDialog())
                {
                    Writer w = new Writer();
                    w.Initialize();
                    w.Write(saveFileDialog2.FileName, _cntrl.UpdatedCode);

                    if (CodeParts.CodeUtilities.Warnings != String.Empty)
                        throw new Exception(CodeParts.CodeUtilities.Warnings);

                    if (MessageBox.Show("Code generation done.\r\n Do you want to see the generated file?", "SCC", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start(saveFileDialog2.FileName);
                    }
                }
            }
        }

        /// <summary>Carica il file delle configurazioni di SCC precedentemente salvato </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadDefinition_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "SCC definitions (*.xml)|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadConfigAndRefreshView(openFileDialog1.FileName);
                lblDefinitionFile.Text = openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf('\\')+1);

                //#LE#:2017-02-27: release 1.2.0.15 : fix 1
                lblDefinitionNameSaved.Text = lblDefinitionFile.Text;
                //per evitare il popup in apertura su pressione pulsante save
                saveFileDialog1.FileName = lblDefinitionFile.Text;
            }       
        }

        // davide - prova
        private List<string> GetOrderedAssociatedStrategies()
        {
            List<string> orderedAssociatedStrategies = new List<string>();
            foreach (object item in lstAssociatedStrategies.Items)
            {
                orderedAssociatedStrategies.Add((string)item);
            }
            return orderedAssociatedStrategies;
        }

        /// <summary>Ricarica la view con i dati salvati sul file XML di configurazione</summary>
        /// <param name="filename"></param>
        private void LoadConfigAndRefreshView(string filename)
        {
            _cntrl.LoadConfig(filename);
            //
            RefreshView();
            ReflectAssemblyDataInterfaces(lblDataInterfacesDLL.Text);
            ReflectAssemblyStrategies(lblStrategiesDLL.Text);

            foreach (string strategyClassName in this._cntrl.OrderedAssociatedStrategies)
            {
                _cntrlass.WhenStrategyClassAddedOrRemoved(
                        lblStrategiesDLL.Text, strategyClassName,
                        AssStrategies.Actions.STRATEGY_ADD,
                        dgwInputs, dgwOutputs, dgwParameters,
                        dgwInputsAssociated, dgwOutputsAssociated, dgwParametersAssociated);
            }
        }

        #endregion

        #region Events for tab General

        /// <summary>verifica se nella tabella (senza pKey) ci sono righe con valore pari a p2</summary>
        /// <param name="p1">tabella a singola colonna</param>
        /// <param name="p2">valore da ricercare</param>
        /// <returns></returns>
        private bool checkSingleTabelWithNoPKey(DataSet p1, string p2)
        {
            DataRow[] foundRows = p1.Tables[0].Select(p1.Tables[0].Columns[0].ColumnName + " = '" + p2 + "'");
            if (foundRows.Length>0) return true;
            return false;
        }

        private void btnSaveNewModelType_Click(object sender, EventArgs e)
        {
            string newitem = this.cmbModelType.Text;
            //#LE#: check se l'elemento è già presente
            if (checkSingleTabelWithNoPKey(_dsModelType, newitem)) return;
            _dsModelType.Tables[0].Rows.Add(newitem);
            string codd = String.Empty;
            try
            {
                //codd = ApplicationDeployment.CurrentDeployment.DataDirectory + "/";
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.StackTrace);
            }
            _dsModelType.WriteXml(codd + "ListOfModelTypes.xml");
            cmbModelType.Items.Add(newitem);
        }

        private void btnSaveNewDomain_Click(object sender, EventArgs e)
        {
            string newitem=this.cmbDomain.Text;
            //#LE#: check se l'elemento è già presente
            if (checkSingleTabelWithNoPKey(_dsDomain, newitem)) return;
            _dsDomain.Tables[0].Rows.Add(newitem);
            string codd = String.Empty;
            try
            {
                //codd = ApplicationDeployment.CurrentDeployment.DataDirectory + "/";
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.StackTrace);
            }           
            _dsDomain.WriteXml(codd + "ListOfDomains.xml");
            cmbDomain.Items.Add(newitem);
        }

        #endregion

        #region Events for tab InputsOutputs

        private void bntSelectVariable_Click(object sender, EventArgs e)
        {
            if (lstVariables.SelectedItem != null)
            {
                string domainClass = cmbDomainClass.SelectedItem.ToString();
                string variable = lstVariables.SelectedItemToVariable();
                DgwInOutAddRow(variable, domainClass);
            }
        }

        #endregion

        #region Events for tab Parameters


        #endregion

        #region Events for tab associated strategies

        private void cmbAssociatedStrategies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(cmbAssociatedStrategies.Text) && !_cntrl.AssociatedStrategies.ContainsKey(cmbAssociatedStrategies.Text))
            {
                lstAssociatedStrategies.Items.Add(cmbAssociatedStrategies.Text);
                _cntrl.AssociatedStrategies.Add(cmbAssociatedStrategies.Text, cmbAssociatedStrategies.Text.Split(".".ToCharArray()).Last().ToLower());
                RefreshListAssociatedStrategiesSwitches();

                //#LE#:gestione associate                
                _cntrlass.WhenStrategyClassAddedOrRemoved(
                    lblStrategiesDLL.Text, cmbAssociatedStrategies.Text, 
                    AssStrategies.Actions.STRATEGY_ADD,
                    dgwInputs,dgwOutputs,dgwParameters, 
                    dgwInputsAssociated,dgwOutputsAssociated,dgwParametersAssociated);

                _cntrl.OrderedAssociatedStrategies = GetOrderedAssociatedStrategies();
            }
        }

        private void RefreshListAssociatedStrategiesSwitches() {
            listBoxAssociatedStrategiesSwitches.Items.Clear();

            foreach (var assstr in _cntrl.AssociatedStrategies)
            {
                listBoxAssociatedStrategiesSwitches.Items.AddRange(_cntrl.GetSwitchesOfAssociatedStrategy(assstr.Key).Select(a => assstr.Value + " - " + a).Distinct().ToArray());
            }
        }

        private void lstAssociatedStrategies_DoubleClick(object sender, EventArgs e)
        {
            if (lstAssociatedStrategies.SelectedIndex>=0 && _cntrl.AssociatedStrategies.ContainsKey((string)lstAssociatedStrategies.Items[lstAssociatedStrategies.SelectedIndex]))
            {
                var associatedStrategy =
                    (string) lstAssociatedStrategies.Items[lstAssociatedStrategies.SelectedIndex];
                _cntrl.AssociatedStrategies.Remove(associatedStrategy);
                lstAssociatedStrategies.Items.RemoveAt(lstAssociatedStrategies.SelectedIndex);
                RefreshListAssociatedStrategiesSwitches();

                //NOTA: la posizione è importante, perchè uso un elemento che poi viene rimosso dal controllo
                //#LE#:gestione associate                
                _cntrlass.WhenStrategyClassAddedOrRemoved(
                    "*", associatedStrategy,
                    AssStrategies.Actions.STRATEGY_REMOVE,
                    dgwInputs, dgwOutputs, dgwParameters,
                    dgwInputsAssociated, dgwOutputsAssociated, dgwParametersAssociated);

                _cntrl.OrderedAssociatedStrategies = GetOrderedAssociatedStrategies();
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            //no elementi selezionati
            if (lstAssociatedStrategies.SelectedIndex.Equals(-1)) return;
            //solo 1 elemento, non ha senso lo spostamento
            if (lstAssociatedStrategies.Items.Count.Equals(1)) return;
            //seleziono ultimo elemento. Non posso scendere oltre
            if ((lstAssociatedStrategies.Items.Count - 1).Equals(lstAssociatedStrategies.SelectedIndex)) return;

            //CASO USO: 
            // A in posizione 0
            // B in posizione 1
            // seleziono A e voglio spostarlo in posizione B
            // quindi A-0, B-1 --> A-0, B-1, A-2 
            // poi cancello A-0
            // inserisco A-0 in posizione 2
            lstAssociatedStrategies.Items.Insert(lstAssociatedStrategies.SelectedIndex + 2, lstAssociatedStrategies.Items[lstAssociatedStrategies.SelectedIndex]);
            // cancello l'elemento A-0. Nota che avendo fatto l'inserimento in posizione successiva il mio indice non si modifica
            lstAssociatedStrategies.Items.RemoveAt(lstAssociatedStrategies.SelectedIndex);

            //#LE#:gestione associate                
            _cntrlass.WhenStrategyClassAddedOrRemoved(
                lblStrategiesDLL.Text, cmbAssociatedStrategies.Text,
                AssStrategies.Actions.STRATEGY_REORDER,
                dgwInputs, dgwOutputs, dgwParameters,
                dgwInputsAssociated, dgwOutputsAssociated, dgwParametersAssociated);

            _cntrl.OrderedAssociatedStrategies = GetOrderedAssociatedStrategies();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            //no elementi selezionati
            if (lstAssociatedStrategies.SelectedIndex.Equals(-1)) return;
            //solo 1 elemento, non ha senso lo spostamento
            if (lstAssociatedStrategies.Items.Count.Equals(1)) return;
            //seleziono primo elemento. Non posso salire oltre
            if (lstAssociatedStrategies.SelectedIndex.Equals(0)) return;

            //CASO USO: 
            // A in posizione 0
            // B in posizione 1
            // seleziono B e voglio spostarlo in posizione A
            // quindi A-0, B-1 --> B-0, A-1, B-2 
            // poi cancello B-2
            // inserisco B-1 in posizione 0
            lstAssociatedStrategies.Items.Insert(lstAssociatedStrategies.SelectedIndex - 1, lstAssociatedStrategies.Items[lstAssociatedStrategies.SelectedIndex]);
            // cancello l'elemento B-2. Nota che avendo fatto l'inserimento in 0 l'indice selezionato mi scala automaticamente
            lstAssociatedStrategies.Items.RemoveAt(lstAssociatedStrategies.SelectedIndex);

            //#LE#:gestione associate                
            _cntrlass.WhenStrategyClassAddedOrRemoved(
                lblStrategiesDLL.Text, cmbAssociatedStrategies.Text,
                AssStrategies.Actions.STRATEGY_REORDER,
                dgwInputs, dgwOutputs, dgwParameters,
                dgwInputsAssociated, dgwOutputsAssociated, dgwParametersAssociated);

            _cntrl.OrderedAssociatedStrategies = GetOrderedAssociatedStrategies();
        }
        #endregion

        private void lblIStrategyInterface_TextChanged(object sender, EventArgs e)
        {
            _cntrl.IStrategyComponentName = lblIStrategyInterface.Text;
        }

        private void dateTimePickerFirstRelease_ValueChanged(object sender, EventArgs e)
        {
            this.dateTimePickerFirstRelease_Leave(sender, e);
        }

        private void buttonAddSwitch_Click(object sender, EventArgs e)
        {
            var suc = new SwitchUserControl();
            suc.SetController(_cntrl);

            SetSwitchNameForm f = new SetSwitchNameForm();
            f.SetController(_cntrl);
            f.Text = "Set Switch";            
            f.ShowDialog();
            if (_cntrl.TmpSwitchName != null)
            {
                suc.SetSwitchName(_cntrl.TmpSwitchName);
                flowLayoutPanel1.Controls.Add(suc);
            }
        }


        private void btnHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, @"Help\\SCC_User_Guide.chm", HelpNavigator.TableOfContents);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetSwitchNameForm f = new SetSwitchNameForm();
            f.SetController(_cntrl);
            f.Text = "Remove Switch";
            f.ShowDialog();
            if (_cntrl.TmpSwitchName != null)
            {
                foreach (var _ctrlDaElidere in flowLayoutPanel1.Controls)
                {
                    if (_ctrlDaElidere.GetType() == typeof (SwitchUserControl))
                    {
                        var mm = (SwitchUserControl) _ctrlDaElidere;
                        if (mm.GetSwitchName.ToLower().Equals(_cntrl.TmpSwitchName.ToLower()))
                            flowLayoutPanel1.Controls.Remove(mm);
                    }
                }                
            }
        }

        /// <summary>leggo i parametri dalla strategy DLL e li carico nella griglia bassa del tab parametri</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StrategiesFullParams_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblStrategiesDLL.Text))
            {
                var alert1 = string.Format("Use button \"Strategies DLL\" before and select one file.");
                var caption1 = string.Format("Show Parameters from Strategy");
                var ret1 = MessageBox.Show(alert1, caption1, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                return;                
            }
            var alert = string.Format("Operation can take time to finish processing all strategies!\nContinue?");
            var caption = string.Format("Load Parameters from Strategy");
            var ret = MessageBox.Show(alert, caption, MessageBoxButtons.OKCancel,MessageBoxIcon.Question,MessageBoxDefaultButton.Button1);
            if (ret != DialogResult.OK) return;

            //carico tutti i parametri della strategy letta come DLL
            _cntrlass.WhenStrategyClassAddedOrRemovedFindAllParameters(
                lblStrategiesDLL.Text, cmbAssociatedStrategies.Items, dgwParametersAssociated);
        }

        /// <summary>rimuove un elemento dalla lista dei parametri inseriti da utente</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgwParameters_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ////non posso cancellare l'header o la prima riga vuota
            //if (e.RowIndex < 0) return;
            //var alert = string.Format("Remove selected element?");
            //var caption = string.Format("Remove Parameter From Collection");

            //var ret = MessageBox.Show(alert, caption, MessageBoxButtons.OKCancel,MessageBoxIcon.Question,MessageBoxDefaultButton.Button1);
            //if (ret != DialogResult.OK) return;
            ////
            //dgwParameters.Rows.RemoveAt(e.RowIndex);
            //dgwParameters.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// <summary>Usando il tasto destro si può copiare la cella su cui viene fatta l'operazione</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgwParameters_CellMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //se non ci sono recor ho selezionato la griglia vuota
                var grid = (DataGridView)sender;
                if (grid.Rows.Count <= 0) return;
                //recupero la riga su cui ho fatto il click
                var hti = grid.HitTest(e.X, e.Y);
                // controllo che si sia selezionato la prima colonna, altrimenti non agisco
                if (hti.ColumnIndex > -1) return;
                //
                var dgvRow = grid.Rows[hti.RowIndex];
                int row = dgwParameters.Rows.Add();
                dgwParameters.Rows[row].Cells[0].Value = string.Format("RENAME({0})", new Random().Next(1, 10000));
                dgwParameters.Rows[row].Cells[1].Value = dgvRow.Cells[1].Value;
                dgwParameters.Rows[row].Cells[2].Value = dgvRow.Cells[2].Value;
                dgwParameters.Rows[row].Cells[3].Value = dgvRow.Cells[3].Value;
                dgwParameters.Rows[row].Cells[4].Value = dgvRow.Cells[4].Value;
                dgwParameters.Rows[row].Cells[5].Value = dgvRow.Cells[5].Value;
                ((DataGridViewComboBoxCell)dgwParameters.Rows[row].Cells[6]).Value = dgvRow.Cells[6].Value;
                //
                dgwParameters.CommitEdit(DataGridViewDataErrorContexts.Commit);

            }
            else if (e.Button == MouseButtons.Left)
            {
                //se non ci sono recor ho selezionato la griglia vuota
                var grid = (DataGridView)sender;
                if (grid.Rows.Count <= 0) return;
                //recupero la riga su cui ho fatto il click
                var hti = grid.HitTest(e.X, e.Y);
                // controllo che si sia selezionato la prima colonna, altrimenti non agisco
                if (hti.ColumnIndex > -1) return;
                //
                var alert = string.Format("Remove selected element?");
                var caption = string.Format("Remove Parameter From Collection");
                //
                var ret = MessageBox.Show(alert, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (ret != DialogResult.OK) return;
                //
                dgwParameters.Rows.RemoveAt(hti.RowIndex);
                dgwParameters.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgwParametersAssociated_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //se seleziono la griglia vuota o l'header
            if (e.RowIndex < 0) return;
            //griglia sorgente
            DataGridView dgv = ((DataGridView)sender);
            DataGridViewRow dgvRow = dgv.Rows[e.RowIndex];
            //
            foreach (DataGridViewRow _row in dgwParameters.Rows)
            {
                if (_row.Cells[0].Value == dgvRow.Cells[0].Value)
                {
                    MessageBox.Show(String.Format("Param \"{0}\" already in the collection!", _row.Cells[0].Value));
                    return;
                }
            }
            //
            int row = dgwParameters.Rows.Add();
            dgwParameters.Rows[row].Cells[0].Value = dgvRow.Cells[0].Value;
            dgwParameters.Rows[row].Cells[1].Value = dgvRow.Cells[1].Value;
            dgwParameters.Rows[row].Cells[2].Value = dgvRow.Cells[2].Value;
            dgwParameters.Rows[row].Cells[3].Value = dgvRow.Cells[3].Value;
            dgwParameters.Rows[row].Cells[4].Value = dgvRow.Cells[4].Value;
            dgwParameters.Rows[row].Cells[5].Value = dgvRow.Cells[5].Value;
            ((DataGridViewComboBoxCell) dgwParameters.Rows[row].Cells[6]).Value = dgvRow.Cells[6].Value;
            //
            dgwParameters.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// <summary>Esegue il salvataggio su un file XML il cui nome è da scegliere</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAsDefinition_Click(object sender, EventArgs e)
        {
            try
            {
                #region 2016-05-19: controllo se ci sono inputs/outputs uguali
                var dups = (dgwInputs.ToListOfInOutPar().Intersect(dgwOutputs.ToListOfInOutPar(), new InputOutputComparer())).ToList();
                if (dups.Any())
                {
                    var f = new InOutDuplicati();
                    f.SetMassage(dups);
                    if (f.ShowDialog() != DialogResult.OK) return;
                }
                #endregion
                if (_cntrl.FillDataSetDefinition())
                {
                    saveFileDialog1.Filter = "SCC definitions (*.xml)|*.xml";
                    saveFileDialog1.FileName = txtStrategyName.Text ?? "";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        _cntrl.SaveDataSetDefinition(saveFileDialog1.FileName);
                        lblDefinitionNameSaved.Text = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf('\\') + 1); ;
                    }
                }
            }
            catch (Exception eee) { this.ShowMessage(eee.Message); }
        }
    }
}
