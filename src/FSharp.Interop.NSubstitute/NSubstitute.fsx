#r @"C:\dev\FSharp\FSharpSpec\src\packages\NSubstitute.1.0.0.0\lib\35\NSubstitute.dll"
#r @"C:\dev\FSharp\FSharpSpec\src\FSharp.Interop.NSubstitute\bin\Debug\FSharp.Interop.NSubstitute.dll"

open System

open FSharp.Interop
open System.ComponentModel
open NSubstitute

type ClimbedTreeEventArgs (meters : int) =
  inherit EventArgs ()
  member x.Meters with get () = meters
type ClimbedTreeDelegate = delegate of obj * ClimbedTreeEventArgs -> unit

type IMonkey =
  abstract Scream : string -> unit
  abstract Name : string with get 
  abstract Age : int with get, set
  abstract Eat : int -> string -> unit
  abstract Count : string -> int
  [<CLIEvent>]
  abstract ClimbedTree : IEvent<ClimbedTreeDelegate, ClimbedTreeEventArgs>

type MyEventArgs(msg:string) =
    inherit EventArgs()
    member this.Message = msg

type MyEventDelegate = delegate of obj * MyEventArgs -> unit

type Foo() = 
    let ev = new Event<MyEventDelegate, MyEventArgs>()

    member this.Ping(msg) =
        ev.Trigger(this, new MyEventArgs(msg))

    [<CLIEvent>]
    member this.GotPinged = ev.Publish


let m = fake<IMonkey>

m.Name |> returns "Bob"
printfn "m.Name -> %A" m.Name

m.Count "bananas" |> returns 2
printfn "m.Count 'bananas' -> %A" <| m.Count "bananas"

m.Count any<string> |> returns 7
printfn "m.Count 'apples' -> %A" <| m.Count "apples"

m.Scream "Hello"

(m |> received).Scream("Hello")
(m |> received).Scream(any<string>)
(m |> clearReceivedCalls)

m.Eat 3 "bananas"

(m |> received).Eat 3 "bananas"
(m |> received).Eat any<int> any<string>

m.Age <- 3
(m |> received).Age <- 3

let npc = fake<INotifyPropertyChanged>
let sender = new obj()
let args = PropertyChangedEventArgs("hello")
let raise = Raise.EventWith(sender, args)
//// npc.PropertyChanged.AddHandler ( PropertyChangedEventHandler(fun se -> -> o -> raise |> ignore ))







