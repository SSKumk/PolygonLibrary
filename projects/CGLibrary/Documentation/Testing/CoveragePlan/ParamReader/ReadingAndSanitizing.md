# Reading And Sanitizing

## Covered scenarios

- `PR-READ-001` Базовый legacy-сценарий чтения строк, bool, чисел, 1D/2D массивов и jagged-структур.
- `PR-READ-002` `PeakString` читает строку без продвижения по потоку.
- `PR-READ-003` `GetSanitizedData` удаляет комментарии и все whitespace-символы по текущему контракту.
- `PR-READ-004` `ReadVector`, `ReadVectors` и `ReadHyperPlanes` корректно строят геометрические объекты из структурированного ввода.
- `PR-READ-005` `ReadNumberLine` пропускает ведущий whitespace и читает строку фиксированной длины.

## Notes

- Для `GetSanitizedData` закрепляется именно текущая семантика глобального удаления whitespace, включая пробелы внутри строковых литералов.
- В active layer фиксируется наблюдаемый текущий контракт `PeakString`; переименование API в `PeekString` не рассматривается как часть этого шага.
