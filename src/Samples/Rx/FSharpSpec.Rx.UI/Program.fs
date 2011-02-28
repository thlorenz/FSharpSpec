// Learn more about F# at http://fsharp.net

open System.Windows
open System
open System.Linq
open System.Diagnostics

open LoginManager

module main =
  
  type App = class
    inherit Application
   
    // No constructor
    new () = {}
   
    override this.OnStartup (args:StartupEventArgs) =
      base.OnStartup(args)
     
     // ShowLogin
    end
  
  [<STAThread()>]
  do 
    let app =  new App() in
    app.Run() |> ignore
