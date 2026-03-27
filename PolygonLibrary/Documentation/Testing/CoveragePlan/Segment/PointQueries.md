# Segment Point Queries

## Scope

Этот файл покрывает запросы о положении точки относительно отрезка.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SEG-QRY-001` | ` ` | `IsEndPoint(p)` возвращает `true` для обоих концов отрезка. | |
| `SEG-QRY-002` | ` ` | `IsEndPoint(p)` возвращает `false` для внутренней точки и для внешней точки. | |
| `SEG-QRY-003` | ` ` | `IsInnerPoint(p)` возвращает `true` для внутренней точки отрезка. | |
| `SEG-QRY-004` | ` ` | `IsInnerPoint(p)` возвращает `false` для концов и внешних точек. | |
| `SEG-QRY-005` | `x` | `ContainsPoint(p)` возвращает `true` для обоих концов и внутренних точек отрезка. | [`SegmentCrossTests.cs`](../../../../Tests/Double-Tests/SegmentCrossTests.cs) |
| `SEG-QRY-006` | `x` | `ContainsPoint(p)` возвращает `false` для коллинеарных точек вне отрезка и для неколлинеарных точек. | [`SegmentCrossTests.cs`](../../../../Tests/Double-Tests/SegmentCrossTests.cs) |
| `SEG-QRY-007` | ` ` | `ComputeAtPoint(x)` корректно вычисляет ординату на не вертикальном отрезке. | |
| `SEG-QRY-008` | ` ` | `ComputeAtPoint(x)` согласован с концами отрезка: на `x1` и `x2` возвращает `y1` и `y2`. | |
| `SEG-QRY-009` | ` ` | `ComputeAtPoint(x)` запрещён для вертикального отрезка согласно текущему debug-контракту. | |

## Gaps

- Нет прямых тестов на `IsEndPoint`, `IsInnerPoint` и `ComputeAtPoint`.

## Notes

- В текущем тесте `ContainsPoint` уже полезно покрыты точки на границе, внутри, почти на линии и явно вне отрезка.



