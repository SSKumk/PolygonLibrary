# MinkowskiDiff

## Scope

Класс [`MinkowskiDiff.cs`](../../../../CGLibrary/Algorithms/Polyhedra/MinkowskiDiff.cs) реализует разность Минковского через H-representation и поиск экстремальных вершин.

В активный слой текущего этапа входят:

- вспомогательные прямые функции `FindExtrInCPOnVector_Naive` и `doSubtract`;
- `Naive` и `Geometric` на репрезентативных случаях `cube - segment`.

## Topics

- [`CurrentAlgorithm.md`](CurrentAlgorithm.md) - прямые helper-ы и текущие наблюдаемые результаты `Naive`/`Geometric`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Current Algorithm | `x` | Зафиксированы прямые helper-ы и три репрезентативных случая разности. |

## Existing Test Sources

- [`MinkowskiDiffTests.cs`](../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiDiffTests.cs)

## Notes

- Старый direct-набор на `MinkowskiDiff` был маленьким; новая структура сохранила его смысл и добавила явную проверку `Geometric`.


