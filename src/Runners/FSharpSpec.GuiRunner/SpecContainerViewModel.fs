namespace FSharpSpec.GuiRunner

open System.Diagnostics
open System.Windows.Input

open System.Collections.ObjectModel
open System.Collections.Generic

open System.Linq

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open SpecsRunnerUtils  
 
type SpecContainerViewModel (specs : SpecInfo, context, controller) =
  inherit TreeViewModel (specs.Name |> removeLeadingGet, controller)
  do base.Children.Add SpecViewModel.Dummy
  
  let getFullNameOfSpec = getFullSpecName context specs.Method.Name
  let children = base.Children

  let extractSpecs () = 
    match children with
    | xs when xs.Count > 0 && (not (children.[0] :?> SpecViewModel).IsDummySpec)  -> ()
    | otherwise             ->
        children.Clear()
        let buildContextAndResolveSpecs () =
         
          hookAssemblyResolve context.Clazz.Assembly
          
          let instantiatedContext = context.Clazz |> instantiate
          let result = specs.Method.Invoke(instantiatedContext, null) 
          match result.GetType() with
          | ty when ty  = typeof<Lazy<(string * SpecDelegate)> list>  -> result :?> Lazy<(string * SpecDelegate)> list
                                                                         |> List.map(fun (r : Lazy<(string * SpecDelegate)>) -> r.Value) 
          | ty when ty = typeof<(string * SpecDelegate)>              -> [result :?> (string * SpecDelegate)]
          | _                                                         ->  result :?> (string * SpecDelegate) list

        
        buildContextAndResolveSpecs ()
        |> List.iter (fun spec -> children.Add <| SpecViewModel(spec, controller, buildContextAndResolveSpecs, getFullNameOfSpec)) 
    


  let runSpecs completed = children |> Seq.iter (fun s -> (s :?> SpecViewModel).runSpec completed)
    
  member private x._runSpecsCommand = 
    ActionCommand ((fun _ -> 
      x.AsITreeViewModel |> resetResolveAndRunSpecs 
      x.IsSelected <- true), 
      (fun _ -> true))
  
  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  interface ITreeViewModel with
    override x.Name with get() =  specs.Name |> removeLeadingGet
    override x.ResolveSpecs () = 
      extractSpecs ()
      base.Children |> Seq.map (fun vm -> (vm :?> SpecViewModel).Spec) |> controller.RegisterSpecs

    override x.RunSpecs completed = 
      runSpecs (fun () ->
        x.AsITreeViewModel.State <- x.aggregateStates
        x.AsITreeViewModel.SpecsRunResult.Clear()
        x.aggregateResults |> Seq.iter(fun r -> x.AsITreeViewModel.SpecsRunResult.Add r)
        completed ())

  override x.OnExpanded () = extractSpecs ()

  

