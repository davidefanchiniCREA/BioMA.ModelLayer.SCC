using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRA.ModelLayer.SCC.CodeParts
{
    internal class CodeIStrategy : ICodePart
    {
        internal CodeIStrategy( Controller crtl)
        {
            _crtl = crtl;
       
        }

            
        private Controller _crtl;

        public string WriteCodeLines(string newCodeText)
        {
            string UpdatedCodeText;
            string codeLines;
            string placeHolder;


            placeHolder = "(#URLOntology#)";

            // Insert here logic  TODO: put link in application configuration?
            codeLines = "http://biomamodelling.org";

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, newCodeText, codeLines);


            placeHolder = "(#Domain#)";

            // Insert here logic
            codeLines = _crtl.Domain;

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            placeHolder = "(#Description#)";

            // Insert here logic
            codeLines = _crtl.StrategyDescription;

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            placeHolder = "(#IsContext#)";

            // Insert here logic
            if (string.IsNullOrEmpty(_crtl.IsContext)) _crtl.IsContext = "false";
            codeLines = _crtl.IsContext;

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            placeHolder = "(#ModelType#)";

            // Insert here logic
            codeLines = _crtl.ModelType;

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            placeHolder = "(#TimeSteps#)";

            // Insert here logic
            codeLines = String.Empty;
            foreach (string time in _crtl.TimeSteps)
            {
                codeLines = codeLines + "ts.Add(" + time + ");\r\n\t\t\t\t";
            }

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);



            
            placeHolder = "(#TypesDomainClasses#)";

            //// Insert here logic
            codeLines = _crtl.DomainClassTypeAndInstances.Values.Select(a=>"typeof("+a.FullName+")").Aggregate((a,b)=>a=a+","+b);
            codeLines = codeLines.TrimEnd(",".ToCharArray());
            
            //// Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            // TODO: implement
            //placeHolder = "(#SetListInputs#)";

            //// Insert here logic
            //codeLines = String.Empty;

            //// Update property
            //UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            // TODO: implement
            //placeHolder = "(#SetListOutputs#)";

            //// Insert here logic
            //codeLines = String.Empty;

            //// Update property
            //UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            // TODO: implement
            //placeHolder = "(#SetListParameters#)";

            //// Insert here logic
            //codeLines = String.Empty;

            //// Update property
            //UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            // TODO: implement
            //placeHolder = "(#SetListAssociatedStrategies#)";

            //// Insert here logic
            //codeLines = String.Empty;

            //// Update property
            //UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            return UpdatedCodeText;
        }
    }
}
