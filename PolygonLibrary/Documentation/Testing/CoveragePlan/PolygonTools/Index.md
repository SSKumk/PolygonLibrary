# PolygonTools

## Scope

Класс [`PolygonTools.cs`](../../../../CGLibrary/Geometry2D/Polygons/PolygonTools.cs) содержит фабрики простых выпуклых полигонов:

- осе-параллельные прямоугольники;
- повёрнутые прямоугольники;
- регулярные многоугольные аппроксимации окружности;
- многоугольные аппроксимации эллипса.

## Topics

- [`RectangleParallel.md`](RectangleParallel.md) - `RectangleParallel` и его вырожденные случаи.
- [`RectangleTurned.md`](RectangleTurned.md) - `RectangleTurned` для совпадающих, осе-диагональных и общих противоположных вершин.
- [`CircleAndEllipse.md`](CircleAndEllipse.md) - `Circle` и `Ellipse`, включая вырожденные полуоси.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| RectangleParallel | `x` | Перенесено из прямого legacy-файла с сохранением диагностических текстов. |
| RectangleTurned | `x` | Перенесено из прямого legacy-файла с сохранением диагностических текстов. |
| Circle and Ellipse | `x` | Добавлен прямой набор для ранее непокрытых фабрик окружности и эллипса. |

## Existing Test Sources

- [`PolygonToolsTests.cs`](../../../../Tests/Archive/Double-Tests/PolygonToolsTests.cs)
- косвенные сценарии в `ConvexPolygon*`, `PolygonExtremeTests.cs`

## Notes

- Для `Circle` и `Ellipse` приоритет отдан собственным фабричным контрактам `PolygonTools`, а не косвенным сценариям потребителей.
