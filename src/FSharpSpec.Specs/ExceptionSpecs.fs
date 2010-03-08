namespace FSharpSpec.Specs

open System
open FSharpSpec

type Exceptions () =
     member x.``FailWith type`` = [
         it "1/0 should fail with DivideByZeroException" 
           (fun () -> 1 / 0 |> ignore) should.failWith typeof<DivideByZeroException>  
         
         ass "\"0/1 should fail with DivideByZeroException\" will fail"
            (it "" (fun () -> 0 / 1 |> ignore) should.failWith typeof<DivideByZeroException>) will.fail  
     ] 
     
     member x.``FailWithMessage`` = [
        it "failwith \"Exact Message\" should.failWith \"Exact Message\"" 
            (fun () -> failwith "Exact Message" |> ignore) should.failWithMessage "Exact Message"
            
        ass "failwith \"Exact Message\" should.failWith \"Different Message\" will fail" 
            (it "" (fun () -> failwith "Exact Message" |> ignore) should.failWithMessage "Different Message") will.fail
     ]
     
     member x.``FailWithMessageContaining`` = [
        it  "failwith \"Exact Message\" should.failWithMessageContaining \"Mess\""
           (fun () -> failwith "Exact Message" |> ignore) should.failWithMessageContaining "Mess"
           
        ass "failwith \"Exact Message\" should.failWithMessageContaining \"Different\" will fail"
           (it "" (fun () -> failwith "Exact Message" |> ignore) should.failWithMessageContaining "Different") will.fail 
     ]
     
   
   