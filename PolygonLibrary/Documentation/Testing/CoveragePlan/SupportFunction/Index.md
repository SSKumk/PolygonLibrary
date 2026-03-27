# SupportFunction

## Scope

Класс [`SupportFunction.cs`](../../../../CGLibrary/Polygons/ConvexPolygons/SupportFunction.cs) описывает кусочно-линейную положительно однородную опорную функцию:

- инициализацию из `GammaPair` или набора точек;
- локализацию направления в конусе соседних нормалей;
- вычисление значения функции;
- линейную комбинацию двух функций;
- локальную выпуклификацию результата.

## Topics

- [`Initialization.md`](Initialization.md) - построение из пар и точек, сортировка, фильтрация, нормализация.
- [`EvaluationAndSearch.md`](EvaluationAndSearch.md) - `FindCone`, `ConicCombination`, `FuncVal`.
- [`CombinationAndConvexification.md`](CombinationAndConvexification.md) - `CombineFunctions`, `CheckTriple`, `ConvexifyFunctionWithInfo`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Initialization | `x` | Перенесён прямой набор на инициализацию из `GammaPair` и из точек. |
| Evaluation and Search | `x` | Перенесён legacy-набор на `FuncVal` и `FindCone`, дополнен `ConicCombination`. |
| Combination and Convexification | `x` | Перенесён `CombineFunctions`, добавлены прямые тесты на `CheckTriple` и `ConvexifyFunctionWithInfo`. |

## Existing Test Sources

- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs)
- [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs)
- [`SupportFunctionEvaluationAndSearchTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionEvaluationAndSearchTests.cs)
- [`SupportFunctionCombinationAndConvexificationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs)

## Notes

- `SupportFunction` тесно зависит от `GammaPair`, поэтому прямой набор здесь нужен именно для отделения локальных ошибок функции от багов в сравнении/нормализации пар.



