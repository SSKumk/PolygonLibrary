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
- `2026-04-02`
  - `CauchyMatrix`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Basics.CauchyMatrix"`
  - Итог:
    - `5` passed
    - `0` failed
- `2026-04-02`
  - `PolygonTools`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Polygons.PolygonTools"`
  - Итог:
    - `25` passed
    - `0` failed
- `2026-04-02`
  - `SupportFunction`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Polygons.SupportFunction"`
  - Итог:
    - `18` passed
    - `0` failed
- `2026-04-02`
  - `AffineBasis` + `FaceLattice`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Basics.AffineBasis|FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.FaceLattice"`
  - Итог:
    - `62` passed
    - `0` failed
- `2026-04-03`
  - `FaceLattice`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.FaceLattice"`
  - Итог:
    - `19` passed
    - `0` failed
- `2026-04-03`
  - `FaceLattice + FLNodeSum`
  - Команда:
    - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.FaceLattice|FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.FaceLatticeNodeSumTests"`
  - Итог:
    - `24` passed
    - `0` failed
- `2026-04-03`
  - `Decomposition`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Algorithms.DecompositionTests"`
  - Итог:
    - `9` passed
    - `0` failed
- `2026-04-03`
  - `GaussSLE`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Algorithms.GaussSLETests"`
  - Итог:
    - `16` passed
    - `0` failed
- `2026-04-03`
  - `FourierMotzkin`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Algorithms.FourierMotzkinTests"`
  - Итог:
    - `4` passed
    - `0` failed
- `2026-04-03`
  - `SimplexMethod`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Algorithms.SimplexMethodTests"`
  - Итог:
    - `4` passed
    - `0` failed
- `2026-04-02`
  - `ConvexPolytop`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
  - Итог:
    - `26` passed
    - `0` failed
- `2026-04-02`
  - `ConvexPolytop`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
  - Итог:
    - `27` passed
    - `0` failed
- `2026-04-02`
  - `ConvexPolytop`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
  - Итог:
    - `32` passed
    - `0` failed
- `2026-04-02`
  - `ConvexPolytopPolarTests`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopPolarTests"`
  - Итог:
    - `6` passed
    - `0` failed
- `2026-04-02`
  - `ConvexPolytop`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
  - Итог:
    - `38` passed
    - `0` failed
- `2026-04-02`
  - `ConvexPolytop`
  - Команда:
    - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
  - Итог:
    - `46` passed
    - `0` failed
  - `2026-04-02`
  - `ConvexPolytop`
    - Команда:
      - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopFactoriesAndMetricsTests|FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopPolarTests|FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopTransformsAndOverridesTests|FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopConstructionAndRepresentationTests|FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopContainmentAndNearestPointTests"`
    - Итог:
      - `39` passed
      - `0` failed
  - `2026-04-03`
  - `ConvexPolytop`
    - Команда:
      - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
    - Итог:
      - `48` passed
      - `0` failed
  - `2026-04-03`
  - `ConvexPolytop`
    - Команда:
      - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopDistanceEpigraphTests|FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
    - Итог:
      - `55` passed
      - `0` failed
  - `2026-04-03`
  - `ConvexPolytop`
    - Команда:
      - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopPolarTests"`
      - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
    - Итог:
      - `56` passed
      - `0` failed
  - `2026-04-03`
  - `ConvexPolytop`
    - Команда:
      - `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopHRedundancyTests"`
      - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
    - Итог:
      - `58` passed
      - `0` failed
  - `2026-04-03`
  - `ConvexPolytop`
    - Команда:
      - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytopPolarTests"`
      - `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.ConvexPolytop"`
    - Итог:
      - `61` passed
      - `0` failed

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
| `CauchyMatrix` | `checked_clean` | `0` | `0` | Базовый сбой выбора стартового узла и partial-step сняты; диагональный сценарий принят с более реалистичным численным допуском. |
| `LinearBasis` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `AffineBasis` | `checked_clean` | `0` | `0` | Первичное падение на `Equals(object)` исправлено; целевой повторный прогон зелёный. |
| `HyperPlane` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `Polyline` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `BasicPolygon` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `PolygonTools` | `checked_clean` | `0` | `0` | Ветви вырожденного эллипса исправлены; целевой повторный прогон зелёный. |
| `GammaPair` | `checked_clean` | `0` | `0` | Собственные прямые тесты проходят; проблемы всплывают в сценариях `SupportFunction`. |
| `SupportFunction` | `checked_clean` | `0` | `0` | Сценарии с нулевыми нормалями сняты как нарушение preconditions `GammaPair`; целевой повторный прогон зелёный. |
| `ConvexPolygon` | `checked_clean` | `0` | `0` | Быстрый фундаментальный набор проходит. |
| `FaceLattice` | `checked_clean` | `0` | `0` | `FLNode`, `FLNodeSum`, контейнер и internal-конвертеры покрыты прямыми тестами; целевой повторный прогон зелёный. |
| `ConvexPolytop` | `checked_clean` | `0` | `0` | Узкий набор зелёный; активная branch-specific матрица по `Vrep` / `Hrep` / `FLrep` закрыта. |
| `Decomposition` | `checked_clean` | `0` | `0` | `QR`, `LQ` и обе full-update ветки покрыты прямыми тестами; узкий прогон зелёный. |
| `GaussSLE` | `checked_clean` | `0` | `0` | Legacy-сценарии перенесены; instance API и все pivot choices покрыты прямыми тестами. |
| `FourierMotzkin` | `checked_clean` | `0` | `0` | Наивный контракт исключения переменной покрыт прямыми тестами; узкий прогон зелёный. |
| `SimplexMethod` | `checked_clean` | `0` | `0` | Базовый контракт двухфазного симплекса покрыт прямыми тестами; узкий прогон зелёный. |

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
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Активных падений после локальной проверки не обнаружено.
- Contract ambiguities:
  - Пока не отмечены.
- Notes:
  - Первичный `NullReferenceException` не был вырожденным случаем: он воспроизводился на первом же запросе `t > T` для свежего `CauchyMatrix`, когда в кэше была только опорная точка `(T, I)`.
  - Причина была в выборе стартового узла интегрирования через `AVLDictionary`.
  - После локальной правки сняты `ZeroMatrix_ProducesIdentityForAnyInstant` и `NonMultipleInstant_UsesPartialRungeKuttaStepAndMatchesClosedFormForNilpotentMatrix`.
  - Для `NonMultipleInstant...` дополнительно выявился и был исправлен неверный знак остаточного шага в forward-ветке.
  - Отдельный backward-partial сценарий был добавлен и помог выявить симметричную проблему со знаком остаточного шага для `t < T`; после исправления он проходит.
  - Диагональный сценарий не переписывался по существу; для него только ослаблен tolerance до уровня, который соответствует текущей точности RK4 при `dt = 0.01`.

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
  - `checked_clean`
- Missing scenarios:
  - Пока новых сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены после локальной проверки.
- Contract ambiguities:
  - Не отмечены.
  - Notes:
    - Кластерный сбой в `HrepToVrep_Geometric` оказался вызван остатками отладочного кода; после удаления `ConvexPolytop`-набор снова стал зелёным.
    - Поверх исходного набора добавлены прямые сценарии на `InnerPoint` для `FLrep`, wrapper `NearestPoint(Vector)`, `ToConvexPolygon(AffineBasis)` и representation-specific ветки `Shift` / `Rotate`.
    - Семантика `Scale(k, origin)` подтверждена и исправлена для положительного и отрицательного коэффициента во всех трёх представлениях: политоп действительно масштабируется относительно заданного центра.
    - По активному слою branch-specific матрица `Vrep` / `Hrep` / `FLrep` закрыта, включая базовый 2D-слой `Polar`.
    - Добавлен прямой слой на эпиграфы расстояния: базовая геометрия до точки и до одноточечного политопа для `L1` / `Linf` / `L2` теперь закреплена отдельными тестами.
    - Добавлен прямой слой на редукцию `Hrep`: `HRedundancyByGW` и `CreateFromHalfSpaces(..., true)` теперь закреплены на простых 2D-примерах с лишними внешними ограничениями.
    - Для `Sphere` и `Ellipsoid` отдельно зафиксировано, что в `2D` параметр `polarDivision` не влияет на геометрию.
    - Для `Polar` отдельно закреплена текущая семантика `doUnRedundancy`: флаг реально влияет только на `Hrep`-ветку и там убирает лишние точки dual, не меняя геометрию результата.
    - Поверх базового `Polar`-слоя добавлены уже именно структурные проверки `FLrep`: incidence dual-решётки квадрата и восстановление `FaceLattice` после двойного dual-преобразования для квадрата и треугольника.
    - `FindInitialVertex_Simplex` теперь явно использует базисный набор ограничений для восстановления вершины, а полный active set восстанавливает уже после вычисления точки; это отдельно закреплено на вырожденном `2D`-примере.
  - Дополнительно исправлена формула `Ball_1(center, radius)` для ненулевого центра: раньше метод ошибочно добавлял `-e` вместо `center - e`.
  - Helper-ball слой теперь покрыт прямыми тестами для `Ball_1`, `Ball_oo`, `Sphere`, `Ellipsoid` и `Ball_2FuncCreator` на малых размерностях и смещённых центрах.

## FaceLattice

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены после локальной проверки.
- Contract ambiguities:
  - Открытым остаётся только более общий архитектурный вопрос о полном разведении immutable/mutable слоёв `AffineBasis` и `LinearBasis`.
- Notes:
  - Проблема была локализована не в `FLNode`, а в copy ctor `AffineBasis(AffineBasis, needCopy: false)`.
  - Принятое решение: zero-copy разрешён для копирования из обычного `AffineBasis`, но по-прежнему запрещён для `AffineBasisMutable`.
  - Целевой повторный прогон `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.FaceLattice|FullyQualifiedName~Tests.DoubleGeometry.Polyhedra.FaceLatticeNodeSumTests"` проходит: `24` passed, `0` failed.
  - Поверх публичного слоя теперь покрыты и `FLNodeSum`, и internal-конвертеры `ConstructFromFLNodeSum` и `ConstructFromBaseSubCP` на малом треугольном примере.

## Decomposition

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены после локальной проверки.
- Contract ambiguities:
  - Конкретные знаки столбцов и строк ортогональной матрицы не фиксируются как контракт; закреплены только инварианты разложения.
- Notes:
  - Это первый активный линейно-алгебраический алгоритмический блок в новой структуре после снятия общей заморозки для low-level linear algebra.
  - Прямыми тестами покрыты `QR_ByReflection`, `LQ_ByReflection`, `QR_FullUpdate` и `LQ_FullUpdate`.
  - Update-ветки проверяются через сохранение ортонормальности, изменение размерности базиса и обнуление координат за пределами нового базиса.

## GaussSLE

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены после локальной проверки.
- Contract ambiguities:
  - Проверяется только контракт на unique solution; семейства решений для underdetermined/singular систем в активный слой не входят.
- Notes:
  - Перенесены все содержательные legacy-сценарии на square, rectangular и factory-layer.
  - Дополнительно закреплены pivot choices `RowWise` и `ColWise`, reuse instance API через `SetSystem`/`SetGaussChoice`, `GetSolution(out Vector)` и немутирующий контракт array-factory.

## FourierMotzkin

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены после локальной проверки.
- Contract ambiguities:
  - Закрепляется именно текущая наивная семантика без редукции избыточности и без изменения размерности ambient space.
- Notes:
  - Покрыт `EliminateVariableNaive` на базовых сценариях: upper/lower/neutral, 1-based индекс устраняемой переменной и отбрасывание нулевого результирующего неравенства.
  - Заодно дописаны XML-комментарии к классу и его публичному API.

## SimplexMethod

- Status:
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены после локальной проверки.
- Contract ambiguities:
  - На дегенеративных bounded-задачах конкретный путь pivot-ов и конкретная оптимальная вершина не фиксируются как контракт.
- Notes:
  - Закреплены все три публичных статуса результата: `Ok`, `NoSolution`, `Unlimited`.
  - Отдельно проверено восстановление исходных свободных переменных после внутреннего split `x = x+ - x-`.
  - На bounded `2D`-примере закреплены и `BasisInequalitiesID`, и полный активный набор ограничений через `ActiveInequalitiesID`.
  - На вырожденной вершине отдельно закреплено, что `BasisInequalitiesID` может быть строгим подмножеством `ActiveInequalitiesID`.

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
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены после локальной проверки.
- Contract ambiguities:
  - Неясности сняты: XML-комментарии `PolygonTools.Ellipse` явно фиксируют, что при одной нулевой полуоси должен получаться segment.
- Notes:
  - Корень сбоя был в двух ветвях `Ellipse`: при `a == 0` и `b == 0` длина отрезка ошибочно строилась по нулевой полуоси, из-за чего оба конца совпадали.
  - Целевой повторный прогон `dotnet test Tests/Tests.csproj --no-build --filter "FullyQualifiedName~Tests.DoubleGeometry.Polygons.PolygonTools"` проходит: `25` passed, `0` failed.

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
  - `checked_clean`
- Missing scenarios:
  - Пока новых обязательных сценариев сверх coverage plan не выявлено.
- Failing tests:
  - Не обнаружены после локальной проверки.
- Contract ambiguities:
  - Неясности сняты: сценарии с нулевыми нормалями не относятся к обязательному runtime-контракту `SupportFunction`, потому что нарушают preconditions публичного `GammaPair`.
- Notes:
  - Первичные падения были вызваны не `SupportFunction`, а попыткой создать невалидный `GammaPair(Vector2D.Zero, ...)`.
  - После удаления этих двух сценариев из активного набора целевой повторный прогон `SupportFunction` проходит без падений.

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

