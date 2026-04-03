# FourierMotzkin

## Scope

Сценарии для наивной реализации `Geometry<double, DConvertor>.FourierMotzkin`:

- разбиение неравенств на upper/lower/neutral;
- 1-based индекс устраняемой переменной;
- перенос neutral-неравенств;
- отбрасывание нулевого нового неравенства.

## Topics

- [`NaiveElimination.md`](NaiveElimination.md) - текущий контракт `EliminateVariableNaive`.

## Current Coverage Summary

| Topic | Status | Notes |
| --- | --- | --- |
| Naive Elimination | `x` | Покрыты базовое комбинирование, перенос neutral, 1-based индекс и отбрасывание нулевого результата. |

## Existing Test Sources

- [`FourierMotzkinTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/FourierMotzkin/FourierMotzkinTests.cs)

## Notes

- Этот слой тестирует именно текущую наивную реализацию без попытки закреплять более сильный алгоритмический контракт.
- Избыточность и качество результата не считаются дефектом в рамках этого класса; фиксируется только корректность текущей формулы комбинирования.
