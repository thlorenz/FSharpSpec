namespace FSharpSpec.Runner

open System
open System.Collections.Generic
open SpecsExtractor 
open System.Linq
open System.Reflection
open System.Diagnostics
open SpecsExtractor

module main = 
     
    [<EntryPoint>]
    let main(args:string[]) =
        let specsPath = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"
        let tree = specsPath |> getContextTree
        Debug.WriteLine(tree.RunSpecs())
        0


