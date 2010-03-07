namespace FSharpSpec.Specs

open System
open FSharpSpec

type Exceptions () =
     member x.FailWith = [
         it "1/0 should fail with DivideByZeroException" 
           (fun () -> 1 / 0 |> ignore) should.failWith typeof<DivideByZeroException>  
         
         ass "\"0/1 should fail with DivideByZeroException\" will fail"
            (it "" (fun () -> 0 / 1 |> ignore) should.failWith typeof<DivideByZeroException>) will.fail  
         
     ] 
   
   