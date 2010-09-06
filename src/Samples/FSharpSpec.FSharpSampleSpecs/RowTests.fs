namespace FSharpSpec.FSharpSampleSpecs

open System
open FSharpSpec

type ``when applying Math_Abs`` () =
    
    member x.``to positive numbers`` =
        let rec getAbsForPositiveNumbersSpecs (numbers : int list) = 
            match numbers with
            | []        -> []
            | x::xs     -> it (sprintf "Abs(%d) returns %d" x x) (Math.Abs(x)) should.equal x 
                           :: getAbsForPositiveNumbersSpecs xs

        getAbsForPositiveNumbersSpecs [1; 2; 3; 4; 11; 111]

    member x.``to negative numbers`` =
        let rec getAbsForNegativeNumbersSpecs (numbers : int list) = 
            match numbers with
            | []        -> []
            | x::xs     -> it (sprintf "Abs(%d) returns %d"  x (-x)) (Math.Abs(x)) should.equal (-x) 
                           :: getAbsForNegativeNumbersSpecs xs

        getAbsForNegativeNumbersSpecs [-1; -2; -3; -4; -11; -111]