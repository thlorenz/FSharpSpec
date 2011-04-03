[<AutoOpen>]
module SlideShow

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Media.Animation
open System.Linq
open System.IO


type TextStyle = Small | Normal | MediumLarge | Large | Headline
type RevealStyle = None | FromLeft | FromRight | Expand
type Transition = FadeOut | FadeIn 
type VertAlignment = Top | Middle |Bottom 
type HoriAlignment = Left | Center| Right 

type StyleSnapshot = 
  {  RevealStyle : RevealStyle;
     TextStyle : TextStyle; 
     LeftMargin : int;
     VertAlignment : VertAlignment; 
     HoriAlignment : HoriAlignment }

let doubleAnimation fromValue toValue duration = 
  DoubleAnimation(fromValue, toValue, Duration(TimeSpan.FromSeconds duration), FillBehavior.HoldEnd)

let animateOpacity fromValue toValue duration  onCompleted (element : FrameworkElement) = 
  let animation = doubleAnimation fromValue toValue duration
  animation.Completed.Add onCompleted
  element.BeginAnimation(FrameworkElement.OpacityProperty, animation)

let slideInFrom start (elem : FrameworkElement) = 
  let ease = QuinticEase()
  ease.EasingMode <- EasingMode.EaseOut
  let animation = doubleAnimation start 0.0 0.5
  animation.EasingFunction <- ease
  let trans =  TranslateTransform()
  elem.RenderTransform <- trans
  trans.BeginAnimation (TranslateTransform.XProperty, animation)

let expand toValue (elem : FrameworkElement) =
  let ease = CubicEase()
  ease.EasingMode <- EasingMode.EaseInOut
  let animation = doubleAnimation 0.0 toValue 5.0
  animation.EasingFunction <- ease
  elem.BeginAnimation(FrameworkElement.HeightProperty, animation)


type SlideShow (slidesPath) =
  let panel = 
    StackPanel(Orientation = Orientation.Vertical)
 
  let border = 
    Border(
      Child = panel, 
      Background = Brushes.DarkBlue,
      BorderBrush = Brushes.Blue,
      CornerRadius = CornerRadius(5.0))

  let win = 
    Window(
      Content = border,
      WindowState = WindowState.Normal, 
      AllowsTransparency = true, 
      Background = Brushes.Transparent,
      WindowStyle = WindowStyle.None,
      ResizeMode = ResizeMode.CanResizeWithGrip,
      Topmost = true,
      Width = 800.0,
      Height = 500.0,
      Left = 800.0,
      Top = 100.0)
  
  let mutable revealStyle = None
  let mutable textStyle = Normal
  let mutable leftMargin = 0
  let mutable vertAlignment = Middle
  let mutable horiAlignment = Center
  
  let mutable styleSnapShot = 
    { RevealStyle = revealStyle;
      TextStyle = textStyle;
      LeftMargin = leftMargin;
      VertAlignment = vertAlignment;
      HoriAlignment = horiAlignment}

  let fadeOut onCompleted = win |> animateOpacity 1.0 0.0 0.5 onCompleted
  let fadeIn onCompleted  = win |> animateOpacity 0.0 1.0 0.5 onCompleted

  do
    win.Show()
    win.MouseLeftButtonDown.Add (fun _ -> win.DragMove())

  member x.Panel with get () = panel
  member x.Slides with get () = x.Panel.Children
  member x.End = win.Close()
  member x.Hide () = win.Visibility <- Visibility.Collapsed
  member x.Show () = win.Visibility <- Visibility.Visible

  member x.SaveStyle () = 
    styleSnapShot <- {  RevealStyle = x.RevealStyle;
                        TextStyle = x.TextStyle;
                        LeftMargin = x.LeftMargin;
                        VertAlignment = vertAlignment;
                        HoriAlignment = horiAlignment}

  member x.LoadStyle () =
    x.RevealStyle <- styleSnapShot.RevealStyle 
    x.TextStyle <- styleSnapShot.TextStyle
    x.LeftMargin <- styleSnapShot.LeftMargin
    vertAlignment <- styleSnapShot.VertAlignment
    horiAlignment <- styleSnapShot.HoriAlignment
  
  member x.ResetStyle () =
    x.RevealStyle <- None
    x.TextStyle <- Normal
    x.LeftMargin <- 0
    vertAlignment <- Middle
    horiAlignment <- Center
   
  member x.Transition transition onCompleted =
      match transition with
      | FadeOut     -> fadeOut onCompleted
      | FadeIn      -> fadeIn  onCompleted
 
  member x.AlignH alignment =
    horiAlignment <- alignment
    match alignment with 
    | Left     -> panel.HorizontalAlignment <- HorizontalAlignment.Left
    | Center   -> panel.HorizontalAlignment <- HorizontalAlignment.Center
    | Right    -> panel.HorizontalAlignment <- HorizontalAlignment.Right

  member x.AlignV alignment =
    vertAlignment <- alignment
    match alignment with 
    | Top     -> panel.VerticalAlignment <- VerticalAlignment.Top
    | Middle -> panel.VerticalAlignment <- VerticalAlignment.Center
    | Bottom  -> panel.VerticalAlignment <- VerticalAlignment.Bottom
  
  member x.RevealStyle with get () = revealStyle and set v = revealStyle <- v
  member x.TextStyle with get () = textStyle and set v  = textStyle <- v
  member x.LeftMargin with get () = leftMargin and set v = leftMargin <- v

  member x.ReadFile fileName = File.ReadAllText(String.Concat(slidesPath, fileName))

  member x.WriteUsing (leftMargin : int) (textStyle : TextStyle) text =
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
        Margin = Thickness(leftMarginf, 0.0,0.0,0.0),
        HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Stretch,
        Background = Brushes.Transparent,
        Foreground = Brushes.White,
        Padding = Thickness(fontSize / 10.0),
        TextWrapping = TextWrapping.WrapWithOverflow)

    match x.RevealStyle with
    | None      -> x.Slides.Add tb 
    | FromLeft  -> tb |> slideInFrom (-win.ActualWidth / 2.0)
                   x.Slides.Add tb 
    | FromRight -> tb |> slideInFrom (win.ActualWidth * 1.5)
                   x.Slides.Add tb
    | Expand    -> tb |> expand win.ActualHeight
                   x.Slides.Add tb
    |> ignore

    win.InvalidateVisual()
  
  member x.Write = x.WriteUsing x.LeftMargin x.TextStyle

  member x.TransitionText nextText onCompleted =
    fadeOut (fun _ -> x.Slides.Clear(); nextText |> x.Write; fadeIn onCompleted )
  

let transitionHeadLine (show : SlideShow) text = 
  show.SaveStyle ()
  show.AlignV Middle
  show.AlignH Center
  show.LeftMargin <- 0
  show.RevealStyle <- None
  show.TextStyle <- Headline
  show.TransitionText text (fun _ -> show.LoadStyle ())

let expandNormal alignment (show : SlideShow) text = 
  show.SaveStyle ()
  show.AlignV alignment
  show.TextStyle <- Normal
  show.RevealStyle <- Expand
  show.Write text

let clear (show : SlideShow) = show.Slides.Clear()