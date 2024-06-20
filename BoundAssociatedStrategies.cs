using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections;
using CRA.ModelLayer.Strategy;
using System.Runtime.Loader;
//using IStrategy = CRA.Core.Preconditions.IStrategy;

namespace CRA.ModelLayer.SCC
{
    internal class CollectibleAssemblyLoadContext
    : AssemblyLoadContext
    {
        public CollectibleAssemblyLoadContext() : base(isCollectible: true)
        {}

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }

    internal class AssStrategies
    {
        #region gList, gestione degli elementi caricati come DLL+strategia
        /// <summary>tiene conto di DLL+strategy presenti nell'elenco strategie associate</summary>
        List<string> gList = new List<string>();

        /// <summary>azioni possibili sul controllo che gestisce le DLL e strategie</summary>
        public enum Actions
        {
            STRATEGY_ADD, // è stata aggiunta una strategy associata
            STRATEGY_REMOVE, // è stata rimossa una strategy associata
            STRATEGY_REMOVEALL, // è stata rimossa tutta la storia delle strategy associata
            STRATEGY_REORDER // è stata modificato l'ordine di una strategy
        }

        /// <summary>compone path-name della dll caricata come strategy e la classe che implementa la strategia</summary>
        /// <param name="fname">path-name della dll caricata come strategy</param>
        /// <param name="strategy">classe che implementa la strategia</param>
        /// <returns></returns>
        private string ComponiFilenameAndStrategy(string fname, string strategy)
        {
            return fname + "$" + strategy;
        }

        /// <summary>recupera path-name della dll caricata come strategy</summary>
        /// <param name="glist"></param>
        /// <returns></returns>
        private string GetFname(string glist)
        {
            return glist.Split('$')[0];
        }

        /// <summary>recupera la classe che implementa la strategy </summary>
        /// <param name="glist"></param>
        /// <returns></returns>
        private string GetStrategy(string glist)
        {
            return glist.Split('$')[1];
        }
        #endregion

        /// <summary>in uno specifico appdomain esegue le operazioni di lettura di input/output (i parametri sono gestiti altrove)</summary>
        /// <param name="selector">for switch case based on click on different list item</param>
        /// <param name="fname">path to dll</param>
        /// <param name="strategyOrInterface">selected item in listBox</param>
        /// <param name="member"></param>
        /// <param name="gList">used in list item 8</param>
        /// <returns></returns>
        private static MarshalByValType Marshalling(MarshalByRefType.Selettore selector, string fname, string strategyOrInterface,
            string member, List<string> gList)
        {
            // Get and display the assembly in our AppDomain that contains the 'Main' method
            var exeAssembly = Assembly.GetEntryAssembly().FullName;
            // Create new AppDomain (security and configuration match current AppDomain)
            //var appSetup = new AppDomainSetup
            //{
            //    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
            //    PrivateBinPath = fname
            //};
            //appSetup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Components";
            //AppDomain ad2 = AppDomain.CreateDomain("AD #1", null, appSetup);

            Assembly loadContextAssembly = CollectibleAssemblyLoadContext.Default.LoadFromAssemblyName(Assembly.GetEntryAssembly().GetName());

            //
            // Load our assembly into the new AppDomain, construct an object, marshal
            // it back to our AD (we really get a reference to a proxy)
            var tmp = new object[]
            {selector, fname, strategyOrInterface, member, gList};
            //var mbrt = (MarshalByRefType) ad2.CreateInstanceAndUnwrap(
            //    exeAssembly,
            //    "CRA.ModelLayer.SCC.MarshalByRefType",
            //    true,
            //    BindingFlags.Default,
            //    null,
            //    tmp,
            //    null,
            //    null);

            var mbrt = (MarshalByRefType)loadContextAssembly.CreateInstance("CRA.ModelLayer.SCC.MarshalByRefType", true, BindingFlags.Default, null, tmp, null, null);

            // 
            MarshalByValType mbvt = mbrt.MethodWithReturn();
            //
            //AppDomain.Unload(ad2);
            //AssemblyLoadContext.Default.Unload();
            //
            return mbvt;
        }

        //When una Classe che implementa IStrategy è aggiunta dal selettore
        //servono Inputs Utente
        //Servono Outputs Utente
        //Servono le classi di Strategy caricaredgwInputs
        public void WhenStrategyClassAddedOrRemoved(
            // DLL e strategy
            string fname, string strategy, Actions action,
            // dati utente
            DataGridView dgwImputsUtente = null,
            DataGridView dgwOutputsUtente = null,
            DataGridView dgwParametersUtente = null,
            // dati strategy
            DataGridView dgwInputsStrategy = null,
            DataGridView dgwOutputsStrategy = null,
            DataGridView dgwParametersStrategy = null)
        {
            //2016-05-17: i parametri sono gestiti altrove
            //2016-06-05: i parametri si aggiungono in relazione alla strategy considerata se siamo in composite
            bool pDaStrategyIsEnabled = true;

            #region fields
            List<InOutPar> InputsDaStrategyPrecedente = new List<InOutPar>();
            List<InOutPar> InputsDaStrategyCorrente = new List<InOutPar>();
            List<InOutPar> InputsDaStrategyMappata = new List<InOutPar>();
            List<InOutPar> OutputsDaStrategyPrecedente = new List<InOutPar>();
            List<InOutPar> OutputsDaStrategyCorrente = new List<InOutPar>();
            List<Parameters> ParametersDaStrategyPrecedente = new List<Parameters>();
            List<Parameters> ParametersDaStrategyCorrente = new List<Parameters>();
            #endregion

            #region actions - tengo traccia degli elementi grazie alla action
            switch (action)
            {
                case Actions.STRATEGY_ADD:
                    gList.Add(ComponiFilenameAndStrategy(fname, strategy));
                    break;
                case Actions.STRATEGY_REMOVE:
                    //TODO: se non mi passano la DLL vuol dire che ho solo il contenuto delle strategy associate.
                    //cerco allora tra tutte le strategy quella che coincide con la strategy clikkata 
                    //e determino io quale doveva essere la DLL caricata.
                    if (fname.Equals("*"))
                    {
                        string tmp = "";
                        foreach (var _g in gList)
                        {
                            if (GetStrategy(_g).Equals(strategy))
                            {
                                tmp = ComponiFilenameAndStrategy(GetFname(_g), strategy);
                                break;
                            }
                        }
                        if(!string.IsNullOrWhiteSpace(tmp)) gList.Remove(tmp);
                    }
                    else
                    {
                        gList.Remove(ComponiFilenameAndStrategy(fname, strategy));
                    }
                    break;
                case Actions.STRATEGY_REMOVEALL:
                    gList.Clear();
                    break;
                    ;
                case Actions.STRATEGY_REORDER:
                    break;
            }
            #endregion

            #region per ogni elemento aggiunto ricalcolo tutto
            foreach (var gg in gList)
            {
                fname = GetFname(gg); //gg.Split('$')[0];
                strategy = GetStrategy(gg); //gg.Split('$')[1];
                //
                // calcolo i nuovi input/output da strategy ripuliti da graficare  
                try
                {
                    MarshalByValType _marshalByValType = Marshalling(
                        MarshalByRefType.Selettore.GET_INPUTS_STRATEGY, fname, strategy, "", gList);
                    //
                    //map input letti con output precedenti
                    InputsDaStrategyMappata = MapNewInputsWithOutputsBefore(_marshalByValType.NuoviInputsDaStrategy, OutputsDaStrategyPrecedente);
                    //merge degli input mappati con input precedenti (NOTA: tutti i precedenti inputs)
                    InputsDaStrategyCorrente = MergeNewInputsWithInputs(InputsDaStrategyMappata, InputsDaStrategyPrecedente);
                    //
                    //merge degli output correnti e precedenti (NOTA: tutti i precedenti outputs)
                    OutputsDaStrategyCorrente = MergeNewOutputsWithOutputs(_marshalByValType.NuoviOutputsDaStrategy, OutputsDaStrategyPrecedente);

                    //merge dei parametri correnti e precedenti
                    if (pDaStrategyIsEnabled) ParametersDaStrategyCorrente = MergeNewParametersWithParametrs(_marshalByValType.NuoviParametersDaStrategy, ParametersDaStrategyPrecedente);
                    
                    //per il prossimo giro /sostituisco quelli che saranno i set precedenti
                    InputsDaStrategyPrecedente = InputsDaStrategyCorrente;
                    OutputsDaStrategyPrecedente = OutputsDaStrategyCorrente;
                    if (pDaStrategyIsEnabled) ParametersDaStrategyPrecedente = ParametersDaStrategyCorrente;

                    _marshalByValType = null;
                }
                catch (NullReferenceException)
                {
                    throw new Exception("Null Reference Exception ");
                }
                catch (Exception err)
                {
                    throw new Exception("Unable to create instance of type ");
                }
            }
            #endregion

            #region popolo la griglia degli inputs da strategy
            dgwInputsStrategy.AutoGenerateColumns = false;

            if (dgwInputsStrategy.Columns.Count<2)// && !dgwInputsStrategy.Columns[0].HeaderText.Contains("Inputs"))
            {
                DataGridViewTextBoxColumn inputsColumn = new DataGridViewTextBoxColumn();
                inputsColumn.DataPropertyName = "PropertyName";
                inputsColumn.HeaderText = "Inputs";
                dgwInputsStrategy.Columns.Add(inputsColumn);
            }

            if (dgwInputsStrategy.Columns.Count <2)// && !dgwInputsStrategy.Columns[1].HeaderText.Contains("Domain Class"))
            {
                DataGridViewTextBoxColumn domainColumnI = new DataGridViewTextBoxColumn();
                domainColumnI.DataPropertyName = "DomainClass";
                domainColumnI.HeaderText = "Domain Class";
                dgwInputsStrategy.Columns.Add(domainColumnI);
            }
            var bindingListI = new BindingList<InOutPar>(InputsDaStrategyCorrente);
            var sourceI = new BindingSource(bindingListI, null);            

            //#LE: 2016-05-08 ricalcolo tutta la griglia delle strategy che aggiungo => devo svuotare le caselle di visualizzazione
            dgwInputsStrategy.Rows.Clear();
            foreach (object _t in sourceI)
            {
                int idx = dgwInputsStrategy.Rows.Add();
                InOutPar iop = (InOutPar)_t;
                dgwInputsStrategy.Rows[idx].Cells[0].Value = iop.PropertyName;
                dgwInputsStrategy.Rows[idx].Cells[1].Value = iop.DomainClass;
            }

            #endregion
            #region popolo la griglia degli outputs da strategy
            dgwOutputsStrategy.AutoGenerateColumns = false;

            if (dgwOutputsStrategy.Columns.Count < 2) //!dgwOutputsStrategy.Columns[0].HeaderText.Contains("Outputs"))
            {
                DataGridViewTextBoxColumn outputsColumn = new DataGridViewTextBoxColumn();
                outputsColumn.DataPropertyName = "PropertyName";
                outputsColumn.HeaderText = "Outputs";
                dgwOutputsStrategy.Columns.Add(outputsColumn);
            }

            if (dgwOutputsStrategy.Columns.Count < 2) //!dgwOutputsStrategy.Columns[1].HeaderText.Contains("Domain Class"))
            {
                DataGridViewTextBoxColumn domainColumnO = new DataGridViewTextBoxColumn();
                domainColumnO.DataPropertyName = "DomainClass";
                domainColumnO.HeaderText = "Domain Class";
                dgwOutputsStrategy.Columns.Add(domainColumnO); 
            }
            
            var bindingListO = new BindingList<InOutPar>(OutputsDaStrategyCorrente);
            var sourceO = new BindingSource(bindingListO, null);

            //#LE: 2016-05-08 ricalcolo tutta la griglia delle strategy che aggiungo => devo svuotare le caselle di visualizzazione
            dgwOutputsStrategy.Rows.Clear();
            foreach (object _t in sourceO)
            {
                int idx = dgwOutputsStrategy.Rows.Add();
                InOutPar iop = (InOutPar)_t;
                dgwOutputsStrategy.Rows[idx].Cells[0].Value = iop.PropertyName;
                dgwOutputsStrategy.Rows[idx].Cells[1].Value = iop.DomainClass;
            }

            #endregion
            #region popolo la griglia dei parameters da strategy
            if (pDaStrategyIsEnabled) GetParametersFromStrategyDll(dgwParametersStrategy, ParametersDaStrategyCorrente);
            #endregion
        }

        /// <summary>copia i parametri della strategy caricata dalla dll nella griglia parametri </summary>
        /// <param name="dgwParametersStrategy">griglia parametri in tab parametri da strategy</param>
        /// <param name="parametersDaStrategyCorrente">parametri letti da strategy dll</param>
        private static void GetParametersFromStrategyDll(DataGridView dgwParametersStrategy, List<Parameters> parametersDaStrategyCorrente)
        {
            dgwParametersStrategy.AutoGenerateColumns = false;

            if (dgwParametersStrategy.Columns.Count < 7)
            {
                DataGridViewTextBoxColumn varInfoNameColumn = new DataGridViewTextBoxColumn();
                varInfoNameColumn.DataPropertyName = "VarInfoName";
                varInfoNameColumn.HeaderText = "NameVar";
                dgwParametersStrategy.Columns.Add(varInfoNameColumn);
            }
            if (dgwParametersStrategy.Columns.Count < 7)
            {
                DataGridViewTextBoxColumn varInfoDescriptionColumn = new DataGridViewTextBoxColumn();
                varInfoDescriptionColumn.DataPropertyName = "VarInfoDescription";
                varInfoDescriptionColumn.HeaderText = "Description";
                dgwParametersStrategy.Columns.Add(varInfoDescriptionColumn);
            }
            if (dgwParametersStrategy.Columns.Count < 7)
            {
                DataGridViewTextBoxColumn varInfoMinValueColumn = new DataGridViewTextBoxColumn();
                varInfoMinValueColumn.DataPropertyName = "VarInfoMinValue";
                varInfoMinValueColumn.HeaderText = "MinValue";
                dgwParametersStrategy.Columns.Add(varInfoMinValueColumn);
            }
            if (dgwParametersStrategy.Columns.Count < 7)
            {
                DataGridViewTextBoxColumn varInfoMaxValueColumn = new DataGridViewTextBoxColumn();
                varInfoMaxValueColumn.DataPropertyName = "VarInfoMaxValue";
                varInfoMaxValueColumn.HeaderText = "MaxValue";
                dgwParametersStrategy.Columns.Add(varInfoMaxValueColumn);
            }
            if (dgwParametersStrategy.Columns.Count < 7)
            {
                DataGridViewTextBoxColumn varInfoDefaultValueColumn = new DataGridViewTextBoxColumn();
                varInfoDefaultValueColumn.DataPropertyName = "VarInfoDefaultValue";
                varInfoDefaultValueColumn.HeaderText = "DefaultValue";
                dgwParametersStrategy.Columns.Add(varInfoDefaultValueColumn);
            }
            if (dgwParametersStrategy.Columns.Count < 7)
            {
                DataGridViewTextBoxColumn varInfoUnitsColumn = new DataGridViewTextBoxColumn();
                varInfoUnitsColumn.DataPropertyName = "VarInfoUnits";
                varInfoUnitsColumn.HeaderText = "Units";
                dgwParametersStrategy.Columns.Add(varInfoUnitsColumn);
            }
            if (dgwParametersStrategy.Columns.Count < 7)
            {
                DataGridViewTextBoxColumn varInfoValueTypesColumn = new DataGridViewTextBoxColumn();
                varInfoValueTypesColumn.DataPropertyName = "VarInfoValueTypes";
                varInfoValueTypesColumn.HeaderText = "ValueType";
                dgwParametersStrategy.Columns.Add(varInfoValueTypesColumn);
            }

            var bindingListP = new BindingList<Parameters>(parametersDaStrategyCorrente);
            var sourceP = new BindingSource(bindingListP, null);

            //#LE: 2016-05-08 ricalcolo tutta la griglia delle strategy che aggiungo => devo svuotare le caselle di visualizzazione
            dgwParametersStrategy.Rows.Clear();
            foreach (object _t in sourceP)
            {
                int row = dgwParametersStrategy.Rows.Add();
                Parameters vi = (Parameters) _t;
                dgwParametersStrategy.Rows[row].Cells[0].Value = vi.VarInfoName;
                dgwParametersStrategy.Rows[row].Cells[1].Value = vi.VarInfoDescription;
                dgwParametersStrategy.Rows[row].Cells[2].Value = vi.VarInfoMinValue.ToString();
                dgwParametersStrategy.Rows[row].Cells[3].Value = vi.VarInfoMaxValue.ToString();
                dgwParametersStrategy.Rows[row].Cells[4].Value = vi.VarInfoDefaultValue.ToString();
                dgwParametersStrategy.Rows[row].Cells[5].Value = vi.VarInfoUnits;
                dgwParametersStrategy.Rows[row].Cells[6].Value = vi.VarInfoValueTypes;
                dgwParametersStrategy.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>When una Classe che implementa IStrategy è aggiunta dal selettore </summary>
        /// <param name="fname"></param>
        /// <param name="strategies"></param>
        /// <param name="dgwParametersStrategy"></param>
        public void WhenStrategyClassAddedOrRemovedFindAllParameters(string fname, IEnumerable strategies, DataGridView dgwParametersStrategy)
        {
            List<Parameters> parametersDaStrategyCorrente = new List<Parameters>(); ;
            List<Parameters> parametersDaStrategyPrecedente = new List<Parameters>();
            List<string> _glist = new List<string>();

            foreach (var strategy in strategies)
            {
                // calcolo i nuovi input/output da strategy ripuliti da graficare  
                try
                {
                    MarshalByValType _marshalByValType = Marshalling(
                        MarshalByRefType.Selettore.GET_PARAMETRS_STRATEGY, fname, strategy.ToString(), "", _glist);

                    if (_marshalByValType.NuoviParametersDaStrategy != null)
                    {
                        parametersDaStrategyCorrente = MergeNewParametersWithParametrs(_marshalByValType.NuoviParametersDaStrategy,
                                parametersDaStrategyPrecedente);
                        parametersDaStrategyPrecedente = parametersDaStrategyCorrente;
                    }
                    _marshalByValType = null;
                }
                catch (NullReferenceException)
                {
                    throw new Exception("Null Reference Exception ");
                }
                catch (Exception err)
                {
                    throw new Exception("Unable to create instance of type ");
                }
            }

            #region popolo la griglia dei parameters da strategy
            GetParametersFromStrategyDll(dgwParametersStrategy, parametersDaStrategyCorrente.OrderBy(name => name.VarInfoName).ToList());
            #endregion
        }

        ///////////////////////////////////////////////////////////////////////////////////
        // CASO USO:

        /// <summary>MAP: prende i nuovi inputs e se ci sono degli output delle strategy precedenti li elimina</summary>
        /// <param name="inpN">inpN: inputs nuovi derivanti dalla nuova classe strategy aggiunta</param>
        /// <param name="outP">outP: outputs delle strutture di strategy precedenti che nel caso devono compensare i nuovi inputs</param>
        /// <returns>calcolo il nuovo inputs risultantie dall'elisione tra output precedenti e inputt nuovi</returns>
        private List<InOutPar> MapNewInputsWithOutputsBefore(
            IEnumerable<InOutPar> inpN,
            IEnumerable<InOutPar> outP)
        {
            //calcolo il nuovo inputs risultantie dall'elisione tra output precedenti e inputt nuovi
            IEnumerable<InOutPar> mapI = inpN.Except(outP);
            return mapI.ToList();
        }

        /// <summary>MERGE: unisce in modo distinto gli input nuovi con gli inputs vecchi</summary>
        /// <param name="mapI">mapI: inputs nuovi su cui ho già applicato un processo di map tra inputs(X) e outputs(X-1)</param>
        /// <param name="inpP">inpP: gli inputs precedenti a cui devo aggiungere gli inputs mapI, su cui devo applicare il processo di union per averli distinti</param>
        /// <returns>//calcolo il nuovo inputs risultantie dal merge distinto dei due differenti set</returns>
        private List<InOutPar> MergeNewInputsWithInputs(
            IEnumerable<InOutPar> mapI,
            IEnumerable<InOutPar> inpP)
        {
            //calcolo il nuovo inputs risultantie dal merge distinto dei due differenti set
            IEnumerable<InOutPar> inputs = inpP.Union(mapI, new InputOutputComparer());
            return inputs.ToList();
        }

        /// <summary>MERGE: unisce in modo distinto gli output nuovi con gli outputs vecchi</summary>
        /// <param name="outN">outN: outputs nuovi derivanti da nuova strategia</param>
        /// <param name="outP">outP: gli outputs precedenti, su cui devo applicare il processo di union per averli distinti</param>
        /// <returns>//calcolo il nuovo outputs risultantie dal merge distinto dei due differenti set</returns>
        private List<InOutPar> MergeNewOutputsWithOutputs(
            IEnumerable<InOutPar> outN,
            IEnumerable<InOutPar> outP)
        {
            //calcolo il nuovo outputs risultantie dal merge distinto dei due differenti set
            IEnumerable<InOutPar> outputs = outP.Union(outN, new InputOutputComparer());
            return outputs.ToList();
        }

        /// <summary>MERGE: unisce in modo distinto i parametri nuovi con i parametri vecchi</summary>
        /// <param name="parN">parametri nuovi derivanti da nuova strategia</param>
        /// <param name="parP">i parametri precedenti, su cui devo applicare il processo di union per averli distinti</param>
        /// <returns>//calcolo i nuovi parametri risultantie dal merge distinto dei due differenti set</returns>
        private List<Parameters> MergeNewParametersWithParametrs(
            IEnumerable<Parameters> parN,
            IEnumerable<Parameters> parP)
        {
            //calcolo il nuovo outputs risultantie dal merge distinto dei due differenti set
            IEnumerable<Parameters> parameters = parP.Union(parN, new ParameterComparer());
            return parameters.ToList();
        }
    }

    // Instances can be marshaled-by-reference across AppDomain boundaries
    public sealed class MarshalByRefType : MarshalByRefObject
    {
        // inputs della strategy caricata
        private List<InOutPar> Inputs { get; set; }

        // outputs della strategy caricata
        private List<InOutPar> Outputs { get; set; }

        // parametrs della strategy caricata
        private List<Parameters> Parameters { get; set; }

        public enum Selettore
        {
            GET_INPUTS_STRATEGY,
            GET_OUTPUTS_STRATEGY,
            GET_PARAMETRS_STRATEGY
        }

        public MarshalByRefType(
            Selettore selettore, 
            string fname,
            string strategyOrInterface, 
            string member,
            List<string> list9)
        {
            var a = Assembly.LoadFrom(fname);
            switch (selettore)
            {
                case Selettore.GET_INPUTS_STRATEGY:
                case Selettore.GET_OUTPUTS_STRATEGY:
                case Selettore.GET_PARAMETRS_STRATEGY:
                    Type[] types;
                    try{types = a.GetTypes();}
                    catch (Exception err)
                    {
                        const string msg ="On Load Strategy Inputs. maybe a problems on types.\t\n";
                        throw new Exception(msg + err.Message);
                    }

                    foreach (var t in types)
                    {
                        try
                        {
                            if (
                                t.IsClass && 
                                t.IsPublic && 
                                !t.IsAbstract && 
                                t.FullName.Equals(strategyOrInterface))
                            {
                                var ele = a.CreateInstance(t.ToString());
                                if (ele != null)
                                {
                                    Inputs = new List<InOutPar>();
                                    Outputs = new List<InOutPar>();
                                    Parameters = new List<Parameters>();

                                    PropertyInfo pi = ele.GetType().GetProperty("ModellingOptionsManager");
                                    if (pi != null)
                                    {
                                        var modellingOptionsManager = (ModellingOptionsManager)pi.GetValue(ele, null);
                                        if (modellingOptionsManager != null)
                                        {
                                            try
                                            {
                                                if (modellingOptionsManager.AllPossibleInputs != null)
                                                {
                                                    Inputs = FromPropertyDescription2InOutPar(modellingOptionsManager.AllPossibleInputs);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex);       
                                            }
                                            //
                                            try
                                            {
                                                if (modellingOptionsManager.AllPossibleOutputs != null)
                                                {
                                                    Outputs = FromPropertyDescription2InOutPar(modellingOptionsManager.AllPossibleOutputs);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex);
                                            }
                                            //
                                            try
                                            {
                                                if (modellingOptionsManager.AllPossibleParameters != null)
                                                {
                                                    Parameters = FromVarInfo2Parametrs(modellingOptionsManager.AllPossibleParameters);
                                                }                                                
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            const string msg = "On Load Strategy Inputs. Maybe a problems on marshalling.\t\n";
                            throw new Exception(msg + err.Message);                        
                        }
                    }
                    break;
            }
        }

        /// <summary></summary>
        public MarshalByValType MethodWithReturn()
        {
            var t = new MarshalByValType(Inputs, Outputs, Parameters);
            return t;
        }

        private List<InOutPar> FromPropertyDescription2InOutPar(IEnumerable<Strategy.PropertyDescription> source)
        {
            var retval = new List<InOutPar>();

            foreach (var _propertyDescription in source)
            {
                //#LE: 2016-05-08: fix ci sono degli Outputs (forse Inputs) che hanno PropertyDescription con proprietà NULL (ModelLayer ?)
                if (!string.IsNullOrEmpty(_propertyDescription.PropertyName) &&
                    !string.IsNullOrEmpty(_propertyDescription.DomainClassType.FullName)
                    )
                {
                    var element = new InOutPar()
                    {
                        PropertyName = _propertyDescription.PropertyName,
                        DomainClass =
                            GetClassFromSingleDomainClasses(_propertyDescription.DomainClassType.FullName)
                            + " - "
                            + _propertyDescription.DomainClassType.FullName
                    };
                    retval.Add(element);
                }
                else
                {
                    _propertyDescription.PropertyName = "AA";
                }
            }
            return retval;
        }

        private List<Parameters> FromVarInfo2Parametrs(IEnumerable<CRA.ModelLayer.Core.VarInfo> source)
        {
            List<Parameters> parameters = new List<Parameters>();
            foreach (var _v in source)
            {
                Parameters p = new Parameters
                {
                    VarInfoDefaultValue = _v.DefaultValue,
                    VarInfoDescription = _v.Description,
                    VarInfoMaxValue = _v.MaxValue,
                    VarInfoMinValue = _v.MinValue,
                    VarInfoName = _v.Name,
                    VarInfoUnits = _v.Units,     
                    VarInfoValueTypes = _v.ValueType.Name
                };
                parameters.Add(p);
            }
            return parameters;
        }

        private string GetClassFromSingleDomainClasses(string singleDomainClasses)
        {
            string[] dom = singleDomainClasses.Split('.');
            return dom[dom.Count() - 1].ToLower();
        }
    }

    /// <summary>Instances can be marshaled-by-value across AppDomain boundaries</summary>
    [Serializable]
    public sealed class MarshalByValType : Object
    {
        /// <summary></summary>
        public MarshalByValType(
            List<InOutPar> nuoviInputsDaStrategy,
            List<InOutPar> nuoviOutputsDaStrategy,
            List<Parameters> nuoviParametersDaStrategy)
        {
            NuoviInputsDaStrategy = nuoviInputsDaStrategy;
            NuoviOutputsDaStrategy = nuoviOutputsDaStrategy;
            NuoviParametersDaStrategy = nuoviParametersDaStrategy;
        }

        /// <summary>nuovi inputs da strategy</summary>
        public List<InOutPar> NuoviInputsDaStrategy { get; private set; }

        /// <summary>nuovi outputs da strategy</summary>
        public List<InOutPar> NuoviOutputsDaStrategy { get; private set; }

        /// <summary>nuovi Parameters da strategy</summary>
        public List<Parameters> NuoviParametersDaStrategy { get; private set; }
    }

    [Serializable]
    public class InOutPar
    {
        public string PropertyName { get; set; }
        public string DomainClass { get; set; }
    }

    [Serializable]
    public class Parameters
    {
        public string VarInfoDescription { get; set; }
        public string VarInfoName { get; set; }
        public string VarInfoUnits { get; set; }
        public double VarInfoMinValue { get; set; }
        public double VarInfoMaxValue { get; set; }
        public double VarInfoDefaultValue { get; set; }
        public string VarInfoValueTypes { get; set; }
    }

    public class InputOutputComparer : IEqualityComparer<InOutPar>
    {
        public bool Equals(InOutPar p1, InOutPar p2)
        {
            //#LE#: Il controllo viene esteso. Oltre alla proprietà viene verificata anche la uguagliana della DomainClass
            //NOTA: modifico il sistema di controllo sull'uguaglianza delle variabili
            //potrebbe avere impatti su composite, perchè le var verranno duplicate se su domain differenti
            //return p1.PropertyName == p2.PropertyName;
            return (p1.PropertyName == p2.PropertyName) && (p1.DomainClass == p2.DomainClass);
        }

        public int GetHashCode(InOutPar p)
        {
            return p.PropertyName.GetHashCode();
        }
    }

    public class ParameterComparer : IEqualityComparer<Parameters>
    {
        public bool Equals(Parameters p1, Parameters p2)
        {
            return p1.VarInfoName == p2.VarInfoName;
        }

        public int GetHashCode(Parameters p)
        {
            return p.VarInfoName.GetHashCode();
        }
    }
}