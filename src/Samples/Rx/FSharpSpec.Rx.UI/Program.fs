namespace FSharpSpec.Rx.UI

open System.Windows
open System
open System.Linq
open System.Diagnostics

module main =
  
  type App = class
    inherit Application
   
    // No constructor
    new () = {}
   
    override this.OnStartup (args:StartupEventArgs) =
      base.OnStartup(args)
     
      LoginManager.ShowLogin
    end
  
  [<STAThread()>]
  do 
    let app =  new App() in
    app.Run() |> ignore
