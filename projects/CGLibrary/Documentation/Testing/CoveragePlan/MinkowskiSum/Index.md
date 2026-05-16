# MinkowskiSum

## Scope

Класс [`MinkowskiSum.cs`](../../../../CGLibrary/Algorithms/Polyhedra/MinkowskiSum.cs) реализует сумму Минковского на уровне точек и политопов.

В активный слой текущего этапа входят:

- `AlgSumPoints`;
- `ByConvexHull`;
- `BySandipDas` на прямых 2D и одном 3D-контрольном случае;
- выбранные regression-кейсы `3D/4D` и один article-based `3D` пример;
- контроль коммутативности `BySandipDas`;
- `onlyHrep`-ветка `BySandipDas`.

Вне активного слоя остаются:

- самые тяжёлые 5D regression-сценарии;
- большая часть article-based и picture-based кейсов;
- exhaustive-переборы из legacy-набора.

## Topics

- [`BasicCases.md`](BasicCases.md) - `AlgSumPoints` и прямые 2D кейсы.
- [`HighDimensionalAndOnlyHrep.md`](HighDimensionalAndOnlyHrep.md) - 3D-контроль и `onlyHrep`.
- [`RegressionAndArticleCases.md`](RegressionAndArticleCases.md) - минимальный regression-слой, article-based пример и коммутативность.
- [`Stress.md`](Stress.md) - тяжёлые 5D representative-кейсы и инвариантности.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Basic Cases | `x` | Перенесены ключевые 2D-сценарии из legacy. |
| High-Dimensional and onlyHrep | `x` | Оставлен один 3D-контроль и прямая `onlyHrep`-ветка. |
| Regression and Article Cases | `x` | Перенесён минимальный нетривиальный regression-слой для `BySandipDas`. |
| Stress | `x` | Выделен отдельный тяжёлый 5D-слой для representative-случаев. |
| Large Regression Sets | `-` | Самые массовые combinatorial-переборы всё ещё вне активного слоя. |

## Existing Test Sources

- ``MinkowskiSumTests.cs``

## Notes

- Старый файл содержал и прямые unit-like кейсы, и очень тяжёлые размерностные regression-наборы. В новой структуре оставлен компактный обязательный слой на сам алгоритм.
