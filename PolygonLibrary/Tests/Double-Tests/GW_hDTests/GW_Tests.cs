using System.Diagnostics;
using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Convertors.DConvertor>;


namespace DoubleTests;

[TestFixture]
public class GW_Tests {

  /// <summary>
  /// The random engine.
  /// </summary>
  private static RandomLC _random = new RandomLC();

#region Auxiliary functions
  /// <summary>
  /// Finds all subsets of a given length for an array.
  /// </summary>
  /// <typeparam name="T">The type of the array elements.</typeparam>
  /// <param name="arr">The input array.</param>
  /// <param name="subsetLength">The length of the subsets.</param>
  /// <returns>A list of subsets of the specified length.</returns>
  private static List<List<T>> Subsets<T>(IReadOnlyList<T> arr, int subsetLength) {
    List<List<T>> subsets = new List<List<T>>();

    void FindSubset(List<T> currentSubset, int currentIndex) {
      if (currentSubset.Count == subsetLength) {
        subsets.Add(new List<T>(currentSubset));

        return;
      }

      if (currentIndex == arr.Count) {
        return;
      }

      FindSubset(new List<T>(currentSubset) { arr[currentIndex] }, currentIndex + 1);
      FindSubset(new List<T>(currentSubset), currentIndex + 1);
    }

    FindSubset(new List<T>(), 0);

    return subsets;
  }

  /// <summary>
  /// Generates all subsets of a given list.
  /// </summary>
  /// <typeparam name="T">The type of elements in the list.</typeparam>
  /// <param name="arr">The input list.</param>
  /// <returns>A list of all subsets of the input list.</returns>
  private static List<List<T>> AllSubsets<T>(IReadOnlyList<T> arr) {
    List<List<T>> subsets      = new List<List<T>>();
    int           totalSubsets = 1 << arr.Count;

    for (int i = 0; i < totalSubsets; i++) {
      List<T> subset = new List<T>();

      for (int j = 0; j < arr.Count; j++) {
        if ((i & (1 << j)) != 0) {
          subset.Add(arr[j]);
        }
      }

      subsets.Add(subset);
    }

    return subsets;
  }


  /// <summary>
  /// Generates a random double value in (0,1): a value between 0 and 1, excluding the values 0 and 1.
  /// </summary>
  /// <returns>The generated random double value.</returns>
  private double GenInner(RandomLC? random = null) {
    double w;
    do {
      w = random?.NextDouble() ?? _random.NextDouble();
    } while (Tools.LT(w, 100 * Tools.Eps) || Tools.GT(w, 1 - 100 * Tools.Eps));

    return w;
  }

  /// <summary>
  /// Generates an affine combination of points. Default coefficients in (0,1).
  /// </summary>
  /// <param name="points">The list of points to combine.</param>
  /// <param name="mult">The multiplier for each coefficient. Default is 1.</param>
  /// <param name="shift">The shift for each coefficient. Default is 0.</param>
  /// <returns>The resulting point.</returns>
  private Point GenAffineCombination(List<Point> points, int mult = 1, int shift = 0) {
    List<double> ws = new List<double>();

    for (int i = 0; i < points.Count; i++) {
      ws.Add(GenInner() * mult + shift);
    }
    Point res = Point.LinearCombination(points, ws);

    return res;
  }

  /// <summary>
  /// Generates a linear combination of the given points.
  /// </summary>
  /// <param name="points">The list of point to lin-combine.</param>
  /// <param name="random">The random to be used. If null, the _random be used.</param>
  /// <returns>A linear combination of the given points.</returns>
  private Point GenConvexCombination(List<Point> points, RandomLC? random = null) {
    List<double> ws = new List<double>();

    double difA = 1;
    for (int i = 0; i < points.Count - 1; i++) {
      double alpha = GenInner(random) * difA;
      ws.Add(alpha);
      difA -= alpha;
    }
    ws.Add(difA);

    Debug.Assert(Tools.EQ(ws.Sum(), 1), "GenConvexCombination: sum of weights does not equal 1.");

    Point res = Point.LinearCombination(points, ws);

    return res;
  }

  /// <summary>
  /// Generates a set of random face dimension-indices for a polyhedron.
  /// </summary>
  /// <param name="fCount">The number of face indices to generate.</param>
  /// <param name="polyhedronDim">The dimension of the polyhedron.</param>
  /// <returns>A HashSet containing the randomly generated face dimension-indices.</returns>
  private static HashSet<int> GenFacesInd(int fCount, int polyhedronDim) {
    HashSet<int> faceInd = new HashSet<int>();

    for (int j = 0; j < fCount; j++) {
      int ind;

      do {
        ind = _random.NextInt(1, polyhedronDim);
      } while (!faceInd.Add(ind));
    }

    return faceInd;
  }

  /// <summary>
  /// Adds points to an existing simplex.
  /// </summary>
  /// <param name="facesDim">The dimensions of the faces where points will be added. If null, no points will be added.</param>
  /// <param name="amount">The number of points to add on each faceDim.</param>
  /// <param name="simplex">The initial simplex to which points will be added.</param>
  /// <param name="random">The random to be used. If null, the _random be used.</param>
  /// <returns>A new simplex with the added points.</returns>
  private List<Point> AddPointsToSimplex(IEnumerable<int>?    facesDim
                                       , int                  amount
                                       , IReadOnlyList<Point> simplex
                                       , RandomLC?            random = null) {
    RandomLC rnd = random ?? _random;

    List<Point> Simplex = new List<Point>(simplex);

    if (facesDim is not null) {
      foreach (int dim in facesDim) {
        Debug.Assert(dim <= simplex[0].Dim);

        List<List<Point>> faces = Subsets(simplex, dim + 1);

        for (int k = 0; k < amount; k++) {
          int   ind    = rnd.NextInt(0, faces.Count - 1);
          Point inFace = GenConvexCombination(faces[ind], random);
          Simplex.Add(inFace);
        }
      }
    }

    return Simplex;
  }

  /// <summary>
  /// Generates a non-zero random vector of the specified dimension. Each coordinate: [-0.5, 0.5] \ {0}.
  /// </summary>
  /// <param name="dim">The dimension of the vector.</param>
  /// <param name="random">If null then _random be used.</param>
  /// <returns>A random vector.</returns>
  private static Vector GenVector(int dim, RandomLC? random = null) {
    double[] v = new double[dim];

    Vector res;

    RandomLC rnd = random ?? _random;
    do {
      for (int i = 0; i < dim; i++) {
        v[i] = rnd.NextDouble() - 0.5;
      }
      res = new Vector(v);
    } while (res.IsZero);


    return res;
  }

  /// <summary>
  /// Generate rotation matrix.
  /// </summary>
  /// <param name="spaceDim">The dimension d of the space.</param>
  /// <returns>Unitary matrix dxd.</returns>
  private static Matrix GenRotation(int spaceDim) {
    LinearBasis basis = new LinearBasis(new[] { GenVector(spaceDim) });

    while (!basis.IsFullDim) {
      basis.AddVector(GenVector(spaceDim));
    }

    return basis.GetMatrix();
  }

  /// <summary>
  /// Rotates the given swarm of points in the space by given unitary matrix.
  /// </summary>
  /// <param name="S">The swarm of points to rotate.</param>
  /// <param name="rotation">Matrix to rotate a swarm.</param>
  /// <returns>The rotated swarm of points.</returns>
  private static List<Point> Rotate(IEnumerable<Point> S, Matrix rotation) {
    IEnumerable<Vector> rotated = S.Select(s => new Vector(s) * rotation);

    return rotated.Select(v => new Point(v)).ToList();
  }

  /// <summary>
  /// Generates a random non-zero vector, each coordinate [-50, 50] \ {0}.
  /// </summary>
  /// <param name="dim">The dimension of the vector.</param>
  /// <returns>A random non-zero vector.</returns>
  private static Vector GenShift(int dim) { return GenVector(dim) * _random.NextInt(1, 100); }

  /// <summary>
  /// Shift given swarm by given vector
  /// </summary>
  /// <param name="S">S to be shifted</param>
  /// <param name="shift">Vector to shift</param>
  /// <returns></returns>
  private static List<Point> Shift(List<Point> S, Vector shift) { return S.Select(s => new Point(s + shift)).ToList(); }


  ///<summary>
  /// Method applies a rotation and a shift to two lists of points.
  ///</summary>
  ///<param name="PDim">The dimension of the space in which the points exist.</param>
  ///<param name="P">A reference to the list of points to be transformed.</param>
  ///<param name="S">A reference to the list of points representing the swarm to be transformed.</param>
  private static void ShiftAndRotate(int PDim, ref List<Point> P, ref List<Point> S) {
    Matrix rotation = GenRotation(PDim);
    Vector shift    = GenVector(PDim) * _random.NextInt(1, 10);

    P = Rotate(P, rotation);
    P = Shift(P, shift);

    S = Rotate(S, rotation);
    S = Shift(S, shift);
  }
#endregion

#region Main Generators
  /// <summary>
  /// Generates a d-ortho-based-simplex in d-space.
  /// </summary>
  /// <param name="simplexDim">The dimension of the simplex.</param>
  /// <param name="pureSimplex">Only vertices of the simplex.</param>
  /// <param name="facesDim">The dimensions of the faces of the simplex to put points on.</param>
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <returns>A list of points representing the simplex.</returns>
  private List<Point> Simplex(int simplexDim, out List<Point> pureSimplex, IEnumerable<int>? facesDim = null, int amount = 1) {
    List<Point> simplex = new List<Point> { new Point(new double[simplexDim]) };

    for (int i = 0; i < simplexDim; i++) {
      double[] v = new double[simplexDim];
      v[i] = 1;
      simplex.Add(new Point(v));
    }
    pureSimplex = new List<Point>(simplex);

    return AddPointsToSimplex(facesDim, amount, simplex);
  }

  /// <summary>
  /// Generates a d-simplex in d-space.
  /// </summary>
  /// <param name="simplexDim">The dimension of the simplex.</param>
  /// <param name="pureSimplex">Only vertices of the simplex.</param>
  /// <param name="facesDim">The dimensions of the faces of the simplex to put points on.</param>
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <param name="seed">The seed to be placed into RandomLC. If null, the _random be used.</param>
  /// <returns>A list of points representing the simplex.</returns>
  private List<Point> SimplexRND(int               simplexDim
                               , out List<Point>   pureSimplex
                               , IEnumerable<int>? facesDim = null
                               , int               amount   = 1
                               , uint?             seed     = null) {
    RandomLC random = seed is null ? _random : new RandomLC(seed);


    List<Point> simplex = new List<Point>();
    do {
      for (int i = 0; i < simplexDim + 1; i++) {
        simplex.Add(new Point(10 * GenVector(simplexDim, random)));
      }
    } while (!new AffineBasis(simplex).IsFullDim);
    List<Point> aux = new List<Point>(simplex);
    aux.RemoveAt(0);
    Debug.Assert(new HyperPlane(new AffineBasis(aux)).FilterIn(simplex).Count() != simplex.Count);


    pureSimplex = new List<Point>(simplex);

    return AddPointsToSimplex(facesDim, amount, simplex, random);
  }

  /// <summary>
  /// Generates a full-dimension hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="pureCube">The list of cube vertices of given dimension.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The amount of points to be placed into random set of faces of faceDim dimension.</param>
  /// <param name="seed">The seed to be placed into RandomLC. If null, the _random be used.</param>
  /// <returns>A list of points representing the hypercube possibly with inner points.</returns>
  private List<Point> Cube(int               cubeDim
                         , out List<Point>   pureCube
                         , IEnumerable<int>? facesDim = null
                         , int               amount   = 1
                         , uint?             seed     = null) {
    RandomLC random = seed is null ? _random : new RandomLC(seed);

    List<List<double>> cube_prev = new List<List<double>>();
    List<List<double>> cube      = new List<List<double>>();
    cube_prev.Add(new List<double>() { 0 });
    cube_prev.Add(new List<double>() { 1 });

    for (int i = 1; i < cubeDim; i++) {
      cube.Clear();

      foreach (List<double> coords in cube_prev) {
        cube.Add(new List<double>(coords) { 0 });
        cube.Add(new List<double>(coords) { 1 });
      }
      cube_prev = new List<List<double>>(cube);
    }

    List<Point> Cube = new List<Point>();

    foreach (List<double> v in cube) {
      Cube.Add(new Point(v.ToArray()));
    }
    pureCube = new List<Point>(Cube);

    if (facesDim is not null) { // накидываем точки на грани нужных размерностей
      foreach (int dim in facesDim) {
        Debug.Assert(dim <= Cube.First().Dim);

        for (int i = 0; i < amount; i++) {
          double[] point = new double[cubeDim];

          for (int j = 0; j < cubeDim; j++) {
            point[j] = -1;
          }

          HashSet<int> constInd = new HashSet<int>();

          for (int j = 0; j < cubeDim - dim; j++) {
            int ind;

            do {
              ind = random.NextInt(0, cubeDim - 1);
            } while (!constInd.Add(ind));
          }

          int zeroOrOne = random.NextInt(0, 1);

          foreach (int ind in constInd) {
            point[ind] = zeroOrOne;
          }

          for (int j = 0; j < cubeDim; j++) {
            if (Tools.EQ(point[j], -1)) {
              point[j] = GenInner(random);
            }
          }
          Cube.Add(new Point(point));
        }
      }
    }

    return Cube;
  }

  /// <summary>
  /// Generates a d-cross_polytope in d-space.
  /// </summary>
  /// <param name="crossDim">The dimension of the cross polytope.</param>
  /// <param name="innerPoints">The amount of inner points of the cross polytope.</param>
  /// <returns></returns>
  private List<Point> CrossPolytop(int crossDim, int innerPoints = 0) {
    List<Point> cross = new List<Point>();

    for (int i = 1; i <= crossDim; i++) {
      Vector v = Vector.CreateOrth(crossDim, i);
      cross.Add(new Point(v));
      cross.Add(new Point(-v));
    }

    List<Point> Cross = new List<Point>(cross);

    for (int i = 0; i < innerPoints; i++) {
      cross.Add(GenConvexCombination(cross));
    }


    return Cross;
  }
#endregion


#region Auxiliary tests
  [Test]
  public void GenCubeHDTest() {
    HashSet<Point> S = new HashSet<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, 1, 0 })
      , new Point(new double[] { 0, 0, 1 })
      , new Point(new double[] { 1, 1, 0 })
      , new Point(new double[] { 0, 1, 1 })
      , new Point(new double[] { 1, 0, 1 })
      , new Point(new double[] { 1, 1, 1 })
      };

    List<Point> cube = Cube(3, out List<Point> _);

    Debug.Assert(S.SetEquals(new HashSet<Point>(cube)), "S is not equal to generated Cube");
  }
#endregion


#region Cube3D-Static Тесты 3D-куба не зависящие от _random
  [Test]
  public void Cube3D_Rotated_Z45() {
    List<Point>  S     = Cube(3, out List<Point> _);
    double angle = Tools.PI / 4;
    double       sin   = double.Sin(angle);
    double       cos   = double.Cos(angle);

    double[,] rotationZ45 = { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } };

    List<Point> Rotated = Rotate(S, new Matrix(rotationZ45));

    Polyhedron P = GiftWrapping.WrapPolyhedron(Rotated);
    Assert.That(P.Vertices.SetEquals(Rotated), "The set of vertices must be equal.");
  }

  /// <summary>
  /// Как-то повёрнутый куб
  /// </summary>
  [Test]
  public void Cube3D_Rotated() {
    HashSet<Point> S = new HashSet<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 0.6800213885880926, 0.3956859369533106, 0.6172548504143999 })
      , new Point(new double[] { -0.47124672587598565, -0.40907382401238557, 0.7813994688115978 })
      , new Point(new double[] { 0.20877466271210693, -0.013387887059074954, 1.3986543192259977 })
      , new Point(new double[] { -0.561691583000748, 0.822247679112118, 0.0917132475755244 })
      , new Point(new double[] { 0.11832980558734463, 1.2179336160654286, 0.7089680979899242 })
      , new Point(new double[] { -1.0329383088767337, 0.4131738550997325, 0.8731127163871222 })
      , new Point(new double[] { -0.3529169202886411, 0.8088597920530431, 1.4903675668015222 })
      };


    Polyhedron P = GiftWrapping.WrapPolyhedron(S);
    Assert.That(P.Vertices.SetEquals(S), "The set of vertices must be equal.");
  }

  /// <summary>
  /// Как-то сдвинутый куб
  /// </summary>
  [Test]
  public void Cube3D_Shifted() {
    HashSet<Point> S = new HashSet<Point>()
      {
        new Point(new double[] { -10.029417029821644, -8.414457472370579, 12.142282885765258 })
      , new Point(new double[] { -10.029417029821644, -8.414457472370579, 13.142282885765258 })
      , new Point(new double[] { -10.029417029821644, -7.414457472370579, 12.142282885765258 })
      , new Point(new double[] { -10.029417029821644, -7.414457472370579, 13.142282885765258 })
      , new Point(new double[] { -9.029417029821644, -8.414457472370579, 12.142282885765258 })
      , new Point(new double[] { -9.029417029821644, -8.414457472370579, 13.142282885765258 })
      , new Point(new double[] { -9.029417029821644, -7.414457472370579, 12.142282885765258 })
      , new Point(new double[] { -9.029417029821644, -7.414457472370579, 13.142282885765258 })
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);
    Assert.That(P.Vertices.SetEquals(S), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube3D_Rotated_Shifted() {
    HashSet<Point> S = new HashSet<Point>()
      {
        new Point(new double[] { 4.989650328990457, 18.100255093909855, 14.491501515962065 })
      , new Point(new double[] { 5.66967171757855, 18.495941030863165, 15.108756366376465 })
      , new Point(new double[] { 4.518403603114471, 17.69118126989747, 15.272900984773663 })
      , new Point(new double[] { 5.198424991702564, 18.08686720685078, 15.890155835188063 })
      , new Point(new double[] { 4.427958745989709, 18.92250277302197, 14.58321476353759 })
      , new Point(new double[] { 5.107980134577802, 19.318188709975285, 15.20046961395199 })
      , new Point(new double[] { 3.9567120201137236, 18.513428949009587, 15.364614232349188 })
      , new Point(new double[] { 4.636733408701816, 18.909114885962897, 15.981869082763588 })
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(S), "The set of vertices must be equal.");
  }

  /// <summary>
  /// Семена устанавливаются так: ТИП РАЗМЕРНОСТЬ НА_ГРАНЯХ_КАКИХ_РАЗМЕРНОСТЕЙ
  /// 1 - куб
  /// 2 - тетр
  /// </summary>
  [Test]
  public void Cube3D_withInnerPoints_On_1D() {
    List<Point> S = Cube(3, out List<Point> cube, new List<int>() { 1 }, 1, 131);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D() {
    List<Point> S = Cube(3, out List<Point> cube, new List<int>() { 2 }, 1, 132);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_3D() {
    List<Point> S = Cube(3, out List<Point> cube, new List<int>() { 3 }, 1, 133);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D() {
    List<Point> S = Cube(3, out List<Point> cube, new List<int>() { 1, 2 }, 1, 1312);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D_3D() {
    List<Point> S = Cube(3, out List<Point> cube, new List<int>() { 2, 3 }, 1, 1323);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D_3D() {
    List<Point> S = Cube(3, out List<Point> cube, new List<int>() { 1, 2, 3 }, 1, 13123);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }
#endregion

#region Suffle-Zone PurePolytops Тесты перестановок без дополнительных точек
  /// <summary>
  /// Shuffles the elements of the S list and wraps it into a Polyhedron.
  /// Asserts that the set of vertices in the Polyhedron is equal to the Polytop list.
  /// </summary>
  /// <param name="Polytop">The list of points representing the Polytop.</param>
  /// <param name="S">The list of points representing the S.</param>
  private static void SwarmShuffle(List<Point> Polytop, List<Point> S) {
    for (int i = 0; i < 10 * Polytop.Count; i++) {
      uint saveSeed = _random.Seed;
      S.Shuffle(_random);
      Polyhedron P = GiftWrapping.WrapPolyhedron(S);
      Assert.That(P.Vertices.SetEquals(Polytop), $"The set of vertices must be equal.\nSeed: {saveSeed}");
    }
  }

  [Test]
  public void Cube3D_Suffled() {
    List<Point> S = Cube(3, out List<Point> cube);
    SwarmShuffle(S, cube);
  }

  [Test]
  public void Cube4D_Suffled() {
    List<Point> S = Cube(4, out List<Point> cube);
    SwarmShuffle(cube, S);
  }

  [Test]
  public void Simplex3D_Suffled() {
    List<Point> S = Simplex(3, out List<Point> simplex);
    SwarmShuffle(simplex, S);
  }

  [Test]
  public void Simplex4D_Suffled() {
    List<Point> S = Simplex(4, out List<Point> simplex);
    SwarmShuffle(simplex, S);
  }
#endregion

#region Cube4D-Static Тесты 4D-куба не зависящие от _random
  [Test]
  public void Cube4D_withInnerPoints_On_1D() {
    List<Point> S = Cube(4, out List<Point> cube, new List<int>() { 1 }, 1, 141);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }


  [Test]
  public void Cube4D_withInnerPoints_On_2D() {
    List<Point> S = Cube(4, out List<Point> cube, new List<int>() { 2 }, 1, 142);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_3D() {
    List<Point> S = Cube(4, out List<Point> cube, new List<int>() { 3 }, 1, 143);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D() {
    List<Point> S = Cube(4, out List<Point> cube, new List<int>() { 1, 2 }, 1, 1412);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_2D_3D() {
    List<Point> S = Cube(4, out List<Point> cube, new List<int>() { 2, 3 }, 1, 1423);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D() {
    List<Point> S = Cube(4, out List<Point> cube, new List<int>() { 1, 2, 3 }, 1, 14123);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D_4D() {
    List<Point> S = Cube
      (
       4
     , out List<Point> cube
     , new List<int>()
         {
           1
         , 2
         , 3
         , 4
         }
     , 1
     , 141234
      );

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equal.");
  }
#endregion

#region Simplex4D Тесты 4D-симплекса не зависящие от _random
  [Test]
  public void Simplex4D_1DEdge_2DNeighborsPointsTest() {
    Point p0 = new Point(new double[] { 0, 0, 0, 0 });
    Point p1 = new Point(new double[] { 1, 0, 0, 0 });
    Point p2 = new Point(new double[] { 0, 1, 0, 0 });
    Point p3 = new Point(new double[] { 0.1, 0, 1, 0 });
    Point p4 = new Point(new double[] { 0.1, 0, 0, 1 });

    List<Point> Simplex = new List<Point>()
      {
        p0
      , p1
      , p2
      , p3
      , p4
      };


    List<Point> S = new List<Point>(Simplex)
      {
        Point.LinearCombination(p1, 0.3, p2, 0.2)
      , Point.LinearCombination(p1, 0.4, p2, 0.1)
      , Point.LinearCombination(p1, 0.4, p3, 0.1)
      , Point.LinearCombination(p1, 0.4, p3, 0.1)
      , Point.LinearCombination(p1, 0.4, p4, 0.1)
      , Point.LinearCombination(p1, 0.4, p4, 0.1)
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);
    Assert.That(P.Vertices.SetEquals(Simplex), "The set of vertices must be equal.");
  }
#endregion

#region AllCubes Генераторы "плохих" тестов для кубов
  [Test]
  public void AllCubes3D_TestRND() {
    const int nPoints = 5000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 3).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Cube(3, out List<Point> P, fID, nPoints);
      ShiftAndRotate(3, ref P, ref S);

      Check(S, P, saveSeed, 3, nPoints, fID, true);
    }
  }

  [Test]
  public void AllCubes4D_TestRND() {
    const int nPoints = 2000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 4).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Cube(4, out List<Point> P, fID, nPoints);
      ShiftAndRotate(4, ref P, ref S);

      Check(S, P, saveSeed, 4, nPoints, fID, true);
    }
  }

  [Test]
  public void AllCubes5D_TestRND() {
    const int nPoints = 5;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 5).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Cube(5, out List<Point> P, fID, nPoints);
      ShiftAndRotate(5, ref P, ref S);

      Check(S, P, saveSeed, 5, nPoints, fID, true);
    }
  }

  [Test]
  public void AllCubes6D_TestRND() {
    const int nPoints = 2;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 6).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Cube(6, out List<Point> P, fID, nPoints);
      ShiftAndRotate(6, ref P, ref S);

      Check(S, P, saveSeed, 6, nPoints, fID, true);
    }
  }
#endregion

#region AllSimplices Генераторы "плохих" тестов для симплексов полученных из базисных орт
  [Test]
  public void AllSimplices3D_TestRND() {
    const int nPoints = 2000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 3).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Simplex(3, out List<Point> P, fID, nPoints);
      ShiftAndRotate(3, ref P, ref S);

      Check(S, P, saveSeed, 3, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices4D_TestRND() {
    const int nPoints = 2000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 4).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Simplex(4, out List<Point> P, fID, nPoints);
      ShiftAndRotate(4, ref P, ref S);

      Check(S, P, saveSeed, 4, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices5D_TestRND() {
    const int nPoints = 1000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 5).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Simplex(5, out List<Point> P, fID, nPoints);
      ShiftAndRotate(5, ref P, ref S);

      Check(S, P, saveSeed, 5, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices6D_TestRND() {
    const int nPoints = 5;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 6).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Simplex(6, out List<Point> P, fID, nPoints);
      ShiftAndRotate(6, ref P, ref S);

      Check(S, P, saveSeed, 6, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices7D_TestRND() {
    const int nPoints = 2;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 7).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> S = Simplex(7, out List<Point> P, fID, nPoints);
      ShiftAndRotate(7, ref P, ref S);

      Check(S, P, saveSeed, 7, nPoints, fID, true);
    }
  }
#endregion

#region AllSimplicesRND Генераторы "плохих" тестов для произвольных симплексов
  [Test]
  public void AllSimplicesRND_3D_TestRND() {
    const int nPoints    = 1;
    const int simplexDim = 3;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, simplexDim).ToList());

    for (int i = 0; i < 1e6; i++) {
      foreach (List<int> fID in fIDs) {
        uint saveSeed = _random.Seed;

        List<Point> S = SimplexRND(simplexDim, out List<Point> P, fID, nPoints);
        Check(S, P, saveSeed, simplexDim, nPoints, fID, true);
      }
    }
  }

  [Test]
  public void AllSimplicesRND_4D_TestRND() {
    const int nPoints    = 1;
    const int simplexDim = 4;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, simplexDim).ToList());

    for (int i = 0; i < 1e4; i++) {
      foreach (List<int> fID in fIDs) {
        uint saveSeed = _random.Seed;

        List<Point> S = SimplexRND(simplexDim, out List<Point> P, fID, nPoints);
        Check(S, P, saveSeed, simplexDim, nPoints, fID, true);
      }
    }
  }

  [Test]
  public void AllSimplicesRND_5D_TestRND() {
    const int nPoints    = 1;
    const int simplexDim = 5;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, simplexDim).ToList());

    for (int i = 0; i < 1e3; i++) {
      foreach (List<int> fID in fIDs) {
        uint saveSeed = _random.Seed;

        List<Point> S = SimplexRND(simplexDim, out List<Point> P, fID, nPoints);
        Check(S, P, saveSeed, simplexDim, nPoints, fID, true);
      }
    }
  }
#endregion


#region Other tests
  /// <summary>
  /// Вершины:
  ///[0] +2.573  A
  ///[1] -0.433     X
  ///[2] -3.942  B
  ///[3] -2.231  C
  ///[4] +0.457  D
  ///
  ///  Доп.точки:
  ///[5] -0.715  0,1,2
  ///[6] -1.250  0,2,3,4
  ///[7] +2.524  0,1,2,3,4  F
  ///
  ///0-2-3-4-6-7
  ///
  /// Точка F очень близка к вершине A так, что с точностью 1e-8 принадлежит гиперплоскостям всех гиперграней, проходящих через эту вершину.
  /// При загрублении точности до такого значения получается не гипер-тетраэдр.
  /// </summary>
  [Test]
  public void Simplex4D_PointCloseToVertex() {
    const uint seed    = 725498027;
    const int  PDim    = 4;
    const int  nPoints = 1;
    List<int>  fID     = new List<int>() { 2, 3, 4 };

    _random = new RandomLC(seed);

    List<Point> S = new List<Point>()
      {
        new Point(new double[] { 2.573083673504434, 4.459384730891181, -0.27379963436950927, -3.9775508290570114 })
      , new Point(new double[] { -0.4334526451848103, -0.4053162935667942, 3.2553497814236554, 3.4524045601609177 })
      , new Point(new double[] { -3.942094170242104, -1.9384525033967692, -0.29372328782773627, 2.603184338100996 })
      , new Point(new double[] { -2.231889343176011, -3.249343109375179, -0.4791314609998676, -3.9361931497548226 })
      , new Point(new double[] { 0.4576718028303406, -1.483829232511071, 1.5060715392478907, -3.912975119639415 })
      , new Point(new double[] { -0.715863786767021, 1.1248964198021802, -0.08999099950936182, -0.41480693316225237 })
      , new Point(new double[] { -1.2504502355127234, -2.0382520989901907, 0.055536783449397165, -3.4765790886104364 })
      , new Point(new double[] { 2.5244380627935232, 4.399463020994712, -0.2616425805524087, -3.92127411448588 })
      };

    var hpABCD = new HyperPlane
      (
       new AffineBasis
         (
          new List<Point>()
            {
              S[0]
            , S[2]
            , S[3]
            , S[4]
            }
         )
      );
    var distABCD = S.Select(s => hpABCD.Eval(s));

    AffineBasis ABDbasis = new AffineBasis(new List<Point>() { S[2], S[0], S[4] });

    Vector BC = Vector.OrthonormalizeAgainstBasis(S[3] - S[2], ABDbasis.Basis);
    Vector BX = Vector.OrthonormalizeAgainstBasis(S[1] - S[2], ABDbasis.Basis);
    Vector BF = Vector.OrthonormalizeAgainstBasis(S[7] - S[2], ABDbasis.Basis);

    double angleCBX = double.Acos(BC * BX);
    double angleCBF = double.Acos(BC * BF);


    var hpABDX = new HyperPlane
      (
       new AffineBasis
         (
          new List<Point>()
            {
              S[0]
            , S[2]
            , S[1]
            , S[4]
            }
         )
      );
    var distABDX = S.Select(s => hpABDX.Eval(s));

    SimplexRND(PDim, out List<Point> polytop, null, 0, seed);
    Check(S, polytop, seed, PDim, nPoints, fID, true);
  }

  /// <summary>
  /// Вершины:
  /// [0] +0.836      -->;  [2]
  /// [1] -2.291      -->;  [7]
  /// [2] +2.140      -->;  [5]
  /// [3] -1.128      -->;  [6]
  /// [4] +3.068      -->;  [8]
  ///
  /// Доп. точки:               номера точек на ребре между которыми лежат данные точки
  /// [5] +2.175 0,4  -->;  [1] 2,8
  /// [6] -1.531 1,4  -->;  [4] 7,8
  /// [7] +0.908 0,2  -->;  [3] 2,5
  /// [8] -2.290 1,4  -->;  [0] 7,8
  /// </summary>
  [Test]
  public void Simplex4D_InnerPointsIn_1D() {
    List<Point> Simplex = new List<Point>()
      {
        new Point(new double[] { 0.8364793532147252, 3.1538275299020646, -2.8732700734104193, 2.4909120326607748 })
      , new Point(new double[] { -2.2910587157334805, -2.149176399025409, 4.5139871187307845, -3.2342020813921 })
      , new Point(new double[] { 2.140466204644289, 1.8671979608170686, 0.043747361061103884, 0.9348952481371575 })
      , new Point(new double[] { -1.128714852065014, -1.7299541148194004, 0.4864426528770571, -1.6846663706667409 })
      , new Point(new double[] { 3.0687608076419592, 1.4408928939236543, 4.602441817895146, 1.823890199145276 })
      };

    List<Point> S = new List<Point>(Simplex)
      {
        new Point(new double[] { 2.175818488745113, 2.126089567011522, 1.6120574743850615, 2.0907078182196503 })
      , new Point(new double[] { -1.5310856984393355, -1.6401376560306955, 4.526529179955908, -2.517011206694256 })
      , new Point(new double[] { 0.9089342229083861, 3.08233710216511, -2.7111885939253577, 2.4044533438785916 })
      , new Point(new double[] { -2.290970227496747, -2.149117128577943, 4.51398857907853, -3.2341185745379626 })
      };

    List<Point> S_shuffled = new List<Point>()
      {
        new Point(new double[] { 2.175818488745113, 2.126089567011522, 1.6120574743850615, 2.0907078182196503 })
      , new Point(new double[] { -1.128714852065014, -1.7299541148194004, 0.4864426528770571, -1.6846663706667409 })
      , new Point(new double[] { 0.8364793532147252, 3.1538275299020646, -2.8732700734104193, 2.4909120326607748 })
      , new Point(new double[] { 2.140466204644289, 1.8671979608170686, 0.043747361061103884, 0.9348952481371575 })
      , new Point(new double[] { -2.2910587157334805, -2.149176399025409, 4.5139871187307845, -3.2342020813921 })
      , new Point(new double[] { -2.290970227496747, -2.149117128577943, 4.51398857907853, -3.2341185745379626 })
      , new Point(new double[] { 3.0687608076419592, 1.4408928939236543, 4.602441817895146, 1.823890199145276 })
      , new Point(new double[] { -1.5310856984393355, -1.6401376560306955, 4.526529179955908, -2.517011206694256 })
      , new Point(new double[] { 0.9089342229083861, 3.08233710216511, -2.7111885939253577, 2.4044533438785916 })
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);
    Assert.That(P.Vertices.SetEquals(Simplex), "The set of vertices must be equal.");
    P = GiftWrapping.WrapPolyhedron(S_shuffled);
    Assert.That(P.Vertices.SetEquals(Simplex), "The set of shuffled vertices must be equal.");
  }

  /// <summary>
  /// Пример из какой-то статьи
  /// </summary>
  [Test]
  public void SomePolytop_3D() {
    List<Point> S = new List<Point>()
      {
        new Point(new double[] { 1, 0, 1 })
      , new Point(new double[] { 1, 0, -1 })
      , new Point(new double[] { 1.25, -1, 1 })
      , new Point(new double[] { 1.25, -1, -1 })
      , new Point(new double[] { 0.25, -1, 1 })
      , new Point(new double[] { 0.25, -1, -1 })
      , new Point(new double[] { -1, 0, 1 })
      , new Point(new double[] { -1, 0, -1 })
      , new Point(new double[] { -1.25, 1, 1 })
      , new Point(new double[] { -1.25, 1, -1 })
      , new Point(new double[] { -0.25, 1, 1 })
      , new Point(new double[] { -0.25, 1, -1 })
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);
    Assert.That(P.Vertices.SetEquals(S), "The set of vertices must be equal.");
  }


  /// <summary>
  /// Что-то не работает. Надо смотреть
  /// </summary>
  [Test]
  public void VeryFlatSimplex() {
    List<Point> Simplex = new List<Point>()
      {
        new Point(new double[] { -2.3793875187121767, 2.3500797192915526, -1.1974150399205774 })
      , new Point(new double[] { -4.910117771921241, -1.4236623087021667, 0.854901237379504 })
      , new Point(new double[] { -3.1594402338749363, -4.895324262300349, 2.742933674655607 })
      , new Point(new double[] { 4.032485061099865, 4.553506423149609, -2.364029653222307 })
      };

    List<Point> S = new List<Point>(Simplex);
    // Point       p = new Point(new double[] { 1.412740433333706, 2.802488742178694, -1.4210405632153025 });
    // S.Add(p);

    var hpABC    = new HyperPlane(new AffineBasis(new List<Point>() { S[3], S[1], S[2] }));
    var distABCD = S.Select(s => hpABC.Eval(s));

    Polyhedron P = GiftWrapping.WrapPolyhedron(Simplex);
    Assert.That(P.Vertices.SetEquals(Simplex));
  }


  /// <summary>
  /// Параллелепипед расположенный в первом квадранте
  /// </summary>
  [Test]
  public void SomeParallelogramm() {
    Point  origin = new Point(3);
    Vector v1     = new Vector(new double[] { 0.5, 1, 1 });
    Vector v2     = new Vector(new double[] { 1, 0.5, 1 });
    Vector v3     = new Vector(new double[] { 1, 1, 0.5 });

    List<Point> S = new List<Point>()
      {
        origin
      , origin + v1
      , origin + v2
      , origin + v3
      , origin + v1 + v2
      , origin + v1 + v3
      , origin + v2 + v3
      , origin + v1 + v2 + v3
      };

    SwarmShuffle(S, S);
  }
#endregion

  /// <summary>
  /// Auxiliary enum for Aux-method.
  /// </summary>
  private enum PType { Cube, Simplex, SimplexRND }


  /// <summary>
  /// Aux procedure.
  /// </summary>
  /// <param name="S">The swarm to convexify.</param>
  /// <param name="Answer">The final list of points.</param>
  /// <param name="seed"></param>
  /// <param name="PDim"></param>
  /// <param name="nPoints"></param>
  /// <param name="fID"></param>
  /// <param name="needShuffle"></param>
  private static void Check(List<Point> S
                          , List<Point> Answer
                          , uint        seed
                          , int         PDim
                          , int         nPoints
                          , List<int>   fID
                          , bool        needShuffle = false) {
    Polyhedron? P = null;

    try {
      if (needShuffle) {
        HashSet<Point> origS = new HashSet<Point>(S);
        S.Shuffle(new RandomLC(seed));
        Debug.Assert(origS.SetEquals(S));
      }

      P = GiftWrapping.WrapPolyhedron(S);
      Debug.Assert(P is not null, nameof(P) + " != null");
    }
    catch (Exception e) {
      Console.WriteLine("Gift wrapping does not success!");
      GenTest(seed, PDim, nPoints, fID, needShuffle);

      Console.WriteLine(e.Message);

      throw new ArgumentException("Error in gift wrapping!");
    }

    try {
      Assert.That(P.Vertices.SetEquals(Answer), "The set of vertices must be equal.");
    }
    catch (Exception e) {
      Console.WriteLine("Gift wrapping success. But sets of vertices do not equal!");
      GenTest(seed, PDim, nPoints, fID, needShuffle);

      Console.WriteLine(e.Message);

      throw new ArgumentException("The set of vertices must be equal.");
    }
  }

  private static void GenTest(uint seed, int PDim, int nPoints, List<int> fID, bool needShuffle) {
    Console.WriteLine();
    Console.WriteLine($"[Test]");
    Console.WriteLine("public void Aux() {");
    Console.WriteLine($"const uint seed   = {seed};");
    Console.WriteLine($"const int PDim    = {PDim};");
    Console.WriteLine($"const int nPoints = {nPoints};");
    Console.WriteLine($"List<int> fID     = new List<int>() {{ {string.Join(", ", fID)} }};");
    Console.WriteLine();
    Console.WriteLine("List<Point> S = (PDim, out List<Point> polytop, fID, nPoints, seed);");
    if (needShuffle) {
      Console.WriteLine("List<Point> origS = new List<Point>(S);");
      Console.WriteLine("S.Shuffle(new RandomLC(seed));");
    }
    Console.WriteLine();
    Console.WriteLine("Polyhedron P = GiftWrapping.WrapPolyhedron(S);");
    Console.WriteLine("Assert.That(P.Vertices.SetEquals(polytop));");
    Console.WriteLine("}");
    Console.WriteLine();
  }

  [Test]
  public void Aux1() {
    const uint seed    = 4191642331;
    const int  PDim    = 3;
    const int  nPoints = 1;
    List<int>  fID     = new List<int>() { 1, 2 };

    List<Point> S = SimplexRND(PDim, out List<Point> polytop, fID, nPoints, seed);

    List<Point> origS = new List<Point>(S);
    S.Shuffle(new RandomLC(seed));
    Polyhedron P = GiftWrapping.WrapPolyhedron(S);
    Assert.That(P.Vertices.SetEquals(polytop));
  }

  [Test]
  public void Aux() {
    const uint seed    = 2056099428;
    const int  PDim    = 3;
    const int  nPoints = 1;
    List<int>  fID     = new List<int>() { 2 };

    List<Point> S     = SimplexRND(PDim,  out List<Point> polytop, fID, nPoints, seed);
    List<Point> origS = new List<Point>(S);
    S.Shuffle(new RandomLC(seed));

    var hpABD    = new HyperPlane(new AffineBasis(new List<Point>() { S[0], S[1], S[3] }));
    var distABD = S.Select(s => hpABD.Eval(s));
    var hpBDC    = new HyperPlane(new AffineBasis(new List<Point>() { S[1], S[3], S[2] }));
    var distBDC = S.Select(s => hpBDC.Eval(s));
    var hpBDE    = new HyperPlane(new AffineBasis(new List<Point>() { S[1], S[3], S[4] }));
    var distBDE = S.Select(s => hpBDE.Eval(s));

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);
    Assert.That(P.Vertices.SetEquals(polytop));
  }

}
