# SegmentPair Construction And Comparison

## Scope

Этот файл покрывает единственный конструктор `SegmentPair` и его `CompareTo`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SGP-CMP-001` | `x` | Конструктор размещает меньший по `Segment.CompareTo` отрезок в `s1`, а больший - в `s2`. | [`SegmentPairConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/SegmentPair/SegmentPairConstructionAndComparisonTests.cs) |
| `SGP-CMP-002` | `x` | Конструктор даёт одинаковый результат для `(a, b)` и `(b, a)`. | [`SegmentPairConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/SegmentPair/SegmentPairConstructionAndComparisonTests.cs) |
| `SGP-CMP-003` | `x` | `CompareTo` сначала сравнивает `s1`, а затем `s2`. | [`SegmentPairConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/SegmentPair/SegmentPairConstructionAndComparisonTests.cs) |
| `SGP-CMP-004` | `x` | Две пары с теми же отрезками в разном порядке сравниваются как равные. | [`SegmentPairConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/SegmentPair/SegmentPairConstructionAndComparisonTests.cs) |
| `SGP-CMP-005` | `x` | Сортировка `SortedSet<SegmentPair>` не создаёт дублей для одинаковых пар. | [`SegmentPairConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/SegmentPair/SegmentPairConstructionAndComparisonTests.cs) |

## Gaps

- Нет.

## Notes

- Это хороший кандидат на короткий unit test file из 3-5 сценариев без тяжёлой геометрии.
