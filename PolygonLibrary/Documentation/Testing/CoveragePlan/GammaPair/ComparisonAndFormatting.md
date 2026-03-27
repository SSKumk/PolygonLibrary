# GammaPair Comparison And Formatting

## Scope

Этот файл покрывает `Equals`, `CompareTo` и `ToString`:

- сравнение по полярному углу нормали;
- сравнение по нормированному значению `Value / |Normal|`;
- строковое представление пары.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `GP-CMP-001` | `~` | `CompareTo` упорядочивает пары по полярному углу нормали. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs), [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `GP-CMP-002` | `~` | При равных углах `CompareTo` сравнивает нормированное значение `Value / |Normal|`. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs), [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `GP-CMP-003` | `~` | `Equals(GammaPair)` считает пары равными, если совпадают угол нормали и нормированное значение. | [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs), [`SupportFunctionTests.cs`](../../../../Tests/Double-Tests/SupportFunctionTests.cs) |
| `GP-CMP-004` | ` ` | `CompareTo(null)` возвращает `1`. | |
| `GP-CMP-005` | ` ` | Пары с одинаковым направлением нормали, но разной длиной, сравниваются по `Value / |Normal|`, а не по сырому `Value`. | |
| `GP-CMP-006` | ` ` | `ToString()` возвращает строку формата `[(x;y);value]` с инвариантной культурой для значения. | |

## Gaps

- Нет прямого отдельного теста на `CompareTo(null)`.
- Нет прямого отдельного теста на `ToString()`.
- Косвенное покрытие через `SupportFunction` не гарантирует, что ошибка локализуется именно в `GammaPair`.

## Notes

- Здесь нет override `Equals(object)`, поэтому документируется именно `Equals(GammaPair)`.
- Для проверки нормированного сравнения нужны пары с сонаправленными нормалями разной длины.



