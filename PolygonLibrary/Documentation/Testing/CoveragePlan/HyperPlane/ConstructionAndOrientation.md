# HyperPlane Construction And Orientation

## Scope

Этот файл покрывает построение `HyperPlane`, выбор ориентации нормали и ленивые инварианты.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `HPL-CTOR-001` | `x` | Конструктор `(normal, origin, true)` нормализует нормаль и сохраняет исходную точку. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CTOR-002` | `x` | Конструктор `(normal, origin, false)` использует нормаль как есть, если она уже допустима по контракту. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CTOR-003` | `x` | Конструктор `(normal, constant)` корректно вычисляет нормализованные `Normal`, `ConstantTerm` и `Origin`. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CTOR-004` | `x` | Конструктор из `AffineBasis` корректно восстанавливает плоскость и возвращает тот же `AffineBasis`. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CTOR-005` | `x` | Конструктор из `AffineBasis` с `toOrient` корректно разворачивает нормаль в требуемую сторону. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CTOR-006` | `x` | Конструктор из набора точек строит плоскость, содержащую все переданные точки. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CTOR-007` | `x` | `OrientNormal(point, isPositive)` меняет знак `Normal` и `ConstantTerm`, когда это требуется. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CTOR-008` | `x` | Ленивое вычисление `Normal` при наличии `AffBasis` сохраняет корректность гиперплоскости. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CTOR-009` | `x` | Ленивое вычисление `AffBasis` при наличии `Normal` сохраняет корректность гиперплоскости. | [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |

## Gaps

- Нет.

## Notes

- Для `HyperPlane` особенно важны инварианты согласованности между `Origin`, `Normal`, `ConstantTerm` и `AffBasis`, а не только отдельные поля.



