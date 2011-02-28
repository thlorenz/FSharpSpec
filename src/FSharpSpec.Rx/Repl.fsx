
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Testing.1.0.2856.0\lib\Net4\System.Reactive.Testing.dll"
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Main.1.0.2856.0\lib\Net4\System.Reactive.dll"
#r "C:\dev\FSharp\FSharpSpec\src\packages\Rx-Core.1.0.2856.0\lib\Net4\System.CoreEx.dll"

#r @"C:\dev\FSharp\FSharpSpec\src\FSharp.Interop\bin\Debug\FSharp.Interop.dll"
#r @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Rx\bin\Debug\FSharpSpec.Rx.dll"
#r @"C:\dev\FSharp\FSharpSpec\src\FSharp.Interop.Rx\bin\Debug\FSharp.Interop.Rx.dll"
#r @"C:\dev\FSharp\FSharpSpec\src\Samples\Rx\FSharpSpec.Rx.UI\bin\Debug\FSharpSpec.Rx.UI.exe"

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
            
let testScheduler = TestScheduler()

let keys = testScheduler.CreateHotObservable( 
             onNext 210L "1", 
             onNext 220L "2", 
             onNext 230L "3", 
             onNext 240L "4" )


testScheduler.Run (fun () -> LoginManager.detectCorrectKeypass keys delay password testScheduler)

                             

