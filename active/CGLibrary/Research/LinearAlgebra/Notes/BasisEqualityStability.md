# Basis Equality Stability

Короткая заметка про численную устойчивость `Equals` у `LinearBasis` и `AffineBasis`.

Связанный код:
- [`../../../CGLibrary/Basics/LinearBasis.cs`](../../../CGLibrary/Basics/LinearBasis.cs)
- [`../../../CGLibrary/Basics/AffineBasis.cs`](../../../CGLibrary/Basics/AffineBasis.cs)

## Что сравнивалось

Сравнивались две схемы equality.

Current:
- `LinearBasis.Equals(obj)` через `CompareTo(other) == 0`;
- `AffineBasis.Equals(other)` через `CompareTo(other) == 0`.

Alternative:
- для `LinearBasis` геометрическое равенство через взаимное `SpanSameSpace`;
- для `AffineBasis` геометрическое равенство через equality линейной части и взаимное `Contains` для origin.

## Репрезентативные 3D-сценарии

Оставлены только три класса кейсов для плоскостей в `R^3`.

1. Хорошо обусловленный equal-case.
2. Borderline equal-case, где current equality уже даёт `false`, а geometric equality ещё может дать `true`.
3. Near-`Tools.Eps` different-case, где две действительно разные плоскости уже начинают схлопываться в equality.

Отдельно повторён affine-вариант тех же сценариев.

## Наблюдение

- Geometric equality действительно иногда выигрывает у current equality по ложным `false` на равных объектах.
- Но выигрыш узкий: в исследовании это проявилось только в отдельных ориентациях и только в узкой полосе параметров около `Tools.Eps`.
- При near-`Tools.Eps` different-case geometric equality не показывает лучшего поведения по ложным `true`.
- Для `AffineBasis` картина почти та же, потому что alternative equality всё равно опирается на ту же global epsilon-policy через `SpanSameSpace` / `Contains`.

Локальный timing на representative-cases тоже не дал аргумента в пользу замены:
- на equal-cases geometric equality обычно медленнее current equality;
- на different-cases может быть чуть быстрее за счёт раннего выхода, но это не меняет общей картины.

## Вывод

Current `Equals` оставлен без изменений.

Причина:
- geometric equality даёт только локальный выигрыш по ложным `false`;
- принципиально не решает проблему near-`Tools.Eps` ложных `true`;
- обычно стоит дороже по времени.

То есть отделение `Equals` от `CompareTo` само по себе концептуально возможно, но текущие минимальные геометрические альтернативы не дают достаточно сильного практического выигрыша, чтобы оправдать замену.

## Воспроизводимость

Минимальный воспроизводимый набор сохранён в research-слое:

- [`../../../Tests/DoubleGeometry/Research/LinearBasis/README.md`](../../../Tests/DoubleGeometry/Research/LinearBasis/README.md)
- [`../../../Tests/DoubleGeometry/Research/LinearBasis/EqualsStabilityResearchTests.cs`](../../../Tests/DoubleGeometry/Research/LinearBasis/EqualsStabilityResearchTests.cs)
