#I @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\"
#r @"PresentationCore.dll"
#r "PresentationFramework.dll"
#r "WindowsBase.dll"
#r "System.Xaml"
#r "Microsoft.CSharp"

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Media.Animation
open System.Linq

type TextStyle = Small | Normal | MediumLarge | Large | Headline
type RevealStyle = None
type Transition = FadeOut | FadeIn
type Alignment = Top | Center| Bottom

let animateOpacity fromValue toValue duration  onCompleted (element : FrameworkElement)= 
   let animation = DoubleAnimation(fromValue, toValue, Duration(TimeSpan.FromSeconds duration), FillBehavior.Stop)
   animation.Completed.Add onCompleted
   element.BeginAnimation(FrameworkElement.OpacityProperty, animation)
   printfn "animation started"
   
type SlideShow () =
  let panel = StackPanel(Orientation = Orientation.Vertical)
  let win = 
    Window(
        Content = panel,
        WindowState = WindowState.Normal, 
        AllowsTransparency = true, 
        Background = Brushes.Transparent,
        WindowStyle = WindowStyle.None,
        Topmost = true,
        Width = 1000.0,
        Height = 800.0,
        Left = 400.0,
        Top = 100.0)
  

  let mutable revealStyle = None

  let fadeOut onCompleted = panel |> animateOpacity 1.0 0.0 0.5 onCompleted
  let fadeIn onCompleted  = panel |> animateOpacity 0.0 1.0 0.5 onCompleted

  do
    win.Show()
    win.MouseLeftButtonDown.Add (fun _ -> win.DragMove())
    panel.Width  <- win.ActualWidth
    panel.Height <- win.ActualHeight
    panel.VerticalAlignment <- VerticalAlignment.Center

  member x.Panel with get () = panel
  member x.Slides with get () = x.Panel.Children
  member x.End = win.Close()
  
  member x.Transition transition onCompleted =
      match transition with
      | FadeOut     -> fadeOut onCompleted
      | FadeIn      -> fadeIn  onCompleted
 
  member x.Align alignment =
    match alignment with 
    | Top     -> panel.VerticalAlignment <- VerticalAlignment.Top
    | Center  -> panel.VerticalAlignment <- VerticalAlignment.Center
    | Bottom  -> panel.VerticalAlignment <- VerticalAlignment.Bottom
  
  member x.RevealStyle with get () = revealStyle and set v = revealStyle <- v

  member x.Write (leftMargin : int) (textStyle : TextStyle) text =
    let leftMarginf = Convert.ToDouble leftMargin
    let fontSize, fontWeight =
      match textStyle with
      | Small       -> (12.0, FontWeights.Normal)
      | Normal      -> (20.0, FontWeights.Normal)
      | MediumLarge -> (35.0, FontWeights.Bold)
      | Large       -> (60.0, FontWeights.Bold)
      | Headline    -> (100.0,FontWeights.ExtraBold)
    
    let tb = 
      TextBlock(
        Text = text, 
        FontSize = fontSize, 
        FontWeight = fontWeight, 
        Margin = Thickness(leftMarginf , 0.0,0.0,0.0),
        HorizontalAlignment = HorizontalAlignment.Left,
        Background = Brushes.White,
        Padding = Thickness(fontSize / 10.0),
        TextWrapping = TextWrapping.Wrap)

    match x.RevealStyle with
    | None    -> x.Slides.Add tb
    
    |> ignore
  
    
let show = SlideShow()
let fadeOut = show.Transition FadeOut
let fadeIn  = show.Transition FadeIn

let transitionText nextText =
  fadeOut (fun _ -> show.Slides.Clear(); nextText |> show.Write 0 Headline; fadeIn (fun _ -> ()) )

"Why FSharpSpec"  |> show.Write 0 Headline
transitionText "What we have so far"
transitionText "NUnit, MSTest if you are old school"
transitionText "MSpec if you are more BDD oriented"
///show.End  
  