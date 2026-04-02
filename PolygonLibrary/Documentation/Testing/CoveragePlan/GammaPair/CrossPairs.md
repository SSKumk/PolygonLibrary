# GammaPair CrossPairs

## Scope

Этот файл покрывает статический метод `GammaPair.CrossPairs`, который находит точку пересечения двух прямых, заданных парами `(normal; value)`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `GP-XP-001` | `x` | Для непараллельных нормалей `CrossPairs` возвращает правильную точку пересечения. | [`GammaPairCrossPairsTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairCrossPairsTests.cs) |
| `GP-XP-002` | `x` | Порядок аргументов не влияет на результат пересечения. | [`GammaPairCrossPairsTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairCrossPairsTests.cs) |
| `GP-XP-003` | `-` | Для параллельных нормалей срабатывает защитный контракт метода. | |
| `GP-XP-004` | `x` | Метод корректно работает на дробных значениях и не только на целочисленных пересечениях. | [`GammaPairCrossPairsTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairCrossPairsTests.cs) |
| `GP-XP-005` | `x` | Метод корректно вычисляет пересечение для ненормированных, но непараллельных нормалей. | [`GammaPairCrossPairsTests.cs`](../../../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairCrossPairsTests.cs) |

## Gaps

- Параллельный случай не входит в активный слой, пока реализация не перейдёт от `Debug.Assert` к явному runtime-контракту.

## Notes

- Исторические диагностические сообщения `Bad crossing ...` сохранены для legacy-набора непараллельных случаев.




