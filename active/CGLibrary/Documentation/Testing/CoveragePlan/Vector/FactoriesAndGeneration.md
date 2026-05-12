# Vector Factories And Generation

## Scope

Этот файл покрывает стандартные фабрики и генераторы `Vector`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `VEC-FAC-001` | `x` | `Zero(dim)` создаёт нулевой вектор нужной размерности. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |
| `VEC-FAC-002` | `x` | `MakeOrth(dim, pos)` создаёт базисный вектор с единицей в позиции `pos`. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |
| `VEC-FAC-003` | `x` | `Ones(dim)` создаёт вектор из единиц. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |
| `VEC-FAC-004` | `x` | `GenVector(dim)` никогда не возвращает нулевой вектор. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |
| `VEC-FAC-005` | `x` | `GenVector(dim, a, b)` возвращает вектор нужной размерности. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |
| `VEC-FAC-006` | `x` | `GenVector(dim, a, b)` генерирует координаты в пределах заданных границ генератора. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |
| `VEC-FAC-007` | `x` | `GenVectorInt(dim, a, b)` возвращает вектор нужной размерности. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |
| `VEC-FAC-008` | `x` | `GenVectorInt(dim, a, b)` генерирует целочисленные координаты в пределах заданных границ генератора. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |
| `VEC-FAC-009` | `x` | При использовании фиксированного генератора псевдослучайных чисел генерация воспроизводима. | [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) |

## Gaps

- Нет.

## Notes

- Для генераторов нужно опираться на детерминированный генератор или фиксированный seed, иначе тесты быстро станут хрупкими.
- Для `GenVector(dim)` важно проверять не только диапазон, но и запрет нулевого результата.
