# TestInfrastructure

## Scope

Общий слой тестовой инфраструктуры.

Использовать:
- когда helper нужен нескольким наборам тестов;
- когда нужен генератор swarm/политопов для regression и stress;
- когда нужны общие affine-helper'ы для тестов.

Не использовать:
- для локальных `*Assert.cs` и `*TestData.cs`;
- для статей, PDF и текстовых справок;
- как место для benchmark/performance-логики.

## Current Contents

- [`TestsBase.cs`](TestsBase.cs)
  Объект: общие affine и random helper'ы для тестов.
  Когда использовать: когда нужен поворот, сдвиг, случайная комбинация или общая матричная подготовка тестового входа.

- [`TestsPolytopes.cs`](TestsPolytopes.cs)
  Объект: общий генераторный слой для swarm, политопов и canonical test objects.
  Когда использовать: когда нужен cube/simplex/sphere/cyclic-polytope сценарий, готовый swarm или canonical `ConvexPolytop`/`FaceLattice`.

- [`TConvertors.cs`](TConvertors.cs)
  Объект: общие convertor-типы для `double`, `DoubleDouble.ddouble` и `Rational`.
  Когда использовать: когда тесту нужно поднять `Geometry<TNum, TConv>` для конкретного числового типа.

- [`StaticHelpers.cs`](StaticHelpers.cs)
  Объект: простые helper'ы для создания и сравнения double-векторов и массивов.
  Когда использовать: когда тесту нужен короткий `V(...)` или базовое сравнение координат через `Tools.EQ`.

## Notes

- Локальные `Assert/TestData` остаются рядом с конкретными тестовыми наборами в `Tests/DoubleGeometry/...`.
- Benchmark и speed-тесты считаются отдельным слоем и не определяют структуру этой папки.
