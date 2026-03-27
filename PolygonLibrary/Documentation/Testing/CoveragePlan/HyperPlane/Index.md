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
| Construction and Orientation | `x` | Есть подробный отдельный test fixture. |
| Evaluation and Containment | `x` | Есть прямые тесты на вычисление, принадлежность и фильтрацию. |
| Comparison, Overrides and Factory | `x` | Есть прямые тесты на фабрику, строки, равенство и сравнение. |

## Existing Test Sources

- [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs)
- [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs)
- [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs)

## Notes

- По `HyperPlane` уже есть редкий для репозитория хороший уровень прямого покрытия, так что здесь можно опираться на реальные тесты без натяжек.



