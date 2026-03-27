# PolygonTools RectangleTurned

## Scope

Этот файл покрывает `PolygonTools.RectangleTurned`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `PTOOLS-RT-001` | `x` | При совпадающих вершинах `RectangleTurned` возвращает одноточечный polygon для всех тестируемых углов. | [`PolygonToolsRectangleTurnedTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleTurnedTests.cs) |
| `PTOOLS-RT-002` | `x` | Для осе-диагоналей `RectangleTurned` вырождается в segment при специальных углах `0`, `pi/2`, `pi`. | [`PolygonToolsRectangleTurnedTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleTurnedTests.cs) |
| `PTOOLS-RT-003` | `x` | Для осе-диагоналей `RectangleTurned` возвращает четырёхугольник при остальных углах из тестового набора. | [`PolygonToolsRectangleTurnedTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleTurnedTests.cs) |
| `PTOOLS-RT-004` | `x` | Для общих противоположных вершин `RectangleTurned` сохраняет четыре вершины при всех тестируемых углах. | [`PolygonToolsRectangleTurnedTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleTurnedTests.cs) |
| `PTOOLS-RT-005` | `x` | Рёбра результата имеют ожидаемый наклон и ортогональны попарно по обходу контура. | [`PolygonToolsRectangleTurnedTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleTurnedTests.cs), [`PolygonToolsAssert.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsAssert.cs) |

## Gaps

- Отдельные точные координаты всех четырёх вершин не фиксируются: legacy-контракт проверял именно геометрические свойства и кратность вершин.

## Notes

- Исторические сообщения `Turned rectangle, alpha = ...` сохранены в том же формате, включая повтор `", i = " + i + ", i = " + i` в сообщении про число контуров.

