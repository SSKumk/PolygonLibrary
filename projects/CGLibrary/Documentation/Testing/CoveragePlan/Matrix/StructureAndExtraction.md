# Matrix Structure And Extraction

## Scope

Этот файл покрывает структурные операции `Matrix`: склейку, извлечение частей и транспонирование.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `MAT-STR-001` | `x` | `hcat(m1, m2)` горизонтально склеивает матрицы с одинаковым числом строк. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-002` | `x` | `hcat(m, v)` добавляет вектор как последний столбец. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-003` | `x` | Экземплярный `hcat(v)` эквивалентен статическому `hcat(this, v)`. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-004` | `x` | `vcat(m, v)` добавляет вектор как последнюю строку. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-005` | `x` | Экземплярный `vcat(v)` эквивалентен статическому `vcat(this, v)`. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-006` | `x` | `TakeRows(params int[] rows)` возвращает строки с заданными индексами в указанном порядке. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-007` | `x` | `TakeCols(params int[] cols)` возвращает столбцы с заданными индексами в указанном порядке. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-008` | `x` | `TakeSubMatrix(int[]? rows, int[]? cols)` извлекает правильный прямоугольный блок по наборам индексов и поддерживает `null` как выбор всех строк или столбцов. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-009` | `x` | `TakeRowVector(row)` возвращает строку как `Vector`. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-010` | `x` | `TakeColumnVector(col)` возвращает столбец как `Vector`. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-011` | `x` | `Transpose()` меняет размеры местами и переносит элементы по правилу `res[j, i] = src[i, j]`. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |
| `MAT-STR-012` | `x` | Двойное транспонирование возвращает матрицу, эквивалентную исходной. | [`MatrixStructureAndExtractionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) |

## Gaps

- Нет.

## Notes

- Для семейства `Take*` лучше использовать матрицу с уникальными значениями во всех ячейках.
- Для `Transpose()` полезно проверять как квадратную, так и прямоугольную матрицу.
