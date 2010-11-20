namespace FSharpSpec.RunnerUtils

open System
open System.Collections.Generic
open System.Linq
open System.Reflection
open System.Diagnostics
open System.Text
open FSharpSpec

module SpecsRunnerUtils =

    let getSpecFailedException (ex : Exception) =
            let innerEx = ex.InnerException
            let assertionEx = 
                match innerEx.GetType() with
                | ty when ty = typeof<SpecFailedException> ->  Some(innerEx :?> SpecFailedException)
                | _                                        ->  None
            assertionEx
    
    let getFailureMessage failure = 
        let specFailedException = failure.Exception |> getSpecFailedException
        match specFailedException with
        | excep when excep.IsSome -> "\t\t" + specFailedException.Value.Data0
        | _                       -> "\t\t" + failure.Exception.InnerException.Message   

    let getFailureStackTrace failure = 
        let specFailedException = failure.Exception |> getSpecFailedException
        match specFailedException with
        | excep when excep.IsSome -> "\t\t" + specFailedException.Value.StackTrace
        | _                       -> "\t\t" + failure.Exception.InnerException.StackTrace   

    let getSingleFailureSummary failure =
            
        failure.FullSpecName + (getFailureMessage failure) + "\n"  
    
    let getMeaningFullStackTrace failure =
        match failure.Exception.InnerException with
        | null  -> failure.Exception.StackTrace
        | _     -> failure.Exception.InnerException.StackTrace 
            
    let getFailureDetails results = 

        let rec failureDetailsToString = function
            | []        -> ""
            | x::xs     -> (new StringBuilder())
                            .AppendLine("\n" + getSingleFailureSummary x + "\n")
                            .AppendLine(getMeaningFullStackTrace x + "\n\n")
                            .ToString() + failureDetailsToString xs
             
        let rec extractFailureDetails = function
            | []                        -> ""
            | (msg, passes, failures, pendings)::xs     -> (failureDetailsToString failures) + extractFailureDetails xs

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
            | (msg, passes, failures, pendings)::xs    -> (failuresToString failures) + (resultsToSummary xs)
            
        let header =  "\n------------------------ Failure Summary -------------------------------------\n\n" 
           
        match results|> resultsToSummary with
        | ""        -> ""
        | summary   -> header + summary


    let writeToDebug    = function | content -> Debug.Write content
    let writeToConsole  = function | content -> printf "%s" content

    let printResultTree results print =
        print "\n------------------------ Specifications -------------------------------------\n\n" 
        results
        |> List.iter (fun (msg, passes, failures, pendings) -> msg |> print)
         

    let printFailureDetails results print = 
        results
        |> getFailureDetails
        |> print
           
        
    let getPendingSummary results =
        let pendings = results |> List.map (fun (msg, passes, failures, pendings) ->  
            match pendings with
            | []    -> null
            | ps    -> ps |> List.reduce (fun acc pp -> acc + pp + "\n"))
            
        let validPendings = pendings |> List.filter(fun p -> not (String.IsNullOrEmpty p))

        match validPendings with
        | []    -> ""
        | ps    -> "\n------------------------ Pending -------------------------------------\n\n" +
                    (ps |>  List.reduce (fun acc p -> acc + p))

    let printPendingSummary results print =
        results
        |> getPendingSummary
        |> print
           
        
    let printFailureSummary results print =
        results
        |> getFailureSummary
        |> print

