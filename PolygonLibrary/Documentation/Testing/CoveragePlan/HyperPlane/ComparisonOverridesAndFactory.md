# HyperPlane Comparison Overrides And Factory

## Scope

Этот файл покрывает фабрику, сравнение, строковое представление и инвариантную проверку `HyperPlane`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `HPL-CMP-001` | `x` | `Make3D_xyParallel(z)` создаёт плоскость `z = const` с ожидаемыми `Normal`, `ConstantTerm` и `Origin`. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CMP-002` | `x` | `ToString()` возвращает строку в формате `normal constant`. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CMP-003` | `x` | `Equals(object)` считает равными только гиперплоскости с одинаковым представлением `Normal` и `ConstantTerm`. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CMP-004` | `x` | `CompareTo(null)` возвращает `1`. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CMP-005` | `x` | `CompareTo` возвращает `0` для идентичных гиперплоскостей. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CMP-006` | `x` | `CompareTo` сначала сравнивает `Normal`, а при совпадении - `ConstantTerm`. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs), [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |
| `HPL-CMP-007` | `x` | `CheckCorrectness(hp)` проходит для корректно построенной гиперплоскости. | [`HyperPlaneTests.cs`](../../../../Tests/SharedTests/HyperPlaneTests.cs) |

## Gaps

- Нет отдельного негативного теста на `CheckCorrectness` для заведомо некорректной гиперплоскости.
- Не зафиксирован отдельно контракт `GetHashCode`, который здесь сознательно не поддерживается.

## Notes

- `Equals` у `HyperPlane` не является геометрическим равенством в смысле “та же самая плоскость при противоположной нормали”; это важно держать явно в тест-плане.



