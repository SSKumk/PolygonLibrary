# Tools

## Scope

Класс [`CGLibrary/Toolkit/Tools.cs`](../../../../CGLibrary/Toolkit/Tools.cs) задаёт базовые числовые соглашения библиотеки:

- глобальную точность `Eps` и производную точность `EpsG`;
- приближённые сравнения чисел;
- общие числовые константы;
- маленькие служебные процедуры, от которых зависят остальные геометрические классы.

Для плана покрытия по `double` это самый нижний слой: ошибки здесь будут размазываться по почти всем остальным тестам.

## Topics

- [`Comparisons.md`](Comparisons.md) - точность, сравнения и компаратор.
- [`MathAndUtilities.md`](MathAndUtilities.md) - константы, инициализация, `Atan2`, `Abs`, `Swap`, комбинации.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Comparisons | `x` | Есть отдельный прямой набор на `Eps`, сравнения, `CMP` и `TNumComparer`. |
| Math and Utilities | `x` | Есть отдельный прямой набор на константы, массивы, `Swap`, `Atan2`, `Abs` и `GetCombinations`. |

## Existing Test Sources

- [`ToolsComparisonsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs)
- [`ToolsMathAndUtilitiesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs)

## Notes

- Для `Tools` особенно важно отделять прямое покрытие контракта от косвенного использования в других тестах.
- При последующей разметке стоит отдавать приоритет сценариям, которые напрямую фиксируют граничное поведение около `Eps`.




