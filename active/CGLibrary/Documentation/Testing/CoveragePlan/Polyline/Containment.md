# Polyline Containment

## Scope

Этот файл покрывает точечные запросы к `Polyline`:

- `ContainsPoint`;
- `ContainsPointInside`.

Проверки проводятся и на выпуклом контуре, и на невыпуклом контуре с вогнутым вырезом.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `PLN-CNT-001` | `x` | `ContainsPoint` и `ContainsPointInside` принимают внутренние точки выпуклого квадрата. | [`PolylineContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineContainmentTests.cs) |
| `PLN-CNT-002` | `x` | Для выпуклого квадрата внешние точки справа и слева отклоняются обоими запросами. | [`PolylineContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineContainmentTests.cs) |
| `PLN-CNT-003` | `x` | Для выпуклого квадрата вершины и точки на ребре принадлежат контуру, но не внутренности. | [`PolylineContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineContainmentTests.cs) |
| `PLN-CNT-004` | `x` | `ContainsPoint` и `ContainsPointInside` принимают внутренние точки невыпуклого контура вне вогнутого выреза. | [`PolylineContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineContainmentTests.cs) |
| `PLN-CNT-005` | `x` | Для невыпуклого контура точки в вогнутом вырезе и вне внешней границы отклоняются. | [`PolylineContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineContainmentTests.cs) |
| `PLN-CNT-006` | `x` | Для невыпуклого контура вершины и точки на вогнутом ребре принадлежат контуру, но не внутренности. | [`PolylineContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineContainmentTests.cs) |
| `PLN-CNT-007` | `x` | Оба запроса корректно работают и для clockwise-порядка обхода вершин. | [`PolylineContainmentTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineContainmentTests.cs) |

## Gaps

- Сценарии для пустой полилинии не закрепляются как часть `ContainsPoint*`: код не определяет явный публичный контракт для вызова этих методов на `Count == 0`.

## Notes

- Исторические диагностические сообщения из `PolylineTests.cs` сохранены без изменения смысла и текста, чтобы миграция не потеряла полезный контекст при возможных падениях.

