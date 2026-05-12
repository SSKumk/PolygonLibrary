# GammaPair Construction

## Scope

Этот файл покрывает конструкторы `GammaPair` и базовые инварианты хранения:

- значение по умолчанию;
- конструктор из нормали и значения;
- режим `ToNormalize`;
- копирующий конструктор.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `GP-CTOR-001` | `x` | Конструктор по умолчанию создаёт пару с нормалью `(1, 0)` и нулевым значением. | [`GammaPairConstructionAndEqualityTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairConstructionAndEqualityTests.cs) |
| `GP-CTOR-002` | `x` | Конструктор `GammaPair(normal, value, false)` сохраняет нормаль и значение без преобразования. | [`GammaPairConstructionAndEqualityTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairConstructionAndEqualityTests.cs) |
| `GP-CTOR-003` | `x` | Конструктор `GammaPair(normal, value, true)` нормализует нормаль до единичной длины. | [`GammaPairConstructionAndEqualityTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairConstructionAndEqualityTests.cs) |
| `GP-CTOR-004` | `x` | Конструктор `GammaPair(normal, value, true)` масштабирует `Value` на ту же длину, что и нормаль. | [`GammaPairConstructionAndEqualityTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairConstructionAndEqualityTests.cs) |
| `GP-CTOR-005` | `x` | Копирующий конструктор создаёт эквивалентную пару с теми же `Normal` и `Value`. | [`GammaPairConstructionAndEqualityTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairConstructionAndEqualityTests.cs) |
| `GP-CTOR-006` | `-` | Конструктор запрещает нулевую нормаль согласно текущему debug-контракту. | |

## Gaps

- Нет runtime-теста на нулевую нормаль: это debug-only precondition, а не стабильный release-контракт.

## Notes

- Для `ToNormalize` нужно брать нормаль с длиной, отличной от `1`, и проверять обе величины: `Normal` и `Value`.
- Косвенное использование конструктора в тестах `SupportFunction` больше не считается основным источником покрытия.
