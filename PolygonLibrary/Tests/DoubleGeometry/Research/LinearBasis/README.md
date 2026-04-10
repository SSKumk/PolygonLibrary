# LinearBasis Research

Что это: минимальный воспроизводимый research-набор по низкоуровневым вопросам `LinearBasis`.

Когда использовать:
- когда нужно быстро проверить, почему historical projector-based `Orthonormalize` плохо ведёт себя на почти зависимых входах и чем лучше `LQ`-подход;
- когда нужно воспроизвести минимальное сравнение current equality и geometric alternative для `LinearBasis` / `AffineBasis`.

Файлы:
- `OrthonormalizeStabilityResearchTests.cs` - минимальный набор по устойчивости `Orthonormalize`.
- `EqualsStabilityResearchTests.cs` - минимальный набор по current vs geometric equality.
