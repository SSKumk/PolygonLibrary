# Intersection Boundary And Degenerate Cases

## Scope

Этот файл покрывает граничные и вырожденные случаи `IntersectionPolygon`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `INT-BND-001` | `x` | Если один многоугольник целиком содержится в другом, результатом является вложенный многоугольник. | [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs), [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs) |
| `INT-BND-002` | `x` | Если пересечение пусто, метод возвращает `null`. | [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs) |
| `INT-BND-003` | `x` | Если многоугольники касаются по площади с частью общих рёбер или вершин, метод возвращает правильный граничный многоугольник. | [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs), [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs) |
| `INT-BND-004` | `x` | Если пересечение вырождается в точку или отрезок, метод возвращает `null` согласно текущему контракту реализации. | [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs) |
| `INT-BND-005` | `x` | Если один из аргументов `null`, метод возвращает `null`. | [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs) |
| `INT-BND-006` | `x` | Алгоритм устойчив на наборе большого числа вручную подобранных граничных конфигураций. | [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs) |

## Gaps

- Нет отдельного маленького сценария, который бы прямо фиксировал выбор `null` для пересечения-точки или пересечения-отрезка как часть контракта.

## Notes

- Здесь особенно важно не потерять бизнес-решение “точка и отрезок считаются пустым пересечением”, потому что это не математически очевидный выбор.



