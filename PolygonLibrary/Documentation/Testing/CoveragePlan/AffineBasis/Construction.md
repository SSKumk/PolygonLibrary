# AffineBasis Construction

## Scope

Этот файл покрывает конструкторы `AffineBasis` и `AffineBasisMutable`, а также их базовые инварианты.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `AFB-CTOR-001` | `x` | `AffineBasis(vecDim)` создаёт полный стандартный аффинный базис в нуле. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-002` | `x` | `AffineBasis(origin)` создаёт нульмерное аффинное пространство с заданным началом. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-003` | `x` | `AffineBasis(origin, linearBasis, false)` корректно принимает неизменяемый `LinearBasis`. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-004` | `x` | `AffineBasis(origin, linearBasis, true)` создаёт эквивалентный аффинный базис с копированием линейной части. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-005` | `x` | Базовый конструктор `AffineBasis(origin, LinearBasisMutable, false)` запрещает передачу mutable-базиса без копирования. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-006` | `x` | `AffineBasisMutable(origin, mutableBasis, true)` создаёт независимую копию линейной части. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-007` | `x` | `AffineBasisMutable(origin, mutableBasis, false)` разделяет mutable-линейный базис с исходным объектом. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-008` | `x` | Конструктор из набора точек строит одномерное аффинное пространство для коллинеарного набора. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-009` | `x` | Конструктор из набора точек строит плоскость для плоского набора точек. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-010` | `x` | Конструктор из одной точки создаёт нульмерное аффинное пространство. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-011` | `x` | Копирующий конструктор `AffineBasisMutable(affineBasis, true)` создаёт независимую копию. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) |
| `AFB-CTOR-012` | `x` | Линейная часть любого непустого сконструированного базиса остаётся ортонормированной. | [`AffineBasisConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs), [`AffineBasisAssert.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisAssert.cs) |

## Gaps

- Нет.

## Notes

- Здесь отдельно фиксируется различие между immutable- и mutable-конструкторами для случая `needCopy: false`.
