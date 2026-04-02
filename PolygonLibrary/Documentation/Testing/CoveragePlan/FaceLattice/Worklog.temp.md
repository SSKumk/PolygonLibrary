# FaceLattice Worklog

Временный рабочий файл для синхронизации решений по `FaceLattice`, `FLNode` и связанным internal-типам.

Правило работы с файлом:

- сюда заносятся открытые вопросы, договорённости, гипотезы и результаты локальной ревизии;
- после принятия очередной пачки правок закрытые вопросы удаляются;
- файл временный и нужен только на период активной переработки класса.

## Current State

- Текущий узкий прогон `FaceLattice`: `17` total, `17` passed, `0` failed.
- Сценарий `Constructor_FromSubNodes_WithExplicitAffBasis` закрыт через правку copy ctor `AffineBasis`.

## Agreed Positions

### AP-001 Empty Subnodes

- Status: `implemented_in_docs`
- User position:
  - Пустой набор `sub` считается нарушением preconditions.
  - В release-ветке специально проверять это не нужно.
  - В XML это можно зафиксировать явно.
- Assistant notes:
  - Это согласуется с остальной идеологией библиотеки.
  - Для тестов такой сценарий не должен входить в обязательный runtime-контракт.
  - Имеет смысл явно прописать precondition в XML у `FLNode(IEnumerable<FLNode> sub, ...)`.
  - Отдельный debug-check на пустой `sub` всё равно может быть полезен для раннего обнаружения ошибок построителя.

### AP-002 Incorrect Hierarchy Validation

- Status: `open`
- User position:
  - В release-режиме дополнительные рантайм-проверки нежелательны.
  - При этом допустим debug-only checker корректности структуры.
- Assistant notes:
  - Это, на мой взгляд, лучший компромисс для `FaceLattice`.
  - Возможный формат:
    - отдельный internal/static checker;
    - вызов только под `#if DEBUG` или из `Debug.Assert`;
    - проверка размерностей, согласованности `Sub/Super`, совместимости `Vertices` и `AffBasis`, отсутствия пустых уровней и т.п.
  - Такой checker полезен не только для `FLNode`, но и для `FaceLattice`, `FLNodeSum`, `ConstructFromFLNodeSum`, `ConstructFromBaseSubCP`.
  - Сейчас это выглядит как основной кандидат на системное усиление корректности без потери release-производительности.

### AP-003 Equals vs CompareTo Consistency

- Status: `implemented_in_tests`
- User position:
  - Можно писать тест на корректное согласование `Equals` и `CompareTo`.
  - Некорректное представление тут отдельно не определено.
  - Текущее упрощение в `FLNode` было сделано намеренно ради скорости.
- Assistant notes:
  - Тест на корректный случай точно нужен.
  - Особенно важно для `SortedSet<FLNode>`, потому что там `CompareTo == 0` фактически задаёт идентичность элемента в контейнере.
  - При текущем дизайне полезно явно закрепить хотя бы инвариант:
    - для корректно построенных узлов `CompareTo(other) == 0` эквивалентно `Equals(other)`.

### AP-004 FaceLattice Equality Ordering Robustness

- Status: `implemented_in_tests`
- User position:
  - Добавить тест.
- Assistant notes:
  - Нужен сценарий, где одна и та же решётка построена с иным порядком узлов на уровнях.
  - Это должен быть чистый тест на инвариантность к порядку хранения.
  - Заодно стоит отдельно проверить инвариантность к порядку `sub` при построении одного и того же `FLNode`.

### AP-005 Level Cache Mutability

- Status: `open`
- User position:
  - В библиотеке по умолчанию предполагается неизменяемость объектов, если не сказано иного.
  - Первичный приоритет здесь у скорости, а не у "безопасности" API.
  - В текущем коде есть места, где клоны `SortedSet` нужны именно для быстрых set-операций, например:
    - `new SortedSet<FLNode>(xi.Super)`
    - затем `IntersectWith(...)`
  - Поэтому `SortedSet` здесь лучше сохранять.
- Assistant notes:
  - Прямых setter'ов нет, но `Sub` и `Super` публично доступны как изменяемые `SortedSet<FLNode>`.
  - После первого обращения к `LevelNodes` кэш уже построен и не инвалидируется.
  - Согласованное направление:
    - `SortedSet` сохраняем;
    - структуру считаем immutable-by-convention после построения;
    - если понадобятся дополнительные страховки, то скорее debug-only, без замедления release-кода и без ухудшения set-операций.

### AP-006 Internal Coverage Scope

- Status: `open`
- User position:
  - Internal helper'ы тоже нужно тестировать.
  - Формулировки про "обязательный минимум" надо убрать.
  - Цель: протестировать библиотеку целиком снизу вверх.
- Assistant notes:
  - Полностью согласен.
  - Для `FaceLattice` это означает, что в coverage backlog надо добавить:
    - `FLNodeSum`
    - `ConstructFromFLNodeSum`
    - `ConstructFromBaseSubCP`
  - Вероятно, их стоит оформить отдельным topical-файлом, а не смешивать с основным public API.
  - Да, отдельная документация под internal-тесты тоже нужна.

## Newly Identified Gaps

### GAP-001 Explicit AffineBasis Contract

- Status: `implemented_variant_1`
- Observation:
  - `FLNode(IEnumerable<FLNode> sub, AffineBasis affBasis)` сейчас логически обещает "использовать переданный базис".
  - Фактически он падает на обычном `AffineBasis`, если внутри него mutable linear basis.
- Agreed direction:
  - Конструктор должен принимать "безопасный и верный" базис пространства, задаваемого `sub`.
- Assistant notes:
  - Реализован минимальный вариант:
    - `AffineBasis(AffineBasis, needCopy: false)` разрешает zero-copy для обычного `AffineBasis`;
    - тот же путь остаётся запрещённым для `AffineBasisMutable`.
  - Более глубокое архитектурное разведение immutable/mutable слоёв вынесено в `todo.md`.

### GAP-002 Invalid Graph Scenarios

- Status: `open`
- Observation:
  - Сейчас не описан контракт для случаев:
    - пустой `sub`;
    - подузлы разной размерности;
    - `affBasis`, не содержащий вершины `sub`;
    - несогласованные ссылки `Sub/Super`.
- Decision pending:
  - Что из этого считаем pure preconditions, а что хотим уметь ловить в debug-checker.
- Agreed direction:
  - Для release это preconditions.
  - В debug-режиме такие нарушения желательно ловить.

### GAP-003 Internal Builders

- Status: `open`
- Observation:
  - `ConstructFromFLNodeSum`, `ConstructFromBaseSubCP` и `FLNodeSum` пока не имеют собственного прямого тестового слоя.
- User position:
  - Пока оставляем этот блок в backlog без немедленных правок.

## Candidate Next Steps

1. Зафиксировать XML/precondition у `FLNode(IEnumerable<FLNode> sub, ...)`.
2. Добавить coverage-план для internal helper'ов `FaceLattice`.
3. Добавить тесты на:
   - согласованность `FLNode.Equals` и `CompareTo` на корректных данных;
   - `FaceLattice.Equals` при ином порядке узлов на уровне.
4. Отдельно обсудить реализацию контракта `FLNode(..., AffineBasis affBasis)` до изменения production-кода.
5. Решить, нужен ли debug-only integrity checker и на каком уровне его вызывать.

## Assistant Thoughts

- Самая важная текущая развилка не в конкретном баге, а в выборе инвариантов:
  - что делает лицо "тем же самым лицом";
  - что может считаться корректной решёткой;
  - допускается ли внешняя мутация графа после построения.
- Пока это не проговорено, исправление одиночного падения легко сделать локально, но можно промахнуться по архитектуре.

## User Notes

- 
- 
- 


## Decision Log

- `AP-005`: приоритет у скорости; `SortedSet` и быстрые set-операции сохраняются, структура пока считается immutable-by-convention.
- `AP-001`: precondition про непустой `sub` зафиксирован в XML `FLNode(IEnumerable<FLNode> sub, ...)`.
- `AP-003`: тест на согласованность `FLNode.Equals` и `CompareTo` для корректно построенных узлов добавлен.
- `AP-004`: тест на инвариантность `FaceLattice.Equals` к эквивалентному порядку построения добавлен.
- `GAP-001`: принят и реализован вариант 1: zero-copy копирование разрешено для обычного `AffineBasis`, но не для `AffineBasisMutable`.
