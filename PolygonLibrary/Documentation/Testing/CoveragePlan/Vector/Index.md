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
| Construction and Identity | `x` | Перенесён прямой набор на конструкторы, доступ к координатам и copy/no-copy семантику. |
| Comparison and Formatting | `x` | Перенесён прямой набор на сравнение, `Equals`, форматирование и инвариантную культуру. |
| Arithmetic | `x` | Перенесён прямой набор на операторы, `Sum`, линейные комбинации и affine operations. |
| Geometry | `x` | Перенесён прямой набор на длину, нормировку, углы, проекции и взаимное расположение. |
| Factories and Generation | `x` | Перенесён прямой набор на стандартные фабрики и генераторы с фиксированным seed. |

## Existing Test Sources

Потенциальные источники текущего покрытия:

- [`VectorTests.cs`](../../../../Tests/SharedTests/VectorTests.cs)
- [`VectorAssert.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorAssert.cs)
- [`VectorConstructionAndIdentityTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs)
- [`VectorComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs)
- [`VectorArithmeticTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs)
- [`VectorGeometryTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs)
- [`VectorFactoriesAndGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs)

## Notes

- При дальнейшей разметке полезно отделять truly generic-сценарии от сценариев, критичных именно для `double` и `Eps`.
- Для `Vector` важны не только happy-path проверки, но и инварианты после цепочек операций.




