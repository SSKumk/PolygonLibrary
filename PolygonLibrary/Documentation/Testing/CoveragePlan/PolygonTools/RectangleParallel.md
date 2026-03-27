# PolygonTools RectangleParallel

## Scope

Этот файл покрывает `PolygonTools.RectangleParallel`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `PTOOLS-RP-001` | `x` | При совпадающих вершинах `RectangleParallel` возвращает одноточечный polygon. | [`PolygonToolsRectangleParallelTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleParallelTests.cs) |
| `PTOOLS-RP-002` | `x` | При вертикально расположенных противоположных вершинах `RectangleParallel` возвращает segment независимо от порядка аргументов. | [`PolygonToolsRectangleParallelTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleParallelTests.cs) |
| `PTOOLS-RP-003` | `x` | При горизонтально расположенных противоположных вершинах `RectangleParallel` возвращает segment независимо от порядка аргументов. | [`PolygonToolsRectangleParallelTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleParallelTests.cs) |
| `PTOOLS-RP-004` | `x` | Для невырожденного прямоугольника `RectangleParallel` возвращает polygon из четырёх вершин независимо от порядка диагональных точек. | [`PolygonToolsRectangleParallelTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleParallelTests.cs) |

## Gaps

- Отдельная фиксация точного порядка вершин не добавляется: для этого factory важнее кардинальность и корректная геометрическая форма результата.

## Notes

- Исторические диагностические сообщения `Vector rectangle`, `Vertical segment rectangle`, `Horizontal segment rectangle`, `LL-RU rectangle`, `LU-RL rectangle` сохранены.

