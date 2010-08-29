namespace FSharpSpec.Runner

open System
open System.Collections.Generic
open System.Linq
open System.Reflection
open System.Diagnostics
open SpecsExtractor
open FSharpSpec

module main = 
     
    [<EntryPoint>]
    let main(args:string[]) =
        
        let printResultTree results =
          results
          |> List.iter (fun r -> Debug.WriteLine((fst r)))
          results

        let printSingleFailureSummary failure =
            let getSpecFailedException (ex : Exception) =
                let innerEx = ex.InnerException
                let assertionEx = 
                    match innerEx.GetType() with
                    | ty when ty = typeof<SpecFailedException> -> Some(innerEx :?> SpecFailedException)
                    | _                                        ->  None
                assertionEx
            Debug.WriteLine("\n" + failure.FullSpecName)           
            let ex = failure.Exception
            let specFailedException = ex |> getSpecFailedException
            if specFailedException.IsSome then
                Debug.WriteLine("\t\t" + specFailedException.Value.Data0)
            else
                Debug.WriteLine("\t\t" + ex.InnerException.Message)

        let printFailureDetails results = 
            Debug.WriteLine("\n------------------------ Failure Details -------------------------------------\n")
            for result in results do
                let failures = snd result     
                failures 
                |> List.iter (fun failure -> 
                                    printSingleFailureSummary failure
                                    Debug.WriteLine("\n" + failure.Exception.ToString() + "\n\n" ))
            results

        let printFailureSummary results =
            Debug.WriteLine("\n------------------------ Failure Summary -------------------------------------\n")
            for result in results do
               let failures = snd result     
               failures 
               |> List.iter printSingleFailureSummary
            results
        
        let specsPath = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"
        let tree = specsPath |> getContextTree

        tree.RunSpecs() 
        |> printResultTree
        |> printFailureDetails
        |> printFailureSummary
        |> ignore
        
        0


