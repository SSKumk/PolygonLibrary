# Linear Algebra References

Источники и algorithm notes для низкоуровневых алгоритмов линейной алгебры.

## Навигация

- [`LinearBasis.md`](./LinearBasis.md)
  Объект: базовая заметка про представление `LinearBasis`, его инварианты и связь с полным ортогональным оператором.
  Когда использовать: когда нужно быстро понять, как в библиотеке устроен линейный базис и почему он хранится как строки полного ортогонального оператора.

- [`BasisEqualityStability.md`](./BasisEqualityStability.md)
  Объект: короткая заметка про current `Equals` у `LinearBasis` / `AffineBasis` и про исследование geometric alternative.
  Когда использовать: когда нужно быстро понять, стоит ли отвязывать equality от `CompareTo` и что показало минимальное 3D-исследование.

- [`QRLQHouseholderDecomposition.md`](./QRLQHouseholderDecomposition.md)
  Объект: подробная заметка про `QR_ByHouseholder`, `LQ_ByHouseholder`, `QR_IncrementalUpdate` и `LQ_IncrementalUpdate`.
  Когда использовать: когда нужно понять математическую постановку, Householder-механику и связь между полными разложениями и инкрементальными обновлениями в `Decomposition`.

- [`QRStability.md`](./QRStability.md)
  Объект: короткая заметка про численные границы применимости `QR_ByHouseholder` при текущем `Tools.Eps`.
  Когда использовать: когда нужно быстро понять, где QR ещё годится как factorization, но уже плохо годится как rank-revealing procedure.

- [`OrthonormalizeStability.md`](./OrthonormalizeStability.md)
  Объект: короткая заметка про численную устойчивость `LinearBasis.Orthonormalize` и про отличие projector-based и `LQ`-подходов.
  Когда использовать: когда нужно быстро понять, почему текущая projector-based формула плохо ведёт себя на почти зависимых входах с большим span-компонентом.

