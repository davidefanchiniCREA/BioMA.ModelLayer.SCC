using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;   


namespace CRA.ModelLayer.SCC
{
    public class Writer
    {
        const int MAX_NUMBER_CUSTOMCODE_AREAS = 20;

        int progressive = 0;

        const string startCustomPlaceholderFirstPart = "//GENERATED CODE END - PLACE YOUR CUSTOM CODE BELOW - Section";
        const string startCustomPlaceholderSecondPart = "\r\n\t\t//Code written below will not be overwritten by a future code generation\r\n\r\n\r\n";
        const string endCustomPlaceholderFirstPart = "\r\n\t\t//End of custom code. Do not place your custom code below. It will be overwritten by a future code generation.\r\n";
        const string endCustomPlaceholderSecondPart = "//PLACE YOUR CUSTOM CODE ABOVE - GENERATED CODE START - Section";

        public void Write(string fileFullName, string newContent)
        {

            if (!File.Exists(fileFullName))
                File.WriteAllText(fileFullName, newContent);
            else
            {
                string oldContent = File.ReadAllText(fileFullName);

                Dictionary<int, string> customCodeAreas = GetCustomCodeAreasFromCode(oldContent);

                string toWrite = ReplaceCustomCodeAreasInCode(newContent, customCodeAreas);

                File.WriteAllText(fileFullName, toWrite);
            }
        }

        private string ReplaceCustomCodeAreasInCode(string newContent, Dictionary<int, string> customCodeAreas)
        {
            string toReturn = newContent;

            for (int i = 1; i < MAX_NUMBER_CUSTOMCODE_AREAS; i++)
            {
                if (customCodeAreas.ContainsKey(i))
                {
                    string findme = startCustomPlaceholderFirstPart + i + "(.*)" + endCustomPlaceholderSecondPart + i;
                    toReturn = Regex.Replace(toReturn, findme, customCodeAreas[i], RegexOptions.Singleline);
                }
            }
            return toReturn;
        }

        private Dictionary<int, string> GetCustomCodeAreasFromCode(string oldContent)
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();

            for (int i = 1; i < MAX_NUMBER_CUSTOMCODE_AREAS; i++)
            {
                string findme = startCustomPlaceholderFirstPart + i + "(.*)" + endCustomPlaceholderSecondPart + i;
                Match m = Regex.Match(oldContent, findme, RegexOptions.Singleline);
                if (m.Success) toReturn.Add(i, m.Value);
            }

            return toReturn;
        }

        public void Initialize()
        {
            progressive = 0;
        }

        public string ProduceCustomCodeArea()
        {
            progressive++;
            return startCustomPlaceholderFirstPart + progressive + startCustomPlaceholderSecondPart + endCustomPlaceholderFirstPart + endCustomPlaceholderSecondPart + progressive;

        }
    }
}
