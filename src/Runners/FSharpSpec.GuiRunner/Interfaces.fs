namespace FSharpSpec.GuiRunner

open System.Collections.ObjectModel

type ITreeViewModel =
  abstract member Name : string
  abstract member Children : seq<ITreeViewModel>
  abstract member State : SpecState with get, set
  abstract member SpecsRunResult : seq<SpecRunResultViewModel> with get, set
  abstract member Reset : unit -> unit

type IGuiController =
  abstract member Selected : ITreeViewModel -> unit
  abstract member SpecRunResults : ObservableCollection<SpecRunResultViewModel> with get