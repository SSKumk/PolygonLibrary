# LinearBasis Construction

## Scope

Этот файл покрывает конструкторы `LinearBasis`, базовые свойства и инварианты ортонормированного хранения.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `LB-CTOR-001` | `x` | `LinearBasis(spaceDim, 0)` создаёт пустой базис в заданном пространстве. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-002` | `x` | Пустой базис корректно выставляет `Empty`, `SpaceDim`, `SubSpaceDim` и `FullDim`. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-003` | `x` | Для пустого базиса `Basis` и `ProjMatrix` недоступны и выбрасывают `ArgumentException`. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-004` | `x` | Конструктор из одного ненулевого вектора нормализует его и создаёт одномерный базис. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-005` | `x` | Конструктор из нулевого вектора выбрасывает `ArgumentException`. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-006` | `x` | `LinearBasis(spaceDim)` создаёт стандартный полный базис. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-007` | `x` | `LinearBasis(spaceDim, subSpaceDim)` создаёт начальный стандартный базис нужной размерности. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-008` | `x` | Конструктор из набора векторов выделяет линейно независимую часть и ортонормирует её. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-009` | `x` | Копирующий конструктор создаёт независимую копию при `needCopy = true`. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-010` | `x` | Конструктор слияния двух базисов объединяет подпространства без дублирования зависимых векторов. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) |
| `LB-CTOR-011` | `x` | Любой непустой сконструированный базис сохраняет ортонормированность векторов. | [`LinearBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs), [`LinearBasisAssert.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisAssert.cs) |

## Gaps

- Нет.

## Notes

- Ортонормированность проверяется отдельным helper-слоем, чтобы не дублировать одно и то же по тестам.
