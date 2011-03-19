namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel
open System.Reflection
open System.Windows.Input

open System.Diagnostics
open System
open System.Linq

open FSharp.Interop
open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsRunnerUtils
open SpecsExtractor

open System.Windows.Threading

type AssemblyViewModel (asm : Assembly, specsRunResult : SpecsRunResult) =
  inherit TreeViewModel(asm.FullName |> getShortAssemblyName, specsRunResult)
  
  let node = asm |> getAllContexts |> getContextTreeOfContexts 
  let _child =  ContextViewModel(node , specsRunResult)
  let mutable count = 0

  member x.runSpecs = 
    x.Child.Reset ()
    x.State <- x.Child.State
    
    Dispatcher.CurrentDispatcher.BeginInvoke(
      DispatcherPriority.SystemIdle, 
      action(fun () ->
        x.Child.runSpecs
        x.State <- x.Child.State)) 
    |> ignore

  member private x._runSpecsCommand = ActionCommand ((fun _ -> x.runSpecs), (fun _ -> true))
 
  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  member x.Child with get () : ContextViewModel = _child

type AssembliesViewModel (assemblies : ObservableCollection<Assembly>, specsRunResult : SpecsRunResult) =
  inherit TreeViewModel("", specsRunResult)
  
  let _children = 
    ObservableCollection<AssemblyViewModel>(assemblies |> Seq.map (fun a -> AssemblyViewModel(a, specsRunResult)))
    
  override x.Children with get () = _children.AsEnumerable() |> Seq.cast<TreeViewModel>
