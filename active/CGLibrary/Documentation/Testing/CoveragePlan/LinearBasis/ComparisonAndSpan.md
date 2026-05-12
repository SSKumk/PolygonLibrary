# LinearBasis Comparison And Span

## Scope

Этот файл покрывает `Equals`, `CompareTo`, перечисление векторов и `SpanSameSpace`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `LB-CMP-001` | `x` | `Equals` считает равными базисы, задающие одно и то же подпространство, даже при другом наборе ортонормированных векторов. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-002` | `x` | `Equals` различает разные подпространства и разные размерности пространства. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-003` | `x` | `Equals` корректно обрабатывает `null`, другой тип и сравнение объекта с самим собой. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-004` | `x` | `CompareTo` сначала сравнивает размерность пространства. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-005` | `x` | `CompareTo` затем сравнивает размерность подпространства. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-006` | `x` | `CompareTo` для равных размерностей использует каноническое сравнение через RREF. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-007` | `x` | Перечисление выдаёт базисные векторы в порядке индексации и корректно работает для пустого базиса. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-008` | `x` | `SpanSameSpace` распознаёт одинаковые подпространства для копий, перестановок и поворотов базиса. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-009` | `x` | `SpanSameSpace` различает разные размерности подпространства и разные размерности пространства. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |
| `LB-CMP-010` | `x` | `SpanSameSpace` корректно обрабатывает пустые базисы. | [`LinearBasisComparisonAndSpanTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) |

## Gaps

- Нет.

## Notes

- Смысл этих тестов не в совпадении конкретных ортонормированных векторов, а в совпадении порождённого подпространства.
