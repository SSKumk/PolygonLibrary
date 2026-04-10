# Linear Algebra References

Источники и algorithm notes для низкоуровневых алгоритмов линейной алгебры.

## Навигация

- [`BasisEqualityStability.md`](./BasisEqualityStability.md)
  Объект: короткая заметка про current `Equals` у `LinearBasis` / `AffineBasis` и про исследование geometric alternative.
  Когда использовать: когда нужно быстро понять, стоит ли отвязывать equality от `CompareTo` и что показало минимальное 3D-исследование.

- [`QRLQHouseholderDecomposition.md`](./QRLQHouseholderDecomposition.md)
  Объект: подробная заметка про `QR_ByHouseholder`, `LQ_ByHouseholder`, `QR_IncrementalUpdate` и `LQ_IncrementalUpdate`.
  Когда использовать: когда нужно понять математическую постановку, Householder-механику и связь между полными разложениями и инкрементальными обновлениями в `Decomposition`.

- [`OrthonormalizeStability.md`](./OrthonormalizeStability.md)
  Объект: короткая заметка про численную устойчивость `LinearBasis.Orthonormalize` и про отличие projector-based и `LQ`-подходов.
  Когда использовать: когда нужно быстро понять, почему текущая projector-based формула плохо ведёт себя на почти зависимых входах с большим span-компонентом.

