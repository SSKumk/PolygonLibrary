# Basic Cases

## Scope

Сценарии:

- пустой swarm;
- одиночная точка;
- аффинно вырожденный линейный случай;
- аффинно вырожденный `2D` случай внутри `3D`;
- аффинно вырожденный `3D` simplex внутри `4D`;
- `WrapVRep` на типовых `2D/3D` наборах;
- `WrapFaceLattice` и `ConstructFL` на типовых `2D/3D` случаях.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| GW-BASIC-001 | x | Конструктор на пустом swarm бросает `ArgumentException` с текущим текстом сообщения. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L12) |
| GW-BASIC-002 | x | Одиночная точка даёт тот же `Vrep` и одноузловую `FaceLattice`. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L20) |
| GW-BASIC-003 | x | Линейный swarm в 2D понижается до отрезка с двумя вершинами и одной гранью. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L34) |
| GW-BASIC-004 | x | `WrapVRep` для квадрата с внутренними точками возвращает только вершины оболочки. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L47) |
| GW-BASIC-005 | x | `WrapVRep` для 3D-симплекса идёт по специальной ветке `S.Count == spaceDim + 1` и возвращает те же вершины. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L62) |
| GW-BASIC-006 | x | `WrapVRep` для 3D-куба с внутренними точками возвращает только вершины куба. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L69) |
| GW-BASIC-007 | x | `WrapVRep` для квадрата в `3D` после affine-reduction возвращает только вершины планарной оболочки. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L76) |
| GW-BASIC-008 | x | `WrapVRep` для тетраэдра в `4D` после affine-reduction идёт по simplex-ветке и возвращает те же вершины. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L88) |
| GW-BASIC-009 | x | `WrapFaceLattice` для квадрата с внутренними точками строит 2D-решётку с числами граней `4/4/1`. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L99) |
| GW-BASIC-010 | x | `WrapFaceLattice` для квадрата в `3D` после affine-reduction строит ту же `2D`-решётку. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L112) |
| GW-BASIC-011 | x | `ConstructFL` для 3D-куба даёт ожидаемое число `0/1/2/3`-граней и правильное множество вершин. | [`GiftWrappingBasicTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingBasicTests.cs#L124) |

## Existing Tests

- Базовый слой теперь собран в одном файле и описывает прямой наблюдаемый контракт `GiftWrapping`.
- Legacy `GW_Tests` остаётся источником для последующего переноса `Regression` и `Stress`.

## Gaps

- Invariance/regression-сценарии из legacy пока не входят в `Basic`.
- Heavy randomized и `4D+` stress-наборы остаются вне базового слоя.
