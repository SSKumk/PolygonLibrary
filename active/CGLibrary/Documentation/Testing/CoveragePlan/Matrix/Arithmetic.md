# Matrix Arithmetic

## Scope

Этот файл покрывает арифметику `Matrix` и умножения на матрицы и векторы.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `MAT-AR-001` | `x` | Унарный минус меняет знак всех элементов матрицы. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-002` | `x` | Сложение одинаково размерных матриц выполняется покомпонентно. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-003` | `x` | Вычитание одинаково размерных матриц выполняется покомпонентно. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-004` | `x` | Умножение матрицы на скаляр слева умножает все элементы на число. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-005` | `x` | Умножение матрицы на скаляр справа эквивалентно левому умножению. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-006` | `x` | Деление матрицы на скаляр делит все элементы на число. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-007` | `x` | Умножение двух матриц даёт результат правильной размерности. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-008` | `x` | Умножение двух матриц вычисляется по стандартной формуле строка-на-столбец. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-009` | `x` | Умножение матрицы на вектор справа даёт корректный результат размера `Rows`. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |
| `MAT-AR-010` | `x` | `MultRowVectorByMatrix(v, m)` даёт корректный результат размера `Cols`. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-AR-011` | `x` | `MultiplyTransposedByVector(v)` эквивалентно умножению `Transpose() * v` в эталонном примере. | [`MatrixLinearOperationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) |
| `MAT-AR-012` | `x` | `MultiplyBySelfTranspose()` возвращает симметричную матрицу `M * M^T` для квадратных и прямоугольных входов. | [`MatrixArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) |

## Gaps

- Нет.

## Notes

- Для умножения удобно иметь один-два ручных эталона, где ожидаемый результат считается на бумаге.
- `MultiplyBySelfTranspose()` полезно проверять и на прямоугольной матрице.
