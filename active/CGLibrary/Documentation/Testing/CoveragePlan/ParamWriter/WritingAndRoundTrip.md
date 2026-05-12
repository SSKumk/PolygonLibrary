# Writing And Round Trip

## Covered scenarios

- `PW-WRITE-001` `WriteNumber` и `WriteString` записывают простые значения, которые затем читаются через `ParamReader`.
- `PW-WRITE-002` `WriteString` экранирует `"`, `\`, `\n`, `\r`, `\t` и сохраняет round-trip с `ParamReader`.
- `PW-WRITE-003` `Write1DArray`, `WriteVector` и `WriteVectors` сохраняют числовые коллекции в round-trip сценарии.
- `PW-WRITE-004` `Write2DArray` и `WriteHyperPlanes` сохраняют структурированные геометрические данные в round-trip сценарии.
- `PW-WRITE-005` Конструктор `ParamWriter(path, append: true)` дописывает новые поля в существующий файл.

## Notes

- `WriteString` теперь симметричен `ParamReader.ReadString`: writer использует тот же набор escape-последовательностей, который reader умеет читать.
