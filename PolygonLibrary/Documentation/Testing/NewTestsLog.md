# New Tests Log

Этот файл нужен как короткий журнал миграции: какие новые тестовые файлы появились для каждого класса и какой legacy-источник был заменён.

Формат записи:

- класс;
- новые тестовые файлы с кратким пояснением в скобках, какую функциональность они покрывают;
- legacy-файл, если он был;
- короткая заметка.

## Entries

### Line2D

- New tests:
  - [`Line2DAssert.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DAssert.cs) (`Vector2D`-ассерты и инварианты прямой)
  - [`Line2DConstructionTests.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) (конструкторы, фабрики, ориентация полуплоскости, инварианты)
  - [`Line2DQueriesAndIntersectionTests.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) (индексатор, `PassesThrough`, `Reorient`, пересечение прямых)
- Legacy:
  - отдельного legacy-файла не было
- Note:
  - пилотный перенос, на котором был зафиксирован формат новой структуры

### Vector2D

- New tests:
  - [`Vector2DAssert.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DAssert.cs) (сравнение `Vector2D` с учётом `Eps`)
  - [`Vector2DConstructionAndComparisonTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) (конструкторы, константы, индексатор, сравнение, `ToString`, `PolarAngle`)
  - [`Vector2DArithmeticAndAnglesTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) (арифметика, нормы, повороты, углы, `FromPolar`)
  - [`Vector2DRelationsAndCombinationsTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) (отношения между векторами, `IsBetween`, линейные комбинации, приведение из `Vector`)
- Legacy:
  - [`VectorsTests.cs`](../../Tests/Double-Tests/VectorsTests.cs)
- Note:
  - сохранены исторические поясняющие сообщения и группировка сценариев для `AngleTest` и `IsBetweenTest`

### Segment

- New tests:
  - [`SegmentAssert.cs`](../../Tests/DoubleGeometry/Basics/Segment/SegmentAssert.cs) (ассерты для `Vector2D` и геометрии результата пересечения)
  - [`SegmentConstructionAndGeometryTests.cs`](../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) (конструкторы, геометрические свойства, сравнение, `Equals`, `ToString`)
  - [`SegmentPointQueriesTests.cs`](../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) (точечные запросы, `ContainsPoint`, `IsEndPoint`, `IsInnerPoint`, `ComputeAtPoint`)
  - [`SegmentIntersectionTests.cs`](../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) (пересечение отрезков, `CrossInfo`, типы и позиции точек пересечения)
- Legacy:
  - [`SegmentCrossTests.cs`](../../Tests/Double-Tests/SegmentCrossTests.cs)
- Note:
  - сохранён исторический текст сообщений из `SegmentContainsPointTest`; старый закомментированный черновик по `Intersect` заменён активным системным набором тестов

### Intersection

- New tests:
  - [`ConvexPolygonIntersectionTestBase.cs`](../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionTestBase.cs) (общие тестовые данные, циклическое сравнение вершин и симметричная проверка `IntersectionPolygon`)
  - [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs) (обычные непустые пересечения, симметрия результата и инвариантность к циклическому сдвигу вершин)
  - [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs) (вложение, пустое пересечение, касания, вырожденные пересечения и `null`-входы)
- Legacy:
  - [`ConvexPolygonIntersectionTests.cs`](../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs)
- Note:
  - сохранены исторические диагностические сообщения `Intersection..` и вынесен отдельный недостающий тест на `null`-аргументы

### SegmentPair

- New tests:
  - [`SegmentPairConstructionAndComparisonTests.cs`](../../Tests/DoubleGeometry/Basics/SegmentPair/SegmentPairConstructionAndComparisonTests.cs) (нормализация порядка пары, `CompareTo` и совместимость с `SortedSet`)
- Legacy:
  - прямого legacy-файла не было; раньше покрытие было только косвенным через [`BentlyOttmannTests.cs`](../../Tests/BentlyOttmannTests.cs)
- Note:
  - для `SegmentPair` добавлен короткий прямой unit-like набор вместо косвенной проверки через большой sweep-line сценарий; по дороге класс был возвращён в активную сборку `CGLibrary`
