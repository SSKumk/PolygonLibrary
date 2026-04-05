# Regression Cases

## Scope

Сценарии:

- `Sphere - axis segment`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKD-101 | `x` | `Naive` и `Geometric` совпадают для `Sphere(3D) - segment(0,z)` при representative-длинах `0.5, 1.0, 1.5, 2.0, 2.5`. | [`MinkowskiDiffRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs#L9) |

## Notes

- `Sphere_Seg0_0_z` перенесён в активный regression-слой.
- `Cyclic` пока не включён: в `double` он вскрывает реальную проблему `Geometric` через `ConvexPolytop.HrepToVrep_Geometric` и должен разбираться отдельно.
- Отдельный `Stress` пока не требуется: combinatorial и generator-based слоя здесь нет.
