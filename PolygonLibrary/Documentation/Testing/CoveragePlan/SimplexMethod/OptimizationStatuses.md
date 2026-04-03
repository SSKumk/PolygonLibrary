# Optimization Statuses

## Scope

Сценарии на статусы решения и базовые свойства optimum для `SimplexMethod`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| SMP-STS-001 | x | Bounded задача с уникальным optimum возвращает `Ok`, корректные `Value` и `Solution`. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L9) |
| SMP-STS-002 | x | Несовместная система ограничений возвращает `NoSolution`. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L37) |
| SMP-STS-003 | x | Неограниченная задача возвращает `Unlimited`. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L49) |
| SMP-STS-004 | x | Свободная исходная переменная может восстанавливаться в отрицательное optimum-значение через внутренний split. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L60) |

## Existing Tests

- Закреплены все три публичных статуса результата.
- Отдельно проверено, что решение в исходных координатах может быть отрицательным, то есть split `x+ - x-` действительно работает.

## Gaps

- Дегенеративные bounded задачи с несколькими оптимальными вершинами пока не выделены в отдельный слой: там не хочется закреплять конкретный tie-breaking.
