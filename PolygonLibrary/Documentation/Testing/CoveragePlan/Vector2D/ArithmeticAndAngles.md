# Vector2D Arithmetic And Angles

## Scope

Этот файл покрывает арифметику, расстояния, повороты и углы `Vector2D`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `V2D-AR-001` | `x` | Унарный минус меняет знак обеих координат. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-002` | `x` | Сложение и вычитание выполняются покоординатно. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-003` | `x` | Умножение на скаляр слева и справа даёт одинаковый результат. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-004` | `x` | Деление на скаляр делит обе координаты на число. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-005` | `x` | Скалярное произведение вычисляется по стандартной формуле. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-006` | `x` | Псевдоскалярное произведение вычисляет ориентированную площадь параллелограмма. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-007` | `x` | `Dist2(p1, p2)` равно квадрату `Dist(p1, p2)`. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-008` | `x` | `Normalize()` возвращает единичный вектор для ненулевого аргумента. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-009` | `x` | `NormalizeZero()` возвращает ноль для нулевого вектора. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-010` | `x` | `TurnCW()` поворачивает вектор на `-pi / 2`. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-011` | `x` | `TurnCCW()` поворачивает вектор на `pi / 2`. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-012` | `x` | `Turn(angle)` на `0` возвращает исходный вектор. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-013` | `x` | `Turn(angle)` на `pi / 2` совпадает с `TurnCCW()`. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-014` | `x` | `Turn(angle)` на `-pi / 2` совпадает с `TurnCW()`. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-015` | `x` | `Angle(v1, v2)` возвращает положительный угол при повороте против часовой стрелки. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-016` | `x` | `Angle(v1, v2)` возвращает отрицательный угол при повороте по часовой стрелке. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-017` | `x` | `Angle(v1, v2)` возвращает `0` при участии нулевого вектора согласно текущему контракту. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-018` | `x` | `Angle2PI(v1, v2)` переводит отрицательный угол в диапазон `[0, 2pi)`. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |
| `V2D-AR-019` | `x` | `FromPolar(angle, radius)` создаёт вектор с заданным углом и длиной. | [`Vector2DArithmeticAndAnglesTests.cs`](../../../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) |

## Gaps

- Обязательных gap-ов для текущего публичного контракта не осталось.

## Notes

- Исторический `AngleTest` из legacy-файла сохранён с прежними поясняющими сообщениями.
