# Line2D

## Scope

Класс [`CGLibrary/Basics/Line2D.cs`](../../../../CGLibrary/Basics/Line2D.cs) описывает прямую на плоскости в общем виде `Ax + By + C = 0` и хранит:

- коэффициенты общего уравнения;
- направляющий вектор;
- нормаль;
- ориентацию положительной полуплоскости;
- операции проверки принадлежности и пересечения двух прямых.

## Topics

- [`Construction.md`](Construction.md) — конструкторы, фабрики и ориентация полуплоскости.
- [`QueriesAndIntersection.md`](QueriesAndIntersection.md) — индексатор, принадлежность точки, переориентация и пересечение прямых.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction | `x` | Базовый набор сценариев покрыт тестами из `Tests/DoubleGeometry/Basics/Line2D`. |
| Queries and Intersection | `x` | Базовый набор сценариев покрыт тестами из `Tests/DoubleGeometry/Basics/Line2D`. |

## Existing Tests

Покрытие `Line2D` вынесено в новую структуру:

- [`Line2DConstructionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs)
- [`Line2DQueriesAndIntersectionTests.cs`](../../../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs)




