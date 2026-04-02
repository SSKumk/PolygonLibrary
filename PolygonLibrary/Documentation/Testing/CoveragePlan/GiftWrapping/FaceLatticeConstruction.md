# Face Lattice Construction

## Scope

Сценарии:

- `WrapFaceLattice`;
- `ConstructFL` на уже созданном `GiftWrapping`;
- согласованность числа граней и множества вершин.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| GW-FL-001 | x | `WrapFaceLattice` для квадрата с внутренними точками строит 2D-решётку с числами граней `4/4/1`. | [`GiftWrappingFaceLatticeTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingFaceLatticeTests.cs#L12) |
| GW-FL-002 | x | `ConstructFL` для 3D-куба даёт ожидаемое число `0/1/2/3`-граней и правильное множество вершин. | [`GiftWrappingFaceLatticeTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingFaceLatticeTests.cs#L24) |

## Existing Tests

- В legacy эти инварианты проверялись в основном через `ConvexPolytop.CreateFromPoints(..., true)`, а не через сам `GiftWrapping`.
- В новой структуре они закреплены прямыми тестами на алгоритм.

## Gaps

- Большие многомерные regression-сценарии по числам граней пока не входят в активный слой.


