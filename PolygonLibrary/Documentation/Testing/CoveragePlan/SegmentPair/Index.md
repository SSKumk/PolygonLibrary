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
| Construction and Comparison | ` ` | Прямого активного покрытия в собираемом тестовом проекте нет. |

## Existing Test Sources

- [`BentlyOttmannTests.cs`](../../../../Tests/BentlyOttmannTests.cs) - использование в `SortedSet<SegmentPair>`, но файл сейчас исключён из сборки тестового проекта.

## Notes

- Несмотря на компактность класса, для sweep-line алгоритмов ошибки в нормализации порядка здесь очень неприятны, так что отдельные маленькие тесты всё равно нужны.



