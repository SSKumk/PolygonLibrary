# SupportFunction Combination And Convexification

## Scope

Этот файл покрывает комбинирование двух опорных функций и последующую локальную выпуклификацию.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SF-COMB-001` | `x` | `CombineFunctions(fa, fb, ca, cb)` корректно складывает значения на совпадающих нормалях. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `SF-COMB-002` | `x` | `CombineFunctions` корректно подмешивает нормали только из первой функции. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `SF-COMB-003` | `x` | `CombineFunctions` корректно подмешивает нормали только из второй функции и заполняет “подозрительные” позиции. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `SF-COMB-004` | ` ` | `CheckTriple(pm, pc, pp)` считает среднюю пару допустимой, если она лежит строго ниже вершины, заданной соседями. | |
| `SF-COMB-005` | ` ` | `CheckTriple(pm, pc, pp)` корректно работает в специальном случае противоположных крайних нормалей. | |
| `SF-COMB-006` | ` ` | `ConvexifyFunctionWithInfo` возвращает ту же функцию, если список подозрительных индексов пуст. | |
| `SF-COMB-007` | ` ` | `ConvexifyFunctionWithInfo` удаляет локально невыпуклые пары и возвращает выпуклый результат. | |
| `SF-COMB-008` | ` ` | `ConvexifyFunctionWithInfo` возвращает `null`, если выпуклая оболочка приводит к недопустимому разрыву угла не меньше `pi`. | |

## Gaps

- Нет прямых тестов на `CheckTriple`.
- Нет прямых тестов на `ConvexifyFunctionWithInfo`.

## Notes

- Это одна из самых ценных зон для будущих тестов, потому что здесь легко получить редкие баги на границах углов и при смешении двух support functions.



