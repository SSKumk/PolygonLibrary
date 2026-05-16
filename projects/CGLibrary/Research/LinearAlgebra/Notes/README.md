# LinearAlgebra Notes

Внутренние алгоритмические заметки по low-level линейной алгебре текущей реализации `CGLibrary`.

## Материалы

- [`LinearBasis.md`](./LinearBasis.md)
  Объект: заметка о представлении `LinearBasis` и его алгоритмическом смысле.
  Когда использовать: когда нужно понять текущее решение по хранению basis-данных.

- [`BasisEqualityStability.md`](./BasisEqualityStability.md)
  Объект: наблюдения по устойчивости сравнения базисов.
  Когда использовать: когда нужно восстановить контекст по equality/stability-сценариям `LinearBasis`.

- [`QRLQHouseholderDecomposition.md`](./QRLQHouseholderDecomposition.md)
  Объект: заметка по QR/LQ-разложениям на отражениях Хаусхолдера.
  Когда использовать: когда нужно сверить внутреннюю логику low-level decomposition-слоя.

- [`DecompositionDiagnostics.md`](./DecompositionDiagnostics.md)
  Объект: diagnostic-наблюдения по decomposition-алгоритмам.
  Когда использовать: когда нужно понять, какие численные ситуации уже исследовались.

- [`QRStability.md`](./QRStability.md)
  Объект: заметка по устойчивости QR-разложения.
  Когда использовать: когда нужно сверить известные stability-границы текущей реализации.

- [`OrthonormalizeStability.md`](./OrthonormalizeStability.md)
  Объект: заметка по устойчивости ортонормализации.
  Когда использовать: когда нужно восстановить контекст по `Orthonormalize`-сценариям.
