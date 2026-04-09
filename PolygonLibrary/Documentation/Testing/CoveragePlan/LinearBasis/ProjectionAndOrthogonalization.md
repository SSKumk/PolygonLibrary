# LinearBasis Projection And Orthogonalization

## Scope

Этот файл покрывает проекции, `Contains`, ортогональное дополнение, `OrthogonalComplementVector()` и контракт `Orthonormalize(Vector)`.

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
| `LB-PROJ-009` | `x` | `OrthogonalComplementVector()` возвращает единичный вектор из ортогонального дополнения либо нулевой вектор для полного базиса. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-010` | `x` | `Orthonormalize(Vector)` для вектора, уже лежащего в подпространстве, возвращает нулевой вектор. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-011` | `x` | `Orthonormalize(Vector)` для ортогонального вектора возвращает его нормализованную версию. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-012` | `x` | `Orthonormalize(Vector)` для общего вектора возвращает нормализованный остаток `v - proj(v)`, ортогональный базису. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-013` | `x` | `Orthonormalize(Vector)` для пустого базиса возвращает нормализованный входной вектор, а для полного базиса и нулевого вектора возвращает ноль. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |
| `LB-PROJ-014` | `x` | Детерминированные и случайные сценарии на проекции, ортогональное дополнение и `Orthonormalize(Vector)` сохраняют инварианты единичной длины и ортогональности. | [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) |

## Gaps

- Нужно отдельно проверить численную устойчивость `Orthonormalize(Vector)` на почти зависимых входах и решить, достаточно ли текущей прямой реализации через `v - proj(v)`.
