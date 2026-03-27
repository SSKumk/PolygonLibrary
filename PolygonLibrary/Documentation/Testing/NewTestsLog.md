# New Tests Log

Этот файл нужен как короткий журнал миграции: какие новые тестовые файлы появились для каждого класса и какой legacy-источник был заменён.

Формат записи:

- класс;
- новые тестовые файлы с кратким пояснением в скобках, какую функциональность они покрывают;
- legacy-файл, если он был;
- короткая заметка.

## Entries

### Line2D

- New tests:
  - [`Line2DAssert.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DAssert.cs) (`Vector2D`-ассерты и инварианты прямой)
  - [`Line2DConstructionTests.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DConstructionTests.cs) (конструкторы, фабрики, ориентация полуплоскости и инварианты)
  - [`Line2DQueriesAndIntersectionTests.cs`](../../Tests/DoubleGeometry/Basics/Line2D/Line2DQueriesAndIntersectionTests.cs) (индексатор, `PassesThrough`, `Reorient`, пересечение прямых)
- Legacy:
  - отдельного legacy-файла не было
- Note:
  - пилотный перенос, на котором был зафиксирован формат новой структуры

### Vector2D

- New tests:
  - [`Vector2DAssert.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DAssert.cs) (сравнение `Vector2D` с учётом `Eps`)
  - [`Vector2DConstructionAndComparisonTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DConstructionAndComparisonTests.cs) (конструкторы, константы, индексатор, сравнение, `ToString`, `PolarAngle`)
  - [`Vector2DArithmeticAndAnglesTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DArithmeticAndAnglesTests.cs) (арифметика, нормы, повороты, углы, `FromPolar`)
  - [`Vector2DRelationsAndCombinationsTests.cs`](../../Tests/DoubleGeometry/Basics/Vector2D/Vector2DRelationsAndCombinationsTests.cs) (отношения между векторами, `IsBetween`, линейные комбинации, приведение из `Vector`)
- Legacy:
  - [`VectorsTests.cs`](../../Tests/Double-Tests/VectorsTests.cs)
- Note:
  - сохранены исторические поясняющие сообщения и группировка сценариев для `AngleTest` и `IsBetweenTest`

### Segment

- New tests:
  - [`SegmentAssert.cs`](../../Tests/DoubleGeometry/Basics/Segment/SegmentAssert.cs) (ассерты для `Vector2D` и геометрии результата пересечения)
  - [`SegmentConstructionAndGeometryTests.cs`](../../Tests/DoubleGeometry/Basics/Segment/SegmentConstructionAndGeometryTests.cs) (конструкторы, геометрические свойства, сравнение, `Equals`, `ToString`)
  - [`SegmentPointQueriesTests.cs`](../../Tests/DoubleGeometry/Basics/Segment/SegmentPointQueriesTests.cs) (точечные запросы, `ContainsPoint`, `IsEndPoint`, `IsInnerPoint`, `ComputeAtPoint`)
  - [`SegmentIntersectionTests.cs`](../../Tests/DoubleGeometry/Basics/Segment/SegmentIntersectionTests.cs) (пересечение отрезков, `CrossInfo`, типы и позиции точек пересечения)
- Legacy:
  - [`SegmentCrossTests.cs`](../../Tests/Double-Tests/SegmentCrossTests.cs)
- Note:
  - сохранён исторический текст сообщений из `SegmentContainsPointTest`; старый закомментированный черновик по `Intersect` заменён активным системным набором тестов

### Intersection

- New tests:
  - [`ConvexPolygonIntersectionTestBase.cs`](../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionTestBase.cs) (общие тестовые данные, циклическое сравнение вершин и симметричная проверка `IntersectionPolygon`)
  - [`ConvexPolygonIntersectionBasicCasesTests.cs`](../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBasicCasesTests.cs) (обычные непустые пересечения, симметрия результата и инвариантность к циклическому сдвигу вершин)
  - [`ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs`](../../Tests/DoubleGeometry/Basics/Intersection/ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests.cs) (вложение, пустое пересечение, касания, вырожденные пересечения и `null`-входы)
- Legacy:
  - [`ConvexPolygonIntersectionTests.cs`](../../Tests/Double-Tests/ConvexPolygonIntersectionTests.cs)
- Note:
  - сохранены исторические диагностические сообщения `Intersection..` и вынесен отдельный недостающий тест на `null`-аргументы

### SegmentPair

- New tests:
  - [`SegmentPairConstructionAndComparisonTests.cs`](../../Tests/DoubleGeometry/Basics/SegmentPair/SegmentPairConstructionAndComparisonTests.cs) (нормализация порядка пары, `CompareTo` и совместимость с `SortedSet`)
- Legacy:
  - прямого legacy-файла не было; раньше покрытие было только косвенным через [`BentlyOttmannTests.cs`](../../Tests/BentlyOttmannTests.cs)
- Note:
  - для `SegmentPair` добавлен короткий прямой unit-like набор вместо косвенной проверки через большой sweep-line сценарий; по дороге класс был возвращён в активную сборку `CGLibrary`

### Tools

- New tests:
  - [`ToolsComparisonsTests.cs`](../../Tests/DoubleGeometry/Basics/Tools/ToolsComparisonsTests.cs) (управление `Eps`, производная `EpsG`, приближённые сравнения, `CMP`, `Sign` и `TNumComparer`)
  - [`ToolsMathAndUtilitiesTests.cs`](../../Tests/DoubleGeometry/Basics/Tools/ToolsMathAndUtilitiesTests.cs) (числовые константы, инициализация массивов, `Swap`, `Atan2`, `Abs`, `GetCombinations`)
- Legacy:
  - отдельного прямого legacy-файла не было
- Note:
  - для `Tools` добавлен первый прямой базовый набор, который фиксирует численные границы по `Eps`, а не только косвенное использование через другие классы

### Vector

- New tests:
  - [`VectorAssert.cs`](../../Tests/DoubleGeometry/Basics/Vector/VectorAssert.cs) (локальные helper-ы для создания и сравнения `Vector`)
  - [`VectorConstructionAndIdentityTests.cs`](../../Tests/DoubleGeometry/Basics/Vector/VectorConstructionAndIdentityTests.cs) (конструкторы, copy/no-copy семантика, доступ к координатам, массивные представления и базовые инварианты)
  - [`VectorComparisonAndFormattingTests.cs`](../../Tests/DoubleGeometry/Basics/Vector/VectorComparisonAndFormattingTests.cs) (лексикографическое сравнение, операторы, `Equals`, `GetHashCode`, строковое представление и инвариантная культура)
  - [`VectorArithmeticTests.cs`](../../Tests/DoubleGeometry/Basics/Vector/VectorArithmeticTests.cs) (арифметические операторы, скалярное произведение, `Sum`, линейные комбинации, `MulByNumAndAdd`, `AffMul`)
  - [`VectorGeometryTests.cs`](../../Tests/DoubleGeometry/Basics/Vector/VectorGeometryTests.cs) (длины, нормировка, углы, параллельность, ортогональность, `OuterProduct`, `SubVector`, `LiftUp`, `ProjectTo2DAffineSpace`)
  - [`VectorFactoriesAndGenerationTests.cs`](../../Tests/DoubleGeometry/Basics/Vector/VectorFactoriesAndGenerationTests.cs) (стандартные фабрики и детерминированные сценарии для генераторов)
- Legacy:
  - [`VectorTests.cs`](../../Tests/SharedTests/VectorTests.cs)
- Note:
  - сохранены полезные диагностические сообщения из legacy-набора для нормировки, скалярного произведения и граничных сценариев `Angle`; дополнительно закрыты прямые сценарии для `IEnumerable<int>`, `Vector2D`, явного приведения к массиву, `Sum` и генераторов

### Matrix

- New tests:
  - [`MatrixAssert.cs`](../../Tests/DoubleGeometry/Basics/Matrix/MatrixAssert.cs) (локальные helper-ы для создания матриц и проверки RREF)
  - [`MatrixConstructionAndAccessTests.cs`](../../Tests/DoubleGeometry/Basics/Matrix/MatrixConstructionAndAccessTests.cs) (конструкторы, copy/no-copy семантика, индексаторы и приведения)
  - [`MatrixComparisonAndFormattingTests.cs`](../../Tests/DoubleGeometry/Basics/Matrix/MatrixComparisonAndFormattingTests.cs) (`Equals`, `CompareTo`, `GetHashCode` и `ToString`)
  - [`MatrixArithmeticTests.cs`](../../Tests/DoubleGeometry/Basics/Matrix/MatrixArithmeticTests.cs) (арифметические операторы, умножения на матрицы и векторы, `MultiplyBySelfTranspose`)
  - [`MatrixStructureAndExtractionTests.cs`](../../Tests/DoubleGeometry/Basics/Matrix/MatrixStructureAndExtractionTests.cs) (`hcat`, `vcat`, семейство `Take*` и `Transpose`)
  - [`MatrixLinearOperationsTests.cs`](../../Tests/DoubleGeometry/Basics/Matrix/MatrixLinearOperationsTests.cs) (фабрики, генераторы, линейные функции и `ToRREF`)
  - [`MatrixMutableTests.cs`](../../Tests/DoubleGeometry/Basics/Matrix/MatrixMutableTests.cs) (контракт `MatrixMutable`, мутации, вставка блоков и умножение)
- Legacy:
  - [`MatrixTests.cs`](../../Tests/SharedTests/MatrixTests.cs)
- Note:
  - сохранены содержательные диагностические сообщения из legacy-набора для copy/no-copy сценариев, `ToString` и эталонных случаев `ToRREF`; заодно coverage-план приведён к реальному API `TakeRows`, `TakeCols` и `TakeSubMatrix`

### LinearBasis

- New tests:
  - [`LinearBasisAssert.cs`](../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisAssert.cs) (проверка ортонормированности и структурного равенства базисов)
  - [`LinearBasisConstructionTests.cs`](../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisConstructionTests.cs) (конструкторы, копирование, слияние базисов и базовые инварианты)
  - [`LinearBasisMutationTests.cs`](../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisMutationTests.cs) (`LinearBasisMutable`, `AddVector`, `AddVectors` и детерминированный сценарий добавления)
  - [`LinearBasisProjectionAndOrthogonalizationTests.cs`](../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisProjectionAndOrthogonalizationTests.cs) (проекции, `Contains`, ортогональное дополнение, `OrthonormalVector` и текущий контракт `Orthonormalize`)
  - [`LinearBasisComparisonAndSpanTests.cs`](../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisComparisonAndSpanTests.cs) (`Equals`, `CompareTo`, перечисление и `SpanSameSpace`)
  - [`LinearBasisGenerationTests.cs`](../../Tests/DoubleGeometry/Basics/LinearBasis/LinearBasisGenerationTests.cs) (фабрики `GenLinearBasis` и их инварианты)
- Legacy:
  - [`LinearBasisTests.cs`](../../Tests/SharedTests/LinearBasisTests.cs)
- Note:
  - сохранены полезные диагностические сообщения для пустого базиса, копирования и геометрических сравнений; при переносе отдельно зафиксировано, что публичный `Orthonormalize(Vector)` пока остаётся `todo` и сейчас бросает `NotImplementedException`

### AffineBasis

- New tests:
  - [`AffineBasisAssert.cs`](../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisAssert.cs) (проверка ортонормированности линейной части аффинного базиса)
  - [`AffineBasisConstructionTests.cs`](../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisConstructionTests.cs) (конструкторы, copy/no-copy семантика, построение по точкам и инварианты)
  - [`AffineBasisFactoriesAndMutationTests.cs`](../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisFactoriesAndMutationTests.cs) (фабрики `FromVectors`, `FromPoints`, `GenAffineBasis` и `AffineBasisMutable.AddVector`)
  - [`AffineBasisProjectionAndContainmentTests.cs`](../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisProjectionAndContainmentTests.cs) (проекции, `ProjectPoints`, `ToOriginalCoords`, `CanonicalOrigin`, `Contains`, `OrthonormalVector`)
  - [`AffineBasisComparisonAndEnumerationTests.cs`](../../Tests/DoubleGeometry/Basics/AffineBasis/AffineBasisComparisonAndEnumerationTests.cs) (`Equals`, `CompareTo`, `GetHashCode` и перечисление)
- Legacy:
  - [`AffineBasisTest.cs`](../../Tests/SharedTests/AffineBasisTest.cs)
- Note:
  - сохранены полезные диагностические сообщения для copy/no-copy сценариев, проекций и сравнений множеств точек; дополнительно явно зафиксированы контракты `CanonicalOrigin`, `GetHashCode` и запрет на `AffineBasis(origin, LinearBasisMutable, needCopy: false)`

### HyperPlane

- New tests:
  - [`HyperPlaneAssert.cs`](../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneAssert.cs) (проверка согласованности `Normal`, `Origin`, `ConstantTerm` и `AffBasis`)
  - [`HyperPlaneConstructionAndOrientationTests.cs`](../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneConstructionAndOrientationTests.cs) (конструкторы, ориентация нормали и ленивые сценарии инициализации)
  - [`HyperPlaneEvaluationAndContainmentTests.cs`](../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneEvaluationAndContainmentTests.cs) (`Eval`, `Contains*`, фильтрация и `AllAtOneSide`)
  - [`HyperPlaneComparisonOverridesAndFactoryTests.cs`](../../Tests/DoubleGeometry/Basics/HyperPlane/HyperPlaneComparisonOverridesAndFactoryTests.cs) (фабрика `Make3D_xyParallel`, `ToString`, `Equals`, `CompareTo`, `GetHashCode`)
- Legacy:
  - [`HyperPlaneTests.cs`](../../Tests/SharedTests/HyperPlaneTests.cs)
- Note:
  - сохранены полезные сообщения для ориентации, строкового формата и согласованности ленивой инициализации; дополнительно вынесен отдельный прямой тест на публичный контракт `GetHashCode`

### Polyline

- New tests:
  - [`PolylineTestData.cs`](../../Tests/DoubleGeometry/Polygons/Polyline/PolylineTestData.cs) (общие наборы вершин для выпуклого и невыпуклого контуров)
  - [`PolylineConstructionAndGeometryTests.cs`](../../Tests/DoubleGeometry/Polygons/Polyline/PolylineConstructionAndGeometryTests.cs) (конструкторы, циклический индексатор, рёбра, площадь, пустая полилиния и `EdgeAngle`)
  - [`PolylineContainmentTests.cs`](../../Tests/DoubleGeometry/Polygons/Polyline/PolylineContainmentTests.cs) (`ContainsPoint` и `ContainsPointInside` для выпуклого и невыпуклого контура с сохранением legacy-сообщений)
- Legacy:
  - [`PolylineTests.cs`](../../Tests/Double-Tests/PolylineTests.cs)
- Note:
  - сохранены исторические диагностические сообщения `${i}th test: ...`; дополнительно добавлен прямой сценарий для clockwise-обхода и явные тесты на базовую геометрию класса

### BasicPolygon

- New tests:
  - [`BasicPolygonTestProbes.cs`](../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonTestProbes.cs) (служебные наследники для прямой проверки базового абстрактного слоя)
  - [`BasicPolygonConstructionTests.cs`](../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonConstructionTests.cs) (конструкторы по списку и массиву, copy/reference-семантика и начальная инициализация)
  - [`BasicPolygonLazyAggregationTests.cs`](../../Tests/DoubleGeometry/Polygons/BasicPolygon/BasicPolygonLazyAggregationTests.cs) (ленивое восстановление `Vertices`, `Edges` и `Contours` из других представлений)
- Legacy:
  - прямого legacy-файла не было
- Note:
  - `BasicPolygon` закрыт прямым unit-like набором через тестовые наследники; абстрактные `Contains*` осознанно не считаются частью базового покрытия, пока не рассматриваются конкретные потомки

### PolygonTools

- New tests:
  - [`PolygonToolsAssert.cs`](../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsAssert.cs) (общие проверки кардинальности контура и геометрии повёрнутого прямоугольника)
  - [`PolygonToolsRectangleParallelTests.cs`](../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleParallelTests.cs) (`RectangleParallel` для точки, segment-case и невырожденного прямоугольника с сохранением legacy-сообщений)
  - [`PolygonToolsRectangleTurnedTests.cs`](../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsRectangleTurnedTests.cs) (`RectangleTurned` для совпадающих, осе-диагональных и общих противоположных вершин с сохранением legacy-сообщений)
  - [`PolygonToolsCircleAndEllipseTests.cs`](../../Tests/DoubleGeometry/Polygons/PolygonTools/PolygonToolsCircleAndEllipseTests.cs) (прямые фабричные сценарии для `Circle` и `Ellipse`, включая вырожденные полуоси)
- Legacy:
  - [`PolygonToolsTests.cs`](../../Tests/Double-Tests/PolygonToolsTests.cs)
- Note:
  - прямой legacy-набор на прямоугольники перенесён без потери диагностических текстов; для `Circle/Ellipse` добавлено новое прямое покрытие, которого раньше не было

### GammaPair

- New tests:
  - [`GammaPairConstructionAndEqualityTests.cs`](../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairConstructionAndEqualityTests.cs) (конструкторы, нормализация, копирование и прямое `Equals(GammaPair)`)
  - [`GammaPairComparisonAndFormattingTests.cs`](../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairComparisonAndFormattingTests.cs) (`CompareTo`, `CompareTo(null)`, нормированное сравнение и `ToString`)
  - [`GammaPairCrossPairsTests.cs`](../../Tests/DoubleGeometry/Polygons/GammaPair/GammaPairCrossPairsTests.cs) (`CrossPairs` для legacy-набора непараллельных случаев, дробных пересечений и ненормированных нормалей)
- Legacy:
  - прямого legacy-файла не было; релевантные косвенные сценарии жили в [`SupportFunctionTests.cs`](../../Tests/Double-Tests/SupportFunctionTests.cs)
- Note:
  - сохранены исторические сообщения `Bad crossing ...`; параллельный случай сознательно не закреплён как mandatory, пока `CrossPairs` опирается на `Debug.Assert`

### SupportFunction

- New tests:
  - [`SupportFunctionTestData.cs`](../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionTestData.cs) (общие legacy-наборы `GammaPair`, эталонные точки и данные для `FindCone`)
  - [`SupportFunctionProbe.cs`](../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionProbe.cs) (служебный доступ к защищённому `CheckTriple`)
  - [`SupportFunctionInitializationTests.cs`](../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionInitializationTests.cs) (инициализация из `GammaPair` и из точек, zero-normal filtering и `ToConvexify`)
  - [`SupportFunctionEvaluationAndSearchTests.cs`](../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionEvaluationAndSearchTests.cs) (`FuncVal`, `FindCone`, `ConicCombination` и явное сравнение implicit/explicit cone lookup)
  - [`SupportFunctionCombinationAndConvexificationTests.cs`](../../Tests/DoubleGeometry/Polygons/SupportFunction/SupportFunctionCombinationAndConvexificationTests.cs) (`CombineFunctions`, `CheckTriple`, `ConvexifyFunctionWithInfo` и сохранённые legacy-сообщения)
- Legacy:
  - [`SupportFunctionTests.cs`](../../Tests/Double-Tests/SupportFunctionTests.cs)
- Note:
  - legacy-набор полностью разложен по темам, старые диагностические тексты сохранены; дополнительно добраны point/segment ctor и прямая выпуклификация, которых раньше не было

### ConvexPolygon

- New tests:
  - [`ConvexPolygonTestData.cs`](../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonTestData.cs) (legacy-наборы вершин, `GammaPair` и базовые polygon-данные)
  - [`ConvexPolygonAssert.cs`](../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonAssert.cs) (циклическое сравнение списков вершин из legacy helper-а)
  - [`ConvexPolygonConstructionTests.cs`](../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonConstructionTests.cs) (конструкторы из точек, `Vector`, `SupportFunction` и lazy-согласование `SF`)
  - [`ConvexPolygonContainmentTests.cs`](../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonContainmentTests.cs) (`Contains` и `ContainsInside` с сохранением legacy-сообщений)
  - [`ConvexPolygonExtremeAndAreaTests.cs`](../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonExtremeAndAreaTests.cs) (`GetExtremeElements`, `Square` и текущий `NearestPoint`)
  - [`ConvexPolygonRandomAndCutTests.cs`](../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonRandomAndCutTests.cs) (случайные точки, барицентрические веса и прямой контракт `CutConvexPolygon`)
  - [`ConvexPolygonMinkowskiTests.cs`](../../Tests/DoubleGeometry/Polygons/ConvexPolygon/ConvexPolygonMinkowskiTests.cs) (операторы суммы и разности Минковского с переносом legacy-сценариев)
- Legacy:
  - [`ConvexPolygonTests.cs`](../../Tests/Double-Tests/ConvexPolygonTests.cs)
  - [`ConvexPolygonCutTests.cs`](../../Tests/Double-Tests/ConvexPolygonCutTests.cs)
  - [`ConvexPolygonLegacyHelpers.cs`](../../Tests/Double-Tests/ConvexPolygonLegacyHelpers.cs)
  - [`PolygonExtremeTests.cs`](../../Tests/Double-Tests/PolygonExtremeTests.cs)
- Note:
  - smoke-only `CreateCP*` и `DoCutTest` заменены прямыми assertions; `IntersectionPolygon` сознательно не дублируется, потому что уже закрыт отдельным классом `Intersection`

### FaceLattice

- New tests:
  - [`FaceLatticeAssert.cs`](../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeAssert.cs) (локальные helper-ы для создания и сравнения `Vector` в polyhedra-сценариях)
  - [`FaceLatticeTestData.cs`](../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeTestData.cs) (построение треугольной иерархии `FLNode` и готовой тестовой решётки)
  - [`FaceLatticeNodeTests.cs`](../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeNodeTests.cs) (конструкторы `FLNode`, уровни, `AllNonStrictSub`, сравнение и overrides с сохранением legacy-сообщений)
  - [`FaceLatticeStructureAndTransformTests.cs`](../../Tests/DoubleGeometry/Polyhedra/FaceLattice/FaceLatticeStructureAndTransformTests.cs) (конструкторы `FaceLattice`, агрегаты, `AllKfaces_ExceptTop`, `VertexTransform`, `Equals`, `GetHashCode`)
- Legacy:
  - [`FaceLatticeTests.cs`](../../Tests/SharedTests/FaceLatticeTests.cs)
- Note:
  - legacy-набор был сосредоточен почти целиком на `FLNode`; при миграции добавлен отдельный прямой слой на сам контейнер `FaceLattice`

### ConvexPolytop

- New tests:
  - [`ConvexPolytopAssert.cs`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopAssert.cs) (helper-ы для создания `Vector` и сравнения множеств вершин политопа)
  - [`ConvexPolytopTestData.cs`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTestData.cs) (базовые данные для unit-square в `Vrep`, `Hrep` и простых матричных преобразований)
  - [`ConvexPolytopConstructionAndRepresentationTests.cs`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopConstructionAndRepresentationTests.cs) (прямые фабрики из `Vrep`/`Hrep`/`FLrep`, lazy-переходы, `WhichRep`, `fVector`, `InnerPoint`)
  - [`ConvexPolytopFactoriesAndMetricsTests.cs`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopFactoriesAndMetricsTests.cs) (базовые фабрики `Zero`, `Cube01_*`, `RectAxisParallel`, `Ball_*` и метрики диаметра)
  - [`ConvexPolytopContainmentAndNearestPointTests.cs`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopContainmentAndNearestPointTests.cs) (`Contains*`, `NearestPoint` и текущие `NotImplementedException`-ветки)
  - [`ConvexPolytopTransformsAndOverridesTests.cs`](../../Tests/DoubleGeometry/Polyhedra/ConvexPolytop/ConvexPolytopTransformsAndOverridesTests.cs) (преобразования `Shift`/`Rotate`/`LiftUp`/`Scale`/`SectionByHyperPlane` и overrides)
- Legacy:
  - отдельного прямого legacy-файла не было; релевантные сценарии были размазаны по [`GW_Tests.cs`](../../Tests/Double-Tests/GW_hDTests/GW_Tests.cs), [`MinkowskiSumTests.cs`](../../Tests/Double-Tests/Minkowski-Tests/MinkowskiSumTests.cs), [`MinkowskiDiffTests.cs`](../../Tests/Double-Tests/Minkowski-Tests/MinkowskiDiffTests.cs) и [`TestsPolytopes.cs`](../../Tests/ToolsForTests/TestsPolytopes.cs)
- Note:
  - в прямой слой вынесены именно контракты самого `ConvexPolytop`; тяжёлые алгоритмические и IO-ветки оставлены за пределами обязательного минимума и будут разбираться следующими классами backlog
