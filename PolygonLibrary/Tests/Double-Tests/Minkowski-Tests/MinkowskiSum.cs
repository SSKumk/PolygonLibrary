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
    FaceLattice sum_CH = MinkSumCH(Simplex3D, Simplex3D);
    FaceLattice sum = MinkSumSDas(Simplex3D_FL, Simplex3D_FL);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }

  [Test]
  public void Cube_3D() {
    FaceLattice sum_CH = MinkSumCH(Cube3D, Cube3D);
    FaceLattice sum = MinkSumSDas(Cube3D_FL, Cube3D_FL);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }

  [Test]
  public void Cube_4D() {
    FaceLattice sum_CH = MinkSumCH(Cube4D, Cube4D);
    FaceLattice sum = MinkSumSDas(Cube4D_FL, Cube4D_FL);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }
  #endregion

  #region Base-polytopes-combined Tests
  [Test]
  public void Cube3D_Simplex3D() {
    FaceLattice sum_CH = MinkSumCH(Simplex3D, Cube3D);
    FaceLattice sum = MinkSumSDas(Simplex3D_FL, Cube3D_FL);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }
  #endregion

  #region 3D tests with pictures
  /// <summary>
  /// Пример из статьи
  /// Computing the Minkowski sum of convex polytopes in Rd. Sandip Das, S. Swami. 2021г
  /// </summary>
  [Test]
  public void Cube3D_Octahedron45XY() {
    List<Point> p = GeneratePointsOnSphere_3D(3, 4);
    List<Point> q = Octahedron3D_list;
    q = Rotate(q, rotate3D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum = MinkSumSDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));


    // sum.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Cube3D_Octahedron45XY.txt");

    // Assert.That(sum.Faces.Count(F => Tools.EQ(F.Normal * Vector.CreateOrth(3, 3))), Is.EqualTo(8));
    // Assert.That(sum.Faces.Count(F => Tools.GT(F.Normal * Vector.CreateOrth(3, 3))), Is.EqualTo(9));
    // Assert.That(sum.Faces.Count(F => Tools.LT(F.Normal * Vector.CreateOrth(3, 3))), Is.EqualTo(9));
  }

  /// <summary>
  /// Пример из статьи
  /// Computing the Minkowski sum of convex polytopes in Rd. Sandip Das, S. Swami. 2021г
  /// P has 16 faces.
  /// Q has 16 faces.
  /// MinkSum(P,Q) has 60 faces.
  /// MinkSum(P,Q) has 210 faces + vertices + edges.
  /// </summary>
  [Test]
  public void WorstCase3D() {
    const int theta = 2;
    List<Point> p = GeneratePointsOnSphere_3D(theta, 8, true, true);
    p = Rotate(p, MakeRotationMatrix(3, 2, 3, -double.Pi / 18));
    List<Point> q = GeneratePointsOnSphere_3D(theta, 8, true, true);
    q = Rotate(q, MakeRotationMatrix(3, 1, 3, double.Pi / 2));

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum = MinkSumSDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));

    // Console.WriteLine($"P has {P.Faces.Count} faces.");
    // Console.WriteLine($"Q has {Q.Faces.Count} faces.");


    // P.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/WorstCase3D_P.txt");
    // Q.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/WorstCase3D_Q.txt");


    //  sum.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/WorstCase3D.txt");
  }

  [Test]
  public void Octahedron_Octahedron45XY() {
    List<Point> p = Octahedron3D_list;
    List<Point> q = Rotate(p, rotate3D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum = MinkSumSDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));
    // sum.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Octahedron_Octahedron45XY.txt");
  }

  [Test]
  public void Pyramid_Pyramid45XY() {
    List<Point> p = Pyramid3D_list;
    List<Point> q = Rotate(p, rotate3D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum = MinkSumSDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));

    // P.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Pyramid.txt");
    // Q.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Pyramid45XY.txt");

    // sum.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Pyramid_Pyramid45XY.txt");
  }


  [Test]
  public void Cube_Cube45XY() {
    List<Point> p = Cube3D_list;
    List<Point> q = Rotate(p, rotate3D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum = MinkSumSDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));

    sum.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Cube_Cube45XY.txt");
    // new GiftWrapping(MinkSum(p, q)).CPolytop.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Cube_Cube45XY++.txt");
  }
  #endregion


  #region Other tests
  [Test]
  public void Cube4D_Cube4D45XY() {
    List<Point> p = Cube4D_list;
    List<Point> q = Rotate(p, rotate4D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum = MinkSumSDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }
  #endregion

}
