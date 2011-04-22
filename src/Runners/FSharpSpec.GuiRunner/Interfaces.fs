namespace FSharpSpec.GuiRunner

open System
open System.Collections.ObjectModel
open System.Windows.Threading
open FSharpSpec

type ITreeViewModel =
  abstract member Name : string
  abstract member Children : ObservableCollection<ITreeViewModel> 
  abstract member State : SpecState with get, set
  abstract member SpecsRunResult : ObservableCollection<SpecRunResultViewModel> with get
  abstract member ResetResults : unit -> unit
  abstract member ResolveSpecs : unit -> unit
  abstract member RunSpecs : (unit -> unit) -> unit

type IGuiController =
  abstract member Selected : ITreeViewModel -> unit
  abstract member AddAssembly : string -> unit
  abstract member RemoveAssembly : string -> unit
  abstract member ResetResults : unit -> unit
  abstract member RegisterSpecs : seq<(string * SpecDelegate)> -> unit
  abstract member ReportResult : AssertionResult -> unit
   

type IGuiRunnerViewModel =
  abstract UpdateSpecsRunResult : seq<SpecRunResultViewModel> -> unit
  abstract member Root : ITreeViewModel with get
  abstract member ResetSpecs : unit -> unit
  abstract member RegisterSpecs : int -> unit
  abstract member PassedSpec : unit -> unit
  abstract member InconclusiveSpec : unit -> unit
  abstract member PendingSpec : unit -> unit
  abstract member FailedSpec : unit -> unit

[<AutoOpen>]
module ITreeViewModelFunctions =
  let resetResolveAndRunSpecs (x : ITreeViewModel) =    
    x.ResetResults ()
   
    Dispatcher.CurrentDispatcher.BeginInvoke(
      DispatcherPriority.DataBind, 
      (Action(fun () ->  
        x.ResolveSpecs () 
        x.RunSpecs (fun () -> ()) 
         )) )
    |> ignore
    