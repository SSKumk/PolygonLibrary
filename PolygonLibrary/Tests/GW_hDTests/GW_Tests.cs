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
  private static readonly Random _random = new Random();

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
  /// <returns>A list of points representing the hypercube.</returns>
  List<Point> CubeHD(int cubeDim, List<int>? facesDim = null) {
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

        int ammount = 50;

        for (int i = 0; i < ammount; i++) {
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
          Console.WriteLine(new Point(point));
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

    do {
      for (int i = 0; i < dim; i++) {
        v[i] = _random.NextDouble() - 0.5;
      }
    } while (new Vector(v).IsZero);


    return new Vector(v);
  }

  /// <summary>
  /// Rotates the given swarm of points in the space.
  /// </summary>
  /// <param name="Swarm">The swarm of points to rotate.</param>
  /// <returns>The rotated swarm of points.</returns>
  List<Point> Rotate(IEnumerable<Point> Swarm) {
    int         spaceDim = Swarm.First().Dim;
    LinearBasis basis    = new LinearBasis(new[] { GenVector(spaceDim) });

    while (!basis.IsFullDim) {
      basis.AddVector(GenVector(spaceDim));
    }

    IEnumerable<Vector> rotated = Swarm.Select(s => new Vector(s) * basis.GetMatrix());

    return rotated.Select(v => new Point(v)).ToList();
  }

  // HashSet<Face> CubeFaceHD(int cubeDim, int faceDim) {
  //   
  // }

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

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_Rotated() {
    List<Point> Swarm   = CubeHD(3);
    List<Point> Rotated = Rotate(Swarm);

    Polyhedron P = GiftWrapping.WrapPolyhedron(Rotated);

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
    List<Point> Swarm = CubeHD(4, new List<int>() { 1, 2, 3, 4 });

    Polyhedron P = GiftWrapping.WrapPolyhedron(Swarm);

    Debug.Assert(P.Vertices.SetEquals(CubeHD(4)), "The set of vertices must be equals.");

    foreach (Point point in P.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube_7D() { //todo Понять, почему неверно заворачивает.
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

}
