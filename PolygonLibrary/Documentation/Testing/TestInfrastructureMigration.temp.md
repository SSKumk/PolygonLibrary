# Test Infrastructure Migration Worklog

## Purpose

Временный рабочий файл для упорядочивания тестовой инфраструктуры.

Использовать:
- когда нужно понять, что из helper-слоя остаётся рядом с тестами;
- когда нужно решить, что считать общей тестовой инфраструктурой;
- когда нужно отделить тестовые генераторы от документации и архивных материалов.

Не использовать:
- как постоянную документацию API;
- как замену локальных `*Assert.cs` и `*TestData.cs`.

## Current State

Сейчас helper-слой тестов состоит из двух разных типов сущностей.

### Local Helpers Near Test Suites

Они уже лежат правильно:
- `*Assert.cs`
- `*TestData.cs`

Примеры:
- [`ConvexPolytopAssert.cs`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopAssert.cs)
- [`ConvexPolytopTestData.cs`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTestData.cs)
- [`GiftWrappingTestData.cs`](../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingTestData.cs)
- [`FaceLatticeAssert.cs`](../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeAssert.cs)

### Shared Test Infrastructure

Сейчас лежит в:
- [`TestsBase.cs`](../../Tests/TestInfrastructure/TestsBase.cs)
- [`TestsPolytopes.cs`](../../Tests/TestInfrastructure/TestsPolytopes.cs)

Это уже не локальные helper'ы, а общий генераторный слой:
- affine-преобразования для тестов;
- генераторы swarm/политопов;
- canonical test objects;
- sphere/simplex/cube factories;
- ready-made `ConvexPolytop` / `FaceLattice` helpers.

### Non-Code Materials

Уже вынесено из `Tests`:
- [`О n-сфере.pdf`](../References/GeometryND/Spheres/О%20n-сфере.pdf)
- [`PolyhedraNaming.txt`](../Development/PolyhedraNaming.txt)

## Principles

### Keep Local

Оставлять рядом с набором:
- `*Assert.cs`
- `*TestData.cs`
- маленькие helper'ы, используемые только одним набором тестов

### Extract to Shared Test Infrastructure

Выносить в общий слой:
- генераторы, используемые несколькими наборами;
- affine helper'ы для regression/stress;
- canonical polytope factories;
- общие seed-based utility.

### Keep Out of CGLibrary

Не переносить в `CGLibrary`:
- test-only generators;
- stress/random scenario builders;
- helper'ы, смысл которых только в тестировании.

### Move Out of Tests

Убирать из `Tests/...`:
- статьи;
- naming notes;
- прочие справочные и организационные материалы.

## Target Structure

### Tests Layer

- `Tests/DoubleGeometry/...`
  - локальные `Assert` и `TestData`

- `Tests/TestInfrastructure/Common/`
  - низкоуровневые общие helper'ы для тестов

- `Tests/TestInfrastructure/Generators/`
  - генераторы тестовых геометрических данных

- `Tests/Archive/`
  - legacy-тесты и устаревшие test-only файлы

### Documentation Layer

- `Documentation/References/...`
  - статьи и PDF

- `Documentation/Development/...`
  - naming notes, соглашения, справочные текстовые материалы

## Proposed Split of Current Shared Helpers

### `TestsBase.cs`

Предлагаемый будущий смысл:
- `Common/TestAffineTransforms.cs`
- `Common/TestRandomHelpers.cs`

Что сюда относится:
- `GenInner`
- `GenShift`
- `Rotate`
- `RotateRND`
- `ShiftAndRotate`
- `MakeRotationMatrix`

### `TestsPolytopes.cs`

Предлагаемый будущий смысл:

- `Generators/Polyhedra/PolytopeFactoryCatalog.cs`
  - canonical objects: `Cube3D`, `Cube4D`, `Simplex3D`, `Simplex4D`
  - canonical face lattices: `CubeFL`, `SimplexFL`, `SphereFL`

- `Generators/Polyhedra/PolytopePointGenerators.cs`
  - `Cube01`
  - `Simplex`
  - `SimplexRND`
  - `CyclicPolytop`

- `Generators/Polyhedra/SpherePointGenerators.cs`
  - `Sphere_list`
  - `MakePointsOnSphere_3D`

- `Generators/Polyhedra/PolytopeWrappingFactories.cs`
  - `CubeGW`
  - `Simplex(...)` returning `GiftWrapping`
  - other wrappers over `GiftWrapping`

## Current Usage Notes

Сейчас `TestsBase` и `TestsPolytopes` ещё реально используются:
- новым слоем [`MinkowskiSum`](../../Tests/DoubleGeometry/Algorithms/MinkowskiSum)
- legacy `GW_Tests`
- legacy `MinkowskiDiffTests`
- `SpeedTests`
- `OtherTests/Sandbox`

Значит:
- резко ломать этот слой пока нельзя;
- сначала нужен план миграции потребителей;
- потом уже физический перенос и разбиение файлов.

## Fixed Decisions

- Имя общего helper-слоя зафиксировано как `Tests/TestInfrastructure`.
- Benchmark/speed helper'ы считаются отдельным слоем и не определяют структуру функционального test-infrastructure.

## Open Questions

- Нужно ли сразу дробить `TestsPolytopes.cs`, или сначала только переименовать и перенести папку?
- Нужно ли оставлять speed/performance потребителей на том же namespace, или потом вынести и их в отдельную benchmark-infrastructure?

## Recommended Migration Order

1. Унести не-кодовые материалы из старого `ToolsForTests` в `Documentation`.
2. Зафиксировать целевое имя и место общего helper-слоя.
3. Перевести активные новые тесты на новый путь/namespace.
4. Только потом дробить `TestsBase.cs` и `TestsPolytopes.cs`.
5. После завершения — архивировать или удалить старый `ToolsForTests`.

## User Notes

Пиши сюда мысли по раскладке helper-слоя, что должно остаться общим, а что нет.

-
