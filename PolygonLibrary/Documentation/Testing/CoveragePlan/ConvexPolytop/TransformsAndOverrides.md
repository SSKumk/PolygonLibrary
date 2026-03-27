# Transforms And Overrides

## Scope

Сценарии:

- `Shift`, `Rotate`, `LiftUp`, `SectionByHyperPlane`, `ShiftToOrigin`, `Scale`;
- `WhichRepToString`, `Equals`, `GetHashCode`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CPT-TR-001 | x | `Shift` и `Rotate` корректно преобразуют множество вершин. | [`ConvexPolytopTransformsAndOverridesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L9) |
| CPT-TR-002 | x | `LiftUp` поднимает политоп в большее пространство и ставит новую координату равной заданному значению. | [`ConvexPolytopTransformsAndOverridesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L35) |
| CPT-TR-003 | x | `SectionByHyperPlane` возвращает сечение политопа по гиперплоскости. | [`ConvexPolytopTransformsAndOverridesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L47) |
| CPT-TR-004 | x | `ShiftToOrigin` возвращает использованную внутреннюю точку и переносит политоп вокруг нуля. | [`ConvexPolytopTransformsAndOverridesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L61) |
| CPT-TR-005 | x | `Scale` с неотрицательным коэффициентом масштабирует политоп, а отрицательный коэффициент пока запрещён. | [`ConvexPolytopTransformsAndOverridesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L79) |
| CPT-OVR-001 | x | `WhichRepToString`, `Equals` и `GetHashCode` следуют текущим контрактам для разных репрезентаций. | [`ConvexPolytopTransformsAndOverridesTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs#L99) |

## Existing Tests

- Исторические алгоритмические тесты регулярно полагались на `Equals` и на прямые преобразования политопов, но не фиксировали это отдельным компактным набором.
- В новой структуре эти контракты собраны в один прямой слой.

## Gaps

- `Polar` и IO-функции вынесены из обязательного минимума в отдельный будущий слой.

