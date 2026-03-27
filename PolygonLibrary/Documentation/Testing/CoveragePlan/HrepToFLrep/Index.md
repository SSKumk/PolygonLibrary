# HrepToFLrep

## Scope

Класс [`HrepToFLrep.cs`](../../../../CGLibrary/Polyhedra/ConvexPolyhedra/HrepToFLrep.cs) содержит попытку геометрической конверсии из `Hrep` в `FaceLattice`.

На текущем этапе у класса есть один публичный метод, и сам код прямо помечен как `!Не работает!`.

Поэтому в обязательный минимум миграции здесь входит только фиксация текущего наблюдаемого контракта:

- `null`, если стартовую вершину найти не удаётся;
- `NotImplementedException` для ограниченных случаев, где конвертация доходит до незавершённого участка.

## Topics

- [`CurrentContract.md`](CurrentContract.md) - текущее публичное поведение `HrepToFLrep_Geometric`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Current Contract | `x` | Зафиксировано текущее поведение метода для ограниченного и неограниченного случая. |

## Existing Test Sources

- прямого legacy-набора не было

## Notes

- Здесь сознательно не документируется "как должно быть", пока реализация сама обрывается `NotImplementedException`.

