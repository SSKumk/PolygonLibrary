# Matrix

## Scope

Класс [`CGLibrary/Basics/Matrix.cs`](../../../../../CGLibrary/Basics/Matrix.cs) задаёт матричный слой библиотеки:

- неизменяемые и изменяемые матрицы;
- базовую арифметику;
- умножение на матрицы и векторы;
- операции извлечения подматриц и транспонирования;
- специальные фабрики и линейные процедуры.

Это фундаментальный слой сразу над `Vector`, и многие алгоритмы далее опираются на корректность его базовых инвариантов.

## Topics

- [`ConstructionAndAccess.md`](ConstructionAndAccess.md) - конструкторы, размеры, индексаторы, приведения.
- [`ComparisonAndFormatting.md`](ComparisonAndFormatting.md) - равенство, сравнение, строковое представление.
- [`Arithmetic.md`](Arithmetic.md) - арифметика и умножения.
- [`StructureAndExtraction.md`](StructureAndExtraction.md) - склейка, извлечение строк, столбцов, подматриц и транспонирование.
- [`LinearOperations.md`](LinearOperations.md) - линейные функции, `ToRREF`, специальные фабрики.
- [`Mutable.md`](Mutable.md) - контракт `MatrixMutable`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Access | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Comparison and Formatting | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Arithmetic | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Structure and Extraction | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Linear Operations | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Mutable | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |

## Existing Test Sources

Потенциальный источник текущего покрытия:

- [`MatrixTests.cs`](../../../../Tests/SharedTests/MatrixTests.cs)

## Notes

- Для `Matrix` особенно важно фиксировать не только результат, но и размерность результата.
- Большую часть сценариев удобно строить на маленьких матрицах `1x1`, `2x2`, `2x3`, `3x2`.




