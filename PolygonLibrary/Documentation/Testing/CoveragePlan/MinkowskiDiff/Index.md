# MinkowskiDiff

## Scope

Класс [`MinkowskiDiff.cs`](../../../../CGLibrary/Algorithms/Polyhedra/MinkowskiDiff.cs) реализует разность Минковского через `Hrep` и поиск экстремальных вершин вычитаемого политопа.

В активный слой текущего этапа входят:

- helper-контракты `FindExtrInCPOnVector_Naive` и `doSubtract`;
- `Naive` и `Geometric` на representative-сценариях `cube - segment`;
- agreement `Naive == Geometric` на `sphere`.

## Topics

- [`BasicCases.md`](BasicCases.md) - helper-ы и базовый `cube - segment`.
- [`RegressionCases.md`](RegressionCases.md) - нетривиальные `sphere` и `cyclic`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Basic Cases | `x` | Зафиксированы helper-контракты и базовая геометрия `cube - segment`. |
| Regression Cases | `x` | Перенесён `sphere`; `cyclic` вынесен в открытый алгоритмический вопрос. |

## Existing Test Sources

- [`MinkowskiDiffTests.cs`](../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiDiffTests.cs)
- [`MinkowskiDiffTests.cs`](../../../../Tests/DoubleDouble-Tests/Minkowski-Tests/MinkowskiDiffTests.cs)

## Notes

- Старый direct-набор на `MinkowskiDiff` был маленьким; новая структура сохраняет его смысл и отдельно фиксирует agreement `Naive` и `Geometric`.
- `Cyclic` пока не входит в активный зелёный слой: в `double` он раскрывает реальную проблему `Geometric`, а не просто недостающий тест.
- Отдельный `Stress` сейчас не требуется: heavy generator-based или combinatorial слоя в legacy нет.
