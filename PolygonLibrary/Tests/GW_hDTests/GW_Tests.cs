using System.Diagnostics;
using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;
using PolygonLibrary.Toolkit;


namespace Tests.GW_hDTests;

[TestFixture]
public class GW_Tests {

  /// <summary>
  /// The random engine.
  /// </summary>
  private static RandomLC _random = new RandomLC(0);

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
  private double GenInner() {
    double w = _random.NextDouble();

    while (Tools.EQ(w) || Tools.EQ(w, 1)) {
      // while (Tools.LT(w,0.01) || Tools.GT(w, 0.99)) {
      w = _random.NextDouble();
    }

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
  /// <returns>A linear combination of the given points.</returns>
  private Point GenConvexCombination(List<Point> points) {
    List<double> ws = new List<double>();

    double difA = 1;
    for (int i = 0; i < points.Count - 1; i++) {
      double alpha = GenInner() * difA;
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
  /// <returns>A new simplex with the added points.</returns>
  private List<Point> AddPointsToSimplex(IEnumerable<int>? facesDim, int amount, IReadOnlyList<Point> simplex) {
    List<Point> Simplex = new List<Point>(simplex);

    if (facesDim is not null) {
      foreach (int dim in facesDim) {
        Debug.Assert(dim <= simplex[0].Dim);

        List<List<Point>> faces = Subsets(simplex, dim + 1);

        for (int k = 0; k < amount; k++) {
          Point inFace = GenConvexCombination(faces[_random.NextInt(0, faces.Count - 1)]);
          Simplex.Add(inFace);
        }
      }
    }

    return Simplex;
  }

  /// <summary>
  /// Generates a d-ortho-based-simplex in d-space. 
  /// </summary>
  /// <param name="simplexDim">The dimension of the simplex.</param>
  /// <param name="pureSimplex">Only vertices of the simplex.</param>
  /// <param name="facesDim">The dimensions of the faces of the simplex to put points on.</param>
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <returns>A list of points representing the simplex.</returns>
  private List<Point> Simplex(int               simplexDim
                            , out List<Point>   pureSimplex
                            , IEnumerable<int>? facesDim = null
                            , int               amount   = 1) {
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
  /// <returns>A list of points representing the simplex.</returns>
  private List<Point> SimplexRND(int               simplexDim
                               , out List<Point>   pureSimplex
                               , IEnumerable<int>? facesDim = null
                               , int               amount   = 1) {
    List<Point> simplex = new List<Point>();
    do {
      for (int i = 0; i < simplexDim + 1; i++) {
        simplex.Add(new Point(10 * GenVector(simplexDim)));
      }
    } while (!new AffineBasis(simplex).IsFullDim);
    List<Point> aux = new List<Point>(simplex);
    aux.RemoveAt(0);
    Debug.Assert(new HyperPlane(new AffineBasis(aux)).FilterIn(simplex).Count() != simplex.Count);


    pureSimplex = new List<Point>(simplex);
    
    return AddPointsToSimplex(facesDim, amount, simplex);
  }

  /// <summary>
  /// Generates a full-dimension hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="pureCube">The list of cube vertices of given dimension.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The amount of points to be placed into random set of faces of faceDim dimension.</param>
  /// <returns>A list of points representing the hypercube possibly with inner points..</returns>
  private List<Point> Cube(int cubeDim, out List<Point> pureCube, IEnumerable<int>? facesDim = null, int amount = 1) {
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
              ind = _random.NextInt(0, cubeDim - 1);
            } while (!constInd.Add(ind));
          }

          int zeroOrOne = _random.NextInt(0, 1);

          foreach (int ind in constInd) {
            point[ind] = zeroOrOne;
          }

          for (int j = 0; j < cubeDim; j++) {
            if (Tools.EQ(point[j], -1)) {
              point[j] = GenInner();
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


  /// <summary>
  /// Generates a non-zero random vector of the specified dimension. Each coordinate: [-0.5, 0.5] \ {0}.
  /// </summary>
  /// <param name="dim">The dimension of the vector.</param>
  /// <returns>A random vector.</returns>
  private static Vector GenVector(int dim) {
    double[] v = new double[dim];

    Vector res;

    do {
      for (int i = 0; i < dim; i++) {
        v[i] = _random.NextDouble() - 0.5;
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
  /// <param name="Swarm">The swarm of points to rotate.</param>
  /// <param name="rotation">Matrix to rotate a swarm.</param>
  /// <returns>The rotated swarm of points.</returns>
  private static List<Point> Rotate(IEnumerable<Point> Swarm, Matrix rotation) {
    IEnumerable<Vector> rotated = Swarm.Select(s => new Vector(s) * rotation);

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
  /// <param name="Swarm">Swarm to be shifted</param>
  /// <param name="shift">Vector to shift</param>
  /// <returns></returns>
  private static List<Point> Shift(List<Point> Swarm, Vector shift) {
    return Swarm.Select(s => new Point(s + shift)).ToList();
  }

  [Test]
  public void GenCubeHDTest() {
    HashSet<Point> Swarm = new HashSet<Point>()
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

    List<Point> S = Cube(3, out List<Point> _);

    Debug.Assert(Swarm.SetEquals(new HashSet<Point>(S)), "Swarm is not equal to generated Cube");
  }

  [Test]
  public void Cube3D() {
    List<Point> Swarm = Cube(3, out List<Point> _);

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);
    Assert.That(P.Vertices.SetEquals(Swarm), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_Rotated_Z45() {
    List<Point>  Swarm = Cube(3, out List<Point> _);
    const double angle = Math.PI / 4;
    double       sin   = Math.Sin(angle);
    double       cos   = Math.Cos(angle);

    double[,] rotationZ45 = { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } };

    List<Point> Rotated = Rotate(Swarm, new Matrix(rotationZ45));

    Polyhedron P = GiftWrapping.WrapPolyhedron(Rotated);
    Assert.That(P.Vertices.SetEquals(Rotated), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_Rotated() {
    uint   saveSeed = _random.Seed;
    int    cubeDim  = 3;
    Matrix rotation = GenRotation(cubeDim);

    List<Point> RotatedCube = Rotate(Cube(cubeDim, out List<Point> _), rotation);

    Polyhedron P = GiftWrapping.WrapPolyhedron(RotatedCube);
    Assert.That(P.Vertices.SetEquals(RotatedCube), $"The set of vertices must be equals. Seed = {saveSeed}");
  }

  [Test]
  public void Cube3D_Shifted() {
    uint        saveSeed    = _random.Seed;
    int         cubeDim     = 3;
    Vector      shift       = GenShift(cubeDim);
    List<Point> ShiftedCube = Shift(Cube(cubeDim, out List<Point> _), shift);

    Polyhedron P = GiftWrapping.WrapPolyhedron(ShiftedCube);
    Assert.That(P.Vertices.SetEquals(ShiftedCube), $"The set of vertices must be equals. Seed = {saveSeed}");
  }

  [Test]
  public void Cube3D_Rotated_Shifted() {
    uint saveSeed = _random.Seed;
    int  cubeDim  = 3;

    List<Point> Swarm = Cube(cubeDim, out List<Point> cube);

    Matrix rotation = GenRotation(cubeDim);
    Vector shift    = GenShift(cubeDim);

    List<Point> RotatedCube        = Rotate(cube, rotation);
    List<Point> RotatedShiftedCube = Shift(RotatedCube, shift);

    Swarm = Rotate(Swarm, rotation);
    Swarm = Shift(Swarm, shift);

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(RotatedShiftedCube), $"The set of vertices must be equals. Seed = {saveSeed}");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D() {
    List<Point> Swarm = Cube(3, out List<Point> cube, new List<int>() { 1 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D() {
    List<Point> Swarm = Cube(3, out List<Point> cube, new List<int>() { 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_3D() {
    List<Point> Swarm = Cube(3, out List<Point> cube, new List<int>() { 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D() {
    List<Point> Swarm = Cube(3, out List<Point> cube, new List<int>() { 1, 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D_3D() {
    List<Point> Swarm = Cube(3, out List<Point> cube, new List<int>() { 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D_3D() {
    List<Point> Swarm = Cube(3, out List<Point> cube, new List<int>() { 1, 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }


  [Test]
  public void Cube4D_withInnerPoints_On_1D() {
    List<Point> Swarm = Cube(4, out List<Point> cube, new List<int>() { 1 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }


  [Test]
  public void Cube4D_withInnerPoints_On_2D() {
    List<Point> Swarm = Cube(4, out List<Point> cube, new List<int>() { 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_3D() {
    List<Point> Swarm = Cube(4, out List<Point> cube, new List<int>() { 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D() {
    List<Point> Swarm = Cube(4, out List<Point> cube, new List<int>() { 1, 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_2D_3D() {
    List<Point> Swarm = Cube(4, out List<Point> cube, new List<int>() { 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D() {
    List<Point> Swarm = Cube(4, out List<Point> cube, new List<int>() { 1, 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D_4D() {
    List<Point> Swarm = Cube
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
      );

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(cube), "The set of vertices must be equals.");
  }

  [Test]
  public void Simplex3D() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, 1, 0 })
      , new Point(new double[] { 0, 0, 1 })
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Assert.That(P.Vertices.SetEquals(Swarm), "The set of vertices must be equals.");
  }

  [Test]
  public void Simplex4D_1DEdge_2DNeighborsPointsTest() {
    Point p0 = new Point(new double[] { 0, 0, 0, 0 });
    Point p1 = new Point(new double[] { 1, 0, 0, 0 });
    Point p2 = new Point(new double[] { 0, 1, 0, 0 });
    Point p3 = new Point(new double[] { 0.1, 0, 1, 0 });
    Point p4 = new Point(new double[] { 0.1, 0, 0, 1 });

    List<Point> Swarm = new List<Point>()
      {
        p0
      , p1
      , p2
      , p3
      , p4
      , Point.LinearCombination(p1, 0.3, p2, 0.2)
      , Point.LinearCombination(p1, 0.4, p2, 0.1)
      , Point.LinearCombination(p1, 0.4, p3, 0.1)
      , Point.LinearCombination(p1, 0.4, p3, 0.1)
      , Point.LinearCombination(p1, 0.4, p4, 0.1)
      , Point.LinearCombination(p1, 0.4, p4, 0.1)
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void AllCubes3D_Test() {
    const int nPoints = 5000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 3).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Cube(3, out List<Point> P, fID, nPoints);
      ShiftAndRotate(3, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 3, nPoints, fID, true);
    }
  }

  [Test]
  public void AllCubes4D_Test() {
    const int nPoints = 2000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 4).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Cube(4, out List<Point> P, fID, nPoints);
      ShiftAndRotate(4, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 4, nPoints, fID, true);
    }
  }

  [Test]
  public void AllCubes5D_Test() {
    const int nPoints = 5;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 5).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Cube(5, out List<Point> P, fID, nPoints);
      ShiftAndRotate(5, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 5, nPoints, fID, true);
    }
  }

  [Test]
  public void AllCubes6D_Test() {
    const int nPoints = 2;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 6).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Cube(6, out List<Point> P, fID, nPoints);
      ShiftAndRotate(6, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 6, nPoints, fID, true);
    }
  }

  [Test]
  public void AllCubes7D_Test() {
    const int nPoints = 1;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 7).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Cube(7, out List<Point> P, fID, nPoints);
      ShiftAndRotate(7, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 7, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices3D_Test() {
    const int nPoints = 2000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 3).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Simplex(3, out List<Point> P, fID, nPoints);
      ShiftAndRotate(3, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 3, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices4D_Test() {
    const int nPoints = 2000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 4).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Simplex(4, out List<Point> P, fID, nPoints);
      ShiftAndRotate(4, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 4, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices5D_Test() {
    const int nPoints = 1000;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 5).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Simplex(5, out List<Point> P, fID, nPoints);
      ShiftAndRotate(5, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 5, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices6D_Test() {
    const int nPoints = 0;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 6).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Simplex(6, out List<Point> P, fID, nPoints);
      ShiftAndRotate(6, ref P, ref Swarm);

      //Check(Swarm, P, saveSeed, 6, nPoints, fID, true);
      Check(Swarm, P, saveSeed, 6, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplices7D_Test() {
    const int nPoints = 2;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 7).ToList());

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Point> Swarm = Simplex(7, out List<Point> P, fID, nPoints);
      ShiftAndRotate(7, ref P, ref Swarm);

      Check(Swarm, P, saveSeed, 7, nPoints, fID, true);
    }
  }

  [Test]
  public void AllSimplicesRND_3D_Test() {
    const int nPoints = 1;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 3).ToList());

    for (int i = 0; i < 1e5; i++) {
      // foreach (List<int> fID in fIDs) {
      uint      saveSeed = _random.Seed;
      List<int> fID      = new List<int>() { 1 };

      List<Point> Swarm = SimplexRND(3, out List<Point> P, fID, nPoints);

      Check(Swarm, P, saveSeed, 3, nPoints, fID, true);
      // }
    }
  }

  [Test]
  public void AllSimplicesRND_4D_Test() {
    const int nPoints = 2;

    List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, 4).ToList());

    for (int i = 0; i < 1e4; i++) {
      // foreach (List<int> fID in fIDs) {

      List<int> fID      = new List<int>() { 1 };
      uint      saveSeed = _random.Seed;

      List<Point> Swarm = SimplexRND(4, out List<Point> P, fID, nPoints);

      Check(Swarm, P, saveSeed, 4, nPoints, fID, true);
      // }
    }
  }


  // [Test]
  // public void AllCrossPolytopTest() {
  //   const int minDim  = 3;
  //   const int maxDim  = 6;
  //   const int nTests  = 10;
  //   const int nPoints = 50;
  //
  //   for (int crossDim = minDim; crossDim <= maxDim; crossDim++) {
  //     for (int k = 0; k < nTests; k++) {
  //       uint saveSeed = _random.Seed;
  //
  //       List<Point> cross = CrossPolytop(crossDim);
  //
  //       Matrix      rotation       = GenRotation(crossDim);
  //       List<Point> Rotated        = Rotate(cross, rotation);
  //       Vector      shift          = GenVector(crossDim) * _random.NextInt(1, 10);
  //       List<Point> RotatedShifted = Shift(Rotated, shift);
  //
  //
  //       List<Point> Swarm = CrossPolytop(crossDim, nPoints);
  //       Swarm = Rotate(Swarm, rotation);
  //       Swarm = Shift(Swarm, shift);
  //
  //       Check(Swarm, RotatedShifted, saveSeed, crossDim, nPoints, new List<int>() { crossDim });
  //     }
  //   }
  // }


  [Test]
  public void Aux() {
    const uint seed    = 236608055;
    const int  PDim    = 3;
    const int  nPoints = 1;
    List<int>  fID     = new List<int>() { 1 };


    _random = new RandomLC(seed);

    List<Point> Swarm = SimplexRND(PDim, out List<Point> polytop, fID, nPoints);

    Check(Swarm, polytop, seed, PDim, nPoints, fID, false);
  }

  // [Test]
  // public void Aux() {
  //   const uint seed    = 2091444945;
  //   const int  PDim    = 5;
  //   const int  nPoints = 1000;
  //   const bool isCube  = false;
  //   List<int>  fID     = new List<int>() { 4, 5 };
  //
  //
  //   _random = new RandomLC(seed);
  //
  //   List<Point> polytop;
  //   List<Point> Swarm = isCube ? Cube(PDim, out polytop, fID, nPoints) : Simplex(PDim, out polytop, fID, nPoints);
  //   ShiftAndRotate(PDim, ref polytop, ref Swarm);
  //
  //   Check(Swarm, polytop, seed, PDim, nPoints, fID, true);
  // }


  [Test]
  public void AuxPoints() {
    // List<Point> S = new List<Point>()
    //   {
    //     new Point(new double[] { 0.9999999999999996, 2.7755575615628914E-17, -5.551115123125783E-17, 5.551115123125783E-17 })
    //   , new Point(new double[] { -5.551115123125783E-16, 1.0000000000000002, 0.9999999999999998, 1 })
    //   , new Point(new double[] { -5.551115123125783E-16, 1.3877787807814457E-16, 0.9999999999999998, 0.9999999999999999 })
    //   , new Point(new double[] { -1.3183898417423734E-16, 1, -3.660266534311063E-16, 1.0000000000000002 })
    //   , new Point(new double[] { -1.6653345369377348E-16, -4.163336342344337E-17, -2.498001805406602E-16, 1 })
    //   , new Point(new double[] { 0.9999999999999993, 1.0000000000000002, 0.9999999999999999, 1 })
    //   , new Point(new double[] { 0.9999999999999996, 1.6653345369377348E-16, 1.0000000000000004, 1 })
    //   , new Point(new double[] { 0.9999999999999999, 1.0000000000000002, -1.1102230246251565E-16, 0.9999999999999999 })
    //   , new Point(new double[] { 1, 0, 3.3306690738754696E-16, 0.9999999999999997 })
    //   , new Point(new double[] { -5.828670879282072E-16, 1, 0.9999999999999997, 2.220446049250313E-16 })
    //   , new Point(new double[] { -6.938893903907228E-16, 1.942890293094024E-16, 0.9999999999999998, -8.326672684688674E-17 })
    //   , new Point(new double[] { -4.440892098500626E-16, 1, -1.6653345369377348E-16, 1.5265566588595902E-16 })
    //   , new Point(new double[] { 0, 0, 0, 0 })
    //   , new Point(new double[] { 0.9999999999999993, 1.0000000000000004, 1, -5.551115123125783E-17 })
    //   , new Point(new double[] { 0.9999999999999993, 1.6653345369377348E-16, 0.9999999999999999, -1.1102230246251565E-16 })
    //   , new Point(new double[] { 0.9999999999999994, 1, -4.163336342344337E-17, 7.632783294297951E-17 })
    //   };
    List<Point> S = new List<Point>()
      {
        new Point(new double[] { -1.980064489466435, 2.9872748292594156, -4.180556189998251 })
      , new Point(new double[] { -1.6113520557358647, 1.8560766040094747, -3.8464289313181492 })
      , new Point(new double[] { 0.42698253770987127, -4.532841988730441, 3.5492071459417254 })
      , new Point(new double[] { -1.921639002173103, 2.7851811388067538, -2.710060894579053 })
      , new Point(new double[] { -1.3995652148499071, 1.0700363307423044, 4.581756107411756 })
      , new Point(new double[] { -0.058344149742821094, -2.937086091362193, -0.866344411826307 })
      , new Point(new double[] { -2.0655917462579887, 3.2581024147239757, -4.72065229428016 })
      };

    // Assert.That(new HashSet<Point>(S).SetEquals(Cube(3, out List<Point> cube)));

    // for (int i = 0; i < 10 * S.Count; i++) {
    // Tools.Shuffle(S);
    Polyhedron P = GiftWrapping.WrapPolyhedron(S);
    // Assert.That(P.Vertices.SetEquals(cube));
    // }
  }


  /// <summary>
  /// Aux procedure.
  /// </summary>
  /// <param name="Swarm">The swarm to convexify.</param>
  /// <param name="Answer">The final list of points.</param>
  /// <param name="seed"></param>
  /// <param name="PDim"></param>
  /// <param name="nPoints"></param>
  /// <param name="fID"></param>
  /// <param name="needShuffle"></param>
  private static void Check(List<Point> Swarm
                          , List<Point> Answer
                          , uint        seed
                          , int         PDim
                          , int         nPoints
                          , List<int>   fID
                          , bool        needShuffle = false) {
    Polyhedron? P = null;

    try {
      if (needShuffle) {
        HashSet<Point> origS = new HashSet<Point>(Swarm);
        Tools.Shuffle(Swarm, new Random((int)seed));
        Debug.Assert(origS.SetEquals(Swarm));
      }

      P = GiftWrapping.WrapPolyhedron(Swarm);
      Debug.Assert(P is not null, nameof(P) + " != null");
    }
    catch (Exception e) {
      Console.WriteLine("Gift wrapping does not success!");
      WriteInfo(seed, PDim, nPoints, fID);

      Console.WriteLine(e.Message);

      throw new ArgumentException("Error in gift wrapping!");
    }

    try {
      Assert.That(P.Vertices.SetEquals(Answer), "The set of vertices must be equals.");
    }
    catch (Exception e) {
      Console.WriteLine("Gift wrapping success. But sets of vertices do not equal!");
      WriteInfo(seed, PDim, nPoints, fID);

      Console.WriteLine(e.Message);

      throw new ArgumentException("The set of vertices must be equals.");
    }
  }

  private static void WriteInfo(uint seed, int PDim, int nPoints, List<int> fID) {
    Console.WriteLine($"The seed = {seed}");
    Console.WriteLine($"The PDim = {PDim}");
    Console.WriteLine($"The nPoints = {nPoints}");
    Console.WriteLine("The fID:");
    Console.WriteLine(string.Join(", ", fID));
  }

  ///<summary>
  /// Method applies a rotation and a shift to two lists of points.
  ///</summary>
  ///<param name="PDim">The dimension of the space in which the points exist.</param>
  ///<param name="P">A reference to the list of points to be transformed.</param>
  ///<param name="Swarm">A reference to the list of points representing the swarm to be transformed.</param>
  private static void ShiftAndRotate(int PDim, ref List<Point> P, ref List<Point> Swarm) {
    Matrix rotation = GenRotation(PDim);
    Vector shift    = GenVector(PDim) * _random.NextInt(1, 10);

    P = Rotate(P, rotation);
    P = Shift(P, shift);

    Swarm = Rotate(Swarm, rotation);
    Swarm = Shift(Swarm, shift);
  }

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
    Assert.That(P.Vertices.SetEquals(S), "The set of vertices must be equals.");
  }

  [Test]
  public void Cross_4D() {
    List<Point> S = CrossPolytop(4, 0);
    Polyhedron  P = GiftWrapping.WrapPolyhedron(S);

    foreach (Point s in P.Vertices) {
      Console.WriteLine(s);
    }
  }

  [Test]
  public void PlanesTest_3D() {
    Point        origin  = new Point(new double[] { 0, 0, 0 });
    List<Vector> Normals = new List<Vector>();
    do { //todo придумать как генерировать не взаимоисключающие плоскости
      Normals.Clear();
      for (int i = 0; i < 5; i++) {
        Normals.Add(GenVector(3));
      }
    } while (Normals.Aggregate((acc, x) => acc + x).IsZero);

    List<HyperPlane> hyperPlanes = new List<HyperPlane>();
    foreach (Vector normal in Normals) {
      hyperPlanes.Add(new HyperPlane(origin, normal));
    }

    List<Point> Swarm = new List<Point>();

    foreach (HyperPlane hp in hyperPlanes) {
      for (int i = 0; i < 10; i++) {
        Point p;
        do {
          p = GenAffineCombination(hp.AffineBasis.GetBasisAsPoints(), 100, -50);
        } while (hyperPlanes.All(HP => HP.Eval(p) <= 0));
        Swarm.Add(p);
      }
    }

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Console.WriteLine("Надо как-то проверять!");
  }


  // [Test]
  // public void DependencyOnPoints() {
  //   Stopwatch stopwatch = new Stopwatch();
  //   const int cubeDim   = 3;
  //
  //   for (int i = 1; i < 1e7; i *= 10) {
  //     List<Point> Swarm = Cube(cubeDim, new[] { cubeDim }, i);
  //
  //     stopwatch.Start();
  //     Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);
  //     stopwatch.Stop();
  //
  //     TimeSpan elapsed = stopwatch.Elapsed;
  //     //Console.WriteLine($"i = {i}. Время выполнения: {elapsed.TotalSeconds}");
  //     Console.WriteLine($"{i}; {elapsed.TotalSeconds}");
  //   }
  // }

}
