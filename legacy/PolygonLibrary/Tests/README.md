# Legacy Tests Data

Исторические тестовые данные и старые наборы, которые не входят в активный тестовый слой `projects/CGLibrary/Tests`.

## Состав

- `Double-Tests/`
- `DoubleDouble-Tests/`
- `LegacyMisc/`
  Объект: исторические тестовые заготовки, не входящие в активный слой.
  Когда использовать: когда нужно восстановить контекст старых сценариев перед проектированием новых тестов.

### LegacyMisc

- [`LegacyMisc/BentlyOttmannTests.cs`](./LegacyMisc/BentlyOttmannTests.cs)
  Объект: исторический reference-сценарий для будущих тестов Bentley-Ottmann.
  Когда использовать: когда нужно сверить старую идею тестирования sweep-line до построения нового активного набора.

`TXTtoOBJ-convertor` сохранён внутри соответствующих наборов в `Minkowski-Tests/3D-pictures/`.
