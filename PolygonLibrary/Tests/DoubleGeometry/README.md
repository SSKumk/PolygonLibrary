# DoubleGeometry Tests

Новая структура тестов для геометрического ядра `double`.

## Навигация

- [`../../Documentation/Testing/README.md`](../../Documentation/Testing/README.md)
  Объект: общий индекс тестовой документации проекта.
  Когда использовать: когда нужны правила тестового слоя, coverage-документация, review-реестры и общее описание процесса миграции.

- [`../../Documentation/Testing/ScenarioFormat.md`](../../Documentation/Testing/ScenarioFormat.md)
  Объект: эталонный формат сценарных документов.
  Когда использовать: когда нужно оформить новый coverage-план или проверить, что текущий файл синхронизирован с принятым шаблоном.

- [`../../Documentation/Testing/DoubleGeometryMigrationOrder.md`](../../Documentation/Testing/DoubleGeometryMigrationOrder.md)
  Объект: порядок закрытия классов в миграции `double`.
  Когда использовать: когда нужно понять, какой класс сейчас в работе, а какой уже перенесён и закрыт.

## Назначение

- хранить новый тестовый слой для геометрического ядра `double`;
- раскладывать тесты по темам и сущностям, а не большими legacy-файлами;
- давать стабильную структуру, синхронизированную с `Documentation/Testing/CoveragePlan/...`.

## Правила

- Полные правила миграции и активного слоя фиксируются в [`../../Documentation/Testing/README.md`](../../Documentation/Testing/README.md).
- Порядок закрытия классов фиксируется в [`../../Documentation/Testing/DoubleGeometryMigrationOrder.md`](../../Documentation/Testing/DoubleGeometryMigrationOrder.md).
- Формат coverage-документов фиксируется в [`../../Documentation/Testing/ScenarioFormat.md`](../../Documentation/Testing/ScenarioFormat.md).

## Локальная структура

- `Infrastructure` - общие ассерт-хелперы, базовые классы, test data.
- `Basics` - базовые геометрические объекты и операции.
- `Polygons` - 2D-полигональные сущности и их вспомогательные объекты.
- `Polyhedra` - полигональные/политопные структуры более высокого уровня.
- `Algorithms` - алгоритмы, использующие базовые геометрические объекты и линейную алгебру.
