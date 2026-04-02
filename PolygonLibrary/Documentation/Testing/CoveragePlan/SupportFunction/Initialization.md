# SupportFunction Initialization

## Scope

Этот файл покрывает создание `SupportFunction` и нормализацию входных данных.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `SF-INIT-001` | `x` | Конструктор из массива `GammaPair` нормализует ненулевые нормали. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-002` | `x` | Конструктор из массива `GammaPair` сортирует пары против часовой стрелки по полярному углу. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-003` | `x` | Конструктор из массива `GammaPair` удаляет дубликаты нормалей после сортировки. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-004` | `-` | Сценарии с нулевой нормалью на входе не входят в обязательное runtime-покрытие `SupportFunction`: публичный конструктор `GammaPair` запрещает их раньше. | |
| `SF-INIT-005` | `-` | Инициализация только парами с нулевыми нормалями не документируется на уровне `SupportFunction`, потому что нарушает preconditions `GammaPair`. | |
| `SF-INIT-006` | `x` | Конструктор из списка точек для выпуклого многоугольника строит корректный набор граничных нормалей. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-007` | `x` | Конструктор из двух точек строит специальную поддержку для отрезка. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-008` | `x` | Конструктор из одной точки строит четыре осевых ограничения, описывающих точку. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |
| `SF-INIT-009` | `x` | При `ToConvexify = true` конструктор сначала выпуклифицирует набор точек. | [`SupportFunctionInitializationTests.cs`](../../../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) |

## Gaps

- Сценарий для пустого списка точек не закрепляется как публичный runtime-контракт: текущий код не оформляет его как явное исключение уровня API.

## Notes

- Содержательные legacy-сообщения `"Wrong number of resultant vectors"` и `"i = ..."` сохранены в сценариях старого набора на инициализацию из `GammaPair`.
- Сценарии `SF-INIT-004` и `SF-INIT-005` сознательно выведены из активного покрытия: они требуют создать невалидный `GammaPair`, что противоречит более низкому публичному контракту.
