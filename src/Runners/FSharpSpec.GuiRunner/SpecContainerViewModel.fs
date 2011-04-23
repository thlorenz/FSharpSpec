namespace FSharpSpec.GuiRunner

open System.Diagnostics
open System.Reflection
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

  let tryGetInstantiatedContext context =
    try
      let instantiatedContext = context.Clazz |> instantiate
      (Some(instantiatedContext), null)
    with
    | ex -> printfn "%A" ex
            (None, ex)
  
  let tryExtractSpecs instantiatedContext (methodInfo : MethodInfo) =
    try
      let result = methodInfo.Invoke(instantiatedContext, null) 
      (Some(result), null)
    with 
    | ex -> 
            (None, ex)
 
  let buildContextAndResolveSpecs () =
         
    hookAssemblyResolve context.Clazz.Assembly
          
    match tryGetInstantiatedContext context with
    | (None, ex)      -> (None, ex)
    | (ictx, _)       -> 
          
      match tryExtractSpecs ictx.Value specs.Method with
      | (None, ex)      -> (None, ex)
      | (specListOpt, _)   ->
              
        let specList = specListOpt.Value
        let resolvedSpecs = 
          match specList.GetType() with
          | ty when ty  = typeof<Lazy<(string * SpecDelegate)> list>  -> specList :?> Lazy<(string * SpecDelegate)> list
                                                                         |> List.map(fun (r : Lazy<(string * SpecDelegate)>) -> r.Value) 
          | ty when ty = typeof<(string * SpecDelegate)>              -> [specList :?> (string * SpecDelegate)]
          | _                                                         ->  specList :?> (string * SpecDelegate) list
      
        (Some(resolvedSpecs), null)

  let extractSpecs () = 
    match children with
    | xs when xs.Count > 0 && (not (children.[0] :?> SpecViewModel).IsDummySpec)  -> ()
    | otherwise             ->
        children.Clear()
        
        match buildContextAndResolveSpecs () with
        | (None, ex)              -> Debug.WriteLine(sprintf "%A" ex)
        | (resolvedSpecs, _)      -> resolvedSpecs.Value
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

  

