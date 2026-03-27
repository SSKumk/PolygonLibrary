# Vector

## Scope

Класс [`CGLibrary/Basics/Vector.cs`](../../../../CGLibrary/Basics/Vector.cs) описывает общий многомерный вектор и задаёт базовый линейно-алгебраический контракт для остального ядра:

- хранение координат и размерности;
- лексикографическое сравнение;
- арифметику и скалярное произведение;
- нормировку, углы, параллельность и ортогональность;
- фабрики стандартных и случайных векторов.

Для `double` это одна из ключевых опорных сущностей: ошибки здесь будут протекать в `Matrix`, `Basis`, `HyperPlane`, полигоны и многогранники.

## Topics

- [`ConstructionAndIdentity.md`](ConstructionAndIdentity.md) - конструкторы, размерность, доступ к координатам, идентичность объекта.
- [`ComparisonAndFormatting.md`](ComparisonAndFormatting.md) - сравнение, равенство, преобразование в строку и массив.
- [`Arithmetic.md`](Arithmetic.md) - арифметические операции и линейные комбинации.
- [`Geometry.md`](Geometry.md) - длина, нормировка, углы, проекции, взаимное расположение.
- [`FactoriesAndGeneration.md`](FactoriesAndGeneration.md) - стандартные фабрики и генерация случайных векторов.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Identity | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Comparison and Formatting | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Arithmetic | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Geometry | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |
| Factories and Generation | ` ` | Есть существующие тестовые файлы, но сценарии ещё не размечены. |

## Existing Test Sources

Потенциальные источники текущего покрытия:

- [`VectorTests.cs`](../../../../Tests/SharedTests/VectorTests.cs)
- [`VectorsTests.cs`](../../../../Tests/Double-Tests/VectorsTests.cs)

## Notes

- При дальнейшей разметке полезно отделять truly generic-сценарии от сценариев, критичных именно для `double` и `Eps`.
- Для `Vector` важны не только happy-path проверки, но и инварианты после цепочек операций.




