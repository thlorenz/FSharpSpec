#r @"..\FSharpSpec\bin\Debug\FSharpSpec.dll"
open FSharpSpec

run 1 shouldn't.equal 2 
run 1 should.equal 2

// Delayed Evaluation to allow multiple assertions
run (1/0) should.equal 1
run (fun () -> 1/0) should.equal 1
run1 (lazy ( 1 / 0 )) should.equal 1


