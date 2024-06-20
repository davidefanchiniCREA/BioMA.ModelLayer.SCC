using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRA.ModelLayer.SCC.CodeParts
{
    class CodeModel : ICodePart
    {

        internal CodeModel( Controller crtl)
        {
            _crtl = crtl;
     
        }

  
        private readonly Controller _crtl;

        public string WriteCodeLines(string newCodeText)
        {
            string UpdatedCodeText;

            // Insert here logic
            string codeLines = String.Empty;


            string placeHolder = "(#TypesAndInstances#)";

            // Insert here logic
            codeLines = _crtl.DomainClassTypeAndInstances.Select(a =>  a.Value.FullName + " " + a.Key).Aggregate((a, b) => a = a  + "," + b );
            codeLines = codeLines.TrimEnd(",".ToCharArray());

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, newCodeText, codeLines);



            placeHolder = "(#ModelMethodName#)";

            // Insert here logic
            codeLines = _crtl.ModelMethodName;
            

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            
            


            placeHolder = "(#TypesAndInstancesForPrePostConditions#)";

            // Insert here logic
            codeLines = _crtl.DomainClassTypeAndInstances.Where(
                a => 
                    //typeof(CRA.Core.Preconditions.IDomainClass).IsAssignableFrom(a.Value) || 
                    typeof(CRA.ModelLayer.Core.IDomainClass).IsAssignableFrom(a.Value)).Select(a => a.Value.FullName + " " + a.Key).Aggregate((a, b) => a = a + "," + b);
            codeLines = codeLines.TrimEnd(",".ToCharArray());

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            placeHolder = "(#InstancesOfTypes#)";

            // Insert here logic
            codeLines = _crtl.DomainClassTypeAndInstances.Keys.Aggregate((a,b)=>a=a+","+b);
            codeLines = codeLines.TrimEnd(",".ToCharArray());

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);


            placeHolder = "(#InstancesOfTypesForPrePostConditions#)";

            // Insert here logic
            codeLines = _crtl.DomainClassTypeAndInstances.Where(a => 
                //typeof(CRA.Core.Preconditions.IDomainClass).IsAssignableFrom(a.Value) || 
                typeof(CRA.ModelLayer.Core.IDomainClass).IsAssignableFrom(a.Value)).Select(a=>a.Key).Aggregate((c, b) => c = c + "," + b);
            codeLines = codeLines.TrimEnd(",".ToCharArray());

            // Update property
            UpdatedCodeText = CodeUtilities.ReplaceStrings(placeHolder, UpdatedCodeText, codeLines);

            return UpdatedCodeText;
        }
    }
}
