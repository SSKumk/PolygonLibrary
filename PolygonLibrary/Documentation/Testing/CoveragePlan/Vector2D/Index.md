# Vector2D

## Scope

Класс [`CGLibrary/Basics/Vector2D.cs`](../../../../CGLibrary/Basics/Vector2D.cs) задаёт двумерный специализированный вектор с 2D-геометрическими операциями:

- координаты `x`, `y` и полярный угол;
- арифметику, скалярное и псевдоскалярное произведения;
- повороты и углы;
- отношения параллельности, ортогональности и принадлежности угловому конусу.

## Topics

- [`ConstructionAndComparison.md`](ConstructionAndComparison.md) - конструкторы, сравнение, доступ к координатам, строковое представление.
- [`ArithmeticAndAngles.md`](ArithmeticAndAngles.md) - арифметика, расстояния, повороты, углы и полярные координаты.
- [`RelationsAndCombinations.md`](RelationsAndCombinations.md) - взаимное расположение, `IsBetween`, линейные комбинации, приведение из `Vector`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Comparison | `x` | Покрытие вынесено в `Tests/DoubleGeometry/Basics/Vector2D`. |
| Arithmetic and Angles | `x` | Покрытие вынесено в `Tests/DoubleGeometry/Basics/Vector2D`. |
| Relations and Combinations | `x` | Покрытие вынесено в `Tests/DoubleGeometry/Basics/Vector2D`. |

## Existing Test Sources

Текущее покрытие:

- [`Vector2DConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs)
- [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs)
- [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs)

Legacy-источник [`VectorsTests.cs`](../../../../Tests/Archive/Double-Tests/VectorsTests.cs) больше не участвует в активной компиляции после переноса.

## Notes

- `Vector2D` частично дублирует идеи `Vector`, но имеет собственный 2D-контракт: его лучше документировать отдельно, а не наследовать сценарии по умолчанию.




