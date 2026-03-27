# Line2D Queries And Intersection

## Scope

Этот файл покрывает запросные и операционные сценарии для `Line2D`:

- значение линейной функции в точке;
- проверка принадлежности точки прямой;
- смена ориентации;
- пересечение двух прямых.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `L2D-QRY-001` | `x` | Индексатор возвращает `0` для точки, лежащей на прямой. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-002` | `x` | Индексатор возвращает положительное значение для точки в положительной полуплоскости. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-003` | `x` | Индексатор возвращает отрицательное значение для точки в отрицательной полуплоскости. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-004` | `x` | `PassesThrough` возвращает `true` для точки на прямой. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-005` | `x` | `PassesThrough` возвращает `false` для точки вне прямой. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-006` | `x` | `Reorient` создаёт геометрически ту же прямую. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-007` | `x` | `Reorient` меняет знак коэффициентов и разворачивает ориентацию полуплоскостей. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-008` | `x` | `Reorient().Reorient()` возвращает эквивалентную исходной прямую. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-009` | `x` | `Intersect` возвращает `SinglePoint` для пересекающихся прямых. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-010` | `x` | `Intersect` возвращает корректную точку пересечения для пересекающихся прямых. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-011` | `x` | `Intersect` возвращает `Parallel` для различных параллельных прямых. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-012` | `x` | `Intersect` возвращает `Overlap` для совпадающих прямых. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-013` | `x` | `Intersect` симметричен по аргументам. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-014` | `x` | Пересечение корректно работает для вертикальной и горизонтальной прямой. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-015` | `x` | Пересечение корректно работает для двух наклонных прямых. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-016` | `x` | Пересечение корректно отражает epsilon-чувствительное поведение в почти совпадающих и почти параллельных случаях. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-017` | `x` | `PassesThrough` использует текущий `Tools.Eps` при решении, лежит ли точка на прямой. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |
| `L2D-QRY-018` | `x` | `Intersect` возвращает `Overlap` для совпадающих прямых независимо от ориентации их направляющих векторов. | [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) |

## Gaps

- Отдельные сценарии для вырожденных прямых здесь не фиксируются, потому что такой контракт в коде явно не описан.

## Notes

- Для пересечения полезно держать компактный набор эталонных случаев: горизонтальная и вертикальная прямая, две наклонные пересекающиеся прямые, различные параллельные прямые и совпадающие прямые.
