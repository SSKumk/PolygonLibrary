using System.Diagnostics;
using NUnit.Framework;
using PolygonLibrary.Basics;
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
  /// Generates a hypercube in the specified dimension.
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

    AffineBasis basis  = new AffineBasis(cubeDim);
    List<Point> Es     = basis.Basis.Select(e => new Point(e)).ToList();
    Random      random = new Random();

    if (facesDim is not null) { // накидываем точки на грани нужных размерностей
      foreach (int dim in facesDim) {
        Debug.Assert(dim <= Cube.First().Dim); //Если равно, то внутрь самого куба

        for (int i = 0; i <= Es.Count - dim; i++) {
          List<Point> toGen = Es.GetRange(i, dim);


          for (int k = 0; k < 10; k++) {
            List<double> Ws = new List<double>(dim);

            for (int j = 0; j < dim; j++) {
              double w = 0;

              while (Tools.EQ(w)) {
                w = random.NextDouble();
              }
              Ws.Add(w);
            }

            Cube.Add(Point.LinearCombination(toGen, Ws));
          }
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
    Random   random = new Random();
    double[] v      = new double[dim];

    do {
      for (int i = 0; i < dim; i++) {
        v[i] = random.NextDouble() - 0.5;
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

  [Test]
  public void CubeTest() {
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

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }
  [Test]
  public void Cube3D_Rotated() {
    List<Point> Swarm   = CubeHD(3);
    List<Point> Rotated = Rotate(Swarm);

    var x = GiftWrapping.ToConvex(Rotated);

    foreach (Point point in x.Vertices) {
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

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 1 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 2 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_3D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 3 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 1, 2 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D_3D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 2, 3 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D_3D() {
    List<Point> Swarm = CubeHD(3, new List<int>() { 1, 2, 3 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }


  [Test]
  public void Cube4D_withInnerPoints_On_1D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 1 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }


  [Test]
  public void Cube4D_withInnerPoints_On_2D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 2 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_3D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 3 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 1, 2 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_2D_3D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 2, 3 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D() {
    List<Point> Swarm = CubeHD(4, new List<int>() { 1, 2, 3 });

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
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

    var x = GiftWrapping.ToConvex(Swarm);

    foreach (Point point in x.Vertices) {
      Console.WriteLine(point);
    }
  }

}
