# SupportFunction Evaluation And Search

## Scope

Этот файл покрывает локализацию направления в конусе нормалей и вычисление значения функции.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SF-EVAL-001` | `x` | `FuncVal(v)` корректно вычисляет значение функции на направлениях, покрытых эталонным набором. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `SF-EVAL-002` | `x` | `FindCone(v)` находит правильную пару соседних нормалей для набора тестовых направлений. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs), [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `SF-EVAL-003` | ` ` | `FindCone(v)` для направления, совпадающего с последней нормалью, возвращает конус `(last, first)`. | |
| `SF-EVAL-004` | ` ` | `FindCone(v)` для направления, совпадающего с первой нормалью, возвращает конус `(0, 1)`. | |
| `SF-EVAL-005` | ` ` | `ConicCombination(v, i, j)` восстанавливает коэффициенты разложения в выбранном конусе. | |
| `SF-EVAL-006` | ` ` | `FuncVal(v, i, j)` даёт тот же результат, что `FuncVal(v)` после автоматического поиска конуса. | |

## Gaps

- Нет отдельного прямого теста на `ConicCombination`.
- Нет явного сравнения перегрузки `FuncVal(v, i, j)` с автоматическим вариантом.

## Notes

- Для `FindCone` особенно важны направления на границах конусов и возле перехода через `-pi / pi`.



