using System.Diagnostics;
using System.Numerics;
using CGLibrary;

namespace Tests.ToolsTests;

public class ToolsTests<TNum, TConv> : Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// The random engine.
  /// </summary>
  private static GRandomLC _random = new GRandomLC();

#region Auxiliary functions
  /// <summary>
  /// Generates a random TNum value in (0,1): a value between 0 and 1, excluding the values 0 and 1.
  /// </summary>
  /// <returns>The generated random TNum value.</returns>
  private static TNum GenInner(GRandomLC rnd) { return rnd.NextPreciseInt(1, 999) / TConv.FromDouble(1000.0); }

  /// <summary>
  /// Generates a non-zero random vector of the specified dimension. Each coordinate: [-0.5, 0.5] \ {0}.
  /// </summary>
  /// <param name="dim">The dimension of the vector.</param>
  /// <param name="random">If null then _random be used.</param>
  /// <returns>A random vector.</returns>
  private static Vector GenVector(int dim, GRandomLC? random = null) {
    GRandomLC rnd = random ?? _random;

    Vector res;
    TNum[] v = new TNum[dim];
    do {
      for (int i = 0; i < dim; i++) {
        v[i] = rnd.NextPrecise() - Tools.HalfOne;
      }
      res = new Vector(v);
    } while (res.IsZero);

    return res;
  }

  /// <summary>
  /// Generates a linear combination of the given points.
  /// </summary>
  /// <param name="points">The list of point to lin-combine.</param>
  /// <param name="random">The random to be used. If null, the _random be used.</param>
  /// <returns>A linear combination of the given points.</returns>
  private static Point GenConvexCombination(IReadOnlyCollection<Point> points, GRandomLC? random = null) {
    GRandomLC  rnd = random ?? _random;
    List<TNum> ws  = new List<TNum>();

    TNum difA = Tools.One;
    for (int i = 0; i < points.Count - 1; i++) {
      TNum alpha = GenInner(rnd) * difA;
      ws.Add(alpha);
      difA -= alpha;
    }
    ws.Add(difA);

    Point res = Point.LinearCombination(points, ws);

    return res;
  }

  /// <summary>
  /// Adds points to an existing simplex.
  /// </summary>
  /// <param name="facesDim">The dimensions of the faces where points will be added. If null, no points will be added.</param>
  /// <param name="amount">The number of points to add on each faceDim.</param>
  /// <param name="simplex">The initial simplex to which points will be added.</param>
  /// <param name="random">The random to be used. If null, the _random be used.</param>
  /// <returns>A new simplex with the added points.</returns>
  private static List<Point> AddPointsToSimplex(IEnumerable<int>?    facesDim
                                       , int                  amount
                                       , IReadOnlyList<Point> simplex
                                       , GRandomLC?           random = null) {
    GRandomLC rnd = random ?? _random;

    List<Point> Simplex = new List<Point>(simplex);

    if (facesDim is not null) {
      foreach (int dim in facesDim) {
        Debug.Assert(dim <= simplex[0].Dim);

        List<List<Point>> faces = simplex.Subsets(dim + 1);

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
  /// Generates a random non-zero vector, each coordinate [-50, 50] \ {0}.
  /// </summary>
  /// <param name="dim">The dimension of the vector.</param>
  /// <param name="random">The random to be used. If null, the _random be used.</param>
  /// <returns>A random non-zero vector.</returns>
  private static Vector GenShift(int dim, GRandomLC? random = null) {
    GRandomLC rnd = random ?? _random;
    return GenVector(dim) * rnd.NextPreciseInt(1, 10);
  }

  /// <summary>
  /// Shift given swarm by given vector
  /// </summary>
  /// <param name="S">S to be shifted</param>
  /// <param name="shift">Vector to shift</param>
  /// <returns></returns>
  private static List<Point> Shift(List<Point> S, Vector shift) { return S.Select(s => new Point(s + shift)).ToList(); }

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

  ///<summary>
  /// Method applies a rotation and a shift to two lists of points.
  ///</summary>
  ///<param name="PDim">The dimension of the space in which the points exist.</param>
  ///<param name="P">A reference to the list of points to be transformed.</param>
  ///<param name="S">A reference to the list of points representing the swarm to be transformed.</param>
  /// <param name="seed">The seed to be placed into GRandomLC. If null, the _random be used.</param>
  public static void ShiftAndRotate(int PDim, ref List<Point> P, ref List<Point> S, uint? seed = null) {
    GRandomLC random   = seed is null ? _random : new GRandomLC(seed);

    Matrix    rotation = GenRotation(PDim);
    Vector    shift    = GenVector(PDim, random) * _random.NextPreciseInt(1, 10);

    P = Rotate(P, rotation);
    P = Shift(P, shift);

    S = Rotate(S, rotation);
    S = Shift(S, shift);
  }

  /// <summary>
  /// Rotates the given swarm of points by given unitary matrix.
  /// </summary>
  /// <param name="S">The swarm of points to rotate.</param>
  /// <param name="rotation">Matrix to rotate a swarm.</param>
  /// <returns>The rotated swarm of points.</returns>
  public static List<Point> Rotate(IEnumerable<Point> S, Matrix rotation) {
    Debug.Assert
      (S.First().Dim == rotation.Rows, "ToolsTests.Rotate: the dimension of points must be equal to the count of rotation rows.");

    IEnumerable<Vector> rotated = S.Select(s => new Vector(s) * rotation);

    return rotated.Select(v => new Point(v)).ToList();
  }

  /// <summary>
  /// Generates a rotation matrix for a given dimension, axes, and angle.
  /// </summary>
  /// <param name="dim">The dimension of the matrix.</param>
  /// <param name="ax1">The first axis for the rotation [1,2, ... d].</param>
  /// <param name="ax2">The second axis for the rotation [1,2, ... d].</param>
  /// <param name="angle">The angle of rotation in radians.</param>
  /// <returns>A rotation matrix of type Matrix.</returns>
  public static Matrix MakeRotationMatrix(int dim, int ax1, int ax2, TNum angle) {
#if DEBUG
    Debug.Assert(dim > 1, "ToolsTest.GenRotationMatrix: the dimension must be greater than 1.");
    Debug.Assert(ax1 > 0 && ax1 < dim + 1, "ToolsTest.GenRotationMatrix: the rotation axis must be one of [1,2, ..., d].");
    Debug.Assert(ax2 > 0 && ax2 < dim + 1, "ToolsTest.GenRotationMatrix: the rotation axis must be one of [1,2, ..., d].");
#endif

    ax1--; //чтобы к индексам привести
    ax2--; //чтобы к индексам привести
    TNum[,] rotM = Matrix.Eye(dim);
    for (int r = 0; r < dim; r++) {
      for (int k = 0; k < dim; k++) {
        if ((r == ax1 && k == ax1) || (r == ax2 && k == ax2)) { // индексы совпали
          rotM[r, k] = TNum.Cos(angle);
        }
        if (r == ax1 && k == ax2) { // у первого синуса минус
          rotM[r, k] = -TNum.Sin(angle);
        }
        if (r == ax2 && k == ax1) {
          rotM[r, k] = TNum.Sin(angle);
        }
      }
    }

    return new Matrix(rotM);
  }
#endregion


#region Main generators
  /// <summary>
  /// Generates a full-dimension hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="pureCube">The list of cube vertices of given dimension.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The amount of points to be placed into random set of faces of faceDim dimension.</param>
  /// <param name="seed">The seed to be placed into GRandomLC. If null, the _random be used.</param>
  /// <param name="needShuffle"></param>
  /// <returns>A list of points representing the hypercube possibly with inner points.</returns>
  public static List<Point> Cube(int               cubeDim
                               , out List<Point>   pureCube
                               , IEnumerable<int>? facesDim    = null
                               , int               amount      = 1
                               , uint?             seed        = null
                               , bool              needShuffle = false
 ) {
    GRandomLC random = seed is null ? _random : new GRandomLC(seed);

    if (cubeDim == 1) {
      List<Point> oneDimCube = new List<Point>() { new Point(new TNum[] { TNum.Zero }), new Point(new TNum[] { TNum.One }) };
      pureCube = oneDimCube;

      return oneDimCube;
    }

    List<List<TNum>> cube_prev = new List<List<TNum>>();
    List<List<TNum>> cube      = new List<List<TNum>>();
    cube_prev.Add(new List<TNum>() { TNum.Zero });
    cube_prev.Add(new List<TNum>() { TNum.One });

    for (int i = 1; i < cubeDim; i++) {
      cube.Clear();

      foreach (List<TNum> coords in cube_prev) {
        cube.Add(new List<TNum>(coords) { TNum.Zero });
        cube.Add(new List<TNum>(coords) { TNum.One });
      }

      cube_prev = new List<List<TNum>>(cube);
    }

    List<Point> Cube = new List<Point>();

    foreach (List<TNum> v in cube) {
      Cube.Add(new Point(v.ToArray()));
    }

    pureCube = new List<Point>(Cube);

    if (facesDim is not null) {                                      // накидываем точки на грани нужных размерностей
      List<int> vectorsInds = Enumerable.Range(0, cubeDim).ToList(); // генерируем список [0,1,2, ... , cubeDim - 1]
      foreach (int dim in facesDim) {
        List<List<int>> allPoints = vectorsInds.Subsets(cubeDim - dim);

        if (cubeDim == dim) {
          for (int i = 0; i < amount; i++) {
            TNum[] point = new TNum[cubeDim];
            for (int j = 0; j < cubeDim; j++) {
              point[j] = GenInner(random);
            }

            Cube.Add(new Point(point));
          }

          continue;
        }

        // Если размерность грани, куда нужно поместить точку меньше размерности куба
        foreach (List<int> fixedInd in allPoints) {
          List<Point> smallCube = ToolsTests<TNum, TConv>.Cube(cubeDim - dim, out List<Point> _);
          foreach (Point pointCube in smallCube) {
            for (int k = 0; k < amount; k++) {
              TNum[] point = new TNum[cubeDim];
              int    s     = 0;
              for (int j = 0; j < cubeDim; j++) {
                point[j] = -TNum.One;
              }

              foreach (int ind in fixedInd) { // на выделенных местах размещаем 1-ки и 0-ки
                point[ind] = pointCube[s];
                s++;
              }

              for (int j = 0; j < cubeDim; j++) {
                if (Tools.EQ(point[j], -TNum.One)) {
                  point[j] = GenInner(random);
                }
              }

              Cube.Add(new Point(point));
            }
          }
        }
      }
    }

    if (needShuffle) {
      Cube.Shuffle(random);
    }

    return Cube;
  }

  /// <summary>
  /// Generates a d-ortho-based-simplex in d-space.
  /// </summary>
  /// <param name="simplexDim">The dimension of the simplex.</param>
  /// <param name="pureSimplex">Only vertices of the simplex.</param>
  /// <param name="facesDim">The dimensions of the faces of the simplex to put points on.</param>
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <param name="seed">The seed to be placed into GRandomLC. If null, the _random be used.</param>
  /// <returns>A list of points representing the axis-based simplex.</returns>
  public static List<Point> Simplex(int               simplexDim
                           , out List<Point>   pureSimplex
                           , IEnumerable<int>? facesDim = null
                           , int               amount   = 1
                           , uint?             seed     = null) {
    GRandomLC   random  = seed is null ? _random : new GRandomLC(seed);
    List<Point> simplex = new List<Point> { new Point(new TNum[simplexDim]) };

    for (int i = 1; i < simplexDim + 1; i++) {
      simplex.Add(new Point(Vector.CreateOrth(simplexDim, i)));
    }
    pureSimplex = new List<Point>(simplex);

    return AddPointsToSimplex(facesDim, amount, simplex, random);
  }

  /// <summary>
  /// Generates a d-simplex in d-space.
  /// </summary>
  /// <param name="simplexDim">The dimension of the simplex.</param>
  /// <param name="pureSimplex">Only vertices of the simplex.</param>
  /// <param name="facesDim">The dimensions of the faces of the simplex to put points on.</param>
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <param name="seed">The seed to be placed into GRandomLC. If null, the _random be used.</param>
  /// <returns>A list of points representing the random simplex.</returns>
  public static List<Point> SimplexRND(int               simplexDim
                              , out List<Point>   pureSimplex
                              , IEnumerable<int>? facesDim = null
                              , int               amount   = 1
                              , uint?             seed     = null) {
    GRandomLC random = seed is null ? _random : new GRandomLC(seed);

    List<Point> simplex = new List<Point>();
    do {
      for (int i = 0; i < simplexDim + 1; i++) {
        simplex.Add(new Point(TConv.FromInt(10) * GenVector(simplexDim, random)));
      }
    } while (!new AffineBasis(simplex).IsFullDim);
    List<Point> aux = new List<Point>(simplex);
    aux.RemoveAt(0);
    Debug.Assert(new HyperPlane(new AffineBasis(aux)).FilterIn(simplex).Count() != simplex.Count);

    pureSimplex = new List<Point>(simplex);

    return AddPointsToSimplex(facesDim, amount, simplex, random);
  }
#endregion

}
