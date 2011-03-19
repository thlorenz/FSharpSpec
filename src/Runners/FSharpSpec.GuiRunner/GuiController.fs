namespace FSharpSpec.GuiRunner

type GuiController () =
  interface IGuiController with
    override x.Selected t = ()  

