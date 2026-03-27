# Vector2D Construction And Comparison

## Scope

Этот файл покрывает создание `Vector2D`, доступ к координатам, сравнение и форматирование.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `V2D-CTOR-001` | `x` | Конструктор по умолчанию создаёт нулевой вектор. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-002` | `x` | Координатный конструктор сохраняет переданные `x` и `y`. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-003` | `x` | Копирующий конструктор создаёт эквивалентный объект. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-004` | `x` | `Zero`, `E1` и `E2` имеют ожидаемые координаты. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-005` | `x` | Индексатор по `0` возвращает `x`. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-006` | `x` | Индексатор по `1` возвращает `y`. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-007` | `x` | `Length` и `Abs` совпадают как нормы вектора. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-008` | `x` | `IsZero` возвращает `true` для нулевого вектора. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-009` | `x` | `CompareToNoEps` использует точное лексикографическое сравнение без допуска. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-010` | `x` | `CompareTo` использует сравнение с учётом `Eps`. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-011` | `x` | Операторы `==` и `!=` согласованы с координатным сравнением по `Eps`. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-012` | `x` | Операторы `<`, `<=`, `>` и `>=` согласованы с `CompareTo`. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-013` | `x` | `ToString()` возвращает строку формата `(x;y)` с инвариантной культурой. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |
| `V2D-CTOR-014` | `x` | `PolarAngle` возвращает `0` для нулевого вектора согласно текущему контракту `Tools.Atan2`. | [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) |

## Gaps

- Обязательных gap-ов для текущего публичного контракта не осталось.

## Notes

- Для сравнения полезно иметь пары точек, различающиеся меньше и больше `Eps`.
- `PolarAngle` отдельно проверяется на осях и на нулевом векторе.
