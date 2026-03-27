# Segment

## Scope

Класс [`Segment.cs`](../../../../CGLibrary/Segments/Segment.cs) описывает невырожденный отрезок на плоскости и связанные с ним структуры пересечения:

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
| Construction and Geometry | ` ` | Явная разметка по этим сценариям ещё не сделана. |
| Point Queries | `~` | Есть прямой тест на `ContainsPoint`, остальное не размечено. |
| Intersection | ` ` | Активного прямого покрытия у текущего test project почти нет. |

## Existing Test Sources

- [`SegmentCrossTests.cs`](../../../../Tests/Double-Tests/SegmentCrossTests.cs) - прямой тест на `ContainsPoint`.
- [`BentlyOttmannTests.cs`](../../../../Tests/BentlyOttmannTests.cs) - косвенное использование `Segment` и `SegmentPair`, но файл исключён из компиляции тестового проекта.

## Notes

- Для `Segment` в первую очередь не хватает системного покрытия `Intersect`, несмотря на наличие закомментированного черновика тестов.



