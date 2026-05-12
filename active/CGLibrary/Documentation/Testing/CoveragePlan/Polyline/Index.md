# Polyline

## Scope

Класс [`Polyline.cs`](../../../../CGLibrary/Geometry2D/Polygons/Polyline.cs) описывает замкнутую полилинию и ограниченный ею полигон:

- построение по списку или массиву вершин;
- доступ к вершинам, рёбрам, циклическому индексатору и ориентации;
- вычисление ориентированной площади;
- проверки принадлежности точки контуру и внутренности полигона.

## Topics

- [`ConstructionAndGeometry.md`](ConstructionAndGeometry.md) - конструкторы, доступ к данным, рёбра, индексатор, площадь и `EdgeAngle`.
- [`Containment.md`](Containment.md) - `ContainsPoint` и `ContainsPointInside` для выпуклого и невыпуклого контура.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction and Geometry | `x` | Перенесено в отдельный файл новой структуры. |
| Containment | `x` | Перенесено из legacy-набора с сохранением диагностических сообщений и дополнено сценарием для clockwise-обхода. |

## Existing Test Sources

- [`PolylineTests.cs`](../../../../Tests/Archive/Double-Tests/PolylineTests.cs)

## Notes

- Параметры `checkSimplicity` и `checkOrientation` пока не реализуют обещанную в комментариях валидацию, поэтому в coverage фиксируется только реально наблюдаемый текущий контракт.
