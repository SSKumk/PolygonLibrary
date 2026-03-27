# Project Tree

Ниже приведено рабочее дерево репозитория с кратким описанием назначения каталогов и ключевых файлов.

Примечание: `bin/`, `obj/`, `.idea/`, `.vscode/` и другие сгенерированные артефакты в дерево не включены.

```text
PolygonLibrary/
├── .ai/                                 # Вспомогательная документация для навигации по репозиторию
│   ├── README.md                        # Назначение папки .ai
│   └── ProjTree.md                      # Это дерево проекта с пояснениями
│
├── CGLibrary.sln                        # Основная solution, объединяющая все проекты
├── AGENTS.md                            # Локальные инструкции для AI-агентов
├── todo.md / TODO.txt                   # Рабочие списки задач и заметки
├── inspectcode.xml                      # Конфигурация статического анализа / инспекций
├── *.dll                                # Внешние локальные зависимости, подключаемые через Reference/HintPath
│
├── CGLibrary/                           # Ядро библиотеки геометрических вычислений
│   ├── CGLibrary.csproj                 # Проект ядра; файлы перечислены вручную
│   ├── GlobalUsings.cs                  # Общие global using-импорты
│   ├── Basics/                          # Базовые математические сущности
│   │   ├── Vector.cs                    # Многомерный вектор
│   │   ├── Vector2D.cs                  # Специализация для 2D
│   │   ├── Matrix.cs                    # Матрицы и матричные операции
│   │   ├── LinearBasis.cs               # Линейные базисы и операции в подпространствах
│   │   ├── AffineBasis.cs               # Аффинные базисы
│   │   ├── HyperPlane.cs                # Гиперплоскости / полупространства
│   │   ├── Line2D.cs                    # Прямая в 2D
│   │   └── CauchyMatrix.cs              # Специализированная матричная логика
│   ├── LinearMath/                      # Алгоритмы линейной алгебры и оптимизации
│   │   ├── GaussSLE.cs                  # Решение СЛАУ методом Гаусса
│   │   ├── SimplexMethod.cs             # Симплекс-метод для линейных ограничений
│   │   ├── FourierMotzkin.cs            # Исключение переменных методом Фурье-Моцкина
│   │   └── Decomposition.cs             # Разложения матриц и связанные процедуры
│   ├── Polygons/                        # 2D-геометрия, полигоны и полилинии
│   │   ├── BasicPolygon.cs              # Общая логика полигона
│   │   ├── Polyline.cs                  # Полилиния
│   │   ├── PolygonTools.cs              # Вспомогательные операции над полигонами
│   │   ├── ConvexPolygons/              # Выпуклые полигоны и dual/support-представления
│   │   │   ├── ConvexPolygon.cs         # Основной класс выпуклого полигона
│   │   │   ├── SupportFunction.cs       # Опорная функция
│   │   │   ├── Intersection.cs          # Пересечение выпуклых полигонов
│   │   │   └── GammaPair.cs             # Вспомогательные dual-структуры
│   │   └── Something/                   # Старые / архивные наработки, не подключены в csproj
│   ├── Polyhedra/                       # Многогранники и многомерная выпуклая геометрия
│   │   └── ConvexPolyhedra/
│   │       ├── ConvexPolytops/
│   │       │   ├── ConvexPolytop.cs     # Главный класс выпуклого многогранника
│   │       │   └── FaceLattice.cs       # Решётка граней
│   │       ├── GiftWrapping/            # Построение convex hull / face lattice по V-rep
│   │       ├── HrepToFLrep.cs           # Переходы между представлениями
│   │       ├── MinkowskiSum.cs          # Сумма Минковского
│   │       └── MinkowskiDiff.cs         # Разность Минковского
│   ├── Segments/                        # Отрезки и операции над ними
│   │   ├── Segment.cs                   # Базовый класс отрезка
│   │   ├── SegmentPair.cs               # Пара отрезков
│   │   └── Bentley-Ottmann/             # Архив / сохранённые реализации и материалы по алгоритму
│   └── Toolkit/                         # Общие вспомогательные утилиты
│       ├── Tools.cs                     # Числовые сравнения, eps, общие helper-методы
│       ├── ParamReader.cs               # Чтение пользовательского текстового формата
│       ├── ParamWriter.cs               # Запись пользовательского текстового формата
│       ├── Convexification.cs           # Вспомогательные алгоритмы выпукления
│       ├── Combinations.cs              # Генерация сочетаний
│       ├── Extensions.cs                # Расширения коллекций и структур
│       ├── Hashes.cs                    # Хеширование и MD5
│       ├── RandomLC.cs                  # Пользовательский генератор случайных чисел
│       └── CollectionOfEnumerables.cs   # Вспомогательные контейнеры/итераторы
│
├── LDG/                                 # Прикладной слой для линейных дифференциальных игр
│   ├── LDG.csproj                       # Библиотека прикладной предметной области
│   ├── GlobalUsing.cs                   # Общие using для проекта LDG
│   ├── PathHolder.cs                    # Работа со структурой каталогов LDG-данных
│   ├── Bridges/                         # Построение мостов игры
│   │   ├── BridgeMain.cs                # Основной orchestrator построения мостов
│   │   ├── GameData.cs                  # Динамика игры и подготовленные данные
│   │   ├── SolverLDG.cs                 # Расчёт сечений мостов
│   │   └── TerminalSet/                 # Чтение и разворачивание разных типов терминальных множеств
│   │       ├── TerminalSet.cs           # Общая оболочка терминальных множеств
│   │       ├── ExplicitTerminalSet.cs   # Явно заданное множество
│   │       ├── MinkowskiTerminalSet.cs  # Множества через операции Минковского
│   │       ├── EpigraphTerminalSet.cs   # Эпиграфы
│   │       └── LevelSetTerminalSet.cs   # Множества уровня
│   ├── PolytopeReader/                  # Чтение многогранников и преобразований из входных файлов
│   │   ├── PolytopeReaderFactory.cs     # Фабрика ридеров
│   │   ├── PolytopeReaders.cs           # Реализации вариантов чтения
│   │   ├── TransformReader.cs           # Чтение аффинных преобразований
│   │   └── Balls/                       # Чтение шаров / специальных объектов
│   └── Trajectories/                    # Построение траекторий в игре
│       ├── TrajectoryMain.cs            # Основной расчёт траектории
│       └── Control/                     # Стратегии управления игроков
│
├── Documentation/                       # Основная документация репозитория
│   └── Development/
│       ├── LibPolytopeFormat.md         # Формат файлов многогранников библиотеки
│       └── LDG/                         # Документация по формату данных и структуре папок LDG
│           ├── DataFormat.md            # Общий формат текстовых конфигурационных файлов
│           ├── FolderStructure.md       # Общая структура корня LDG
│           ├── GameFolderStructure.md   # Структура папки конкретной игры
│           ├── GameHashForming.md       # Формирование контрольных хешей
│           ├── VisualizationConfig.md   # Формат конфигурации визуализации
│           └── IOFormat/                # Форматы конкретных типов файлов
│               ├── Dynamics.md
│               ├── Polytopes.md
│               ├── ProblemConfig.md
│               ├── TerminalSets.md
│               └── TrajectoryConfig.md
│
├── Tests/                               # Тестовый проект и вспомогательные материалы
│   ├── Tests.csproj                     # NUnit-тесты; часть файлов исключена из компиляции
│   ├── TConvertors.cs                   # Конверторы числовых типов для тестов
│   ├── ToolsForTests/                   # Генераторы тестовых многогранников и общие helper’ы
│   │   ├── TestsBase.cs                 # Общие базовые инструменты для тестов
│   │   └── TestsPolytopes.cs            # Наборы тестовых многогранников
│   ├── SharedTests/                     # Тесты базовых математических сущностей
│   │   ├── VectorTests.cs
│   │   ├── MatrixTests.cs
│   │   ├── LinearBasisTests.cs
│   │   ├── AffineBasisTest.cs
│   │   ├── HyperPlaneTests.cs
│   │   ├── FaceLatticeTests.cs
│   │   └── GaussSLETests.cs
│   ├── ToolkitTests/                    # Тесты утилит, в частности ParamReader
│   ├── Double-Tests/                    # Тесты геометрии на double
│   │   ├── ConvexPolygonTests.cs
│   │   ├── ConvexPolygonIntersectionTests.cs
│   │   ├── SegmentCrossTests.cs
│   │   ├── PolygonToolsTests.cs
│   │   ├── QuickHullTests.cs
│   │   ├── PolylineTests.cs
│   │   ├── VectorsTests.cs
│   │   ├── GW_hDTests/                  # Тесты gift wrapping в больших размерностях
│   │   └── Minkowski-Tests/             # Тесты суммы/разности Минковского
│   ├── DoubleDouble-Tests/              # Аналогичные тесты для DoubleDouble
│   ├── OtherTests/                      # Песочница, заметки и тесты для LDG
│   ├── SpeedTests/                      # Производительные / сравнительные тесты
│   ├── Figurs for algorithms/           # Иллюстрации и визуальные материалы к алгоритмам
│   ├── _Store/                          # Архивные версии тестов
│   └── BentlyOttmannTests.cs            # Отдельный тестовый файл; сейчас исключён из компиляции csproj
│
├── Bridges/                             # Консольный проект для запуска расчёта мостов
│   ├── Bridges.csproj
│   ├── Main.cs                          # Точка входа; запускает BridgeCreator из LDG
│   └── Convertor.cs                     # Конверторы числовых типов для запуска
│
├── Trajectories/                        # Консольный проект для расчёта траекторий
│   ├── Trajectories.csproj
│   ├── Main.cs                          # Точка входа для расчёта траекторий
│   └── Convertor.cs                     # Числовые конверторы
│
├── Graphics/                            # Консольный проект визуализации
│   ├── Graphics.csproj
│   ├── Main.cs                          # Точка входа в визуализацию
│   ├── GraphicsMain.cs                  # Основная логика сборки визуализации
│   ├── VisTools.cs                      # Вспомогательные средства визуализации
│   ├── VisualizationColor.cs            # Цветовые настройки
│   ├── Convertor.cs                     # Конверторы числовых типов
│   └── Draw/                            # Низкоуровневые drawer-интерфейсы и реализации
│
├── Profile/                             # Benchmark/профилировочный проект
│   ├── Profile.csproj
│   ├── Profile.cs                       # Точка входа / экспериментальный запуск
│   ├── GlobalUsings.cs
│   ├── Benchmarks/                      # BenchmarkDotNet-бенчмарки по алгоритмам
│   └── Наблюдения и результаты/         # Исследовательские заметки и результаты измерений
│
└── Sandbox/                             # Локальная песочница для быстрых ручных проверок
    ├── Sandbox.csproj
    ├── Sandbox.cs                       # Ручные эксперименты с мостами, вектограммами и многогранниками
    └── Convertor.cs                     # Конверторы числовых типов
```

## Краткая карта по ролям проектов

- `CGLibrary` — математическое и геометрическое ядро.
- `LDG` — предметный слой для линейных дифференциальных игр, построенный поверх `CGLibrary`.
- `Tests` — unit/integration/performance tests и тестовые данные.
- `Bridges`, `Trajectories`, `Graphics`, `Sandbox`, `Profile` — прикладные консольные проекты для расчёта, визуализации, ручных экспериментов и бенчмарков.
- `Documentation` — описание форматов данных и структуры входных/выходных директорий.

## Практические замечания

- В `CGLibrary.csproj` отключены default compile items, поэтому новые `.cs`-файлы ядра нужно добавлять в проект явно.
- В репозитории есть архивные и исследовательские каталоги (`Polygons/Whatever`, `Tests/_Store`, `Segments/Bentley-Ottmann`, заметки в `Profile/`), которые полезны как исторический контекст, но не всегда участвуют в сборке.
- Часть консольных проектов содержит жёстко прописанные локальные пути к данным LDG и служит скорее рабочими entry point’ами, чем “готовыми продуктами”.
