namespace FSharpSpec

open System

module internal PrintUtils =
    let private escapeLineFeed (s:string) = s.Replace("\n","\\n")


    let prettifySpecName (specName:string) =
        specName
        |> escapeLineFeed
