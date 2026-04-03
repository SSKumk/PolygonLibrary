# Decomposition

## Scope

Сценарии для `Geometry<double, DConvertor>.Decomposition`:

- `QR_ByReflection`;
- `LQ_ByReflection`;
- `QR_FullUpdate`;
- `LQ_FullUpdate`.

## Topics

- [`Factorizations.md`](Factorizations.md) - прямые QR/LQ-разложения, ортонормальность и треугольная структура.
- [`IncrementalUpdates.md`](IncrementalUpdates.md) - инкрементальные QR/LQ-обновления полного ортонормированного базиса.

## Current Coverage Summary

| Topic | Status | Notes |
| --- | --- | --- |
| Factorizations | `x` | `QR_ByReflection` и `LQ_ByReflection` покрыты square/tall/wide сценариями с проверкой реконструкции и структуры. |
| Incremental Updates | `x` | `QR_FullUpdate` и `LQ_FullUpdate` покрыты независимыми, зависимыми и нулевыми добавлениями. |

## Existing Test Sources

- [`DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs)

## Notes

- Для `QR` и `LQ` не фиксируется конкретный знак столбцов/строк `Q`, а только инварианты разложения.
- Update-сценарии проверяются через сохранение ортонормальности и обнуление координат за пределами текущего базиса, а не через жёсткую конкретную матрицу.
