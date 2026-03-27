# FaceLattice

## Scope

Класс [`FaceLattice.cs`](../../../../CGLibrary/Polyhedra/ConvexPolyhedra/ConvexPolytops/FaceLattice.cs) описывает face-lattice представление выпуклого политопа.

В рамках обязательного минимума здесь покрываются:

- `FaceLattice` как контейнер уровней решётки;
- `FLNode` как основная публичная единица лица и подграни;
- базовые операции обхода, сравнения и преобразования вершин.

Вне обязательного минимума пока остаются:

- internal-конвертеры `ConstructFromFLNodeSum` и `ConstructFromBaseSubCP`;
- отдельный прямой набор на `FLNodeSum`, поскольку в текущем backlog он не выделен как самостоятельный класс.

## Topics

- [`NodeStructure.md`](NodeStructure.md) - `FLNode`: конструкторы, иерархия, уровни, сравнение и overrides.
- [`LatticeAndTransform.md`](LatticeAndTransform.md) - `FaceLattice`: конструкторы, агрегаты, `AllKfaces_ExceptTop`, `VertexTransform`, `Equals`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Node Structure | `x` | Legacy-набор на `FLNode` перенесён в новую структуру и дополнен `GetHashCode`. |
| Lattice and Transform | `x` | Добавлен прямой набор на сам `FaceLattice`. |
| Internal Conversion Helpers | `-` | Не входит в обязательный минимум текущей миграции. |

## Existing Test Sources

- [`FaceLatticeTests.cs`](../../../../Tests/SharedTests/FaceLatticeTests.cs)

## Notes

- Исторический legacy-файл покрывал в основном `FLNode`, а не всю `FaceLattice`. В новой структуре это разделено явно.

