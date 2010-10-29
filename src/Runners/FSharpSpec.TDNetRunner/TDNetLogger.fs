namespace FSharpSpec.TDNetRunner

open FSharpSpec.RunnerUtils
open TestDriven.Framework

type TDNetLogger (listener : ITestListener) =
    let _listener = listener
   
    let testRunner = "FSharpSpec"
    
    interface ISpecsResultsLogger with
        member x.SpecPassed specName =
            let testResult = 
                new TestResult (
                    Name = specName, 
                    State = TestState.Passed, 
                    TotalTests = 1, 
                    TestRunnerName = testRunner)
            
            _listener.TestFinished testResult
      
        member x.SpecPending specName =
            let testResult = 
                new TestResult (
                    Name = specName, 
                    State = TestState.Ignored, 
                    TotalTests = 1, 
                    TestRunnerName = testRunner)
            
            _listener.TestFinished testResult
       
        member x.SpecFailed specName message stackTrace =
            let testResult = 
                new TestResult (
                    Name = specName, 
                    State = TestState.Failed, 
                    TotalTests = 1,
                    Message = message,
                    StackTrace = stackTrace,
                    TestRunnerName = testRunner)
            
            _listener.TestFinished testResult