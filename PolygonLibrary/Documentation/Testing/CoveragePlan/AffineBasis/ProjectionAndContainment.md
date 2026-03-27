# AffineBasis Projection And Containment

## Scope

Этот файл покрывает проекции точек, перевод координат, `Contains`, `CanonicalOrigin` и `OrthonormalVector`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `AFB-PROJ-001` | `x` | `ProjectPointToSubSpace_in_OrigSpace` корректно проецирует точки на плоскость в исходных координатах. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |
| `AFB-PROJ-002` | `x` | `ProjectPointToSubSpace_in_OrigSpace` корректно работает для прямой и нульмерного аффинного пространства. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |
| `AFB-PROJ-003` | `x` | `ProjectPointToSubSpace` возвращает координаты в аффинном базисе нужной размерности. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |
| `AFB-PROJ-004` | `x` | `ProjectPoints` корректно проецирует набор точек. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |
| `AFB-PROJ-005` | `x` | `ToOriginalCoords(Vector)` корректно переводит координаты аффинного базиса в исходное пространство. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |
| `AFB-PROJ-006` | `x` | `ToOriginalCoords(IEnumerable<Vector>)` корректно переводит набор точек. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |
| `AFB-PROJ-007` | `x` | `CanonicalOrigin` совпадает с проекцией начала координат на аффинное пространство. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |
| `AFB-PROJ-008` | `x` | `Contains` корректно работает для плоскости, полного пространства, точки и сдвинутого аффинного подпространства. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |
| `AFB-PROJ-009` | `x` | `OrthonormalVector` возвращает единичный вектор, ортогональный линейной части, если дополнение ненулевое. | [`AffineBasisProjectionAndContainmentTests.cs`](../../../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) |

## Gaps

- Нет.

## Notes

- Для `Contains` здесь проверяется именно принадлежность аффинному подпространству, а не только линейной части.
