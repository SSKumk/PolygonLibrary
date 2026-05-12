# ConvexPolygon Construction

## Scope

Этот файл покрывает конструкторы `ConvexPolygon` и согласование представлений `Contour`, `Vertices`, `SF`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `CPOLY-CTOR-001` | `x` | Конструктор из `Vector2D` без convexify создаёт один контур из переданных вершин. | [`ConvexPolygonConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonConstructionTests.cs) |
| `CPOLY-CTOR-002` | `x` | Конструктор из `Vector2D` с convexify строит выпуклую оболочку входного множества. | [`ConvexPolygonConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonConstructionTests.cs) |
| `CPOLY-CTOR-003` | `x` | Конструктор из `Vector` проектирует точки в `Vector2D`. | [`ConvexPolygonConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonConstructionTests.cs) |
| `CPOLY-CTOR-004` | `x` | Конструктор из `SupportFunction` лениво восстанавливает контур и вершины. | [`ConvexPolygonConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonConstructionTests.cs) |
| `CPOLY-CTOR-005` | `x` | Для polygon, созданного из вершин, `SF` вычисляется из контура по требованию. | [`ConvexPolygonConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonConstructionTests.cs) |

## Gaps

- Явный сценарий с полностью неинициализированным polygon без вершин и без `SF` не входит в активный слой: это не штатный способ создания `ConvexPolygon`.

## Notes

- Старые smoke-тесты `CreateCPOfPointsTest*` и `CreateCPOfCFTest1` заменены на прямые проверки содержимого.


