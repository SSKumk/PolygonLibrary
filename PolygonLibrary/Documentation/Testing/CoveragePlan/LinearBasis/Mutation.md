# LinearBasis Mutation

## Scope

Этот файл покрывает изменяемый слой `LinearBasisMutable` и добавление векторов в базис.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `LB-MUT-001` | `x` | `AddVector` добавляет первый ненулевой вектор в пустой базис. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) |
| `LB-MUT-002` | `x` | `AddVector` добавляет независимый вектор и увеличивает размерность подпространства. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) |
| `LB-MUT-003` | `x` | `AddVector` корректно ортогонализует независимый, но неортогональный вектор. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) |
| `LB-MUT-004` | `x` | `AddVector` отвергает зависимый вектор и не меняет размерность. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) |
| `LB-MUT-005` | `x` | `AddVector` игнорирует нулевой вектор. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) |
| `LB-MUT-006` | `x` | `AddVector` игнорирует новые векторы для полного базиса. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) |
| `LB-MUT-007` | `x` | `AddVectors` добавляет несколько векторов и останавливается на полном базисе. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) |
| `LB-MUT-008` | `x` | Детерминированный сценарий `AddVector` с фиксированным генератором сохраняет ожидаемую геометрию результата. | [`LinearBasisMutationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) |

## Gaps

- Нет.

## Notes

- Здесь тестируется именно мутация объекта, а не только итоговая одинаковость подпространств.
