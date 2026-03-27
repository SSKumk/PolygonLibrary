# Current Contract

## Scope

Сценарии:

- индексатор в момент `T`;
- нулевая, диагональная и нильпотентная матрицы;
- частичный последний шаг, когда `|t - tCur| < dt`.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| CM-001 | x | В опорный момент `T` индексатор возвращает единичную матрицу. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L9) |
| CM-002 | x | Для нулевой матрицы фундаментальная матрица тождественно равна единичной при любом времени. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L16) |
| CM-003 | x | Для диагональной матрицы результат совпадает с точным экспоненциальным решением на положительном и отрицательном времени. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L24) |
| CM-004 | x | Для нильпотентной матрицы корректно отрабатывает частичный последний шаг и совпадает с замкнутой формулой. | [`CauchyMatrixTests.cs`](../../../../Tests/DoubleGeometry/Basics/CauchyMatrix/CauchyMatrixTests.cs#L42) |

## Existing Tests

- Раньше прямого тестового слоя на `CauchyMatrix` не было.
- Новый набор фиксирует математически контролируемые случаи без тяжёлых численных экспериментов.

## Gaps

- Отдельные долгие численные regression-сценарии можно добавлять позже, если понадобится проверка накопления ошибки на больших промежутках.

