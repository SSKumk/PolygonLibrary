# GiftWrapping

## Scope

Класс [`GiftWrapping.cs`](../../../../CGLibrary/Polyhedra/ConvexPolyhedra/GiftWrapping/GiftWrapping.cs) реализует построение выпуклой оболочки и `FaceLattice` по swarm of points.

В обязательный минимум миграции входят:

- базовые ветки инициализации: пустой, точечный, линейный, симплициальный случай;
- извлечение `Vrep` из swarm с внутренними точками;
- построение `FaceLattice` для 2D и 3D типовых политопов.

Вне обязательного минимума пока остаются:

- большой legacy stress-набор на случайные и тяжёлые входы;
- многомерные pathological cases из старого `GW_Tests`;
- производительные и отладочные сценарии из `SpeedTests`.

## Topics

- [`InitializationAndDegenerateCases.md`](InitializationAndDegenerateCases.md) - пустой, точечный и линейный случаи.
- [`HullExtraction.md`](HullExtraction.md) - `WrapVRep` для типовых наборов точек.
- [`FaceLatticeConstruction.md`](FaceLatticeConstruction.md) - `ConstructFL` и `WrapFaceLattice`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Initialization and Degenerate Cases | `x` | Закрыты пустой, точечный и линейный сценарии. |
| Hull Extraction | `x` | Зафиксированы прямые 2D/3D кейсы и ветка симплекса. |
| Face Lattice Construction | `x` | Закрыты типовые 2D и 3D результаты по числу граней и вершинам. |
| Randomized Stress Cases | `-` | Исключены из обязательного минимума и оставлены в истории. |

## Existing Test Sources

- [`GW_Tests.cs`](../../../../Tests/Double-Tests/GW_hDTests/GW_Tests.cs)

## Notes

- Старый `GW_Tests` смешивал прямые проверки обёртки с большими randomized/regression сценариями через `ConvexPolytop.CreateFromPoints(..., true)`. В новой структуре оставлен компактный прямой слой на сам `GiftWrapping`.

