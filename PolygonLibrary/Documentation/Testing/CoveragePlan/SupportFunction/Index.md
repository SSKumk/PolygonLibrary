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
| Initialization | `~` | Есть прямые тесты на создание из массива пар; конструктор по точкам и часть краевых случаев ещё не размечены. |
| Evaluation and Search | `x` | Есть прямые тесты на `FuncVal` и `FindCone`. |
| Combination and Convexification | `~` | Есть прямой тест на `CombineFunctions`, но не на выпуклификацию. |

## Existing Test Sources

- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs)
- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs)
- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs)
- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs)

## Notes

- `SupportFunction` тесно зависит от `GammaPair`, поэтому некоторые баги здесь будут выглядеть как баги сортировки/сравнения пар, и наоборот.



