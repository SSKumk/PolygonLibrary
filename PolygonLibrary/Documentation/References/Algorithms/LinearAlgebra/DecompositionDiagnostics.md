# Decomposition Diagnostics

Короткая заметка про diagnostic-API в `Decomposition` и про то, как эти данные используются в `LinearBasis`.

Связанный код:
- [`../../../../CGLibrary/LinearMath/Decomposition.cs`](../../../../CGLibrary/LinearMath/Decomposition.cs)
- [`../../../../CGLibrary/Basics/LinearBasis.cs`](../../../../CGLibrary/Basics/LinearBasis.cs)
- [`../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/Decomposition/DecompositionTests.cs)

## Что добавлено

В `Decomposition` есть два слоя diagnostics.

### 1. Pivot diagnostics для full QR/LQ

Методы:
- `QR_ByHouseholderWithDiagnostics`
- `LQ_ByHouseholderWithDiagnostics`

Возвращают:
- `PivotMagnitudes`
- `RelativePivots`
- `NumericRank`

Смысл:
- `PivotMagnitudes[i] = |R_ii|` или `|L_ii|`
- `RelativePivots[i] = PivotMagnitudes[i] / maxPivot`
- `NumericRank` считается по текущей absolute zero-policy через `Tools.Eps`

Эти данные полезны, когда нужно понять:
- какие pivots реально малы;
- плохая ли это обусловленность в относительном смысле;
- где текущая policy уже начинает ронять численный ранг.

### 2. Quality diagnostics для full QR/LQ

Методы:
- `QR_ByHouseholderWithFullDiagnostics`
- `LQ_ByHouseholderWithFullDiagnostics`

Дополнительно возвращают:
- `ReconstructionError`
- `OrthogonalityError`
- `TriangularLeakage`

Смысл:
- `ReconstructionError` показывает, насколько хорошо факторы восстанавливают исходную матрицу;
- `OrthogonalityError` показывает, насколько близок возвращённый ортогональный множитель к идеальному;
- `TriangularLeakage` показывает, сколько численного мусора осталось вне ожидаемой треугольной структуры.

Последняя метрика особенно важна для scale-sweep кейсов: reconstruction ещё может быть хорошей, а triangular structure уже фактически разрушена.

### 3. Incremental diagnostics для QR/LQ updates

Методы:
- `QR_IncrementalUpdateWithDiagnostics`
- `LQ_IncrementalUpdateWithDiagnostics`

Возвращают:
- `InputNorm`
- `TrailingNorm`
- `Rho`
- `Accepted`

Смысл:
- `TrailingNorm` это норма активного хвоста, по которому текущий шаг пытается построить новый независимый direction;
- `Rho = TrailingNorm / InputNorm`
- `Accepted` показывает, увеличил ли шаг активную размерность.

Для инкрементального построения базиса это самый полезный low-level diagnostic:
- `Rho ~ 1` означает хороший новый direction;
- маленький, но ненулевой `Rho` означает numerically fragile step;
- `Accepted == false` означает, что текущая policy уже схлопнула хвост в ноль.

## Как это используется в LinearBasis

`LinearBasis` и `LinearBasisMutable` используют `LQ_IncrementalUpdateCoreWithDiagnostics` в двух местах:
- при `Orthonormalize`;
- при инкрементальном добавлении вектора в basis storage.

В `DEBUG`-режиме поверх этих данных печатаются предупреждения двух уровней.

### Potentially bad

Шаг принят, но

```text
Rho <= 100 * Tools.Eps
```

Это значит: новый direction ещё не потерян, но уже находится рядом с порогом rejection.

### Definitely bad for the current numerical policy

Шаг не принят при ненулевом входе.

Это не утверждение о математической зависимости векторов. Это более узкий и практический вывод:

- для текущего алгоритма;
- при текущем `Tools.Eps`;
- активный хвост уже стал неотличим от нуля.

То есть библиотека в этой точке уже не может численно отличить exact dependence от very small independent component.

## Практический смысл

Эти diagnostics не заменяют отдельные stability-исследования, но они дают runtime-сигналы в тех местах, где basis-building уже подошёл к границе применимости текущей numeric policy.
