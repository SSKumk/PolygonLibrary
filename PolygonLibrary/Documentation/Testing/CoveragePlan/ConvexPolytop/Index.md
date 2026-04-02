# ConvexPolytop

## Scope

Класс [`ConvexPolytop.cs`](../../../../CGLibrary/Polyhedra/ConvexPolyhedra/ConvexPolytops/ConvexPolytop.cs) задаёт основной high-level контейнер для выпуклого политопа с поддержкой `Vrep`, `Hrep` и `FLrep`.

В активный слой текущего этапа входят:

- прямые фабрики и переходы между представлениями;
- базовые геометрические фабрики и простые численные метрики;
- point queries и `NearestPoint` для тех веток, которые явно реализованы;
- базовые геометрические преобразования и overrides.

Вне активного слоя пока остаются:

- `CreateFromReader` и `WriteIn`;
- `Polar`;
- distance-epigraph API;
- большие генераторы `SimplexRND`, `Cyclic`, `Sphere`, `Ellipsoid`;
- низкоуровневые конвертеры `HrepToVrep_*`, `FindInitialVertex_*`, `HRedundancyByGW`, `FindClosePairs_Naive`, `MergePoints`.

## Topics

- [`ConstructionAndRepresentations.md`](ConstructionAndRepresentations.md) - построение из `Vrep`, `Hrep`, `FLrep`, lazy-переходы и агрегаты.
- [`FactoriesAndMetrics.md`](FactoriesAndMetrics.md) - базовые фабрики и простые метрики расстояния.
- [`ContainmentAndNearestPoint.md`](ContainmentAndNearestPoint.md) - `Contains*` и `NearestPoint`.
- [`TransformsAndOverrides.md`](TransformsAndOverrides.md) - `Shift`, `Rotate`, `LiftUp`, `SectionByHyperPlane`, `Scale`, `Equals`.
- [`RepresentationBranchMatrix.md`](RepresentationBranchMatrix.md) - вспомогательная матрица по веткам `Vrep` / `Hrep` / `FLrep` для методов с разной реализацией.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Representations | `x` | Добавлен прямой набор на три репрезентации и переходы между ними. |
| Factories and Metrics | `x` | Закрыты прямые фабрики и численные метрики малой стоимости. |
| Containment and NearestPoint | `x` | Зафиксированы как рабочие, так и ещё не реализованные ветки. |
| Transforms and Overrides | `x` | Базовые преобразования и общие контракты `Equals`/`GetHashCode` закрыты прямыми тестами, включая branch-specific ветки `Shift` / `Rotate` и масштабирование при положительном и отрицательном коэффициенте. |
| IO, Polar and Advanced Builders | `-` | Оставлены вне активного слоя этой миграции. |

## Existing Test Sources

- [`GW_Tests.cs`](../../../../Tests/Double-Tests/GW_hDTests/GW_Tests.cs)
- [`MinkowskiSumTests.cs`](../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiSumTests.cs)
- [`MinkowskiDiffTests.cs`](../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiDiffTests.cs)
- [`TestsPolytopes.cs`](../../../../Tests/ToolsForTests/TestsPolytopes.cs)

## Notes

- Старое покрытие `ConvexPolytop` было размазано по алгоритмическим тестам. В новой структуре добавлен отдельный прямой unit-like слой именно на сам класс.
- По текущему активному слою branch-specific матрица для `Vrep` / `Hrep` / `FLrep` закрыта; вне неё сознательно оставлен только `Polar`.


