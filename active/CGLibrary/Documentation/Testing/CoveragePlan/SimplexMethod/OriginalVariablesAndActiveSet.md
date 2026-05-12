# Original Variables And Active Set

## Scope

Сценарии на восстановление исходных переменных и активный набор ограничений.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| SMP-ACT-001 | x | `Solution` возвращается в исходном пространстве размерности `d`, а не в augment-форме. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L9) |
| SMP-ACT-002 | x | `ActiveInequalitiesID` соответствует всем ограничениям, активным в оптимальной вершине bounded-задачи. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L9) |
| SMP-ACT-003 | x | Конструктор/solver по `HyperPlane` и objective-функции корректно работают без ручной augment-подготовки. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L9) |
| SMP-ACT-004 | x | На вырожденной вершине `BasisInequalitiesID` является подмножеством `ActiveInequalitiesID`. | [`SimplexMethodTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/SimplexMethod/SimplexMethodTests.cs#L87) |

## Existing Tests

- На bounded `2D`-задаче закреплены и optimum, и полный активный набор ограничений.
- На вырожденной вершине отдельно закреплено различие между базисным набором и полным активным набором.
- Проверяется, что `Solution` имеет размерность исходного пространства, а не внутренней переменной раскладки.

## Gaps

- Детальный состав активного набора на дегенеративных вершинах и tie-breaking между несколькими равноправными optimum-точками не закрепляется.
