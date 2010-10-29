namespace FSharpSpec.Specs

open System
open FSharpSpec


type Contains () =
    
    member x.``Valid Strings`` = 
        let hello = "hello"
        let hel = "hel"
        [
            it "hello should.contain hel" hello should.contain hel
            it "hel shouldn't.contain hello" hel shouldn't.contain hello 
           
            ass "\"hel should.contain hello\" will fail" (it "" hel should.contain hello) will.fail
            ass "\"helllo shouldn't.contain hel\" will fail" (it "" hello shouldn't.contain hel) will.fail
        ]
    member x.``Invalid Strings`` = [
            //  it "null should.contain \"some string\"" null should.contain "some string"
        ]
             
    
    member x.``list containing single values`` =
        let ``1; 2; 3`` = [1; 2; 3]
        [
            it "``1; 2; 3`` should.contain 1" ``1; 2; 3`` should.contain 1
            it "``1; 2; 3`` should.contain 2" ``1; 2; 3`` should.contain 2
            
            it "``1; 2; 3`` shouldn't.contain 4" ``1; 2; 3`` shouldn't.contain 4
            
            ass "``1; 2; 3`` should.contain 4 will fail" (it "" ``1; 2; 3`` should.contain 4) will.fail
            ass "``1; 2; 3`` shouldn't.contain 1 will fail" (it "" ``1; 2; 3`` shouldn't.contain 1) will.fail
        ]

  