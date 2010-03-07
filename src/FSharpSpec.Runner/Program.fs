﻿namespace FSharpSpec.Runner

open System
open System.Collections.Generic
open SpecsExtractor 
open System.Linq
open System.Reflection

exception WrongSecond of int

module main = 
     
    [<EntryPoint>]
    let main(args:string[]) =
        let dllPath = args.[0] 
        let verbose = Array.Exists(args, fun arg -> arg.Equals("-verbose"))
        let errorReports = new Dictionary<string, Exception>() 
        
        printf "Testing: %s" dllPath
        let allSpecs = allContextsSorted dllPath
          
        for concern in allSpecs.Keys do
            printfn "\n\n%s" concern
            for _ in 1 .. concern.Length do printf "--"
            printfn ""
            for (context, specifications) in allSpecs.[concern] do
                let cleanContext = 
                    match context with
                    | s when s.StartsWith("get_")  -> s.Substring(4)
                    | s                            -> s
                    
                printf "\n  %s:" cleanContext
                for(specName, specDelegate) in specifications do 
                    try  
                      specDelegate.Invoke() |> ignore
                      if verbose then printf "\n\t -> %s" specName 
                    with   
                      | ex -> printf "\n\t ~ %s (Failed)" specName 
                              errorReports.Add(String.Format("concern: {0} Context: {1} \n  -> {2}", concern, cleanContext, specName), ex)
                printfn ""
                
        printfn "\n____________________________________________________________" 
        for entry in errorReports do
            printfn "\n\n%s:\n%s" entry.Key (entry.Value.ToString())
            printfn "==========================================================="
        
        match errorReports.Count with
        | 0         -> printfn "No Failures found -> GREEN"
        | _         -> printfn "\n\nFound the following errors (full stacktraces above):\n"
                       for entry in errorReports do
                           printfn "%s: %A" entry.Key entry.Value
                            
        let key = Console.ReadLine()
        0
