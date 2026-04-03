# Factories And Metrics

## Scope

Сценарии:

- `Zero`, `Cube01_VRep`, `Cube01_HRep`, `RectAxisParallel`, `Ball_1`, `Ball_oo`;
- `Sphere`, `Ellipsoid`, `Ball_2FuncCreator`;
- `MinimalDiameter`, `MinDistBtwVs`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CPT-FAC-001 | x | `Zero` создаёт одноточечный 1D-политоп в начале координат. | [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L9) |
| CPT-FAC-002 | x | `Cube01_VRep` и `Cube01_HRep` описывают один и тот же 0-1-куб. | [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L19) |
| CPT-FAC-003 | x | `RectAxisParallel` возвращает все вершины прямоугольника по двум противоположным углам. | [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L31) |
| CPT-FAC-004 | x | `Ball_1` в 2D даёт ромб с вершинами на координатных осях. | [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L46) |
| CPT-FAC-005 | x | `Ball_1` при ненулевом центре корректно сдвигает ромб вокруг заданной точки. | [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L63) |
| CPT-FAC-006 | x | `Ball_oo` в 2D даёт осепараллельный квадрат. | [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L78) |
| CPT-FAC-007 | x | `Ball_oo` при ненулевом центре корректно сдвигает квадрат вокруг заданной точки. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L9) |
| CPT-FAC-008 | x | `Sphere` в 1D возвращает отрезок `center ± radius`. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L23) |
| CPT-FAC-009 | x | `Sphere` в 2D при четырёх азимутальных делениях возвращает четыре кардинальные точки вокруг центра. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L35) |
| CPT-FAC-010 | x | `Sphere` в 2D не зависит от `polarDivision`: при фиксированном `azimuthsDivisions` результат остаётся тем же. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L52) |
| CPT-FAC-011 | x | `Sphere` в 3D при `azimuthsDivisions = 4`, `polarDivision = 2` даёт шесть осевых вершин октаэдра. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L60) |
| CPT-FAC-012 | x | `Ellipsoid` в 1D возвращает отрезок `center ± semiAxis`. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L77) |
| CPT-FAC-013 | x | `Ellipsoid` в 2D при четырёх азимутальных делениях возвращает осевые вершины, масштабированные по полуосям. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L95) |
| CPT-FAC-014 | x | `Ellipsoid` в 2D не зависит от `polarDivision`: при фиксированном `azimuthsDivisions` результат остаётся тем же. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L113) |
| CPT-FAC-015 | x | `Ball_2FuncCreator` порождает ту же фабрику, что и прямой вызов `Sphere` с теми же параметрами сетки. | [`ConvexPolytopBallFactoriesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopBallFactoriesTests.cs#L131) |
| CPT-MET-001 | x | `MinimalDiameter` и `MinDistBtwVs` возвращают минимальное попарное расстояние между вершинами. | [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L92) |

## Existing Tests

- Старые алгоритмические тесты активно использовали эти фабрики как данность, но не документировали их прямым набором.
- В новой структуре они закреплены отдельными unit-like сценариями малой стоимости.
- Для `Ball_1` добавлен отдельный сценарий с ненулевым центром: именно он выявил и закрепил баг в формуле сдвига.
- Для `Sphere` и `Ellipsoid` отдельно зафиксировано, что в 2D параметр `polarDivision` на геометрию не влияет.

## Gaps

- `SimplexRND` и `Cyclic` пока оставлены за пределами активного слоя этого этапа.


