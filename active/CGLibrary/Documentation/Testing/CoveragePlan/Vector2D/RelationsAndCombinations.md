# Vector2D Relations And Combinations

## Scope

Этот файл покрывает отношения между 2D-векторами, `IsBetween`, линейные комбинации и явное приведение из общего `Vector`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `V2D-REL-001` | `x` | `AreParallel` возвращает `true` для сонаправленных и противонаправленных векторов. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-002` | `x` | `AreCodirected` возвращает `true` только для сонаправленных векторов. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-003` | `x` | `AreCounterdirected` возвращает `true` только для противонаправленных векторов. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-004` | `x` | `AreOrthogonal` возвращает `true` для ортогональных ненулевых векторов. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-005` | `x` | `AreOrthogonal` считает нулевой вектор ортогональным любому согласно текущему контракту. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-006` | `x` | `IsBetween(v1, v2)` возвращает `true` для вектора строго внутри конуса от `v1` к `v2`. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-007` | `x` | `IsBetween(v1, v2)` возвращает `false` для вектора на границе конуса. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-008` | `x` | `IsBetween(v1, v2)` возвращает `false` для вектора вне конуса. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-009` | `x` | Явное приведение из двумерного `Vector` переносит обе координаты без искажений. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-010` | `x` | `LinearCombination(p1, w1, p2, w2)` строит корректную комбинацию двух точек. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-011` | `x` | `LinearCombination(p1, w1, p2, w2, p3, w3)` строит корректную комбинацию трёх точек. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |
| `V2D-REL-012` | `x` | `LinearCombination(IEnumerable<Vector2D>, IEnumerable<TNum>)` суммирует пары точка-вес в порядке перечисления. | [`Vector2DRelationsAndCombinationsTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) |

## Gaps

- Обязательных gap-ов для текущего публичного контракта не осталось.

## Notes

- Исторический `IsBetweenTest` из legacy-файла сохранён с прежними группирующими комментариями и поясняющими сообщениями.
