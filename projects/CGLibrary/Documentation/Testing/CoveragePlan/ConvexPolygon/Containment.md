# ConvexPolygon Containment

## Scope

Этот файл покрывает `Contains` и `ContainsInside`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `CPOLY-CNT-001` | `x` | `Contains` принимает внутренние и граничные точки на плотной аппроксимации круга и отвергает внешние. | [`ConvexPolygonContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonContainmentTests.cs) |
| `CPOLY-CNT-002` | `x` | `Contains` корректно обрабатывает legacy-набор лучевых и конусных случаев на восьмиугольнике. | [`ConvexPolygonContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonContainmentTests.cs) |
| `CPOLY-CNT-003` | `x` | `ContainsInside` принимает только строго внутренние точки и отвергает границу на плотной аппроксимации круга. | [`ConvexPolygonContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonContainmentTests.cs) |
| `CPOLY-CNT-004` | `x` | `ContainsInside` корректно обрабатывает legacy-набор лучевых и конусных случаев на восьмиугольнике. | [`ConvexPolygonContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonContainmentTests.cs) |

## Gaps

- В mandatory-сценариях пробелов не осталось.

## Notes

- Legacy-диагностика `inside*`, `boundary*`, `outside*` и `ContainsTest2, test #...` сохранена.

