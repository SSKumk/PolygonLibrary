# FaceLattice

## Scope

Класс [`FaceLattice.cs`](../../../../CGLibrary/Polyhedra/ConvexPolyhedra/ConvexPolytops/FaceLattice.cs) описывает face-lattice представление выпуклого политопа.

В рамках текущего полного покрытия здесь рассматриваются:

- `FaceLattice` как контейнер уровней решётки;
- `FLNode` как основная публичная единица лица и подграни;
- базовые операции обхода, сравнения и преобразования вершин;
- internal-конвертеры `ConstructFromFLNodeSum` и `ConstructFromBaseSubCP`.

## Topics

- [`NodeStructure.md`](NodeStructure.md) - `FLNode`: конструкторы, иерархия, уровни, сравнение и overrides.
- [`LatticeAndTransform.md`](LatticeAndTransform.md) - `FaceLattice`: конструкторы, агрегаты, `AllKfaces_ExceptTop`, `VertexTransform`, `Equals`.
- [`InternalConstruction.md`](InternalConstruction.md) - internal-конвертеры `ConstructFromFLNodeSum` и `ConstructFromBaseSubCP`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Node Structure | `x` | Legacy-набор на `FLNode` перенесён в новую структуру и дополнен `GetHashCode`. |
| Lattice and Transform | `x` | Добавлен прямой набор на сам `FaceLattice`. |
| Internal Conversion Helpers | `x` | `ConstructFromFLNodeSum` и `ConstructFromBaseSubCP` покрыты прямыми треугольными сценариями. |

## Existing Test Sources

- [`FaceLatticeTests.cs`](../../../../Tests/SharedTests/FaceLatticeTests.cs)

## Notes

- Исторический legacy-файл покрывал в основном `FLNode`, а не всю `FaceLattice`. В новой структуре это разделено явно.
- Internal-конвертеры вынесены в отдельный direct-layer на малом треугольном примере; higher-dimensional временные структуры остаются следующим шагом.
