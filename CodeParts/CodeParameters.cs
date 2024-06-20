using CRA.ModelLayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CRA.ModelLayer.SCC.CodeParts
{
    internal class CodeParameters : ICodePart
    {
        internal CodeParameters( Controller crtl)
        {
            _crtl = crtl;
      
        }

        
        private Controller _crtl;

        public string WriteCodeLines(string newCodeText)
        {

            string UpdatedCodeText;
            string placeHolder = "(#SetVarInfo#)";            
            string codeLines;
            string oneCodeLine = string.Empty;

            foreach (string par in _crtl.Parameters.Keys)
            {
                oneCodeLine = oneCodeLine +
                    _crtl.Parameters[par].Name + "VarInfo.Name = \"" + _crtl.Parameters[par].Name + "\";\r\n\t\t\t\t" +
                    _crtl.Parameters[par].Name + "VarInfo.Description =\" " + _crtl.Parameters[par].Description + "\";\r\n\t\t\t\t" +
                    _crtl.Parameters[par].Name + "VarInfo.MaxValue = " + _crtl.Parameters[par].MaxValue + ";\r\n\t\t\t\t" +
                    _crtl.Parameters[par].Name + "VarInfo.MinValue = " + _crtl.Parameters[par].MinValue + ";\r\n\t\t\t\t" +
                    _crtl.Parameters[par].Name + "VarInfo.DefaultValue = " + _crtl.Parameters[par].DefaultValue + ";\r\n\t\t\t\t" +
                    _crtl.Parameters[par].Name + "VarInfo.Units = \"" + _crtl.Parameters[par].Units + "\";\r\n\t\t\t\t" +
                    _crtl.Parameters[par].Name + "VarInfo.ValueType = " + "CRA.ModelLayer.Core.VarInfoValueTypes.GetInstanceForName(\"" + _crtl.Parameters[par].ValueType.Name + "\");\r\n\r\n\t\t\t\t";
            }
            codeLines = oneCodeLine + "\r\n";

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, newCodeText, codeLines);

            
            
            
            //(#SetDefaultValuesParameters#)
            //example       _modellingOptionsManager.Parameters.Where(a => a.Name.Equals("maximumTemperatureForGrowth")).CurrentValue = _maximumTemperatureForGrowthVarInfo.DefaultValue;           
             placeHolder = "(#SetDefaultValuesParameters#)";

            //// Insert here logic
             codeLines = "";
            foreach (var par in _crtl.Parameters)
            {
                if(par.Value.ValueType.TypeForCurrentValue.Equals(typeof(Double)))codeLines = codeLines + "\r\n\t\t\t\t _modellingOptionsManager.Parameters.Where(a => a.Name.Equals(\"" + par.Key + "\")).ToArray().FirstOrDefault().CurrentValue = " + par.Key + "VarInfo.DefaultValue;";
                if (par.Value.ValueType.TypeForCurrentValue.Equals(typeof(Int32))) codeLines = codeLines + "\r\n\t\t\t\t _modellingOptionsManager.Parameters.Where(a => a.Name.Equals(\"" + par.Key + "\")).ToArray().FirstOrDefault().CurrentValue = Convert.ToDouble( " + par.Key + "VarInfo.DefaultValue);";
            }

            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            //(#ParametersProperties#)
            //Example
            // private static VarInfo _maximumTemperatureForGrowthVarInfo = new VarInfo();
            ///// <summary>
            ///// MaximumTemperatureForGrowth VarInfo
            ///// </summary>
            //public static VarInfo MaximumTemperatureForGrowthVarInfo
            //{
            //    get { return _maximumTemperatureForGrowthVarInfo; }
            //}
            placeHolder = "(#ParametersVarInfo#)";

            //// Insert here logic
            codeLines = "";
            foreach (var par in _crtl.Parameters)
            {
                codeLines = codeLines + "\r\n\t\t\t\tprivate static VarInfo _" + par.Key + "VarInfo= new VarInfo();";
                codeLines = codeLines + "\r\n\t\t\t\t/// <summary> \r\n\t\t\t\t///" + par.Key + " VarInfo definition\r\n\t\t\t\t/// </summary>";
                codeLines = codeLines + "\r\n\t\t\t\tpublic static VarInfo " + par.Key + "VarInfo";
                codeLines = codeLines + "\r\n\t\t\t\t{";
                codeLines = codeLines + "\r\n\t\t\t\t\tget { return _" + par.Key + "VarInfo; }";
                codeLines = codeLines + "\r\n\t\t\t\t}";
            }





            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);




            //Example
            //public System.Double OptimumTemperatureForGrowth
            //{
            //    get
            //    {
            //        VarInfo vi = _modellingOptionsManager.GetParameterByName("OptimumTemperatureForGrowth");
            //        if (vi != null ) return (System.Double)vi.CurrentValue;
            //        else throw new Exception("Parameter 'OptimumTemperatureForGrowth' not found in strategy 'RGRTRYanHunt'");
            //    }
            //    set
            //    {
            //        VarInfo vi = _modellingOptionsManager.GetParameterByName("OptimumTemperatureForGrowth");
            //        if (vi != null) vi.CurrentValue = value;
            //        else throw new Exception("Parameter 'OptimumTemperatureForGrowth' not found in strategy 'RGRTRYanHunt'");
            //    }
            //}

            placeHolder = "(#ParametersInstances#)";
            codeLines = "";
            
            foreach (KeyValuePair<string,VarInfo> par in _crtl.Parameters)
            {
                codeLines = codeLines + "\r\n\t\t\tpublic " + GetFullName(par.Value.ValueType.TypeForCurrentValue) + " " + par.Key + "";
                codeLines = codeLines + "\r\n\t\t\t{ \r\n\t\t\t\tget {";
                codeLines = codeLines + "\r\n\t\t\t\t\t\tVarInfo vi= _modellingOptionsManager.GetParameterByName(\"" + par.Key + "\");";
                codeLines = codeLines + "\r\n\t\t\t\t\t\tif (vi != null && vi.CurrentValue!=null) return (" + GetFullName(par.Value.ValueType.TypeForCurrentValue) + ")vi.CurrentValue ;";
                codeLines = codeLines + "\r\n\t\t\t\t\t\telse throw new Exception(\"Parameter '" + par.Key + "' not found (or found null) in strategy '" + _crtl.StrategyName + "'\");";
                codeLines = codeLines + "\r\n\t\t\t\t } set {";
                codeLines = codeLines + "\r\n\t\t\t\t\t\t\tVarInfo vi = _modellingOptionsManager.GetParameterByName(\"" + par.Key + "\");";
                codeLines = codeLines + "\r\n\t\t\t\t\t\t\tif (vi != null)  vi.CurrentValue=value;";
                codeLines = codeLines + "\r\n\t\t\t\t\t\telse throw new Exception(\"Parameter '" + par.Key + "' not found in strategy '" + _crtl.StrategyName + "'\");";
                codeLines = codeLines + "\r\n\t\t\t\t}\r\n\t\t\t}";
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            return UpdatedCodeText;
        }

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
    }


}
