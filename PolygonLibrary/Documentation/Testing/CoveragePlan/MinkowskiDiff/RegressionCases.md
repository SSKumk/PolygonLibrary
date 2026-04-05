# Regression Cases

## Scope

Сценарии:

- неосевой `2D`-случай `triangle - point`;
- неосевой `3D`-случай `rotated cube - point`;
- smooth `3D`-случаи на сфере;
- красные signal-тесты на polyhedral `3D`-входах, где `Geometric` сейчас ломается на simplex-based выборе стартовой вершины.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKD-101 | `x` | `Naive` и `Geometric` для `triangle - point` дают точный сдвиг треугольника на `-point`. | [`MinkowskiDiffRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs#L9) |
| MKD-102 | `x` | `Naive` и `Geometric` для `rotated cube - point` дают точный сдвиг повёрнутого куба на `-point`. | [`MinkowskiDiffRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs#L32) |
| MKD-103 | `todo` | `Naive` и `Geometric` для `tetrahedron - point` должны давать точный сдвиг тетраэдра на `-point`, но сейчас сигнализируют о баге vertex-selection в simplex-пути. | [`MinkowskiDiffRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs#L49) |
| MKD-104 | `todo` | `Naive` и `Geometric` для `octahedron - point` должны давать точный сдвиг октаэдра на `-point`, но сейчас сигнализируют о той же simplex-проблеме. | [`MinkowskiDiffRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs#L76) |
| MKD-105 | `x` | `Naive` и `Geometric` для `sphere - point` дают точный сдвиг аппроксимированной сферы на `-point`. | [`MinkowskiDiffRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs#L97) |
| MKD-106 | `x` | `Naive` и `Geometric` совпадают для `Sphere(3D) - segment(0,z)` при representative-длинах `0.5, 1.0, 1.5, 2.0, 2.5`. | [`MinkowskiDiffRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs#L115) |

## Notes

- Regression-слой теперь покрывает и `2D`, и `3D` на неосевых или менее симметричных примерах.
- `tetrahedron - point` и `octahedron - point` сознательно оставлены красными: они показывают, что текущий simplex-based старт в `FindInitialVertex_Simplex` может вернуть точку оптимального лица вместо вершины.
- `Cyclic` пока не включён: в `double` он вскрывает реальную проблему `Geometric` через `ConvexPolytop.HrepToVrep_Geometric` и вынесен в research.
- Отдельный `Stress` пока не требуется: combinatorial и generator-based слоя здесь нет.
