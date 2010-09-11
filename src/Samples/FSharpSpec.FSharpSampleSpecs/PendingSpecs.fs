namespace FSharpSpec.FSharpSampleSpecs

open System
open FSharpSpec

type ``Pending specs samples``() =
    member x.``when we are further along with development`` = [
       soon.it "fullfills the first requirement"
       soon.it "fullfills the second requirement"
       ]

