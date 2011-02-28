namespace FSharpSpec.Rx.UI

module LoginManager =

  open System
  open FSharp.Interop
  open System.Windows
  open System.Windows.Controls
  open System.Windows.Input
  open System.Linq
  open System.Collections.Generic

  open System.Concurrency;
  open System.Threading;

  // UI
  let buttons = 
    let makeButton num = 
      let b = Button ( Content = num, Width = 50.0, Height = 50.0 )
  
      b.SetValue(Grid.RowProperty, (num - 1) / 3  )
      b.SetValue(Grid.ColumnProperty, (num - 1) % 3 )
      b.SetValue(TextBlock.FontSizeProperty, 20.0 )
      b

    [1 .. 9] 
    |> List.map makeButton

  let addChildTo (panel : Panel) child =
    panel.Children.Add child |> ignore

  let makeGrid  = 
  
    let addElements elements (pnl : Panel)  =
      elements
      |> List.iter (fun el -> addChildTo pnl el)
      pnl
  
    let rec addRowDefinitions rows (grid :Grid) = 
      [1 .. rows] |> List.iter(fun _ -> grid.RowDefinitions.Add (RowDefinition()))
      grid

    let addColumnDefinitions rows (grid :Grid) =
      [1 .. rows] |> List.iter(fun _ -> grid.ColumnDefinitions.Add (ColumnDefinition()))
      grid
  
    Grid( HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top )
    |> addRowDefinitions 3
    |> addColumnDefinitions 3
    |> addElements buttons

  let makeTextBlock = (fun () -> 
      TextBlock( 
            Width = 150.0, 
            Margin = Thickness(5.0),
            Background = Media.Brushes.AntiqueWhite, 
            HorizontalAlignment = HorizontalAlignment.Center)
      )

  let numTextBlock = makeTextBlock()
  let resultTextBlock = makeTextBlock()
  let stackPanel = StackPanel()
 
  let makeWindow =
    let pnl = stackPanel
    makeGrid |> addChildTo pnl
    numTextBlock|> addChildTo pnl
    resultTextBlock |> addChildTo pnl

    Window( Topmost = true, SizeToContent = SizeToContent.WidthAndHeight, Content = pnl )

  // end UI



  let toClickEventObservable (btn : Button) = btn.Click :> IObservable<_>

  open Obs

  let toPressedContent (buttons : seq<Button>) =
        buttons
        |> Seq.map toClickEventObservable
        |> mergeAllSeq
        |> map (fun e -> e.Source)
        |> ofType<Button>
        |> map (fun btn -> btn.Content.ToString())
 
  let keys =  buttons |> toPressedContent

  let detectCorrectKeypass keypresses delay (password : string) scheduler = 
    keypresses 
    |> bufferWithTimeOrCount delay password.Length scheduler
    |> map (fun (listStr : IList<string>) -> String.Join("", listStr.ToArray())) 
    |> filter ( fun guess -> guess <> "" )
    |> act (fun guess -> numTextBlock.Text <- guess.ToString())
    |> map ( fun guess -> guess = password )
    |> distinctUntilChanged 


  let password = "1234"
  let delay = TimeSpan.FromSeconds(4.0)
  let scheduler = Scheduler.Dispatcher

  let correctKeypasses = (detectCorrectKeypass keys delay password scheduler)

  let ShowLogin = 
    resultTextBlock.Text <- "Please enter your password."

    correctKeypasses
      .ObserveOnDispatcher()
      .Subscribe(fun (result : bool) -> 
                      match result with
                      | true      -> resultTextBlock.Text <- "Got it !!!"
                      | otherwise -> resultTextBlock.Text <- "Try again !!!"
                      )
      |> ignore
      
    let win = makeWindow
    win.Show()

