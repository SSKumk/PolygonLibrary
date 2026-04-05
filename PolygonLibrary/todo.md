# TODO

Канонический backlog репозитория.

Этот файл сводит вместе:
- активные задачи из прежнего `todo.md`;
- legacy-заметки из [`TODO.txt`](./TODO.txt);
- важный контекст и ссылки на код, тесты и документацию.

`TODO.txt` пока сохраняется как legacy-source, но рабочим файлом считать нужно именно этот.

## Приоритеты

- `High` - блокирует корректность, ведёт к красным тестам или ломает базовый алгоритмический контракт.
- `Medium` - важная инженерная или архитектурная задача, но не срочный стоппер.
- `Research` - исследовательская задача с незафиксированным целевым решением.
- `Low` - полезная доработка или организационная полировка.

## High Priority

### Simplex vertex-solution for H2V recovery

Статус: `open`

Суть:
- [`SimplexMethod`](./CGLibrary/LinearMath/SimplexMethod.cs) / [`ConvexPolytop.FindInitialVertex_Simplex`](./CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs) должны гарантировать, что для `HrepToVrep_Geometric` возвращается именно вершина, а не произвольная точка оптимального лица.

Контекст:
- красные signal-тесты:
  - [`MinkowskiDiffRegressionTests.cs`](./Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs)
- описание наблюдаемого поведения:
  - [`RegressionCases.md`](./Documentation/Testing/CoveragePlan/MinkowskiDiff/RegressionCases.md)
- связанный код:
  - [`SimplexMethod.cs`](./CGLibrary/LinearMath/SimplexMethod.cs)
  - [`ConvexPolytop.cs`](./CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs)

Пример, который сейчас ломается:
- после вычитания точки `p = (0.1, 0.2, 0.3)` из тетраэдра `conv{0, e1, e2, e3}` получается система
  - `x >= -0.1`
  - `y >= -0.2`
  - `z >= -0.3`
  - `x + y + z <= 0.4`
- `SimplexMethod.Solve(HPs, _ => 1)` возвращает допустимую оптимальную точку `(0.4, 0, 0)`, то есть точку на грани `x + y + z = 0.4`, а не вершину;
- затем `FindInitialVertex_Simplex` пытается восстановить вершину по `BasisInequalitiesID`, получает только одну исходную грань и падает на `GaussSLE`.

Что нужно обеспечить:
- либо сам `SimplexMethod` должен возвращать `vertex-solution`;
- либо должен появиться отдельный корректный `vertex-mode`, который используется в `ConvexPolytop.HrepToVrep_Geometric`.

Исходная формулировка:
- `High priority. SimplexMethod / ConvexPolytop.FindInitialVertex_Simplex: гарантировать, что симплекс для H2V-восстановления возвращает именно вершину, а не произвольную точку оптимального лица.`

## Medium Priority

### LinearBasis cheap incremental RREF

Статус: `open`

Контекст:
- [`LinearBasis.cs`](./CGLibrary/Basics/LinearBasis.cs)
- coverage:
  - [`Documentation/Testing/CoveragePlan/LinearBasis/Index.md`](./Documentation/Testing/CoveragePlan/LinearBasis/Index.md)

Исходная формулировка:
- `LinearBasis: RREF научиться "дёшево" обновлять (и вообще хранить) при добавлении очередного вектора.`

### LinearBasis MultiplyTransposeBySelf

Статус: `open`

Контекст:
- [`LinearBasis.cs`](./CGLibrary/Basics/LinearBasis.cs)

Исходная формулировка:
- `LinearBasis: В одну операцию! MultiplyTransposeBySelf()`

### Immutable / mutable split for AffineBasis and LinearBasis

Статус: `open`

Контекст:
- [`LinearBasis.cs`](./CGLibrary/Basics/LinearBasis.cs)
- [`AffineBasis.cs`](./CGLibrary/Basics/AffineBasis.cs)
- потенциально затрагивает:
  - [`FaceLattice.cs`](./CGLibrary/GeometryND/Polyhedra/FaceLattice.cs)
  - [`HyperPlane.cs`](./CGLibrary/Basics/HyperPlane.cs)

Что нужно продумать и сделать:
- базовый `LinearBasis` и базовый `AffineBasis` должны опираться на реально immutable-внутреннее представление, а не на mutable-тип "по договорённости";
- zero-copy сценарии с `needCopy: false` должны остаться доступны для быстрых путей, но без скрытого aliasing mutable-состояния;
- `LinearBasisMutable` и `AffineBasisMutable` должны остаться отдельным явным mutable-слоем;
- нужно пересмотреть copy ctor'ы, factory-методы и поля хранения (`_Basis`, `_linearBasis`), чтобы контракты типов и внутренняя реализация больше не расходились;
- отдельно проверить влияние на `FaceLattice`, `HyperPlane` и другие места, где сейчас рассчитывается на дешёвое переиспользование базисов.

Исходная формулировка:
- `AffineBasis / LinearBasis: Полностью развести immutable- и mutable-сущности.`

### Разобраться с картинками и визуальными артефактами

Статус: `open`

Контекст:
- текущие материалы для визуальной проверки живут здесь:
  - [`Tests/Double-Tests/Minkowski-Tests/3D-pictures`](./Tests/Double-Tests/Minkowski-Tests/3D-pictures)
  - [`Tests/DoubleDouble-Tests/Minkowski-Tests/3D-pictures`](./Tests/DoubleDouble-Tests/Minkowski-Tests/3D-pictures)

Что имеется в виду:
- отдельно решить судьбу `3D-pictures` и похожих файлов, которые сейчас используются для визуальной проверки результатов "глазами";
- понять, должны ли они жить в `Tests`, в `Documentation/References` или в отдельном слое visual-regression материалов;
- зафиксировать для них понятный статус, чтобы они не выглядели случайным хвостом миграции.

Исходная формулировка:
- `Средний приоритет. Разобраться с картинками и визуальными артефактами в репозитории.`

### Дочистить структуру папки Tests

Статус: `open`

Контекст:
- текущая структура:
  - [`Tests/Archive/README.md`](./Tests/Archive/README.md)
  - [`Tests/TestInfrastructure/README.md`](./Tests/TestInfrastructure/README.md)
  - [`Documentation/Development/RepoStructure.md`](./Documentation/Development/RepoStructure.md)

Что имеется в виду:
- ещё раз пройтись по `Tests` и убедиться, что там не осталось лишних исторических директорий, случайных файлов и неочевидных хвостов;
- проверить, не нужно ли дополнительно упорядочить `Archive`, `OtherTests`, `SpeedTests`, `Figurs for algorithms` и другие служебные слои;
- после этого зафиксировать целевую чистую структуру `Tests`, чтобы новые хвосты больше не накапливались.

Исходная формулировка:
- `Средний приоритет. Аккуратно дочистить структуру папки Tests.`

### Visualization / Bridges / Trajectories factory methods

Статус: `open`

Контекст:
- связано с прикладными проектами визуализации и мостов;
- потенциально должно лечь на общий интерфейс построения примеров и конфигураций.

Исходная формулировка:
- `Сделать фабричные методы для проектов Мосты, Траектории и Визуализация. Чтобы писать Bridge.Create(... входные данные, NumType, NumAccuracy)`

### FaceLattice.Equals revisit

Статус: `open`

Контекст:
- [`FaceLattice.cs`](./CGLibrary/GeometryND/Polyhedra/FaceLattice.cs)
- coverage:
  - [`Documentation/Testing/CoveragePlan/FaceLattice/Index.md`](./Documentation/Testing/CoveragePlan/FaceLattice/Index.md)

Исходная формулировка из [`TODO.txt`](./TODO.txt):
- `FaceLattice.Equals()`

### LDG folder structure and bridge-computation workflow

Статус: `open`

Контекст:
- существующая документация:
  - [`FolderStructure.md`](./Documentation/Development/LDG/FolderStructure.md)
  - [`GameFolderStructure.md`](./Documentation/Development/LDG/GameFolderStructure.md)
  - [`DataFormat.md`](./Documentation/Development/LDG/DataFormat.md)
  - [`IOFormat/`](./Documentation/Development/LDG/IOFormat)

Что нужно продумать:
- структуру папок и файлов для динамик, ограничений на управления и терминальных множеств;
- формат одного "примера" и связь между динамикой, `P`, `Q` и терминальным объектом;
- формат связи и состав файлов для explicit / Minkowski / function / epigraph сценариев.

Исходные формулировки из [`TODO.txt`](./TODO.txt):
- `12.2024 Вычисление мостов`
- `Структура папок LDG`
- `Подумать о формате файла связи`

### LDG2D namespace / project

Статус: `open`

Контекст:
- [`LDG`](./LDG)

Исходная формулировка:
- `Добавить в нашу библиотеку проект LDG2D. Для этого сделать пространство имён.`

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

### Polytope precomputation

Статус: `open`

Контекст:
- связано с дорогостоящими многогранными вычислениями и, вероятно, с LDG / bridge-слоем.

Исходная формулировка:
- `Предпросчёт многогранников.`

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

## Closed History From Previous todo.md

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

## Imported Notes From TODO.txt

Ниже сохранён исходный текст из [`TODO.txt`](./TODO.txt), чтобы не потерять старые формулировки.

```text
*) FaceLattice.Equals()
========================================
12.2024
Вычисление мостов

) Структура папок LDG

  1. Много файлов динамики (name?, A,B,C, dt, t0, T, ProjDim, ProjInd, n-p-q-dimension)
  2. Много файлов многогранников ограничений на управления / терминальных множеств (name?)

  Папка ОДНОГО примера
    Файл примера:
      ИМЯ_динамики
      ИМЯ_P  [ссылка на готовый ИЛИ на тип, который умеет считаться в программе]
      ИМЯ_Q  [ссылка на готовый ИЛИ на тип, который умеет считаться в программе]
      Задание терминального объекта
            задание одного многогранника это базовый многогранник + коэффициент масштабирования + перенос (флаг есть ли перенос, если есть, то прочитать вектор переноса)
        Explicit  -- список многогранников [список имён]
        Минковский  -- многогранник [имя] + набор констант
        Функция  -- тип функции + набор констант
        Надграфик  -- тип функции + срезающая константа


) Много терминальных множеств ТОЛЬКО в случае Explicit sets. В остальных случаях только 1 объект.

) Подумать о формате файла связи
```
