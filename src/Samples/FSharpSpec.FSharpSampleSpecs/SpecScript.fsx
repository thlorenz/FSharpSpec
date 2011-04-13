#r @"..\..\FSharpSpec\bin\Debug\FSharpSpec.dll"
open FSharpSpec

let mutable sut = 0
let contexts =  
  [ 
    ("when an int is 0",  (fun () -> sut <- 0))
    ("and i add 1",       (fun () -> sut <- sut + 1))
    ("and i add 2",       (fun () -> sut <- sut + 2))
  ] 
  |> Map.ofList

let setup id = 
  printfn "%s" id
  contexts.[id] ()

"int specifications"    |> printfn "%s"
"=================="    |> printfn "%s"

"when an int is 0"      |> setup 
"and i add 1"           |> setup
" -> int is 1"          |> run sut should.equal 1

"and i add 2"           |> setup
"-> int is 3"           |> run sut should.equal 3

"when an int is 0"      |> setup 
"and i add 2"           |> setup
"-> int is 2"           |> run sut should.equal 2
"and i add 2"           |> setup
"-> int is 4"           |> run sut should.equal 4

