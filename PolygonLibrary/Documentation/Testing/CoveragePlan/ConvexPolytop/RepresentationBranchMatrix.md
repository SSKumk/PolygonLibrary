# ConvexPolytop Representation Branch Matrix

## Scope

Вспомогательная таблица по публичным методам [`ConvexPolytop.cs`](../../../../CGLibrary/Polyhedra/ConvexPolyhedra/ConvexPolytops/ConvexPolytop.cs), у которых логика различается для `Vrep`, `Hrep` и `FLrep`.

Этот файл не заменяет основной coverage plan. Он нужен как быстрый ориентир, где уже есть прямые branch-specific тесты, а где пока покрыта только часть ветвей.

## Matrix

| Method | Vrep branch | Hrep branch | FLrep branch | Notes |
| --- | --- | --- | --- | --- |
| `InnerPoint` | `x` | `x` | ` ` | Для `FLrep` нет отдельного прямого теста именно на lazy-ветку вычисления. |
| `GetInFLrep` / `GetInHrep` / `GetInVrep` | `x` | `x` | `x` | Есть прямой кросс-представленческий сценарий эквивалентности. |
| `Contains` | `x` | `x` | `x` | Для `Vrep` зафиксирован текущий `NotImplementedException`. |
| `NearestPoint` | ` ` | `x` | `x` | Для чистого `Vrep` отдельной ветки нет; для `Hrep` зафиксирован текущий `NotImplementedException`, для `FLrep` покрыты рабочие ветки. |
| `Shift` | `x` | ` ` | ` ` | Прямые branch-specific тесты для `Hrep` и `FLrep` пока не добавлены. |
| `Rotate` | `x` | ` ` | ` ` | Прямые branch-specific тесты для `Hrep` и `FLrep` пока не добавлены. |
| `Scale` | `x` | `x` | `x` | После последних правок покрыты положительный и отрицательный коэффициенты, включая ненулевой центр. |
| `Equals` | `x` | `x` | `x` | Есть кросс-представленческие сценарии; часть веток покрывается через lazy materialization. |
| `Polar` | ` ` | ` ` | ` ` | Вынесен из активного слоя. |

## Notes

- Таблица отслеживает именно прямое ветвление по представлениям, а не полное математическое покрытие каждого метода.
- Для `Shift` и `Rotate` следующими кандидатами на добор остаются отдельные тесты `Hrep` и `FLrep`, чтобы закрыть branch-specific поведение так же плотно, как это уже сделано для `Scale`.
