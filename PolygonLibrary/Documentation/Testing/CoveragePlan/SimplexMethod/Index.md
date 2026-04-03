# SimplexMethod

## Scope

Сценарии для `Geometry<double, DConvertor>.SimplexMethod`:

- статусы `Ok`, `NoSolution`, `Unlimited`;
- корректность оптимального значения и решения;
- свободные переменные через внутреннее разложение `x = x+ - x-`;
- `BasisInequalitiesID` и `ActiveInequalitiesID` для оптимального решения.

## Topics

- [`OptimizationStatuses.md`](OptimizationStatuses.md) - базовые статусы и корректность решения/значения.
- [`OriginalVariablesAndActiveSet.md`](OriginalVariablesAndActiveSet.md) - восстановление исходных переменных и активный набор ограничений.

## Current Coverage Summary

| Topic | Status | Notes |
| --- | --- | --- |
| Optimization Statuses | `x` | `Ok`, `NoSolution`, `Unlimited` и scenario с отрицательным optimum point покрыты прямыми тестами. |
| Original Variables And Active Set | `x` | Проверяются восстановление исходных переменных после split, полный active set и basis-vs-active distinction на вырожденной вершине. |

## Existing Test Sources

- [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs)

## Notes

- Текущий слой не закрепляет поведение на дегенеративных tie-breaking сценариях и не требует конкретной траектории pivot-ов.
- Пустой список ограничений и другие некорректные входы пока не считаются release-контрактом: код исходит из корректного входа.
