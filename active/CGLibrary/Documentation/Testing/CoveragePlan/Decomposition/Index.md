# Decomposition

## Scope

Сценарии для `Geometry<double, DConvertor>.Decomposition`:

- `QR_ByHouseholder`;
- `LQ_ByHouseholder`;
- `QR_IncrementalUpdate`;
- `LQ_IncrementalUpdate`.

## Topics

- [`Factorizations.md`](Factorizations.md) - прямые QR/LQ-разложения, ортонормальность и треугольная структура.
- [`IncrementalUpdates.md`](IncrementalUpdates.md) - инкрементальные QR/LQ-обновления полного ортонормированного базиса.

## Current Coverage Summary

| Topic | Status | Notes |
| --- | --- | --- |
| Factorizations | `x` | `QR_ByHouseholder` и `LQ_ByHouseholder` покрыты square/tall/wide и rank-deficient сценариями с проверкой реконструкции и структуры. |
| Incremental Updates | `x` | `QR_IncrementalUpdate` и `LQ_IncrementalUpdate` покрыты независимыми, зависимыми, нулевыми и двухшаговыми сценариями. |

## Existing Test Sources

- [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs)

## Notes

- Для `QR` и `LQ` не фиксируется конкретный знак столбцов/строк `Q`, а только инварианты разложения.
- Update-сценарии проверяются через сохранение ортонормальности и обнуление координат за пределами текущего базиса, а не через жёсткую конкретную матрицу.

