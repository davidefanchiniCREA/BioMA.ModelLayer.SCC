using System;
using System.Linq;

namespace CRA.ModelLayer.SCC.CodeParts
{
    internal class CodeDirectives : ICodePart
    {
        internal CodeDirectives( Controller crtl)
        {
            _crtl = crtl;
        }
     
        private readonly Controller _crtl;

        public string WriteCodeLines(string newCodeText)
        {
            string placeHolder = "(#Directives#)";
            
            // Insert here logic
            string codeLines="";
            foreach (var namespaceOfDomainClass in _crtl.DomainClassTypeAndInstances.Values.Select(a=>a.Namespace).Distinct())
            {
                codeLines += "\r\nusing " + namespaceOfDomainClass + ";";
            }

            codeLines += "\r\n\r\n";
            foreach (var asse in _crtl.LoadedAssemblies)
            {
                codeLines += "\r\n//To make this project compile please add the reference to assembly: " + asse + "";
            }
            
            // Update property
            string _updatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, newCodeText, codeLines);

            placeHolder = "(#CurrentDate#)";

            // Insert here logic
            codeLines = DateTime.Now.ToShortDateString();

            // Update property
            _updatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, _updatedCodeText, codeLines);

            placeHolder = "(#InterfaceIStrategyComponent#)";

            // Insert here logic
            codeLines = _crtl.IStrategyComponentName;

            // Update property
            _updatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, _updatedCodeText, codeLines);

            return _updatedCodeText;
        }
    }
}
 