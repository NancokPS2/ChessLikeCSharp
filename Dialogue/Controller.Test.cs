using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Dialogue;

public partial class Controller
{
    public class Test : ITest
    {
        public ITest.TestResult TestRun()
        {
            Parser parser = new();
            parser.Advance(60f);
            return ITest.TestResult.SUCCESS;
        }
    }

}
