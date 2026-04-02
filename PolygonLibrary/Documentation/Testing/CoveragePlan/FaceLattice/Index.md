# FaceLattice

## Scope

Класс [`FaceLattice.cs`](../../../../CGLibrary/Polyhedra/ConvexPolyhedra/ConvexPolytops/FaceLattice.cs) описывает face-lattice представление выпуклого политопа.

В рамках текущего полного покрытия здесь рассматриваются:

- `FaceLattice` как контейнер уровней решётки;
- `FLNode` как основная публичная единица лица и подграни;
- базовые операции обхода, сравнения и преобразования вершин.

Пока прямым набором ещё не закрыты:

- internal-конвертеры `ConstructFromFLNodeSum` и `ConstructFromBaseSubCP`;
- отдельный набор на `FLNodeSum`;
- временный рабочий журнал обсуждения и решений: [`Worklog.temp.md`](Worklog.temp.md).

## Topics

- [`NodeStructure.md`](NodeStructure.md) - `FLNode`: конструкторы, иерархия, уровни, сравнение и overrides.
- [`LatticeAndTransform.md`](LatticeAndTransform.md) - `FaceLattice`: конструкторы, агрегаты, `AllKfaces_ExceptTop`, `VertexTransform`, `Equals`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Node Structure | `x` | Legacy-набор на `FLNode` перенесён в новую структуру и дополнен `GetHashCode`. |
| Lattice and Transform | `x` | Добавлен прямой набор на сам `FaceLattice`. |
| Internal Conversion Helpers | `-` | Должны быть покрыты отдельным следующим шагом. |

## Existing Test Sources

- [`FaceLatticeTests.cs`](../../../../Tests/SharedTests/FaceLatticeTests.cs)

## Notes

- Исторический legacy-файл покрывал в основном `FLNode`, а не всю `FaceLattice`. В новой структуре это разделено явно.
