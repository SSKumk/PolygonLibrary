# Initial Vertex Recovery

## Scope

Сценарии для low-level восстановления стартовой вершины по `Hrep`:

- `FindInitialVertex_Simplex`;
- доуточнение simplex-optimum, если он лежит на оптимальной грани, а не в вершине;
- возврат полного active set в найденной вершине;
- работа с вырожденной вершиной, у которой активных гиперплоскостей больше, чем размерность пространства.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CPT-IVR-001 | x | `FindInitialVertex_Simplex` в `2D` доуточняет simplex-optimum с ребра оптимума до максимизирующей вершины и возвращает active set найденной вершины. | [../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs#L15) |
| CPT-IVR-002 | x | `FindInitialVertex_Simplex` в `3D` доуточняет simplex-optimum с двумерной оптимальной грани до максимизирующей вершины, даже если simplex basis initially задаёт только часть нужных гиперплоскостей. | [../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs#L39) |
| CPT-IVR-003 | x | `FindInitialVertex_Simplex` в `4D` и `5D` доуточняет non-vertex simplex-optimum на оптимальном лице до вершины и сохраняет принадлежность найденной точки множеству максимизирующих вершин. | [../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs#L64) |
| CPT-IVR-004 | x | `FindInitialVertex_Simplex` корректно обрабатывает вырожденную максимизирующую вершину в `2D-5D`: simplex может сразу вернуть вершину, а наружу метод должен вернуть полный active set из `dim + 1` гиперплоскостей. | [../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopSimplexVertexRecoveryTests.cs#L90), [../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs#L97) |

## Gaps

- `FindInitialVertex_Naive` не вынесен в отдельный активный слой и здесь не фиксируется.
- Тяжёлые сценарии `HrepToVrep_*` и полная геометрическая конвертация остаются вне этой темы.

## Notes

- В текущем контракте `FindInitialVertex_Simplex` опирается на simplex basis только как на стартовое лицо, а наружу возвращает уже полный active set в финальной вершине.
- Вырожденная вершина в этом coverage-файле считается нормальным публичным контрактом именно для этого API, а не "некорректным" входом.
