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
 
type SpecContainerViewModel (specs : SpecInfo, context, specRunResults) =
  inherit TreeViewModel (specs.Name |> removeLeadingGet, specRunResults)
  
  let _instantiatedSpecs = ObservableCollection<SpecViewModel>([ SpecViewModel.Dummy ])
  let getFullNameOfSpec = getFullSpecName context specs.Method.Name
  

  let extractSpecs () = 
    match _instantiatedSpecs with
    | xs when xs.Count > 0 && (not _instantiatedSpecs.[0].IsDummySpec)  -> ()
    | otherwise             ->
        _instantiatedSpecs.Clear()
        let buildContextAndResolveSpecs () =
          let instantiatedContext = context.Clazz |> instantiate
          specs.Method.Invoke(instantiatedContext, null) :?> (string * SpecDelegate) list
        
        buildContextAndResolveSpecs ()
        |> List.iter (fun spec -> _instantiatedSpecs.Add <| SpecViewModel(spec, specRunResults, buildContextAndResolveSpecs, getFullNameOfSpec)) 

  member x.runSpecs = 
    _instantiatedSpecs |> Seq.iter (fun c -> c.State <- NotRunYet)
    extractSpecs ()
    _instantiatedSpecs |> Seq.iter (fun s -> s.runSpec)
    x.State <- _instantiatedSpecs.Cast<TreeViewModel>() |> TreeViewModel.aggregatedResults 

  member private x._runSpecsCommand = ActionCommand ((fun _ -> x.runSpecs), (fun _ -> true))

  override x.Name with get() =  specs.Name |> removeLeadingGet
  override x.Children with get() = _instantiatedSpecs.AsEnumerable() |> Seq.cast<TreeViewModel>

  override x.OnExpanded () = extractSpecs ()

  member x.RunSpecsCommand with get () = x._runSpecsCommand :> ICommand

