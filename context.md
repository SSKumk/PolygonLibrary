# Context

Файл хранит краткое состояние работы для продолжения задачи после сбоя сжатия контекста.

## Текущая ветка

- Репозиторий: `F:\Works\IMM\Аспирантура\_PolygonLibrary`.
- Ветка: `2026-05-CleanUp-with-Codex`.
- Базовый коммит на момент создания файла: `96ef74cc Mark active project boundary complete`.

## Правила работы

- Основная рабочая папка: `active/CGLibrary/`.
- Перед изменениями в активном проекте читать `active/CGLibrary/README.md`.
- Правила документации: `active/CGLibrary/Documentation/DocumentationConventions.md`.
- Правила разработки: `active/CGLibrary/Documentation/Development/README.md`.
- Структура проекта: `active/CGLibrary/Documentation/Development/RepoStructure.md`.
- Тесты запускать только с явного разрешения пользователя.
- Не менять production-код для задач по документации или структуре.

## Что уже сделано

- Активный проект перенесён из `PolygonLibrary/` в `active/CGLibrary/`.
- Слои репозитория зафиксированы в `.ai/ACTIVE_PROJECT_BOUNDARY.md`.
- `AGENTS.md` переведён на новый путь `active/CGLibrary/`.
- `.gitignore` обновлён под новый путь активного проекта.
- Пункт 1 в `.ai/GLOBAL_CLEANUP_QUESTIONS.md` отмечен как выполненный.

## Коммиты текущей серии

- `17246eb0 Move active project into active directory`.
- `d10ccc89 Document active project boundary`.
- `96ef74cc Mark active project boundary complete`.

## Проверка после переноса

Быстрые `double`-тесты активного проекта прошли:

- `Basics`: 330 passed.
- `Polygons`: 99 passed.
- `Polyhedra`: 94 passed.
- `Toolkit`: 38 passed.
- `TestInfrastructure`: 13 passed.
- быстрые `Algorithms`: 127 passed.

`Research` и `Stress`-тесты не запускались.

## Worktree

Случайно созданный worktree `C:\Users\anton\.codex\worktrees\7ea0\_PolygonLibrary` снят с учёта Git.

Папка на диске пока не удалена: Windows сообщает, что путь используется другим процессом. Это не влияет на основной репозиторий.

## Следующий вопрос

Следующий пункт плана: расширить пункт 2 в `.ai/GLOBAL_CLEANUP_QUESTIONS.md`.

Нужно обсудить и зафиксировать модель документационной карты:

- что документируется в корневом `README.md`;
- что живёт в `active/CGLibrary/README.md`;
- что считается постоянной документацией в `active/CGLibrary/Documentation/`;
- что остаётся рабочей памятью в `.ai/`;
- что не документируется, чтобы не плодить дубли и устаревшие тексты.

Предварительное направление: не писать сразу корневой `README.md`, а сначала расширить рабочий вопрос пункта 2 и, при необходимости, завести отдельный рабочий файл `.ai/DOCUMENTATION_MAP.md`.

## Как обновлять этот файл

Обновлять файл перед длинными переходами контекста, перед рискованной сменой задачи и после важных коммитов.
