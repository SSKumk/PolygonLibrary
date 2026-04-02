# Current Contract

## Scope

Сценарии:

- индексатор в момент `T`;
- нулевая, диагональная и нильпотентная матрицы;
- частичный последний шаг, когда `|t - tCur| < dt`, в обоих направлениях.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CM-001 | x | В опорный момент `T` индексатор возвращает единичную матрицу. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L9) |
| CM-002 | x | Для нулевой матрицы фундаментальная матрица тождественно равна единичной при любом времени. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L16) |
| CM-003 | ~ | Для диагональной матрицы результат совпадает с точным экспоненциальным решением на положительном и отрицательном времени. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L24) |
| CM-004 | x | Для нильпотентной матрицы корректно отрабатывает частичный последний шаг при `t > T` и совпадает с замкнутой формулой. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L42) |
| CM-005 | x | Для нильпотентной матрицы корректно отрабатывает частичный последний шаг при `t < T` и совпадает с замкнутой формулой. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L55) |

## Existing Tests

- Раньше прямого тестового слоя на `CauchyMatrix` не было.
- Новый набор фиксирует математически контролируемые случаи без тяжёлых численных экспериментов.

## Gaps

- Отдельные долгие численные regression-сценарии можно добавлять позже, если понадобится проверка накопления ошибки на больших промежутках.

## Notes

- По состоянию на ревизию `2026-04-02` сценарий `CM-003` покрыт тестом, но текущая реализация его не проходит из-за погрешности точности RK4 на жёстком tolerance.
