namespace FSharpSpec.TDNetRunner

open FSharpSpec.RunnerUtils
open TestDriven.Framework

type TDNetLogger (listener : ITestListener) =
    let _listener = listener
    
    interface ISpecsResultsLogger with
        member x.SpecPassed specName =
            let testResult = new TestResult (Name = specName, State = TestState.Passed, TotalTests = 1)
            _listener.TestFinished testResult
      
        member x.SpecIgnored specName =
            let testResult = new TestResult (Name = specName, State = TestState.Ignored, TotalTests = 1)
            _listener.TestFinished testResult
       
        member x.SpecFailed specName message stackTrace =
            let testResult = 
                new TestResult (
                    Name = specName, 
                    State = TestState.Failed, 
                    TotalTests = 1,
                    Message = message,
                    StackTrace = stackTrace)
            _listener.TestFinished testResult