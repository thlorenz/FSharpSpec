namespace FSharpSpec.Runner

open System
open System.Collections.Generic
open System.Linq
open System.Reflection
open System.Diagnostics
open System.Text
open SpecsExtractor
open FSharpSpec

module main = 
     
    [<EntryPoint>]
    let main(args:string[]) =
        
        let printResultTree results =
          results
          |> List.iter (fun r -> Debug.WriteLine((fst r)))
          results

        let getSingleFailureSummary failure =
            let sb = new StringBuilder()
            let getSpecFailedException (ex : Exception) =
                let innerEx = ex.InnerException
                let assertionEx = 
                    match innerEx.GetType() with
                    | ty when ty = typeof<SpecFailedException> -> Some(innerEx :?> SpecFailedException)
                    | _                                        ->  None
                assertionEx
            sb.AppendLine()
              .AppendLine(failure.FullSpecName) |> ignore
                        
            let ex = failure.Exception
            let specFailedException = ex |> getSpecFailedException
            if specFailedException.IsSome then
                sb.AppendLine("\t\t" + specFailedException.Value.Data0) |> ignore
            else
                sb.AppendLine("\t\t" + ex.InnerException.Message) |> ignore
            
            sb.ToString()

        let printFailureDetails results = 
            Debug.WriteLine("\n------------------------ Failure Details -------------------------------------\n")
            for result in results do
                let failures = snd result     
                failures 
                |> List.iter (fun failure -> 
                                    Debug.WriteLine(getSingleFailureSummary failure)
                                    Debug.WriteLine("\n" + failure.Exception.ToString() + "\n\n" ))
            results

        let getFailureSummary (results : (string * FailureInfo list) list) =

            let rec failuresToString failures = 
                match failures with
                | []             -> ""
                | x::xs          -> (getSingleFailureSummary x) + "\n" + (failuresToString xs)

            let rec resultsToSummary (results : (string * FailureInfo list) list) = 
                match results with
                | []             -> ""
                | x::xs          -> (failuresToString (snd x)) + (resultsToSummary xs)
            
            results
            |> resultsToSummary
          
        
        let printFailureSummary results =
            let header =  "\n------------------------ Failure Summary -------------------------------------\n" 
            let failureSummary = getFailureSummary results
            if (failureSummary.Length > 0 ) then
                Debug.Write(header + failureSummary)

            results

        let specsPath = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"
        let tree = specsPath |> getContextTree

        tree.RunSpecs() 
        |> printResultTree
        |> printFailureDetails
        |> printFailureSummary
        |> ignore
        
        0


