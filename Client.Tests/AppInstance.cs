using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.White;
using TestStack.White.Configuration;

namespace Client.Tests
{
    public static class TestApp
    {
        private static string EXE_LOCATION = @"E:\Ram\Git\Reaction_FoxitVersion\Reactions-NG\Client\bin\Debug\Reactions.exe";

        public static Application GetInstance()
        {
            return Application.Launch(EXE_LOCATION);
        }
    }
}
