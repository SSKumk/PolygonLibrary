# ConvexPolygon Extreme Area And Nearest

## Scope

Этот файл покрывает `GetExtremeElements`, `Square` и текущий контракт `NearestPoint`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `CPOLY-EXT-001` | `x` | `GetExtremeElements` возвращает одну вершину или ребро в зависимости от направления, если у polygon нет codirected normal. | [`ConvexPolygonExtremeAndAreaTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonExtremeAndAreaTests.cs) |
| `CPOLY-EXT-002` | `x` | `GetExtremeElements` возвращает ребро для направлений, совпадающих с нормалями polygon. | [`ConvexPolygonExtremeAndAreaTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonExtremeAndAreaTests.cs) |
| `CPOLY-AREA-001` | `x` | `Square` вычисляет ожидаемую площадь для квадрата и восьмиугольника из legacy-набора. | [`ConvexPolygonExtremeAndAreaTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonExtremeAndAreaTests.cs) |
| `CPOLY-NP-001` | `x` | `NearestPoint` пока бросает `NotImplementedException`. | [`ConvexPolygonExtremeAndAreaTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonExtremeAndAreaTests.cs) |

## Gaps

- В mandatory-сценариях пробелов не осталось.

## Notes

- Для `NearestPoint` зафиксирован текущий наблюдаемый публичный контракт, как и раньше для других `todo`-методов библиотеки.

