module FSharpSpecUnitTestProvider

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor
open System.Collections.Generic

open JetBrains.Application
open JetBrains.CommonControls
open JetBrains.Metadata.Reader.API
open JetBrains.ProjectModel
open JetBrains.ReSharper.Psi
open JetBrains.ReSharper.Psi.Tree
open JetBrains.ReSharper.TaskRunnerFramework
open JetBrains.ReSharper.UnitTestFramework
open JetBrains.ReSharper.UnitTestFramework.UI
open JetBrains.ReSharper.UnitTestExplorer
open JetBrains.TreeModels
open JetBrains.UI.TreeView
open JetBrains.Util

open System.Diagnostics

[<UnitTestProvider>]
type FSharpSpecUnitTestProvider() =
    do
      Debug.Listeners.Add(new DefaultTraceListener()) |> ignore
    
    let ProviderId = "FSharpSpec"
    
    interface IUnitTestProvider with
        member x.ID with get() = ProviderId
        member x.Name with get() = ProviderId

        member x.Icon with get() = null
        member x.Serialize element = null
        member x.Deserialize(solution, elementString) = null
        member x.CompareUnitTestElements(unitTestElementX, unitTestElementY) = 1
        
        member x.ExploreAssembly(assembly, project, unitTestConsumer) = 
          assembly.Location |> ignore
          
          ()
        
        member x.ExploreExternal unitTestElementConsumer = () // No implementation needed
       
        member x.ExploreFile(psiFile, unitTestElementLocationConsumer, checkForInterrupt) = ()
        
        
        member x.ExploreSolution(solution, unitTestElementConsumer) = () // No implementation needed
       
        member x.GetCustomOptionsControl solution = null
        member x.GetTaskRunnerInfo() = new RemoteTaskRunnerInfo()
        member x.GetTaskSequence(unitTestElement, explicitElements) = new List<UnitTestTask>() :> IList<UnitTestTask>
        
        member x.IsElementOfKind(declaredElement : IDeclaredElement, unitTestElementKind : UnitTestElementKind) = 
          //let isSpecification (elem  : IDeclaredElement) =
          match unitTestElementKind with
          | UnitTestElementKind.Test          -> true
          | UnitTestElementKind.TestContainer -> true
          | otherwise                         -> false
        
        member x.IsElementOfKind(element : UnitTestElement, elementKind : UnitTestElementKind) = true
        
        member x.Present(element : UnitTestElement,item : IPresentableItem ,node : TreeModelNode , state :PresentationState) = ()
