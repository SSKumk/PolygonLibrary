# Line2D Construction

## Scope

Этот файл покрывает сценарии построения `Line2D` и связанные с этим инварианты:

- коэффициенты `A`, `B`, `C`;
- направляющий вектор `Direct`;
- нормаль `Normal`;
- ориентация положительной полуплоскости.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `L2D-CTOR-001` | `x` | Конструктор по умолчанию создаёт ось `Ox`. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-002` | `x` | Конструктор по двум точкам создаёт прямую, проходящую через обе точки. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-003` | `x` | Конструктор по двум точкам выставляет положительную полуплоскость слева от направления `p1 -> p2`. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-004` | `x` | Конструктор по двум точкам работает для горизонтальной прямой. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-005` | `x` | Конструктор по двум точкам работает для вертикальной прямой. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-006` | `x` | Конструктор по двум точкам работает для наклонной прямой. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-007` | `x` | Конструктор по двум точкам корректен при перестановке точек, если учитывать смену ориентации. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-008` | `x` | Конструктор по двум точкам и внешней точке выбирает ориентацию так, чтобы внешняя точка лежала в положительной полуплоскости. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-009` | `x` | Конструктор по двум точкам и внешней точке выбрасывает ошибку, если внешняя точка лежит на прямой. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-010` | `x` | Фабрика `Line2D_PointAndDirect` создаёт прямую по точке и направлению. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-011` | `x` | Фабрика `Line2D_PointAndDirect` с внешней точкой корректно выбирает ориентацию. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-012` | `x` | Фабрика `Line2D_PointAndNormal` создаёт прямую по точке и нормали. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-013` | `x` | Копирующий конструктор создаёт эквивалентную прямую. | [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-014` | `x` | После построения `Normal` и `Direct` ортогональны. | [`Line2DAssert.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DAssert.cs), [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-015` | `x` | После построения `Normal` и `Direct` нормированы. | [`Line2DAssert.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DAssert.cs), [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |
| `L2D-CTOR-016` | `x` | Коэффициенты `A`, `B`, `C` согласованы с `Normal`. | [`Line2DAssert.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DAssert.cs), [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) |

## Gaps

- В этом классе пока не зафиксированы отдельные сценарии для вырожденного направления `p1 == p2`, потому что такой контракт явно не описан кодом.

## Notes

- При написании тестов важно проверять не только прохождение прямой через опорные точки, но и знак индексатора на контрольных точках по разные стороны прямой.
