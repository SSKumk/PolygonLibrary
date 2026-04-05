# MinkowskiSum References

Папка для статей и внешних материалов по алгоритму [`MinkowskiSum.cs`](../../../../../CGLibrary/Algorithms/Polyhedra/MinkowskiSum.cs).

## Источники

- [`!) 2021г. S. Das, S. Swami. A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.pdf`](./!)%202021%D0%B3.%20S.%20Das,%20S.%20Swami.%20A%20Worst-Case%20Optimal%20Algorithm%20to%20Compute%20the%20Minkowski%20Sum%20of%20Convex%20Polytopes.pdf)
  Объект: рабочая статья по алгоритму суммы Минковского, на который ссылается реализация `BySandipDas`.
  Когда использовать: когда нужно сверять шаги алгоритма, леммы и theorem-based условия с кодом `MinkowskiSum`.

## Связь с кодом

- Основная привязка статьи в коде идёт через [`BySandipDas`](../../../../../CGLibrary/Algorithms/Polyhedra/MinkowskiSum.cs), который прямо помечен как реализация по этой работе.
- Внутри метода уже есть явные ссылки на:
  - `Lemma 3`;
  - TODO про конец абзаца перед `Theorem 2`.
- При разборе регрессий по статье в первую очередь нужно смотреть:
  - [`MinkowskiSumBasicTests.cs`](../../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumBasicTests.cs);
  - [`MinkowskiSumHighDimensionalTests.cs`](../../../../../Tests/DoubleGeometry/Algorithms/MinkowskiSum/MinkowskiSumHighDimensionalTests.cs);
  - legacy-набор [`MinkowskiSumTests.cs`](../../../../../Tests/Double-Tests/Minkowski-Tests/MinkowskiSumTests.cs), где уже отмечены article-based примеры.

## Замечания

- В XML-комментарии у [`MinkowskiSum.cs`](../../../../../CGLibrary/Algorithms/Polyhedra/MinkowskiSum.cs) сейчас указан другой состав авторов, чем в имени PDF. Перед дальнейшей стабилизацией алгоритма это стоит отдельно перепроверить по самой статье и затем синхронизировать.
