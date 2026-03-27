# Matrix Mutable

## Scope

Этот файл покрывает модифицируемый слой `MatrixMutable`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `MAT-MUT-001` | `x` | Конструктор `MatrixMutable(row, col, ar, needCopy)` создаёт изменяемую матрицу с тем же содержимым, что и базовый `Matrix`. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |
| `MAT-MUT-002` | `x` | Конструктор `MatrixMutable(Matrix m, true)` создаёт независимую изменяемую копию. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |
| `MAT-MUT-003` | `x` | Индексатор с `set` изменяет нужный элемент и не затрагивает остальные. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |
| `MAT-MUT-004` | `x` | `Transpose()` возвращает изменяемую транспонированную матрицу. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |
| `MAT-MUT-005` | `x` | `Eye(d)` создаёт изменяемую единичную матрицу. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |
| `MAT-MUT-006` | `x` | `SetSubMatrix(startRow, startCol, subMatrix)` вставляет блок в правильную позицию. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |
| `MAT-MUT-007` | `x` | `SetSubVector(startRow, startCol, vector)` вставляет столбец значений в правильную позицию. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |
| `MAT-MUT-008` | `x` | `SwapRowBlocks(m, d, k)` переносит первые `k` строк вниз и сохраняет относительный порядок строк. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |
| `MAT-MUT-009` | `x` | Оператор умножения двух `MatrixMutable` возвращает корректный изменяемый результат. | [`MatrixMutableTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) |

## Gaps

- Нет.

## Notes

- Для `MatrixMutable` важно проверять именно эффект мутации, а не только итоговые значения.
- `SwapRowBlocks` удобнее тестировать на матрице, где каждая строка легко различима визуально.
