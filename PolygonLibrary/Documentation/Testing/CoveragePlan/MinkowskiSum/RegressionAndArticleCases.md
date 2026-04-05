# Regression And Article Cases

## Scope

Сценарии:

- компактный regression-слой из legacy;
- один article-based 3D-пример;
- отдельный контроль коммутативности `P + Q == Q + P`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKS-R-001 | x | `BySandipDas` для `Simplex3D + Simplex3D` совпадает с `ByConvexHull`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L10) |
| MKS-R-002 | x | `BySandipDas` для `Cube4D + Cube4D` совпадает с `ByConvexHull`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L18) |
| MKS-R-003 | x | Article-based 3D-пример `Cube3D + Octahedron45XY` совпадает с `ByConvexHull`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L26) |
| MKS-R-004 | x | `BySandipDas` коммутативен на паре `Simplex3D` и `Cube3D`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L37) |
| MKS-R-005 | x | Article-based `WorstCase3D` совпадает с `ByConvexHull`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L44) |
| MKS-R-006 | x | Поворотный 3D-кейс `Cube3D + Cube45XY` совпадает с `ByConvexHull`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L60) |
| MKS-R-007 | x | Два независимых квадрата в разных 2D-плоскостях 4D-пространства дают корректную сумму. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L71) |
| MKS-R-008 | x | Точка в `3D` и `Cube3D` дают корректную сумму против `ByConvexHull`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L90) |
| MKS-R-009 | x | Отрезок в `3D` и `Cube3D` дают корректную сумму против `ByConvexHull`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L99) |
| MKS-R-010 | x | Плоский квадрат в `3D` и `Cube3D` дают корректную сумму против `ByConvexHull`. | [`MinkowskiSumRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumRegressionTests.cs#L112) |

## Existing Tests

- Сценарии выбраны из legacy-набора как минимальный нетривиальный regression-слой без тяжёлых exhaustive-переборов.

## Gaps

- Самые тяжёлые 5D и массовые combinatorial-regression примеры из legacy всё ещё остаются вне активного слоя.
- Часть article-based picture-cases всё ещё остаётся вне активного слоя, но минимальный representative-набор уже перенесён.
- Mixed-dimension-in-3D representative-кейсы теперь тоже перенесены в основной regression-слой.
