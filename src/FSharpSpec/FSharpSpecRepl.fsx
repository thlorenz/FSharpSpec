

let source = Seq.empty

let containedItems = [1;2;3] |> List.toSeq

let testForEmpty = 
  match source with
  | s when s |> Seq.isEmpty -> ("", "Passed")
  | otherwise               -> (sprintf "%A was expected to be empty but wasn't" source, "Failed")


