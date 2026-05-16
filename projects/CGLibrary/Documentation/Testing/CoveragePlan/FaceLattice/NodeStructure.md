# Node Structure

## Scope

Сценарии для `FLNode`:

- создание вершины;
- создание ребра и грани из подузлов;
- восстановление `InnerPoint`, `AffBasis`, `Vertices`, связей `Sub/Super`;
- построение уровней и перечисление всех непустых подграней;
- сравнение, равенство и `GetHashCode`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| FLN-CTOR-001 | x | Конструктор по одной вершине создаёт 0-мерный узел с пустыми `Sub` и `Super`. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L9) |
| FLN-CTOR-002 | x | Конструктор по двум вершинам создаёт ребро с правильными `Sub`, `Super`, `Vertices`, `InnerPoint` и `AffBasis`. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L26) |
| FLN-CTOR-003 | x | Конструктор по трём рёбрам создаёт грань с правильными `Sub`, `Super`, `Vertices`, `InnerPoint` и `AffBasis`. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L59) |
| FLN-CTOR-005 | x | Конструктор по `sub` инвариантен к порядку перечисления эквивалентных подузлов. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs) |
| FLN-CTOR-004 | x | Конструктор с явным `AffineBasis` использует переданный базис без самостоятельного восстановления. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L87) |
| FLN-LVL-001 | x | `GetAllLevels` и `GetLevelBelowNonStrict` корректно строят уровни для ребра и грани. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L100) |
| FLN-SUB-001 | x | `AllNonStrictSub` возвращает сам узел и все его непустые подграни. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L120) |
| FLN-EQ-001 | x | `Equals` сравнивает узлы по множеству вершин, а не по ссылке. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L136) |
| FLN-CMP-001 | x | `CompareTo` упорядочивает узлы по размерности, затем по числу и лексикографическому порядку вершин. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L156) |
| FLN-CMP-002 | x | Для корректно построенных узлов `CompareTo == 0` согласован с `Equals`. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs) |
| FLN-OVR-001 | x | `GetHashCode` публично запрещён и бросает `InvalidOperationException`. | [`FaceLatticeNodeTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs#L174) |

## Existing Tests

- Перенесены все содержательные legacy-сценарии на построение vertex, edge и face-узлов.
- Сохранены исторические диагностические сообщения про размерности, `InnerPoint`, уровни и сравнение.
- Дополнительно закреплён публичный контракт `GetHashCode`, который в legacy не проверялся отдельно.
- Дополнительно закреплена инвариантность к порядку `sub` и согласованность `Equals`/`CompareTo` на корректных узлах.

## Gaps

- Отдельных обязательных сценариев на заведомо некорректные графы подузлов пока нет: код не фиксирует такой runtime-контракт явно.
- Для `FLNode(IEnumerable<FLNode> sub, ...)` нужно явно описать preconditions в XML, включая недопустимость пустого набора `sub`.
