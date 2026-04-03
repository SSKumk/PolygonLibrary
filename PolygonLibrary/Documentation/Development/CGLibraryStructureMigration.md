# CGLibrary Structure Migration

План по рационализации папочной структуры `CGLibrary` без изменения единого `namespace CGLibrary`.

## Goal

- сохранить единый `namespace CGLibrary`, потому что библиотека во многом организована как набор файлов одного большого слоя, включая `partial`-части `Geometry<TNum, TConv>`;
- улучшить физическую структуру папок для навигации, сопровождения и дальнейшей миграции;
- отделить сущности от алгоритмов;
- завести явную папку `Archive/` для архивного и legacy-кода, который пока не удаляется окончательно.

## Target Structure

- `Basics/`
  Что это: универсальные геометрические и линейно-алгебраические примитивы.
  Когда использовать: когда тип не привязан жёстко ни к 2D, ни к `nD`, а является базовым строительным блоком библиотеки.

- `LinearMath/`
  Что это: низкоуровневая линейная алгебра и оптимизационные алгоритмы.
  Когда использовать: когда код решает вычислительные задачи сам по себе, а не описывает геометрические сущности.

- `Geometry2D/`
  Что это: плоская геометрия, полигоны, полилинии, отрезки и связанные структуры.
  Когда использовать: когда сущность или операция живёт именно в двумерной геометрии.

- `GeometryND/`
  Что это: многомерные геометрические сущности, прежде всего многогранники и решётки граней.
  Когда использовать: когда код описывает `nD`-объекты как доменные сущности, а не алгоритмы их построения.

- `Algorithms/`
  Что это: алгоритмы построения, преобразования и обхода геометрических объектов.
  Когда использовать: когда код является алгоритмом над уже существующими сущностями, а не самой сущностью.

- `Toolkit/`
  Что это: общие helper-методы, соглашения о числах, ввод-вывод и техническая инфраструктура.
  Когда использовать: когда код не является доменной геометрической сущностью и не относится к отдельному алгоритмическому слою.

- `Archive/`
  Что это: архивный, legacy или экспериментальный код, который не должен участвовать в целевой рабочей структуре.
  Когда использовать: когда файл или папка сохраняется ради истории или отложенного анализа, но не считается частью актуальной архитектуры.

## Proposed Mapping

### Basics

- `Vector.cs`
- `Vector2D.cs`
- `Matrix.cs`
- `LinearBasis.cs`
- `AffineBasis.cs`
- `HyperPlane.cs`
- `CauchyMatrix.cs`

Note:
- `Line2D.cs` логичнее увести в `Geometry2D`, потому что это уже не общий примитив, а сущность плоской геометрии.

### LinearMath

- `Decomposition.cs`
- `GaussSLE.cs`
- `SimplexMethod.cs`
- `FourierMotzkin.cs`

### Geometry2D

- `Line2D.cs`
- `Segments/`
  - `Segment.cs`
  - `SegmentPair.cs`
- `Polygons/`
  - `BasicPolygon.cs`
  - `Polyline.cs`
  - `PolygonTools.cs`
- `ConvexPolygons/`
  - `ConvexPolygon.cs`
  - `SupportFunction.cs`
  - `GammaPair.cs`
  - `Intersection.cs`

### GeometryND

- `Polyhedra/`
  - `ConvexPolytop.cs`
  - `FaceLattice.cs`

### Algorithms

- `ConvexHull2D/`
  - `Convexification.cs`
- `Polyhedra/`
  - `GiftWrapping.cs`
  - `HrepToFLrep.cs`
  - `MinkowskiSum.cs`
  - `MinkowskiDiff.cs`
  - `BaseSubCP.cs`
  - `SubPoint.cs`
  - `SubPoint2D.cs`
  - `SubPolytop.cs`
  - `SubSimplex.cs`
  - `SubTwoDimensional.cs`
  - `SubTwoDimensionalEdge.cs`
  - `SubZeroDimensional.cs`

### Toolkit

- `Tools.cs`
- `RandomLC.cs`
- `Combinations.cs`
- `Extensions.cs`
- `ParamReader.cs`
- `ParamWriter.cs`
- `Hashes.cs`

### Archive

Candidates:
- `Polygons/Something/`
- разовые временные хвосты вроде `NewFile1.txt`
- иные legacy-папки, которые не хочется удалять сразу, но не нужно держать в рабочей структуре

## Migration Stages

### Stage 1. Safe Cleanup

- удалить явный мусор вроде `NewFile1.txt`;
- завести `CGLibrary/Archive/`;
- перенести в `Archive/` всё, что уже явно не относится к целевой рабочей структуре.

### Stage 2. 2D / nD Split

- вынести `Line2D`, `Segments` и весь polygon-слой в `Geometry2D/`;
- вынести `ConvexPolytop` и `FaceLattice` в `GeometryND/`.

### Stage 3. Algorithm Extraction

- вынести `Convexification` из `Toolkit/` в `Algorithms/ConvexHull2D/`;
- вынести `GiftWrapping`, `Minkowski*`, `HrepToFLrep` и их внутренние helper-типы в `Algorithms/Polyhedra/`.

### Stage 4. Final Cleanup

- обновить ссылки в документации;
- обновить `RepoStructure.md`;
- проверить, не осталось ли папок с неинформативными именами;
- при необходимости унифицировать физические подпапки без изменения `namespace`.

## Constraints

- единый `namespace CGLibrary` сохраняется;
- миграция должна быть поэтапной, без массового одномоментного переезда всего дерева;
- сначала перемещаются архивные и явно лишние хвосты, потом рабочие сущности;
- после каждого этапа нужно синхронизировать документацию и тесты.
