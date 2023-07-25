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
  private static readonly Random _random = new Random(0);

  /// <summary>
  /// Generates a random double value between 0 and 1, excluding the values 0 and 1.
  /// </summary>
  /// <returns>The generated random double value.</returns>
  double GenInner() {
    double w = _random.NextDouble();

    while (Tools.EQ(w) || Tools.EQ(w, 1)) {
      w = _random.NextDouble();
    }

    return w;
  }

  /// <summary>
  /// Finds all subsets of a given length for an array.
  /// </summary>
  /// <typeparam name="T">The type of the array elements.</typeparam>
  /// <param name="arr">The input array.</param>
  /// <param name="subsetLength">The length of the subsets.</param>
  /// <returns>A list of subsets of the specified length.</returns>
  private static List<List<T>> FindSubsets<T>(IReadOnlyList<T> arr, int subsetLength) {
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
  /// Generates a d-simplex in d-space. 
  /// </summary>
  /// <param name="simplexDim">The dimension of the simplex.</param>
  /// <param name="facesDim">The dimensions of the faces of the simplex to put points on.</param>
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <returns>A list of points representing the simplex.</returns>
  List<Point> Simplex(int simplexDim, IEnumerable<int>? facesDim = null, int amount = 50) {
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

        List<List<Point>> subsets = FindSubsets(simplex, dim + 1);

        foreach (List<Point> points in subsets) {
          for (int k = 0; k < amount; k++) {
            List<double> ws = new List<double>();

            double difA = 1;

            for (int i = 0; i < points.Count; i++) {
              if (Tools.LT(difA)) {
                ws.Add(0.0);
              } else {
                double alpha = GenInner() * difA;
                ws.Add(alpha);
                difA -= alpha;
              }
            }

            Simplex.Add(Point.LinearCombination(points, ws));
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
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <returns>A list of points representing the hypercube.</returns>
  List<Point> Cube(int cubeDim, IEnumerable<int>? facesDim = null, int amount = 50) {
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
        Debug.Assert(dim <= Cube.First().Dim); //Если равно, то внутрь самого куба

        for (int i = 0; i < amount; i++) {
          double[] point = new double[cubeDim];

          for (int j = 0; j < cubeDim; j++) {
            point[j] = -1;
          }

          HashSet<int> constInd = new HashSet<int>();

          for (int j = 0; j < cubeDim - dim; j++) {
            int ind;

            do {
              ind = _random.Next(0, cubeDim);
            } while (!constInd.Add(ind));
          }

          int zeroOrOne = _random.Next(0, 2);

          foreach (int ind in constInd) {
            point[ind] = zeroOrOne;
          }

          for (int j = 0; j < cubeDim; j++) {
            if (Tools.EQ(point[j], -1)) {
              point[j] = GenInner();
            }
          }
          // Console.WriteLine(new Point(point));
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
  /// <param name="facesDim">TODO генерировать точки!</param>
  /// <param name="amount">TODO</param>
  /// <returns></returns>
  List<Point> CrossPolytop(int crossDim, IEnumerable<int>? facesDim = null, int amount = 50) {
    List<Point> cross = new List<Point>();

    for (int i = 1; i <= crossDim; i++) {
      Vector v = Vector.CreateOrth(crossDim, i);
      cross.Add(new Point(v));
      cross.Add(new Point(-v));
    }


    return cross;
  }


  /// <summary>
  /// Generates a non-zero random vector of the specified dimension.
  /// </summary>
  /// <param name="dim">The dimension of the vector.</param>
  /// <returns>A random vector. Each coordinate: [-0.5, 0.5) \ {0}.</returns>
  Vector GenVector(int dim) {
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
  Matrix GenRotation(int spaceDim) {
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
  List<Point> Rotate(IEnumerable<Point> Swarm, Matrix rotation) {
    IEnumerable<Vector> rotated = Swarm.Select(s => new Vector(s) * rotation);

    return rotated.Select(v => new Point(v)).ToList();
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

    List<Point> S = Cube(3);

    Debug.Assert(Swarm.SetEquals(new HashSet<Point>(S)), "Swarm is not equal to generated Cube");
  }

  [Test]
  public void Cube3D() {
    List<Point> Swarm = Cube(3);

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(Swarm), "The set of vertices must be equals.");

    //P.WriteToObjFile("cube");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_Rotated() {
    int    cubeDim  = 3;
    Matrix rotation = GenRotation(cubeDim);

    List<Point> RotatedCube = Rotate(Cube(cubeDim), rotation);

    Polyhedron P = GiftWrapping.WrapPolyhedron(RotatedCube);
    Debug.Assert(P.Vertices.SetEquals(RotatedCube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_Shifted() {
    int         cubeDim     = 3;
    Vector      shift       = GenShift(cubeDim);
    List<Point> shiftedCube = Shift(Cube(cubeDim), shift);

    Polyhedron P = GiftWrapping.WrapPolyhedron(shiftedCube);
    Debug.Assert(P.Vertices.SetEquals(shiftedCube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_Rotated_Shifted() {
    int cubeDim = 3;

    Matrix      rotation           = GenRotation(cubeDim);
    List<Point> RotatedCube        = Rotate(Cube(cubeDim), rotation);
    Vector      shift              = GenShift(cubeDim);
    List<Point> RotatedShiftedCube = Shift(RotatedCube, shift);

    List<Point> Swarm = Cube(cubeDim);
    Swarm = Rotate(Swarm, rotation);
    Swarm = Shift(Swarm, shift);
    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(RotatedShiftedCube), "The set of vertices must be equals.");
  }

  private Vector GenShift(int cubeDim) { return GenVector(cubeDim) * _random.Next(1, 100); }

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

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
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

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
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


  //todo и дальше в том же духе ...

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
    const int maxDim  = 3;
    const int nTests  = 5000;
    const int nPoints = 20;

    for (int cubeDim = 3; cubeDim <= maxDim; cubeDim++) {
      for (int fDim = 0; fDim <= cubeDim; fDim++) {
        for (int k = 0; k < nTests; k++) {
          Matrix      rotation           = GenRotation(cubeDim);
          List<Point> RotatedCube        = Rotate(Cube(cubeDim), rotation);
          Vector      shift              = GenVector(cubeDim) * _random.Next(1, 100);
          List<Point> RotatedShiftedCube = Shift(RotatedCube, shift);


          // Vector      shift              = GenVector(cubeDim) * _random.Next(1, 4);
          // List<Point> ShiftedCube        = Shift(Cube(cubeDim),shift);


          List<Point> Swarm = Cube(cubeDim, GenFacesInd(fDim, cubeDim), nPoints);
          Swarm = Rotate(Swarm, rotation);
          Swarm = Shift(Swarm, shift);
          Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

          try {
            // Debug.Assert(P.Vertices.SetEquals(RotatedCube), "The set of vertices must be equals.");
            // Debug.Assert(P.Vertices.SetEquals(ShiftedCube), "The set of vertices must be equals.");
            Debug.Assert(P.Vertices.SetEquals(RotatedShiftedCube), "The set of vertices must be equals.");
          }
          catch (Exception e) {
            foreach (Point s in Swarm) {
              Console.WriteLine(s);
            }

            throw new ArgumentException();
          }
        }
      }
    }
  }

  [Test]
  public void AllSimplexTest() {
    const int maxDim  = 3;
    const int nTests  = 50000;
    const int nPoints = 3;


    for (int polyhedronDim = 3; polyhedronDim <= maxDim; polyhedronDim++) {
      for (int fDim = 0; fDim <= polyhedronDim; fDim++) {
        for (int k = 0; k < nTests; k++) {
          List<Point> simplex               = Simplex(polyhedronDim);
          Matrix      rotation              = GenRotation(polyhedronDim);
          List<Point> RotatedSimplex        = Rotate(simplex, rotation);
          Vector      shift                 = GenVector(polyhedronDim) * _random.Next(1, 100);
          List<Point> RotatedShiftedSimplex = Shift(RotatedSimplex, shift);


          List<Point> Swarm = Simplex(polyhedronDim, GenFacesInd(fDim, polyhedronDim), nPoints);
          Swarm = Rotate(Swarm, rotation);
          Swarm = Shift(Swarm, shift);

          Polyhedron? P = null;

          try {
            P = GiftWrapping.WrapPolyhedron(Swarm);
          }
          catch (Exception e) {
            foreach (Point s in Swarm) {
              Console.WriteLine(s);
            }
          }

          try {
            Debug.Assert(P.Vertices.SetEquals(RotatedShiftedSimplex), "The set of vertices must be equals.");
          }
          catch (Exception e) {
            foreach (Point s in P.Vertices) {
              Console.WriteLine(s);
            }

            Console.WriteLine("=========================");

            foreach (Point s in RotatedShiftedSimplex) {
              Console.WriteLine(s);
            }

            throw new ArgumentException();
          }
        }
      }
    }
  }

  [Test]
  public void AuxSimplex() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 16.149344503250603, -21.452113890066798, 26.855425210369486 })
      , new Point(new double[] { 16.80853556618116, -21.116318533312754, 27.528261461573972 })
      , new Point(new double[] { 15.41674070428436, -21.36711896185478, 27.530752935195338 })
      , new Point(new double[] { 15.979760257836379, -20.514021495597756, 26.55339235926446 })
      , new Point(new double[] { 16.38827147512058, -21.27751746756958, 27.263557389903834 })
      , new Point(new double[] { 16.159975630929436, -21.45179235917561, 26.856069499133856 })
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    foreach (Point s in P.Vertices) {
      Console.WriteLine(s);
    }
  }

  [Test]
  public void AllCrossPolytopTest() {
    const int minDim = 3;
    const int maxDim = 7;
    const int nTests = 5000;

    for (int polyhedronDim = minDim; polyhedronDim <= maxDim; polyhedronDim++) {
      // for (int fDim = 0; fDim <= polyhedronDim; fDim++) {
      for (int k = 0; k < nTests; k++) {
        List<Point> cross = CrossPolytop(polyhedronDim);

        Matrix      rotation       = GenRotation(polyhedronDim);
        List<Point> Rotated        = Rotate(cross, rotation);
        Vector      shift          = GenVector(polyhedronDim) * _random.Next(1, 100);
        List<Point> RotatedShifted = Shift(Rotated, shift);


        // List<Point> Swarm = CrossPolytop(polyhedronDim, GenFacesInd(fDim, polyhedronDim), nPoints);
        // Swarm = Rotate(Swarm, rotation);
        // Swarm = Shift(Swarm, shift);

        Polyhedron? P = null;

        try {
          P = GiftWrapping.WrapPolyhedron(RotatedShifted);
        }
        catch (Exception e) {
          foreach (Point s in RotatedShifted) {
            Console.WriteLine(s);
          }
        }

        try {
          Debug.Assert(P.Vertices.SetEquals(RotatedShifted), "The set of vertices must be equals.");
        }
        catch (Exception e) {
          foreach (Point s in P.Vertices) {
            Console.WriteLine(s);
          }

          Console.WriteLine("=========================");

          foreach (Point s in RotatedShifted) {
            Console.WriteLine(s);
          }

          throw new ArgumentException();
        }
        // }
      }
    }
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

    foreach (Point s in P.Vertices) {
      Console.WriteLine(s);
    }
  }

  [Test]
  public void Cross_4D() {
    List<Point> S = CrossPolytop(4);

    Polyhedron P = GiftWrapping.WrapPolyhedron(S);

    foreach (Point s in P.Vertices) {
      Console.WriteLine(s);
    }
  }


  /// <summary>
  /// Generates a set of random face dimension-indices for a polyhedron.
  /// </summary>
  /// <param name="fDim">The number of face indices to generate.</param>
  /// <param name="polyhedronDim">The dimension of the polyhedron.</param>
  /// <returns>A HashSet containing the randomly generated face dimension-indices.</returns>
  private static HashSet<int> GenFacesInd(int fDim, int polyhedronDim) {
    HashSet<int> faceInd = new HashSet<int>();

    for (int j = 0; j < fDim; j++) {
      int ind;

      do {
        ind = _random.Next(1, polyhedronDim + 1);
      } while (!faceInd.Add(ind));
    }

    return faceInd;
  }


  /// <summary>
  /// Shift given swarm by given vector
  /// </summary>
  /// <param name="Swarm">Swarm to be shifted</param>
  /// <param name="shift">Vector to shift</param>
  /// <returns></returns>
  private static List<Point> Shift(List<Point> Swarm, Vector shift) { return Swarm.Select(s => new Point(s + shift)).ToList(); }

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
