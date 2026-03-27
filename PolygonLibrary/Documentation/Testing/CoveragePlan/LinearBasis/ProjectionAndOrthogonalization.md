# LinearBasis Projection And Orthogonalization

## Scope

Этот файл покрывает проекции, `Contains`, ортогональное дополнение, `OrthonormalVector` и текущий наблюдаемый контракт `Orthonormalize`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| `LB-PROJ-001` | `x` | `ProjectVectorToSubSpace_in_OrigSpace` проецирует вектор на подпространство в координатах исходного пространства. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-002` | `x` | Проекция вектора, уже лежащего в подпространстве, возвращает тот же вектор. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-003` | `x` | Проекция вектора, ортогонального подпространству, даёт нулевой вектор. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-004` | `x` | Для полного базиса проекция в исходных координатах возвращает исходный вектор. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-005` | `x` | `Contains` различает векторы внутри подпространства, вне его, а также специальные случаи пустого и полного базиса. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-006` | `x` | `ProjectVectorToSubSpace` возвращает координаты в базисе нужной размерности. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-007` | `x` | `ProjectVectorsToSubSpace` корректно проецирует набор векторов. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-008` | `x` | `OrthogonalComplement()` возвращает корректное ортогональное дополнение для частичного, полного и пустого базиса. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-009` | `x` | `OrthonormalVector()` возвращает ортонормированный вектор из дополнения либо нулевой вектор для полного базиса. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-010` | `x` | `Orthonormalize(Vector)` в текущей реализации остаётся неготовым API и бросает `NotImplementedException`. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-011` | `x` | Детерминированные сценарии на проекции и ортогональное дополнение с фиксированным генератором сохраняются при переносе. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |

## Gaps

- По текущему публичному API обязательных gap-ов нет, но `Orthonormalize(Vector)` остаётся функционально не реализованным.

## Notes

- Здесь сознательно зафиксирован именно текущий наблюдаемый контракт `Orthonormalize`, а не желаемое будущее поведение.
