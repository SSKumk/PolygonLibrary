# AffineBasis Comparison And Enumeration

## Scope

Этот файл покрывает `Equals`, `CompareTo`, `GetHashCode` и перечисление векторов линейной части.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `AFB-CMP-001` | `x` | `Equals` считает равными аффинные базисы, задающие одно и то же аффинное пространство. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |
| `AFB-CMP-002` | `x` | `Equals` различает разные линейные части, разные сдвиги и разные размерности пространства. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |
| `AFB-CMP-003` | `x` | `Equals` корректно обрабатывает `null`, другой тип и сравнение объекта с самим собой. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |
| `AFB-CMP-004` | `x` | `CompareTo` считает `null` меньшим, чем любой аффинный базис. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |
| `AFB-CMP-005` | `x` | `CompareTo` сравнивает аффинные пространства по размерности линейной части. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |
| `AFB-CMP-006` | `x` | `CompareTo` различает пространства с разными линейными частями. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |
| `AFB-CMP-007` | `x` | `CompareTo` для параллельных пространств использует `CanonicalOrigin`. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |
| `AFB-CMP-008` | `x` | `GetHashCode()` для `AffineBasis` остаётся запрещённой операцией и выбрасывает `InvalidOperationException`. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |
| `AFB-CMP-009` | `x` | Перечисление `AffineBasis` проходит по векторам линейной части в их порядке. | [`AffineBasisComparisonAndEnumerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) |

## Gaps

- Нет.

## Notes

- Для `CompareTo` важно различать случаи “одна и та же направляющая часть, но разный сдвиг”.
