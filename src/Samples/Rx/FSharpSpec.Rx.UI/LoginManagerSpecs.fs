module LoginManagerSpecs

open System
open System.Linq
open System.Collections.Generic
open System.Concurrency


open System.Reactive
open System.Reactive.Testing

open FSharp.Interop
open FSharpSpec
open FSharpSpec.Rx.UI


type LoginManagerSpecs () =
 
  member x.delay = TimeSpan.FromTicks(50L)

type ``when the user enters 1234`` () =
  inherit LoginManagerSpecs ()
            
  let testScheduler = TestScheduler()
  
  let keys = testScheduler.CreateHotObservable( 
                              onNext 210L "1", 
                              onNext 220L "2", 
                              onNext 230L "3", 
                              onNext 240L "4" )

  member x.``and the password is 1234`` =
    let password = "1234"  
   
    let record = testScheduler.Run (fun () -> 
      LoginManager.detectCorrectKeypass keys x.delay password testScheduler)
    
    it "accepts the entry" record should.contain <| onNext 240L true

  
  member x.``and the password is xxxx`` =
    let password = "xxxx"    
   
    let record = testScheduler.Run (fun () -> 
      LoginManager.detectCorrectKeypass keys x.delay password testScheduler)
    
    it "rejects the entry" record should.contain <| onNext 240L false

type ``when the user types too slow`` () =
  inherit LoginManagerSpecs ()

  let testScheduler = TestScheduler()
  
  let keys = testScheduler.CreateHotObservable( 
                              onNext 210L "1", 
                              onNext 220L "2", 
                              onNext 230L "3", 
                              onNext 340L "4" )
   
  member x.``and the password is 1234`` =
    let password = "1234"  
   
    let record = testScheduler.Run (fun () -> 
      LoginManager.detectCorrectKeypass keys x.delay password testScheduler)

    it "rejects the entry when the timeout is exceeded" record should.contain <| onNext 251L false