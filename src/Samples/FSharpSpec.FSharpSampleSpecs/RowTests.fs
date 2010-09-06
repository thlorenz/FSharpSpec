namespace FSharpSpec.FSharpSampleSpecs

open System
open FSharpSpec

type ``when applying Math_Abs`` () =
    
    member x.``to positive numbers`` =
        let rec getAbsolutesSpecs (numbers : int list) = 
            match numbers with
            | []        -> []
            | x::xs     -> it (sformat "Abs({0}) returns {0}" x) (System.Math.Abs(x)) should.equal x :: getAbsolutesSpecs xs

        getAbsolutesSpecs [1; 2; 3; 4; 11; 111]

    member x.``to negative numbers`` =
        let rec getAbsolutesSpecs (numbers : int list) = 
            match numbers with
            | []        -> []
            | x::xs     -> it (ssformat "Abs({0}) returns {1}" x (-x)) (System.Math.Abs(x)) should.equal (-x) :: getAbsolutesSpecs xs

        getAbsolutesSpecs [-1; -2; -3; -4; -11; -111]