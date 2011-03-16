namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel
open System.Reflection
open System.Windows.Input

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsRunnerUtils
open SpecsExtractor

type AssemblyViewModel (asm : Assembly, specsRunResult : SpecsRunResult) =
  inherit TreeViewModel(asm.FullName |> getShortAssemblyName, specsRunResult)
  
  let node = asm |> getAllContexts |> getContextTreeOfContexts 
  let _child =  ContextViewModel(node , specsRunResult)

  member x.runSpecs = 
    x.State <- NotRunYet
    x.Child.runSpecs
    x.State <- x.Child.State

  member private x._runSpecsCommand = ActionCommand ((fun _ -> x.runSpecs), (fun _ -> true))
 
  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  member x.Child with get () : ContextViewModel = _child

type AssembliesViewModel (assemblies : ObservableCollection<Assembly>, specsRunResult : SpecsRunResult) =
  inherit TreeViewModel("", specsRunResult)
  
  let _children = 
    ObservableCollection<AssemblyViewModel>(assemblies |> Seq.map (fun a -> AssemblyViewModel(a, specsRunResult)))
    
  member x.Children with get () = _children
