# SupportFunction Evaluation And Search

## Scope

Этот файл покрывает локализацию направления в конусе нормалей и вычисление значения функции.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SF-EVAL-001` | `x` | `FuncVal(v)` корректно вычисляет значение функции на направлениях, покрытых эталонным набором. | [`SupportFunctionEvaluationAndSearchTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionEvaluationAndSearchTests.cs) |
| `SF-EVAL-002` | `x` | `FindCone(v)` находит правильную пару соседних нормалей для набора тестовых направлений. | [`SupportFunctionEvaluationAndSearchTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionEvaluationAndSearchTests.cs) |
| `SF-EVAL-003` | `x` | `FindCone(v)` для направления, совпадающего с последней нормалью, возвращает конус `(last, first)`. | [`SupportFunctionEvaluationAndSearchTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionEvaluationAndSearchTests.cs) |
| `SF-EVAL-004` | `x` | `FindCone(v)` для направления, совпадающего с первой нормалью, возвращает конус `(0, 1)`. | [`SupportFunctionEvaluationAndSearchTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionEvaluationAndSearchTests.cs) |
| `SF-EVAL-005` | `x` | `ConicCombination(v, i, j)` восстанавливает коэффициенты разложения в выбранном конусе. | [`SupportFunctionEvaluationAndSearchTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionEvaluationAndSearchTests.cs) |
| `SF-EVAL-006` | `x` | `FuncVal(v, i, j)` даёт тот же результат, что `FuncVal(v)` после автоматического поиска конуса. | [`SupportFunctionEvaluationAndSearchTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionEvaluationAndSearchTests.cs) |

## Gaps

- В mandatory-сценариях пробелов не осталось.

## Notes

- Исторические сообщения `FindCone1: test #...` и `FindCone2: test #...` сохранены без потери смысла.

