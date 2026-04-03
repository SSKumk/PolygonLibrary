# Instance And Factories

## Scope

Сценарии на instance API и factory-методы `GaussSLE`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| GSS-INS-001 | x | `SetSystem` позволяет переиспользовать один instance для новой системы. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L142) |
| GSS-INS-002 | x | `SetGaussChoice` влияет на последующий `Solve` без пересоздания instance. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L142) |
| GSS-INS-003 | x | `GetSolution(out Vector)` возвращает тот же unique solution, что и массивная версия. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L142) |
| GSS-FAC-001 | x | Factory по функциям корректно решает систему. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L176) |
| GSS-FAC-002 | x | Factory по `HyperPlane` корректно находит точку пересечения. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L191) |
| GSS-FAC-003 | x | Factory по `HyperPlane` корректно обрабатывает пустой список. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L206) |
| GSS-FAC-004 | x | Factory по массивам бросает `ArgumentException`, если число строк `A` не совпадает с длиной `b`. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L214) |
| GSS-FAC-005 | x | Factory по массивам не мутирует входные `A` и `b`. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L224) |

## Existing Tests

- Перенесены legacy-сценарии на factory по функциям и `HyperPlane`.
- Дополнительно закреплены reuse instance API и немутирующий контракт array-factory.

## Gaps

- Private `SetSystem(TNum[,], TNum[])` покрывается только косвенно через публичный array-factory и конструктор.
