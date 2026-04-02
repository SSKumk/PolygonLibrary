# Factories And Metrics

## Scope

Сценарии:

- `Zero`, `Cube01_VRep`, `Cube01_HRep`, `RectAxisParallel`, `Ball_1`, `Ball_oo`;
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
| CPT-MET-001 | x | `MinimalDiameter` и `MinDistBtwVs` возвращают минимальное попарное расстояние между вершинами. | [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L92) |

## Existing Tests

- Старые алгоритмические тесты активно использовали эти фабрики как данность, но не документировали их прямым набором.
- В новой структуре они закреплены отдельными unit-like сценариями малой стоимости.
- Для `Ball_1` добавлен отдельный сценарий с ненулевым центром: именно он выявил и закрепил баг в формуле сдвига.

## Gaps

- `SimplexRND`, `Cyclic`, `Sphere` и `Ellipsoid` пока оставлены за пределами активного слоя этого этапа.


