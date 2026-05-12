# Basic Cases

## Scope

Сценарии:

- `FindExtrInCPOnVector_Naive`;
- `doSubtract`;
- точные `2D` случаи на квадрате;
- точные `3D` случаи на кубе;
- граничные случаи `cube - segment`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKD-001 | `x` | `FindExtrInCPOnVector_Naive` возвращает вершину с максимальным скалярным произведением. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L9) |
| MKD-002 | `x` | `doSubtract` корректно сдвигает свободный член гиперплоскости. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L23) |
| MKD-003 | `x` | `Naive` и `Geometric` для вычитания нулевого отрезка возвращают исходный куб. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L34) |
| MKD-004 | `x` | `Naive` и `Geometric` для длинного осевого отрезка возвращают `null`. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L50) |
| MKD-005 | `x` | `Naive` и `Geometric` для единичного осевого отрезка дают непустую 2D-разность и совпадают друг с другом. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L61) |
| MKD-006 | `x` | `Naive` и `Geometric` для `square - point` дают точный сдвиг квадрата на `-point`. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L77) |
| MKD-007 | `x` | `Naive` и `Geometric` для `square - short horizontal segment` дают ожидаемый прямоугольник. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L95) |
| MKD-008 | `x` | Для `square - unit horizontal segment` текущий контракт возвращает `null`, так как непустой результат уже одномерен. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L117) |
| MKD-009 | `x` | `Naive` и `Geometric` для `cube - point` дают точный сдвиг куба на `-point`. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L128) |
| MKD-010 | `x` | `Naive` и `Geometric` для `cube - short axis segment` дают ожидаемый осевой бокс. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L146) |

## Notes

- Базовый слой теперь покрывает как `2D`, так и `3D` на точных analytically-checkable примерах.
- В `2D` отдельно зафиксирован текущий контракт: если разность непуста, но одномерна, `MinkowskiDiff` возвращает `null`.
