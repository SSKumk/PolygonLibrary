# ConvexPolytop

## Scope

Класс [`ConvexPolytop.cs`](../../../../CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs) задаёт основной high-level контейнер для выпуклого политопа с поддержкой `Vrep`, `Hrep` и `FLrep`.

В активный слой текущего этапа входят:

- прямые фабрики и переходы между представлениями;
- `FindInitialVertex_Simplex` как локальный шаг восстановления вершины по `Hrep`;
- базовые геометрические фабрики и простые численные метрики;
- базовые эпиграфы расстояния до точки и до политопа для малых контрольных примеров;
- базовая редукция избыточных полупространств;
- point queries и `NearestPoint` для тех веток, которые явно реализованы;
- базовые геометрические преобразования и overrides.

Вне активного слоя пока остаются:

- `CreateFromReader` и `WriteIn`;
- `Polar`;
- большие генераторы `SimplexRND`, `Cyclic`, `Sphere`, `Ellipsoid`;
- низкоуровневые конвертеры `HrepToVrep_*`, `FindInitialVertex_Naive`, `FindClosePairs_Naive`, `MergePoints`.

## Topics

- [`ConstructionAndRepresentations.md`](ConstructionAndRepresentations.md) - построение из `Vrep`, `Hrep`, `FLrep`, lazy-переходы и агрегаты.
- [`InitialVertexRecovery.md`](InitialVertexRecovery.md) - восстановление стартовой вершины по `Hrep` через simplex-based seed.
- [`FactoriesAndMetrics.md`](FactoriesAndMetrics.md) - базовые фабрики и простые метрики расстояния.
- [`DistanceEpigraph.md`](DistanceEpigraph.md) - эпиграфы расстояния до точки и до политопа для базовых норм.
- [`HRedundancy.md`](HRedundancy.md) - удаление избыточных полупространств на базовых 2D-примерах.
- [`ContainmentAndNearestPoint.md`](ContainmentAndNearestPoint.md) - `Contains*` и `NearestPoint`.
- [`TransformsAndOverrides.md`](TransformsAndOverrides.md) - `Shift`, `Rotate`, `LiftUp`, `SectionByHyperPlane`, `Scale`, `Equals`.
- [`RepresentationBranchMatrix.md`](RepresentationBranchMatrix.md) - вспомогательная матрица по веткам `Vrep` / `Hrep` / `FLrep` для методов с разной реализацией.
- [`Polar.md`](Polar.md) - отдельный план покрытия для dual-оператора `Polar`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Representations | `x` | Добавлен прямой набор на три репрезентации и переходы между ними. |
| Initial Vertex Recovery | `x` | Закрыты simplex-based сценарии доуточнения optimal-face точки до вершины и сценарии с вырожденной вершиной. |
| Factories and Metrics | `x` | Закрыты прямые фабрики и численные метрики малой стоимости. |
| Distance Epigraph | `x` | Закрыт базовый слой эпиграфов расстояния до точки и до одноточечного политопа для `L1` / `Linf` / `L2`. |
| HRedundancy | `x` | Закрыты базовые сценарии удаления избыточных полупространств для центрированного и сдвинутого квадрата. |
| Containment and NearestPoint | `x` | Зафиксированы как рабочие, так и ещё не реализованные ветки. |
| Transforms and Overrides | `x` | Базовые преобразования и общие контракты `Equals`/`GetHashCode` закрыты прямыми тестами, включая branch-specific ветки `Shift` / `Rotate` и масштабирование при положительном и отрицательном коэффициенте. |
| Polar | `~` | Закрыты базовые `Vrep` / `Hrep` / `FLrep` 2D-сценарии, `Polar(out shift)` и двойное преобразование; остаются более тонкие duality- и redundancy-вопросы. |
| IO and Advanced Builders | `~` | `FindInitialVertex_Simplex` уже вынесен в активный слой; более тяжёлые low-level builders и `HrepToVrep_*` остаются вне него. |

## Existing Test Sources

- [`GW_Tests.cs`](../../../../Tests/Archive/Double-Tests/GW_hDTests/GW_Tests.cs)
- [`MinkowskiSumTests.cs`](../../../../Tests/Archive/Double-Tests/Minkowski-Tests/MinkowskiSumTests.cs)
- [`MinkowskiDiffTests.cs`](../../../../Tests/Archive/Double-Tests/Minkowski-Tests/MinkowskiDiffTests.cs)
- [`TestsPolytopes.cs`](../../../../Tests/TestInfrastructure/TestsPolytopes.cs)

## Notes

- Старое покрытие `ConvexPolytop` было размазано по алгоритмическим тестам. В новой структуре добавлен отдельный прямой unit-like слой именно на сам класс.
- По текущему активному слою branch-specific матрица для `Vrep` / `Hrep` / `FLrep` закрыта.
- Отдельными малыми слоями уже закреплены `Polar`, distance-epigraph API и базовая редукция `Hrep`; вне активного слоя остаются только более тяжёлые high-dimensional и algorithmic ветки.


