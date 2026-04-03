# List And Array

## Covered scenarios

- `EXT-LA-001` `List.BinarySearchByPredicate` находит первый подходящий элемент и корректно зажимает requested range.
- `EXT-LA-002` `List.BinaryCyclicSearchByPredicate` работает на wrap-around сценарии, а `GetAtCyclic` нормализует отрицательные и большие индексы.
- `EXT-LA-003` `List.GetAtCyclic` бросает исключение на пустом списке.
- `EXT-LA-004` `List.Shuffle` детерминирован при одинаковом `seed` и сохраняет множество элементов.
- `EXT-LA-005` `Array.BinarySearchByPredicate` и `Array.BinaryCyclicSearchByPredicate` повторяют базовый контракт list-веток.

## Notes

- В active layer закреплены только корректные сценарии с индексами, для которых предпосылки метода выполняются.
- Контракт на текущую нормализацию результата `BinaryCyclicSearchByPredicate` при `pred(lower) == true` требует отдельного обсуждения.
