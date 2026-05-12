# Regression And Invariance

## Scope

Сценарии:

- `ConvexPolytop.CreateFromPoints(..., true)` как основной публичный потребитель `GiftWrapping`;
- повороты и сдвиги куба в `3D`;
- inner points на `1D/2D/3D` гранях куба и их комбинациях.
- invariance к перестановке точек во входном swarm.

## Scenarios

| ID | Status | Scenario | Tests |
| --- | --- | --- | --- |
| GW-REG-001 | x | Канонический `Cube3D` через `CreateFromPoints(..., true)` сохраняет множество вершин. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L61) |
| GW-REG-002 | x | `Cube3D`, повернутый на `45°` в `XY`, сохраняет множество вершин. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L68) |
| GW-REG-003 | x | Hand-crafted rotated `Cube3D` сохраняет множество вершин. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L76) |
| GW-REG-004 | x | Hand-crafted shifted `Cube3D` сохраняет множество вершин. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L81) |
| GW-REG-005 | x | Hand-crafted rotated and shifted `Cube3D` сохраняет множество вершин. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L86) |
| GW-REG-006 | x | Inner points на `1D`-гранях `Cube3D` не меняют оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L91) |
| GW-REG-007 | x | Inner points на `2D`-гранях `Cube3D` не меняют оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L98) |
| GW-REG-008 | x | Inner points в `3D` не меняют оболочку `Cube3D`. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L105) |
| GW-REG-009 | x | Inner points на `1D/2D`-гранях `Cube3D` не меняют оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L112) |
| GW-REG-010 | x | Inner points на `2D/3D`-уровнях `Cube3D` не меняют оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L119) |
| GW-REG-011 | x | Inner points на `1D/2D/3D`-уровнях `Cube3D` не меняют оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L126) |
| GW-REG-012 | x | `Cube3D` инвариантен к перестановке входного swarm. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L133) |
| GW-REG-013 | x | `Cube4D` инвариантен к перестановке входного swarm. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L140) |
| GW-REG-014 | x | `Simplex3D` инвариантен к перестановке входного swarm. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L147) |
| GW-REG-015 | x | `Simplex4D` инвариантен к перестановке входного swarm. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L154) |
| GW-REG-016 | x | `Cube4D` с inner points на `1D`-гранях сохраняет оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L161) |
| GW-REG-017 | x | `Cube4D` с inner points на `2D`-гранях сохраняет оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L168) |
| GW-REG-018 | x | `Cube4D` с inner points на `3D`-гранях сохраняет оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L175) |
| GW-REG-019 | x | `Cube4D` с inner points на `1D/2D`-уровнях сохраняет оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L182) |
| GW-REG-020 | x | `Cube4D` с inner points на `2D/3D`-уровнях сохраняет оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L189) |
| GW-REG-021 | x | `Cube4D` с inner points на `1D/2D/3D`-уровнях сохраняет оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L196) |
| GW-REG-022 | x | `Cube4D` с inner points на `1D/2D/3D/4D`-уровнях сохраняет оболочку. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L203) |
| GW-REG-023 | x | Hand-crafted `Simplex4D` с точками на ребре и соседних `2D`-гранях сохраняет множество вершин. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L210) |
| GW-REG-024 | x | Hand-crafted `Simplex4D` с inner points на `1D` сохраняет множество вершин и после shuffle. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L239) |
| GW-REG-025 | x | Косой `3D`-параллелепипед инвариантен к перестановке входного swarm. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L275) |
| GW-REG-026 | x | Hand-crafted `3D` polytope из legacy сохраняет множество вершин. | [`GiftWrappingRegressionTests.cs`](../../../../Tests/DoubleGeometry/Algorithms/GiftWrapping/GiftWrappingRegressionTests.cs#L295) |

## Existing Tests

- Этот слой перенесён из legacy `GW_Tests`, где проверка шла в основном через `ConvexPolytop.CreateFromPoints(..., true)`.
- Здесь сохранён именно этот интеграционный контракт, а не только прямой вызов `GiftWrapping.WrapVRep`.

## Gaps

- Heavy random-наборы ещё не перенесены в отдельный stress-слой.
