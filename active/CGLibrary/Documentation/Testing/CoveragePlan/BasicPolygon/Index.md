# BasicPolygon

## Scope

Класс [`BasicPolygon.cs`](../../../../CGLibrary/Geometry2D/Polygons/BasicPolygon.cs) задаёт базовый полигональный каркас:

- хранение контуров, вершин и рёбер;
- ленивое восстановление недостающих представлений;
- базовые конструкторы для одно-контурного полигона.

Абстрактные методы `Contains` и `ContainsInside` в этом плане не рассматриваются, потому что базовый класс не даёт им реализации.

## Topics

- [`Construction.md`](Construction.md) - базовые конструкторы и начальная инициализация.
- [`LazyAggregation.md`](LazyAggregation.md) - ленивое вычисление `Contours`, `Vertices` и `Edges`.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction | `x` | Покрыты оба базовых конструктора и различие copy/reference-семантики. |
| Lazy Aggregation | `x` | Покрыто восстановление `Vertices`, `Edges` и `Contours` из других представлений. |

## Existing Test Sources

- прямого legacy-файла не было

## Notes

- `BasicPolygon` тестируется через небольшой служебный наследник, потому что сам класс абстрактный.

