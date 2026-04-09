# DONE

Канонический журнал закрытых задач репозитория.

Этот файл хранит:
- задачи, которые были закрыты и больше не входят в активный backlog;
- исторически закрытые пункты из прежнего `todo.md`, чтобы не держать их в рабочем списке;
- краткую фиксацию того, чем задача была закрыта и где лежат подтверждающие тесты или документация.

Активным backlog-файлом по-прежнему остаётся [`todo.md`](./todo.md).

## Recently Closed

### Simplex vertex-solution for H2V recovery

Статус: `closed`

Дата закрытия:
- `2026-04-09`

Что было сделано:
- [`SimplexMethod`](./CGLibrary/LinearMath/SimplexMethod.cs) оставлен LP-solver'ом без ложной гарантии "вершины исходного многогранника";
- в [`ConvexPolytop.FindInitialVertex_Simplex`](./CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs) добавлено доведение simplex-optimum до вершины через `RefineOptimalPointToVertex`;
- добавлены прямые тесты на optimal-face случаи и на вырожденные вершины;
- coverage-документация вынесена в отдельную тему `InitialVertexRecovery`;
- алгоритм отдельно описан в reference-слое.

Подтверждение:
- production:
  - [`ConvexPolytop.cs`](./CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs)
- тесты:
  - [`ConvexPolytopSimplexVertexRecoveryTests.cs`](./Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs)
  - [`MinkowskiDiffRegressionTests.cs`](./Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs)
- coverage:
  - [`Documentation/Testing/CoveragePlan/ConvexPolytop/InitialVertexRecovery.md`](./Documentation/Testing/CoveragePlan/ConvexPolytop/InitialVertexRecovery.md)
- algorithm note:
  - [`Documentation/References/Algorithms/Polyhedra/InitialVertexRecovery/SimplexOptimalFaceRefinement.md`](./Documentation/References/Algorithms/Polyhedra/InitialVertexRecovery/SimplexOptimalFaceRefinement.md)

Коммиты:
- `9e8b948` `Refine simplex optimum to a polytope vertex`
- `c0f9fab` `Add simplex vertex recovery tests`
- `652ffa5` `Document ConvexPolytop initial vertex recovery coverage`
- `b8b92f2` `Add algorithm note for simplex optimal face refinement`

Результат:
- `MinkowskiDiffRegressionTests` теперь проходят зелёно.

## Imported Closed History From Previous todo.md

### Visualization

1. [x] ~~Модуль печати кадра в файл.~~
1. [x] ~~Все настройки (цвет, размер, толщина) всех объектов.~~
1. [x] ~~На каждом кадре выводим всю траекторию целиком, её сегменты соединяем цилиндрами.~~

### CGLibrary

1. [x] ~~AffineBasis: Возможно ли привести их к какому-то каноническому виду? -- Да, можно. Это RREF для LinearBasis и проекция 0 в качестве Origin.~~
1. [x] ~~ConvexPolytop: Избавиться от привязки к InnerPoint во всяких сравнениях.~~
1. [x] ~~TODO XML у методов ConvexPolytope DistanceTo...!~~
1. [x] ~~Внести в ParamReader ReadBool(). Подумать над форматом в файл  [Формата!](./Documentation/Development/LDG/DataFormat.md)~~
1. [x] ~~ParamReader -- научиться читать строку из чисел и превращать её в массив.~~

### Markdown-файлы

1. [x] ~~Описание файла [IO-многогранников](./Documentation/Development/LibPolytopeFormat.md)~~
1. [x] ~~Надо ли в файл [многогранника](./Documentation/Development/LDG/IOFormat/Polytopes.md) добавлять поле `doRedundancy`, чтобы  исключать лишние объекты? Надо.~~
1. [x] ~~Какие символы НЕ может включать в себя Имя_поля в стандартной записи.~~

### Счёт примеров

1. [x] ~~Заготовить файлы различных динамик~~
1. [x] ~~простые движения 2D~~
1. [x] ~~простые движения 3D~~
1. [x] ~~простые движения 4D~~
1. [x] ~~Заготовить файлы различных explicit sets~~
