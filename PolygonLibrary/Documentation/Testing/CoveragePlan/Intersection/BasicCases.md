# Intersection Basic Cases

## Scope

Этот файл покрывает основные сценарии непустого пересечения выпуклых многоугольников.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `INT-BAS-001` | `x` | Пересечение двух выпуклых многоугольников возвращает корректный результирующий многоугольник при обычном частичном перекрытии. | [`ConvexPolygonIntersectionTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs) |
| `INT-BAS-002` | `x` | Результат не зависит от порядка аргументов `P` и `Q`. | [`ConvexPolygonIntersectionTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs) |
| `INT-BAS-003` | `x` | Результат корректен при циклическом сдвиге вершин обоих многоугольников. | [`ConvexPolygonIntersectionTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs) |
| `INT-BAS-004` | `x` | Алгоритм корректно обрабатывает пересечение прямоугольника и треугольника. | [`ConvexPolygonIntersectionTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs) |
| `INT-BAS-005` | `x` | Алгоритм корректно обрабатывает пересечение двух четырёхугольников с несколькими точками пересечения границ. | [`ConvexPolygonIntersectionTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs) |

## Gaps

- Нет отдельного минимального unit-like сценария на `null`-входы.
- Нет отдельной фиксации поведения на численно почти параллельных рёбрах вне существующего набора regression tests.

## Notes

- Текущий набор тестов здесь уже сильный, но плохо структурирован по типам случаев; этот файл нужен именно как карта сценариев.



