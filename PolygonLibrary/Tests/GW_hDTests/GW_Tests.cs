using System.Diagnostics;
using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;
using PolygonLibrary.Toolkit;


namespace Tests.GW_hDTests;

/* todo
 * Куб_3D
 * Куб_3D с точками внутри
 * Куб_3D с точками внутри граней 
 * Куб_3D с точками внутри ребёр
 * Куб_3D с точками внутри и внутри граней
 * Куб_3D с точками внутри, внутри граней и внутри ребёр
 * 
 * Куб_4D
 * Куб_4D с точками внутри
 * Куб_4D с точками внутри 3D-граней 
 * Куб_4D с точками внутри 2D-граней 
 * Куб_4D с точками внутри ребёр (1D-граней)
 * Комбинации:
 * ...
 *
 *
 * Написать генератор, который получает размерность куба и список размерностей граней,
 * внутри которых генерировать точки
 */

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
  double GenIn() {
    double w = _random.NextDouble();

    while (Tools.EQ(w) || Tools.EQ(w, 1)) {
      w = _random.NextDouble();
    }

    return w;
  }

  /// <summary>
  /// Generates a full-dimension hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The amount of points to be placed into each face of faceDim dimension.</param>
  /// <returns>A list of points representing the hypercube.</returns>
  List<Point> CubeHD(int cubeDim, IEnumerable<int>? facesDim = null, int amount = 50) {
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
              point[j] = GenIn();
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

    List<Point> S = CubeHD(3);

    Debug.Assert(Swarm.SetEquals(new HashSet<Point>(S)), "Swarm is not equal to generated Cube");
  }

  [Test]
  public void Cube3D() {
    List<Point> Swarm = CubeHD(3);

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

    List<Point> RotatedCube = Rotate(CubeHD(cubeDim), rotation);

    Polyhedron P = GiftWrapping.WrapPolyhedron(RotatedCube);
    Debug.Assert(P.Vertices.SetEquals(RotatedCube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_Shifted() {
    int         cubeDim     = 3;
    Vector      shift       = GenShift(cubeDim);
    List<Point> shiftedCube = Shift(CubeHD(cubeDim), shift);

    Polyhedron P = GiftWrapping.WrapPolyhedron(shiftedCube);
    Debug.Assert(P.Vertices.SetEquals(shiftedCube), "The set of vertices must be equals.");
  }

  [Test]
  public void Cube3D_Rotated_Shifted() {
    int cubeDim = 3;

    Matrix      rotation           = GenRotation(cubeDim);
    List<Point> RotatedCube        = Rotate(CubeHD(cubeDim), rotation);
    Vector      shift              = GenShift(cubeDim);
    List<Point> RotatedShiftedCube = Shift(RotatedCube, shift);

    List<Point> Swarm = CubeHD(cubeDim);
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
    List<Point> Swarm = CubeHD(3, new List<int>() { 1 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_3D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 1, 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D_3D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D_3D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 1, 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(3)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }


  [Test]
  public void Cube4D_withInnerPoints_On_1D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 1 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }


  [Test]
  public void Cube4D_withInnerPoints_On_2D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_3D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 1, 2 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_2D_3D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 1, 2, 3 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D_4D() {
    List<Point> Swarm = CubeHD
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

    Debug.Assert(P.Vertices.SetEquals(CubeHD(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube_6D_withAll() {
    const int cubeDim = 6;

    List<Point> Swarm = CubeHD
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

    Debug.Assert(P.Vertices.SetEquals(CubeHD(cubeDim)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube_7D() {
    const int   cubeDim = 7;
    List<Point> Swarm   = CubeHD(cubeDim);

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(cubeDim)), "The set of vertices must be equals.");

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
    for (int cubeDim = 5; cubeDim < 6; cubeDim++) {
      for (int fDim = 0; fDim <= cubeDim; fDim++) {
        int amountTests = 500;

        for (int k = 0; k < amountTests; k++) {
          HashSet<int> faceInd = new HashSet<int>();

          for (int j = 0; j < fDim; j++) {
            int ind;

            do {
              ind = _random.Next(1, cubeDim + 1);
            } while (!faceInd.Add(ind));
          }


          Matrix      rotation           = GenRotation(cubeDim);
          List<Point> RotatedCube        = Rotate(CubeHD(cubeDim), rotation);
          Vector      shift              = GenVector(cubeDim) * _random.Next(1, 100);
          List<Point> RotatedShiftedCube = Shift(RotatedCube, shift);


          // Vector      shift              = GenVector(cubeDim) * _random.Next(1, 4);
          // List<Point> ShiftedCube        = Shift(CubeHD(cubeDim),shift);


          List<Point> Swarm = CubeHD(cubeDim, faceInd, 1);
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
  public void Aux() {
    Point c1 = new Point(new double[] { 0, 0, 0 });
    Point c2 = new Point(new double[] { -0.8852764610241171, 1.0281272583892638, 0.39904877911552494 });
    Point c3 = new Point(new double[] { -0.39411469384962644, 0.7535899157161732, -0.5633372911470121 });
    Point c4 = new Point(new double[] { -0.14918247106298443, 1.1553809721987207, -0.8017726606767539 });
    Point c5 = new Point(new double[] { -0.7732788753097883, 0.4713955613516855, -0.4240589649300647 });
    Point c6 = new Point(new double[] { 0.6240964042468039, 0.6839854108470351, -0.37771369574668917 });
    Point c7 = new Point(new double[] { -0.26118005677731315, 1.7121126692362987, 0.021335083368835772 });
    Point c8 = new Point(new double[] { 0.5120988185324752, 1.2407171078846133, 0.44539404829890045 });
    Point c9 = new Point(new double[] { -0.11199758571432869, 0.5567316970375782, 0.8231077440455896 });

    List<Point> Swarm = new List<Point>()
      {
        c1
      , c2
      , c3
      , c4
      , c5
      , c6
      , c7
      , c8
      , c9
      };

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }


  [Test]
  public void DependencyOnPoints() {
    Stopwatch stopwatch = new Stopwatch();
    const int cubeDim   = 6;

    for (int i = 1; i < 1e7; i *= 10) {
      List<Point> Swarm = CubeHD(cubeDim, new[] { cubeDim }, i);

      stopwatch.Start();
      Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);
      stopwatch.Stop();

      TimeSpan elapsed = stopwatch.Elapsed;
      //Console.WriteLine($"i = {i}. Время выполнения: {elapsed.TotalSeconds}");
      Console.WriteLine($"{i}; {elapsed.TotalSeconds}");
    }
  }


  /// <summary>
  /// Shift given swarm by given vector
  /// </summary>
  /// <param name="Swarm">Swarm to be shifted</param>
  /// <param name="shift">Vector to shift</param>
  /// <returns></returns>
  private static List<Point> Shift(List<Point> Swarm, Vector shift) { return Swarm.Select(s => new Point(s + shift)).ToList(); }

}
