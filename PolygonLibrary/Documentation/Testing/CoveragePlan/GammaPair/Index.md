# GammaPair

## Scope

Класс [`GammaPair.cs`](../../../../CGLibrary/Polygons/ConvexPolygons/GammaPair.cs) хранит пару `(normal; value)` для кусочно-линейной опорной функции и задаёт:

- хранение нормали и значения;
- конструкторы с опциональной нормализацией;
- сравнение и равенство по полярному углу нормали и нормированному значению;
- строковое представление;
- вычисление точки пересечения двух прямых, заданных парами.

## Topics

- [`Construction.md`](Construction.md) - конструкторы, инварианты хранения и нормализация.
- [`ComparisonAndFormatting.md`](ComparisonAndFormatting.md) - `Equals`, `CompareTo` и `ToString`.
- [`CrossPairs.md`](CrossPairs.md) - `CrossPairs` и связанные геометрические случаи.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction | ` ` | Явная разметка покрытия по конструкторам ещё не выполнена. |
| Comparison and Formatting | `~` | Есть косвенное покрытие через `SupportFunction`, но не все сценарии закреплены отдельно. |
| CrossPairs | `~` | Есть прямой тест на основные пересечения, но ветка с параллельностью зависит от режима выполнения assert. |

## Existing Test Sources

Надёжно релевантные текущие тесты:

- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) - создание `SupportFunction` из массива пар и сравнение результатов после сортировки/дедупликации.
- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) - ещё один сценарий сортировки и сравнения пар.
- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) - прямые проверки `CrossPairs`.

## Notes

- Для `GammaPair` нельзя автоматически считать любой тест `SupportFunction` прямым покрытием конструктора или `Equals`: часть сценариев там проходит опосредованно.
- Статусы здесь намеренно выставлены консервативно.



