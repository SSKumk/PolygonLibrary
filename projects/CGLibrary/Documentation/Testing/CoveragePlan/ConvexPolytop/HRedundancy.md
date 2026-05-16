# HRedundancy

## Scope

Сценарии для:

- `HRedundancyByGW`;
- `CreateFromHalfSpaces(..., doHRedundancy: true)`.

Здесь фиксируется базовая редукция избыточных полупространств на малых 2D-примерах, где итоговую геометрию легко проверить напрямую.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CPT-HRED-001 | x | `HRedundancyByGW` на центрированном квадрате удаляет заведомо внешние дублирующие ограничения и сохраняет геометрию. | [`ConvexPolytopHRedundancyTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopHRedundancyTests.cs#L9) |
| CPT-HRED-002 | x | `CreateFromHalfSpaces(..., true)` использует ту же редукцию и убирает лишние грани у сдвинутого квадрата. | [`ConvexPolytopHRedundancyTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopHRedundancyTests.cs#L34) |

## Existing Tests

- До этого прямого покрытия на `HRedundancyByGW` не было: поведение редукции проявлялось только косвенно через `Polar` и higher-level построители.
- Текущий слой фиксирует базовый ожидаемый контракт: внешние, менее жёсткие ограничения исчезают, а геометрия политопа сохраняется.

## Gaps

- Пока не покрыты более тонкие случаи с почти совпадающими гиперплоскостями и численно близкими константами.
- Не покрыты более высокие размерности и связь с тяжёлыми алгоритмическими сценариями `GiftWrapping`.
