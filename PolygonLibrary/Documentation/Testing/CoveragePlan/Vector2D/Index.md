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
| Construction and Comparison | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Arithmetic and Angles | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Relations and Combinations | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |

## Existing Test Sources

Потенциальный источник текущего покрытия:

- [`VectorsTests.cs`](../../../../Tests/Double-Tests/VectorsTests.cs)

## Notes

- `Vector2D` частично дублирует идеи `Vector`, но имеет собственный 2D-контракт: его лучше документировать отдельно, а не наследовать сценарии по умолчанию.




