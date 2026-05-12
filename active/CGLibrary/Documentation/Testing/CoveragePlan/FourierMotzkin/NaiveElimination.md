# Naive Elimination

## Scope

Сценарии на `EliminateVariableNaive`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| FMT-NAI-001 | x | При одном upper и одном lower bound метод строит корректное комбинированное неравенство после исключения переменной. | [`FourierMotzkinTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/FourierMotzkin/FourierMotzkinTests.cs#L9) |
| FMT-NAI-002 | x | Neutral-неравенства переносятся в результат без модификации. | [`FourierMotzkinTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/FourierMotzkin/FourierMotzkinTests.cs#L9) |
| FMT-NAI-003 | x | `variableNum` трактуется как 1-based индекс устраняемой переменной. | [`FourierMotzkinTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/FourierMotzkin/FourierMotzkinTests.cs#L30) |
| FMT-NAI-004 | x | Если upper или lower bounds отсутствуют, в результате остаются только neutral-неравенства. | [`FourierMotzkinTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/FourierMotzkin/FourierMotzkinTests.cs#L44) |
| FMT-NAI-005 | x | Нулевое комбинированное неравенство не добавляется в результат. | [`FourierMotzkinTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/FourierMotzkin/FourierMotzkinTests.cs#L58) |

## Existing Tests

- Закреплён именно текущий наивный контракт без редукции избыточности.
- Проверки сделаны на малых системах, где ожидаемое неравенство легко выписать вручную.

## Gaps

- Пустой набор `HPs` и некорректный номер переменной не фиксируются как release-контракт: текущая реализация исходит из корректного входа.
