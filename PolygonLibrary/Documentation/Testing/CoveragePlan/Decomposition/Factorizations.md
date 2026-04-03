# Factorizations

## Scope

Сценарии на прямые разложения `QR_ByReflection` и `LQ_ByReflection`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| DEC-FAC-001 | x | `QR_ByReflection` для square-матрицы восстанавливает исходную матрицу, даёт ортонормальный `Q` и upper-triangular `R`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L55) |
| DEC-FAC-002 | x | `QR_ByReflection` для tall-матрицы корректно работает и сохраняет треугольную структуру `R`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L69) |
| DEC-FAC-003 | x | `QR_ByReflection` для rank-deficient матрицы сохраняет реконструкцию и upper-triangular форму. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L83) |
| DEC-FAC-004 | x | `LQ_ByReflection` для wide-матрицы восстанавливает исходную матрицу, даёт ортонормальный `Q` и lower-triangular `L`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L97) |
| DEC-FAC-005 | x | `LQ_ByReflection` для square-матрицы также удовлетворяет инвариантам `A = L * Q`. | [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs#L111) |

## Existing Tests

- Закреплены именно инварианты разложения, а не конкретные знаки столбцов/строк `Q`.
- Для `QR` и `LQ` покрыты square, rectangular и rank-deficient сценарии.

## Gaps

- Сценарии `d < m` для `QR_ByReflection` и симметрично некорректные входы для `LQ_ByReflection` не закрепляются как runtime-контракт: в коде это `Debug.Assert`.
