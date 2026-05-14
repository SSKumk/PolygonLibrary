# TODO

Backlog активного слоя `CGLibrary`.

Этот файл сводит вместе:
- активные задачи, относящиеся к `CGLibrary`;
- старые формулировки задач по `CGLibrary`;
- важный контекст и ссылки на код, тесты и документацию.

Рабочим backlog-файлом считать нужно именно этот.

Задачи LDG ведутся отдельно в [`../LDG/todo.md`](../LDG/todo.md).

Закрытые задачи ведутся отдельно в [`done.md`](./done.md).

## Приоритеты

- `High` - блокирует корректность, ведёт к красным тестам или ломает базовый алгоритмический контракт.
- `Medium` - важная инженерная или архитектурная задача, но не срочный стоппер.
- `Research` - исследовательская задача с незафиксированным целевым решением.
- `Low` - полезная доработка или организационная полировка.

## High Priority

Сейчас явных открытых high-priority задач нет.

Последняя закрытая high-priority задача:
- [`Simplex vertex-solution for H2V recovery`](./done.md)

## Medium Priority

### LinearBasis MultiplyTransposeBySelf

Статус: `open`

Контекст:
- [`LinearBasis.cs`](./CGLibrary/Basics/LinearBasis.cs)

Исходная формулировка:
- `LinearBasis: В одну операцию! MultiplyTransposeBySelf()`

### Проверки совместимости численных типов

Статус: `open`

Контекст:
- старые `Double-Tests` и `DoubleDouble-Tests` вынесены в [`../../legacy/PolygonLibrary/Tests`](../../legacy/PolygonLibrary/Tests);
- будущая проверка должна быть оформлена как отдельный проект или отдельный явный слой, а не как набор случайных старых данных.

Что имеется в виду:
- спроектировать проверку для очередного численного типа: тип корректно работает с основными алгоритмами библиотеки;
- определить минимальный набор алгоритмов и входных случаев;
- не смешивать эту проверку с быстрыми `double` regression-тестами.

### Реализовать алгоритм Bentley-Ottmann

Статус: `open`

Контекст:
- историческая заготовка перенесена в [`../../legacy/PolygonLibrary/CGLibrary/Geometry2D/Segments/Bentley-Ottmann`](../../legacy/PolygonLibrary/CGLibrary/Geometry2D/Segments/Bentley-Ottmann)
- исторический reference-тест перенесён в [`../../legacy/PolygonLibrary/Tests/LegacyMisc/BentlyOttmannTests.cs`](../../legacy/PolygonLibrary/Tests/LegacyMisc/BentlyOttmannTests.cs)
- активный слой отрезков:
  - [`Segment.cs`](./CGLibrary/Geometry2D/Segments/Segment.cs)
  - [`SegmentPair.cs`](./CGLibrary/Geometry2D/Segments/SegmentPair.cs)

Что имеется в виду:
- реализовать sweep-line алгоритм Bentley-Ottmann для пересечений отрезков;
- сначала описать целевой контракт и место алгоритма в активной структуре;
- после реализации добавить тесты в активный `DoubleGeometry`-слой и coverage-документы.

Исходная формулировка:
- `Реализовать алгоритм Бентли-Оттмана.`

### FaceLattice.Equals revisit

Статус: `open`

Контекст:
- [`FaceLattice.cs`](./CGLibrary/GeometryND/Polyhedra/FaceLattice.cs)
- coverage:
  - [`Documentation/Testing/CoveragePlan/FaceLattice/Index.md`](./Documentation/Testing/CoveragePlan/FaceLattice/Index.md)

Исходная формулировка:
- `FaceLattice.Equals()`

### Move non-geometry helpers into Toolkit

Статус: `open`

Контекст:
- [`CGLibrary/Toolkit`](./CGLibrary/Toolkit)

Исходная формулировка:
- `Закинуть в Toolkit всё, что не геометрия`

### ParamReader StringBuilder pass

Статус: `open`

Контекст:
- [`ParamReader.cs`](./CGLibrary/Toolkit/ParamReader.cs)

Исходная формулировка:
- `ParamReader посмотреть, где надо использовать StringBuilder`

## Research Priority

### HrepToFLrep

Статус: `research`

Контекст:
- [`HrepToFLrep.cs`](./CGLibrary/Algorithms/Polyhedra/HrepToFLrep.cs)
- coverage:
  - [`Documentation/Testing/CoveragePlan/HrepToFLrep/Index.md`](./Documentation/Testing/CoveragePlan/HrepToFLrep/Index.md)

Что имеется в виду:
- текущий `HrepToFLrep_Geometric` явно помечен как неработающий и не должен сейчас рассматриваться как active-fix target;
- нужно отдельно решить, стоит ли вообще достраивать решётку граней этим путём, а не через `GiftWrapping`;
- если возвращаться к теме, то сначала надо описать целевой алгоритм, ожидаемый контракт и отличие от GW-подхода;
- только после этого имеет смысл писать "нормальные" тесты не на current contract, а на целевое поведение.

Исходная формулировка:
- `HrepToFLrep: Исследовательская задача.`

### MinkowskiDiff cyclic numerical limits in double

Статус: `research`

Контекст:
- research-тесты:
  - [`MinkowskiDiffCyclicResearchTests.cs`](./Tests/DoubleGeometry/Research/MinkowskiDiff/MinkowskiDiffCyclicResearchTests.cs)
  - [`Tests/DoubleGeometry/Research/MinkowskiDiff/README.md`](./Tests/DoubleGeometry/Research/MinkowskiDiff/README.md)
- активный coverage note:
  - [`RegressionCases.md`](./Documentation/Testing/CoveragePlan/MinkowskiDiff/RegressionCases.md)

Проблема:
- `cyclic` в `double` не является обычным regression-case;
- он показывает численные срывы `Geometric` / `HrepToVrep_Geometric` и должен жить в отдельном исследовательском слое, пока не принято решение по целевому контракту.

## Low Priority

Пока явных задач низкого приоритета отдельно не выделено. Если нужно, сюда можно будет перенести косметические и не срочные доработки.

## Code TODO Registry

Этот раздел связывает `TODO`-пометки из `.cs`-кода с backlog-задачами.  
Пометки из кода не удаляются; здесь фиксируется, как они обработаны.

### Already Tracked By Active Tasks

- [`LinearBasis.cs`](./CGLibrary/Basics/LinearBasis.cs), line 85
  - `todo: В одну операцию! MultiplyTransposeBySelf()`
  - Статус: уже отражено в задаче `LinearBasis MultiplyTransposeBySelf`.

- [`HrepToFLrep.cs`](./CGLibrary/Algorithms/Polyhedra/HrepToFLrep.cs), lines 97 and 114
  - `todo: потенциальная проблема Теперь мы сравниваем не векторы, но узлы`
  - `todo: а в любом случае стоит собирать ребро?`
  - Статус: входят в исследовательскую задачу `HrepToFLrep`.

### Already Tracked By Closed Tasks

- [`MinkowskiDiffRegressionTests.cs`](./Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs), lines 53 and 79
  - `TODO high priority`
  - Статус: задача закрыта и перенесена в [`done.md`](./done.md), раздел `Simplex vertex-solution for H2V recovery`.

### New Medium Tasks From Code

#### LinearBasis.Orthonormalize

Статус: `open`

Источник:
- [`LinearBasis.cs`](./CGLibrary/Basics/LinearBasis.cs), line 142

Пометка в коде:
- `throw new NotImplementedException("todo");`

Что нужно сделать:
- определить контракт `Orthonormalize(Vector v)`;
- реализовать метод или явно понизить его до недоступного/internal API, если он не должен быть публичным;
- проверить, как он должен вести себя для пустого базиса и для вектора из span базиса.
- отдельно проверить численную устойчивость прямой реализации через `v - ToOriginalCoords(ProjectVectorToSubSpace(v))`, особенно на почти зависимых входах.

#### Polyline constructor checks

Статус: `open`

Источник:
- [`Polyline.cs`](./CGLibrary/Geometry2D/Polygons/Polyline.cs), line 135

Пометка в коде:
- `TODO: Write checks !!!`

Что нужно сделать:
- решить, какие именно проверки должны выполняться в конструкторе `Polyline(List<Vector2D> ps, PolylineOrientation orient, ...)`;
- синхронизировать это с общим policy по preconditions;
- при необходимости закрепить в XML и тестах.

#### ConvexPolytop non-full-dimensional H2V support

Статус: `open`

Источник:
- [`ConvexPolytop.cs`](./CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs), line 1434

Пометка в коде:
- `todo: научиться работать не с полноразмернымми многогранниками`

Что нужно сделать:
- решить, должен ли `HrepToVrep_Geometric` поддерживать неполноразмерные многогранники напрямую;
- если да, то описать целевой контракт и связь с affine-reduction;
- если нет, то закрепить ограничение жёстче в API и документации.

### New Research / Architectural Tasks From Code

#### Alternative H-representation algorithms for ConvexPolytop

Статус: `research`

Источник:
- [`ConvexPolytop.cs`](./CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs), line 218

Пометка в коде:
- `todo: Возможно стоит реализовать Double Description Method и/или Reverse Search Fukud-ы`

Что нужно сделать:
- отдельно оценить, нужен ли library-level альтернативный путь к `Hrep`;
- не смешивать это с текущей стабилизацией `GiftWrapping`/`SimplexMethod`.

### New Low-Priority Engineering Tasks From Code

#### MatrixMutable transpose naming / in-place policy

Статус: `open`

Источник:
- [`Matrix.cs`](./CGLibrary/Basics/Matrix.cs), line 1224

Пометка в коде:
- `todo: Transpose inplace нормально!`

Что нужно сделать:
- определиться, должен ли `MatrixMutable.Transpose()` быть in-place, иметь отдельную in-place версию или сохранить текущую семантику;
- синхронизировать имя, XML и ожидания API.

#### Line2D segment-dependent API

Статус: `open`

Источники:
- [`Line2D.cs`](./CGLibrary/Geometry2D/Line2D.cs), lines 3 and 178

Пометки в коде:
- `TODO: Uncomment when segments are ready`

Что нужно сделать:
- решить, нужны ли сейчас закомментированные segment-dependent части `Line2D`;
- либо вернуть их в живой код;
- либо убрать legacy-комментарий, если направление больше неактуально.

#### GammaPair comparator cleanup

Статус: `open`

Источник:
- [`GammaPair.cs`](./CGLibrary/Geometry2D/Polygons/ConvexPolygons/GammaPair.cs), line 35

Пометка в коде:
- `todo: Debug.Assert(other != null, nameof(other) + " != null"); убрать такую дичь`

Что нужно сделать:
- убрать исторический comment-to-self;
- привести `CompareTo` / null-handling к аккуратному и документированному виду.

#### ConvexPolytop distance epigraph micro-check

Статус: `open`

Источник:
- [`ConvexPolytop.cs`](./CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs), line 899

Пометка в коде:
- `todo: кажется, что можно не проверять!`

Что нужно сделать:
- проверить, действительно ли `.Where(ContainsNonStrict)` лишний в этой ветке;
- если да, удалить и зафиксировать reasoning тестом или комментарием.

#### MinkowskiDiff callback naming

Статус: `open`

Источник:
- [`MinkowskiDiff.cs`](./CGLibrary/Algorithms/Polyhedra/MinkowskiDiff.cs), line 106

Пометка в коде:
- `todo Как назвать?`

Что нужно сделать:
- дать осмысленное имя callback-параметру `doSubtract`, чтобы сигнатура `MinkDiff(...)` читалась без внутреннего знания алгоритма.

#### BasicPolygon contours and vertex-order cleanup

Статус: `open`

Источники:
- [`BasicPolygon.cs`](./CGLibrary/Geometry2D/Polygons/BasicPolygon.cs), line 33
- [`BasicPolygon.cs`](./CGLibrary/Geometry2D/Polygons/BasicPolygon.cs), lines 104 and 131

Пометки в коде:
- `todo Привести к единообразному виду как в Segment`
- `todo ???` у `Vertices.Sort()`

Что нужно сделать:
- привести `Contours` к более единообразному API;
- решить, нужен ли детерминированный порядок вершин в соответствующих конструкторах или комментарии уже потеряли смысл.

#### GiftWrapping cleanup / optimization notes

Статус: `open`

Источники:
- [`GiftWrapping.cs`](./CGLibrary/Algorithms/Polyhedra/GiftWrapping/GiftWrapping.cs), line 280
- [`GiftWrapping.cs`](./CGLibrary/Algorithms/Polyhedra/GiftWrapping/GiftWrapping.cs), line 384
- [`GiftWrapping.cs`](./CGLibrary/Algorithms/Polyhedra/GiftWrapping/GiftWrapping.cs), line 519

Пометки в коде:
- `todo: norm?`
- `todo: Может быть, что если после удаления точек их стало d+1, то создать симплекс и перестать овыпукляться?`
- `todo !важная проверка!`

Что нужно сделать:
- проверить, какие из этих пометок ещё содержательны;
- развести optimisation idea, debug-check и stale comment;
- не менять алгоритм без отдельного решения, но убрать неясные хвосты.

### Archival / Historical Notes From Code

#### Archived Vector precondition policy discussion

Статус: `archival-note`

Источник:
- удалённый архивный `VectorTests.cs`, lines 85, 114, 205, 309, 355, 448, 533

Повторяющаяся пометка:
- `todo: Надо ли делать throw или же Debug.Assert (как сейчас)`

Смысл:
- старый спор про runtime validation policy для `Vector`;
- сейчас это уже не active test TODO, а историческая заметка.

#### Archived ConvexPolygonCut testability note

Статус: `archival-note`

Источник:
- удалённый архивный `ConvexPolygonCutTests.cs`, line 11

Пометка в коде:
- `todo Придумать как тестировать`

Смысл:
- старый комментарий из уже архивированного набора;
- как отдельная active-задача сейчас не поднимается.

## Imported Historical Notes

Ниже сохранены старые формулировки, чтобы не потерять исторический контекст backlog.

```text
*) FaceLattice.Equals()
```
