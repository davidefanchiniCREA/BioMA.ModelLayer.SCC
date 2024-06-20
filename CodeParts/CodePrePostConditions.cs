using CRA.ModelLayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CRA.ModelLayer.SCC.CodeParts
{
    class CodePrePostConditions : ICodePart
    {
        internal CodePrePostConditions( Controller crtl)
        {
            _crtl = crtl;
       
        }

        internal string UpdatedCodeText { get; set; }
        private Controller _crtl;

        public string WriteCodeLines(string newCodeText)
        {

            string UpdatedCodeText;
            //preconditions on Inputs

            //example
            //prc.AddCondition(new RangeBasedCondition(UNIMI_CRA.Soil.PTF.PTFDataVarInfo.Clay));//inpus
            //prc.AddCondition(new RangeBasedCondition(_optimumTemperatureForGrowthVarInfo));//param
            
            string placeHolder = "(#PreConditionsRangeBasedInputs#)";
            string codeLines = "";
            int i = 0;
            foreach (var par in _crtl.Inputs)
            {
                i++;
                codeLines = codeLines + "\r\n\t\t\t\t\tRangeBasedCondition r"+i+" = new RangeBasedCondition(" + Controller.GetDomainClassTypeFromCmbDomainClass(par.Value) + "VarInfo." + par.Key + ");";
                codeLines = codeLines + "\r\n\t\t\t\t\tif(r" + i + ".ApplicableVarInfoValueTypes.Contains( " + Controller.GetDomainClassTypeFromCmbDomainClass(par.Value) + "VarInfo." + par.Key + ".ValueType)){prc.AddCondition(r" + i + ");}";
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, newCodeText, codeLines);


            // preconditions on Parameters
            RangeBasedCondition r = new RangeBasedCondition(new VarInfo());
            placeHolder = "(#PreConditionsRangeBasedParameters#)";
            codeLines = "";
            foreach (var par in _crtl.Parameters)
            {
                if (r.ApplicableVarInfoValueTypes.Contains(par.Value.ValueType)) codeLines = codeLines + "\r\n\t\t\t\t\tprc.AddCondition(new RangeBasedCondition(_modellingOptionsManager.GetParameterByName(\"" + par.Key + "\")" + "));";
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            //postcondition

            placeHolder = "(#PostConditionsRangeBased#)";
            codeLines = "";
            foreach (var par in _crtl.Outputs)
            {
                i++;
                codeLines = codeLines + "\r\n\t\t\t\t\tRangeBasedCondition r" + i + " = new RangeBasedCondition(" + Controller.GetDomainClassTypeFromCmbDomainClass(par.Value) + "VarInfo." + par.Key + ");";
                codeLines = codeLines + "\r\n\t\t\t\t\tif(r" + i + ".ApplicableVarInfoValueTypes.Contains( " + Controller.GetDomainClassTypeFromCmbDomainClass(par.Value) + "VarInfo." + par.Key + ".ValueType)){prc.AddCondition(r" + i + ");}";
            }
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            return UpdatedCodeText;

        }

    }
}
