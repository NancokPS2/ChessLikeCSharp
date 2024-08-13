using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface ITest
{
    public enum TestResult
    {
        SUCCESS,
        FAILURE
    }
    public static List<ITest> global_test_targets = new();

    /// <summary>
    /// Runs a test.
    /// </summary>
    /// <returns>The success status.</returns>
    public TestResult TestRun();

    public Dictionary<ITest, TestResult> TestRunAllRegistered()
    {
        Dictionary<ITest, TestResult> output = new();
        foreach (ITest testee in global_test_targets)
        {
            output[testee] = testee.TestRun();
        }
        return output;
    }

    public void TestRegisterThis()
    {
        global_test_targets.Add(this);
    }

}
