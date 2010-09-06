namespace FSharpSpec.FSharpSampleSpecs

open System

[<AutoOpen>]
module SpecHelpers =
    let sformat s (arg : obj) = String.Format(s, arg)
    let ssformat s (arg1 : obj) (arg2 : obj) = String.Format(s, arg1, arg2)
