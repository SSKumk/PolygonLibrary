# Vector Geometry

## Scope

Этот файл покрывает геометрический контракт `Vector`:

- длину и квадрат длины;
- нормировку;
- углы, косинусы и взаимное расположение;
- подпространственные операции вроде `SubVector`, `OuterProduct`, `ProjectTo2DAffineSpace`, `LiftUp`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `VEC-GEO-001` | `x` | `Length2` равен сумме квадратов координат. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-002` | `x` | `Length` равен квадратному корню из `Length2`. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-003` | `x` | `Length` и `Length2` корректны для нулевого вектора. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-004` | `x` | `Normalize()` возвращает вектор единичной длины, сонаправленный исходному ненулевому вектору. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-005` | `x` | `NormalizeZero()` возвращает нулевой вектор для нулевого аргумента. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-006` | `x` | `NormalizeZero()` совпадает с `Normalize()` для ненулевого вектора. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-007` | `x` | `CosAngle(v1, v2)` возвращает `1` при участии нулевого вектора согласно текущему контракту реализации. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-008` | `x` | `CosAngle(v1, v2)` равен `1` для сонаправленных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-009` | `x` | `CosAngle(v1, v2)` равен `-1` для противоположно направленных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-010` | `x` | `CosAngle(v1, v2)` равен `0` для ортогональных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-011` | `x` | `Angle(v1, v2)` возвращает `0` для сонаправленных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-012` | `x` | `Angle(v1, v2)` возвращает `pi` для противоположно направленных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-013` | `x` | `Angle(v1, v2)` возвращает `pi / 2` для ортогональных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-014` | `x` | `AreParallel` возвращает `true` для сонаправленных и противонаправленных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-015` | `x` | `AreCodirected` возвращает `true` только для сонаправленных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-016` | `x` | `AreCounterdirected` возвращает `true` только для противонаправленных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-017` | `x` | `AreOrthogonal` возвращает `true` для ортогональных векторов. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-018` | `x` | `AreOrthogonal` считает нулевой вектор ортогональным любому вектору согласно текущему контракту. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-019` | `x` | `OuterProduct(v)` создаёт матрицу размера `SpaceDim x v.SpaceDim` с элементами `this[i] * v[j]`. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-020` | `x` | `SubVector(start, end)` возвращает непрерывный фрагмент координат в правильном порядке. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-021` | `x` | `LiftUp(d, val)` сохраняет исходные координаты и дополняет хвост значением `val`. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |
| `VEC-GEO-022` | `x` | `ProjectTo2DAffineSpace(o, u1, u2)` корректно проектирует точку на заданную аффинную плоскость в простом ортонормированном случае. | [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) |

## Gaps

- Нет.

## Notes

- Для углов и отношений удобнее использовать небольшие ортонормированные примеры в 2D и 3D.
- Для `ProjectTo2DAffineSpace` сначала стоит зафиксировать простой базис, где ожидаемый результат можно посчитать вручную.
