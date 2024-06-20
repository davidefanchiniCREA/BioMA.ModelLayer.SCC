using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CRA.ModelLayer.SCC.CodeParts
{
    internal class CodeSetCurrentValues : ICodePart
    {
        internal CodeSetCurrentValues( Controller crtl)
        {
            _crtl = crtl;
     
        }

     
        private Controller _crtl;

        public string WriteCodeLines(string newCodeText)
        {
            string UpdatedCodeText;


            ///inputs

            //example:  ExogenousVarInfo.SoilTemperatureLayer1.CurrentValue = ex.SoilTemperatureLayer1;
            string placeHolder = "(#SetCurrentValuesInputs#)";
            string codeLines = "";
            foreach (var par in _crtl.Inputs)
            {
                string domainClassInstance = _crtl.DomainClassTypeAndInstances.Where(a => a.Value.FullName.Equals(Controller.GetDomainClassTypeFromCmbDomainClass(par.Value))).Select(a => a.Key).FirstOrDefault();
                if (domainClassInstance != null)
                {
                    codeLines = codeLines + "\r\n\t\t\t\t\t" + Controller.GetDomainClassTypeFromCmbDomainClass(par.Value) + "VarInfo." + par.Key + ".CurrentValue=" + domainClassInstance + "." + par.Key + ";";
                }
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, newCodeText, codeLines);


            ///OLD non serve piu
            /////Parameters
            
            ////example:  Var1VarInfo.CurrentValue = _modellingOptionsManager.Parameters.Where(a => a.Name.Equals("Var1"));
            //string placeHolder = "(#SetCurrentValuesParameters#)";
            //string codeLines = "";
            //foreach (var par in _crtl.Parameters)
            //{
            //    codeLines = codeLines + "\r\n\t\t\t\t\t" + par.Key + "VarInfo.CurrentValue = _modellingOptionsManager.Parameters.Where(a => a.Name.Equals(\"" + par.Key + "\")).Select(a=> { if (a != null) return a.CurrentValue; else return null;}).FirstOrDefault();";
            //}
            //UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


         

            //outputs

            //example:  ExogenousVarInfo.SoilTemperatureLayer1.CurrentValue = ex.SoilTemperatureLayer1;
            placeHolder = "(#SetCurrentValuesOutputs#)";
            codeLines = "";
            foreach (var par in _crtl.Outputs)
            {
                string domainClassInstance = _crtl.DomainClassTypeAndInstances.Where(a => a.Value.FullName.Equals(Controller.GetDomainClassTypeFromCmbDomainClass( par.Value))).Select(a => a.Key).FirstOrDefault();
                if (domainClassInstance != null)
                {
                    codeLines = codeLines + "\r\n\t\t\t\t\t" + Controller.GetDomainClassTypeFromCmbDomainClass( par.Value )+ "VarInfo." + par.Key + ".CurrentValue=" + domainClassInstance + "." + par.Key + ";";
                }
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            return UpdatedCodeText;
        }

    }
}
