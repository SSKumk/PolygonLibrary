# LinearBasis Generation

## Scope

Этот файл покрывает фабрики `GenLinearBasis` и их базовые инварианты.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `LB-GEN-001` | `x` | `GenLinearBasis(spaceDim)` создаёт полный ортонормированный базис нужной размерности. | [`LinearBasisGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisGenerationTests.cs) |
| `LB-GEN-002` | `x` | `GenLinearBasis(spaceDim, subSpaceDim)` создаёт частичный ортонормированный базис нужной размерности. | [`LinearBasisGenerationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisGenerationTests.cs) |
| `LB-GEN-003` | `x` | Генераторы с фиксированным `GRandomLC` дают детерминированные сценарии, используемые в проекциях, дополнениях и добавлении векторов. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs), [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |

## Gaps

- Нет.

## Notes

- Для генераторов важнее инварианты и детерминированность сценариев, чем конкретный способ построения базиса.
