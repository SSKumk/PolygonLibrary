# Basic Cases

## Scope

Сценарии:

- `AlgSumPoints`;
- `BySandipDas` на точках, отрезках и квадрате;
- сравнение с `ByConvexHull`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKS-B-001 | x | `AlgSumPoints` возвращает все попарные суммы двух множеств точек. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L9) |
| MKS-B-002 | x | `BySandipDas` для двух точек совпадает с прямым ожидаемым результатом. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L19) |
| MKS-B-003 | x | `BySandipDas` для двух ортогональных отрезков строит единичный квадрат. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L26) |
| MKS-B-004 | x | `BySandipDas` для сдвинутых отрезков совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L33) |
| MKS-B-005 | x | `BySandipDas` для диагонального отрезка и квадрата совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L40) |

## Existing Tests

- Перенесены характерные unit-like 2D-сценарии из начала legacy-файла.

## Gaps

- Большой перечень 2D/3D article-cases из legacy оставлен за пределами обязательного минимума.

