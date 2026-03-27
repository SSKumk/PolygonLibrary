# Current Contract

## Scope

Сценарии для публичного метода `HrepToFLrep_Geometric`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| H2F-001 | x | Для ограниченного 2D-политопа конвертация пока обрывается `NotImplementedException` с текущим текстом сообщения. | [`HrepToFLrepCurrentContractTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/HrepToFLrep/HrepToFLrepCurrentContractTests.cs#L9) |
| H2F-002 | x | Для `Hrep`, в котором нельзя найти стартовую вершину, метод возвращает `null`. | [`HrepToFLrepCurrentContractTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/HrepToFLrep/HrepToFLrepCurrentContractTests.cs#L18) |

## Existing Tests

- Раньше прямого тестового слоя на `HrepToFLrep` не было вовсе.
- Новый набор фиксирует текущее наблюдаемое поведение, чтобы последующий рефакторинг или доработка алгоритма были осознанными.

## Gaps

- Реальный успешный сценарий конвертации пока отсутствует в коде, поэтому и в обязательном покрытии его нет.

