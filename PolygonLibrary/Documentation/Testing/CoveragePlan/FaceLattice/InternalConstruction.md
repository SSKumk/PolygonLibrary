# Internal Construction

## Scope

Сценарии для internal-конвертеров:

- `FaceLattice.ConstructFromFLNodeSum`;
- `FaceLattice.ConstructFromBaseSubCP`.

Здесь фиксируется корректность перевода временных внутренних структур в обычную `FaceLattice` на малом треугольном примере.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| FL-INT-001 | x | `ConstructFromFLNodeSum` строит решётку, эквивалентную обычной треугольной `FaceLattice`. | [`FaceLatticeInternalConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeInternalConstructionTests.cs#L9) |
| FL-INT-002 | x | `ConstructFromBaseSubCP` строит решётку, эквивалентную обычной треугольной `FaceLattice`. | [`FaceLatticeInternalConstructionTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeInternalConstructionTests.cs#L62) |

## Existing Tests

- До этого прямого покрытия на internal-конструкторы не было.
- Оба сценария проверяются на одном и том же треугольнике, чтобы сравнение было максимально прозрачным.

## Gaps

- Пока не покрыты более сложные случаи с higher-dimensional временными структурами.
- Не покрыты патологии в самих временных данных, если `Sub`/`Faces` собраны несогласованно.
