namespace FSharpSpec.GuiRunner

open System
open System.IO
open System.Linq
open System.Diagnostics

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils



open System.Windows
open System.Windows.Controls
open System.Windows.Input

open System.Windows.Markup
open System.Windows.Media
open System.Xml

open FSharpSpec.GuiRunner



module main =
  let path = @"C:\dev\FSharp\FSharpSpec\src\FSharpSpec.Specs\bin\Debug\FSharpSpec.Specs.dll"

  let asm = path |> getAssembly
  let allContexts = asm |> getAllContexts
  let getName (asmFullName : string) = (asmFullName.Split(',')).[0]
  
  type App = class
    inherit Application
   
    new () = {}
   
    override this.OnStartup (args:StartupEventArgs) =
      base.OnStartup(args)
      
      let tree = allContexts |> getContextTreeOfContexts
      let asmRoot = ContextViewModel(tree)

      let loadGuiRunnerView (fileName : string) =
        let reader = XmlReader.Create fileName
        XamlReader.Load reader :?> UserControl

      let view () = 
        let userControl = loadGuiRunnerView @"C:\dev\FSharp\FSharpSpec\src\Runners\FSharpSpec.GuiRunner\GuiRunnerView.xaml" 
        userControl.DataContext <- asmRoot
        userControl

      let win = Window( Topmost = true, Width=700.0, Height = 600.0, Content = view ())
      win.Show()
      
    end
  
  [<STAThread()>]
  do 
    let app =  new App() in
    app.Run() |> ignore
