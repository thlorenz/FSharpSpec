module BDDSpecs

open System
open FSharpSpec

type ``Given a TestType(0)`` () =
      let _sut = new TestType(0)
     
      member x.sut = _sut
          
type ``named ContextName`` () = 
    inherit ``Given a TestType(0)`` ()    
    
    do 
      base.sut.Name <- "ContextName"
        
    member x.``when I increment by 1`` = 
        
        x.sut.IncrementValue() |> ignore
        
        [
            it "should have name ContextName" x.sut.Name should.equal "ContextName"
            it "should have Value 1" x.sut.Value should.equal 1
        ]

    member x.``when I increment twice`` = 
        x.sut.IncrementValue() |>ignore
        x.sut.IncrementValue() |>ignore
        [
            it "should have name ContextName" x.sut.Name should.equal "ContextName"
            it "should have Value 2" x.sut.Value should.equal 2
        ]
 
type ``that has been incremented`` () = 
  inherit ``named ContextName`` ()
    
  do 
    base.sut.IncrementValue() |> ignore
        
  member x.initially = 
      [
          it "should have name ContextName" x.sut.Name should.equal "ContextName"
          it "should have Value 1" x.sut.Value should.equal 1
      ]

  member x.``when I increment by 1`` = 
      x.sut.IncrementValue() |>ignore
      [
          it "should have name ContextName" x.sut.Name should.equal "ContextName"
          it "should have Value 2" x.sut.Value should.equal 2
      ]
 
type ``and was incremented again`` () = 
  inherit ``that has been incremented`` ()
    
  do 
    base.sut.IncrementValue() |> ignore
        
  member x.initially = 
      [
          it "should have name ContextName" x.sut.Name should.equal "ContextName"
          it "should have Value 2" x.sut.Value should.equal 2
      ]

  member x.``when I increment by 1`` = 
      x.sut.IncrementValue() |>ignore
      [
          it "should have name ContextName" x.sut.Name should.equal "ContextName"
          it "should have Value 3" x.sut.Value should.equal 3
      ]