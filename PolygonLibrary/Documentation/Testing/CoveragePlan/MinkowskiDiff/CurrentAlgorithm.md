# Current Algorithm

## Scope

Сценарии:

- `FindExtrInCPOnVector_Naive`;
- `doSubtract`;
- `Naive` и `Geometric` на нулевом, пограничном и пустом сегменте.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| MKD-001 | x | `FindExtrInCPOnVector_Naive` возвращает вершину с максимальным скалярным произведением. | [`MinkowskiDiffCurrentTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffCurrentTests.cs#L9) |
| MKD-002 | x | `doSubtract` корректно сдвигает свободный член гиперплоскости. | [`MinkowskiDiffCurrentTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffCurrentTests.cs#L23) |
| MKD-003 | x | `Naive` и `Geometric` для вычитания нулевого отрезка возвращают исходный куб. | [`MinkowskiDiffCurrentTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffCurrentTests.cs#L34) |
| MKD-004 | x | `Naive` и `Geometric` для длинного осевого отрезка возвращают `null`. | [`MinkowskiDiffCurrentTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffCurrentTests.cs#L50) |
| MKD-005 | x | `Naive` и `Geometric` для единичного осевого отрезка дают непустую 2D-разность и совпадают друг с другом. | [`MinkowskiDiffCurrentTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffCurrentTests.cs#L61) |

## Existing Tests

- Сохранён смысл старого `Cube_Seg0_0_z`, но сценарий разложен на прямые проверяемые случаи.
- Дополнительно добавлена синхронная проверка `Geometric`, которой в legacy не было.

## Gaps

- Sphere/cyclic и другие тяжёлые кейсы из legacy пока не входят в активный слой.


