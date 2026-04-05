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
| MKS-B-004 | x | `BySandipDas` для точки и диагонального отрезка совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L33) |
| MKS-B-005 | x | `BySandipDas` для диагонального отрезка и самого себя совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L40) |
| MKS-B-006 | x | `BySandipDas` для коллинеарных сдвинутых диагональных отрезков совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L47) |
| MKS-B-007 | x | `BySandipDas` для сдвинутых ортогональных отрезков совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L54) |
| MKS-B-008 | x | `BySandipDas` для диагонального отрезка и квадрата совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L61) |
| MKS-B-009 | x | `BySandipDas` для двух неортогональных отрезков совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L68) |
| MKS-B-010 | x | `BySandipDas` для осевого отрезка и квадрата совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L75) |
| MKS-B-011 | x | `BySandipDas` для квадрата и самого себя совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L82) |
| MKS-B-012 | x | `BySandipDas` для повёрнутого квадрата и сдвинутого квадрата совпадает с `ByConvexHull`. | [`MinkowskiSumBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs#L89) |

## Existing Tests

- Перенесены характерные unit-like 2D-сценарии из начала legacy-файла.

## Gaps

- 2D-слой по точкам, отрезкам и квадратам доведён до representative-покрытия legacy-набора.
- Дальше вне `Basic` остаются уже в основном 3D/4D/article-based и stress-сценарии.


