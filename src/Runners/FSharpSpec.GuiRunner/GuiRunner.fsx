#I @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec\bin\Debug"
#r "FSharpSpec.dll"
#I @"C:\dev\FSharp\FSharpSpec\src\Runners\FSharpSpec.RunnerUtils\bin\Debug"
#r "FSharpSpec.RunnerUtils.dll"

#I @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\"
#r @"PresentationCore.dll"
#r "PresentationFramework.dll"
#r "WindowsBase.dll"
#r "System.Xaml"
#r "Microsoft.CSharp"

open System
open System.IO
open System.Linq
open System.Diagnostics

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils
       
let path = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"

let asm = path |> getAssembly
let allContexts = asm |> getAllContexts
let getName (asmFullName : string) = (asmFullName.Split(',')).[0]

open System.Collections.ObjectModel

open System.Windows
open System.Windows.Controls

type ContextsViewModel () =
  let _childContexts = new ObservableCollection<ContextsViewModel>()
  let _specContainers = ObservableCollection()
  
  member x.ChildContexts = _childContexts
  member x.SpecContainers = _specContainers

  member public x.Explore (tree : Node) = ()

let runSpecsAndShowResults name printer (node : Node) = 
  printfn "Running specs"
  let results = node.RunSpecs() 
  
  printResultTree     results printer          
  printFailureDetails results printer
  printPendingSummary results printer
  printFailureSummary results printer
        
let tree = allContexts |> getContextTreeOfContexts

let uiTree = TreeView()
let win = Window( Topmost = true, Width=300.0, Height = 200.0, Content = uiTree)
win.Show()

let rootItem = TreeViewItem ( Header = (getName asm.FullName))
uiTree.Items.Add rootItem


let foundInItemsOf (treeItem : TreeViewItem) name =
  treeItem.Items.Cast<TreeViewItem>()
  |> Seq.filter (fun i -> i.Header = name)

let addOrFindItem rootItem (node : Node) name = 
  let foundItems = name |> foundInItemsOf rootItem
  match name with
  | n when Seq.length foundItems > 0  -> Seq.head foundItems
  | n                                 -> 
      let newItem = TreeViewItem ( Header = n, DataContext = node )
      rootItem.Items.Add newItem |> ignore
      newItem.MouseDown |> Event.add (fun i -> if i.Source.Equals newItem then node |> runSpecsAndShowResults name writeToConsole )
      newItem

let rec populateTree (rootItem : TreeViewItem) (node : Node) =
  
  let ctx = node.Context
  let clazz = ctx.Clazz
  let moduleType = clazz.DeclaringType
  let contextName = clazz.Name    

  if (not <| clazz.Equals(typeof<obj>)) then 
    
    let specsContainers = ctx.SpecLists
    
    let contextItem = 
      match moduleType with
      | n when n = null ->  addOrFindItem rootItem node contextName
      | n               ->  let moduleItem = addOrFindItem rootItem node moduleType.Name 
                            addOrFindItem moduleItem node contextName

   
    let instantiatedContext = ctx.Clazz |> instantiate
    specsContainers |> Array.iter (fun container ->
      let containerItem =  container.Name  |> removeLeadingGet|> addOrFindItem contextItem node
      let specs = container.Invoke(instantiatedContext, null) :?> (string * SpecDelegate) list
      specs 
      |> List.map  (fun s -> fst s)
      |> List.map  (fun n -> TreeViewItem (Header = n))
      |> List.iter (fun i -> containerItem.Items.Add i |> ignore)
    )

  node.Children
  |> List.iter (fun c -> populateTree rootItem c)

rootItem.Items.Clear()
populateTree rootItem tree


