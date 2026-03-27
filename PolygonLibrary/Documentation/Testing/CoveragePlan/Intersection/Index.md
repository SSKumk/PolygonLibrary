# Intersection

## Scope

Файл [`Intersection.cs`](../../../../CGLibrary/Polygons/ConvexPolygons/Intersection.cs) содержит алгоритм `ConvexPolygon.IntersectionPolygon(P, Q)`:

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
| Basic Cases | `x` | Есть большой отдельный набор прямых тестов на пересечение многоугольников. |
| Boundary and Degenerate Cases | `x` | В тестовом наборе уже много случаев касаний, пустых пересечений и вложения. |

## Existing Test Sources

- [`ConvexPolygonIntersectionTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs)
- [`ConvexPolygonIntersectionTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs)
- [`ConvexPolygonIntersectionTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs)

## Notes

- Формально это не отдельный класс, а алгоритм в partial `ConvexPolygon`, но как объект тестирования его удобнее держать в своей папке.



