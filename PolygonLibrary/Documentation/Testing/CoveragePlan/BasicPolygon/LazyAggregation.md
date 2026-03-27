# BasicPolygon Lazy Aggregation

## Scope

Этот файл покрывает ленивое восстановление представлений `BasicPolygon`:

- `Vertices` из `Contours`;
- `Edges` из `Contours`;
- `Contours` из `Vertices`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `BPOLY-LAZY-001` | `x` | `Vertices` объединяет вершины всех контуров в их текущем порядке и кэширует результат. | [`BasicPolygonLazyAggregationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonLazyAggregationTests.cs) |
| `BPOLY-LAZY-002` | `x` | `Edges` объединяет рёбра всех контуров, сортирует их и кэширует результат. | [`BasicPolygonLazyAggregationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonLazyAggregationTests.cs) |
| `BPOLY-LAZY-003` | `x` | `Contours` строит единственный counterclockwise-контур как выпуклую оболочку множества вершин и кэширует результат. | [`BasicPolygonLazyAggregationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonLazyAggregationTests.cs) |

## Gaps

- Сценарии обращения к `Vertices`, `Edges` или `Contours`, когда ни одно исходное представление не инициализировано, не фиксируются как публичный контракт: код опирается на preconditions наследников.

## Notes

- Для сценария `Contours <- Vertices` используется детерминированный набор точек с одной внутренней точкой и дубликатом, чтобы зафиксировать именно поведение через выпуклую оболочку, а не случайный порядок обхода.

