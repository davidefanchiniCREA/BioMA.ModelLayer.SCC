using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using CRA.ModelLayer.SCC.CodeParts;
using System.Reflection;
using CRA.ModelLayer.Core;
using CRA.ModelLayer.Strategy;

namespace CRA.ModelLayer.SCC
{
    public class Controller
    {
        #region Constructor

        public Controller()
        {
            TimeSteps = new List<string>();
            Domains = new DataSet("Domains");
            ModelTypes = new DataSet("ModelTypes");
            Inputs = new Dictionary<string, string>();
            Outputs = new Dictionary<string, string>();
            AssociatedStrategies = new Dictionary<string, string>();
            OrderedAssociatedStrategies = new List<string>();
            Switches = new Dictionary<string, BuildingSwitch>();
            Parameters = new Dictionary<string, VarInfo>();
            LoadedAssemblies = new List<string>();
            Definition = new DataSet("StrategyDefinition");
            CompositeStrategy = "";
            InitializeDataSetDefinition();

            _codeParts.Add(new CodeDirectives(this));
            _codeParts.Add(new CodeConstructor(this));
            _codeParts.Add(new CodeComposite(this));
            _codeParts.Add(new CodePrePostConditions(this));
            _codeParts.Add(new CodeIStrategy(this));
            _codeParts.Add(new CodeModel(this));
            _codeParts.Add(new CodeParameters(this));
            _codeParts.Add(new CodeSetCurrentValues(this));
        }

        private List<ICodePart> _codeParts = new List<ICodePart>();

        internal static string GetCmbDomainClassFromDomainClassTypeAndId(Type t, string id)
        {
            return id + " - " + t.FullName;
        }

        internal static string GetDomainClassTypeFromCmbDomainClass(string s)
        {
            return s.Split(" - ".ToCharArray()).Last();
        }

        internal static string GetDomainClassIdFromCmbDomainClass(string s)
        {
            return s.Split(" - ".ToCharArray()).First();
        }

        private void InitializeDataSetDefinition()
        {
            #region Tables

            Definition.Tables.Add("Global");
            Definition.Tables.Add("General");
            Definition.Tables.Add("Inputs");
            Definition.Tables.Add("Outputs");
            Definition.Tables.Add("Parameters");
            Definition.Tables.Add("AssociatedStrategies");
            Definition.Tables.Add("Switches");
            Definition.Tables.Add("LoadedAssemblies");
            Definition.Tables.Add("SwitchesModelingOptions");

            #endregion

            #region Columns

            Definition.Tables["Global"].Columns.Add("SimpleStrategy");
            Definition.Tables["Global"].Columns.Add("CompositeStrategy");
            Definition.Tables["Global"].Columns.Add("DataInterfacesDLL");
            Definition.Tables["Global"].Columns.Add("StrategiesDLL");
            Definition.Tables["Global"].Columns.Add("IStrategyComponentName");
            Definition.Tables["LoadedAssemblies"].Columns.Add("Name");


            Definition.Tables["General"].Columns.Add("Domain");
            Definition.Tables["General"].Columns.Add("ModelType");
            Definition.Tables["General"].Columns.Add("NameSpace");
            Definition.Tables["General"].Columns.Add("StrategyName");
            Definition.Tables["General"].Columns.Add("StrategyDescription");
            Definition.Tables["General"].Columns.Add("IsContext");
            Definition.Tables["General"].Columns.Add("TimeSteps");
            Definition.Tables["General"].Columns.Add("AuthorFirstRelease");
            Definition.Tables["General"].Columns.Add("Institution");
            Definition.Tables["General"].Columns.Add("Email");
            Definition.Tables["General"].Columns.Add("DateFirstRelease");
            Definition.Tables["General"].Columns.Add("AuthorRevision");
            Definition.Tables["General"].Columns.Add("EmailAuthorRevision");
            Definition.Tables["General"].Columns.Add("DateRevision");

            Definition.Tables["Inputs"].Columns.Add("Variable");
            Definition.Tables["Inputs"].Columns.Add("DomainClass");

            Definition.Tables["Outputs"].Columns.Add("Variable");
            Definition.Tables["Outputs"].Columns.Add("DomainClass");

            Definition.Tables["Parameters"].Columns.Add("VarName");
            Definition.Tables["Parameters"].Columns.Add("Description");
            Definition.Tables["Parameters"].Columns.Add("MaxValue");
            Definition.Tables["Parameters"].Columns.Add("MinValue");
            Definition.Tables["Parameters"].Columns.Add("DefaultValue");
            Definition.Tables["Parameters"].Columns.Add("Units");
            Definition.Tables["Parameters"].Columns.Add("ValueType");


            Definition.Tables["Switches"].Columns.Add("SwitchName");
            Definition.Tables["Switches"].Columns.Add("SwitchDescription");
            Definition.Tables["Switches"].Columns.Add("SwitchValues");

            //  Columns associated strategies
            Definition.Tables["AssociatedStrategies"].Columns.Add("StrategyFullName");

            Definition.Tables["SwitchesModelingOptions"].Columns.Add("SwitchName");
            Definition.Tables["SwitchesModelingOptions"].Columns.Add("SwitchValue");
            Definition.Tables["SwitchesModelingOptions"].Columns.Add("Inputs");
            Definition.Tables["SwitchesModelingOptions"].Columns.Add("Outputs");
            Definition.Tables["SwitchesModelingOptions"].Columns.Add("Parameters");
            Definition.Tables["SwitchesModelingOptions"].Columns.Add("AssociatedStrategies");

            #endregion
        }

        #endregion

        #region Properties

        internal DataSet Definition { get; set; }
        internal string SimpleStrategy { get; set; }
        internal string CompositeStrategy { get; set; }
        internal string Domain { get; set; }
        internal DataSet Domains { get; set; }
        internal string ModelType { get; set; }
        internal DataSet ModelTypes { get; set; }
        internal string IsContext { get; set; }
        internal string NameSpace { get; set; }
        internal string StrategyName { get; set; }
        internal string StrategyDescription { get; set; }
        internal List<string> TimeSteps { get; set; }
        internal string Author { get; set; }
        internal string Email { get; set; }
        internal string DateFirstRelease { get; set; }
        internal string Institution { get; set; }
        internal string AuthorRevision { get; set; }
        internal string DateRevision { get; set; }
        internal string EmailRevision { get; set; }
        internal Dictionary<string, string> Inputs { get; set; }
        internal Dictionary<string, string> Outputs { get; set; }
        internal Dictionary<string, string> AssociatedStrategies { get; set; }
        // davide - prova
        // qui tengo traccia dell'ordine con cui le associated strategies devono essere enumerate
        internal List<string> OrderedAssociatedStrategies { get; set; }
        internal Dictionary<string, VarInfo> Parameters { get; set; }
        internal string UpdatedCode { get; set; }
        internal string CreationReport { get; set; }
        internal string IStrategyComponentName { get; set; }
        internal string DataInterfacesDLL { get; set; }
        internal string StrategiesDLL { get; set; }
        //    internal string TypesAndInstances { get; set; }
        //   internal string InstancesOfTypes { get; set;  }
        internal List<string> LoadedAssemblies;

        public string ModelMethodName;

        public Dictionary<string, Type> DomainClassTypeAndInstances { get; set; }

        public Dictionary<string, BuildingSwitch> Switches { get; set; }

        #endregion

        #region CreateCode

        internal bool GenerateCode(string templateFileName)
        {
            CreationReport = String.Empty;
            string o = "";
            string newCode = ReadTemplate(templateFileName);
            bool res = CreateCode(newCode, out o);
            UpdatedCode = o;
            return res;
        }

        // TODO: carica template file da file configurazione?
        private string ReadTemplate(string templateFileName)
        {
            // Put file content in a string
            string newCode = string.Empty;

            List<string> lines = File.ReadAllLines(templateFileName).ToList();

            foreach (string line in lines)
            {
                newCode += line + "\r\n";
            }
            return newCode;
        }

        private bool CreateCode(string startCode, out string updatedcode)
        {


            foreach (string key in Parameters.Keys)
            {
                if (key is System.DBNull || Parameters[key].Description is System.DBNull ||
                    Parameters[key].MaxValue is System.DBNull || Parameters[key].MinValue is System.DBNull ||
                    Parameters[key].DefaultValue is System.DBNull || Parameters[key].Units is System.DBNull ||
                    Parameters[key].ValueType is System.DBNull
                    || key == null || Parameters[key].Description == null || Parameters[key].MaxValue == null ||
                    Parameters[key].MinValue == null || Parameters[key].DefaultValue == null ||
                    Parameters[key].Units == null || Parameters[key].ValueType == null)
                {
                    string err = "Please set all the fields for the parameter '" + key +
                                 "' otherwise it is impossible to generate the code!";
                    MessageToTheView(err);
                    updatedcode = "";
                    return false;
                }
            }

            string newCode = startCode;

            foreach (ICodePart cp in _codeParts)
            {
                newCode = cp.WriteCodeLines(newCode);
            }

            if (newCode.Contains("(#"))
            {
                if (MessageToTheView != null)
                    MessageToTheView(
                        "WARNING! Not all code replacements were made. Check if any of the required fields is empty.");
            }


            updatedcode = newCode;
            return true;
        }

        public event Action<string> MessageToTheView;
        public string TmpSwitchName;

        #endregion

        #region Save Definition

        public void SaveDataSetDefinition(string SCCFile)
        {
            this.Definition.WriteXml(SCCFile);
        }

        public bool FillDataSetDefinition()
        {
            // Reset tables
            foreach (DataTable dt in Definition.Tables)
                dt.Rows.Clear();

            // Table Global
            DataRow dr = Definition.Tables["Global"].NewRow();
            dr["SimpleStrategy"] = SimpleStrategy;
            dr["CompositeStrategy"] = CompositeStrategy;
            dr["DataInterfacesDLL"] = DataInterfacesDLL;
            dr["StrategiesDLL"] = StrategiesDLL;
            dr["IStrategyComponentName"] = IStrategyComponentName;

            Definition.Tables["Global"].Rows.Add(dr);

            // Table General
            dr = Definition.Tables["General"].NewRow();
            dr["Domain"] = Domain;
            dr["ModelType"] = ModelType;
            dr["NameSpace"] = NameSpace;
            dr["StrategyName"] = StrategyName;
            dr["StrategyDescription"] = StrategyDescription;
            dr["IsContext"] = IsContext;
            string allTimeSteps = string.Empty;
            foreach (string ts in TimeSteps)
            {
                allTimeSteps += (ts + ",");
            }
            dr["TimeSteps"] = allTimeSteps;
            dr["AuthorFirstRelease"] = Author;
            dr["Institution"] = Institution;
            dr["Email"] = Email;
            dr["DateFirstRelease"] = DateFirstRelease;
            dr["AuthorRevision"] = AuthorRevision;
            dr["EmailAuthorRevision"] = EmailRevision;
            dr["DateRevision"] = DateRevision;
            Definition.Tables["General"].Rows.Add(dr);

            foreach (string asse in LoadedAssemblies)
            {
                dr = Definition.Tables["LoadedAssemblies"].NewRow();
                dr["Name"] = asse;
                Definition.Tables["LoadedAssemblies"].Rows.Add(dr);
            }
            // Table AssociatedStaretegies
            // davide - prova
            //foreach (string asse in AssociatedStrategies.Keys)
            foreach (string asse in OrderedAssociatedStrategies)
            {
                dr = Definition.Tables["AssociatedStrategies"].NewRow();
                dr["StrategyFullName"] = asse;
                Definition.Tables["AssociatedStrategies"].Rows.Add(dr);
            }
            // Table Switches
            foreach (string sw in Switches.Keys)
            {
                dr = Definition.Tables["Switches"].NewRow();
                dr["SwitchName"] = sw;
                dr["SwitchDescription"] = Switches[sw].SwitchDescription;
                dr["SwitchValues"] = Switches[sw].AcceptableSwitchValues.Aggregate((a, b) => a = a + SEPARATOR + b);
                Definition.Tables["Switches"].Rows.Add(dr);
            }

            // Table Inputs
            foreach (string key in Inputs.Keys)
            {
                dr = Definition.Tables["Inputs"].NewRow();
                dr["Variable"] = key;
                dr["DomainClass"] = Inputs[key];
                Definition.Tables["Inputs"].Rows.Add(dr);
            }

            // Table Outputs
            foreach (string key in Outputs.Keys)
            {
                dr = Definition.Tables["Outputs"].NewRow();
                dr["Variable"] = key;
                dr["DomainClass"] = Outputs[key];
                Definition.Tables["Outputs"].Rows.Add(dr);
            }

            // Table Parameters
            foreach (string key in Parameters.Keys)
            {

                if (key is System.DBNull || Parameters[key].Description is System.DBNull ||
                    Parameters[key].MaxValue is System.DBNull || Parameters[key].MinValue is System.DBNull ||
                    Parameters[key].DefaultValue is System.DBNull || Parameters[key].Units is System.DBNull ||
                    Parameters[key].ValueType is System.DBNull
                    || key == null || Parameters[key].Description == null || Parameters[key].MaxValue == null ||
                    Parameters[key].MinValue == null || Parameters[key].DefaultValue == null ||
                    Parameters[key].Units == null || Parameters[key].ValueType == null)
                {
                    throw new Exception("Please set all the fields for the parameter '" + key +
                                        "' otherwise it is impossible to save the definition!");

                }

                dr = Definition.Tables["Parameters"].NewRow();
                dr["VarName"] = key;
                dr["Description"] = Parameters[key].Description;
                dr["MaxValue"] = Parameters[key].MaxValue.ToString();
                dr["MinValue"] = Parameters[key].MinValue.ToString();
                dr["DefaultValue"] = Parameters[key].DefaultValue.ToString();
                dr["Units"] = Parameters[key].Units;
                dr["ValueType"] = Parameters[key].ValueType.Name.ToString();
                Definition.Tables["Parameters"].Rows.Add(dr);
            }

            // Table SwitchesModelingOptions
            foreach (BuildingSwitch sw in Switches.Values)
            {
                foreach (string swv in sw.AcceptableSwitchValues)
                {
                    dr = Definition.Tables["SwitchesModelingOptions"].NewRow();
                    dr["SwitchName"] = sw.SwitchName;
                    dr["SwitchValue"] = swv;
                    dr["Inputs"] = "";
                    if (sw.SwitchValueRelatedModelingOptions4SwitchValues[swv].Inputs.Count > 0)
                        dr["Inputs"] =
                            sw.SwitchValueRelatedModelingOptions4SwitchValues[swv].Inputs.Select(a => a)
                                .Aggregate((a, b) => a = a + SEPARATOR + b);
                    dr["Outputs"] = "";
                    if (sw.SwitchValueRelatedModelingOptions4SwitchValues[swv].Outputs.Count > 0)
                        dr["Outputs"] =
                            sw.SwitchValueRelatedModelingOptions4SwitchValues[swv].Outputs.Select(a => a)
                                .Aggregate((a, b) => a = a + SEPARATOR + b);
                    dr["Parameters"] = "";
                    if (sw.SwitchValueRelatedModelingOptions4SwitchValues[swv].Parameters.Count > 0)
                        dr["Parameters"] =
                            sw.SwitchValueRelatedModelingOptions4SwitchValues[swv].Parameters.Select(a => a)
                                .Aggregate((a, b) => a = a + SEPARATOR + b);
                    dr["AssociatedStrategies"] = "";
                    if (sw.SwitchValueRelatedModelingOptions4SwitchValues[swv].AssociatedStrategies.Count > 0)
                        dr["AssociatedStrategies"] =
                            sw.SwitchValueRelatedModelingOptions4SwitchValues[swv].AssociatedStrategies.Select(a => a)
                                .Aggregate((a, b) => a = a + SEPARATOR + b);
                    Definition.Tables["SwitchesModelingOptions"].Rows.Add(dr);
                }
            }



            return true;
        }

        #endregion

        public static string SEPARATOR = "%,%";

        internal Dictionary<string, VarInfo> GetParametersOfAssociatedStrategy(string strategyFullName)
        {
            return GetParametersOfAStrategy(this.StrategiesDLL, strategyFullName);
        }

        private Dictionary<string, VarInfo> GetParametersOfAStrategy(string assemblyName, string strategyFullName)
        {

            Dictionary<string, VarInfo> toret = new Dictionary<string, VarInfo>();

            Assembly a = Assembly.LoadFrom(assemblyName);
            Type[] types = a.GetTypes();

            foreach (Type t in types)
            {
                try
                {
                    // Discover classes 
                    if (t.IsClass && t.FullName.Equals(strategyFullName))
                    {
                        // TODO: togliere old IStrategy
                        //if (typeof (CRA.Core.Preconditions.IStrategy).IsAssignableFrom(t) ||
                        if ( typeof (IStrategy).IsAssignableFrom(t))
                        {
                            if (t.GetConstructor(new Type[] {}) != null)
                            {
                                object instance = t.GetConstructor(new Type[] {}).Invoke(new object[] {});
                                IStrategy newTypeStrategy;
                                //if (instance is CRA.Core.Preconditions.IStrategy)
                                //{
                                //    newTypeStrategy =
                                //        new CRA.ModelLayer.StrategyConverter.StrategyConverter(
                                //            instance as CRA.Core.Preconditions.IStrategy);
                                //    foreach (VarInfo par in newTypeStrategy.Parameters())
                                //    {
                                //        int size;
                                //        if (par.ValueType == null)
                                //        {
                                //            par.ValueType =
                                //                GetVarInfoValueTypeForPropertyOfAStrategy(
                                //                    (instance as CRA.Core.Preconditions.IStrategy), par.Name, out size);
                                //                //to avoid null ValueType when reading old strategies
                                //            par.Size = size;
                                //        }
                                //        toret.Add(par.Name, par);
                                //    }
                                //}
                                //else 
                                if (instance is IStrategy)
                                {
                                    newTypeStrategy = instance as IStrategy;
                                    foreach (VarInfo par in newTypeStrategy.AllPossibleParameters())
                                    {
                                        int size;
                                        if (par.ValueType == null)
                                        {
                                            par.ValueType =
                                                GetVarInfoValueTypeForPropertyOfAStrategy((instance as IStrategy),
                                                    par.Name, out size);
                                                //to avoid null ValueType when reading old strategies
                                            par.Size = size;
                                        }
                                        toret.Add(par.Name, par);
                                    }
                                    //foreach (var assStrat in newTypeStrategy.AllPossibleAssociatedStrategies()) { 
                                    //    Dictionary<string, VarInfo> assStratPar=GetParametersOfAssociatedStrategy(assStrat);
                                    //    foreach (var d in assStratPar) {
                                    //        if (toret.ContainsKey(d.Key)) toret.Add(d.Key, d.Value);
                                    //    }
                                    //}
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return toret;
        }

        internal IEnumerable<string> GetSwitchesOfAssociatedStrategy(string strategyFullName)
        {
            return GetSwitchesOfAStrategy(this.StrategiesDLL, strategyFullName).Distinct();
        }

        private List<string> GetSwitchesOfAStrategy(string assemblyName, string strategyFullName)
        {

            List<string> toret = new List<string>();

            Assembly a = Assembly.LoadFrom(assemblyName);
            Type[] types = a.GetTypes();

            foreach (Type t in types)
            {
                try
                {
                    // Discover classes 
                    if (t.IsClass && t.FullName.Equals(strategyFullName))
                    {

                        if (typeof (IStrategy).IsAssignableFrom(t))
                        {
                            if (t.GetConstructor(new Type[] {}) != null)
                            {
                                object instance = t.GetConstructor(new Type[] {}).Invoke(new object[] {});
                                IStrategy newTypeStrategy;
                                if (instance is IStrategy)
                                {
                                    newTypeStrategy = instance as IStrategy;
                                    foreach (string sw in newTypeStrategy.ModellingOptionsManager.SwitchesNames)
                                    {
                                        toret.Add(sw);
                                    }
                                }
                                else
                                {
                                    continue;
                                }

                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return toret;
        }

        public void LoadConfig(string filename)
        {
            this.Definition.Clear();
            Definition.ReadXml(filename);

            //move values from definition to the other controller properties

            // Global
            this.SimpleStrategy = this.Definition.Tables["Global"].Rows[0]["SimpleStrategy"].ToString();
            this.CompositeStrategy = this.Definition.Tables["Global"].Rows[0]["CompositeStrategy"].ToString();
            this.DataInterfacesDLL = this.Definition.Tables["Global"].Rows[0]["DataInterfacesDLL"].ToString();
            this.StrategiesDLL = this.Definition.Tables["Global"].Rows[0]["StrategiesDLL"].ToString();

            // General
            this.Domain = this.Definition.Tables["General"].Rows[0]["Domain"].ToString();
            this.ModelType = this.Definition.Tables["General"].Rows[0]["ModelType"].ToString();
            this.StrategyName = this.Definition.Tables["General"].Rows[0]["StrategyName"].ToString();
            this.NameSpace = this.Definition.Tables["General"].Rows[0]["Namespace"].ToString();
            this.StrategyDescription = this.Definition.Tables["General"].Rows[0]["Strategydescription"].ToString();
            this.TimeSteps =
                this.Definition.Tables["General"].Rows[0]["TimeSteps"].ToString()
                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            this.IsContext = this.Definition.Tables["General"].Rows[0]["IsContext"].ToString();
            this.Author = this.Definition.Tables["General"].Rows[0]["AuthorFirstRelease"].ToString();
            this.Institution = this.Definition.Tables["General"].Rows[0]["Institution"].ToString();
            this.Email = this.Definition.Tables["General"].Rows[0]["Email"].ToString();
            this.DateFirstRelease = this.Definition.Tables["General"].Rows[0]["DateFirstRelease"].ToString();
            this.AuthorRevision = this.Definition.Tables["General"].Rows[0]["AuthorRevision"].ToString();
            this.EmailRevision = this.Definition.Tables["General"].Rows[0]["EmailAuthorRevision"].ToString();
            this.DateRevision = this.Definition.Tables["General"].Rows[0]["DateRevision"].ToString();


            // Assoc strat

            this.AssociatedStrategies = new Dictionary<string, string>();
            this.OrderedAssociatedStrategies = new List<string>();

            for (int inx = 0; inx < this.Definition.Tables["AssociatedStrategies"].Rows.Count; inx++)
            {
                object o = this.Definition.Tables["AssociatedStrategies"].Rows[inx]["StrategyFullName"];
                if (!this.AssociatedStrategies.ContainsKey((string)o))
                {
                    this.AssociatedStrategies.Add((string)o, ((string)o).Split(".".ToCharArray()).Last().ToLower());
                    this.OrderedAssociatedStrategies.Add((string)o);
                }
            }

            //Switches and   SwitchesModelingOptions
            this.Switches = new Dictionary<string, BuildingSwitch>();

            for (int inx = 0; inx < this.Definition.Tables["Switches"].Rows.Count; inx++)
            {
                string swname = (string) this.Definition.Tables["Switches"].Rows[inx]["SwitchName"];
                string swdesc = (string) this.Definition.Tables["Switches"].Rows[inx]["SwitchDescription"];
                string swvalues = (string) this.Definition.Tables["Switches"].Rows[inx]["SwitchValues"];
                string[] swvals = swvalues.Split(SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, ModellingOptions> d = new Dictionary<string, ModellingOptions>();
                foreach (string s in swvals)
                {
                    d.Add(s, null);
                }

                if (!this.Switches.ContainsKey((string) swname))
                    this.Switches.Add((string) swname,
                        new BuildingSwitch(swname, swdesc, d, this.GeneralModelingOptions));

            }

            for (int inx = 0; inx < this.Definition.Tables["SwitchesModelingOptions"].Rows.Count; inx++)
            {
                string swname = (string) this.Definition.Tables["SwitchesModelingOptions"].Rows[inx]["SwitchName"];
                string swvalue = (string) this.Definition.Tables["SwitchesModelingOptions"].Rows[inx]["SwitchValue"];
                string[] inputs =
                    ((string) this.Definition.Tables["SwitchesModelingOptions"].Rows[inx]["Inputs"]).Split(
                        SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string[] outputs =
                    ((string) this.Definition.Tables["SwitchesModelingOptions"].Rows[inx]["Outputs"]).Split(
                        SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string[] parameters =
                    ((string) this.Definition.Tables["SwitchesModelingOptions"].Rows[inx]["Parameters"]).Split(
                        SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string[] assSts =
                    ((string) this.Definition.Tables["SwitchesModelingOptions"].Rows[inx]["AssociatedStrategies"]).Split
                        (SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (this.Switches.ContainsKey(swname))
                {

                    BuildingSwitch bs = this.Switches[swname];

                    if (!bs.SwitchValueRelatedModelingOptions4SwitchValues.ContainsKey(swvalue))
                        bs.SwitchValueRelatedModelingOptions4SwitchValues.Add(swvalue,
                            new SwitchValueRelatedModelingOptions());

                    foreach (string i in inputs.ToArray())
                    {
                        if (!bs.SwitchValueRelatedModelingOptions4SwitchValues[swvalue].Inputs.Contains(i))
                            bs.SwitchValueRelatedModelingOptions4SwitchValues[swvalue].Inputs.Add(i);
                    }
                    foreach (string i in outputs.ToArray())
                    {
                        if (!bs.SwitchValueRelatedModelingOptions4SwitchValues[swvalue].Outputs.Contains(i))
                            bs.SwitchValueRelatedModelingOptions4SwitchValues[swvalue].Outputs.Add(i);
                    }
                    foreach (string i in parameters.ToArray())
                    {
                        if (!bs.SwitchValueRelatedModelingOptions4SwitchValues[swvalue].Parameters.Contains(i))
                            bs.SwitchValueRelatedModelingOptions4SwitchValues[swvalue].Parameters.Add(i);
                    }
                    foreach (string i in assSts.ToArray())
                    {
                        if (!bs.SwitchValueRelatedModelingOptions4SwitchValues[swvalue].AssociatedStrategies.Contains(i))
                            bs.SwitchValueRelatedModelingOptions4SwitchValues[swvalue].AssociatedStrategies.Add(i);
                    }
                }
            }

            // Inputs
            this.Inputs = new Dictionary<string, string>();
            for (int inx = 0; inx < this.Definition.Tables["Inputs"].Rows.Count; inx++)
            {
                object[] o =
                {
                    this.Definition.Tables["Inputs"].Rows[inx]["Variable"],
                    this.Definition.Tables["Inputs"].Rows[inx]["DomainClass"]
                };
                this.Inputs.Add(o[0].ToString(), o[1].ToString());
            }

            // Outputs
            this.Outputs = new Dictionary<string, string>();
            for (int inx = 0; inx < this.Definition.Tables["Outputs"].Rows.Count; inx++)
            {
                object[] o =
                {
                    this.Definition.Tables["Outputs"].Rows[inx]["Variable"],
                    this.Definition.Tables["Outputs"].Rows[inx]["DomainClass"]
                };
                this.Outputs.Add(o[0].ToString(), o[1].ToString());
            }

            // Parameters
            this.Parameters = new Dictionary<string, VarInfo>();
            for (int inx = 0; inx < this.Definition.Tables["Parameters"].Rows.Count; inx++)
            {
                VarInfo v = new VarInfo();
                v.Name = (string) this.Definition.Tables["Parameters"].Rows[inx]["VarName"];
                v.Description = (string) this.Definition.Tables["Parameters"].Rows[inx]["Description"];
                v.MinValue = double.Parse((string) this.Definition.Tables["Parameters"].Rows[inx]["MinValue"]);
                v.MaxValue = double.Parse((string) this.Definition.Tables["Parameters"].Rows[inx]["MaxValue"]);
                v.DefaultValue = double.Parse((string) this.Definition.Tables["Parameters"].Rows[inx]["DefaultValue"]);
                v.Units = (string) this.Definition.Tables["Parameters"].Rows[inx]["Units"];
                v.ValueType =
                    VarInfoValueTypes.GetInstanceForName(
                        (string) this.Definition.Tables["Parameters"].Rows[inx]["ValueType"]);
                v.VarType = VarInfo.Type.PARAMETER;
                this.Parameters.Add(v.Name, v);
            }

            ReflectAssemblyDataInterfaces();
        }

        private void ReflectAssemblyDataInterfaces()
        {
            if (string.IsNullOrEmpty(this.DataInterfacesDLL)) return;

            // Discover the higher lever interface inheriting from IStrategy         
            Assembly asse = Assembly.LoadFrom(this.DataInterfacesDLL);

            this.LoadedAssemblies = new List<string>();
            this.LoadedAssemblies.Add(asse.FullName);
            foreach (AssemblyName assed in asse.GetReferencedAssemblies())
            {
                this.LoadedAssemblies.Add(assed.FullName);
            }

            Type higherLevelStrategyInterface = GetComponentInterfaceFormInterfaceDll(asse);

            if (higherLevelStrategyInterface != null)
            {
                string nameInterface = higherLevelStrategyInterface.ToString();

                this.IStrategyComponentName = higherLevelStrategyInterface.Name;

                // Discover method))
                if (higherLevelStrategyInterface.FullName == nameInterface)
                {
                    MemberInfo[] ms = higherLevelStrategyInterface.GetMembers();
                    foreach (MemberInfo m in ms)
                    {
                        if (m.MemberType == MemberTypes.Method &&
                            m.ToString().Contains("Void"))
                        {
                            string modelCall = m.ToString();

                            if (!m.Name.Contains("Conditions") && !m.Name.Equals("SetParametersDefaultValue"))
                            {
                                ModelMethodName = m.Name;

                                int openP = modelCall.IndexOf('(');
                                int closeP = modelCall.IndexOf(')');
                                string typesParameters = modelCall.Substring(openP + 1,
                                    modelCall.Length - openP -
                                    (modelCall.Length - closeP + 1));
                                string[] singleDomainClasses = typesParameters.Split(',');


                                this.DomainClassTypeAndInstances = new Dictionary<string, Type>();

                                for (int inx = 0; inx < (singleDomainClasses.Count()); inx++)
                                {
                                    string[] dom = singleDomainClasses[inx].Split('.');
                                    string instance = dom[dom.Count() - 1].ToLower();
                                    if (this.DomainClassTypeAndInstances.Keys.Contains(instance))
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
                                        this.DomainClassTypeAndInstances.Add(instance, methodArgumentType);


                                }
                            }
                        }

                    }
                }
            }
        }

        public static Type GetComponentInterfaceFormInterfaceDll(Assembly asse)
        {
            Type higherLevelStrategyInterface = null;

            Type[] types = asse.GetTypes();


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

            return higherLevelStrategyInterface;
        }

        public void BatchRun_LoadConfigAndGenerateStrategies(string configFile, string destfile)
        {
            LoadConfig(configFile);

            CodeParts.CodeUtilities.Warnings = String.Empty;

            if (this.GenerateCode("StrategyTemplate.txt"))
            {

                //Write the file keeping the custom code areas
                Writer w = new Writer();
                w.Initialize();
                w.Write(destfile, this.UpdatedCode);

            }
        }

        //public void ReadOldStrategy(CRA.Core.Preconditions.IStrategy oldStrategy, Type oldStrategyBaseInterface,
        //    List<CRA.Core.Preconditions.IStrategy> allStrategies)
        //{
        //    //Switches
        //    this.Switches = new Dictionary<string, BuildingSwitch>(); //not present in old strategies

        //    CRA.ModelLayer.StrategyConverter.StrategyConverter sc =
        //        new CRA.ModelLayer.StrategyConverter.StrategyConverter(oldStrategy);

        //    //  Assoc strat
        //    this.AssociatedStrategies = new Dictionary<string, string>(); //no way to know from old strategy
        //    foreach (string p in sc.AssociatedStrategies())
        //    {
        //        if (p != null) this.AssociatedStrategies.Add(p, p);
        //    }

        //    IEnumerable<CRA.Core.Preconditions.IStrategy> associatedStrategiesInstances =
        //        allStrategies.Where(a => this.AssociatedStrategies.ContainsKey(a.GetType().FullName));

        //    List<string> associatedStrategiesParameters = new List<string>();
        //    foreach (CRA.Core.Preconditions.IStrategy s in associatedStrategiesInstances)
        //    {
        //        associatedStrategiesParameters.AddRange(
        //            (new StrategyConverter.StrategyConverter(s)).Parameters().Select(a => a.Name));
        //    }


        //    // Inputs
        //    this.Inputs = new Dictionary<string, string>();
        //    foreach (PropertyDescription p in sc.Inputs())
        //    {
        //        if (p != null && p.PropertyName != null && p.DomainClassType != null)
        //            this.Inputs.Add(p.PropertyName, p.DomainClassType.FullName);
        //    }

        //    //outputs
        //    this.Outputs = new Dictionary<string, string>();
        //    foreach (PropertyDescription p in sc.Outputs())
        //    {
        //        if (p != null && p.PropertyName != null && p.DomainClassType != null)
        //            this.Outputs.Add(p.PropertyName, p.DomainClassType.FullName);
        //    }

        //    // Parameters
        //    this.Parameters = new Dictionary<string, VarInfo>();
        //    foreach (VarInfo p in sc.Parameters())
        //    {
        //        {
        //            if (p != null && p.Name != null)
        //            {
        //                int size;
        //                p.ValueType = GetVarInfoValueTypeForPropertyOfAStrategy(oldStrategy, p.Name, out size);
        //                p.Size = size;
        //                this.Parameters.Add(p.Name, p);
        //            }
        //        }
        //    }

        //    // Global                        
        //    if (this.AssociatedStrategies != null && this.AssociatedStrategies.Count > 0)
        //    {
        //        this.SimpleStrategy = "";
        //        this.CompositeStrategy = "checked";
        //    }
        //    else
        //    {
        //        this.SimpleStrategy = "checked";
        //        this.CompositeStrategy = "";
        //    }

        //    this.DataInterfacesDLL = oldStrategyBaseInterface.Assembly.Location;
        //    this.StrategiesDLL = oldStrategy.GetType().Assembly.Location;

        //    // General
        //    this.Domain = ""; //no way to know from old strategy
        //    this.ModelType = ""; //no way to know from old strategy
        //    this.StrategyName = oldStrategy.GetType().Name;
        //    this.NameSpace = oldStrategy.GetType().Namespace;
        //    this.StrategyDescription = oldStrategy.Description;
        //    this.TimeSteps = new List<string>(); //no way to know from old strategy
        //    this.IsContext = "false"; //no way to know from old strategy
        //    this.Author = ""; //no way to know from old strategy
        //    this.Institution = ""; //no way to know from old strategy
        //    this.Email = ""; //no way to know from old strategy
        //    this.DateFirstRelease = ""; //no way to know from old strategy
        //    this.AuthorRevision = ""; //no way to know from old strategy
        //    this.EmailRevision = ""; //no way to know from old strategy
        //    this.DateRevision = ""; //no way to know from old strategy
        //}

        private VarInfoValueTypes GetVarInfoValueTypeForPropertyOfAStrategy(object strategy, string propertyName,
            out int propertySize)
        {
            propertySize = 1;

            //try to get the correct value type from the Type of the property of the old strategy
            PropertyInfo corresponentProperty =
                strategy.GetType().GetProperties().Where(a => a.Name.Equals(propertyName)).FirstOrDefault();
            if (corresponentProperty != null)
            {
                Type t = corresponentProperty.PropertyType;

                var suitableVarInfoValueTypeForThisPropertyType =
                    VarInfoValueTypes.Values.Where(a => a.TypeForCurrentValue.Equals(t)).LastOrDefault();
                if (suitableVarInfoValueTypeForThisPropertyType != null)
                {
                    //if size required check in the value of the property (if set)
                    if (suitableVarInfoValueTypeForThisPropertyType.RequiresSizeInTypeDefinition)
                    {
                        object val = corresponentProperty.GetValue(strategy, new object[0]);
                        if (val != null && val.GetType().IsArray)
                        {
                            int size = ((Array) val).Length;

                            propertySize = size;
                        }
                        return suitableVarInfoValueTypeForThisPropertyType;
                    }
                    else
                    {
                        return suitableVarInfoValueTypeForThisPropertyType;
                    }
                }
                else
                {
                    return VarInfoValueTypes.GetInstanceForName("Double"); //default
                }
            }
            else
            {
                return VarInfoValueTypes.GetInstanceForName("Double"); //default
            }
        }

        public SwitchValueRelatedModelingOptions GeneralModelingOptions
        {
            get
            {
                return new SwitchValueRelatedModelingOptions()
                {
                    Inputs = this.Inputs.Select(a => a.Key).ToArray().ToList(),
                    Outputs = this.Outputs.Select(a => a.Key).ToArray().ToList(),
                    Parameters = this.Parameters.Select(a => a.Key).ToArray().ToList(),
                    AssociatedStrategies = this.AssociatedStrategies.Select(a => a.Key).ToArray().ToList()
                };
            }
        }
    }
}
