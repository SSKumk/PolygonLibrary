# HyperPlane Evaluation And Containment

## Scope

Этот файл покрывает вычисление положения точек и работу с коллекциями точек.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `HPL-EVAL-001` | `x` | `Eval(point)` возвращает `0` для точки на гиперплоскости. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-002` | `x` | `Eval(point)` возвращает положительное значение в положительном полупространстве. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-003` | `x` | `Eval(point)` возвращает отрицательное значение в отрицательном полупространстве. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-004` | `x` | `Contains`, `ContainsPositive`, `ContainsNegative` и `ContainsNegativeNonStrict` согласованы между собой. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-005` | `x` | `FilterIn` возвращает только точки, лежащие на гиперплоскости. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-006` | `x` | `FilterNotIn` возвращает только точки вне гиперплоскости. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-007` | `x` | `AllAtOneSide` возвращает `(true, 0)` для набора точек, лежащих на гиперплоскости. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-008` | `x` | `AllAtOneSide` возвращает `(true, 1)` для точек строго в положительном полупространстве. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-009` | `x` | `AllAtOneSide` возвращает `(true, -1)` для точек строго в отрицательном полупространстве. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-010` | `x` | `AllAtOneSide` возвращает `false` для смешанного набора точек. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |
| `HPL-EVAL-011` | `x` | `Contains` корректно работает для плоскости, заданной через `AffineBasis`. | [`HyperPlaneEvaluationAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) |

## Gaps

- Нет.

## Notes

- Для всех `Contains*` важно держать примеры по обе стороны и на самой границе, иначе легко пропустить инверсию знака.



