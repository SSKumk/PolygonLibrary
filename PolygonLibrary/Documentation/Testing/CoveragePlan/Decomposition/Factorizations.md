# Factorizations

## Scope

Сценарии на прямые разложения `QR_ByHouseholder` и `LQ_ByHouseholder`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| DEC-FAC-001 | x | `QR_ByHouseholder` для square-матрицы восстанавливает исходную матрицу, даёт ортонормальный `Q` и upper-triangular `R`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L55) |
| DEC-FAC-002 | x | `QR_ByHouseholder` для tall-матрицы корректно работает и сохраняет треугольную структуру `R`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L69) |
| DEC-FAC-003 | x | `QR_ByHouseholder` для rank-deficient матрицы сохраняет реконструкцию и upper-triangular форму. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L83) |
| DEC-FAC-004 | x | `LQ_ByHouseholder` для wide-матрицы восстанавливает исходную матрицу, даёт ортонормальный `Q` и lower-triangular `L`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L97) |
| DEC-FAC-005 | x | `LQ_ByHouseholder` для square-матрицы также удовлетворяет инвариантам `A = L * Q`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L111) |
| DEC-FAC-006 | x | `LQ_ByHouseholder` для rank-deficient wide-матрицы сохраняет реконструкцию и lower-triangular форму. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L125) |
| DEC-FAC-007 | x | `LQ_ByHouseholder` для tall-матрицы также удовлетворяет инвариантам `A = L * Q`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L137) |

## Existing Tests

- Закреплены именно инварианты разложения, а не конкретные знаки столбцов/строк `Q`.
- Для `QR` и `LQ` покрыты square, tall/wide и rank-deficient сценарии.

## Gaps

- Сценарии `d < m` для `QR_ByHouseholder` и симметрично некорректные входы для `LQ_ByHouseholder` не закрепляются как runtime-контракт: в коде это `Debug.Assert`.

