# Matrix Construction And Access

## Scope

Этот файл покрывает создание матриц, размеры, индексаторы и преобразование к массивам.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `MAT-CTOR-001` | `x` | Конструктор по умолчанию создаёт матрицу `1x1`, заполненную нулём. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-002` | `x` | Конструктор `Matrix(row, col, ar, true)` копирует входной массив. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-003` | `x` | Конструктор `Matrix(row, col, ar, false)` использует исходный массив без копирования. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-004` | `x` | Конструктор `Matrix(row, matrix, true)` берёт первые `row` строк исходной матрицы и создаёт независимую копию. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-005` | `x` | Конструктор из двумерного массива сохраняет размеры и порядок обхода по строкам. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-006` | `x` | Копирующий конструктор с `needCopy = true` создаёт независимую копию. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-007` | `x` | Копирующий конструктор с `needCopy = false` делит внутреннее хранилище с исходной матрицей. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-008` | `x` | Конструктор из `Vector` создаёт матрицу из одной строки. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-009` | `x` | Конструктор из списка `Vector` создаёт матрицу, где каждый вектор становится отдельной строкой. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-010` | `x` | Свойства `Rows` и `Cols` отражают реальные размеры матрицы. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-011` | `x` | Индексатор `[i, j]` возвращает правильный элемент по координатам строки и столбца. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-012` | `x` | Индексатор `[i]` возвращает правильный элемент внутреннего линейного хранения. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-013` | `x` | Неявное приведение к `TNum[,]` создаёт двумерный массив с тем же содержимым и без разделения состояния. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |
| `MAT-CTOR-014` | `x` | Явное приведение из `TNum[,]` создаёт эквивалентную матрицу без разделения состояния. | [`MatrixConstructionAndAccessTests.cs`](../../../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) |

## Gaps

- Нет.

## Notes

- Для сценариев с разделяемым хранилищем удобно использовать `MatrixMutable` как инструмент наблюдения эффекта.
- Стоит проверять не только значения, но и форму результирующей матрицы.
