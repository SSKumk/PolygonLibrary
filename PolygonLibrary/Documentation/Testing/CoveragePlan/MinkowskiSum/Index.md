# MinkowskiSum

## Scope

Класс [`MinkowskiSum.cs`](../../../../CGLibrary/Algorithms/Polyhedra/MinkowskiSum.cs) реализует сумму Минковского на уровне точек и политопов.

В активный слой текущего этапа входят:

- `AlgSumPoints`;
- `ByConvexHull`;
- `BySandipDas` на прямых 2D и одном 3D-контрольном случае;
- `onlyHrep`-ветка `BySandipDas`.

Вне активного слоя остаются:

- тяжёлые 4D/5D regression-сценарии;
- большие примеры из статьи и picture-based кейсы;
- exhaustive-переборы из legacy-набора.

## Topics

- [`BasicCases.md`](BasicCases.md) - `AlgSumPoints` и прямые 2D кейсы.
- [`HighDimensionalAndOnlyHrep.md`](HighDimensionalAndOnlyHrep.md) - 3D-контроль и `onlyHrep`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Basic Cases | `x` | Перенесены ключевые 2D-сценарии из legacy. |
| High-Dimensional and onlyHrep | `x` | Оставлен один 3D-контроль и прямая `onlyHrep`-ветка. |
| Large Regression Sets | `-` | Вне активного слоя текущей миграции. |

## Existing Test Sources

- [`MinkowskiSumTests.cs`](../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiSumTests.cs)

## Notes

- Старый файл содержал и прямые unit-like кейсы, и очень тяжёлые размерностные regression-наборы. В новой структуре оставлен компактный обязательный слой на сам алгоритм.


