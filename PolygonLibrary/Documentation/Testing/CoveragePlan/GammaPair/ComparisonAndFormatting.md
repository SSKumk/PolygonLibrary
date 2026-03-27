# GammaPair Comparison And Formatting

## Scope

Этот файл покрывает `Equals`, `CompareTo` и `ToString`:

- сравнение по полярному углу нормали;
- сравнение по нормированному значению `Value / |Normal|`;
- строковое представление пары.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `GP-CMP-001` | `x` | `CompareTo` упорядочивает пары по полярному углу нормали. | [`GammaPairComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairComparisonAndFormattingTests.cs) |
| `GP-CMP-002` | `x` | При равных углах `CompareTo` сравнивает нормированное значение `Value / |Normal|`. | [`GammaPairComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairComparisonAndFormattingTests.cs) |
| `GP-CMP-003` | `x` | `Equals(GammaPair)` считает пары равными, если совпадают угол нормали и нормированное значение. | [`GammaPairConstructionAndEqualityTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairConstructionAndEqualityTests.cs) |
| `GP-CMP-004` | `x` | `CompareTo(null)` возвращает `1`. | [`GammaPairComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairComparisonAndFormattingTests.cs) |
| `GP-CMP-005` | `x` | Пары с одинаковым направлением нормали, но разной длиной, сравниваются по `Value / |Normal|`, а не по сырому `Value`. | [`GammaPairComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairComparisonAndFormattingTests.cs) |
| `GP-CMP-006` | `x` | `ToString()` возвращает строку формата `[(x;y);value]` с инвариантной культурой для значения. | [`GammaPairComparisonAndFormattingTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairComparisonAndFormattingTests.cs) |

## Gaps

- В mandatory-сценариях пробелов не осталось.

## Notes

- Здесь нет override `Equals(object)`, поэтому документируется именно `Equals(GammaPair)`.
- Для проверки нормированного сравнения нужны пары с сонаправленными нормалями разной длины.



