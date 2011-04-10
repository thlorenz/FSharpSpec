namespace FSharpSpec.GuiRunner


type ITreeViewModel =
  abstract member Name : string
  abstract member Children : seq<ITreeViewModel>
  abstract member State : SpecState with get, set
  abstract member SpecsRunResult : seq<SpecRunResultViewModel> with get, set
  abstract member Reset : unit -> unit
  abstract member Add : ITreeViewModel -> unit

type IGuiController =
  abstract member Selected : ITreeViewModel -> unit
  abstract member AddAssembly : string -> unit
  abstract member RemoveAssembly : string -> unit
   

type IGuiRunnerViewModel =
  abstract UpdateSpecsRunResult : seq<SpecRunResultViewModel> -> unit
  abstract member Root : ITreeViewModel with get