# Distance Epigraph

## Scope

Сценарии для:

- `BuildDistanceEpigraph`;
- `BuildDistanceEpigraph_L1`, `BuildDistanceEpigraph_Linf`, `BuildDistanceEpigraph_L2`;
- `BuildDistanceEpigraph_ToPolytope_L1`, `BuildDistanceEpigraph_ToPolytope_Linf`, `BuildDistanceEpigraph_ToPolytope_L2`.

Здесь фиксируется базовая геометрия эпиграфов расстояния на малых 2D-примерах. Тяжёлые алгоритмические и высокоразмерные сценарии остаются отдельным слоем.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CPT-DIST-001 | x | `BuildDistanceEpigraph(unitBall, point, k)` строит конус с вершиной в `point` на высоте `0` и поднятым масштабированным основанием на высоте `k`. | [`ConvexPolytopDistanceEpigraphTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopDistanceEpigraphTests.cs#L9) |
| CPT-DIST-002 | x | `BuildDistanceEpigraph_L1` для точки в 2D строит конус над ромбом `Ball_1(point, k)` на высоте `k`. | [`ConvexPolytopDistanceEpigraphTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopDistanceEpigraphTests.cs#L27) |
| CPT-DIST-003 | x | `BuildDistanceEpigraph_Linf` для точки в 2D строит конус над квадратом `Ball_oo(point, k)` на высоте `k`. | [`ConvexPolytopDistanceEpigraphTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopDistanceEpigraphTests.cs#L44) |
| CPT-DIST-004 | x | `BuildDistanceEpigraph_L2` в 2D согласован с `BuildDistanceEpigraph` и сферой той же дискретизации. | [`ConvexPolytopDistanceEpigraphTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopDistanceEpigraphTests.cs#L61) |
| CPT-DIST-005 | x | `BuildDistanceEpigraph_ToPolytope_L1` для одноточечного политопа совпадает с точечной версией. | [`ConvexPolytopDistanceEpigraphTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopDistanceEpigraphTests.cs#L71) |
| CPT-DIST-006 | x | `BuildDistanceEpigraph_ToPolytope_Linf` для одноточечного политопа совпадает с точечной версией. | [`ConvexPolytopDistanceEpigraphTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopDistanceEpigraphTests.cs#L82) |
| CPT-DIST-007 | x | `BuildDistanceEpigraph_ToPolytope_L2` для одноточечного политопа совпадает с точечной версией. | [`ConvexPolytopDistanceEpigraphTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopDistanceEpigraphTests.cs#L93) |

## Existing Tests

- До этого прямого покрытия на этот API не было: сценарии жили только косвенно через более крупные алгоритмы и прикладной код.
- Текущий слой фиксирует малые контрольные примеры, на которых легко глазами проверить форму вершины и верхнего основания.

## Gaps

- Поведение на нетривиальных политопах выше одноточечного пока не закреплено прямыми тестами.
- `DistTo_MakeBase` пока проверяется косвенно через публичные обёртки.
- Высокоразмерные сценарии и согласованность с `Polar`/duality остаются отдельным слоем.
