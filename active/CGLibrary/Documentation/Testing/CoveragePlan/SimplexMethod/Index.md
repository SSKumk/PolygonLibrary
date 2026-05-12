# SimplexMethod

## Scope

Сценарии для `Geometry<double, DConvertor>.SimplexMethod`:

- статусы `Ok`, `NoSolution`, `Unlimited`;
- корректность оптимального значения и решения;
- свободные переменные через внутреннее разложение `x = x+ - x-`;
- `BasisInequalitiesID` и `ActiveInequalitiesID` для возвращаемого оптимального решения;
- отсутствие гарантии, что optimum в исходном пространстве является вершиной исходного многогранника.

## Topics

- [`OptimizationStatuses.md`](OptimizationStatuses.md) - базовые статусы и корректность решения/значения.
- [`OriginalVariablesAndActiveSet.md`](OriginalVariablesAndActiveSet.md) - восстановление исходных переменных и активный набор ограничений.

## Current Coverage Summary

| Topic | Status | Notes |
| --- | --- | --- |
| Optimization Statuses | `x` | `Ok`, `NoSolution`, `Unlimited` и scenario с отрицательным optimum point покрыты прямыми тестами. |
| Original Variables And Active Set | `x` | Проверяются восстановление исходных переменных после split, active set для возвращаемого optimum и отсутствие vertex-guarantee в исходном пространстве. |

## Existing Test Sources

- [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs)

## Notes

- Текущий слой не закрепляет поведение на дегенеративных tie-breaking сценариях и не требует конкретной траектории pivot-ов.
- Для задач со свободными исходными переменными не закрепляется требование, что возвращаемый optimum обязан быть вершиной исходного многогранника.
- Пустой список ограничений и другие некорректные входы пока не считаются release-контрактом: код исходит из корректного входа.
