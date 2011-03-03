namespace FSharpSpec.TDNetRunner

open System.Reflection
open TestDriven.Framework;

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils

type SpecsRunner() =
  let runSpecs listener (tree : Node) =
    let logger = new TDNetLogger(listener) :> ISpecsResultsLogger
    let printOutput = function | content -> listener.WriteLine(content, Category.Output)

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

  interface ITestRunner with
      member x.RunAssembly(listener : ITestListener , assembly : Assembly) = 
            
          printfn "RunAssembly\n Listener: %A\n Assembly: %A" listener assembly
          
          assembly 
          |> getContextTreeOfAssembly
          |> runSpecs listener

          TestRunState.Success

      member x.RunMember(listener : ITestListener , assembly : Assembly, memberInfo : MemberInfo) : TestRunState =  
            
          printfn "RunMember\n Listener: %A\n Assembly: %A\n MemberInfo: %A Module: %A" listener assembly memberInfo memberInfo.Module
         
          assembly 
          |> getContextTreeOfTypesInAssembly memberInfo
          |> runSpecs listener

          TestRunState.Success

      member x.RunNamespace(listener : ITestListener , assembly : Assembly, nameSpace : string) = 
          printfn "RunNamespace\n Listener: %A\n Assembly: %A\n Namespace: %A" listener assembly nameSpace
          TestRunState.Failure

    

    