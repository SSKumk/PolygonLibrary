# Orthonormalize Stability

Эта заметка фиксирует минимальный вывод по численной устойчивости `LinearBasis.Orthonormalize`.

Связанный код:
- [`../../../CGLibrary/Basics/LinearBasis.cs`](../../../CGLibrary/Basics/LinearBasis.cs)
- [`QRLQHouseholderDecomposition.md`](./QRLQHouseholderDecomposition.md)

## Проблема

Исторически `Orthonormalize(Vector v)` использовал projector-based формулу:

```text
residual = v - B^T (B v),
q = normalize(residual).
```

Если `v` почти лежит в `span(B)`, а его компонент в `span(B)` большой по норме, здесь появляется катастрофическое вычитание близких больших векторов.

## Что сравнивалось

Сравнивались две схемы:

- historical projector-based `v - B^T(Bv)`;
- `LQ`-based вариант через `Decomposition.LQ_IncrementalUpdate(...)`.

Во втором варианте берётся полный ортогональный оператор, составленный из:

- активного `LinearBasis`;
- `OrthogonalComplement()`.

После `LQ`-update новый ортогональный вектор читается как новая активная строка обновлённого оператора.

## Минимально репрезентативные сценарии

Использовалось семейство входов

```text
v = u + eps * n,
u in span(B),
n in span(B)^⊥, ||n|| = 1.
```

Оставлены три сценария:

1. `d = 10`, `k = 9`, `||u|| ~= 1`, `eps = 1e-6`
2. `d = 10`, `k = 9`, `||u|| ~= 1e8`, `eps = 1e-6`
3. `d = 30`, `k = 29`, `||u|| ~= 1e8`, `eps = 1e-7`

## Наблюдение

- При `||u|| ~= 1` обе схемы ещё работают приемлемо, хотя `LQ` обычно точнее.
- При `||u|| ~= 1e8` projector-based вариант быстро теряет направление ортогональной компоненты.
- В тех же сценариях `LQ`-подход остаётся практически на уровне машинного нуля по ошибке направления.

Это означает, что проблема не сводится только к policy через `Tools.Eps`: projector-based формула может вернуть ненулевой, но уже неправильный unit-вектор.

## Вывод

Если `Orthonormalize` используется как строитель нового направления при росте базиса, `LQ`-подход предпочтительнее projector-based формулы.

Именно поэтому production-реализация переведена на `LQ`, а historical projector-based вариант сохранён только в research-слое как воспроизводимый baseline.

## Воспроизводимость

Минимальный воспроизводимый набор сохранён в research-слое:

- [`../../../Tests/DoubleGeometry/Research/LinearBasis/README.md`](../../../Tests/DoubleGeometry/Research/LinearBasis/README.md)
- [`../../../Tests/DoubleGeometry/Research/LinearBasis/OrthonormalizeStabilityResearchTests.cs`](../../../Tests/DoubleGeometry/Research/LinearBasis/OrthonormalizeStabilityResearchTests.cs)
