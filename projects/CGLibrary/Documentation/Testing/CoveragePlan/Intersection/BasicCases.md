# Intersection Basic Cases

## Scope

Этот файл покрывает основные сценарии непустого пересечения выпуклых многоугольников.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `INT-BAS-001` | `x` | Пересечение двух выпуклых многоугольников возвращает корректный результирующий многоугольник при обычном частичном перекрытии. | [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs) |
| `INT-BAS-002` | `x` | Результат не зависит от порядка аргументов `P` и `Q`. | [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs) |
| `INT-BAS-003` | `x` | Результат корректен при циклическом сдвиге вершин обоих многоугольников. | [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs), [`ConvexPolygonIntersectionTestBase.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionTestBase.cs) |
| `INT-BAS-004` | `x` | Алгоритм корректно обрабатывает пересечение прямоугольника и треугольника. | [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs) |
| `INT-BAS-005` | `x` | Алгоритм корректно обрабатывает пересечение двух четырёхугольников с несколькими точками пересечения границ. | [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs) |

## Gaps

- Нет отдельной фиксации поведения на численно почти параллельных рёбрах вне существующего набора regression tests.

## Notes

- Текущий набор тестов здесь уже сильный, но плохо структурирован по типам случаев; этот файл нужен именно как карта сценариев.



