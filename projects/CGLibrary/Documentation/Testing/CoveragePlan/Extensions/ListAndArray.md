# List And Array

## Covered scenarios

- `EXT-LA-001` `List.BinarySearchByPredicate` находит первый подходящий элемент и корректно зажимает requested range.
- `EXT-LA-002` `List.BinaryCyclicSearchByPredicate` работает на wrap-around сценарии, а `GetAtCyclic` нормализует отрицательные и большие индексы.
- `EXT-LA-003` `List.BinaryCyclicSearchByPredicate` нормализует возвращаемый индекс и в ранней ветке `pred(lower) == true`.
- `EXT-LA-004` `List.GetAtCyclic` бросает исключение на пустом списке.
- `EXT-LA-005` `null`-списки и `null`-массивы возвращают `-1` в search-helper-методах, где это поддерживается текущим кодом.
- `EXT-LA-006` `List.Shuffle` детерминирован при одинаковом `seed` и сохраняет множество элементов.
- `EXT-LA-007` `Array.BinarySearchByPredicate` и `Array.BinaryCyclicSearchByPredicate` повторяют базовый контракт list-веток, включая нормализацию раннего возврата.

## Notes

- В active layer закреплены только корректные сценарии с индексами, для которых предпосылки метода выполняются.
