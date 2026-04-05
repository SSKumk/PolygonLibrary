# Segment

## Scope

Класс [`Segment.cs`](../../../../CGLibrary/Geometry2D/Segments/Segment.cs) описывает невырожденный отрезок на плоскости и связанные с ним структуры пересечения:

- хранение концов, направляющего вектора, нормали и длины;
- принадлежность точки отрезку;
- вычисление ординаты на несущей прямой;
- пересечение двух отрезков с детальной классификацией результата через `CrossInfo`.

## Topics

- [`ConstructionAndGeometry.md`](ConstructionAndGeometry.md) - конструкторы, индексатор, геометрические свойства.
- [`PointQueries.md`](PointQueries.md) - `IsEndPoint`, `IsInnerPoint`, `ContainsPoint`, `ComputeAtPoint`.
- [`Intersection.md`](Intersection.md) - `Intersect`, `CrossInfo`, `CrossType`, `IntersectPointPos`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Geometry | `x` | Покрытие вынесено в `Tests/DoubleGeometry/Basics/Segment`. |
| Point Queries | `x` | Покрытие вынесено в `Tests/DoubleGeometry/Basics/Segment`. |
| Intersection | `x` | Покрытие вынесено в `Tests/DoubleGeometry/Basics/Segment`. |

## Existing Test Sources

- [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs)
- [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs)
- [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs)
- [`BentlyOttmannTests.cs`](../../../../Tests/BentlyOttmannTests.cs) - косвенное использование `Segment` и `SegmentPair`, но файл исключён из компиляции тестового проекта.

Legacy-источник [`SegmentCrossTests.cs`](../../../../Tests/Archive/Double-Tests/SegmentCrossTests.cs) больше не участвует в активной компиляции после переноса.

## Notes

- Для `Segment` в первую очередь не хватает системного покрытия `Intersect`, несмотря на наличие закомментированного черновика тестов.



