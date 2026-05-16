# AffineBasis Factories And Mutation

## Scope

Этот файл покрывает фабрики `AffineBasis` и mutable-операцию `AddVector`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `AFB-FAC-001` | `x` | `FromVectors(origin, vectors)` строит аффинный базис с заданным началом и направляющими векторами. | [`AffineBasisFactoriesAndMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisFactoriesAndMutationTests.cs) |
| `AFB-FAC-002` | `x` | `FromPoints(origin, points)` строит аффинный базис по точкам, лежащим в одном аффинном пространстве. | [`AffineBasisFactoriesAndMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisFactoriesAndMutationTests.cs) |
| `AFB-FAC-003` | `x` | `GenAffineBasis(spaceDim, subSpaceDim)` создаёт аффинный базис нужных размерностей. | [`AffineBasisFactoriesAndMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisFactoriesAndMutationTests.cs) |
| `AFB-MUT-001` | `x` | `AffineBasisMutable.AddVector` добавляет новый независимый вектор в линейную часть. | [`AffineBasisFactoriesAndMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisFactoriesAndMutationTests.cs) |
| `AFB-MUT-002` | `x` | `AffineBasisMutable.AddVector` отвергает зависимый вектор и не меняет размерность подпространства. | [`AffineBasisFactoriesAndMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisFactoriesAndMutationTests.cs) |
| `AFB-MUT-003` | `x` | После успешного `AddVector` проекционная матрица линейной части пересчитывается. | [`AffineBasisFactoriesAndMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisFactoriesAndMutationTests.cs) |

## Gaps

- Нет.

## Notes

- `AddVector` принимает именно вектор направления, а не точку аффинного пространства.
