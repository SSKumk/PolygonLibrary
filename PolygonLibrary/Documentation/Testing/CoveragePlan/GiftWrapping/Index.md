# GiftWrapping

## Scope

Класс [`GiftWrapping.cs`](../../../../CGLibrary/Algorithms/Polyhedra/GiftWrapping/GiftWrapping.cs) реализует построение выпуклой оболочки и `FaceLattice` по swarm of points.

В активный слой текущего этапа входят:

- базовый прямой контракт алгоритма;
- интеграционные regression-сценарии через `ConvexPolytop.CreateFromPoints(..., true)`;
- representative stress-наборы по кубам, симплексам и fixed-seed random simplices.

## Topics

- [`BasicCases.md`](BasicCases.md) - базовый прямой контракт `GiftWrapping`.
- [`RegressionAndInvariance.md`](RegressionAndInvariance.md) - интеграционные regression-сценарии через `CreateFromPoints(..., true)`.
- [`Stress.md`](Stress.md) - representative stress-наборы по кубам, симплексам и fixed-seed random simplices.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Basic Cases | `x` | Закрыты пустой, точечный, линейный, affine-reduction и типовые `2D/3D` сценарии. |
| Regression and Invariance | `x` | Перенесены `Cube3D/Cube4D`, shuffle-invariance, inner points и hand-crafted `Simplex4D`. |
| Stress | `x` | Выделен representative stress-набор по кубам, симплексам и fixed-seed random simplices. |

## Existing Test Sources

- [`GW_Tests.cs`](../../../../Tests/Archive/Double-Tests/GW_hDTests/GW_Tests.cs)

## Notes

- Старый `GW_Tests` смешивал прямые проверки алгоритма с большими randomized/regression-сценариями через `ConvexPolytop.CreateFromPoints(..., true)`.
- В новый слой перенесён компактный базовый контракт, meaningful regression и representative stress без полного combinatorial legacy-перебора.
