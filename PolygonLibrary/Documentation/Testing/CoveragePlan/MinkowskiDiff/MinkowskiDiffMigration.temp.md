# MinkowskiDiff Migration

## Purpose

Временный рабочий файл для переноса `MinkowskiDiff` в новую структуру `DoubleGeometry`.

Нужен для того, чтобы:

- не потерять смысл legacy-набора;
- явно развести `basic`, `regression` и возможный `stress`;
- зафиксировать, что уже покрыто, а что ещё требует переноса;
- отдельно отметить сценарии, которые не дают новой ценности и могут быть не перенесены.

## Current New-Layer State

Сейчас в активном новом слое есть:

- [`MinkowskiDiffBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffBasicTests.cs)
- [`MinkowskiDiffRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/MinkowskiDiff/MinkowskiDiffRegressionTests.cs)
- [`Index.md`](Index.md)
- [`BasicCases.md`](BasicCases.md)
- [`RegressionCases.md`](RegressionCases.md)

Уже покрыто:

- helper `FindExtrInCPOnVector_Naive`;
- helper `doSubtract`;
- `Naive` и `Geometric` на representative-сценариях `cube - segment`;
- `Naive` и `Geometric` на representative-сценариях `sphere - segment`.

## Legacy Sources

### Double

- [`MinkowskiDiffTests.cs`](../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiDiffTests.cs)

Содержит:

- `Cube_Seg0_0_z`
- `Sphere_Seg0_0_z`
- закомментированный `Cyclic`

### DoubleDouble

- [`MinkowskiDiffTests.cs`](../../../../Tests/DoubleDouble-Tests/Minkowski-Tests/MinkowskiDiffTests.cs)

Содержит:

- `Cube_Seg0_0_z`
- `Sphere_Seg0_0_z`
- `Cyclic`

## Algorithm Structure

Публичные режимы:

- `MinkowskiDiff.Naive(F, G)`
- `MinkowskiDiff.Geometric(F, G)`

Оба режима используют общий private-путь `MinkDiff(...)`.

Фактическое различие между ними сейчас только в этапе `Hrep -> Vrep`:

- `Naive` использует `ConvexPolytop.HrepToVrep_Naive`
- `Geometric` использует `ConvexPolytop.HrepToVrep_Geometric`

Отсюда основной смысл тестов:

- helper-контракты;
- согласованность `Naive` и `Geometric`;
- непустой / пустой результат на representative-входах;
- один-два нетривиальных геометрических сценария из legacy.

## Target Test Structure

### Basic

Файл:

- `MinkowskiDiffBasicTests.cs`

Сюда входят:

- прямые helper-контракты;
- `cube - segment` как базовый геометрический слой;
- agreement `Naive == Geometric` на простых representative-входах;
- проверка границы, где разность становится пустой.

### Regression

Файл:

- `MinkowskiDiffRegressionTests.cs`

Сюда входят:

- `Sphere_Seg0_0_z`.

### Stress

Сейчас отдельный `Stress` выглядит не обязательным.

Причины:

- legacy-набор очень маленький;
- нет большого combinatorial или generator-based слоя, как в `GiftWrapping` или `MinkowskiSum`;
- `MinkowskiDiff` пока выглядит как тонкий алгоритм поверх уже протестированных building blocks.

`Stress` добавлять только если в ходе переноса всплывут:

- тяжёлые размерности;
- seed-based random cases;
- заметная численная хрупкость.

## Migration Map

| Legacy Scenario | Current Status | Planned Target | Notes |
| --- | --- | --- | --- |
| `Cube_Seg0_0_z` | `x` | `Basic` | Смысл покрыт новым базовым слоем. |
| `Sphere_Seg0_0_z` | `x` | `Regression` | Перенесён как smooth/rounded case и для `Naive == Geometric`. |
| `Cyclic` | `!` | `Regression?` | В `double` вскрывает реальную проблему `Geometric`: `ConvexPolytop.HrepToVrep_Geometric` падает с debug-assert про unbounded inequalities. |

## Open Questions

1. Нужна ли отдельная проверка на представление результата (`WhichRep`)?

Текущий ответ: скорее нет.
Важнее геометрическая согласованность и equality результата, а не лениво материализованное внутреннее представление.

2. Нужен ли отдельный `Stress`?

Текущий ответ: скорее нет, если не всплывёт новый материал.

3. Что делать с `Cyclic`?

Это уже не вопрос миграции, а отдельный алгоритмический баг/ограничение. Решение надо принимать отдельно, прежде чем включать его в зелёный active-layer.

## Next Steps

1. Прогнать новый узкий слой `MinkowskiDiff`.
2. Обсудить, считать ли `Cyclic` обязательным для текущего active-layer или отдельным багом `Geometric`.
3. Если новых meaningful-категорий нет, архивировать legacy `MinkowskiDiffTests`, оставив пометку про `Cyclic`.
4. После архивирования удалить этот временный файл.

## User Notes

- Здесь можно фиксировать решения по численной устойчивости и судьбе `Stress`, если они появятся по ходу разбора.
