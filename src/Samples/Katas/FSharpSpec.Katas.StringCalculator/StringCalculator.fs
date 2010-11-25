namespace FSharpSpec.Katas.StringCalculator  

open System

type StringCalculator () =
    let determineSeparatorAndExtractNumberString (separatorWithNums:string) =
        let defaultSeparators = [',';'\n']
        match separatorWithNums.Trim() with
        | s when s.StartsWith("//") -> 
            let customSeparator = s.TrimStart('/').[0]
            let numbers = s.TrimStart('/', customSeparator, '\n')
            (List.toArray (customSeparator :: defaultSeparators), numbers)
                
        | _                         -> (List.toArray defaultSeparators, separatorWithNums)
    
    let validate nums =
        let filterNegatives = 
            nums
            |> Array.filter (fun n -> n < 0)
            |> Array.toList
       
        match filterNegatives with
        | []        -> nums
        | ns        ->
            let listOfViolators = 
                ns
                |> List.map(fun n -> n.ToString())
                |> List.toArray
                
            let csvViolators = String.Join(", ", listOfViolators)

            new ArgumentException(sprintf "No negatives allowed but found %s" csvViolators ) |> raise
    
    let ignoreLargeNumbers ns =
        ns
        |> Array.filter(fun n -> n <= 1000)
        

    let sumUpNumbers (separators, (nums:string)) = 
        nums.Split(separators)
        |> Array.map(fun s -> Int32.Parse s)
        |> validate
        |> ignoreLargeNumbers
        |> Array.sum 

    member x.Add numbers =
        match numbers with 
        | ""        -> 0
        | n         -> 
            determineSeparatorAndExtractNumberString n
            |> sumUpNumbers 
