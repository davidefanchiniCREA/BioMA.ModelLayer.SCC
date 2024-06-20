using CRA.ModelLayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CRA.ModelLayer.SCC.CodeParts
{
    class CodeComposite : ICodePart
    {

        internal CodeComposite(Controller crtl)
        {
            _crtl = crtl;

        }


        private Controller _crtl;

        public string WriteCodeLines(string newCodeText)
        {

            string UpdatedCodeText;

            // Insert here logic
            string placeHolder = "(#Composite#)";

            // Insert here logic
            string codeLines;
            codeLines = "";

            if (_crtl.CompositeStrategy.Equals("checked"))
            {

                codeLines += "\r\n\t\t\t#region Composite class: associations\r\n\r\n\t\t\t//Declaration of the associated strategies";
                // davide - prova
                //foreach (var assoc in _crtl.AssociatedStrategies)
                foreach (var assoc in _crtl.OrderedAssociatedStrategies)
                {

                    //codeLines += "\r\n\t\t\t" + assoc.Key + " _" + assoc.Value + " = new " + assoc.Key + "();";
                    codeLines += "\r\n\t\t\t" + assoc + " _" + _crtl.AssociatedStrategies[assoc] + " = new " + assoc + "();";
                }

                string s = _crtl.DomainClassTypeAndInstances.Select(a => a.Value.FullName + " " + a.Key).Aggregate((a, b) => a = a + "," + b);
                s = s.TrimEnd(",".ToCharArray());
                string t = _crtl.DomainClassTypeAndInstances.Keys.Aggregate((a, b) => a = a + "," + b);
                t = t.TrimEnd(",".ToCharArray());

                codeLines += "\r\n\r\n\t\t\t//Call of the associated strategies\r\n\t\t\tprivate void EstimateOfAssociatedClasses(" + s + "){";
                // davide - prova
                //foreach (var assoc in _crtl.AssociatedStrategies)
                foreach (var assoc in _crtl.OrderedAssociatedStrategies)
                {

                    //codeLines += "\r\n\t\t\t\t_" + assoc.Value + "."+_crtl.ModelMethodName+"(" + t + ");";
                    codeLines += "\r\n\t\t\t\t_" + _crtl.AssociatedStrategies[assoc] + "." + _crtl.ModelMethodName + "(" + t + ");";
                }
                codeLines += "\r\n\t\t\t}";
                codeLines += "\r\n\r\n\t\t\t#endregion\r\n";

            }


            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, newCodeText, codeLines);

            placeHolder = "(#CompositeCall#)";
            codeLines = "";
            if (_crtl.CompositeStrategy.Equals("checked"))
            {
                string t = _crtl.DomainClassTypeAndInstances.Keys.Aggregate((a, b) => a = a + "," + b);
                t = t.TrimEnd(",".ToCharArray());
                codeLines += "\r\n\t\t\t\t\tEstimateOfAssociatedClasses(" + t + ");";
            }

            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);





            placeHolder = "(#CompositePreConditions#)";
            codeLines = "";
            if (_crtl.CompositeStrategy.Equals("checked"))
            {
                string t = _crtl.DomainClassTypeAndInstances.Where(a => 
                    //typeof(CRA.Core.Preconditions.IDomainClass).IsAssignableFrom(a.Value) || 
                    typeof(CRA.ModelLayer.Core.IDomainClass).IsAssignableFrom(a.Value)).Select(a => a.Key).Aggregate((c, b) => c = c + "," + b);
                t = t.TrimEnd(",".ToCharArray());
                codeLines += "\r\n\t\t\t\t\tstring ret = \"\";";
                foreach (var inst in _crtl.AssociatedStrategies)
                {
                    codeLines += "\r\n\t\t\t\t\t ret += _" + inst.Value + ".TestPreConditions(" + t + ", \"strategy " + inst.Key + "\");";
                }
                codeLines += "\r\n\t\t\t\t\tif (ret != \"\") { pre.TestsOut(PreconditionsWriter, ret, true, \"   preconditions tests of associated classes\"); }";

            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);





            // Preconditions on Parameters Composite
            placeHolder = "(#CompositePreConditionsParameters#)";
            codeLines = "";
            if (_crtl.CompositeStrategy.Equals("checked"))
            {

                codeLines += "\r\n\t\t\t\t\tstring ret = \"\";";
                foreach (var inst in _crtl.AssociatedStrategies)
                {
                    codeLines += "\r\n\t\t\t\t\t ret += _" + inst.Value + ".TestPreconditionsOnParameters(\"strategy " + inst.Key + "\");";
                }
                codeLines += "\r\n\t\t\t\t\tif (ret != \"\") { pre.TestsOut(PreconditionsWriter, ret, true, \"   preconditions tests of associated classes on Parameters\"); }";
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            placeHolder = "(#CompositePostConditions#)";
            codeLines = "";
            if (_crtl.CompositeStrategy.Equals("checked"))
            {
                string t = _crtl.DomainClassTypeAndInstances.Where(a => 
                    //typeof(CRA.Core.Preconditions.IDomainClass).IsAssignableFrom(a.Value) || 
                    typeof(CRA.ModelLayer.Core.IDomainClass).IsAssignableFrom(a.Value)).Select(a => a.Key).Aggregate((c, b) => c = c + "," + b);
                t = t.TrimEnd(",".ToCharArray());
                codeLines += "\r\n\t\t\t\t\tstring ret = \"\";";
                foreach (var inst in _crtl.AssociatedStrategies)
                {
                    codeLines += "\r\n\t\t\t\t\t ret += _" + inst.Value + ".TestPostConditions(" + t + ", \"strategy " + inst.Key + "\");";
                }
                codeLines += "\r\n\t\t\t\t\tif (ret != \"\") { pre.TestsOut(PreconditionsWriter, ret, true, \"   postconditions tests of associated classes\"); }";

            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);




            placeHolder = "(#SetDefaultValuesParametersComposite#)";
            codeLines = "";
            if (_crtl.CompositeStrategy.Equals("checked"))
            {
                foreach (var inst in _crtl.AssociatedStrategies)
                {
                    codeLines += "\r\n\t\t\t\t\t_" + inst.Value + ".SetParametersDefaultValue();";
                }

            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            placeHolder = "(#ParametersVarInfoComposite#)";

            //// Insert here logic
            codeLines = "";
            if (_crtl.CompositeStrategy.Equals("checked"))
            {
                List<string> alreadyDoneParams = new List<string>();
                foreach (var assoc in _crtl.AssociatedStrategies)
                {
                    foreach (KeyValuePair<string, VarInfo> par in _crtl.GetParametersOfAssociatedStrategy(assoc.Key))
                    {
                        //insert each parameter only one time (the same param could belong to many different associated strategies)
                        if (!alreadyDoneParams.Contains(par.Key))
                        {
                            alreadyDoneParams.Add(par.Key);
                            codeLines = codeLines + "\r\n\t\t\t\t/// <summary> \r\n\t\t\t\t///" + par.Key + " VarInfo definition\r\n\t\t\t\t/// </summary>";
                            codeLines = codeLines + "\r\n\t\t\t\tpublic static VarInfo " + par.Key + "VarInfo";
                            codeLines = codeLines + "\r\n\t\t\t\t{";
                            codeLines = codeLines + "\r\n\t\t\t\t\tget { return " + assoc.Key + "." + par.Key + "VarInfo; }";
                            codeLines = codeLines + "\r\n\t\t\t\t}";
                        }
                    }
                }
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            placeHolder = "(#ParametersInstancesComposite#)";
            codeLines = "";
            if (_crtl.CompositeStrategy.Equals("checked"))
            {
                //collect all the different params from the associated strategies
                List<KeyValuePair<string, VarInfo>> allParamsFromAssociatedStrategies = new List<KeyValuePair<string, VarInfo>>();
                foreach (var assoc in _crtl.AssociatedStrategies)
                {
                    foreach (KeyValuePair<string, VarInfo> par in _crtl.GetParametersOfAssociatedStrategy(assoc.Key))
                    {
                        allParamsFromAssociatedStrategies.Add(par);
                    }
                }


                //get the distinct params from the associated strategies (one param must be present in more than one associated strategies)
                string[] allDistinctParamsFromAssociatedStrategies = allParamsFromAssociatedStrategies.Select(a => a.Key).Distinct().ToArray();

                //for each parameters
                foreach (string paramName in allDistinctParamsFromAssociatedStrategies)
                {
                   
                    KeyValuePair<string,string>[] associatedStrategiesHavingAtLeastOneParam= _crtl.AssociatedStrategies.Where(a=>_crtl.GetParametersOfAssociatedStrategy(a.Key).ContainsKey(paramName)).ToArray();

                    for (int i = 0; i < associatedStrategiesHavingAtLeastOneParam.Length;i++ )
                    {
                        var assocStrategy = associatedStrategiesHavingAtLeastOneParam[i];
                        //get the associated class(es)  which it belongs to
                        //the getter uses one of the assoc strategies the parameter belongs to
                        if (i==0)
                        {
                            //param property declaration
                            //#LE#:2016-07-11 - fix della componente
                            //codeLines = codeLines + "\r\n\t\t\tpublic " + _crtl.GetParametersOfAssociatedStrategy(assocStrategy.Key)[paramName].ValueType.TypeForCurrentValue + " " + paramName + "";
                            //codeLines = codeLines + "\r\n\t\t\tpublic " + _crtl.GetParametersOfAssociatedStrategy(assocStrategy.Key)[paramName].ValueType.Label + " " + paramName + "";
                            codeLines = codeLines + "\r\n\t\t\tpublic " + GetFullName(_crtl.GetParametersOfAssociatedStrategy(assocStrategy.Key)[paramName].ValueType.TypeForCurrentValue) + " " + paramName + "";

                            //get
                            codeLines = codeLines + "\r\n\t\t\t{ \r\n\t\t\t\tget {";
                            codeLines = codeLines + "\r\n\t\t\t\t\t\treturn _" + assocStrategy.Value + "." + paramName + " ;";
                            codeLines = codeLines + "\r\n\t\t\t\t}";
                            //set
                            codeLines = codeLines + "\r\n\t\t\t\tset {";
                        }
                        //the setter must set the value to the parameter in all the strategies the parameter belongs to                            
                        codeLines = codeLines + "\r\n\t\t\t\t\t\t_" + assocStrategy.Value + "." + paramName + "=value;";
                        if (i == associatedStrategiesHavingAtLeastOneParam.Length-1)
                        {
                            codeLines = codeLines + "\r\n\t\t\t\t}\r\n\t\t\t}";
                        }

                    }
                }
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            return UpdatedCodeText;
        }

        //#LE#: mutuata da una analoga in CodeParameters
        static string GetFullName(Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            StringBuilder sb = new StringBuilder();

            sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
            sb.Append(t.GetGenericArguments().Aggregate("<",

                delegate(string aggregate, Type type)
                {
                    return aggregate + (aggregate == "<" ? "" : ",") + GetFullName(type);
                }
                ));
            sb.Append(">");

            return sb.ToString();
        }
        //#region Composite class: associations
        // // Instances of all classes needed to generate the inputs used by PotentialPhenologyC
        // PotentialPhenologyC f = new PotentialPhenologyC();
        // PanicleHeight h = new PanicleHeight();
        // RUEactualC rue = new RUEactualC();
        // InterceptedAbsorbedRadiation irad = new InterceptedAbsorbedRadiation();
        // PotentialWaterUptake pwu = new PotentialWaterUptake();
        // PotentialTranspiration pt = new PotentialTranspiration();
        // RUEbasedBiomassAccumulation ragb = new RUEbasedBiomassAccumulation();
        // PartitioningWARM p = new PartitioningWARM();
        // SpecificLeafAreaWARM sla = new SpecificLeafAreaWARM();
        // LeafLife ll = new LeafLife();
        // RootDepth rd = new RootDepth();

        // private void EstimateOfAssociatedClasses(Rates r, States s1, Auxiliary a, States s, Exogenous ex, RatesExternal re, StatesExternal se, ActEvents ae)
        // {
        //     f.Update(r, s1, a, s, ex, re, se, ae);
        //     h.Update(r, s1, a, s, ex, re, se, ae);
        //     rue.Update(r, s1, a, s, ex, re, se, ae);
        //     irad.Update(r, s1, a, s, ex, re, se, ae);
        //     pwu.Update(r, s1, a, s, ex, re, se, ae);
        //     pt.Update(r, s1, a, s, ex, re, se, ae);
        //     ragb.Update(r, s1, a, s, ex, re, se, ae);
        //     p.Update(r, s1, a, s, ex, re, se, ae);
        //     sla.Update(r, s1, a, s, ex, re, se, ae);
        //     ll.Update(r, s1, a, s, ex, re, se, ae);
        //     rd.Update(r, s1, a, s, ex, re, se, ae);
        // }
        // #endregion


    }
}
