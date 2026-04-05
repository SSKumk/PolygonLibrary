# Basic Cases

## Scope

Сценарии:

- `FindExtrInCPOnVector_Naive`;
- `doSubtract`;
- `Naive` и `Geometric` на нулевом, граничном и пустом `cube - segment`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKD-001 | `x` | `FindExtrInCPOnVector_Naive` возвращает вершину с максимальным скалярным произведением. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L9) |
| MKD-002 | `x` | `doSubtract` корректно сдвигает свободный член гиперплоскости. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L23) |
| MKD-003 | `x` | `Naive` и `Geometric` для вычитания нулевого отрезка возвращают исходный куб. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L34) |
| MKD-004 | `x` | `Naive` и `Geometric` для длинного осевого отрезка возвращают `null`. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L50) |
| MKD-005 | `x` | `Naive` и `Geometric` для единичного осевого отрезка дают непустую 2D-разность и совпадают друг с другом. | [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs#L61) |

## Notes

- Это прямой перенос уже существующего нового слоя `cube - segment` из старого `Current`.
- Базовый контракт `MinkowskiDiff` сейчас прежде всего задаётся helper-ами и agreement `Naive == Geometric` на representative-случаях.
