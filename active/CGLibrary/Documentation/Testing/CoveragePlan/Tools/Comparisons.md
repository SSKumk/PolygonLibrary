# Tools Comparisons

## Scope

Этот файл покрывает контракт сравнения чисел с учётом `Eps`:

- обновление `Eps` и `EpsG`;
- `CMP`, `EQ`, `NE`, `GT`, `GE`, `LT`, `LE`;
- `TNumComparer`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `TLS-CMP-001` | `x` | Установка положительного `Eps` обновляет `EpsG` как `100 * Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-002` | `x` | `EQ(a)` считает число нулём, если `|a| < Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-003` | `x` | `EQ(a)` не считает число нулём, если `|a| >= Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-004` | `x` | `EQ(a, b)` эквивалентен проверке `EQ(a - b)`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-005` | `x` | `NE(a, b)` является логическим отрицанием `EQ(a, b)`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-006` | `x` | `GT(a)` возвращает `true` только для значений строго больше `Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-007` | `x` | `LT(a)` возвращает `true` только для значений строго меньше `-Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-008` | `x` | `GE(a)` считает граничное значение `-Eps` принадлежащим допустимой области. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-009` | `x` | `LE(a)` считает граничное значение `Eps` принадлежащим допустимой области. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-010` | `x` | `CMP(a)` возвращает `0` для значений в окрестности нуля по `Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-011` | `x` | `CMP(a)` возвращает `+1` для чисел строго больше `Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-012` | `x` | `CMP(a)` возвращает `-1` для чисел строго меньше `-Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-013` | `x` | `CMP(a, b)` согласован с `EQ`, `GT` и `LT` для пары чисел. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-CMP-014` | `x` | `TNumComparer.Compare(a, b)` делегирует в `CMP(a, b)` и использует текущий `Eps`. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |

## Gaps

- Нет.

## Notes

- Для `double` нужно обязательно иметь тесты с числами чуть меньше, ровно на границе и чуть больше границы точности.
- При написании тестов на `Eps` полезно сохранять и восстанавливать исходное значение, чтобы не создавать скрытой межтестовой зависимости.
