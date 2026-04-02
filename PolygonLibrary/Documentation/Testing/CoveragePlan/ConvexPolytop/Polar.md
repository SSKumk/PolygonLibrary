# Polar

## Scope

Сценарии для:

- `Polar(bool doUnRedundancy = false)`;
- `Polar(out Vector shiftToOrigin, bool doUnRedundancy = false)`.

Метод требует аккуратного рассмотрения, потому что:

- имеет разные ветки для `Vrep`, `Hrep` и `FLrep`;
- опирается на геометрическую предпосылку "начало координат лежит строго внутри политопа";
- участвует в `HRedundancyByGW`, то есть его ошибки будут проявляться не локально.

## Current Code Observations

- `Polar(out Vector, ...)` сначала вызывает `ShiftToOrigin`, а затем обычный `Polar(...)`.
- `Polar(bool)`:
  - для `Vrep` переводит вершины в гиперплоскости `new HyperPlane(v, 1)`;
  - для `Hrep` переводит гиперплоскости в точки `hp.Normal / hp.ConstantTerm`;
  - для `FLrep` строит двойственную решётку граней через обращение уровней.
- В `Hrep`-ветке есть чувствительное место: деление на `hp.ConstantTerm`.
- В `FLrep`-ветке есть чувствительные места:
  - ориентация `HyperPlane(oldNode.AffBasis, (InnerPoint, false))`;
  - корректность dual-связей `Sub` / `Super`;
  - согласованность размерностей уровней при обращении решётки.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CPT-POL-001 | x | `Polar(out shift)` возвращает сдвиг, который совпадает с `ShiftToOrigin`, и для сдвинутого квадрата даёт dual к центрированному политопу. | [`ConvexPolytopPolarTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopPolarTests.cs#L42) |
| CPT-POL-002 | x | `Polar` для `Vrep` на 2D-политопе, содержащем начало координат, строит корректный dual в `Hrep`. | [`ConvexPolytopPolarTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopPolarTests.cs#L9) |
| CPT-POL-003 | x | `Polar` для `Hrep` на 2D-политопе, содержащем начало координат, строит корректный dual в `Vrep`. | [`ConvexPolytopPolarTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopPolarTests.cs#L21) |
| CPT-POL-004 | x | `Polar` для `FLrep` на базовом 2D-примере строит dual в `FLrep` с ожидаемой геометрией и `fVector`. | [`ConvexPolytopPolarTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopPolarTests.cs#L60) |
| CPT-POL-005 | x | `Polar(Polar(P))` восстанавливает исходный политоп для простого полноразмерного 2D-примера после допустимой нормализации представления. | [`ConvexPolytopPolarTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopPolarTests.cs#L34) |
| CPT-POL-006 | x | `Polar` согласован между `Vrep`, `Hrep` и `FLrep` для одного и того же центрированного квадрата. | [`ConvexPolytopPolarTests.cs`](../../../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopPolarTests.cs#L74) |
| CPT-POL-007 |   | `doUnRedundancy` в `Hrep`-ветке не меняет геометрию результата, а влияет только на наличие лишних точек. | |
| CPT-POL-008 |   | Для политопа, у которого начало координат не находится внутри, `Polar(out shift)` сначала корректно переносит его в ноль и только потом строит dual. | |
| CPT-POL-009 |   | Для осесимметричных эталонов (`Ball_1`, `Ball_oo`) `Polar` даёт ожидаемую парную норму: ромб ↔ квадрат. | |
| CPT-POL-010 |   | Для вырожденных или граничных случаев, где `hp.ConstantTerm = 0`, поведение не закрепляется как нормальный runtime-контракт и должно быть отдельно специфицировано. | |

## Open Questions

- Считаем ли мы `Polar` частью "фундаментального" слоя `ConvexPolytop` или уже отдельным геометрическим оператором повышенной сложности?
- Хотим ли мы закреплять `Polar(Polar(P)) = P` буквально через `Equals`, или через геометрическую эквивалентность множеств вершин / граней?
- Нужно ли считать сценарии с началом координат на границе политопа нарушением preconditions, или для них должен быть явный контракт?

## Notes

- Для первого прохода разумно ограничиться малыми 2D-примерами, где dual легко проверить руками:
  - квадрат;
  - ромб (`Ball_1`);
  - квадрат `Ball_oo`;
  - треугольник с началом координат строго внутри.
- После базового слоя следующим шагом остаются:
  - более строгая проверка duality на уровне самой решётки граней, а не только геометрии и `fVector`;
  - сценарий `doUnRedundancy`;
  - связь с `HRedundancyByGW`.
- В базовом 2D-слое уже подтверждены:
  - `Ball_oo(0, r) -> Polar() -> Ball_1(0, 1 / r)`;
  - `Ball_1(0, r).GetInHrep() -> Polar() -> Ball_oo(0, 1 / r)`;
  - `Polar(Polar(P))` для центрированного квадрата;
  - `Polar(out shift)` для сдвинутого квадрата;
  - базовая `FLrep`-ветка и согласованность `Vrep` / `Hrep` / `FLrep` на одном квадрате.
- Для `Polar(out shift)` пока сознательно не фиксируется приоритет представления результата: здесь важны сам сдвиг и геометрия dual, а не текущая lazy-materialized репрезентация объекта.
