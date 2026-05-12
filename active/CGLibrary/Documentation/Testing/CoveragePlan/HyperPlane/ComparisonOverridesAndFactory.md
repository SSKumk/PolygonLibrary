# HyperPlane Comparison Overrides And Factory

## Scope

Этот файл покрывает фабрику, сравнение, строковое представление и инвариантную проверку `HyperPlane`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `HPL-CMP-001` | `x` | `Make3D_xyParallel(z)` создаёт плоскость `z = const` с ожидаемыми `Normal`, `ConstantTerm` и `Origin`. | [`HyperPlaneComparisonOverridesAndFactoryTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneComparisonOverridesAndFactoryTests.cs) |
| `HPL-CMP-002` | `x` | `ToString()` возвращает строку в формате `normal constant`. | [`HyperPlaneComparisonOverridesAndFactoryTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneComparisonOverridesAndFactoryTests.cs) |
| `HPL-CMP-003` | `x` | `Equals(object)` считает равными только гиперплоскости с одинаковым представлением `Normal` и `ConstantTerm`. | [`HyperPlaneComparisonOverridesAndFactoryTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneComparisonOverridesAndFactoryTests.cs) |
| `HPL-CMP-004` | `x` | `CompareTo(null)` возвращает `1`. | [`HyperPlaneComparisonOverridesAndFactoryTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneComparisonOverridesAndFactoryTests.cs) |
| `HPL-CMP-005` | `x` | `CompareTo` возвращает `0` для идентичных гиперплоскостей. | [`HyperPlaneComparisonOverridesAndFactoryTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneComparisonOverridesAndFactoryTests.cs) |
| `HPL-CMP-006` | `x` | `CompareTo` сначала сравнивает `Normal`, а при совпадении - `ConstantTerm`. | [`HyperPlaneComparisonOverridesAndFactoryTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneComparisonOverridesAndFactoryTests.cs) |
| `HPL-CMP-007` | `x` | `CheckCorrectness(hp)` проходит для корректно построенной гиперплоскости. | [`HyperPlaneAssert.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneAssert.cs), [`HyperPlaneConstructionAndOrientationTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) |
| `HPL-CMP-008` | `x` | `GetHashCode()` для `HyperPlane` остаётся запрещённой операцией и выбрасывает `InvalidOperationException`. | [`HyperPlaneComparisonOverridesAndFactoryTests.cs`](../../../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneComparisonOverridesAndFactoryTests.cs) |

## Gaps

- Нет.

## Notes

- `Equals` у `HyperPlane` не является геометрическим равенством в смысле “та же самая плоскость при противоположной нормали”; это важно держать явно в тест-плане.



