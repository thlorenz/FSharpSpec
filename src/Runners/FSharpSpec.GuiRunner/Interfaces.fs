namespace FSharpSpec.GuiRunner


type ITreeViewModel =
  abstract member Name : string
  abstract member Children : seq<ITreeViewModel>
  abstract member State : SpecState with get, set
  abstract member SpecsRunResult : seq<SpecRunResultViewModel> with get, set
  abstract member Reset : unit -> unit

type IGuiController =
  abstract member Selected : ITreeViewModel -> unit

type IGuiRunnerViewModel =
  abstract UpdateSpecsRunResult : seq<SpecRunResultViewModel> -> unit
