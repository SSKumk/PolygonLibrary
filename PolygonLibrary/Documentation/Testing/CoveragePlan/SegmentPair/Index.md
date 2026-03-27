# SegmentPair

## Scope

Класс [`SegmentPair.cs`](../../../../CGLibrary/Segments/SegmentPair.cs) задаёт упорядоченную пару отрезков:

- автоматически сортирует два `Segment` в конструкторе;
- поддерживает `CompareTo` для использования в `SortedSet` и похожих структурах.

## Topics

- [`ConstructionAndComparison.md`](ConstructionAndComparison.md) - конструктор и порядок сравнения.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Comparison | `x` | Есть прямой unit-like набор на нормализацию порядка, `CompareTo` и `SortedSet`. |

## Existing Test Sources

- [`SegmentPairConstructionAndComparisonTests.cs`](../../../../Tests/DoubleGeometry/Basics/SegmentPair/SegmentPairConstructionAndComparisonTests.cs)
- [`BentlyOttmannTests.cs`](../../../../Tests/BentlyOttmannTests.cs) - историческое косвенное использование в `SortedSet<SegmentPair>`, но файл сейчас исключён из сборки тестового проекта.

## Notes

- Несмотря на компактность класса, для sweep-line алгоритмов ошибки в нормализации порядка здесь очень неприятны, так что отдельные маленькие тесты всё равно нужны.
- При миграции выяснилось, что `SegmentPair.cs` лежал в дереве, но не был подключён к `CGLibrary.csproj`; перед тестированием класс пришлось вернуть в активную сборку библиотеки.



