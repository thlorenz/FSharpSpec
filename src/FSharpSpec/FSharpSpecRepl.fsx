

let source = [1;2] |> List.toSeq

let containedItems = [1;2;3] |> List.toSeq

let notContainedButShould = ( fun containedItem -> 
  source 
  |> Seq.exists(fun sourceItem -> sourceItem = containedItem) 
  |> not )  

let containedButShouldn't = ( fun sourceItem -> 
  containedItems 
  |> Seq.exists(fun containedItem -> containedItem = sourceItem) 
  |> not )

let itemsNotContainedButThatShouldHaveBeen =  containedItems |> Seq.filter notContainedButShould
let itemsContainedThatShouldn'tHaveBeen = source |> Seq.filter containedButShouldn't
   
