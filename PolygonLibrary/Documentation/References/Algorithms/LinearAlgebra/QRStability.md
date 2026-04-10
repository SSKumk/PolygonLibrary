# QR Stability

Эта заметка фиксирует минимальный вывод по численной устойчивости `Decomposition.QR_ByHouseholder(...)` при текущей absolute zero-policy через `Tools.Eps`.

Связанный код:
- [`../../../../CGLibrary/LinearMath/Decomposition.cs`](../../../../CGLibrary/LinearMath/Decomposition.cs)
- [`QRLQHouseholderDecomposition.md`](./QRLQHouseholderDecomposition.md)

## Что исследовалось

Использовались два минимально репрезентативных семейства.

### 1. Scale sweep

```text
A(s) = s * A0,
```

где `A0` получена как `Q0 * R0` для умеренно невырожденной upper-triangular `R0`.

Это семейство показывает, где абсолютный `Tools.Eps` уже мешает самому построению Householder-reflector'ов.

### 2. Near-rank-deficient sweep

```text
R(delta, scale) =
[ scale, scale       ]
[ 0    , scale*delta ]
[ 0    , 0           ]

A(delta, scale) = Q0 * R(delta, scale).
```

При `delta > 0` точный ранг всё ещё равен `2`, но второй pivot контролируемо мал.

Это позволяет отделить:
- устойчивость самого QR-разложения как factorization;
- пригодность `QR` как practical rank-revealing procedure.

## Обусловленность исследуемых семейств

Для текущих семейств число обусловленности полезно, но только как дополнительная ось.

### Scale sweep

Для

```text
A(s) = s * A0
```

двухнорменное число обусловленности не меняется:

```text
cond_2(A(s)) = cond_2(A0).
```

Поэтому scale sweep показывает не рост обусловленности, а именно влияние абсолютной `Tools.Eps`-policy.

### Near-rank-deficient sweep

Для

```text
R(delta, scale) =
[ scale, scale       ]
[ 0    , scale*delta ]
[ 0    , 0           ]
```

из-за левого ортогонального множителя `Q0` имеем

```text
cond_2(A(delta, scale)) = cond_2(R(delta, scale)).
```

Выносим `scale`:

```text
R(delta, scale) = scale *
[ 1, 1     ]
[ 0, delta ]
[ 0, 0     ].
```

Значит число обусловленности не зависит от `scale` и определяется только `delta`.

Для матрицы

```text
B(delta) =
[ 1, 1     ]
[ 0, delta ]
```

имеем

```text
B^T B =
[ 1, 1         ]
[ 1, 1+delta^2 ]
```

с собственными значениями

```text
lambda_± = ((2 + delta^2) ± sqrt(4 + delta^4)) / 2.
```

Отсюда

```text
cond_2(B(delta)) = sqrt(lambda_+ / lambda_-)
                  = lambda_+ / |delta|
                  ~ 2 / |delta|    при delta -> 0.
```

То есть near-rank-deficient семейство становится плохо обусловленным именно как `1 / delta`, но scale shift сам по себе число обусловленности не меняет.

## Репрезентативные числа

### Scale sweep

| `s` | `max abs(Q*R-A)` | `max abs(Q^TQ-I)` | `max abs(R_ij, i>j) / s` | Интерпретация |
| --- | ---: | ---: | ---: | --- |
| `1e-7` | `1.06e-22` | `5.55e-16` | `2.65e-16` | Нормальный режим: и разложение, и triangular form устойчивы. |
| `1e-8` | `3.31e-24` | `2.22e-16` | `5.00e-1` | Реконструкция ещё хорошая, но triangular structure уже численно ненадёжна. |
| `1e-9` | `0` | `0` | `2.12` | Практически вырожденный режим: получается почти `Q = I`, `R = A`. |

### Near-rank-deficient sweep

| `scale` | `delta` | ожидаемый второй pivot `scale*delta` | `double` numeric rank | `abs(R[1,1]) / (scale*delta)` | Интерпретация |
| --- | ---: | ---: | ---: | ---: | --- |
| `1` | `1e-7` | `1e-7` | `2` | `1.00` | Второй pivot ещё уверенно различим. |
| `1` | `1e-8` | `1e-8` | `1` | `9.66e-1` | На уровне `Tools.Eps` QR уже роняет численный ранг, хотя сам pivot ещё близок к ожидаемому. |
| `1e4` | `1e-12` | `1e-8` | `1` | `9.66e-1` | Та же граница в scale-shifted форме: решает абсолютный размер второго pivot. |
| `1e8` | `1e-14` | `1e-6` | `2` | `9.94e-1` | При pivot существенно больше `Tools.Eps` разложение и rank detection ещё работают. |

## Вывод

`QR_ByHouseholder` пригоден как factorization существенно дальше, чем как rank-revealing procedure.

Практическая граница сейчас задаётся не только арифметикой Householder, а всей текущей zero-policy:
- при абсолютном масштабе порядка `Tools.Eps` алгоритм ещё может хорошо реконструировать `A`, но уже теряет надёжную triangular form;
- для почти вырожденных входов решающим оказывается абсолютный размер второго pivot `scale * delta`;
- когда этот pivot опускается к `Tools.Eps`, `double` начинает ронять numeric rank, хотя сам diagonal entry в `R` ещё остаётся близким к ожидаемому.

Число обусловленности здесь полезно как пояснение only для near-rank-deficient семейства:
- `cond_2` растёт примерно как `2 / delta`;
- но scale sweep показывает, что одной обусловленности недостаточно для объяснения срыва;
- practically границу в текущем коде определяют вместе и плохая обусловленность, и абсолютный размер малого pivot относительно `Tools.Eps`.

Иными словами: при текущем контракте QR в `double` стоит использовать как factorization для умеренно масштабированных матриц, но не как сильный rank detector у границы `Tools.Eps`.

## Воспроизводимость

Минимальный воспроизводимый набор сохранён в research-слое:

- [`../../../../Tests/DoubleGeometry/Research/Decomposition/README.md`](../../../../Tests/DoubleGeometry/Research/Decomposition/README.md)
- [`../../../../Tests/DoubleGeometry/Research/Decomposition/QRStabilityResearchTests.cs`](../../../../Tests/DoubleGeometry/Research/Decomposition/QRStabilityResearchTests.cs)
