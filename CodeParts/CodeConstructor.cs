using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRA.ModelLayer.SCC.CodeParts
{
    internal class CodeConstructor : ICodePart
    {
        internal CodeConstructor(Controller crtl)
        {
            _crtl = crtl;

        }


        private Controller _crtl;

        public string WriteCodeLines(string newCodeText)
        {
            string UpdatedCodeText;

            // Insert here logic
            string placeHolder = "(#Namespace#)";

            // Insert here logic
            string codeLines;
            codeLines = _crtl.NameSpace;

            // Update property
            string tempUpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, newCodeText, codeLines);


            // Insert here logic
            placeHolder = "(#ClassName#)";

            // Insert here logic
            codeLines = _crtl.StrategyName;

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, tempUpdatedCodeText, codeLines);


            // Insert here logic
            placeHolder = "(#SetPublisherMetaInformation#)";



            // Insert here logic
            if (_crtl.Author == null) _crtl.Author = "";
            if (_crtl.DateFirstRelease == null) _crtl.DateFirstRelease = "";
            codeLines = "\r\n\t\t\t\t_pd = new CRA.ModelLayer.MetadataTypes.PublisherData();";
            codeLines += "\r\n\t\t\t\t_pd.Add(\"Creator\", \"" + _crtl.Email + "\");";
            codeLines += "\r\n\t\t\t\t_pd.Add(\"Date\", \"" + _crtl.DateFirstRelease + "\");";
            codeLines += "\r\n\t\t\t\t_pd.Add(\"Publisher\", \"" + _crtl.Institution + "\");";

            //"\r\n\t\tAuthor:" +_crtl.Author+"\r\n\t\tInstitution:"+ _crtl.Institution+"\r\n\t\tDate of first release:"+ _crtl.DateFirstRelease+"\r\n\t\tEmail:"+ _crtl.Email+"\r\n\t\tAuthorRevision:"+ _crtl.AuthorRevision+"\r\n\t\tEmail of revision author:"+ _crtl.EmailRevision+"\r\n\t\tDate of revision:"+ _crtl.DateRevision;

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);



            // Insert here logic
            placeHolder = "(#ClassHeaderComments#)";

            //example
            ////author:xxx
            ////date:xxx


            // Insert here logic
            if (_crtl.Author == null) _crtl.Author = "";
            if (_crtl.DateFirstRelease == null) _crtl.DateFirstRelease = "";
            codeLines = "";
            codeLines += "\r\n //Author:" + _crtl.Author + " " + _crtl.Email;
            codeLines += "\r\n //Institution:" + _crtl.Institution;
            codeLines += "\r\n //Author of revision:" + _crtl.AuthorRevision + " " + _crtl.EmailRevision;
            codeLines += "\r\n //Date first release:" + _crtl.DateFirstRelease;
            codeLines += "\r\n //Date of revision:" + _crtl.DateRevision;


            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            ////ModellingOptionsManager

            //example
            //ModellingOptions mo = new ModellingOptions();
            //VarInfo v1 = new VarInfo();
            //v1.CurrentValue = 0.1;
            //v1.DefaultValue = 0.3;
            //v1.Description = "desc";
            //v1.Id = 0;
            //v1.MaxValue = 100;
            //v1.MinValue = -100;
            //v1.Name = "Var1";
            //v1.Size = 1;
            //v1.Units = "C";
            //v1.URL = "";
            //v1.VarType = VarInfo.Type.PARAMETER;
            //mo.Parameters = new List<VarInfo>() { v1 };
            //mo.AssociatedStrategies = new List<string>() { "as1", "as2" };
            //PropertyDescription pd1 = new PropertyDescription();
            //pd1.DomainClassType = typeof(DC1);
            //pd1.PropertyName = "P1DC1";
            //pd1.PropertyType = typeof(double);
            //pd1.PropertyVarInfo = CRA.Clima.Rain.Interfaces.RainDataVarInfo.Rain.ToVarInfo();
            //mo.Inputs = new List<PropertyDescription>() { pd1 };
            //PropertyDescription pd2 = new PropertyDescription();
            //pd2.DomainClassType = typeof(DC1);
            //pd2.PropertyName = "P2DC1";
            //pd2.PropertyType = typeof(double);
            //PropertyDescription pd3 = new PropertyDescription();
            //pd3.DomainClassType = typeof(DC1);
            //pd3.PropertyName = "P3DC1";
            //pd3.PropertyType = typeof(double);
            //PropertyDescription pd4 = new PropertyDescription();
            //pd4.DomainClassType = typeof(DC2);
            //pd4.PropertyName = "P1DC2";
            //pd4.PropertyType = typeof(double);
            //mo.Outputs = new List<PropertyDescription>() { pd2, pd3, pd4 };
            //_modellingOptionsManager = new ModellingOptionsManager(mo);


            placeHolder = "(#ModellingOptionsManager#)";
            codeLines = "";
            string ids = "";
            int switchProgrNumber = 0;
            int i = 0;
            int j = 0;

            if (_crtl.Switches.Count > 0)
            {
                foreach (BuildingSwitch sw in _crtl.Switches.Values)
                {
                    codeLines += "\r\n\r\n\t\t\t\t//Switch number " + switchProgrNumber + " (" + sw.SwitchName + ")";
                    int switchValueProgrNumber = 0;
                    codeLines += "\r\n\t\t\t\tDictionary<string, ModellingOptions> d" + switchProgrNumber + "= new Dictionary<string, ModellingOptions>();";

                    foreach (string swValue in sw.SwitchValueRelatedModelingOptions4SwitchValues.Keys)
                    {
                        codeLines += "\r\n\t\t\t\t//Switch value '" + swValue + "'";
                        string modelingOptionIdentifier = "mo" + switchProgrNumber + "_" + switchValueProgrNumber;

                        // Insert here logic
                        codeLines += "\r\n\t\t\t\tModellingOptions " + modelingOptionIdentifier + " = new ModellingOptions();";
                        codeLines += "\r\n\t\t\t\t//Parameters\r\n\t\t\t\tList<VarInfo> _parameters" + switchProgrNumber + "_" + switchValueProgrNumber + " = new List<VarInfo>();";

                        List<KeyValuePair<string, string>> allParamsFromAssociatedStrategies = new List<KeyValuePair<string, string>>();
                        if (_crtl.CompositeStrategy.Equals("checked"))
                        {
                            //collect all the different params from the associated strategies               
                            foreach (var assoc in _crtl.AssociatedStrategies)
                            {
                                foreach (string par in _crtl.GetParametersOfAssociatedStrategy(assoc.Key).Select(a => a.Key))
                                {
                                    allParamsFromAssociatedStrategies.Add(new KeyValuePair<string, string>(assoc.Value, par));//name of the strategy, name of the param
                                }
                            }
                        }




                        //filter the params by using only the params related to this value of the switch
                        var ss = _crtl.Parameters;//.Where(a => sw.SwitchValueRelatedModelingOptions4SwitchValues[swValue].Parameters.Contains(a.Key));
                        foreach (var par in ss)
                        {

                            /*
                            //if it is a parameter that belongs to a simple strategy associated to this one
                            if (allParamsFromAssociatedStrategies.Select(a => a.Value).Contains(par.Key))
                            {
                                //  VarInfo v0 = new CompositeStrategyVarInfo(_potentialphenologyc, "A");

                                IEnumerable<string> strategyNames = allParamsFromAssociatedStrategies.Where(a => a.Value.Equals(par.Key)).Select(a => a.Key);
                                foreach (string s in strategyNames)
                                {
                                    i++;
                                    codeLines = codeLines + "\r\n\t\t\t\tVarInfo v" + i + " = new CompositeStrategyVarInfo(_" + s + ",\"" + par.Key + "\");";
                                    codeLines = codeLines + "\r\n\t\t\t\t _parameters" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(v" + i + ");";
                                }
                            }
                            //if it is a parameter of this strategy    
                            else
                            {*/
                                i++;
                                codeLines = codeLines + "\r\n\t\t\t\tVarInfo v" + i + " = new VarInfo();";
                                //codeLines = codeLines + "\r\n\t\tVarInfo v"+i+".CurrentValue = "+par.Value.DefaultValue+";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".DefaultValue = " + par.Value.DefaultValue + ";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Description = \"" + par.Value.Description + "\";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Id = " + par.Value.Id + ";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".MaxValue = " + par.Value.MaxValue + ";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".MinValue = " + par.Value.MinValue + ";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Name = \"" + par.Value.Name + "\";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Size = " + par.Value.Size + ";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Units = \"" + par.Value.Units + "\";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".URL = \"" + par.Value.URL + "\";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".VarType = CRA.ModelLayer.Core.VarInfo.Type." + par.Value.VarType + ";";
                                codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".ValueType = VarInfoValueTypes.GetInstanceForName(\"" + par.Value.ValueType.Name + "\");";
                                codeLines = codeLines + "\r\n\t\t\t\t _parameters" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(v" + i + ");";
                         /*   }*/

                        }
                        //params of the associated strategies
                        foreach (var par in allParamsFromAssociatedStrategies) {
                            i++;
                            codeLines = codeLines + "\r\n\t\t\t\tVarInfo v" + i + " = new CompositeStrategyVarInfo(_" + par.Key + ",\"" + par.Value + "\");";
                            codeLines = codeLines + "\r\n\t\t\t\t _parameters" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(v" + i + ");";                     
                        }

                        codeLines += "\r\n\t\t\t\t" + modelingOptionIdentifier + ".Parameters=_parameters" + switchProgrNumber + "_" + switchValueProgrNumber + ";";


                        codeLines += "\r\n\t\t\t\t//Inputs\r\n\t\t\t\tList<PropertyDescription> _inputs" + switchProgrNumber + "_" + switchValueProgrNumber + " = new List<PropertyDescription>();";


                        //filter the inputs by using only the inputs related to this value of the switch
                        var tt = _crtl.Inputs;//.Where(a => sw.SwitchValueRelatedModelingOptions4SwitchValues[swValue].Inputs.Contains(a.Key));
                        foreach (var inp in tt)
                        {
                            j++;
                            codeLines += "\r\n\t\t\t\tPropertyDescription pd" + j + " = new PropertyDescription();";
                            codeLines += "\r\n\t\t\t\tpd" + j + ".DomainClassType = typeof(" + Controller.GetDomainClassTypeFromCmbDomainClass(inp.Value) + ");";
                            codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyName = \"" + inp.Key + "\";";
                            codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyType = (( " + Controller.GetDomainClassTypeFromCmbDomainClass(inp.Value) + "VarInfo." + inp.Key + ")).ValueType.TypeForCurrentValue;";
                            codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyVarInfo =( " + Controller.GetDomainClassTypeFromCmbDomainClass(inp.Value) + "VarInfo." + inp.Key + ");";
                            codeLines += "\r\n\t\t\t\t_inputs" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(pd" + j + ");";
                        }
                        codeLines += "\r\n\t\t\t\t" + modelingOptionIdentifier + ".Inputs=_inputs" + switchProgrNumber + "_" + switchValueProgrNumber + ";";

                        codeLines += "\r\n\t\t\t\t//Outputs\r\n\t\t\t\tList<PropertyDescription> _outputs" + switchProgrNumber + "_" + switchValueProgrNumber + " = new List<PropertyDescription>();";

                        //filter the outputs by using only the outputs related to this value of the switch
                        var rr = _crtl.Outputs;//.Where(a => sw.SwitchValueRelatedModelingOptions4SwitchValues[swValue].Outputs.Contains(a.Key));
                        foreach (var outp in rr)
                        {
                            j++;
                            codeLines += "\r\n\t\t\t\tPropertyDescription pd" + j + " = new PropertyDescription();";
                            codeLines += "\r\n\t\t\t\tpd" + j + ".DomainClassType = typeof(" + Controller.GetDomainClassTypeFromCmbDomainClass(outp.Value) + ");";
                            codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyName = \"" + outp.Key + "\";";
                            codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyType =  (( " + Controller.GetDomainClassTypeFromCmbDomainClass(outp.Value) + "VarInfo." + outp.Key + ")).ValueType.TypeForCurrentValue;";
                            codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyVarInfo =(  " + Controller.GetDomainClassTypeFromCmbDomainClass(outp.Value) + "VarInfo." + outp.Key + ");";
                            codeLines += "\r\n\t\t\t\t_outputs" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(pd" + j + ");";
                        }
                        codeLines += "\r\n\t\t\t\t" + modelingOptionIdentifier + ".Outputs=_outputs" + switchProgrNumber + "_" + switchValueProgrNumber + ";";

                        //associated strategies

                        codeLines += "\r\n\t\t\t\t//Associated strategies";
                        codeLines += "\r\n\t\t\t\tList<string> lAssStrat" + switchProgrNumber + "_" + switchValueProgrNumber + " = new List<string>();";
                        if (_crtl.CompositeStrategy.Equals("checked"))
                        {

                            foreach (var assoc in _crtl.AssociatedStrategies.Keys)
                            {

                                codeLines += "\r\n\t\t\t\tlAssStrat" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(typeof(" + assoc + ").FullName);";
                            }
                        }
                        codeLines += "\r\n\t\t\t\t" + modelingOptionIdentifier + ".AssociatedStrategies = lAssStrat" + switchProgrNumber + "_" + switchValueProgrNumber + ";";
                        codeLines += "\r\n\t\t\t\t//Adding the modeling options to the dictionary of the values of the switch";
                        codeLines += "\r\n\t\t\t\td" + switchProgrNumber + ".Add(\"" + swValue + "\"," + modelingOptionIdentifier + ");\r\n";

                        switchValueProgrNumber++;
                    }
                    codeLines += "\r\n\r\n\t\t\t\t//Creating the switch from the dictionary of the modeling options";
                    codeLines += "\r\n\t\t\t\tSwitch s" + switchProgrNumber + " = new Switch(\"" + sw.SwitchName + "\", \"" + sw.SwitchDescription + "\", d" + switchProgrNumber + ");\r\n";
                    ids += "s" + switchProgrNumber + ",";
                    switchProgrNumber++;
                }
                

                    //switches of the associated strategies of a composite
                     if (_crtl.CompositeStrategy.Equals("checked"))
                        {
                            //collect all the different switches from the associated strategies               
                            foreach (var assoc in _crtl.AssociatedStrategies)
                            {
                                foreach (string sw in _crtl.GetSwitchesOfAssociatedStrategy(assoc.Key))
                                {
                                    codeLines += "\r\n\t\t\t\tSwitch s" + switchProgrNumber + " = new CompositeSwitch(_" + assoc.Value + ", \"" + sw + "\", \"Switch from associated strategy '" + assoc.Value + "'\");";
                                    ids += "s" + switchProgrNumber + ",";
                                    switchProgrNumber++;
                                }
                            }
                        }

                codeLines += "\r\n\r\n\t\t\t\t//Creating the modeling options manager of the strategy";
                codeLines += "\r\n\t\t\t\t_modellingOptionsManager = new ModellingOptionsManager(" + ids.TrimEnd(",".ToCharArray()) + ");";

            }
            else {
               
                int switchValueProgrNumber = 0;
                string modelingOptionIdentifier = "mo" + switchProgrNumber + "_" + switchValueProgrNumber;

                codeLines += "\r\n\t\t\t\tModellingOptions " + modelingOptionIdentifier + " = new ModellingOptions();";
                codeLines += "\r\n\t\t\t\t//Parameters\r\n\t\t\t\tList<VarInfo> _parameters" + switchProgrNumber + "_" + switchValueProgrNumber + " = new List<VarInfo>();";

                List<KeyValuePair<string, string>> allParamsFromAssociatedStrategies = new List<KeyValuePair<string, string>>();
                if (_crtl.CompositeStrategy.Equals("checked"))
                {
                    //collect all the different params from the associated strategies               
                    foreach (var assoc in _crtl.AssociatedStrategies)
                    {
                        foreach (string par in _crtl.GetParametersOfAssociatedStrategy(assoc.Key).Select(a => a.Key))
                        {
                            allParamsFromAssociatedStrategies.Add(new KeyValuePair<string, string>(assoc.Value, par));//name of the strategy, name of the param
                        }
                    }
                }




                
                var ss = _crtl.Parameters;
                foreach (var par in ss)
                {

                    /*
                    //if it is a parameter that belongs to a simple strategy associated to this one
                    if (allParamsFromAssociatedStrategies.Select(a => a.Value).Contains(par.Key))
                    {
                        //  VarInfo v0 = new CompositeStrategyVarInfo(_potentialphenologyc, "A");

                        IEnumerable<string> strategyNames = allParamsFromAssociatedStrategies.Where(a => a.Value.Equals(par.Key)).Select(a => a.Key);
                        foreach (string s in strategyNames)
                        {
                            i++;
                            codeLines = codeLines + "\r\n\t\t\t\tVarInfo v" + i + " = new CompositeStrategyVarInfo(_" + s + ",\"" + par.Key + "\");";
                            codeLines = codeLines + "\r\n\t\t\t\t _parameters" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(v" + i + ");";
                        }
                    }
                    //if it is a parameter of this strategy    
                    else
                    {*/
                        i++;
                        codeLines = codeLines + "\r\n\t\t\t\tVarInfo v" + i + " = new VarInfo();";
                        //codeLines = codeLines + "\r\n\t\tVarInfo v"+i+".CurrentValue = "+par.Value.DefaultValue+";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".DefaultValue = " + par.Value.DefaultValue + ";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Description = \"" + par.Value.Description + "\";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Id = " + par.Value.Id + ";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".MaxValue = " + par.Value.MaxValue + ";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".MinValue = " + par.Value.MinValue + ";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Name = \"" + par.Value.Name + "\";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Size = " + par.Value.Size + ";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".Units = \"" + par.Value.Units + "\";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".URL = \"" + par.Value.URL + "\";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".VarType = CRA.ModelLayer.Core.VarInfo.Type." + par.Value.VarType + ";";
                        codeLines = codeLines + "\r\n\t\t\t\t v" + i + ".ValueType = VarInfoValueTypes.GetInstanceForName(\"" + par.Value.ValueType.Name + "\");";
                        codeLines = codeLines + "\r\n\t\t\t\t _parameters" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(v" + i + ");";
                  /*  }*/

                }

                //params of the associated strategies
                foreach (var par in allParamsFromAssociatedStrategies)
                {
                    i++;
                    codeLines = codeLines + "\r\n\t\t\t\tVarInfo v" + i + " = new CompositeStrategyVarInfo(_" + par.Key + ",\"" + par.Value + "\");";
                    codeLines = codeLines + "\r\n\t\t\t\t _parameters" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(v" + i + ");";
                }

                codeLines += "\r\n\t\t\t\t" + modelingOptionIdentifier + ".Parameters=_parameters" + switchProgrNumber + "_" + switchValueProgrNumber + ";";


                codeLines += "\r\n\t\t\t\t//Inputs\r\n\t\t\t\tList<PropertyDescription> _inputs" + switchProgrNumber + "_" + switchValueProgrNumber + " = new List<PropertyDescription>();";


                //filter the inputs by using only the inputs related to this value of the switch
                var tt = _crtl.Inputs;
                foreach (var inp in tt)
                {
                    j++;
                    codeLines += "\r\n\t\t\t\tPropertyDescription pd" + j + " = new PropertyDescription();";
                    codeLines += "\r\n\t\t\t\tpd" + j + ".DomainClassType = typeof(" + Controller.GetDomainClassTypeFromCmbDomainClass(inp.Value) + ");";
                    codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyName = \"" + inp.Key + "\";";
                    codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyType = (( " + Controller.GetDomainClassTypeFromCmbDomainClass(inp.Value) + "VarInfo." + inp.Key + ")).ValueType.TypeForCurrentValue;";
                    codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyVarInfo =( " + Controller.GetDomainClassTypeFromCmbDomainClass(inp.Value) + "VarInfo." + inp.Key + ");";
                    codeLines += "\r\n\t\t\t\t_inputs" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(pd" + j + ");";
                }
                codeLines += "\r\n\t\t\t\t" + modelingOptionIdentifier + ".Inputs=_inputs" + switchProgrNumber + "_" + switchValueProgrNumber + ";";

                codeLines += "\r\n\t\t\t\t//Outputs\r\n\t\t\t\tList<PropertyDescription> _outputs" + switchProgrNumber + "_" + switchValueProgrNumber + " = new List<PropertyDescription>();";

                //filter the outputs by using only the outputs related to this value of the switch
                var rr = _crtl.Outputs;
                foreach (var outp in rr)
                {
                    j++;
                    codeLines += "\r\n\t\t\t\tPropertyDescription pd" + j + " = new PropertyDescription();";
                    codeLines += "\r\n\t\t\t\tpd" + j + ".DomainClassType = typeof(" + Controller.GetDomainClassTypeFromCmbDomainClass(outp.Value) + ");";
                    codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyName = \"" + outp.Key + "\";";
                    codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyType =  (( " + Controller.GetDomainClassTypeFromCmbDomainClass(outp.Value) + "VarInfo." + outp.Key + ")).ValueType.TypeForCurrentValue;";
                    codeLines += "\r\n\t\t\t\tpd" + j + ".PropertyVarInfo =(  " + Controller.GetDomainClassTypeFromCmbDomainClass(outp.Value) + "VarInfo." + outp.Key + ");";
                    codeLines += "\r\n\t\t\t\t_outputs" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(pd" + j + ");";
                }
                codeLines += "\r\n\t\t\t\t" + modelingOptionIdentifier + ".Outputs=_outputs" + switchProgrNumber + "_" + switchValueProgrNumber + ";";

                //associated strategies

                codeLines += "\r\n\t\t\t\t//Associated strategies";
                codeLines += "\r\n\t\t\t\tList<string> lAssStrat" + switchProgrNumber + "_" + switchValueProgrNumber + " = new List<string>();";
                if (_crtl.CompositeStrategy.Equals("checked"))
                {
                    // davide - prova
                    //foreach (var assoc in _crtl.AssociatedStrategies.Keys)
                    foreach (var assoc in _crtl.OrderedAssociatedStrategies)
                    {

                        codeLines += "\r\n\t\t\t\tlAssStrat" + switchProgrNumber + "_" + switchValueProgrNumber + ".Add(typeof(" + assoc + ").FullName);";
                    }
                }
                codeLines += "\r\n\t\t\t\t" + modelingOptionIdentifier + ".AssociatedStrategies = lAssStrat" + switchProgrNumber + "_" + switchValueProgrNumber + ";";
                codeLines += "\r\n\t\t\t\t//Adding the modeling options to the modeling options manager";

                //switches of the associated strategies of a composite
                if (_crtl.CompositeStrategy.Equals("checked"))
                {
                    int g = 0;
                    string ids2 = "";
                    //collect all the different switches from the associated strategies               
                    foreach (var assoc in _crtl.AssociatedStrategies)
                    {
                        foreach (string sw in _crtl.GetSwitchesOfAssociatedStrategy(assoc.Key))
                        {
                            codeLines += "\r\n\t\t\t\tSwitch s" + g + " = new CompositeSwitch(_" + assoc.Value + ", \"" + sw + "\", \"Switch from associated strategy '" + assoc.Value + "'\" );";
                            ids2 += "s" + g + ",";
                            g++;
                        }
                    }

                    codeLines += "\r\n\r\n\t\t\t\t//Creating the modeling options manager of the strategy";
                    //if in the assoc strategies there were no switches 
                    if (ids2.Equals(""))
                    {
                        codeLines += "\r\n\t\t\t\t_modellingOptionsManager = new ModellingOptionsManager(" + modelingOptionIdentifier + ");";
                    }
                    //if in the assoc strategies there were  switches 
                    else
                    {
                        codeLines += "\r\n\t\t\t\t_modellingOptionsManager = new ModellingOptionsManager(" + modelingOptionIdentifier + "," + ids2.TrimEnd(",".ToCharArray()) + ");";
                    }
                }
                else
                {
                    codeLines += "\r\n\t\t\t\t_modellingOptionsManager = new ModellingOptionsManager(" + modelingOptionIdentifier + ");";
                }
            }

         
            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            return UpdatedCodeText;
        }





    }



}
