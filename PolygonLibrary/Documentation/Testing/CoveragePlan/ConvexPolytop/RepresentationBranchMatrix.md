# ConvexPolytop Representation Branch Matrix

## Scope

Вспомогательная таблица по публичным методам [`ConvexPolytop.cs`](../../../../CGLibrary/GeometryND/Polyhedra/ConvexPolytop.cs), у которых логика различается для `Vrep`, `Hrep` и `FLrep`.

Этот файл не заменяет основной coverage plan. Он нужен как быстрый ориентир, где уже есть прямые branch-specific тесты, а где пока покрыта только часть ветвей.

## Matrix

| Method | Vrep branch | Hrep branch | FLrep branch | Notes |
| --- | --- | --- | --- | --- |
| `InnerPoint` | `x` | `x` | `x` | Для `FLrep` есть отдельный прямой тест на использование `Top.InnerPoint`. |
| `GetInFLrep` / `GetInHrep` / `GetInVrep` | `x` | `x` | `x` | Есть прямой кросс-представленческий сценарий эквивалентности. |
| `Contains` | `x` | `x` | `x` | Для `Vrep` зафиксирован текущий `NotImplementedException`. |
| `NearestPoint` | `x` | `x` | `x` | Для `Vrep` и `Hrep` зафиксирован текущий `NotImplementedException`, для `FLrep` покрыты рабочие ветки. |
| `Shift` | `x` | `x` | `x` | Есть прямые branch-specific тесты на `Vrep`, `Hrep` и `FLrep` с проверкой сохранения приоритета представления. |
| `Rotate` | `x` | `x` | `x` | Есть прямые branch-specific тесты на `Vrep`, `Hrep` и `FLrep` с проверкой сохранения приоритета представления. |
| `Scale` | `x` | `x` | `x` | После последних правок покрыты положительный и отрицательный коэффициенты, включая ненулевой центр. |
| `Equals` | `x` | `x` | `x` | Есть кросс-представленческие сценарии; часть веток покрывается через lazy materialization. |
| `Polar` | ` ` | ` ` | ` ` | Вынесен из активного слоя. |

## Notes

- Таблица отслеживает именно прямое ветвление по представлениям, а не полное математическое покрытие каждого метода.
- По активному слою матрица branch-specific веток закрыта; вне её сознательно остаётся только `Polar`, который вынесен в отдельный будущий слой.
