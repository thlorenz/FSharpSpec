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

        let getFailureDetails results = 
            let rec failureDetailsToString failures =
                match failures with
                | []        -> ""
                | x::xs     -> (getSingleFailureSummary x) + "\n" + 
                               x.Exception.ToString() + "\n\n" + 
                               (failureDetailsToString xs)
             
            let rec extractFailureDetails results =
                match results with
                | []                        -> ""
                | (output, failure)::xs     -> (failureDetailsToString failure) + extractFailureDetails xs

            let header = "\n------------------------ Failure Details -------------------------------------\n"

            match results |> extractFailureDetails with
            | ""        -> ""
            | details   -> header + details

       
        let getFailureSummary (results : (string * FailureInfo list) list) =

            let rec failuresToString failures = 
                match failures with
                | []             -> ""
                | x::xs          -> (getSingleFailureSummary x) + "\n" + (failuresToString xs)

            let rec resultsToSummary (results : (string * FailureInfo list) list) = 
                match results with
                | []             -> ""
                | x::xs          -> (failuresToString (snd x)) + (resultsToSummary xs)
            
            let header =  "\n------------------------ Failure Summary -------------------------------------\n" 
            match results|> resultsToSummary with
            | ""        -> ""
            | summary   -> header + summary

            
        let printFailureDetails results =     
            Debug.Write(results |> getFailureDetails)
            results
        
        let printFailureSummary results =
            Debug.Write(results |> getFailureSummary)
            results

        let specsPath = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"
        let tree = specsPath |> getContextTree

        tree.RunSpecs() 
        |> printResultTree
        |> printFailureDetails
        |> printFailureSummary
        |> ignore
        
        0


