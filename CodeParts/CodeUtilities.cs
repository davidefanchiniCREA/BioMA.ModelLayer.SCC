using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRA.ModelLayer.SCC.CodeParts
{
    internal static class CodeUtilities 
    {
        internal static string ReplaceStrings(string placeHolder, string codeText, string content)
        {
            return codeText.Replace(placeHolder, content);
            /*old
            string updatedCode = codeText;
            
            try
            {
            
            int lengthP = placeHolder.Length;
            //int lengthD = content.Length;
            int item = 0;
            
            // Find position of all occurrencies
            int counter = 0;
            while (counter < (codeText.Length - lengthP))
            {
                if (counter < updatedCode.Length)
                    item = updatedCode.IndexOf(placeHolder, counter);
                
                if (item > -1)
                {
                    string begin = updatedCode.Substring(0, item);
                    string end = updatedCode.Substring(item + lengthP);
                    updatedCode = begin + content + end;
                }
                counter += lengthP;
            }
            }
            catch (Exception)
            {
                Warnings += "The placeholder " + placeHolder + " could not be replaced.\r\n";
            }

            return updatedCode;
            */
        }

        internal static string Warnings { get; set; }
    }

}
