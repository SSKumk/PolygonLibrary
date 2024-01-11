using System.Diagnostics;
using System.Numerics;
using CGLibrary;

namespace Tests.ToolsTests;

public class TestsPolytopes<TNum, TConv> : TestsBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>
  where TConv : INumConvertor<TNum> {

#region Pre-defined objects
  public static readonly List<Point> Octahedron3D_list = MakePointsOnSphere_3D(2, 4, true, true);
  public static readonly List<Point> Pyramid3D_list    = MakePointsOnSphere_3D(2, 4, true);
  public static readonly List<Point> Simplex2D_list    = Simplex_list(2);
  public static readonly List<Point> Simplex3D_list    = Simplex_list(3);
  public static readonly List<Point> Simplex4D_list    = Simplex_list(4);
  public static readonly List<Point> Simplex5D_list    = Simplex_list(5);
  public static readonly List<Point> SimplexRND2D_list = SimplexRND_list(2);
  public static readonly List<Point> SimplexRND3D_list = SimplexRND_list(3);
  public static readonly List<Point> SimplexRND4D_list = SimplexRND_list(4);
  public static readonly List<Point> SimplexRND5D_list = SimplexRND_list(5);
  public static readonly List<Point> Cube2D_list       = Cube_list(2);
  public static readonly List<Point> Cube3D_list       = Cube_list(3);
  public static readonly List<Point> Cube4D_list       = Cube_list(4);
  public static readonly List<Point> Cube5D_list       = Cube_list(5);


  public static readonly ConvexPolytop Cube3D       = Cube(3).CPolytop;
  public static readonly ConvexPolytop Cube4D       = Cube(4).CPolytop;
  public static readonly ConvexPolytop Simplex3D    = Simplex(3).CPolytop;
  public static readonly ConvexPolytop Simplex4D    = Simplex(4).CPolytop;
  public static readonly ConvexPolytop Octahedron3D = new GiftWrapping(Octahedron3D_list).CPolytop;


  public static readonly FaceLattice Cube3D_FL    = CubeFL(3);
  public static readonly FaceLattice Cube4D_FL    = CubeFL(4);
  public static readonly FaceLattice Simplex3D_FL = Simplex(3).FaceLattice;
  public static readonly FaceLattice Simplex4D_FL = Simplex(4).FaceLattice;


  public static readonly Matrix rotate3D_45XY = MakeRotationMatrix(3, 1, 2, TNum.Pi / TConv.FromInt(4));
  public static readonly Matrix rotate4D_45XY = MakeRotationMatrix(4, 1, 2, TNum.Pi / TConv.FromInt(4));
#endregion

#region Polytopes and Polytopes-list Fabrics
  public static List<Point> Cube_list(int           dim) => Cube(dim, out _);
  public static List<Point> CubeRotatedRND_list(int dim) => Rotate(Cube_list(dim), GenLinearBasis(dim).GetMatrix());
  public static List<Point> Simplex_list(int        dim) => Simplex(dim, out _);
  public static List<Point> SimplexRND_list(int     dim) => SimplexRND(dim, out _);

  public static GiftWrapping Cube(int           dim) => new GiftWrapping(Cube_list(dim));
  public static GiftWrapping CubeRotatedRND(int dim) => new GiftWrapping(CubeRotatedRND_list(dim));
  public static GiftWrapping Simplex(int        dim) => new GiftWrapping(Simplex(dim, out _));
  public static GiftWrapping SimplexRND(int     dim) => new GiftWrapping(SimplexRND(dim, out _));

  public static GiftWrapping Sphere(int dim, int theta, int phi, TNum radius) =>
    new GiftWrapping(Sphere_list(dim, theta, phi, radius));


  public static FaceLattice CubeFL(int dim) => Cube(dim).FaceLattice;
  public static FaceLattice CubeRotatedRND_FL(int dim) => CubeRotatedRND(dim).FaceLattice;
  public static FaceLattice SimplexFL(int dim) => SimplexRND(dim).FaceLattice;
  public static FaceLattice SphereFL(int dim, int theta, int phi, TNum radius) => Sphere(dim, theta, phi, radius).FaceLattice;
#endregion

#region Polytopes generators
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
                               , bool              needShuffle = false) {
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
          List<Point> smallCube = TestsPolytopes<TNum, TConv>.Cube(cubeDim - dim, out List<Point> _);
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
  /// Generates a list of Cartesian coordinates for points on a nD-sphere.
  /// </summary>
  /// <param name="dim">The dimension of the sphere. It is greater than 1.</param>
  /// <param name="thetaPoints">The number of points at each zenith angle. Theta in [0, Pi].
  ///  thetaPoints should be greater than 2 for proper calculation.</param>
  /// <param name="phiPoints">The number of points by azimuthal angle. Phi in [0, 2*Pi).</param>
  /// <param name="radius">The radius of a sphere.</param>
  /// <returns>A list of points on the sphere.</returns>
  public static List<Point> Sphere_list(int dim, int thetaPoints, int phiPoints, TNum radius) {
    Debug.Assert(dim > 1, "The dimension of a sphere must be 2 or greater.");
    // Phi in [0, 2*Pi)
    // Theta in [0, Pi]
    HashSet<Point> Ps        = new HashSet<Point>();
    int            N         = dim - 2;
    TNum           thetaStep = Tools.PI / TConv.FromInt(thetaPoints);
    TNum           phiStep   = Tools.PI2 / TConv.FromInt(phiPoints);

    List<TNum> thetaAll = new List<TNum>();
    for (int i = 0; i <= thetaPoints; i++) {
      thetaAll.Add(thetaStep * TConv.FromInt(i));
    }

    // цикл по переменной [0, 2*Pi)
    for (int i = 0; i < phiPoints; i++) {
      TNum phi = phiStep * TConv.FromInt(i);

      // соберём все наборы углов вида [Phi, t1, t2, t3, ..., t(n-2)]
      List<List<TNum>> thetaAngles_prev = new List<List<TNum>>() { new List<TNum>() { phi } };
      List<List<TNum>> thetaAngles      = new List<List<TNum>>() { new List<TNum>() { phi } };
      // сколько раз нужно углы добавлять
      for (int k = 0; k < N; k++) {
        thetaAngles.Clear();
        // формируем наборы добавляя к каждому текущему набору всевозможные углы из theta all
        foreach (List<TNum> angle in thetaAngles_prev) {
          foreach (TNum theta in thetaAll) {
            thetaAngles.Add(new List<TNum>(angle) { theta });
          }
        }
        thetaAngles_prev = new List<List<TNum>>(thetaAngles);
      }

      foreach (List<TNum> s in thetaAngles) {
        List<TNum> point = new List<TNum>();
        // собрали 1 и 2 координаты
        TNum sinsN = Tools.One;
        for (int k = 1; k <= N; k++) { sinsN *= TNum.Sin(s[k]); }
        point.Add(radius * TNum.Cos(phi) * sinsN);
        point.Add(radius * TNum.Sin(phi) * sinsN);

        //добавляем серединные координаты
        for (int j = 2; j <= N; j++) {
          TNum sinsJ = Tools.One;
          for (int k = 1; k < j; k++) {
            sinsJ *= TNum.Sin(s[k]);
          }
          point.Add(radius * TNum.Cos(s[j]) * sinsJ);
        }

        // последнюю координату
        if (dim > 2) {
          point.Add(radius * TNum.Cos(s[1]));
        }

        // точка готова, добавляем в наш массив
        Ps.Add(new Point(point.ToArray()));
      }
    }

    return Ps.ToList();
  }

  /// <summary>
  /// Generates a list of Cartesian coordinates for points on a 3D-sphere.
  /// </summary>
  /// <param name="thetaDivisions">The number of divisions by zenith angle. thetaDivisions should be greater than or equal to 3 for proper calculation.
  /// Both poles are skipped by default.</param>
  /// <param name="phiDivisions">The number of divisions by azimuthal angle. Phi in [0, 2*Pi).</param>
  /// <param name="addUpperPole">A boolean flag indicating whether to add the upper pole to the list of points.</param>
  /// <param name="addBottomPole">A boolean flag indicating whether to add the bottom pole to the list of points.</param>
  /// <returns>A list of points on the sphere.</returns>
  public static List<Point> MakePointsOnSphere_3D(int  thetaDivisions
                                                , int  phiDivisions
                                                , bool addUpperPole  = false
                                                , bool addBottomPole = false) {
    List<Point> points = new List<Point>();
    if (addUpperPole) {
      points.Add(new Point(new TNum[] { TNum.Zero, TNum.Zero, TNum.One }));
    }
    if (addBottomPole) {
      points.Add(new Point(new TNum[] { TNum.Zero, TNum.Zero, -TNum.One }));
    }

    TNum thetaStep = Tools.PI / TConv.FromInt(thetaDivisions);
    TNum phiStep   = Tools.PI2 / TConv.FromInt(phiDivisions);

    for (int i = 1; i < thetaDivisions; i++) {
      TNum theta = thetaStep * TConv.FromInt(i);
      for (int j = 0; j < phiDivisions; j++) {
        TNum phi = phiStep * TConv.FromInt(j);

        TNum x = TNum.Sin(theta) * TNum.Cos(phi);
        TNum y = TNum.Sin(theta) * TNum.Sin(phi);
        TNum z = TNum.Cos(theta);

        points.Add(new Point(new TNum[] { x, y, z }));
      }
    }

    return points;
  }

}
