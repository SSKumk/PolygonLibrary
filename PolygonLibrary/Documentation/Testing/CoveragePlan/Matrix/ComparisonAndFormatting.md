# Matrix Comparison And Formatting

## Scope

Этот файл покрывает `Equals`, `CompareTo`, `GetHashCode` и строковое представление `Matrix`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `MAT-CMP-001` | `x` | `Equals(Matrix)` возвращает `true` для матриц одинаковой формы и совпадающих элементов. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-002` | `x` | `Equals(Matrix)` возвращает `false` для матриц разной формы. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-003` | `x` | `Equals(Matrix)` возвращает `false` для матриц одинаковой формы, но с отличающимися элементами. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-004` | `x` | `Equals(Matrix)` использует сравнение с точностью `Eps`. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-005` | `x` | `GetHashCode()` для `Matrix` остаётся запрещённой операцией и выбрасывает `InvalidOperationException`. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-006` | `x` | `CompareTo` сначала сравнивает число строк. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-007` | `x` | `CompareTo` затем сравнивает число столбцов. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-008` | `x` | `CompareTo` при равной форме выполняет лексикографическое сравнение элементов. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-009` | `x` | `CompareTo` считает `null` меньшим, чем любая матрица, и сохраняет симметрию и транзитивность на эталонных примерах. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |
| `MAT-CMP-010` | `x` | `ToString()` выводит элементы построчно и выравнивает столбцы по ширине. | [`MatrixComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) |

## Gaps

- Нет.

## Notes

- Для `CompareTo` нужны матрицы, различающиеся сначала формой, затем содержимым.
- Для `ToString()` полезно фиксировать небольшие примеры с числами разной длины.
