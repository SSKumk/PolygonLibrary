using DoubleDouble;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests.Minkowski_Tests;

[TestFixture]
public class MinkowskiSum2D {

  private Vector      u0, u1, u2, u3, u4;
  private Vector      p0, p1, p2, p3;
  private FaceLattice pu3;
  private FaceLattice su1,    su2, su3, su4;
  private FaceLattice qu1_GW, qp1_GW;

  [OneTimeSetUp]
  public void SetUp() {
    u0 = new Vector(new ddouble[] { 0, 0 });
    u1 = new Vector(new ddouble[] { 1, 0 });
    u2 = new Vector(new ddouble[] { 0, 1 });
    u3 = new Vector(new ddouble[] { 1, 1 });
    u4 = new Vector(new ddouble[] { 1, 2 });


    p0 = new Vector(new ddouble[] { 0.5, 0 });
    p1 = new Vector(new ddouble[] { 1, 0.5 });
    p2 = new Vector(new ddouble[] { 0.5, 1 });
    p3 = new Vector(new ddouble[] { 0, 0.5 });

    pu3 = new FaceLattice(u3);
    su1 = GiftWrapping.WrapFaceLattice(new List<Vector> { u0, u1 });
    su2 = GiftWrapping.WrapFaceLattice(new List<Vector> { u0, u2 });
    su3 = GiftWrapping.WrapFaceLattice(new List<Vector> { u0, u3 });
    su4 = GiftWrapping.WrapFaceLattice(new List<Vector> { u3, u4 });

    qu1_GW = GiftWrapping.WrapFaceLattice
      (
       new List<Vector>
         {
           u0
         , u1
         , u2
         , u3
         }
      );
    qp1_GW = GiftWrapping.WrapFaceLattice
      (
       new List<Vector>
         {
           p0
         , p1
         , p2
         , p3
         }
      );
  }

  private static FaceLattice MSumCH(FaceLattice F, FaceLattice G)
    => MinkowskiSum.ByConvexHull(new ConvexPolytop(F.Vertices), new ConvexPolytop(G.Vertices)).FL;


  // Сумма точек
  [Test]
  public void Point_Point() {
    FaceLattice pu3_pu3 = MinkowskiSum.BySandipDas(pu3, pu3);
    Assert.That(pu3_pu3, Is.EqualTo(new FaceLattice(new Vector(new ddouble[] { 2, 2 }))));
  }

  // Сдвинутый отрезок
  [Test]
  public void Point_Seg() {
    FaceLattice pu3_su1 = MinkowskiSum.BySandipDas(pu3, su1);
    Assert.That(pu3_su1, Is.EqualTo(MSumCH(pu3, su1)));
  }

  // Сдвинутый отрезок
  [Test]
  public void Point_Seg1() {
    FaceLattice pu3_su3 = MinkowskiSum.BySandipDas(pu3, su3);
    Assert.That(pu3_su3, Is.EqualTo(MSumCH(pu3, su3)));
  }

  // Удвоенный отрезок
  [Test]
  public void doubled_Seg() {
    FaceLattice su3_su3 = MinkowskiSum.BySandipDas(su3, su3);
    Assert.That(su3_su3, Is.EqualTo(MSumCH(su3, su3)));
  }

  // Единичный квадрат
  [Test]
  public void Seg_Seg() {
    FaceLattice qu1 = MinkowskiSum.BySandipDas(su1, su2);
    Assert.That(qu1, Is.EqualTo(qu1_GW));
  }

  // Отрезки
  [Test]
  public void Seg_AnotherSeg() {
    FaceLattice q = MinkowskiSum.BySandipDas(su3, su4);
    Assert.That(q, Is.EqualTo(MSumCH(su3, su4)));
  }

  // Прямоугольник
  [Test]
  public void Seg_Square() {
    FaceLattice su1_qu1 = MinkowskiSum.BySandipDas(su1, qu1_GW);
    Assert.That(su1_qu1, Is.EqualTo(MSumCH(su1, qu1_GW)));
  }

  // Шестиугольник
  [Test]
  public void Seg45_Square() {
    FaceLattice su3_qu1 = MinkowskiSum.BySandipDas(su3, qu1_GW);
    Assert.That(su3_qu1, Is.EqualTo(MSumCH(su3, qu1_GW)));
  }

  // Квадрат в два раза больший
  [Test]
  public void Square_Square() {
    FaceLattice double_qu1 = MinkowskiSum.BySandipDas(qu1_GW, qu1_GW);
    Assert.That(double_qu1, Is.EqualTo(MSumCH(qu1_GW, qu1_GW)));
  }

  [Test]
  public void Square45_Square() {
    FaceLattice qp1_qu1 = MinkowskiSum.BySandipDas(qp1_GW, qu1_GW);
    Assert.That(qp1_qu1, Is.EqualTo(MSumCH(qp1_GW, qu1_GW)));
  }

}

[TestFixture]
public class MinkowskiSum_hD {

  private static FaceLattice MinkSumCH(ConvexPolytop F, ConvexPolytop G) => MinkowskiSum.ByConvexHull(F, G).FL;

#region Base Polytopes Tests
  [Test]
  public void Simplex_3D() {
    FaceLattice sum_CH = MinkSumCH(Simplex3D, Simplex3D);
    FaceLattice sum    = MinkowskiSum.BySandipDas(Simplex3D_FL, Simplex3D_FL);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }

  [Test]
  public void Cube_3D() {
    FaceLattice sum_CH = MinkSumCH(Cube3D, Cube3D);
    FaceLattice sum    = MinkowskiSum.BySandipDas(Cube3D_FL, Cube3D_FL);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }

  [Test]
  public void Cube_4D() {
    FaceLattice sum_CH = MinkSumCH(Cube4D, Cube4D);
    FaceLattice sum    = MinkowskiSum.BySandipDas(Cube4D_FL, Cube4D_FL);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }
#endregion

#region Base-polytopes-combined Tests
  [Test]
  public void Cube3D_Simplex3D() {
    FaceLattice sum_CH = MinkSumCH(Simplex3D, Cube3D);
    FaceLattice sum    = MinkowskiSum.BySandipDas(Simplex3D_FL, Cube3D_FL);

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
    List<Vector> p = MakePointsOnSphere_3D(3, 4);
    List<Vector> q = Octahedron3D_list;
    q = Rotate(q, rotate3D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum    = MinkowskiSum.BySandipDas(P.FaceLattice, Q.FaceLattice);

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
    const int    theta = 2;
    List<Vector> p     = MakePointsOnSphere_3D(theta, 8, true, true);
    p = Rotate(p, MakeRotationMatrix(3, 2, 3, -double.Pi / 18));
    List<Vector> q = MakePointsOnSphere_3D(theta, 8, true, true);
    q = Rotate(q, MakeRotationMatrix(3, 1, 3, double.Pi / 2));

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum    = MinkowskiSum.BySandipDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));

    // Console.WriteLine($"P has {P.Faces.Count} faces.");
    // Console.WriteLine($"Q has {Q.Faces.Count} faces.");


    // P.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/WorstCase3D_P.txt");
    // Q.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/WorstCase3D_Q.txt");


    //  sum.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/WorstCase3D.txt");
  }

  [Test]
  public void Octahedron_Octahedron45XY() {
    List<Vector> p = Octahedron3D_list;
    List<Vector> q = Rotate(p, rotate3D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum    = MinkowskiSum.BySandipDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));
    // sum.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Octahedron_Octahedron45XY.txt");
  }

  [Test]
  public void Pyramid_Pyramid45XY() {
    List<Vector> p = Pyramid3D_list;
    List<Vector> q = Rotate(p, rotate3D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum    = MinkowskiSum.BySandipDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));

    // P.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Pyramid.txt");
    // Q.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Pyramid45XY.txt");

    // sum.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Pyramid_Pyramid45XY.txt");
  }


  [Test]
  public void Cube_Cube45XY() {
    List<Vector> p = Cube3D_list;
    List<Vector> q = Rotate(p, rotate3D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum    = MinkowskiSum.BySandipDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));

    // sum.WriteTXTasCPolytop("../../../Double-Tests/Minkowski-Tests/3D-pictures/Cube_Cube45XY.txt");
    // new GiftWrapping(MinkSum(p, q)).CPolytop.WriteTXT("../../../Double-Tests/Minkowski-Tests/3D-pictures/Cube_Cube45XY++.txt");
  }
#endregion


#region Other tests
  [Test]
  public void Cube4D_Cube4D45XY() {
    List<Vector> p = Cube4D_list;
    List<Vector> q = Rotate(p, rotate4D_45XY);

    GiftWrapping P = new GiftWrapping(p);
    GiftWrapping Q = new GiftWrapping(q);

    FaceLattice sum_CH = MinkSumCH(P.CPolytop, Q.CPolytop);
    FaceLattice sum    = MinkowskiSum.BySandipDas(P.FaceLattice, Q.FaceLattice);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }

  [Test]
  public void Cube2D_Cube2DinAnotherPlane() {
    var p0 = new Vector(new ddouble[] { 0, 0, 0, 0 });
    var p1 = new Vector(new ddouble[] { 1, 0, 0, 0 });
    var p2 = new Vector(new ddouble[] { 0, 1, 0, 0 });
    var p3 = new Vector(new ddouble[] { 1, 1, 0, 0 });

    var u0 = new Vector(new ddouble[] { 0, 0, 1, 1 });
    var u1 = new Vector(new ddouble[] { 1, 0, 1, 1 });
    var u2 = new Vector(new ddouble[] { 0, 1, 1, 1 });
    var u3 = new Vector(new ddouble[] { 1, 1, 1, 1 });

    ConvexPolytop P = GiftWrapping.WrapPolytop
      (
       new List<Vector>()
         {
           p0
         , p1
         , p2
         , p3
         }
      );
    ConvexPolytop Q = GiftWrapping.WrapPolytop
      (
       new List<Vector>()
         {
           u0
         , u1
         , u2
         , u3
         }
      );

    FaceLattice sum_CH = MinkSumCH(P, Q);
    FaceLattice sum    = MinkowskiSum.BySandipDas(P.FL, Q.FL);

    Assert.That(sum_CH, Is.EqualTo(sum));
  }
#endregion

}

[TestFixture]
public class MinkowskiSum5D {

  // private static Matrix rotate5D_45_12, rotate5D_45_35;
  private static List<Vector> cube5D,       cube4D,       cube3D,       cube2D;
  private static List<Vector> simplex5D,    simplex4D,    simplex3D,    simplex2D;
  private static List<Vector> simplexRND5D, simplexRND4D, simplexRND3D, simplexRND2D;

  private static readonly List<List<Vector>> allCubes_lst,     all_lst;
  private static          List<List<Vector>> allSimplices_lst, allSimplicesRND_lst;

  // private List<FaceLattice> allCubes_FL;
  // private Vector shift;

  static MinkowskiSum5D() {
    cube5D = Cube5D_list;
    cube4D = Cube4D_list.Select(p => p.ExpandTo(5)).ToList();
    cube3D = Cube3D_list.Select(p => p.ExpandTo(5)).ToList();
    cube2D = Cube2D_list.Select(p => p.ExpandTo(5)).ToList();

    simplex5D = Simplex5D_list;
    simplex4D = Simplex4D_list.Select(p => p.ExpandTo(5)).ToList();
    simplex3D = Simplex3D_list.Select(p => p.ExpandTo(5)).ToList();
    simplex2D = Simplex2D_list.Select(p => p.ExpandTo(5)).ToList();

    simplexRND5D = SimplexRND5D_list;
    simplexRND4D = SimplexRND4D_list.Select(p => p.ExpandTo(5)).ToList();
    simplexRND3D = SimplexRND3D_list.Select(p => p.ExpandTo(5)).ToList();
    simplexRND2D = SimplexRND2D_list.Select(p => p.ExpandTo(5)).ToList();

    allCubes_lst = new List<List<Vector>>()
      {
        cube2D
      , cube3D
      , cube4D
      , cube5D
      };
    allSimplices_lst = new List<List<Vector>>()
      {
        simplex2D
      , simplex3D
      , simplex4D
      , simplex5D
      };
    allSimplicesRND_lst = new List<List<Vector>>()
      {
        simplexRND2D
      , simplexRND3D
      , simplexRND4D
      , simplexRND5D
      };

    all_lst = new List<List<Vector>>();
    all_lst.AddRange(allCubes_lst);
    all_lst.AddRange(allSimplices_lst);
    all_lst.AddRange(allSimplicesRND_lst);
  }

  public static IEnumerable<TestCaseData> AllCubes_AllTransformed(
      bool   needToRot
    , int    fst
    , int    snd
    , double angle
    , bool   needToShift
    ) {
    Matrix rotate5D = MakeRotationMatrix(5, fst, snd, angle);
    Vector shift    = GenShift(5, new GRandomLC(111));

    IEnumerable<FaceLattice> allCubes_FL = allCubes_lst.Select(GiftWrapping.WrapFaceLattice);

    IEnumerable<FaceLattice> all_FL       = all_lst.Select(GiftWrapping.WrapFaceLattice);
    IEnumerable<FaceLattice> all_FL_Rot   = all_lst.Select(l => GiftWrapping.WrapFaceLattice(Rotate(l, rotate5D)));
    IEnumerable<FaceLattice> all_FL_Shift = all_lst.Select(l => GiftWrapping.WrapFaceLattice(Shift(l, shift)));


    IEnumerable<FaceLattice> toTreat = all_FL;
    if (needToRot) {
      toTreat = all_FL_Rot;
    } else if (needToShift) {
      toTreat = all_FL_Shift;
    }

    // перебираем кубы
    foreach (FaceLattice cube in allCubes_FL) {
      // перебираем всё остальное
      foreach (FaceLattice other in all_FL) {
        yield return new TestCaseData(cube, other);
      }
    }
  }

  /// <summary>
  /// All cubes sum to all (cubes, simplices and rnd simplices)
  /// </summary>
  [Test, TestCaseSource(nameof(AllCubes_AllTransformed), new object[] { false, 1, 2, 0, false })]
  public void AllCubes_AllTest(FaceLattice cube, FaceLattice other) {
    FaceLattice sum_CH = MinkowskiSum.BySandipDas(cube, other);
    FaceLattice sum    = MinkowskiSum.BySandipDas(cube, other);
    FaceLattice sumSim = MinkowskiSum.BySandipDas(other, cube);

    Assert.That(sum_CH, Is.EqualTo(sum));
    Assert.That(sum_CH, Is.EqualTo(sumSim));
  }

  [Test, TestCaseSource(nameof(AllCubes_AllTransformed), new object[] { false, 1, 2, 0, true })]
  public void AllCubes_AllShiftedTest(FaceLattice cube, FaceLattice other) {
    FaceLattice sum_CH = MinkowskiSum.BySandipDas(cube, other);
    FaceLattice sum    = MinkowskiSum.BySandipDas(cube, other);
    FaceLattice sumSim = MinkowskiSum.BySandipDas(other, cube);

    Assert.That(sum_CH, Is.EqualTo(sum));
    Assert.That(sum_CH, Is.EqualTo(sumSim));
  }

  [Test, TestCaseSource(nameof(AllCubes_AllTransformed), new object[] { true, 1, 2, 45, false })]
  public void AllCubes_AllRotated12_45degTest(FaceLattice cube, FaceLattice other) {
    FaceLattice sum_CH = MinkowskiSum.BySandipDas(cube, other);
    FaceLattice sum    = MinkowskiSum.BySandipDas(cube, other);
    FaceLattice sumSim = MinkowskiSum.BySandipDas(other, cube);

    Assert.That(sum_CH, Is.EqualTo(sum));
    Assert.That(sum_CH, Is.EqualTo(sumSim));
  }

  [Test, TestCaseSource(nameof(AllCubes_AllTransformed), new object[] { true, 3, 5, 45, false })]
  public void AllCubes_AllRotated35_45degTest(FaceLattice cube, FaceLattice other) {
    FaceLattice sum_CH = MinkowskiSum.BySandipDas(cube, other);
    FaceLattice sum    = MinkowskiSum.BySandipDas(cube, other);
    FaceLattice sumSim = MinkowskiSum.BySandipDas(other, cube);

    Assert.That(sum_CH, Is.EqualTo(sum));
    Assert.That(sum_CH, Is.EqualTo(sumSim));
  }

}
