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
| 0 | `Line2D` | `closed` | Пилотный перенос уже выполнен до фиксации общего backlog. |
| 1 | `Vector2D` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Vector2D`, legacy `VectorsTests.cs` исключён из компиляции. |
| 2 | `Segment` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Segment`, legacy `SegmentCrossTests.cs` исключён из компиляции. |
| 3 | `Intersection` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Intersection`, legacy `ConvexPolygonIntersectionTests.cs` исключён из компиляции. |
| 4 | `SegmentPair` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/SegmentPair` отдельным unit-like набором. |
| 5 | `Tools` | `closed` | Перенесён в `Tests/DoubleGeometry/Basics/Tools` прямым базовым набором на численные контракты. |
| 6 | `Vector` | `in_progress` | База для общей линейной алгебры. |
| 7 | `Matrix` | `pending` | Следом за `Vector`. |
| 8 | `LinearBasis` | `pending` | Опирается на `Vector` и `Matrix`. |
| 9 | `AffineBasis` | `pending` | Опирается на `LinearBasis`. |
| 10 | `HyperPlane` | `pending` | Опирается на базисы и векторы. |
| 11 | `Polyline` | `pending` | Базовая 2D-полигональная сущность. |
| 12 | `BasicPolygon` | `pending` | Низкоуровневый полигональный слой. |
| 13 | `PolygonTools` | `pending` | Вспомогательные polygon-операции. |
| 14 | `GammaPair` | `pending` | Вспомогательный объект для support-function логики. |
| 15 | `SupportFunction` | `pending` | Опирается на `Vector2D`, `GammaPair`, polygon-сценарии. |
| 16 | `ConvexPolygon` | `pending` | Основной 2D-объект более высокого уровня. |
| 17 | `FaceLattice` | `pending` | База для политопной структуры. |
| 18 | `ConvexPolytop` | `pending` | Высокоуровневая политопная сущность. |
| 19 | `HrepToFLrep` | `pending` | Преобразование представлений. |
| 20 | `GiftWrapping` | `pending` | Алгоритм на поверх базовых геометрических сущностей. |
| 21 | `MinkowskiSum` | `pending` | Алгоритмический уровень. |
| 22 | `MinkowskiDiff` | `pending` | Алгоритмический уровень. |
| 23 | `CauchyMatrix` | `pending` | Отдельный вычислительный объект; можно переносить после базовой линейной алгебры. |

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
