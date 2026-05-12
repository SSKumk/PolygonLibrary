# Stress

## Scope

Сценарии:

- тяжёлые `5D` кейсы, которые уже не относятся к компактному обязательному слою;
- oracle-backed проверки против `ByConvexHull` на representative-парах;
- устойчивость `BySandipDas` к коммутативности и аффинным преобразованиям на `5D` примере.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKS-S-001 | x | `BySandipDas` для `Cube5D + Cube5D` совпадает с `ByConvexHull`. | [`MinkowskiSumStressTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumStressTests.cs#L9) |
| MKS-S-002 | x | `BySandipDas` для `Cube5D + RandomSimplex5D` совпадает с `ByConvexHull`. | [`MinkowskiSumStressTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumStressTests.cs#L19) |
| MKS-S-003 | x | На паре `Cube5D` и `Simplex5D` сохраняются коммутативность, инвариантность к повороту и ожидаемый сдвиг суммы. | [`MinkowskiSumStressTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumStressTests.cs#L30) |
| MKS-S-004 | x | Поднятый `Cube2D` в `5D` и `Cube5D` дают корректную сумму против `ByConvexHull`. | [`MinkowskiSumStressTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumStressTests.cs#L55) |
| MKS-S-005 | x | Поднятый `Simplex3D` в `5D` и `Cube5D` дают корректную сумму против `ByConvexHull`. | [`MinkowskiSumStressTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumStressTests.cs#L65) |

## Notes

- Это stress-слой, а не минимальный обязательный контракт алгоритма.
- Сценарии intentionally дороже обычного regression-набора и должны запускаться осознанно.
- Отдельно закреплены mixed-dimension-in-5D случаи, когда пространство `5D`, а собственная размерность одного из слагаемых меньше.
