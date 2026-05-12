# SupportFunction Combination And Convexification

## Scope

Этот файл покрывает комбинирование двух опорных функций и последующую локальную выпуклификацию.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SF-COMB-001` | `x` | `CombineFunctions(fa, fb, ca, cb)` корректно складывает значения на совпадающих нормалях. | [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs) |
| `SF-COMB-002` | `x` | `CombineFunctions` корректно подмешивает нормали только из первой функции. | [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs) |
| `SF-COMB-003` | `x` | `CombineFunctions` корректно подмешивает нормали только из второй функции и заполняет “подозрительные” позиции. | [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs) |
| `SF-COMB-004` | `x` | `CheckTriple(pm, pc, pp)` считает среднюю пару допустимой, если она лежит строго ниже вершины, заданной соседями. | [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs), [`SupportFunctionProbe.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionProbe.cs) |
| `SF-COMB-005` | `x` | `CheckTriple(pm, pc, pp)` корректно работает в специальном случае противоположных крайних нормалей. | [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs), [`SupportFunctionProbe.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionProbe.cs) |
| `SF-COMB-006` | `x` | `ConvexifyFunctionWithInfo` возвращает ту же функцию, если список подозрительных индексов пуст. | [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs) |
| `SF-COMB-007` | `x` | `ConvexifyFunctionWithInfo` удаляет локально невыпуклые пары и возвращает выпуклый результат. | [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs) |
| `SF-COMB-008` | `x` | `ConvexifyFunctionWithInfo` возвращает `null`, если выпуклая оболочка приводит к недопустимому разрыву угла не меньше `pi`. | [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs) |

## Gaps

- В mandatory-сценариях пробелов не осталось.

## Notes

- Исторические сообщения `sf11: ...`, `sf22: ...`, `sf12: ...` сохранены при переносе legacy-набора `CombineSFTest`.

