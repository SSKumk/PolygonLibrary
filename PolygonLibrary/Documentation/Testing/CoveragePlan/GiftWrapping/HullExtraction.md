# Hull Extraction

## Scope

Сценарии:

- `WrapVRep` на 2D и 3D swarm;
- симплициальная быстрая ветка;
- отбрасывание внутренних точек.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| GW-VREP-001 | x | `WrapVRep` для квадрата с внутренними точками возвращает только вершины оболочки и сохраняет legacy-сообщение "The set of vertices must be equal.". | [`GiftWrappingHullExtractionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingHullExtractionTests.cs#L12) |
| GW-VREP-002 | x | `WrapVRep` для 3D-симплекса идёт по специальной ветке `S.Count == spaceDim + 1` и возвращает те же вершины. | [`GiftWrappingHullExtractionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingHullExtractionTests.cs#L27) |
| GW-VREP-003 | x | `WrapVRep` для 3D-куба с внутренними точками возвращает только вершины куба и сохраняет legacy-сообщение "The set of vertices must be equal.". | [`GiftWrappingHullExtractionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingHullExtractionTests.cs#L34) |

## Existing Tests

- Из `GW_Tests` сохранён ключевой диагностический текст про совпадение множества вершин.
- Рандомизированные перестановки и очень большие наборы точек намеренно не тащатся в активный слой.

## Gaps

- Тяжёлые shuffled/randomized stress-сценарии оставлены вне активного слоя этого этапа.


