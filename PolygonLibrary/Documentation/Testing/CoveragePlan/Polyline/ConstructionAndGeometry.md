# Polyline Construction And Geometry

## Scope

Этот файл покрывает сценарии построения `Polyline` и базовые геометрические свойства:

- конструкторы по умолчанию, списку и массиву;
- `Count`, `Vertices`, `Edges`, `Orientation`, `IsEmpty` и циклический индексатор;
- ориентированную площадь `Square`;
- защищённый хелпер `EdgeAngle`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `PLN-CTOR-001` | `x` | Конструктор по умолчанию создаёт одноточечную полилинию в начале координат. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |
| `PLN-CTOR-002` | `x` | Конструктор по списку сохраняет переданный список вершин и объявленную ориентацию. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |
| `PLN-CTOR-003` | `x` | Конструктор по массиву копирует вершины во внутренний список. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |
| `PLN-CTOR-004` | `x` | Конструктор по пустому списку создаёт пустую полилинию с нулевой площадью и без рёбер. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |
| `PLN-ACC-001` | `x` | `Edges` строит замкнутую цепочку рёбер по соседним вершинам и последней замыкающей дуге. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |
| `PLN-ACC-002` | `x` | `Edges` кэширует построенный список между повторными обращениями. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |
| `PLN-ACC-003` | `x` | Индексатор по вершинам работает циклически для положительных и отрицательных индексов. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |
| `PLN-GEO-001` | `x` | `Square` вычисляет ориентированную площадь по порядку обхода вершин. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |
| `PLN-GEO-002` | `x` | `EdgeAngle` вычисляет полярный угол ребра и работает циклически по индексу. | [`PolylineConstructionAndGeometryTests.cs`](../../../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) |

## Gaps

- Отдельные тесты на изменение `Vertices` после кэширования `Edges` или `Square` не добавляются: это наблюдаемая деталь реализации, а не закреплённый контракт уровня API.

## Notes

- Для `Polyline` важно явно различать конструктор по списку и по массиву: первый сохраняет исходный контейнер, второй создаёт новую коллекцию.

