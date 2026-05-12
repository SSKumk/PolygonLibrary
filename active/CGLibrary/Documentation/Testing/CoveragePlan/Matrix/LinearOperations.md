# Matrix Linear Operations

## Scope

Этот файл покрывает специальные фабрики и линейные операции `Matrix`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `MAT-LIN-001` | `x` | `Zero(rows, cols)` создаёт нулевую матрицу заданной формы. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-002` | `x` | `One(rows, cols)` создаёт матрицу из единиц заданной формы. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-003` | `x` | `Eye(d)` создаёт единичную матрицу размера `d x d`. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-004` | `x` | `GenMatrix(dimRow, dimCol, a, b)` создаёт матрицу нужной формы с детерминированным результатом для фиксированного генератора и элементами в пределах заданных границ генератора. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-005` | `x` | `GenMatrixInt(dimRow, dimCol, a, b)` создаёт матрицу нужной формы с целочисленными значениями в пределах заданных границ генератора. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-006` | `x` | `GenNonSingular(dim, a, b)` возвращает квадратную невырожденную матрицу. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-007` | `x` | `GenONMatrix(dim)` возвращает ортонормированную матрицу. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-008` | `x` | `Hilbert(dim)` создаёт матрицу Гильберта по формуле `1 / (i + j + 1)`. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-009` | `x` | `MultiplyRowByVector(rowInd, v)` вычисляет скалярное произведение строки на вектор. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-010` | `x` | `MultiplyRowByDiffOfVectors(rowInd, v1, v2)` эквивалентен произведению строки на `(v1 - v2)`. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-011` | `x` | `MultiplyColumnByVector(colInd, v)` вычисляет скалярное произведение столбца на вектор. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-012` | `x` | `ToRREF()` возвращает матрицу в приведённой ступенчатой форме для единичной, нулевой и типовой невырожденной матрицы. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-013` | `x` | `ToRREF()` корректно обрабатывает матрицы с линейно зависимыми строками, нулевыми столбцами и прямоугольные случаи. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-LIN-014` | `x` | `ToRREF()` не меняет матрицу, уже находящуюся в RREF, и не мутирует исходный объект. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |

## Gaps

- Нет.

## Notes

- Для `GenNonSingular` и `GenONMatrix` удобнее проверять инварианты, а не конкретные значения.
- Для `ToRREF()` полезно иметь заранее известные эталонные матрицы и их форму результата.
