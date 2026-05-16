# AffineBasis

## Scope

Класс [`CGLibrary/Basics/AffineBasis.cs`](../../../../CGLibrary/Basics/AffineBasis.cs) задаёт аффинный базис как пару из точки-начала и линейного базиса:

- построение аффинного подпространства;
- проекции точек и перевод координат;
- проверку принадлежности;
- сравнение аффинных подпространств;
- mutable-вариант с добавлением направляющих векторов.

Это следующий слой над `LinearBasis`.

## Topics

- [`Construction.md`](Construction.md) - конструкторы, copy/no-copy сценарии и базовые инварианты.
- [`FactoriesAndMutation.md`](FactoriesAndMutation.md) - фабрики и `AffineBasisMutable.AddVector`.
- [`ProjectionAndContainment.md`](ProjectionAndContainment.md) - проекции, `Contains`, `ToOriginalCoords`, `CanonicalOrigin`, `OrthonormalVector`.
- [`ComparisonAndEnumeration.md`](ComparisonAndEnumeration.md) - `Equals`, `CompareTo`, `GetHashCode`, перечисление.

## Current Coverage Summary

| Topic | Status | Comment |
| --- | --- | --- |
| Construction | `x` | Базовые конструкторы и copy/no-copy контракты перенесены. |
| Factories and Mutation | `x` | Фабрики и mutable-сценарии перенесены. |
| Projection and Containment | `x` | Проекции, координатные преобразования и `Contains` перенесены. |
| Comparison and Enumeration | `x` | Сравнение, canonical origin, `GetHashCode` и перечисление зафиксированы. |

## Existing Test Sources

Исторический источник покрытия:

- ``AffineBasisTest.cs``

## Notes

- Для `AffineBasis` важно фиксировать не только линейную часть, но и положение аффинного подпространства через `Origin` и `CanonicalOrigin`.
- Контракт `AffineBasis(origin, LinearBasisMutable, needCopy: false)` специально зафиксирован отдельно: базовый immutable-конструктор такой объект не принимает.
