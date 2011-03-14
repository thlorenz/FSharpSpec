namespace FSharpSpec.GuiRunner

type MainViewModel (contextRoot, specsRunResult) =
  member x.Root with get () = contextRoot
  member x.SpecsRunResult with get () = specsRunResult

