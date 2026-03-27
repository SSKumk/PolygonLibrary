# Vector Arithmetic

## Scope

Этот файл покрывает арифметические операции `Vector` и построение линейных комбинаций.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `VEC-AR-001` | `x` | Унарный минус меняет знак всех координат. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-002` | `x` | Сложение двух векторов одинаковой размерности выполняется покоординатно. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-003` | `x` | Вычитание двух векторов одинаковой размерности выполняется покоординатно. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-004` | `x` | Левое умножение на число умножает все координаты на скаляр. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-005` | `x` | Правое умножение на число эквивалентно левому умножению. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-006` | `x` | Деление на число делит все координаты на скаляр. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-007` | `x` | Скалярное произведение вычисляется как сумма попарных произведений координат. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-008` | `x` | `Sum(IEnumerable<Vector>)` возвращает сумму всей последовательности векторов. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-009` | `x` | `LinearCombination(v1, w1, v2, w2)` строит покоординатную линейную комбинацию двух векторов. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-010` | `x` | `LinearCombination(Vs, Ws)` корректно обрабатывает несколько векторов и весов. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-011` | `x` | `MulByNumAndAdd(v1, a, v2)` эквивалентен вычислению `v1 * a + v2`. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |
| `VEC-AR-012` | `x` | `AffMul(v1, origin, v2)` эквивалентен скалярному произведению `(v1 - origin) * v2`. | [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) |

## Gaps

- Нет.

## Notes

- Для арифметики полезно использовать маленькие целые координаты, чтобы ожидаемые значения читались без вычислений в уме.
- Отдельно пригодятся сценарии с нулевым скаляром и отрицательным скаляром.
