# Convexification

## Scope

- [`../../../../CGLibrary/Algorithms/ConvexHull2D/Convexification.cs`](../../../../CGLibrary/Algorithms/ConvexHull2D/Convexification.cs)
  Объект: 2D helper-алгоритмы выпукления `QuickHull2D`, `ArcHull2D`, `GrahamHull` и вспомогательный `IsLeft`.
  Когда использовать: когда нужно проверить базовые контракты построения выпуклой оболочки и важные различия между тремя реализациями.

## Scenario Files

- [`HullAlgorithms.md`](HullAlgorithms.md)
  Объект: базовые и regression-сценарии для `QuickHull2D`, `ArcHull2D` и `GrahamHull`.
  Когда использовать: когда нужно сверить текущий контракт оболочек на квадратах, вырожденных наборах и materialized/non-materialized входах.
