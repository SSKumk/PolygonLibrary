using System.Diagnostics;
using System.Numerics;
using CGLibrary;

namespace Tests.ToolsTests;

public class TestsPolytopes<TNum, TConv> : TestsBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>
  where TConv : INumConvertor<TNum> {

#region Pre-defined objects
  public static readonly List<Vector> Octahedron3D_list = MakePointsOnSphere_3D(2, 4, true, true);
  public static readonly List<Vector> Pyramid3D_list    = MakePointsOnSphere_3D(2, 4, true);
  public static readonly List<Vector> Simplex2D_list    = Simplex_list(2);
  public static readonly List<Vector> Simplex3D_list    = Simplex_list(3);
  public static readonly List<Vector> Simplex4D_list    = Simplex_list(4);
  public static readonly List<Vector> Simplex5D_list    = Simplex_list(5);
  public static readonly List<Vector> SimplexRND2D_list = SimplexRND_list(2);
  public static readonly List<Vector> SimplexRND3D_list = SimplexRND_list(3);
  public static readonly List<Vector> SimplexRND4D_list = SimplexRND_list(4);
  public static readonly List<Vector> SimplexRND5D_list = SimplexRND_list(5);
  public static readonly List<Vector> Cube2D_list       = Cube_list(2);
  public static readonly List<Vector> Cube3D_list       = Cube_list(3);
  public static readonly List<Vector> Cube4D_list       = Cube_list(4);
  public static readonly List<Vector> Cube5D_list       = Cube_list(5);


  public static readonly ConvexPolytop Cube3D       = ConvexPolytop.CreateFromPoints(Cube_list(3));
  public static readonly ConvexPolytop Cube4D       = ConvexPolytop.CreateFromPoints(Cube_list(4));
  public static readonly ConvexPolytop Simplex3D    = ConvexPolytop.CreateFromPoints(Simplex(3, out _));
  public static readonly ConvexPolytop Simplex4D    = ConvexPolytop.CreateFromPoints(Simplex(4, out _));
  public static readonly ConvexPolytop Octahedron3D = ConvexPolytop.CreateFromPoints(Octahedron3D_list.ToHashSet());


  public static readonly Matrix rotate3D_45XY = MakeRotationMatrix(3, 1, 2, TNum.Pi / TConv.FromInt(4));
  public static readonly Matrix rotate4D_45XY = MakeRotationMatrix(4, 1, 2, TNum.Pi / TConv.FromInt(4));
#endregion

#region Polytopes and Polytopes-list Fabrics
  public static List<Vector> Cube_list(int           dim) => Cube01(dim, out _);
  private static List<Vector> CubeRotatedRND_list(int dim) => Rotate(Cube_list(dim), Matrix.GenONMatrix(dim));
  public static  List<Vector> Simplex_list(int        dim) => Simplex(dim, out _);
  public static  List<Vector> SimplexRND_list(int     dim) => SimplexRND(dim, out _);

  private static GiftWrapping CubeGW(int         dim) => new GiftWrapping(Cube_list(dim));
  private static GiftWrapping CubeRotatedRND(int dim) => new GiftWrapping(CubeRotatedRND_list(dim));
  public static  GiftWrapping Simplex(int        dim) => new GiftWrapping(Simplex(dim, out _));
  private static GiftWrapping SimplexRND(int     dim) => new GiftWrapping(SimplexRND(dim, out _));

  private static GiftWrapping Sphere(int dim, int theta, int phi, TNum radius)
    => new GiftWrapping(Sphere_list(dim, theta, phi, radius));


  public static FaceLattice CubeFL(int dim) => CubeGW(dim).ConstructFL();
  public static FaceLattice CubeRotatedRND_FL(int dim) => CubeRotatedRND(dim).ConstructFL();
  public static FaceLattice SimplexFL(int dim) => SimplexRND(dim).ConstructFL();
  public static FaceLattice SphereFL(int dim, int theta, int phi, TNum radius) => Sphere(dim, theta, phi, radius).ConstructFL();
#endregion

#region Polytopes generators
  /// <summary>
  /// Generates a full-dimension 0-1-hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="pureCube">The list of cube vertices of given dimension.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The number of points to be placed into a random set of faces of faceDim dimension.</param>
  /// <param name="rnd">The random engine to be used. If null, the _random be used.</param>
  /// <param name="needShuffle"></param>
  /// <returns>A list of points representing the hypercube, possibly with inner points.</returns>
  public static List<Vector> Cube01(
      int               cubeDim
    , out List<Vector>  pureCube
    , IEnumerable<int>? facesDim    = null
    , int               amount      = 1
    , GRandomLC?        rnd         = null
    , bool              needShuffle = false
    )
    => Cube01
      (
       cubeDim
     , Tools.One
     , out pureCube
     , facesDim
     , amount
     , rnd
     , needShuffle
      );

  /// <summary>
  /// Generates a full-dimension hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="d">The value of non-zero coordinates.</param>
  /// <param name="pureCube">The list of cube vertices of given dimension.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The number of points to be placed into a random set of faces of faceDim dimension.</param>
  /// <param name="rnd">The random engine to be used. If null, the _random be used.</param>
  /// <param name="needShuffle"></param>
  /// <returns>A list of points representing the hypercube, possibly with inner points.</returns>
  private static List<Vector> Cube01(
      int               cubeDim
    , TNum              d
    , out List<Vector>  pureCube
    , IEnumerable<int>? facesDim    = null
    , int               amount      = 1
    , GRandomLC?        rnd         = null
    , bool              needShuffle = false
    ) {
    GRandomLC random = rnd ?? _random;

    if (cubeDim == 1) {
      List<Vector> oneDimCube = new List<Vector>() { new Vector(new TNum[] { TNum.Zero }), new Vector(new TNum[] { d }) };
      pureCube = oneDimCube;

      return oneDimCube;
    }

    List<List<TNum>> cube_prev = new List<List<TNum>>();
    List<List<TNum>> cube      = new List<List<TNum>>();
    cube_prev.Add(new List<TNum>() { TNum.Zero });
    cube_prev.Add(new List<TNum>() { d });

    for (int i = 1; i < cubeDim; i++) {
      cube.Clear();

      foreach (List<TNum> coords in cube_prev) {
        cube.Add(new List<TNum>(coords) { TNum.Zero });
        cube.Add(new List<TNum>(coords) { d });
      }

      cube_prev = new List<List<TNum>>(cube);
    }

    List<Vector> Cube = new List<Vector>();

    foreach (List<TNum> v in cube) {
      Cube.Add(new Vector(v.ToArray()));
    }

    pureCube = new List<Vector>(Cube);

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

            Cube.Add(new Vector(point));
          }

          continue;
        }

        // Если размерность грани, куда нужно поместить точку меньше размерности куба
        foreach (List<int> fixedInd in allPoints) {
          List<Vector> smallCube = Cube01(cubeDim - dim, out List<Vector> _);
          foreach (Vector pointCube in smallCube) {
            for (int k = 0; k < amount; k++) {
              TNum[] point = new TNum[cubeDim];
              int    s     = 0;
              for (int j = 0; j < cubeDim; j++) {
                point[j] = -d;
              }

              foreach (int ind in fixedInd) { // на выделенных местах размещаем 1-ки и 0-ки
                point[ind] = pointCube[s];
                s++;
              }

              for (int j = 0; j < cubeDim; j++) {
                if (Tools.EQ(point[j], -d)) {
                  point[j] = GenInner(random);
                }
              }

              Cube.Add(new Vector(point));
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
  /// <param name="amount">The number of points to be placed into each face of faceDim dimension.</param>
  /// <param name="rnd">The random engine to be used. If null, the _random be used.</param>
  /// <returns>A list of points representing the axis-based simplex.</returns>
  public static List<Vector> Simplex(
      int               simplexDim
    , out List<Vector>  pureSimplex
    , IEnumerable<int>? facesDim = null
    , int               amount   = 1
    , GRandomLC?        rnd      = null
    ) {
    GRandomLC random = rnd ?? _random;

    List<Vector> simplex = new List<Vector> { new Vector(new TNum[simplexDim]) };

    for (int i = 1; i < simplexDim + 1; i++) {
      simplex.Add(new Vector(Vector.MakeOrth(simplexDim, i)));
    }
    pureSimplex = new List<Vector>(simplex);

    return AddPointsToEachFaceInSimplex(facesDim, amount, simplex, random);
  }

  /// <summary>
  /// Generates a d-simplex in d-space.
  /// </summary>
  /// <param name="simplexDim">The dimension of the simplex.</param>
  /// <param name="pureSimplex">Only vertices of the simplex.</param>
  /// <param name="facesDim">The dimensions of the faces of the simplex to put points on.</param>
  /// <param name="amount">The number of points to be placed into each face of faceDim dimension.</param>
  /// <param name="rnd">The random engine to be used. If null, the _random be used.</param>
  /// <returns>A list of points representing the random simplex.</returns>
  public static List<Vector> SimplexRND(
      int               simplexDim
    , out List<Vector>  pureSimplex
    , IEnumerable<int>? facesDim = null
    , int               amount   = 1
    , GRandomLC?        rnd      = null
    ) {
    GRandomLC random = rnd ?? _random;

    List<Vector> simplex = new List<Vector>();
    do {
      for (int i = 0; i < simplexDim + 1; i++) {
        simplex.Add(new Vector(Vector.GenVector(simplexDim, TConv.FromInt(0), TConv.FromInt(10), random)));
      }
    } while (!new AffineBasis(simplex).IsFullDim);
    List<Vector> aux = new List<Vector>(simplex);
    aux.RemoveAt(0);
    Debug.Assert(new HyperPlane(new AffineBasis(aux)).FilterIn(simplex).Count() != simplex.Count);

    pureSimplex = new List<Vector>(simplex);

    return AddPointsToEachFaceInSimplex(facesDim, amount, simplex, random);
  }

  /// <summary>
  /// Makes the cyclic polytop in specified dimension with specified amount of points.
  /// </summary>
  /// <param name="pDim">The dimension of the cyclic polytop.</param>
  /// <param name="amountOfPoints">The amount of vertices in cyclic polytop.</param>
  /// <param name="step">The step of increasing the moment on the moments curve. init = 1 + step.</param>
  /// <returns></returns>
  public static List<Vector> CyclicPolytop(int pDim, int amountOfPoints, TNum step) {
    Debug.Assert
      (
       amountOfPoints > pDim
     , $"TestPolytopes.Cyclic: The amount of points must be greater than the dimenstion of the space. Dim = {pDim}, amount = {amountOfPoints}"
      );
    List<Vector> cycP      = new List<Vector>() { new Vector(pDim) };
    TNum         baseCoord = Tools.One + step;
    for (int i = 1; i < amountOfPoints; i++) {
      TNum[] point      = new TNum[pDim];
      TNum   coordinate = baseCoord;
      TNum   multiplyer = coordinate;
      for (int t = 0; t < pDim; t++) { // (i, i^2, i^3 , ... , i^d)
        point[t]   =  coordinate;
        coordinate *= multiplyer;
      }
      cycP.Add(new Vector(point));
      baseCoord += step;
    }

    return cycP;
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
  private static List<Vector> AddPointsToEachFaceInSimplex(
      IEnumerable<int>?     facesDim
    , int                   amount
    , IReadOnlyList<Vector> simplex
    , GRandomLC?            random = null
    ) {
    GRandomLC rnd = random ?? _random;

    List<Vector> Simplex = new List<Vector>(simplex);

    if (facesDim is not null) {
      foreach (int dim in facesDim) {
        Debug.Assert(dim <= simplex[0].Dim);

        List<List<Vector>> faces = simplex.Subsets(dim + 1);

        for (int k = 0; k < amount; k++) {
          int    ind    = rnd.NextInt(0, faces.Count - 1);
          Vector inFace = GenConvexCombination(faces[ind], random);
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
  public static List<Vector> Sphere_list(int dim, int thetaPoints, int phiPoints, TNum radius) {
    Debug.Assert(dim > 1, "The dimension of a sphere must be 2 or greater.");
    // Phi in [0, 2*Pi)
    // Theta in [0, Pi]
    SortedSet<Vector> Ps        = new SortedSet<Vector>();
    int               N         = dim - 2;
    TNum              thetaStep = Tools.PI / TConv.FromInt(thetaPoints);
    TNum              phiStep   = Tools.PI2 / TConv.FromInt(phiPoints);

    List<TNum> thetaAll = new List<TNum>();
    for (int i = 0; i <= thetaPoints; i++) {
      thetaAll.Add(thetaStep * TConv.FromInt(i));
    }

    // цикл по переменной [0, 2*Pi)
    for (int i = 0; i < phiPoints; i++) {
      TNum phi = phiStep * TConv.FromInt(i);

      // соберём все наборы углов вида [Phi, t1, t2, t3, ..., t(n-2)]
      // где t_i принимают все возможные свои значения из theta_all
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
        if (dim >= 4) { // Их нет для 2Д и 3Д сфер
          TNum sinsJ = Tools.One;
          for (int j = 2; j <= N; j++) {
            sinsJ *= TNum.Sin(s[j - 1]);
            point.Add(radius * TNum.Cos(s[j]) * sinsJ);
          }
        }

        // Было так:
        // добавляем серединные координаты
        //       for (int j = 2; j <= N; j++) {
        //         TNum sinsJ = Tools.One;
        //         for (int k = 1; k < j; k++) { // Возможно стоит
        //           sinsJ *= TNum.Sin(s[k]);
        //         }
        //         point.Add(radius * TNum.Cos(s[j]) * sinsJ);
        //       }

        // последнюю координату
        if (dim >= 3) { // У 2Д сферы её нет
          point.Add(radius * TNum.Cos(s[1]));
        }

        // точка готова, добавляем в наш массив
        Ps.Add(new Vector(point.ToArray()));
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
  public static List<Vector> MakePointsOnSphere_3D(
      int  thetaDivisions
    , int  phiDivisions
    , bool addUpperPole  = false
    , bool addBottomPole = false
    ) {
    List<Vector> points = new List<Vector>();
    if (addUpperPole) {
      points.Add(new Vector(new TNum[] { TNum.Zero, TNum.Zero, TNum.One }));
    }
    if (addBottomPole) {
      points.Add(new Vector(new TNum[] { TNum.Zero, TNum.Zero, -TNum.One }));
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

        points.Add(new Vector(new TNum[] { x, y, z }));
      }
    }

    return points;
  }

}
