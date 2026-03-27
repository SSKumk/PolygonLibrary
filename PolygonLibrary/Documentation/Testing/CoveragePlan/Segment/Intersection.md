# Segment Intersection

## Scope

Этот файл покрывает `Segment.Intersect`, а также контракт результирующей структуры `CrossInfo`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SEG-XP-001` | `x` | Пересечение двух непараллельных отрезков в одной внутренней точке возвращает `CrossType.SinglePoint` и корректную точку `fp`. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |
| `SEG-XP-002` | `x` | Пересечение в конце одного или обоих отрезков корректно размечает `IntersectPointPos`. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |
| `SEG-XP-003` | `x` | Непересекающиеся непараллельные отрезки возвращают `CrossType.NoCross`. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |
| `SEG-XP-004` | `x` | Коллинеарные перекрывающиеся отрезки возвращают `CrossType.Overlap` и корректные `fp`/`sp`. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |
| `SEG-XP-005` | `x` | Коллинеарные отрезки, касающиеся в одной точке, возвращают `CrossType.SinglePoint`. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |
| `SEG-XP-006` | `x` | Коллинеарные, но непересекающиеся отрезки возвращают `CrossType.NoCross`. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |
| `SEG-XP-007` | `x` | Результат `Intersect(s1, s2)` согласован с `Intersect(s2, s1)` по типу пересечения и геометрии результата. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |
| `SEG-XP-008` | `x` | Разворот порядка концов одного из отрезков не меняет геометрию пересечения. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |
| `SEG-XP-009` | `x` | `CrossInfo` корректно заполняет ссылки `s1` и `s2` исходными объектами. | [`SegmentIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) |

## Gaps

- Обязательных gap-ов для текущего публичного контракта не осталось.

## Notes

- Активный системный набор тестов теперь заменяет старый закомментированный черновик из legacy-файла.
