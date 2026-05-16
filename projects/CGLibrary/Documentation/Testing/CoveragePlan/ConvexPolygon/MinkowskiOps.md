# ConvexPolygon Minkowski Operations

## Scope

Этот файл покрывает операторы суммы и разности Минковского.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `CPOLY-MINK-001` | `x` | Оператор `+` строит ожидаемый octagon для суммы повёрнутого прямоугольника и квадратa. | [`ConvexPolygonMinkowskiTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonMinkowskiTests.cs) |
| `CPOLY-MINK-002` | `x` | Оператор `+` удваивает квадрат при сумме polygon с самим собой. | [`ConvexPolygonMinkowskiTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonMinkowskiTests.cs) |
| `CPOLY-MINK-003` | `x` | Оператор `-` возвращает ожидаемые legacy-результаты для непустых разностей. | [`ConvexPolygonMinkowskiTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonMinkowskiTests.cs) |
| `CPOLY-MINK-004` | `x` | Оператор `-` возвращает ожидаемый вырожденный segment-результат. | [`ConvexPolygonMinkowskiTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonMinkowskiTests.cs) |
| `CPOLY-MINK-005` | `x` | Оператор `-` возвращает `null` для empty-difference сценариев из legacy-набора. | [`ConvexPolygonMinkowskiTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonMinkowskiTests.cs) |

## Gaps

- В mandatory-сценариях пробелов не осталось.

## Notes

- Используются старые диагностические сообщения `Sum ...`, `Diff ...` и циклическое сравнение вершин из legacy helper-а.

