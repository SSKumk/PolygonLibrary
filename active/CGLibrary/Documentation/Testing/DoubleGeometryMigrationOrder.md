# DoubleGeometry Migration Order

Этот файл фиксирует порядок закрытия классов при переносе тестов в `Tests/DoubleGeometry/`.

Статусы:

- `closed` - класс перенесён, coverage-документация синхронизирована, legacy-файл исключён или не требуется.
- `in_progress` - класс находится в текущей миграции.
- `pending` - класс ещё не переносился.

Принципы порядка:

- сначала компактные базовые 2D-сущности и операции;
- затем базовая линейная алгебра;
- затем полигональные сущности;
- затем полиэдральные структуры и алгоритмы.

## Current Status

| Order | Class | Status | Notes |
| --- | --- | --- | --- |
| 0 | `Line2D` | `closed` | Пилотный перенос выполнен до фиксации общего backlog. |
| 1 | `Vector2D` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Vector2D`, legacy `VectorsTests.cs` исключён из компиляции. |
| 2 | `Segment` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Segment`, legacy `SegmentCrossTests.cs` исключён из компиляции. |
| 3 | `Intersection` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Intersection`, legacy `ConvexPolygonIntersectionTests.cs` исключён из компиляции. |
| 4 | `SegmentPair` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/SegmentPair` отдельным unit-like набором. |
| 5 | `Tools` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Tools` прямым базовым набором на численные контракты. |
| 6 | `Vector` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Vector`, legacy ``VectorTests.cs`` архивирован. |
| 7 | `Matrix` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Matrix`, legacy ``MatrixTests.cs`` архивирован. |
| 8 | `LinearBasis` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/LinearBasis`, legacy ``LinearBasisTests.cs`` архивирован. |
| 9 | `AffineBasis` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/AffineBasis`, legacy ``AffineBasisTest.cs`` архивирован. |
| 10 | `HyperPlane` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/HyperPlane`, legacy ``HyperPlaneTests.cs`` архивирован. |
| 11 | `Polyline` | `closed` | Перенесён в `Tests/DoubleGeometry/Polygons/Polyline`, legacy ``PolylineTests.cs`` архивирован. |
| 12 | `BasicPolygon` | `closed` | Перенесён в `Tests/DoubleGeometry/Polygons/BasicPolygon` прямым unit-like набором через тестовые наследники. |
| 13 | `PolygonTools` | `closed` | Перенесён в `Tests/DoubleGeometry/Polygons/PolygonTools`, legacy ``PolygonToolsTests.cs`` архивирован. |
| 14 | `GammaPair` | `closed` | Перенесён в `Tests/DoubleGeometry/Polygons/GammaPair` прямым набором на конструкторы, сравнение и `CrossPairs`. |
| 15 | `SupportFunction` | `closed` | Перенесён в `Tests/DoubleGeometry/Polygons/SupportFunction`, legacy ``SupportFunctionTests.cs`` архивирован. |
| 16 | `ConvexPolygon` | `closed` | Перенесён в `Tests/DoubleGeometry/Polygons/ConvexPolygon`, legacy `ConvexPolygon*` и `PolygonExtremeTests.cs` исключены из компиляции. |
| 17 | `FaceLattice` | `closed` | Перенесён в `Tests/DoubleGeometry/Polyhedra/FaceLattice`, legacy ``FaceLatticeTests.cs`` архивирован. |
| 18 | `ConvexPolytop` | `closed` | Перенесён в `Tests/DoubleGeometry/Polyhedra/ConvexPolytop` прямым unit-like набором; legacy-источники остаются у алгоритмических классов. |
| 19 | `HrepToFLrep` | `closed` | Перенесён прямым текущим контрактом: `null` без стартовой вершины и `NotImplementedException` на ограниченном случае. |
| 20 | `GiftWrapping` | `closed` | Перенесён в `Tests/DoubleGeometry/Algorithms/GiftWrapping`, legacy `legacy/PolygonLibrary/Tests/Double-Tests/GW_hDTests/GW_Tests.cs` больше не участвует в активной компиляции. |
| 21 | `MinkowskiSum` | `closed` | Перенесён в `Tests/DoubleGeometry/Algorithms/MinkowskiSum`, legacy `MinkowskiSumTests.cs` исключён из компиляции. |
| 22 | `MinkowskiDiff` | `in_progress` | Перенесён в `Tests/DoubleGeometry/Algorithms/MinkowskiDiff` базовым и regression-слоем; `cyclic` вынесен в research, а `tetrahedron - point` / `octahedron - point` оставлены красными signal-тестами на simplex-bug. |
| 23 | `CauchyMatrix` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/CauchyMatrix` прямым математически контролируемым набором. |

## Workflow Reminder

1. Уточнить coverage plan по реальному коду и legacy-тестам.
2. Перенести класс в `Tests/DoubleGeometry/...` по тематическим файлам.
3. Сохранить полезные поясняющие сообщения и смысловые комментарии из legacy-тестов.
4. Дописать недостающие тесты до согласованного покрытия.
5. Обновить `Documentation/Testing/CoveragePlan/...`.
6. Исключить legacy-файл из активной компиляции, если он больше не нужен.
7. Проверить обычную сборку `Tests.csproj` без запуска долгих тестов.
8. Обновить `Documentation/Testing/NewTestsLog.md`.
9. После закрытия класса сделать отдельный коммит.
