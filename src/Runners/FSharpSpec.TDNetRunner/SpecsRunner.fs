namespace FSharpSpec.TDNetRunner

open System.Reflection
open TestDriven.Framework;

open FSharpSpec
open SpecsRunnerUtils
open SpecsExtractor

type SpecsRunner() =
    interface ITestRunner with
        member x.RunAssembly(listener : ITestListener , assembly : Assembly) = 
            printfn "RunAssembly\n Listener: %A\n Assembly: %A" listener assembly

            let runSpecs =
                let printError = function | content -> listener.TestFinished(new TestResult(Message = content, State = TestState.Failed ))
                let printOutput = function | content -> listener.WriteLine(content, Category.Info)

                let tree = assembly |> getContextTreeOfAssembly
                let results = tree.RunSpecs() 
           
                printResultTree results printOutput
                printFailureDetails results printError
                printPendingSummary results printOutput
                printFailureSummary results printOutput

            

            runSpecs

            TestRunState.Failure

        member x.RunMember(listener : ITestListener , assembly : Assembly, memberInfo : MemberInfo) : TestRunState =  
            printfn "RunMember\n Listener: %A\n Assembly: %A\n MemberInfo: %A" listener assembly memberInfo
            
            TestRunState.Failure

        member x.RunNamespace(listener : ITestListener , assembly : Assembly, nameSpace : string) = 
            printfn "RunNamespace\n Listener: %A\n Assembly: %A\n Namespace: %A" listener assembly nameSpace
            TestRunState.Failure

    

    