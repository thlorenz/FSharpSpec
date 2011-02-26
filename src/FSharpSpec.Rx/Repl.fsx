
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Testing.1.0.2856.0\lib\Net4\System.Reactive.Testing.dll"
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Main.1.0.2856.0\lib\Net4\System.Reactive.dll"
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Core.1.0.2856.0\lib\Net4\System.CoreEx.dll"

#r @"C:\dev\FSharp\FSharpSpec\src\FSharp.Interop\bin\Debug\FSharp.Interop.dll"
#r @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Rx\bin\Debug\FSharpSpec.Rx.dll"
#r @"C:\dev\FSharp\FSharpSpec\src\FSharp.Interop.Rx\bin\Debug\FSharp.Interop.Rx.dll"

open System.Threading
open System
open System.Linq
open System.Collections.Generic
open System.Concurrency


open System.Reactive
open System.Reactive.Testing

open FSharp.Interop
open FSharpSpec.Rx

open Obs


let password = "1234"
let delay = TimeSpan.FromTicks(50L)
            
let scheduler = TestScheduler()

let keys = scheduler.CreateHotObservable( 
             onNext 210L "1", 
             onNext 220L "2", 
             onNext 230L "3", 
             onNext 240L "4" )

let target = func <| fun () ->
  keys 
  |> bufferWithTimeOrCount  delay password.Length scheduler
  |> map ( fun strList -> String.Join(String.Empty, strList.ToArray()) )
  |> filter ( fun guess -> guess <> "" )
  |> map ( fun guess -> guess = password )
  |> distinctUntilChanged 



scheduler.Run target 

                             

