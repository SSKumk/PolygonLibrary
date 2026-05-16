# HyperPlane

## Scope

Класс [`HyperPlane.cs`](../../../../CGLibrary/Basics/HyperPlane.cs) описывает гиперплоскость и ориентированное полупространство:

- построение по нормали, константе, аффинному базису и набору точек;
- ленивое восстановление `Normal`, `AffBasis` и `ConstantTerm`;
- вычисление положения точек относительно гиперплоскости;
- фильтрацию наборов точек и сравнение гиперплоскостей.

## Topics

- [`ConstructionAndOrientation.md`](ConstructionAndOrientation.md) - конструкторы, ориентация нормали, ленивые инварианты.
- [`EvaluationAndContainment.md`](EvaluationAndContainment.md) - `Eval`, `Contains*`, `FilterIn`, `FilterNotIn`, `AllAtOneSide`.
- [`ComparisonOverridesAndFactory.md`](ComparisonOverridesAndFactory.md) - `CompareTo`, `Equals`, `ToString`, `Make3D_xyParallel`, `CheckCorrectness`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Orientation | `x` | Перенесено в отдельный файл новой структуры. |
| Evaluation and Containment | `x` | Перенесено в отдельный файл новой структуры. |
| Comparison, Overrides and Factory | `x` | Перенесено в отдельный файл новой структуры, включая `GetHashCode`. |

## Existing Test Sources

- ``HyperPlaneTests.cs``

## Notes

- По `HyperPlane` уже есть редкий для репозитория хороший уровень прямого покрытия, так что здесь можно опираться на реальные тесты без натяжек.
