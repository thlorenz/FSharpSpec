namespace FSharpSpec.GuiRunner

open System
open System.IO
open System.Linq
open System.Diagnostics
open System.Collections.ObjectModel
open System.Reflection

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
  
  let loadEmbeddedFile resourcePath =
    let asm = Assembly.GetExecutingAssembly()
    let stream = asm.GetManifestResourceStream(resourcePath)
    match stream with
    | null        -> failwith "Unable to find resource %s" resourcePath
    | s           -> use sr = new StreamReader(s)
                     sr.ReadToEnd()
  
//  let path = @"C:\dev\FSharp\FSharpSpec\src\Samples\FSharpSpec.FSharpSampleSpecs\bin\Debug\FSharpSpec.FSharpSampleSpecs.dll"
//  let asm = path |> getAssembly
  
  type App = class
    inherit Application
   
    new () = {}
   
    override this.OnStartup (args:StartupEventArgs) =
      base.OnStartup(args)
      
      let controller = GuiController() 
      let specsRunResult = SpecsRunResult()
      let asmRoot = AssembliesViewModel(new ObservableCollection<Assembly>(), controller :> IGuiController)
      
      let guiRunnerViewModel = GuiRunnerViewModel (asmRoot, controller :> IGuiController)
      controller.GuiRunnerViewModel <- guiRunnerViewModel :> IGuiRunnerViewModel

      let loadGuiRunnerView (resourcePath : string) =
        let xamlDoc = loadEmbeddedFile resourcePath
        XamlReader.Parse (xamlDoc.ToString()) :?> UserControl

      let view () = 
        let userControl = loadGuiRunnerView "GuiRunnerView.xaml" 
        userControl.DataContext <- guiRunnerViewModel
        userControl

      let win = Window( Width=700.0, Height = 600.0, Content = view (), Topmost = false, Title = "FSharpSpec Gui Runner")
      win.Show()
      
    end
  
  [<STAThread()>]
  do 
    let app =  new App() in
    app.Run() |> ignore
