namespace FSharpSpec.GuiRunner

open System
open System.Windows.Threading
open FSharpSpec

type ITreeViewModel =
  abstract member Name : string
  abstract member Children : seq<ITreeViewModel>
  abstract member State : SpecState with get, set
  abstract member SpecsRunResult : seq<SpecRunResultViewModel> with get, set
  abstract member Add : ITreeViewModel -> unit
  abstract member ResetResults : unit -> unit
  abstract member ResolveSpecs : unit -> unit
  abstract member RunSpecs : unit -> unit

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

[<AutoOpen>]
module ITreeViewModelFunctions =
  let resetResolveAndRunSpecs (x : ITreeViewModel) =    
    x.ResetResults ()
    
    Dispatcher.CurrentDispatcher.BeginInvoke(
      DispatcherPriority.SystemIdle, 
      (Action(fun () ->
        x.ResolveSpecs () 
        x.RunSpecs ())))
    |> ignore
    