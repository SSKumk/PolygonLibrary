# Construction And Representations

## Scope

Сценарии:

- `CreateFromPoints`, `CreateFromHalfSpaces`, `CreateFromFaceLattice`;
- `WhichRep`, `Is*rep`, `SpaceDim`, `PolytopDim`;
- lazy-доступ к `Vrep`, `Hrep`, `FLrep`, `fVector`, `InnerPoint`;
- `GetInVrep`, `GetInHrep`, `GetInFLrep`;
- `ToConvexPolygon`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CPT-CTOR-001 | x | `CreateFromPoints` без convexify сохраняет все поданные точки как `Vrep` и оставляет полигон в `Vrep`-приоритете. | [`ConvexPolytopConstructionAndRepresentationTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs#L9) |
| CPT-CTOR-002 | x | `CreateFromPoints(..., true)` убирает внутренние точки, строит `FLrep` и сохраняет legacy-контракт "The set of vertices must be equal.". | [`ConvexPolytopConstructionAndRepresentationTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs#L24) |
| CPT-CTOR-003 | x | `CreateFromHalfSpaces` создаёт `Hrep`-политоп и делает доступными point queries. | [`ConvexPolytopConstructionAndRepresentationTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs#L37) |
| CPT-CTOR-004 | x | `CreateFromFaceLattice` использует переданную решётку и корректно вычисляет `fVector`. | [`ConvexPolytopConstructionAndRepresentationTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs#L53) |
| CPT-REP-001 | x | `GetInFLrep`, `GetInHrep` и `GetInVrep` строят эквивалентные политопы с нужным приоритетом представления. | [`ConvexPolytopConstructionAndRepresentationTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs#L69) |
| CPT-REP-002 | x | `ToConvexPolygon` для 2D-политопа проектирует вершины в заданный аффинный базис. | [`ConvexPolytopTransformsAndOverridesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L109) |
| CPT-REP-003 | x | `ToConvexPolygon` для политопа размерности, отличной от 2, бросает `ArgumentException`. | [`ConvexPolytopTransformsAndOverridesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L136) |

## Existing Tests

- Из `GW_Tests` перенесён центральный инвариант про совпадение множества вершин после convexify.
- Дополнительно выделены прямые unit-like проверки `WhichRep`, `InnerPoint`, `fVector` и переходов между представлениями.

## Gaps

- `CreateFromReader` и связанные file-based сценарии сознательно не входят в активный слой этого этапа.


