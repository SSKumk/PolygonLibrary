# Incremental Updates

## Scope

Сценарии на `QR_IncrementalUpdate` и `LQ_IncrementalUpdate`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| DEC-UPD-001 | x | `QR_IncrementalUpdate` на независимом векторе увеличивает размер базиса и сохраняет ортонормальность `currentQ`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L125) |
| DEC-UPD-002 | x | После `QR_IncrementalUpdate` добавленный вектор имеет нулевые координаты за пределами нового базиса. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L125) |
| DEC-UPD-003 | x | `QR_IncrementalUpdate` на зависимом или нулевом векторе не меняет размер базиса и не ломает матрицу. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L144) |
| DEC-UPD-004 | x | `LQ_IncrementalUpdate` на независимом векторе увеличивает размер базиса и сохраняет ортонормальность `currentQ`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L162) |
| DEC-UPD-005 | x | После `LQ_IncrementalUpdate` добавленный вектор имеет нулевые координаты за пределами нового базиса. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L162) |
| DEC-UPD-006 | x | `LQ_IncrementalUpdate` на зависимом или нулевом векторе не меняет размер базиса и не ломает матрицу. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L181) |
| DEC-UPD-007 | x | Второй независимый вызов `QR_IncrementalUpdate` увеличивает активный prefix до `2` и оставляет у первого вектора нулевой хвост после первой активной координаты. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L168) |
| DEC-UPD-008 | x | Второй независимый вызов `LQ_IncrementalUpdate` увеличивает активный prefix до `2` и оставляет у первого вектора нулевой хвост после первой активной координаты. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L213) |

## Existing Tests

- Update-ветки покрыты на `3D` и проверяются через ортонормальность, размер базиса и координаты в обновлённом базисе.
- Зависимые и нулевые добавления закреплены как no-op на уровне результата.
- Отдельно закреплён сценарий второго независимого обновления при ненулевом prefix.

## Gaps

- Предусловия про размерность `currentQ`, диапазон `currentBasisDimension` и согласованность размерности `v` не закрепляются как release-контракт: в коде это `Debug.Assert`.

