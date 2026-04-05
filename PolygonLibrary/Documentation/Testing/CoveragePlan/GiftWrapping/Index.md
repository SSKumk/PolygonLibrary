# GiftWrapping

## Scope

Класс [`GiftWrapping.cs`](../../../../CGLibrary/Algorithms/Polyhedra/GiftWrapping/GiftWrapping.cs) реализует построение выпуклой оболочки и `FaceLattice` по swarm of points.

В активный слой текущего этапа входят:

- базовый прямой контракт алгоритма на пустом, точечном и линейном случаях;
- `WrapVRep` для типовых `2D/3D` наборов точек;
- `WrapFaceLattice` и `ConstructFL` для типовых `2D/3D` политопов.

Вне активного слоя пока остаются:

- invariance/regression-сценарии из legacy `GW_Tests`;
- многомерные hand-crafted cases;
- heavy random/stress слои и производительные прогоны.

## Topics

- [`BasicCases.md`](BasicCases.md) - базовый прямой контракт `GiftWrapping`.
- [`RegressionAndInvariance.md`](RegressionAndInvariance.md) - интеграционные regression-сценарии через `CreateFromPoints(..., true)`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Basic Cases | `x` | Закрыты пустой, точечный, линейный, типовые `2D/3D` и базовые `FaceLattice`-сценарии. |
| Regression and Invariance | `x` | Перенесён первый `Cube3D`-пакет: повороты, сдвиги и inner points. |
| Stress | `-` | Heavy random и `4D+` генераторные серии ещё не выделены в новый stress-слой. |

## Existing Test Sources

- [`GW_Tests.cs`](../../../../Tests/Double-Tests/GW_hDTests/GW_Tests.cs)

## Notes

- Старый `GW_Tests` смешивал прямые проверки алгоритма с большими randomized/regression-сценариями через `ConvexPolytop.CreateFromPoints(..., true)`.
- На текущем шаге базовый слой сведён в один файл, чтобы дальше наращивать `Regression` и `Stress` отдельно.
