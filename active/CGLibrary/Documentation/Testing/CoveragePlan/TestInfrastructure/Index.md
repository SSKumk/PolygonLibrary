# TestInfrastructure

## Scope

Вспомогательный общий слой тестовой инфраструктуры в [`Tests/TestInfrastructure`](../../../../Tests/TestInfrastructure).

В активный слой текущего этапа входит:
- sanity-проверка генераторов swarm и canonical test objects из [`TestsPolytopes.cs`](../../../../Tests/TestInfrastructure/TestsPolytopes.cs).

Вне активного слоя пока остаётся:
- детальная декомпозиция `TestsBase.cs` и `TestsPolytopes.cs`;
- speed/performance-потребители этого helper-слоя.

## Topics

- [`PolytopeGenerators.md`](PolytopeGenerators.md)
  Объект: компактная sanity-проверка генераторов кубов, симплексов, cyclic polytope и сферических наборов.
  Когда использовать: когда нужно быстро убедиться, что общий генераторный слой не вносит ошибку в алгоритмические тесты.
