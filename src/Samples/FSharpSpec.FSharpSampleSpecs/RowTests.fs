module RowTests

open System
open FSharpSpec

type ``when applying Math_Abs`` () =
  member x.``to positive numbers``() =
      [1; 2; 3; 4; 11; 111]
      |> List.map (fun x ->
          it (sprintf "Abs(%d) returns %d" x x) (Math.Abs x) should.equal x)

  member x.``to negative numbers`` =
        
      [-1; -2; -3; -4; -11; -111]
      |> List.map (fun x ->
          it (sprintf "Abs(%d) returns %d"  x (-x)) (Math.Abs x) should.equal (-x)) 

type ``when calculating Math_Sqrt`` () =  
  member x.``of positive numbers`` =
        
      [ 1.0, 1.0; 
        4.0, 2.0;
        9.0, 3.0 ]
      |> List.map (fun (square, expectedRoot) ->
          it (sprintf "Sqrt(%f) returns %f" square expectedRoot) (Math.Sqrt square) should.equal expectedRoot)