# Vector Construction And Identity

## Scope

Этот файл покрывает создание `Vector`, размерность, индексный доступ и базовые инварианты хранения координат.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `VEC-CTOR-001` | `x` | Конструктор `Vector(int n)` создаёт нулевой вектор размерности `n`. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-002` | `x` | Конструктор `Vector(IEnumerable<int>)` корректно переводит целые координаты в `double`. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-003` | `x` | Конструктор `Vector(TNum[] nv, true)` копирует входной массив. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-004` | `x` | Конструктор `Vector(TNum[] nv, false)` использует исходный массив без копирования. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-005` | `x` | Копирующий конструктор создаёт независимый эквивалентный объект. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-006` | `x` | Конструктор из `Vector2D` создаёт двумерный вектор с теми же координатами. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-007` | `x` | `SpaceDim` равен числу координат вектора. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-008` | `x` | Индексатор возвращает правильные координаты по всем допустимым индексам. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-009` | `x` | Явное приведение к `TNum[]` возвращает копию координат, а не внутреннее хранилище. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-010` | `x` | `GetCopyAsArray()` возвращает массив с теми же координатами и не делит состояние с объектом. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-011` | `x` | `IsZero` возвращает `true` для нулевого вектора. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |
| `VEC-CTOR-012` | `x` | `IsZero` возвращает `false` для вектора с координатой вне нулевой окрестности по `Eps`. | [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) |

## Gaps

- Нет.

## Notes

- Для сценариев с `needCopy` удобно использовать изменяемый массив-источник и проверять, отражаются ли его изменения в объекте.
- Для `double` полезно отдельно иметь пример с координатами около `Eps`.
