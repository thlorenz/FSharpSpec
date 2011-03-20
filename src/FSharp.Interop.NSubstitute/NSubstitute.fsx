#r @"C:\dev\FSharp\FSharpSpec\src\packages\NSubstitute.1.0.0.0\lib\35\NSubstitute.dll"
#r @"C:\dev\FSharp\FSharpSpec\src\FSharp.Interop.NSubstitute\bin\Debug\FSharp.Interop.NSubstitute.dll"

open System
open NSubstitute
open NSubstitute.Core

open FSharp.Interop

type IMonkey =
  abstract Scream : string -> unit
  abstract Name : string with get 
  abstract Age : int with get, set
  abstract Eat : int -> string -> unit
  abstract Count : string -> int


// MonkeyMock
let m = Substitute.For<IMonkey>()

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

