module ContainsSpecs
  
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

  member x.``Invalid Strings`` = 
    ass "null should.contain \"some string\" will fail" 
      (it "null should.contain \"some string\"" null should.contain "some string") will.fail
    
  member x.``list containing single values`` =
    let ``1; 2; 3`` = [1; 2; 3]
    [
      it "``1; 2; 3`` should.contain 1" ``1; 2; 3`` should.contain 1
      it "``1; 2; 3`` should.contain 2" ``1; 2; 3`` should.contain 2
            
      it "``1; 2; 3`` shouldn't.contain 4" ``1; 2; 3`` shouldn't.contain 4
            
      ass "``1; 2; 3`` should.contain 4 will fail" (it "" ``1; 2; 3`` should.contain 4) will.fail
      ass "``1; 2; 3`` shouldn't.contain 1 will fail" (it "" ``1; 2; 3`` shouldn't.contain 1) will.fail
    ]

  member x.``list containing multiple values`` = [
    it "[1;2;3] should.contain1 [1;2]" [1;2;3] should.contain1 [1;2]
    it "[1;2;3] should.contain1 [1;2;3]" [1;2;3] should.contain1 [1;2;3]
   
    ass "[1;2] should.contain1 [] will fail" (it "" [1;2] should.contain1 []) will.fail
    ass "[] should.contain1 [1;2] will fail" (it "" [] should.contain1 [1;2]) will.fail
    ass "[1;2;3] should.contain [1;4] will fail" 
      (it "" [1;2;3] should.contain1 [1;4]) will.fail
    ass "[1;2;3] should.contain [1;2;3;4] will fail" 
      (it "" [1;2;3] should.contain1 [1;2;3;4]) will.fail
  ]

  member x.``contains only`` = [
      it "[1] contains only [1]" [1] should.containOnly [1]
      it "[1;2] should.containOnly [1;2]" [1;2] should.containOnly [1;2]
      it "[1;2;3] should.containOnly [1;2;3]" [1;2;3] should.containOnly [1;2;3]
      it "[] should.containOnly []" [] should.containOnly []

      ass "[1;2] contains only [1] will fail" 
        ( it "" [1;2] should.containOnly [1] ) will.fail
      ass "[] contains only [1] will fail" 
        ( it "" [] should.containOnly [1] ) will.fail
      ass "[1] contains only [] will fail" 
        ( it "" [1] should.containOnly [] ) will.fail
      ass "[1] contains only [1;2] will fail" 
        ( it "" [1] should.containOnly [1;2] ) will.fail

  ]