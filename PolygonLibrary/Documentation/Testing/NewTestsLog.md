# New Tests Log

Этот файл нужен как короткий журнал миграции: какие новые тестовые файлы появились для каждого класса и какой legacy-источник был заменён.

Формат записи:

- класс;
- новые тестовые файлы;
- legacy-файл, если он был;
- короткая заметка.

## Entries

### Line2D

- New tests:
  - [`Line2DAssert.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DAssert.cs)
  - [`Line2DConstructionTests.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs)
  - [`Line2DQueriesAndIntersectionTests.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs)
- Legacy:
  - отдельного legacy-файла не было
- Note:
  - пилотный перенос, на котором был зафиксирован формат новой структуры

### Vector2D

- New tests:
  - [`Vector2DAssert.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DAssert.cs)
  - [`Vector2DConstructionAndComparisonTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs)
  - [`Vector2DArithmeticAndAnglesTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs)
  - [`Vector2DRelationsAndCombinationsTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs)
- Legacy:
  - [`VectorsTests.cs`](../../Tests/Double-Tests/VectorsTests.cs)
- Note:
  - сохранены исторические поясняющие сообщения и группировка сценариев для `AngleTest` и `IsBetweenTest`
