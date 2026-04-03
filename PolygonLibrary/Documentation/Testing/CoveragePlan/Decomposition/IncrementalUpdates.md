# Incremental Updates

## Scope

Сценарии на `QR_FullUpdate` и `LQ_FullUpdate`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| DEC-UPD-001 | x | `QR_FullUpdate` на независимом векторе увеличивает размер базиса и сохраняет ортонормальность `currentQ`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L125) |
| DEC-UPD-002 | x | После `QR_FullUpdate` добавленный вектор имеет нулевые координаты за пределами нового базиса. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L125) |
| DEC-UPD-003 | x | `QR_FullUpdate` на зависимом или нулевом векторе не меняет размер базиса и не ломает матрицу. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L144) |
| DEC-UPD-004 | x | `LQ_FullUpdate` на независимом векторе увеличивает размер базиса и сохраняет ортонормальность `currentQ`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L162) |
| DEC-UPD-005 | x | После `LQ_FullUpdate` добавленный вектор имеет нулевые координаты за пределами нового базиса. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L162) |
| DEC-UPD-006 | x | `LQ_FullUpdate` на зависимом или нулевом векторе не меняет размер базиса и не ломает матрицу. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L181) |

## Existing Tests

- Update-ветки покрыты на `3D` и проверяются через ортонормальность, размер базиса и координаты в обновлённом базисе.
- Зависимые и нулевые добавления закреплены как no-op на уровне результата.

## Gaps

- Предусловия про размерность `currentQ`, диапазон `currentBasisDimension` и согласованность размерности `v` не закрепляются как release-контракт: в коде это `Debug.Assert`.
