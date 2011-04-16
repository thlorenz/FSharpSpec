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

  member private x._runSpecsCommand = 
     ActionCommand ((fun _ ->
      x.AsITreeViewModel |> resetResolveAndRunSpecs
      x.IsSelected <- true), 
      (fun _ -> true))
 
  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  member x.Child with get () : ContextViewModel = _child
  
  interface ITreeViewModel with
    override x.ResetResults () = 
      x.Child.AsITreeViewModel.ResetResults ()
      x.AsITreeViewModel.State <- x.Child.AsITreeViewModel.State

    override x.ResolveSpecs () = x.Child.AsITreeViewModel.ResolveSpecs ()
   
    override x.RunSpecs completed = 
      x.Child.AsITreeViewModel.RunSpecs (fun () ->
        x.AsITreeViewModel.State <- x.Child.AsITreeViewModel.State
        completed ())
   
type AssembliesViewModel (assemblies : ObservableCollection<Assembly>, controller) =
  inherit TreeViewModel("", controller)
  
  let _children = 
    ObservableCollection<ITreeViewModel>(assemblies |> Seq.map (fun a -> AssemblyViewModel(a, controller) :> ITreeViewModel))
  
  interface ITreeViewModel with  
    override x.Children with get () = _children.AsEnumerable() |> Seq.cast<ITreeViewModel>
    override x.Add child = 
      _children.Add child 
      x.OnPropertyChanged("Children")
      