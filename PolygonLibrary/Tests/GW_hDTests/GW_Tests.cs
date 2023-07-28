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
  private Point GenLinearCombination(List<Point> points) {
    List<double> ws = new List<double>();

    double difA = 1;
    for (int i = 0; i < points.Count - 1; i++) {
      double alpha = GenInner() * difA;
      ws.Add(alpha);
      difA -= alpha;
    }
    ws.Add(difA);
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
  /// Generates a d-simplex in d-space. 
  /// </summary>
  /// <param name="simplexDim">The dimension of the simplex.</param>
  /// <param name="facesDim">The dimensions of the faces of the simplex to put points on.</param>
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <returns>A list of points representing the simplex.</returns>
  private List<Point> Simplex(int simplexDim, IEnumerable<int>? facesDim = null, int amount = 50) {
    List<Point> simplex = new List<Point> { new Point(new double[simplexDim]) };

    for (int i = 0; i < simplexDim; i++) {
      double[] v = new double[simplexDim];
      v[i] = 1;
      simplex.Add(new Point(v));
    }

    List<Point> Simplex = new List<Point>(simplex);

    if (facesDim is not null) {
      foreach (int dim in facesDim) {
        Debug.Assert(dim <= simplex.First().Dim);

        List<List<Point>> subsets = Subsets(simplex, dim + 1);

        foreach (List<Point> points in subsets) {
          for (int k = 0; k < amount; k++) {
            Point res = GenLinearCombination(points);
            Simplex.Add(res);
          }
        }
      }
    }


    return Simplex;
  }


  /// <summary>
  /// Generates a full-dimension hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The amount of points to be placed into random set of faces of faceDim dimension.</param>
  /// <returns>A list of points representing the hypercube.</returns>
  private List<Point> Cube(int cubeDim, IEnumerable<int>? facesDim = null, int amount = 50) {
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
      cross.Add(GenLinearCombination(cross));
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
  private static List<Point> Shift(List<Point> Swarm, Vector shift) { return Swarm.Select(s => new Point(s + shift)).ToList(); }

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

    List<Point> S = Cube(3);

    Debug.Assert(Swarm.SetEquals(new HashSet<Point>(S)), "Swarm is not equal to generated Cube");
  }

  [Test]
  public void Cube3D() {
    List<Point> Swarm = Cube(3);

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);
    Debug.Assert(P.Vertices.SetEquals(Swarm), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_Rotated_Z45() {
    List<Point>  Swarm = Cube(3);
    const double angle = Math.PI / 4;
    double       sin   = Math.Sin(angle);
    double       cos   = Math.Cos(angle);

    double[,] rotationZ45 = { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } };

    List<Point> Rotated = Rotate(Swarm, new Matrix(rotationZ45));

    Polyhedron P = GiftWrapping.WrapPolyhedron(Rotated);
    Debug.Assert(P.Vertices.SetEquals(Rotated), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_Rotated() {
    uint   saveSeed = _random.GetSeed();
    int    cubeDim  = 3;
    Matrix rotation = GenRotation(cubeDim);

    List<Point> RotatedCube = Rotate(Cube(cubeDim), rotation);

    Polyhedron P = GiftWrapping.WrapPolyhedron(RotatedCube);
    Debug.Assert(P.Vertices.SetEquals(RotatedCube), $"The set of vertices must be equals. Seed = {saveSeed}");
  }

  [Test]
  public void Cube3D_Shifted() {
    uint        saveSeed    = _random.GetSeed();
    int         cubeDim     = 3;
    Vector      shift       = GenShift(cubeDim);
    List<Point> ShiftedCube = Shift(Cube(cubeDim), shift);

    Polyhedron P = GiftWrapping.WrapPolyhedron(ShiftedCube);
    Debug.Assert(P.Vertices.SetEquals(ShiftedCube), $"The set of vertices must be equals. Seed = {saveSeed}");
  }

  [Test]
  public void Cube3D_Rotated_Shifted() {
    uint saveSeed = _random.GetSeed();
    int  cubeDim  = 3;

    Matrix      rotation           = GenRotation(cubeDim);
    List<Point> RotatedCube        = Rotate(Cube(cubeDim), rotation);
    Vector      shift              = GenShift(cubeDim);
    List<Point> RotatedShiftedCube = Shift(RotatedCube, shift);

    List<Point> Swarm = Cube(cubeDim);
    Swarm = Rotate(Swarm, rotation);
    Swarm = Shift(Swarm, shift);
    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(RotatedShiftedCube), $"The set of vertices must be equals. Seed = {saveSeed}");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D() {
    List<Point> Swarm = Cube(3, new List<int>() { 1 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D() {
    List<Point> Swarm = Cube(3, new List<int>() { 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_3D() {
    List<Point> Swarm = Cube(3, new List<int>() { 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D() {
    List<Point> Swarm = Cube(3, new List<int>() { 1, 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D_3D() {
    List<Point> Swarm = Cube(3, new List<int>() { 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D_3D() {
    List<Point> Swarm = Cube(3, new List<int>() { 1, 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }


  [Test]
  public void Cube4D_withInnerPoints_On_1D() {
    List<Point> Swarm = Cube(4, new List<int>() { 1 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }


  [Test]
  public void Cube4D_withInnerPoints_On_2D() {
    List<Point> Swarm = Cube(4, new List<int>() { 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_3D() {
    List<Point> Swarm = Cube(4, new List<int>() { 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D() {
    List<Point> Swarm = Cube(4, new List<int>() { 1, 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_2D_3D() {
    List<Point> Swarm = Cube(4, new List<int>() { 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D() {
    List<Point> Swarm = Cube(4, new List<int>() { 1, 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D_4D() {
    List<Point> Swarm = Cube
      (
       4
     , new List<int>()
         {
           1
         , 2
         , 3
         , 4
         }
      );

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(4)), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube_6D_withAll() {
    const int cubeDim = 6;

    List<Point> Swarm = Cube
      (
       cubeDim
     , new List<int>()
         {
           1
         , 2
         , 3
         , 4
         , 5
         , 6
         }
      );

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(cubeDim)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube_7D() {
    const int   cubeDim = 7;
    List<Point> Swarm   = Cube(cubeDim);

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Cube(cubeDim)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
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

    Debug.Assert(P.Vertices.SetEquals(Swarm), "The set of vertices must be equals.");
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
  public void AllCubesTest() {
    const int minDim  = 4;
    const int maxDim  = 6;
    const int nTests  = 1;
    const int nPoints = 100;

    for (int cubeDim = minDim; cubeDim <= maxDim; cubeDim++) {
      List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, cubeDim).ToList());
      foreach (List<int> fID in fIDs) {
        for (int k = 0; k < nTests; k++) { //todo Может nTests и не нужен, так как есть fIDs
          uint saveSeed = _random.GetSeed();

          List<Point> cube = Cube(cubeDim);

          Matrix      rotation       = GenRotation(cubeDim);
          List<Point> RotatedCube    = Rotate(cube, rotation);
          Vector      shift          = GenVector(cubeDim) * _random.NextInt(1, 10);
          List<Point> RotatedShifted = Shift(RotatedCube, shift);


          // Vector      shift              = GenVector(cubeDim) * _random.NextInt(1, 4);
          // List<Point> ShiftedCube        = Shift(Cube(cubeDim),shift);


          List<Point> Swarm = Cube(cubeDim, fID, nPoints);
          Swarm = Rotate(Swarm, rotation);
          Swarm = Shift(Swarm, shift);

          Check(Swarm, RotatedShifted, saveSeed, cubeDim, nPoints, fID);
        }
      }
    }
  }

  [Test]
  public void AllSimplexTest() {
    const int minDim  = 3;
    const int maxDim  = 3;
    const int nTests  = 1;
    const int nPoints = 1000;


    for (int simplexDim = minDim; simplexDim <= maxDim; simplexDim++) {
      List<List<int>> fIDs = AllSubsets(Enumerable.Range(1, simplexDim).ToList());
      foreach (List<int> fID in fIDs) {
        for (int k = 0; k < nTests; k++) {
          uint saveSeed = _random.GetSeed();

          List<Point> simplex = Simplex(simplexDim);

          Matrix      rotation       = GenRotation(simplexDim);
          List<Point> RotatedSimplex = Rotate(simplex, rotation);
          Vector      shift          = GenVector(simplexDim) * _random.NextInt(1, 10);
          List<Point> RotatedShifted = Shift(RotatedSimplex, shift);

          List<Point> Swarm = Simplex(simplexDim, fID, nPoints);
          Swarm = Rotate(Swarm, rotation);
          Swarm = Shift(Swarm, shift);

          Check(Swarm, RotatedShifted, saveSeed, simplexDim, nPoints, fID);
        }
      }
    }
  }

  [Test]
  public void AllCrossPolytopTest() {
    const int minDim  = 3;
    const int maxDim  = 3;
    const int nTests  = 5000;
    const int nPoints = 50;

    for (int crossDim = minDim; crossDim <= maxDim; crossDim++) {
      for (int k = 0; k < nTests; k++) {
        uint saveSeed = _random.GetSeed();

        List<Point> cross = CrossPolytop(crossDim);

        Matrix      rotation       = GenRotation(crossDim);
        List<Point> Rotated        = Rotate(cross, rotation);
        Vector      shift          = GenVector(crossDim) * _random.NextInt(1, 10);
        List<Point> RotatedShifted = Shift(Rotated, shift);


        List<Point> Swarm = CrossPolytop(crossDim, nPoints);
        Swarm = Rotate(Swarm, rotation);
        Swarm = Shift(Swarm, shift);

        Check(Swarm, RotatedShifted, saveSeed, crossDim, nPoints, new List<int>() { crossDim });
      }
    }
  }


  [Test]
  public void Aux() {
    const uint seed    = 2642205987;
    const int  PDim    = 6;
    const int  nPoints = 10;
    List<int>  fID     = new List<int>() { 1, 5, 6 };


    _random = new RandomLC(seed);
    List<Point> polytop = Cube(PDim);

    Matrix      rotation       = GenRotation(PDim);
    List<Point> RotatedCube    = Rotate(polytop, rotation);
    Vector      shift          = GenVector(PDim) * _random.NextInt(1, 10);
    List<Point> RotatedShifted = Shift(RotatedCube, shift);


    // List<Point> Swarm = Cube(PDim, fID, nPoints);
    List<Point> Swarm = Simplex(PDim, fID, nPoints);

    Swarm = Rotate(Swarm, rotation);
    Swarm = Shift(Swarm, shift);

    Check(Swarm, RotatedShifted, seed, PDim, nPoints, fID);
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
  private static void Check(List<Point> Swarm
                          , List<Point> Answer
                          , uint        seed
                          , int         PDim
                          , int         nPoints
                          , List<int>   fID) {
    Polyhedron? P = null;

    try {
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
      Debug.Assert(P.Vertices.SetEquals(Answer), "The set of vertices must be equals.");
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
    Debug.Assert(P.Vertices.SetEquals(S), "The set of vertices must be equals.");
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
