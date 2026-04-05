# GiftWrapping Migration Worklog

## Purpose

Временный рабочий файл для контроля переноса `GiftWrapping` в `Tests/DoubleGeometry`.

Использовать:
- когда нужно понять, что уже перенесено;
- когда нужно решить, что переносить следующим шагом;
- когда нужно не потерять legacy-сценарии до их окончательной архивации.

Не использовать:
- как постоянную документацию покрытия;
- как дублирование итогового `Index.md` и тематических coverage-файлов.

## Current State

Сейчас в новом слое уже есть:
- [`GiftWrappingTestData.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingTestData.cs)
- [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs)

Покрыто сейчас:
- пустой swarm;
- одиночная точка;
- линейный случай;
- `WrapVRep` для квадрата, тетраэдра и куба;
- `WrapFaceLattice` / `ConstructFL` для типовых `2D/3D` случаев.

Не покрыто как полноценный новый слой:
- invariance/regression-сценарии из legacy;
- heavy random/stress слой;
- часть `4D+` hand-crafted случаев.

## Target Structure

Целевая структура тестов:
- `GiftWrappingTestData.cs`
- `GiftWrappingBasicTests.cs`
- `GiftWrappingRegressionTests.cs`
- `GiftWrappingStressTests.cs`

Целевая структура coverage-документации:
- `Index.md`
- `BasicCases.md`
- `RegressionAndInvariance.md`
- `Stress.md`

Принцип:
- `Basic` — прямой контракт и базовые ветки алгоритма;
- `Regression` — hand-crafted и исторически важные сценарии;
- `Stress` — тяжёлые, seed-based и генераторные серии.

## Legacy Sources

Основной legacy-источник:
- [`GW_Tests.cs`](../../../../Tests/Double-Tests/GW_hDTests/GW_Tests.cs)

Параллельный источник для проверки смыслового соответствия:
- [`GW_Tests.cs`](../../../../Tests/DoubleDouble-Tests/GW_hDTests/GW_Tests.cs)

## Migration Map

### Already Covered

- `Constructor_EmptySwarm_ThrowsArgumentException`
- `SinglePointSwarm_ProducesSamePointInVrepAndSingleNodeFaceLattice`
- `LineSwarmInHigherDimensionalSpace_IsReducedToSegmentEndpoints`
- `WrapVRep_ForSquareWithInnerPoints_ReturnsOnlyHullVertices`
- `WrapVRep_ForTetrahedronFollowsSimplexShortcut`
- `WrapVRep_ForCubeWithInnerPoints_ReturnsCubeVertices`
- `WrapFaceLattice_ForSquareWithInnerPoints_BuildsExpectedFaceCounts`
- `ConstructFL_FromWrappedCube_PreservesCubeVertexSetAndFaceCounts`

### Candidate Regression Layer

Перенести в первую очередь:
- `Cube3D` - done
- `Cube3D_Rotated_Z45` - done
- `Cube3D_Rotated` - done
- `Cube3D_Shifted` - done
- `Cube3D_Rotated_Shifted` - done
- `Cube3D_withInnerPoints_On_1D` - done
- `Cube3D_withInnerPoints_On_2D` - done
- `Cube3D_withInnerPoints_On_3D` - done
- `Cube3D_withInnerPoints_On_1D_2D` - done
- `Cube3D_withInnerPoints_On_2D_3D` - done
- `Cube3D_withInnerPoints_On_1D_2D_3D` - done
- `Cube3D_Shuffled` - done
- `Cube4D_Shuffled` - done
- `Simplex3D_Shuffled` - done
- `Simplex4D_Shuffled` - done
- `Cube4D_withInnerPoints_On_1D` - done
- `Cube4D_withInnerPoints_On_2D` - done
- `Cube4D_withInnerPoints_On_3D` - done
- `Cube4D_withInnerPoints_On_1D_2D` - done
- `Cube4D_withInnerPoints_On_2D_3D` - done
- `Cube4D_withInnerPoints_On_1D_2D_3D` - done
- `Cube4D_withInnerPoints_On_1D_2D_3D_4D` - done
- `Simplex4D_1DEdge_2DNeighborsPointsTest` - done
- `Simplex4D_InnerPointsIn_1D` - done

### Candidate Stress Layer

Оставить в stress:
- `AllCubes3D_TestRND`
- `AllCubes4D_TestRND`
- `AllCubes5D_TestRND`
- `AllCubes6D_TestRND`
- random/simplex generators для `3D+`
- большие seed-based сценарии с проверкой `Vrep/Hrep/FLrep`

## Open Questions

- Для `Regression` проверять только `Vrep`, или сразу возвращать legacy-проверки `Hrep` и `FLrep.NumberOfKFaces`?
- Какой объём `Stress` считать активным слоем, а какой сразу отправлять в архив?
- Нужно ли отдельно фиксировать shuffle-invariance как самостоятельную подтему?

## Current Observations

- В [`GiftWrapping.cs`](../../../../CGLibrary/Algorithms/Polyhedra/GiftWrapping/GiftWrapping.cs) есть debug-хвост:
  - `Console.WriteLine($"GW.Rem =  {toRemove.Count}.\t");`
- В [`DoubleGeometryMigrationOrder.md`](../../../DoubleGeometryMigrationOrder.md) статус `GiftWrapping = closed` выглядит преждевременным относительно реального объёма legacy-набора.
- Legacy `GW_Tests` в основном проверяет `ConvexPolytop.CreateFromPoints(..., true)`, а не только прямой API `GiftWrapping`.

## Next Steps

1. Перенести минимальный regression-набор без изменения алгоритма.
2. Синхронизировать coverage-документацию под новый regression-слой.
3. Отдельно выделить `Stress`.
4. После завершения переноса архивировать legacy `GW_Tests`.

## User Notes

Пиши здесь свои мысли по очередным идеям, что переносить или что оставлять за бортом.

- 
