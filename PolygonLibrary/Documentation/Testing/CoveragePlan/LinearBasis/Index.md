# LinearBasis

## Scope

Класс [`CGLibrary/Basics/LinearBasis.cs`](../../../../CGLibrary/Basics/LinearBasis.cs) задаёт ортонормированные линейные базисы и их mutable-вариант:

- построение подпространства;
- добавление векторов;
- проекции и проверки принадлежности;
- ортогональное дополнение;
- сравнение и генерацию.

Это следующий фундаментальный слой после `Vector` и `Matrix`.

## Topics

- [`Construction.md`](Construction.md) - конструкторы, базовые свойства и инварианты.
- [`Mutation.md`](Mutation.md) - `LinearBasisMutable`, `AddVector`, `AddVectors`.
- [`ProjectionAndOrthogonalization.md`](ProjectionAndOrthogonalization.md) - проекции, `Contains`, ортогональное дополнение и текущий контракт `Orthonormalize`.
- [`ComparisonAndSpan.md`](ComparisonAndSpan.md) - `Equals`, `CompareTo`, перечисление и `SpanSameSpace`.
- [`Generation.md`](Generation.md) - фабрики и детерминированные генерационные сценарии.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction | `x` | Базовые и составные конструкторы перенесены. |
| Mutation | `x` | Контракт `LinearBasisMutable` и добавление векторов перенесены. |
| Projection and Orthogonalization | `x` | Проекции и дополнения покрыты; для `Orthonormalize` зафиксирован текущий placeholder-контракт. |
| Comparison and Span | `x` | Перенесены сравнение, перечисление и сценарии на одинаковость подпространств. |
| Generation | `x` | Перенесены стандартные и детерминированные генерационные сценарии. |

## Existing Test Sources

Исторический источник покрытия:

- [`LinearBasisTests.cs`](../../../../Tests/Archive/SharedTests/LinearBasisTests.cs)

## Notes

- Для `LinearBasis` важно проверять не только геометрию подпространства, но и ортонормированность хранимых векторов.
- Метод `Orthonormalize(Vector)` сейчас публичный, но в коде остаётся не реализован и бросает `NotImplementedException`; это отражено в тематическом файле без притворства, что алгоритм уже готов.
