namespace FSharpSpec.ConsoleRunner

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils

module main = 
     
    [<EntryPoint>]
    let main(args:string[]) =
        
      //  let specsPath = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"
      // let specsPath = @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.FSharpSampleSpecs\bin\Debug\FSharpSpec.FSharpSampleSpecs.dll"
        let specsPath = @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.Katas\bin\Debug\FSharpSpec.Katas.dll"
        
        let runSpecsContainedInPath specsPath print =
           
            let tree = specsPath |> getContextTree
            let results = tree.RunSpecs() 
           
            printResultTree results print
            printFailureDetails results print
            printPendingSummary results print
            printFailureSummary results print
        
        runSpecsContainedInPath specsPath writeToConsole

        System.Console.ReadKey() |> ignore

        0


