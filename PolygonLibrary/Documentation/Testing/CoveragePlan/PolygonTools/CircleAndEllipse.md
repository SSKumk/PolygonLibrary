# PolygonTools Circle And Ellipse

## Scope

Этот файл покрывает `PolygonTools.Circle` и `PolygonTools.Ellipse`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `PTOOLS-CE-001` | `x` | `Circle` с нулевым радиусом возвращает одноточечный polygon в центре. | [`PolygonToolsCircleAndEllipseTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs) |
| `PTOOLS-CE-002` | `x` | `Circle` с положительным радиусом возвращает `n` вершин на окружности заданного радиуса. | [`PolygonToolsCircleAndEllipseTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs) |
| `PTOOLS-CE-003` | `x` | `Circle` с дополнительным углом поворачивает нулевую вершину на заданный угол. | [`PolygonToolsCircleAndEllipseTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs) |
| `PTOOLS-CE-004` | `x` | `Ellipse` с нулевыми полуосями возвращает одноточечный polygon в центре. | [`PolygonToolsCircleAndEllipseTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs) |
| `PTOOLS-CE-005` | `x` | `Ellipse` с нулевой малой полуосью возвращает segment вдоль повёрнутой большой оси. | [`PolygonToolsCircleAndEllipseTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs) |
| `PTOOLS-CE-006` | `x` | `Ellipse` с нулевой большой полуосью возвращает segment вдоль повёрнутой малой оси. | [`PolygonToolsCircleAndEllipseTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs) |
| `PTOOLS-CE-007` | `x` | `Ellipse` с положительными полуосями возвращает `n` вершин на повернутом эллипсе. | [`PolygonToolsCircleAndEllipseTests.cs`](../../../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs) |

## Gaps

- Дополнительные сценарии на эквивалентность разных convenience-overload'ов отдельно не выделяются: они покрываются через основные перегрузки, на которые эти методы делегируют.

## Notes

- Для вырожденных эллипсов в качестве контракта взяты XML-комментарии самого `PolygonTools`: при одной нулевой полуоси должен получаться segment, а не точка.

