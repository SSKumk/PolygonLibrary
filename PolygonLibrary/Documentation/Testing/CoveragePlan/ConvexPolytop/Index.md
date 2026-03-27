# ConvexPolytop

## Scope

Класс [`ConvexPolytop.cs`](../../../../CGLibrary/Polyhedra/ConvexPolyhedra/ConvexPolytops/ConvexPolytop.cs) задаёт основной high-level контейнер для выпуклого политопа с поддержкой `Vrep`, `Hrep` и `FLrep`.

В обязательный минимум текущей миграции входят:

- прямые фабрики и переходы между представлениями;
- базовые геометрические фабрики и простые численные метрики;
- point queries и `NearestPoint` для тех веток, которые явно реализованы;
- базовые геометрические преобразования и overrides.

Вне обязательного минимума пока остаются:

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

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Representations | `x` | Добавлен прямой набор на три репрезентации и переходы между ними. |
| Factories and Metrics | `x` | Закрыты прямые фабрики и численные метрики малой стоимости. |
| Containment and NearestPoint | `x` | Зафиксированы как рабочие, так и ещё не реализованные ветки. |
| Transforms and Overrides | `x` | Закрыты базовые преобразования и общие контракты `Equals`/`GetHashCode`. |
| IO, Polar and Advanced Builders | `-` | Оставлены вне обязательного минимума этой миграции. |

## Existing Test Sources

- [`GW_Tests.cs`](../../../../Tests/Double-Tests/GW_hDTests/GW_Tests.cs)
- [`MinkowskiSumTests.cs`](../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiSumTests.cs)
- [`MinkowskiDiffTests.cs`](../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiDiffTests.cs)
- [`TestsPolytopes.cs`](../../../../Tests/ToolsForTests/TestsPolytopes.cs)

## Notes

- Старое покрытие `ConvexPolytop` было размазано по алгоритмическим тестам. В новой структуре добавлен отдельный прямой unit-like слой именно на сам класс.

