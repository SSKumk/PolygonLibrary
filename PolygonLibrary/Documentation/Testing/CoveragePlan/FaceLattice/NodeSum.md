# Node Sum

## Scope

Сценарии для `FLNodeSum`:

- создание узла с заданными `InnerPoint` и `AffBasis`;
- явное связывание узлов через `AddSub` и `AddSuper`;
- построение уровней и перечисление всех непустых подграней;
- сравнение, равенство и `GetHashCode`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| FLS-CTOR-001 | x | Конструктор сохраняет `InnerPoint`, `AffBasis`, размерность и оставляет `Sub`/`Super` пустыми. | [`FaceLatticeNodeSumTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeSumTests.cs#L48) |
| FLS-LVL-001 | x | После явного связывания треугольной иерархии `GetAllLevels` и `GetLevelBelowNonStrict` корректно строят уровни. | [`FaceLatticeNodeSumTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeSumTests.cs#L61) |
| FLS-SUB-001 | x | `AllNonStrictSub` возвращает сам узел и все его непустые подграни. | [`FaceLatticeNodeSumTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeSumTests.cs#L82) |
| FLS-EQ-001 | x | `Equals` и `CompareTo` согласованы с текущим контрактом сравнения по `AffBasis` и размерности. | [`FaceLatticeNodeSumTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeSumTests.cs#L96) |
| FLS-OVR-001 | x | `GetHashCode` публично запрещён и бросает `InvalidOperationException`. | [`FaceLatticeNodeSumTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeSumTests.cs#L119) |

## Existing Tests

- Покрыт корректный треугольный сценарий с явным построением всей иерархии `FLNodeSum`.
- Отдельно закреплено, что текущая идентичность `FLNodeSum` задаётся через `AffBasis`, а не через `InnerPoint` или соседние узлы.
- Проверено, что уровень `dim > PolytopDim` даёт пустой результат.

## Gaps

- Отдельный слой на заведомо некорректные графы `FLNodeSum` пока не выделен: в `Release` это рассматривается как нарушение preconditions.
- Наборы выше малого треугольного примера пока не вынесены в отдельный direct-layer.
