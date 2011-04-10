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
  
  let _instantiatedSpecs = ObservableCollection<SpecViewModel>([ SpecViewModel.Dummy ])
  let getFullNameOfSpec = getFullSpecName context specs.Method.Name

  let extractSpecs () = 
    match _instantiatedSpecs with
    | xs when xs.Count > 0 && (not _instantiatedSpecs.[0].IsDummySpec)  -> ()
    | otherwise             ->
        _instantiatedSpecs.Clear()
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
        |> List.iter (fun spec -> _instantiatedSpecs.Add <| SpecViewModel(spec, controller, buildContextAndResolveSpecs, getFullNameOfSpec)) 

  member x.runSpecs = 
    x.AsITreeViewModel.Children |> Seq.iter (fun c -> c.State <- NotRunYet)
    extractSpecs ()
    _instantiatedSpecs |> Seq.iter (fun s -> s.runSpec)
    x.AsITreeViewModel.State <- x.aggregateStates
    x.AsITreeViewModel.SpecsRunResult <- x.aggregateResults

  member private x._runSpecsCommand = ActionCommand ((fun _ -> x.runSpecs; x.IsSelected <- true), (fun _ -> true))
  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

  interface ITreeViewModel with
    override x.Name with get() =  specs.Name |> removeLeadingGet
    override x.Children with get() = _instantiatedSpecs.AsEnumerable() |> Seq.cast<ITreeViewModel>

  override x.OnExpanded () = extractSpecs ()

  

