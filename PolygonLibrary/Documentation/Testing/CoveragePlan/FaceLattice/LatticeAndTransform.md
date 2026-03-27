# Lattice And Transform

## Scope

Сценарии для `FaceLattice`:

- конструкторы по одной вершине и по готовым уровням;
- агрегаты `Vertices`, `NumberOfKFaces`, `NumberOfNonZeroKFaces`, индексатор;
- перечисление всех нетоповых граней;
- преобразование вершин через `VertexTransform`;
- сравнение решёток и `GetHashCode`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| FL-CTOR-001 | x | Конструктор по точке создаёт одноуровневую решётку с единственной вершиной в топе. | [`FaceLatticeStructureAndTransformTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeStructureAndTransformTests.cs#L9) |
| FL-CTOR-002 | x | Конструктор по готовым уровням принимает единственный top-node и корректно считает агрегаты решётки. | [`FaceLatticeStructureAndTransformTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeStructureAndTransformTests.cs#L24) |
| FL-AGG-001 | x | `AllKfaces_ExceptTop` возвращает все грани ниже топа без самого политопа. | [`FaceLatticeStructureAndTransformTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeStructureAndTransformTests.cs#L39) |
| FL-TR-001 | x | `VertexTransform` преобразует вершины и сохраняет число уровней и количество узлов на каждом уровне. | [`FaceLatticeStructureAndTransformTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeStructureAndTransformTests.cs#L52) |
| FL-EQ-001 | x | `Equals` различает эквивалентную и геометрически сдвинутую решётку, а также разные по высоте решётки. | [`FaceLatticeStructureAndTransformTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeStructureAndTransformTests.cs#L74) |
| FL-OVR-001 | x | `GetHashCode` публично запрещён и бросает `InvalidOperationException`. | [`FaceLatticeStructureAndTransformTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeStructureAndTransformTests.cs#L89) |

## Existing Tests

- На саму `FaceLattice` legacy-покрытия почти не было; в новой структуре добавлены прямые тесты на контейнерный уровень и преобразование вершин.

## Gaps

- Internal-конструкторы из `FLNodeSum` и `BaseSubCP` пока оставлены вне обязательного минимума этой миграции.

