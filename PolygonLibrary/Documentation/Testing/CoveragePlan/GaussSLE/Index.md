# GaussSLE

## Scope

Сценарии для `Geometry<double, DConvertor>.GaussSLE`:

- решение square, overdetermined и underdetermined систем;
- pivot choices `No`, `RowWise`, `ColWise`, `All`;
- instance API: `SetSystem`, `SetGaussChoice`, `GetSolution`;
- factory-методы по функциям, массивам и `HyperPlane`.

## Topics

- [`SystemsAndPivoting.md`](SystemsAndPivoting.md) - базовые типы систем и выбор пивота.
- [`InstanceAndFactories.md`](InstanceAndFactories.md) - instance API, переиспользование solver-а и factory-методы.

## Current Coverage Summary

| Topic | Status | Notes |
| --- | --- | --- |
| Systems and Pivoting | `x` | Square/rectangular сценарии и все четыре pivot choices покрыты прямыми тестами. |
| Instance and Factories | `x` | `SetSystem`, `SetGaussChoice`, оба `GetSolution` и factory-методы покрыты прямыми тестами. |

## Existing Test Sources

- [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs)

## Notes

- Under\-determined и singular/inconsistent случаи трактуются как отсутствие единственного решения.
- Проверяется именно текущий контракт на unique solution, а не поиск произвольного семейства решений.
