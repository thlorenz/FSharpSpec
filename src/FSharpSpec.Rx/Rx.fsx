
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Core.1.0.2856.0\lib\Net35\System.Observable.dll"
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Core.1.0.2856.0\lib\Net35\System.CoreEx.dll"
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Main.1.0.2856.0\lib\Net35\System.Reactive.dll"
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Testing.1.0.2856.0\lib\Net35\System.Reactive.Testing.dll"
#r @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Rx\bin\Debug\FSharpSpec.Rx.dll"


open System.Threading
open System
open System.Linq
open System.Collections.Generic
open System.Concurrency
open System.Reactive
open System.Reactive.Testing

open FSharpSpec.Rx


let enteredPassKey = "1234"
            
let scheduler = TestScheduler()
let keys = scheduler.CreateHotObservable (
                             rx.OnNext 210L "1",
                             rx.OnNext 220L "2", 
                             rx.OnNext 230L "3", 
                             rx.OnNext 240L "4")

rx.OnNext 3L "a"
rx.OnNext 5L "b"






