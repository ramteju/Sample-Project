using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Client.Tests
{
    public class ReadableFact : FactAttribute
    {
        public ReadableFact(string charsToReplace = "_", string replacementChars = " ", [CallerMemberName] string testMethodName = "")
        {
            if (charsToReplace != null)
            {
                base.DisplayName = String.Concat(testMethodName.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' '); 
            }
        }
    }
}
