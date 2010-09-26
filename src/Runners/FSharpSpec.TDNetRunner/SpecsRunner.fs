namespace FSharpSpec.TDNetRunner

open System.Reflection
open TestDriven.Framework;

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils

type SpecsRunner() =
    interface ITestRunner with
        member x.RunAssembly(listener : ITestListener , assembly : Assembly) = 
            
            let logger = new TDNetLogger(listener) :> ISpecsResultsLogger
            
            printfn "RunAssembly\n Listener: %A\n Assembly: %A" listener assembly

            let runSpecs =
                let printOutput = function | content -> listener.WriteLine(content, Category.Info)

                let tree = assembly |> getContextTreeOfAssembly
                let results = tree.RunSpecs() 
           
                printResultTree results printOutput
               
                results
                |> List.iter(fun (msg, passes, failures, pendings) -> 
                    passes    |> List.iter(fun passedSpec -> logger.SpecPassed passedSpec)
                    failures  |> List.iter(fun failedSpec -> 
                                    logger.SpecFailed failedSpec.FullSpecName (
                                        getFailureMessage failedSpec) (getFailureStackTrace failedSpec))
                    pendings  |> List.iter(fun pendingSpec -> logger.SpecPending pendingSpec))  

                printPendingSummary results printOutput
                printFailureSummary results printOutput
            
            runSpecs

            TestRunState.Success

        member x.RunMember(listener : ITestListener , assembly : Assembly, memberInfo : MemberInfo) : TestRunState =  
            printfn "RunMember\n Listener: %A\n Assembly: %A\n MemberInfo: %A" listener assembly memberInfo
            
            TestRunState.Failure

        member x.RunNamespace(listener : ITestListener , assembly : Assembly, nameSpace : string) = 
            printfn "RunNamespace\n Listener: %A\n Assembly: %A\n Namespace: %A" listener assembly nameSpace
            TestRunState.Failure

    

    