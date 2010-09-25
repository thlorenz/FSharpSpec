﻿namespace FSharpSpec

open System
open System.Collections.Generic
open System.Linq
open System.Reflection
open System.Diagnostics
open System.Text
open FSharpSpec
open SpecsExtractor

module SpecsRunnerUtils =

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
            | (m, f, p)::xs     -> (failureDetailsToString f) + extractFailureDetails xs

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
            | (m,f,p)::xs    -> (failuresToString f) + (resultsToSummary xs)
            
        let header =  "\n------------------------ Failure Summary -------------------------------------\n\n" 
           
        match results|> resultsToSummary with
        | ""        -> ""
        | summary   -> header + summary


    let writeToDebug    = function | content -> Debug.Write content
    let writeToConsole  = function | content -> printf "%s" content
        
       

    let printResultTree results print =
        print "\n------------------------ Specifications -------------------------------------\n\n" 
        results
        |> List.iter (fun (m,f,p) -> m |> print)
         

    let printFailureDetails results print = 
        results
        |> getFailureDetails
        |> print
           
        
    let getPendingSummary (results : ('a *'b * string list) list) =
        let pendings = results |> List.map (fun (m,f,p) ->  
            match p with
            | []    -> ""
            | ps    -> ps |> List.reduce (fun acc pp -> acc + pp + "\n"))
            
        match pendings with
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

