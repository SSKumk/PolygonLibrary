# ConvexPolygon

## Scope

Класс [`ConvexPolygon.cs`](../../../../CGLibrary/Geometry2D/Polygons/ConvexPolygons/ConvexPolygon.cs) описывает основной 2D-выпуклый полигон:

- построение из вершин, `Vector` и `SupportFunction`;
- ленивое согласование `Contour`, `Vertices`, `SF` и `Square`;
- точечные запросы `Contains*`;
- экстремальные элементы, случайные точки, разрез по вершинам;
- операции суммы и разности Минковского.

`IntersectionPolygon` документируется и тестируется отдельно в [`Intersection`](../Intersection).

## Topics

- [`Construction.md`](Construction.md) - конструкторы и ленивое согласование представлений.
- [`Containment.md`](Containment.md) - `Contains` и `ContainsInside`.
- [`ExtremeAreaAndNearest.md`](ExtremeAreaAndNearest.md) - `GetExtremeElements`, `Square`, `NearestPoint`.
- [`RandomAndCut.md`](RandomAndCut.md) - `GenerateDataForRandomPoint`, `GenerateRandomPoint`, `CutConvexPolygon`.
- [`MinkowskiOps.md`](MinkowskiOps.md) - операторы `+` и `-`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction | `x` | Конструкторы и ленивое восстановление `SF`/`Contour` покрыты прямым набором. |
| Containment | `x` | Legacy-сценарии `Contains*` перенесены почти полностью без потери сообщений. |
| Extreme, Area and Nearest | `x` | Покрыты экстремальные элементы, площадь и текущий `NotImplementedException` для `NearestPoint`. |
| Random and Cut | `x` | Случайные точки и `CutConvexPolygon` покрыты прямым набором. |
| Minkowski Operations | `x` | Legacy-сценарии сумм и разностей перенесены в отдельный файл. |

## Existing Test Sources

- [`ConvexPolygonTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonTests.cs)
- [`ConvexPolygonCutTests.cs`](../../../../Tests/Double-Tests/ConvexPolygonCutTests.cs)
- [`ConvexPolygonLegacyHelpers.cs`](../../../../Tests/Double-Tests/ConvexPolygonLegacyHelpers.cs)
- [`PolygonExtremeTests.cs`](../../../../Tests/Double-Tests/PolygonExtremeTests.cs)

## Notes

- Старые `CreateCP*`-тесты были фактически smoke-only и не проверяли поведение. В новой структуре они заменены прямыми assertions на реальный контракт.

