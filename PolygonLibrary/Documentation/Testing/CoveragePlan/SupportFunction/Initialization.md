# SupportFunction Initialization

## Scope

Этот файл покрывает создание `SupportFunction` и нормализацию входных данных.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SF-INIT-001` | `x` | Конструктор из массива `GammaPair` нормализует ненулевые нормали. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-002` | `x` | Конструктор из массива `GammaPair` сортирует пары против часовой стрелки по полярному углу. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-003` | `x` | Конструктор из массива `GammaPair` удаляет дубликаты нормалей после сортировки. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-004` | `x` | Конструктор из массива `GammaPair` отбрасывает пары с нулевой нормалью. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-005` | `x` | Конструктор из массива `GammaPair` запрещает инициализацию, если все нормали нулевые. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-006` | `x` | Конструктор из списка точек для выпуклого многоугольника строит корректный набор граничных нормалей. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-007` | `x` | Конструктор из двух точек строит специальную поддержку для отрезка. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-008` | `x` | Конструктор из одной точки строит четыре осевых ограничения, описывающих точку. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-009` | `x` | При `ToConvexify = true` конструктор сначала выпуклифицирует набор точек. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |

## Gaps

- Сценарий для пустого списка точек не закрепляется как публичный runtime-контракт: текущий код не оформляет его как явное исключение уровня API.

## Notes

- Содержательные legacy-сообщения `"Wrong number of resultant vectors"` и `"i = ..."` сохранены в сценариях старого набора на инициализацию из `GammaPair`.

