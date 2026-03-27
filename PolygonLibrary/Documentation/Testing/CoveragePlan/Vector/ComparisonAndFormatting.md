# Vector Comparison And Formatting

## Scope

Этот файл покрывает сравнение и представление `Vector`:

- `CompareTo` и операторы сравнения;
- `Equals`;
- `ToString` и `ToStringBraceAndDelim`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `VEC-CMP-001` | `x` | `CompareTo` возвращает `0` для векторов, совпадающих с точностью `Eps`. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-002` | `x` | `CompareTo` использует лексикографический порядок по координатам. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-003` | `x` | `CompareTo` считает `null` меньшим, чем любой реальный вектор. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-004` | `x` | Оператор `==` согласован с `CompareTo == 0`. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-005` | `x` | Оператор `!=` согласован с `CompareTo != 0`. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-006` | `x` | Операторы `<`, `<=`, `>` и `>=` согласованы с `CompareTo`. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-007` | `x` | `Equals(object)` возвращает `true` для эквивалентного `Vector`. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-008` | `x` | `Equals(object)` возвращает `false` для объекта другого типа. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-009` | `x` | `ToString()` использует круглые скобки и запятую как разделитель. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-010` | `x` | `ToStringBraceAndDelim` применяет пользовательские скобки и разделитель. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-011` | `x` | `ToStringBraceAndDelim` корректно работает при отсутствии открывающей или закрывающей скобки. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |
| `VEC-CMP-012` | `x` | Строковое представление использует инвариантную культуру и не зависит от локали процесса. | [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) |

## Gaps

- Нет.

## Notes

- Для лексикографического порядка полезно иметь пары, различающиеся в первой, средней и последней координате.
- Для строкового представления стоит фиксировать именно формат разделителей, а не только наличие чисел.
