# High Dimensional And OnlyHrep

## Scope

Сценарии:

- 3D-контроль `BySandipDas` против `ByConvexHull`;
- `onlyHrep`-ветка.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKS-H-000 | x | `AlgSumPoints` для 3D-куба схлопывает повторные попарные суммы, но не теряет уникальные вершины результата. | [`MinkowskiSumHighDimensionalTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumHighDimensionalTests.cs#L9) |
| MKS-H-001 | x | `BySandipDas` для 3D-куба и самого себя совпадает с `ByConvexHull`. | [`MinkowskiSumHighDimensionalTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumHighDimensionalTests.cs#L9) |
| MKS-H-002 | x | `BySandipDas(..., onlyHrep: true)` возвращает эквивалентный `Hrep`-политоп. | [`MinkowskiSumHighDimensionalTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumHighDimensionalTests.cs#L20) |

## Existing Tests

- Из legacy сохранён принцип сравнения `BySandipDas` с `ByConvexHull`, но без тяжёлых многомерных переборов.

## Gaps

- 4D/5D regression-сценарии и picture-based кейсы остаются вне активного слоя.


