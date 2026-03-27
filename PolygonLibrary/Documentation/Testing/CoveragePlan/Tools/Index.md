# Tools

## Scope

Класс [`CGLibrary/Toolkit/Tools.cs`](F:\Works\IMM\Аспирантура\_PolygonLibrary\PolygonLibrary\CGLibrary\Toolkit\Tools.cs) задаёт базовые числовые соглашения библиотеки:

- глобальную точность `Eps` и производную точность `EpsG`;
- приближённые сравнения чисел;
- общие числовые константы;
- маленькие служебные процедуры, от которых зависят остальные геометрические классы.

Для плана покрытия по `double` это самый нижний слой: ошибки здесь будут размазываться по почти всем остальным тестам.

## Topics

- [`Comparisons.md`](F:\Works\IMM\Аспирантура\_PolygonLibrary\PolygonLibrary\Documentation\Testing\CoveragePlan\Tools\Comparisons.md) - точность, сравнения и компаратор.
- [`MathAndUtilities.md`](F:\Works\IMM\Аспирантура\_PolygonLibrary\PolygonLibrary\Documentation\Testing\CoveragePlan\Tools\MathAndUtilities.md) - константы, инициализация, `Atan2`, `Abs`, `Swap`, комбинации.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Comparisons | ` ` | Сценарии ещё не размечены по фактическому покрытию. |
| Math and Utilities | ` ` | Сценарии ещё не размечены по фактическому покрытию. |

## Existing Test Sources

Выделенного `ToolsTests.cs` в проекте нет. Часть поведения косвенно проверяется почти всеми базовыми тестами, но явная привязка сценариев к тестам ещё не выполнена.

## Notes

- Для `Tools` особенно важно отделять прямое покрытие контракта от косвенного использования в других тестах.
- При последующей разметке стоит отдавать приоритет сценариям, которые напрямую фиксируют граничное поведение около `Eps`.
