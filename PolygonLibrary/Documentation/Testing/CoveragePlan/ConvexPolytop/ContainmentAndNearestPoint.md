# Containment And NearestPoint

## Scope

Сценарии:

- `Contains`, `ContainsOnBorder`, `ContainsNonStrict`, `ContainsStrict`, `ContainsComplement`;
- `NearestPoint` для реализованных веток `FLrep`/`Hrep`;
- текущие `NotImplementedException`-контракты.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CPT-CON-001 | x | `Contains` в `FLrep` различает внутреннюю точку, границу и внешнюю точку кодами `-1/0/1`. | [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs#L9) |
| CPT-CON-002 | x | `Contains` для чистого `Vrep` пока не реализован и бросает `NotImplementedException`. | [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs#L22) |
| CPT-NP-000 | x | `NearestPoint` для чистого `Vrep` пока не реализован и бросает `NotImplementedException`. | [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs#L30) |
| CPT-NP-001 | x | `NearestPoint` для внутренней точки в `FLrep` возвращает ближайшую проекцию на грань и `position = -1`. | [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs#L40) |
| CPT-NP-002 | x | `NearestPoint` для внешней точки в `FLrep` возвращает ближайшую видимую граничную точку и `position = 1`. | [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs#L53) |
| CPT-NP-003 | x | Перегрузка `NearestPoint(point)` возвращает тот же результат, что и `NearestPoint(point, out position)`, если статус точки не нужен. | [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs#L66) |
| CPT-NP-004 | x | `NearestPoint` на границе возвращает саму точку и `position = 0`. | [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs#L80) |
| CPT-NP-005 | x | `NearestPoint` для внешней точки в чистом `Hrep` пока не реализован и бросает `NotImplementedException`. | [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs#L93) |

## Existing Tests

- Раньше `Contains` и `NearestPoint` проверялись лишь косвенно через более крупные алгоритмы и вспомогательные объекты.
- В новой структуре зафиксированы и рабочие ветки, и текущие нереализованные ветки API.

## Gaps

- Специализированные сценарии `NearestPoint` для более высоких размерностей и для неоднозначно ближайших граней пока не вынесены в текущий активный слой.


