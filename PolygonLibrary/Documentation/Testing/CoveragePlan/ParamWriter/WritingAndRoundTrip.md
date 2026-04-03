# Writing And Round Trip

## Covered scenarios

- `PW-WRITE-001` `WriteNumber` и `WriteString` записывают простые значения, которые затем читаются через `ParamReader`.
- `PW-WRITE-002` `Write1DArray`, `WriteVector` и `WriteVectors` сохраняют числовые коллекции в round-trip сценарии.
- `PW-WRITE-003` `Write2DArray` и `WriteHyperPlanes` сохраняют структурированные геометрические данные в round-trip сценарии.
- `PW-WRITE-004` Конструктор `ParamWriter(path, append: true)` дописывает новые поля в существующий файл.

## Notes

- В active layer пока закреплён безопасный контракт для простых строк без escape-последовательностей.
- Экранирование кавычек, обратных слэшей и управляющих символов не закреплено и требует отдельного решения.
