# Segment Point Queries

## Scope

Этот файл покрывает запросы о положении точки относительно отрезка.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SEG-QRY-001` | `x` | `IsEndPoint(p)` возвращает `true` для обоих концов отрезка. | [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) |
| `SEG-QRY-002` | `x` | `IsEndPoint(p)` возвращает `false` для внутренней точки и для внешней точки. | [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) |
| `SEG-QRY-003` | `x` | `IsInnerPoint(p)` возвращает `true` для внутренней точки отрезка. | [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) |
| `SEG-QRY-004` | `x` | `IsInnerPoint(p)` возвращает `false` для концов и внешних точек. | [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) |
| `SEG-QRY-005` | `x` | `ContainsPoint(p)` возвращает `true` для обоих концов и внутренних точек отрезка. | [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) |
| `SEG-QRY-006` | `x` | `ContainsPoint(p)` возвращает `false` для коллинеарных точек вне отрезка и для неколлинеарных точек. | [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) |
| `SEG-QRY-007` | `x` | `ComputeAtPoint(x)` корректно вычисляет ординату на не вертикальном отрезке. | [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) |
| `SEG-QRY-008` | `x` | `ComputeAtPoint(x)` согласован с концами отрезка: на `x1` и `x2` возвращает `y1` и `y2`. | [`SegmentPointQueriesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) |
| `SEG-QRY-009` | `-` | `ComputeAtPoint(x)` запрещён для вертикального отрезка согласно текущему debug-контракту. | |

## Gaps

- Обязательных gap-ов для текущего публичного контракта не осталось.

## Notes

- Исторический `SegmentContainsPointTest` сохранён с прежним текстом сообщений.
