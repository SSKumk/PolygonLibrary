# MinkowskiDiff

## Scope

Класс [`MinkowskiDiff.cs`](../../../../CGLibrary/Algorithms/Polyhedra/MinkowskiDiff.cs) реализует разность Минковского через `Hrep` и поиск экстремальных вершин вычитаемого политопа.

В активный слой текущего этапа входят:

- helper-контракты `FindExtrInCPOnVector_Naive` и `doSubtract`;
- точные `2D` и `3D` сценарии на вычитании точки и короткого осевого отрезка;
- неосевые `2D` и `3D` regression-примеры;
- agreement `Naive == Geometric` на `sphere`;
- красные TODO-signal сценарии `tetrahedron - point` и `octahedron - point`, фиксирующие баг vertex-selection в simplex-пути `HrepToVrep_Geometric`.

## Topics

- [`BasicCases.md`](BasicCases.md) - helper-ы и базовый `cube - segment`.
- [`RegressionCases.md`](RegressionCases.md) - нетривиальные `2D`/`3D` случаи, signal-тесты на simplex-bug и заметки про `cyclic`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Basic Cases | `x` | Зафиксированы helper-контракты и точная базовая геометрия `2D`/`3D`. |
| Regression Cases | `in_progress` | Перенесены неосевые и smooth `2D`/`3D` случаи; `cyclic` вынесен в research, а simplex-bug зафиксирован красными signal-тестами. |

## Existing Test Sources

- ``MinkowskiDiffTests.cs``
- ``MinkowskiDiffTests.cs``
- [`MinkowskiDiffCyclicResearchTests.cs`](../../../../Tests/DoubleGeometry/Research/MinkowskiDiff/MinkowskiDiffCyclicResearchTests.cs)

## Notes

- Старый direct-набор на `MinkowskiDiff` был маленьким; новая структура сохраняет его смысл и отдельно фиксирует agreement `Naive` и `Geometric`.
- `Cyclic` пока не входит в активный зелёный слой: в `double` он раскрывает реальную проблему `Geometric`, а не просто недостающий тест.
- `tetrahedron - point` и `octahedron - point` теперь оставлены в активном слое как красные TODO-signal тесты, чтобы не потерять simplex-bug в `FindInitialVertex_Simplex`.
- Отдельный `Stress` сейчас не требуется: heavy generator-based или combinatorial слоя в legacy нет.
