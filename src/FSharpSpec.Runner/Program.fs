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
        
        let getSingleFailureSummary failure =
           
            let getSpecFailedException (ex : Exception) =
                let innerEx = ex.InnerException
                let assertionEx = 
                    match innerEx.GetType() with
                    | ty when ty = typeof<SpecFailedException> ->  Some(innerEx :?> SpecFailedException)
                    | _                                        ->  None
                assertionEx
            
            let extractException failure = 
                let specFailedException = failure.Exception |> getSpecFailedException
                match specFailedException with
                | excep when excep.IsSome -> "\t\t" + specFailedException.Value.Data0
                | _                       -> "\t\t" + failure.Exception.InnerException.Message     
            
            failure.FullSpecName + (extractException failure) + "\n"  
            
        let getFailureDetails results = 
            let rec failureDetailsToString = function
                | []        -> ""
                | x::xs     -> (new StringBuilder())
                                .AppendLine("\n" + getSingleFailureSummary x + "\n")
                                .AppendLine(x.Exception.ToString() + "\n\n")
                                .ToString() + failureDetailsToString xs
                                
             
            let rec extractFailureDetails = function
                | []                        -> ""
                | (output, failure)::xs     -> (failureDetailsToString failure) + extractFailureDetails xs

            let header = "\n------------------------ Failure Details -------------------------------------\n"

            match results |> extractFailureDetails with
            | ""        -> ""
            | details   -> header + details

       
        let getFailureSummary results =

            let rec failuresToString = function
                | []             -> ""
                | x::xs          -> (getSingleFailureSummary x) + "\n" + (failuresToString xs)

            let rec resultsToSummary = function
                | []             -> ""
                | x::xs          -> (failuresToString (snd x)) + (resultsToSummary xs)
            
            let header =  "\n------------------------ Failure Summary -------------------------------------\n\n" 
           
            match results|> resultsToSummary with
            | ""        -> ""
            | summary   -> header + summary

        let writeToDebug    = function | content -> Debug.Write content
        let writeToConsole  = function | content -> printf "%s" content
        
        let print = writeToDebug    

        let printResultTree results =
          print "\n------------------------ Specifications -------------------------------------\n\n" 
          results
          |> List.iter (fun r -> fst r |> print)
          results

        let printFailureDetails results = 
            results
            |> getFailureDetails
            |> print
            results
        
        let printFailureSummary results =
            results
            |> getFailureSummary
            |> print
            results
         

       // let specsPath = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"
        let specsPath = @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.FSharpSampleSpecs\bin\Debug\FSharpSpec.FSharpSampleSpecs.dll"
        let tree = specsPath |> getContextTree
        
        tree.RunSpecs() 
        |> printResultTree
        |> printFailureDetails
        |> printFailureSummary
        |> ignore
        
        0


