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

type AssemblyViewModel (asm : Assembly, controller : IGuiController) =
  inherit TreeViewModel(asm.FullName |> getShortAssemblyName, controller)
  
  let node = asm |> getAllContexts |> getContextTreeOfContexts 
  let _child =  ContextViewModel(node, controller)
  let mutable count = 0

  member x.runSpecs = 
    x.Child.AsI.Reset ()
    x.AsI.State <- x.Child.AsI.State
    
    Dispatcher.CurrentDispatcher.BeginInvoke(
      DispatcherPriority.SystemIdle, 
      action(fun () ->
        x.Child.runSpecs
        x.AsI.State <- x.Child.AsI.State)) 
    |> ignore

  member private x._runSpecsCommand = ActionCommand ((fun _ -> x.runSpecs), (fun _ -> true))
  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  member x.Child with get () : ContextViewModel = _child

type AssembliesViewModel (assemblies : ObservableCollection<Assembly>, controller) =
  inherit TreeViewModel("", controller)
  
  let _children = 
    ObservableCollection<AssemblyViewModel>(assemblies |> Seq.map (fun a -> AssemblyViewModel(a, controller)))
  
  interface ITreeViewModel with  
    override x.Children with get () = _children.AsEnumerable() |> Seq.cast<ITreeViewModel>
