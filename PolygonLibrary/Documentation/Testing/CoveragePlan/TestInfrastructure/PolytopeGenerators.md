# Polytope Generators

## Scope

Покрываются генераторы из [`TestsPolytopes.cs`](../../../../Tests/TestInfrastructure/TestsPolytopes.cs), которые используются несколькими тестовыми слоями.

## Covered Scenarios

| ID | Status | Scenario | Test |
| --- | --- | --- | --- |
| TI-POLY-001 | `x` | `Cube01` без inner points даёт чистый гиперкуб с `2^d` вершинами. | [`PolytopesGeneratorsTests.cs`](../../../../Tests/DoubleGeometry/TestInfrastructure/PolytopesGeneratorsTests.cs) |
| TI-POLY-002 | `x` | `Cube01` с inner points сохраняет все вершины чистого куба. | [`PolytopesGeneratorsTests.cs`](../../../../Tests/DoubleGeometry/TestInfrastructure/PolytopesGeneratorsTests.cs) |
| TI-POLY-003 | `x` | `Simplex` даёт `d+1` аффинно независимых вершин. | [`PolytopesGeneratorsTests.cs`](../../../../Tests/DoubleGeometry/TestInfrastructure/PolytopesGeneratorsTests.cs) |
| TI-POLY-004 | `x` | `SimplexRND` даёт full-dimensional simplex при фиксированном seed. | [`PolytopesGeneratorsTests.cs`](../../../../Tests/DoubleGeometry/TestInfrastructure/PolytopesGeneratorsTests.cs) |
| TI-POLY-005 | `x` | `CyclicPolytop` соблюдает размерность и число вершин. | [`PolytopesGeneratorsTests.cs`](../../../../Tests/DoubleGeometry/TestInfrastructure/PolytopesGeneratorsTests.cs) |
| TI-POLY-006 | `x` | `Sphere_list` возвращает точки нужной размерности на сфере заданного радиуса. | [`PolytopesGeneratorsTests.cs`](../../../../Tests/DoubleGeometry/TestInfrastructure/PolytopesGeneratorsTests.cs) |
| TI-POLY-007 | `x` | `MakePointsOnSphere_3D` добавляет полюса и сохраняет единичный радиус. | [`PolytopesGeneratorsTests.cs`](../../../../Tests/DoubleGeometry/TestInfrastructure/PolytopesGeneratorsTests.cs) |

## Notes

- Это sanity-слой, а не полный coverage-план для всей test-infrastructure.
- Цель слоя: быстро поймать ошибку в генераторах, чтобы не искать её потом в алгоритмах.
