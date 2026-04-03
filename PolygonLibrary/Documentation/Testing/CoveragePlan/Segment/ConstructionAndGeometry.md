# Segment Construction And Geometry

## Scope

Этот файл покрывает создание `Segment`, доступ к концам и производные геометрические свойства.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SEG-CTOR-001` | `x` | Конструктор по четырём координатам создаёт отрезок с заданными концами. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-002` | `x` | Конструктор по двум `Vector2D` создаёт отрезок с заданными концами. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-003` | `x` | Копирующий конструктор создаёт эквивалентный отрезок. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-004` | `-` | Конструктор запрещает совпадающие концы согласно текущему debug-контракту. | |
| `SEG-CTOR-005` | `x` | Индексатор `[0]` возвращает первый конец, `[1]` - второй. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-006` | `x` | `Directional` равен `p2 - p1`. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-007` | `x` | `Normal` ортогонален `Directional`. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-008` | `x` | `DirectionalNormalized` имеет единичную длину и то же направление, что `Directional`. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-009` | `x` | `Length` равен длине вектора `p2 - p1`. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-010` | `x` | `PolarAngle` совпадает с полярным углом направляющего вектора. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-011` | `x` | `IsVertical` возвращает `true` только для вертикальных отрезков. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-012` | `x` | `CompareTo` сравнивает отрезки лексикографически по первому, затем по второму концу. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-013` | `x` | `Equals(object)` считает равными только отрезки с тем же порядком концов. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |
| `SEG-CTOR-014` | `x` | `ToString()` возвращает строку формата `[p1;p2]`. | [`SegmentConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) |

## Gaps

- Обязательных gap-ов для текущего публичного контракта не осталось.

## Notes

- Сценарий с совпадающими концами оставлен вне активного слоя как debug-precondition, а не как release-контракт.

