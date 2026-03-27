# HyperPlane Construction And Orientation

## Scope

Этот файл покрывает построение `HyperPlane`, выбор ориентации нормали и ленивые инварианты.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `HPL-CTOR-001` | `x` | Конструктор `(normal, origin, true)` нормализует нормаль и сохраняет исходную точку. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CTOR-002` | `x` | Конструктор `(normal, origin, false)` использует нормаль как есть, если она уже допустима по контракту. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CTOR-003` | `x` | Конструктор `(normal, constant)` корректно вычисляет нормализованные `Normal`, `ConstantTerm` и `Origin`. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CTOR-004` | `x` | Конструктор из `AffineBasis` корректно восстанавливает плоскость и возвращает тот же `AffineBasis`. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CTOR-005` | `x` | Конструктор из `AffineBasis` с `toOrient` корректно разворачивает нормаль в требуемую сторону. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CTOR-006` | `x` | Конструктор из набора точек строит плоскость, содержащую все переданные точки. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CTOR-007` | `x` | `OrientNormal(point, isPositive)` меняет знак `Normal` и `ConstantTerm`, когда это требуется. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CTOR-008` | `x` | Ленивое вычисление `Normal` при наличии `AffBasis` сохраняет корректность гиперплоскости. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CTOR-009` | `x` | Ленивое вычисление `AffBasis` при наличии `Normal` сохраняет корректность гиперплоскости. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |

## Gaps

- Нет отдельного сценария на ошибочную ориентационную точку, лежащую в самой гиперплоскости.
- Нет отдельного прямого сценария на некорректный `AffineBasis` вне debug-контракта.

## Notes

- Для `HyperPlane` особенно важны инварианты согласованности между `Origin`, `Normal`, `ConstantTerm` и `AffBasis`, а не только отдельные поля.



