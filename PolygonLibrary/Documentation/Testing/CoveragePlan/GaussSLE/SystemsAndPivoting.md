# Systems And Pivoting

## Scope

Сценарии на типы систем и выбор пивота в `GaussSLE`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| GSS-SYS-001 | x | Простая square `2x2` система с единственным решением корректно решается. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L11) |
| GSS-SYS-002 | x | Square `3x3` система с единственным решением корректно решается. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L25) |
| GSS-SYS-003 | x | Singular square система не даёт unique solution. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L38) |
| GSS-SYS-004 | x | Consistent overdetermined система корректно решается. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L50) |
| GSS-SYS-005 | x | Inconsistent overdetermined система корректно отвергается. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L64) |
| GSS-SYS-006 | x | Underdetermined система не даёт unique solution. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L78) |
| GSS-PIV-001 | x | `GaussChoice.No` падает на нулевом диагональном элементе без перестановок. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L90) |
| GSS-PIV-002 | x | `GaussChoice.All` справляется с нулевым диагональным элементом. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L103) |
| GSS-PIV-003 | x | `GaussChoice.RowWise` корректно выбирает пивот перестановкой строк. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L116) |
| GSS-PIV-004 | x | `GaussChoice.ColWise` корректно выбирает пивот перестановкой столбцов и возвращает решение в исходном порядке переменных. | [`GaussSLETests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GaussSLE/GaussSLETests.cs#L129) |

## Existing Tests

- Перенесены все содержательные legacy-сценарии на square/rectangular системы.
- Дополнительно добраны `RowWise` и `ColWise`, которые в legacy не проверялись отдельно.

## Gaps

- Предусловия конструктора и низкоуровневые размерностные `Debug.Assert` не закрепляются как release-контракт.
