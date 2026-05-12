# BasicPolygon Construction

## Scope

Этот файл покрывает явные конструкторы `BasicPolygon`:

- построение по списку вершин;
- построение по массиву вершин.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `BPOLY-CTOR-001` | `x` | Конструктор по списку создаёт один контур с counterclockwise-ориентацией. | [`BasicPolygonConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonConstructionTests.cs) |
| `BPOLY-CTOR-002` | `x` | Конструктор по списку передаёт исходный список вершин в единственный контур, но хранит отдельную копию списка в `Vertices`. | [`BasicPolygonConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonConstructionTests.cs) |
| `BPOLY-CTOR-003` | `x` | Конструктор по массиву копирует вершины и для `Contours`, и для `Vertices`. | [`BasicPolygonConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonConstructionTests.cs) |

## Gaps

- Базовый конструктор без параметров не фиксируется отдельным тестовым контрактом: без дополнительной инициализации от наследника базовый класс не определяет полезного публичного состояния.

## Notes

- Флаги `checkOrient` и `checkCross` передаются дальше в `Polyline`, но сами проверки не являются обязанностью `BasicPolygon`, поэтому здесь они отдельно не специфицируются.

