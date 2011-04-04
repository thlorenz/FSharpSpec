module TypeCheckingSpecs

open System
open FSharpSpec

type TypeChecking () =
  member x.Structs = [
    it "1 should.be typeof<int>" 1 should.be typeof<int>
    it "1.0 should.be typeof<double>" 1.0 should.be typeof<double>
    it "1 shouldn't.be typeof<double>" 1 shouldn't.be typeof<double>
    it "1.0 shouldn't.be typeof<int>" 1.0 shouldn't.be typeof<int>
    it "(TestType(0)) should.be typeof<TestType>" (TestType(0)) should.be typeof<TestType>
    it "\"a string\" should.be1 typeof<string>" "a string" should.be1 typeof<string>
        
    ass "1 should.be typeof<double> will.fail" (it "" 1 should.be typeof<double>) will.fail
    ass "1.0 should.be typeof<int> will.fail" (it "" 1.0 should.be typeof<int>) will.fail
    ass "1 shouldn't.be typeof<int> will.fail" (it "" 1 shouldn't.be typeof<int>) will.fail
    ass "1.0 shouldn't.be typeof<double> will.fail" (it "" 1.0 shouldn't.be typeof<double>) will.fail
  ]
        
        