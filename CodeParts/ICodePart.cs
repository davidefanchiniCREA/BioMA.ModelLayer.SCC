using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRA.ModelLayer.SCC.CodeParts
{
    internal interface ICodePart
    {
        string WriteCodeLines(string originalCode);
    }
}
