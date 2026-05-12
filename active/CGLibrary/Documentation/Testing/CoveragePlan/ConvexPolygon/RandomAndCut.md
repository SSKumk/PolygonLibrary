# ConvexPolygon Random And Cut

## Scope

Этот файл покрывает случайные точки и разрез polygon по двум вершинам.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `CPOLY-RND-001` | `x` | `GenerateDataForRandomPoint` возвращает корректный индекс треугольника и барицентрические веса. | [`ConvexPolygonRandomAndCutTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonRandomAndCutTests.cs) |
| `CPOLY-RND-002` | `x` | `GenerateRandomPoint` возвращает точки, лежащие внутри polygon. | [`ConvexPolygonRandomAndCutTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonRandomAndCutTests.cs) |
| `CPOLY-CUT-001` | `x` | `CutConvexPolygon` бросает `ArgumentException` для соседних вершин. | [`ConvexPolygonRandomAndCutTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonRandomAndCutTests.cs) |
| `CPOLY-CUT-002` | `x` | `CutConvexPolygon` по несоседним вершинам возвращает два ожидаемых подpolygon. | [`ConvexPolygonRandomAndCutTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonRandomAndCutTests.cs) |

## Gaps

- Старый `DoCutTest` был smoke-only; в новом наборе он заменён прямой спецификацией только для штатных и граничных сценариев.

## Notes

- Для `GenerateRandomPoint` используется детерминированный `GRandomLC`, чтобы сценарий оставался воспроизводимым без массовых прогонов.

