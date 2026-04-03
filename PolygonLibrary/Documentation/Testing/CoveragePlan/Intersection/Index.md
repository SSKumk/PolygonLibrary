# Intersection

## Scope

Файл [`Intersection.cs`](../../../../CGLibrary/Geometry2D/Polygons/ConvexPolygons/Intersection.cs) содержит алгоритм `ConvexPolygon.IntersectionPolygon(P, Q)`:

- пересечение двух выпуклых многоугольников;
- обработку входов `null`;
- возврат `null` для пустого пересечения, а также для вырожденного пересечения в точку или отрезок;
- симметричность по порядку аргументов.

## Topics

- [`BasicCases.md`](BasicCases.md) - обычные непустые пересечения и симметрия результата.
- [`BoundaryAndDegenerateCases.md`](BoundaryAndDegenerateCases.md) - касания, вложение, пустое и вырожденное пересечение.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Basic Cases | `x` | Сценарии перенесены в отдельный базовый набор для непустых пересечений. |
| Boundary and Degenerate Cases | `x` | Граничные случаи, пустые пересечения и `null`-входы вынесены в отдельный файл. |

## Existing Test Sources

- [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs)
- [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs)
- [`ConvexPolygonIntersectionTestBase.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionTestBase.cs)

## Notes

- Формально это не отдельный класс, а алгоритм в partial `ConvexPolygon`, но как объект тестирования его удобнее держать в своей папке.



