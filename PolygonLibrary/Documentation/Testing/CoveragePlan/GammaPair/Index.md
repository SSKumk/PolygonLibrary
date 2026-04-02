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
| Construction | `x` | Добавлен прямой набор на конструкторы, нормализацию и `Equals`. |
| Comparison and Formatting | `x` | Добавлен прямой набор на `CompareTo`, `CompareTo(null)` и `ToString`. |
| CrossPairs | `x` | Основные непараллельные сценарии перенесены напрямую; параллельный случай вынесен из активного слоя. |

## Existing Test Sources

Надёжно релевантные текущие тесты:

- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) - создание `SupportFunction` из массива пар и сравнение результатов после сортировки/дедупликации.
- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) - ещё один сценарий сортировки и сравнения пар.
- [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) - прямые проверки `CrossPairs`.
- [`GammaPairConstructionAndEqualityTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairConstructionAndEqualityTests.cs)
- [`GammaPairComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairComparisonAndFormattingTests.cs)
- [`GammaPairCrossPairsTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairCrossPairsTests.cs)

## Notes

- Для `GammaPair` теперь есть собственный прямой набор, поэтому косвенные сценарии `SupportFunction` остаются только как дополнительный фон.
- Параллельный случай `CrossPairs` не включён в активный слой, потому что реализация всё ещё опирается на `Debug.Assert`, а не на стабильный runtime-`throw`.




