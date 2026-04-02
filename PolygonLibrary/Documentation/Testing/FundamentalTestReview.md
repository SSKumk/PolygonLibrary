# Fundamental Test Review

Этот файл нужен для ревизии фундаментального слоя `double` без алгоритмов.

Назначение:

- фиксировать сценарии тестирования, которые не были предусмотрены в текущем coverage plan;
- фиксировать реальные падения тестов после запуска фундаментального слоя;
- не исправлять здесь код и не предлагать "молчаливые" изменения контракта, а только документировать наблюдения;
- отделять проблемы фундамента от временно замороженного алгоритмического слоя.

Граница текущей ревизии:

- входит: `Tests/DoubleGeometry/Basics`, `Tests/DoubleGeometry/Polygons`, `Tests/DoubleGeometry/Polyhedra`;
- не входит: `Tests/DoubleGeometry/Algorithms`, legacy-алгоритмы и любые долгие stress/perf-наборы.

## Initial Sweep Metadata

- Дата ревизии: `2026-04-02`
- Команда запуска:
  - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry&FullyQualifiedName!~Tests.DoubleGeometry.Algorithms" --logger "trx;LogFileName=FundamentalDoubleGeometry.trx"`
- Итог:
  - `460` выполнено
  - `444` passed
  - `16` failed

## Incremental Verification

- `2026-04-02`
  - `AffineBasis`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Basics.AffineBasis"`
  - Итог:
    - `43` passed
    - `0` failed
- `2026-04-02`
  - `Tools`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Basics.Tools"`
  - Итог:
    - `11` passed
    - `0` failed
- `2026-04-02`
  - `CauchyMatrix`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Basics.CauchyMatrix"`
  - Итог:
    - `4` passed
    - `1` failed

## Status Summary

| Class | Status | Missing scenarios | Failing tests | Notes |
| --- | --- | --- | --- | --- |
| `Line2D` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `Vector2D` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `Segment` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `Intersection` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `SegmentPair` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `Tools` | `checked_clean` | `0` | `0` | Первичные падения оказались хрупкими тестовыми ожиданиями на границе `double`; целевой повторный прогон зелёный. |
| `Vector` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `Matrix` | `checked_clean` | `0` | `0` | Собственные быстрые тесты проходят; сбой проявляется через `CauchyMatrix`. |
| `CauchyMatrix` | `has_failures` | `0` | `1` | Базовый сбой выбора стартового узла и backward partial-step сняты; осталась только точность диагонального случая. |
| `LinearBasis` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `AffineBasis` | `checked_clean` | `0` | `0` | Первичное падение на `Equals(object)` исправлено; целевой повторный прогон зелёный. |
| `HyperPlane` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `Polyline` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `BasicPolygon` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `PolygonTools` | `has_failures` | `0` | `2` | Вырожденный эллипс возвращает не ожидаемый повёрнутый отрезок. |
| `GammaPair` | `checked_clean` | `0` | `0` | Собственные прямые тесты проходят; проблемы всплывают в сценариях `SupportFunction`. |
| `SupportFunction` | `has_failures` | `0` | `2` | Инициализация через пары с нулевыми нормалями утыкается в `Debug.Assert` внутри `GammaPair`. |
| `ConvexPolygon` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `FaceLattice` | `has_failures` | `0` | `1` | Явная `AffineBasis` для узла конфликтует с текущим конструктором `AffineBasis`. |
| `ConvexPolytop` | `has_failures` | `0` | `4` | Общий сбой в `HrepToVrep_Geometric`. |
| `HrepToFLrep` | `has_failures` | `0` | `1` | Текущий наблюдаемый контракт уже не совпадает с ожиданием теста. |

## AffineBasis

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Активных падений после локальной проверки не обнаружено.
- Contract ambiguities:
  - Неясности нет: для `Equals(object)` ожидается безопасный `false`, а не исключение.
- Notes:
  - На первичном полном прогоне падал [`../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs#L52`](../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs#L52) `Equals_NullOrDifferentType`.
  - Исправление: добавлен typed `Equals(AffineBasis?)`, а `Equals(object?)` сведён к безопасной проверке типа.

## BasicPolygon

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## CauchyMatrix

- Status:
  - `has_failures`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - [`../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L25`](../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L25) `DiagonalMatrix_MatchesExactExponentialForPositiveAndNegativeInstants`
    - Наблюдаемое поведение: диагональный случай расходится с точным `exp` на `1.94e-08` при tolerance `1e-08`.
- Contract ambiguities:
  - Нужно решить, считать ли текущую точность RK4 при `dt = 0.01` достаточной для этого контракта, или тест должен давать больший допуск.
- Notes:
  - Первичный `NullReferenceException` не был вырожденным случаем: он воспроизводился на первом же запросе `t > T` для свежего `CauchyMatrix`, когда в кэше была только опорная точка `(T, I)`.
  - Причина была в выборе стартового узла интегрирования через `AVLDictionary`.
  - После локальной правки сняты `ZeroMatrix_ProducesIdentityForAnyInstant` и `NonMultipleInstant_UsesPartialRungeKuttaStepAndMatchesClosedFormForNilpotentMatrix`.
  - Для `NonMultipleInstant...` дополнительно выявился и был исправлен неверный знак остаточного шага в forward-ветке.
  - Отдельный backward-partial сценарий был добавлен и помог выявить симметричную проблему со знаком остаточного шага для `t < T`; после исправления он проходит.

## ConvexPolygon

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## ConvexPolytop

- Status:
  - `has_failures`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - [`../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs#L71`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs#L71) `GetInRepresentations_ConstructEquivalentPolytopesWithRequestedPriority`
    - Наблюдаемое поведение: `ArgumentOutOfRangeException`.
    - Стек указывает на `ConvexPolytop.HrepToVrep_Geometric`.
  - [`../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L21`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs#L21) `Cube01Factories_ProduceEquivalentCubes`
    - Наблюдаемое поведение: `ArgumentOutOfRangeException`.
    - Сбой происходит при сравнении через ленивое достроение представлений.
  - [`../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L52`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L52) `SectionByHyperPlane_ForUnitSquare_ReturnsVerticalMidSegment`
    - Наблюдаемое поведение: `ArgumentOutOfRangeException`.
    - Стек снова указывает на `ConvexPolytop.HrepToVrep_Geometric`.
  - [`../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L109`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L109) `WhichRepToString_EqualsAndGetHashCode_FollowCurrentRepresentationContracts`
    - Наблюдаемое поведение: `ArgumentOutOfRangeException`.
    - Сбой проявляется через `Equals`, который лениво требует `FLrep`.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Это ещё один кластерный дефект: несколько разных публичных сценариев падают в одном и том же преобразовании `Hrep -> Vrep`.

## FaceLattice

- Status:
  - `has_failures`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - [`../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L83`](../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L83) `Constructor_FromSubNodes_WithExplicitAffBasis`
    - Наблюдаемое поведение: `ArgumentException` с текстом `Found LinearBasisMutable in AffineBasis constructor!`.
    - Стек проходит через `FLNode..ctor(IEnumerable<...> sub, AffineBasis affBasis)`.
- Contract ambiguities:
  - Нужно уточнить, допускает ли этот конструктор произвольную переданную `AffineBasis`, если внутри неё сидит mutable-база.
- Notes:
  - Похожая проблема затрагивает и `HrepToFLrep`, то есть это не только локальная проблема узла.

## GammaPair

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Стоит помнить, что создание пары с нулевой нормалью в debug-режиме упирается в `Debug.Assert`; это всплывает в соседних тестах `SupportFunction`.
- Notes:
  - Собственный unit-like набор проходит.

## HrepToFLrep

- Status:
  - `has_failures`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - [`../../Tests/DoubleGeometry/Polyhedra/HrepToFLrep/HrepToFLrepCurrentContractTests.cs#L10`](../../Tests/DoubleGeometry/Polyhedra/HrepToFLrep/HrepToFLrepCurrentContractTests.cs#L10) `HrepToFLrepGeometric_ForBoundedUnitSquare_CurrentlyThrowsNotImplementedException`
    - Наблюдаемое поведение: вместо ожидаемого `NotImplementedException` приходит `ArgumentException` с текстом `Found LinearBasisMutable in AffineBasis constructor!`.
    - Стек указывает на `FLNode..ctor(..., AffineBasis)` внутри `HrepToFLrep_Geometric`.
- Contract ambiguities:
  - Здесь уже есть сдвиг текущего наблюдаемого контракта: тест описывает старое поведение, код до него больше не доходит.
- Notes:
  - Причина выглядит зависимой от `FaceLattice` / `AffineBasis`, а не изолированной внутри самого `HrepToFLrep`.

## HyperPlane

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## Intersection

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## Line2D

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## LinearBasis

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Уже известный `NotImplementedException` в публичном `Orthonormalize(Vector)` остаётся частью текущего явно зафиксированного контракта и не является новым падением этой ревизии.
- Notes:
  - Быстрый фундаментальный набор проходит.

## Matrix

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены в собственном наборе.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - В этой ревизии `Matrix` фигурирует как upstream-зависимость падений `CauchyMatrix`, но не как самостоятельный упавший набор.

## PolygonTools

- Status:
  - `has_failures`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - [`../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs#L65`](../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs#L65) `Ellipse_ZeroMinorSemiaxis_ReturnsSegmentAlongRotatedMajorAxis`
    - Наблюдаемое поведение: вместо ожидаемых повёрнутых концов отрезка контур содержит две одинаковые вершины `<(1;-1)>`.
  - [`../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs#L84`](../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs#L84) `Ellipse_ZeroMajorSemiaxis_ReturnsSegmentAlongRotatedMinorAxis`
    - Наблюдаемое поведение: вместо ожидаемых повёрнутых концов отрезка контур содержит две одинаковые вершины `<(1;-1)>`.
- Contract ambiguities:
  - Надо отдельно решить, считается ли вырожденный эллипс официально поддержанным сценарием или это лишь полезное расширение текущего API.
- Notes:
  - Оба падения согласованны между собой и указывают на один и тот же вид деградации.

## Polyline

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## Segment

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## SegmentPair

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## SupportFunction

- Status:
  - `has_failures`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - [`../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs#L40`](../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs#L40) `Constructor_GammaPairs_DropsZeroNormalsAndKeepsNonZeroPairs`
    - Наблюдаемое поведение: вместо фильтрации нулевой нормали тест ловит `DebugAssertException` из конструктора `GammaPair`.
  - [`../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs#L59`](../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs#L59) `Constructor_GammaPairs_ThrowsWhenAllNormalsAreZero`
    - Наблюдаемое поведение: вместо ожидаемого `ArgumentException` от `SupportFunction` тест ловит `DebugAssertException` из конструктора `GammaPair`.
- Contract ambiguities:
  - Нужно решить, имеет ли смысл вообще тестировать и документировать сценарии с нулевыми нормалями на уровне `SupportFunction`, если более низкий контракт `GammaPair` считает их нарушением preconditions.
- Notes:
  - Это хороший кандидат на пересмотр самого сценария тестирования, а не только на поиск ошибки реализации.

## Tools

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Активных падений после локальной проверки не обнаружено.
- Contract ambiguities:
  - Для `EQ`/`NE` граница уже фактически зафиксирована кодом как строгая: `EQ` использует `|a| < Eps`, а `NE` является отрицанием `EQ`.
- Notes:
  - На первичном полном прогоне падали [`../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs#L22`](../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs#L22) и [`../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs#L32`](../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs#L32).
  - Исправление было только в тестах: ожидание для `EpsG` переведено с точного битового равенства на формульную/численно устойчивую проверку, а boundary-case для `EQ/NE` переписан без артефакта `1.0 + 1e-6`.

## Vector

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## Vector2D

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены.
- Contract ambiguities:
  - Не отмечены.
- Notes:
  - Быстрый фундаментальный набор проходит.

## Suggestions For Ongoing Use

- `Contract ambiguities`
  - отдельный блок нужен, когда тест падает не из-за очевидной поломки, а из-за незафиксированного контракта;
- `Numerical sensitivity`
  - стоит отмечать сценарии, завязанные на `Tools.Eps`, порядок обхода, нормализацию и точность double-сравнений;
- `Legacy mismatch`
  - полезно фиксировать случаи, где новый набор ожидает уже не то, что исторически ожидал legacy;
- `Upstream dependency`
  - полезно явно отмечать, когда падение в классе на самом деле вызвано нижележащей зависимостью, чтобы не чинить один и тот же дефект в пяти местах.
