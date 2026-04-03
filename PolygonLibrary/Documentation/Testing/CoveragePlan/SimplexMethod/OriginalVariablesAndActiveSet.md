# Original Variables And Active Set

## Scope

Сценарии на восстановление исходных переменных и активный набор ограничений.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| SMP-ACT-001 | x | `Solution` возвращается в исходном пространстве размерности `d`, а не в augment-форме. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L9) |
| SMP-ACT-002 | x | `ActiveInequalitiesID` соответствует ограничениям, активным в оптимальной вершине bounded-задачи. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L9) |
| SMP-ACT-003 | x | Конструктор/solver по `HyperPlane` и objective-функции корректно работают без ручной augment-подготовки. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L9) |

## Existing Tests

- На bounded `2D`-задаче закреплены и optimum, и активные ограничения.
- Проверяется, что `Solution` имеет размерность исходного пространства, а не внутренней переменной раскладки.

## Gaps

- Детальный состав активного набора на дегенеративных вершинах не закрепляется.
