﻿namespace FSharpSpec.ResharperRunner

open FSharpSpec
open FSharpSpec.RunnerUtils
open SpecsExtractor

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



[<UnitTestProvider>]
type FSharpSpecUnitTestProvider() =
    let ProviderId = "FSharpSpec"
  //  let presenter = new Presenter()
  //  let _taskFactory = new UnitTestTaskFactory(ProviderId)
  //  let _unitTestElementComparer = new UnitTestElementComparer()
//    interface IUnitTestProvider with
//        member x.ID with get() = ProviderId
//        member x.Name with get() = ProviderId
//        member x.Icon with get() = null
//        member x.Serialize element = null
//        member x.Deserialize(solution, elementString) = null
//        member x.CompareUnitTestElements(unitTestElementX, unitTestElementY) = _unitTestElementComparer.Compare(x, y)
