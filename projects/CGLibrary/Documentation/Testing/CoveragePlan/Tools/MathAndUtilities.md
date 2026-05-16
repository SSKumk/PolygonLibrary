# Tools Math And Utilities

## Scope

Этот файл покрывает оставшийся базовый контракт `Tools`:

- числовые константы;
- инициализацию одномерных и двумерных массивов;
- `Sign`, `Swap`, `Atan2`, `Abs`;
- `GetCombinations`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `TLS-UTIL-001` | `x` | Константы `Zero`, `HalfOne`, `One`, `MinusOne`, `Two`, `Six` имеют ожидаемые значения для `double`. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-002` | `x` | Константы `PI`, `HalfPI` и `PI2` согласованы между собой. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-003` | `x` | `InitTNumArray(k)` создаёт массив длины `k`, заполненный нулями. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-004` | `x` | `InitTNum2DArray(row, col)` создаёт двумерный массив нужного размера, заполненный нулями. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-005` | `x` | `Sign(x)` согласован с `CMP(x)` для отрицательных, нулевых и положительных значений. | [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) |
| `TLS-UTIL-006` | `x` | `Swap(ref a, ref b)` меняет местами значения для value type. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-007` | `x` | `Swap(ref a, ref b)` меняет местами ссылки для reference type. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-008` | `x` | `Atan2(0, 0)` возвращает `0`. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-009` | `x` | `Atan2(y, x)` корректно работает в первой четверти. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-010` | `x` | `Atan2(y, x)` корректно работает во второй четверти. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-011` | `x` | `Atan2(y, x)` корректно работает в третьей четверти. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-012` | `x` | `Atan2(y, x)` корректно работает в четвёртой четверти. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-013` | `x` | `Atan2(y, x)` корректно работает на положительной полуоси `Ox`. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-014` | `x` | `Atan2(y, x)` корректно работает на отрицательной полуоси `Ox`. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-015` | `x` | `Atan2(y, x)` корректно работает на положительной полуоси `Oy`. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-016` | `x` | `Atan2(y, x)` корректно работает на отрицательной полуоси `Oy`. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-017` | `x` | `Abs(x)` возвращает `0` для значений, попадающих в нулевую окрестность по `Eps`. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-018` | `x` | `Abs(x)` возвращает положительное значение для отрицательного аргумента вне окрестности нуля. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-019` | `x` | `GetCombinations(n, k)` перечисляет все комбинации без повторов в лексикографическом порядке. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-020` | `x` | `GetCombinations(n, 1)` возвращает все одноэлементные комбинации. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |
| `TLS-UTIL-021` | `x` | `GetCombinations(n, n)` возвращает ровно одну комбинацию, содержащую все индексы. | [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) |

## Gaps

- Нет.

## Notes

- Для `Atan2` стоит использовать компактный набор эталонных точек на осях и в четвертях.
- Для `GetCombinations` важно проверять не только количество результатов, но и конкретную последовательность.
