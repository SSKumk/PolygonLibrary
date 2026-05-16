# Stress

## Scope

Сценарии:

- representative stress-наборы для `ConvexPolytop.CreateFromPoints(..., true)`;
- большие по размерности кубы и симплексы;
- fixed-seed random simplices;
- совместная проверка геометрии результата и его комбинаторики.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| GW-STRESS-001 | x | Representative cubes `3D..6D` с inner points сохраняют `Vrep`, число граней `Hrep` и `FLrep.NumberOfKFaces` после shift/rotate/shuffle. | [`GiftWrappingStressTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingStressTests.cs#L49) |
| GW-STRESS-002 | x | Representative axis-simplices `3D..7D` с inner points сохраняют `Vrep`, число граней `Hrep` и `FLrep.NumberOfKFaces` после shift/rotate/shuffle. | [`GiftWrappingStressTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingStressTests.cs#L64) |
| GW-STRESS-003 | x | Representative fixed-seed random simplices `3D..5D` сохраняют `Vrep`, число граней `Hrep` и `FLrep.NumberOfKFaces` после shift/rotate/shuffle. | [`GiftWrappingStressTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingStressTests.cs#L79) |

## Existing Tests

- Старый `GW_Tests` содержал значительно более тяжёлые массовые прогоны `AllCubes*`, `AllSimplices*`, `AllSimplicesRND*`.
- В новый слой перенесён representative-поднабор, который сохраняет смысл этих проверок без тысяч итераций.

## Gaps

- Полный combinatorial перебор legacy-генераторов пока не переносится как обязательный активный слой.
- Отдельные ad-hoc примеры вроде `SomePolytop_3D` и `SomeParallelogram` пока остаются вне нового stress-слоя.
