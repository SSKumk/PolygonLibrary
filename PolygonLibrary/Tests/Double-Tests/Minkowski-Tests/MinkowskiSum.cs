using CGLibrary;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.Double_Tests.Minkowski_Tests;

[TestFixture]
public class MinkowskiSum {

#region Base Polytopes Tests
  [Test]
  public void Simplex_3D() {
    Polytop sum = MinkSumCH(Simplex3D, Simplex3D);

    Assert.That
      (sum.Vertices.SetEquals(Simplex3D.Vertices.Select(v => 2 * v)), "Sum of two 3D-simplexes: sets of vertices are not equal!");
  }

  [Test]
  public void Cube_4D() {
    Polytop sum = MinkSumCH(Cube4D, Cube4D);

    Assert.That
      (sum.Vertices.SetEquals(Cube4D.Vertices.Select(v => 2 * v)), "Sum of two 4D-cubes: sets of vertices are not equal!");
  }
#endregion

#region Base-polytopes-combined Tests
  [Test]
  public void Cube3D_Simplex3D() {
    Polytop sum = MinkSumCH(Simplex3D, Cube3D);

    Console.WriteLine(string.Join('\n', sum.Vertices));
  }
#endregion

#region Other tests

  /// <summary>
  /// Пример из статьи
  /// Computing the Minkowski sum of convex polytopes in Rd. Sandip Das, S. Swami. 2018г
  ///
  /// Результат см в Геогебре. Проверил глазами -- верно.
  /// </summary>
  [Test]
  public void Cube3D_Octahedron() {
    List<Point> p = GeneratePointsOnSphere_3D(3, 4);
    List<Point> q = GeneratePointsOnSphere_3D(2, 4, true);
    q = Rotate(q, MakeRotationMatrix(3, 1, 2, double.Pi / 4));

    // Console.WriteLine($"P has {p.Count} vertices.");
    // Console.WriteLine($"Q has {q.Count} vertices.");
    //
    // Console.WriteLine(string.Join('\n', p));
    // Console.WriteLine();
    // Console.WriteLine(string.Join('\n', q));
    // Console.WriteLine();

    Polytop P = new GiftWrapping(p).Polytop;
    Polytop Q = new GiftWrapping(q).Polytop;

    Polytop sum = MinkSumCH(P, Q);
    Console.WriteLine($"MinkSum(P,Q) has {sum.Vertices.Count} vertices.");

    Console.WriteLine(string.Join('\n', sum.Vertices));
  }

  /// <summary>
  /// Пример из статьи
  /// Computing the Minkowski sum of convex polytopes in Rd. Sandip Das, S. Swami. 2018г
  /// </summary>
  [Test]
  public void WorstCase3D() {
    const int   theta = 2;
    List<Point> p     = GeneratePointsOnSphere_3D(theta, 30, true);
    List<Point> q     = Rotate(p, MakeRotationMatrix(3, 1, 3, double.Pi / 2));

    Console.WriteLine($"P has {p.Count} vertices.");
    Console.WriteLine($"Q has {q.Count} vertices.");

    Polytop P = new GiftWrapping(p).Polytop;
    Polytop Q = new GiftWrapping(q).Polytop;

    Polytop sum = MinkSumCH(P, Q);
    Console.WriteLine($"MinkSum(P,Q) has {sum.Vertices.Count} vertices.");

    // Console.WriteLine(string.Join('\n', sum.Vertices));
  }
#endregion

}
