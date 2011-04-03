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


let panel = StackPanel(Orientation = Orientation.Vertical)
let border = new Border(Child = panel, Background = Brushes.DarkBlue, BorderBrush = Brushes.Black, CornerRadius = CornerRadius(5.0), BorderThickness = Thickness(2.0))
let win = 
  Window(
      Content = border,
      WindowState = WindowState.Normal, 
      AllowsTransparency = true, 
      Background = Brushes.Transparent,
      WindowStyle = WindowStyle.None,
      Topmost = true,
      Width = 500.0,
      Height = 300.0,
      Left = 600.0,
      Top = 20.0)
win.MouseLeftButtonDown.Subscribe(fun _ -> win.DragMove())  
win.Show()

let animation fromValue toValue duration = 
  DoubleAnimation(fromValue, toValue, Duration(TimeSpan.FromSeconds duration), FillBehavior.HoldEnd)

let slideAnimation (elem : FrameworkElement) =
  let cubicEase = CubicEase()
  cubicEase.EasingMode <- EasingMode.EaseOut
  let an = animation -(elem.ActualWidth) 0.0 0.5
  an.EasingFunction <- cubicEase
  an

panel.Children.Clear()
let slideFromLeft leftMargin text = 
  let tb = TextBlock(Text = text, Foreground = Brushes.White)
  tb.Margin <- Thickness(leftMargin, 0.0, 0.0, 0.0)
  tb.RenderTransform <- TranslateTransform()
  
  let trans = tb.RenderTransform :?> TranslateTransform
  panel.Children.Add tb |> ignore
  
  printfn "starting animation"
  trans.BeginAnimation (TranslateTransform.XProperty, slideAnimation tb)

slideFromLeft 200.0 "Why FSharp?" 
