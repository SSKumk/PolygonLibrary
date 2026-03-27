# SupportFunction Initialization

## Scope

Этот файл покрывает создание `SupportFunction` и нормализацию входных данных.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SF-INIT-001` | `~` | Конструктор из массива `GammaPair` нормализует ненулевые нормали. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs), [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `SF-INIT-002` | `~` | Конструктор из массива `GammaPair` сортирует пары против часовой стрелки по полярному углу. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs), [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `SF-INIT-003` | `~` | Конструктор из массива `GammaPair` удаляет дубликаты нормалей после сортировки. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `SF-INIT-004` | ` ` | Конструктор из массива `GammaPair` отбрасывает пары с нулевой нормалью. | |
| `SF-INIT-005` | ` ` | Конструктор из массива `GammaPair` запрещает инициализацию, если все нормали нулевые. | |
| `SF-INIT-006` | ` ` | Конструктор из списка точек для выпуклого многоугольника строит корректный набор граничных нормалей. | |
| `SF-INIT-007` | ` ` | Конструктор из двух точек строит специальную поддержку для отрезка. | |
| `SF-INIT-008` | ` ` | Конструктор из одной точки строит четыре осевых ограничения, описывающих точку. | |
| `SF-INIT-009` | ` ` | При `ToConvexify = true` конструктор сначала выпуклифицирует набор точек. | |

## Gaps

- Нет явных прямых тестов на поведение с нулевыми нормалями.
- Нет прямых тестов на конструктор из точек для случая отрезка и точки.
- Нет прямого покрытия `ToConvexify`.

## Notes

- Для `SF-INIT-006` важно проверять не только число пар, но и согласованность значений с исходными вершинами.



