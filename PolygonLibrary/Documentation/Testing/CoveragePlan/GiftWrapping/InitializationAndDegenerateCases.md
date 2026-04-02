# Initialization And Degenerate Cases

## Scope

Сценарии:

- пустой swarm;
- одинокая точка;
- аффинно вырожденный линейный случай в большем пространстве.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| GW-INIT-001 | x | Конструктор на пустом swarm бросает `ArgumentException` с текущим текстом сообщения. | [`GiftWrappingInitializationTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingInitializationTests.cs#L12) |
| GW-INIT-002 | x | Одинокая точка даёт тот же `Vrep` и одноузловую `FaceLattice`. | [`GiftWrappingInitializationTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingInitializationTests.cs#L20) |
| GW-INIT-003 | x | Линейный swarm в 2D понижается до отрезка с двумя вершинами и одной гранью. | [`GiftWrappingInitializationTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingInitializationTests.cs#L34) |

## Existing Tests

- В legacy такие граничные ветки явно почти не были отделены; новый набор фиксирует их напрямую.

## Gaps

- Более сложные аффинно вырожденные случаи в высоких размерностях пока не входят в активный слой.


