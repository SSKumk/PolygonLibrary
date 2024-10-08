using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Tests.DoubleDouble_Tests.GW_hDTests;

[TestFixture]
public class GW_Tests {

  [OneTimeSetUp]
  public void SetUp() { Tools.Eps = 1e-16; }

#region Auxiliary tests
  [Test]
  public void GenCubeListHDTest() {
    SortedSet<Vector> S = new SortedSet<Vector>()
      {
        new Vector(new ddouble[] { 0, 0, 0 })
      , new Vector(new ddouble[] { 1, 0, 0 })
      , new Vector(new ddouble[] { 0, 1, 0 })
      , new Vector(new ddouble[] { 0, 0, 1 })
      , new Vector(new ddouble[] { 1, 1, 0 })
      , new Vector(new ddouble[] { 0, 1, 1 })
      , new Vector(new ddouble[] { 1, 0, 1 })
      , new Vector(new ddouble[] { 1, 1, 1 })
      };

    List<Vector> cube = Cube01(3, out List<Vector> _);

    Debug.Assert(S.SetEquals(new SortedSet<Vector>(cube)), "S is not equal to generated Cube01");
  }

  [Test]
  public void Aux() {
    const int PDim    = 3;
    const int nPoints = 1;
    List<int> fID     = new List<int>() { 1 };
    GRandomLC random  = new GRandomLC(166605533);

    List<Vector> S = Simplex(PDim, out List<Vector> polytop, fID, nPoints, random);
    ShiftAndRotate(3, ref polytop, ref S, random);
    S.Shuffle(random);

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(polytop), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(4), $"The number of facets of the cube must be equal to 4.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(15), $"The number of faces of the cube must be equal to 15.");
  }
#endregion

#region Simplex3D-Static Тесты 3D-симлека не зависящие от _random
  [Test]
  public void Simplex3D() {
    List<Vector> S = new List<Vector>()
      {
        new Vector(new ddouble[] { 1, 0, 0 })
      , new Vector(new ddouble[] { 0, 1, 0 })
      , new Vector(new ddouble[] { 0, 0, 1 })
      , new Vector(new ddouble[] { 1, 1, 1 })
      };

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(S), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(4), "The number of facets of the simplex must be equal to 4.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(15), "The number of faces of the simplex must be equal to 15.");
  }

  [Test]
  public void Simplex3D_1D() {
    List<Vector> S = new List<Vector>()
      {
        new Vector(new ddouble[] { 1, 0, 0 })
      , new Vector(new ddouble[] { 0, 1, 0 })
      , new Vector(new ddouble[] { 0, 0, 1 })
      , new Vector(new ddouble[] { 1, 1, 1 })
      };

    List<Vector> Swarm = new List<Vector>()
      {
        // new Vector(new ddouble[] { 0.5, 0.5, 0.5 })
        new Vector(new ddouble[] { 1, 0, 0 })
      , new Vector(new ddouble[] { 0.5, 0.5, 1 })
      , new Vector(new ddouble[] { 0, 1, 0 })
      , new Vector(new ddouble[] { 0.5, 0, 0.5 })
      , new Vector(new ddouble[] { 0, 0, 1 })
      , new Vector(new ddouble[] { 1, 0.5, 0.5 })
      , new Vector(new ddouble[] { 1, 1, 1 })
      , new Vector(new ddouble[] { 0, 0.5, 0.5 })
      , new Vector(new ddouble[] { 0.5, 0, 0.5 })
      };

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(Swarm, true);
    Assert.That(P.Vrep.SetEquals(S), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(4), "The number of facets of the simplex must be equal to 4.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(15), "The number of faces of the simplex must be equal to 15.");
  }
#endregion


#region Cube3D-Static Тесты 3D-куба не зависящие от _random
  [Test]
  public void Cube3D() {
    List<Vector> S = Cube3D_list;

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(S), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  [Test]
  public void Cube3D_Rotated_Z45() {
    List<Vector> S     = Cube3D_list;
    ddouble      angle = Tools.PI / 4;
    ddouble      sin   = ddouble.Sin(angle);
    ddouble      cos   = ddouble.Cos(angle);

    ddouble[,] rotationZ45 = { { cos, -sin, 0 }, { sin, cos, 0 }, { 0, 0, 1 } };

    List<Vector> Rotated = Rotate(S, new Matrix(rotationZ45));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(Rotated, true);
    Assert.That(P.Vrep.SetEquals(Rotated), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  /// <summary>
  /// Как-то повёрнутый куб
  /// </summary>
  [Test]
  public void Cube3D_Rotated() {
    SortedSet<Vector> S = new SortedSet<Vector>()
      {
        new Vector(new ddouble[] { 0, 0, 0 })
      , new Vector(new ddouble[] { 0.6800213885880926, 0.3956859369533106, 0.6172548504143999 })
      , new Vector(new ddouble[] { -0.47124672587598565, -0.40907382401238557, 0.7813994688115978 })
      , new Vector(new ddouble[] { 0.20877466271210693, -0.013387887059074954, 1.3986543192259977 })
      , new Vector(new ddouble[] { -0.561691583000748, 0.822247679112118, 0.0917132475755244 })
      , new Vector(new ddouble[] { 0.11832980558734463, 1.2179336160654286, 0.7089680979899242 })
      , new Vector(new ddouble[] { -1.0329383088767337, 0.4131738550997325, 0.8731127163871222 })
      , new Vector(new ddouble[] { -0.3529169202886411, 0.8088597920530431, 1.4903675668015222 })
      };


    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(S), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  /// <summary>
  /// Как-то сдвинутый куб
  /// </summary>
  [Test]
  public void Cube3D_Shifted() {
    SortedSet<Vector> S = new SortedSet<Vector>()
      {
        new Vector(new ddouble[] { -10.029417029821644, -8.414457472370579, 12.142282885765258 })
      , new Vector(new ddouble[] { -10.029417029821644, -8.414457472370579, 13.142282885765258 })
      , new Vector(new ddouble[] { -10.029417029821644, -7.414457472370579, 12.142282885765258 })
      , new Vector(new ddouble[] { -10.029417029821644, -7.414457472370579, 13.142282885765258 })
      , new Vector(new ddouble[] { -9.029417029821644, -8.414457472370579, 12.142282885765258 })
      , new Vector(new ddouble[] { -9.029417029821644, -8.414457472370579, 13.142282885765258 })
      , new Vector(new ddouble[] { -9.029417029821644, -7.414457472370579, 12.142282885765258 })
      , new Vector(new ddouble[] { -9.029417029821644, -7.414457472370579, 13.142282885765258 })
      };

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(S), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D() {
    List<Vector> S = Cube01(3, out List<Vector> cube, new List<int>() { 1 }, 1, new GRandomLC(131));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D() {
    List<Vector> S = Cube01(3, out List<Vector> cube, new List<int>() { 2 }, 1, new GRandomLC(132));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_3D() {
    List<Vector> S = Cube01(3, out List<Vector> cube, new List<int>() { 3 }, 1, new GRandomLC(133));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D() {
    List<Vector> S = Cube01(3, out List<Vector> cube, new List<int>() { 1, 2 }, 1, new GRandomLC(1312));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_2D_3D() {
    List<Vector> S = Cube01(3, out List<Vector> cube, new List<int>() { 2, 3 }, 1, new GRandomLC(1323));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }

  [Test]
  public void Cube3D_withInnerPoints_On_1D_2D_3D() {
    List<Vector> S = Cube01(3, out List<Vector> cube, new List<int>() { 1, 2, 3 }, 1, new GRandomLC(13123));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(6), "The number of facets of the cube must be equal to 6.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(27), "The number of faces of the cube must be equal to 27.");
  }
#endregion

#region Suffle-Zone PurePolytops Тесты перестановок без дополнительных точек
  /// <summary>
  /// Shuffles the elements of the S list and wraps it into a Polyhedron.
  /// Asserts that the set of vertices in the Polyhedron is equal to the Polytop list.
  /// </summary>
  /// <param name="S">The list of points representing the Polytop.</param>
  /// <param name="nameOfTest">The name of the current test.</param>
  /// <param name="numberOfHRep">The number of half-spaces in Hrep.</param>
  /// <param name="numberOfFVec">The number of all faces of the polytop.</param>
  private static void SwarmShuffleAndCheckVertices(List<Vector> S, string nameOfTest, int numberOfHRep, int numberOfFVec) {
    int sCount = 10 * S.Count;
    for (int i = 0; i < sCount; i++) {
      uint saveSeed = _random.Seed;
      S.Shuffle(_random);
      ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
      Assert.That(P.Vrep.SetEquals(S), $"{nameOfTest}: The set of vertices must be equal.\nSeed: {saveSeed}");
      Assert.That(P.Hrep, Has.Count.EqualTo(numberOfHRep), $"The number of facets of the cube must be equal to {numberOfHRep}.");
      Assert.That
        (P.FLrep.NumberOfKFaces, Is.EqualTo(numberOfFVec), $"The number of faces of the cube must be equal to {numberOfFVec}.");
    }
  }

  [Test]
  public void Cube3D_Shuffled() {
    List<Vector> S = Cube01(3, out List<Vector> _);
    SwarmShuffleAndCheckVertices(S, "Cube3D_Shuffled", 6, 27);
  }

  [Test]
  public void Cube4D_Suffled() {
    List<Vector> S = Cube01(4, out List<Vector> _);
    SwarmShuffleAndCheckVertices(S, "Cube4D_Shuffled", 8, 81);
  }

  [Test]
  public void Simplex3D_Suffled() {
    List<Vector> S = Simplex(3, out List<Vector> _);
    SwarmShuffleAndCheckVertices(S, "Simplex3D_Shuffled", 4, 15);
  }

  [Test]
  public void Simplex4D_Suffled() {
    List<Vector> S = Simplex(4, out List<Vector> _);
    SwarmShuffleAndCheckVertices(S, "Simplex4D_Shuffled", 5, 31);
  }
#endregion

#region Cube4D-Static Тесты 4D-куба не зависящие от _random
  [Test]
  public void Cube4D_withInnerPoints_On_1D() {
    List<Vector> S = Cube01(4, out List<Vector> cube, new List<int>() { 1 }, 1, new GRandomLC(141));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
  }


  [Test]
  public void Cube4D_withInnerPoints_On_2D() {
    List<Vector> S = Cube01(4, out List<Vector> cube, new List<int>() { 2 }, 1, new GRandomLC(142));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_3D() {
    List<Vector> S = Cube01(4, out List<Vector> cube, new List<int>() { 3 }, 1, new GRandomLC(143));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D() {
    List<Vector> S = Cube01(4, out List<Vector> cube, new List<int>() { 1, 2 }, 1, new GRandomLC(1412));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_2D_3D() {
    List<Vector> S = Cube01(4, out List<Vector> cube, new List<int>() { 2, 3 }, 1, new GRandomLC(1423));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D() {
    List<Vector> S = Cube01(4, out List<Vector> cube, new List<int>() { 1, 2, 3 }, 1, new GRandomLC(14123));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
  }

  [Test]
  public void Cube4D_withInnerPoints_On_1D_2D_3D_4D() {
    List<Vector> S = Cube01
      (
       4
     , out List<Vector> cube
     , new List<int>()
         {
           1
         , 2
         , 3
         , 4
         }
     , 1
     , new GRandomLC(141234)
      );

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);

    Assert.That(P.Vrep.SetEquals(cube), "The set of vertices must be equal.");
  }
#endregion

#region Simplex4D Тесты 4D-симплекса не зависящие от _random
  [Test]
  public void Simplex4D_1DEdge_2DNeighborsPointsTest() {
    Vector p0 = new Vector(new ddouble[] { 0, 0, 0, 0 });
    Vector p1 = new Vector(new ddouble[] { 1, 0, 0, 0 });
    Vector p2 = new Vector(new ddouble[] { 0, 1, 0, 0 });
    Vector p3 = new Vector(new ddouble[] { 0.1, 0, 1, 0 });
    Vector p4 = new Vector(new ddouble[] { 0.1, 0, 0, 1 });

    List<Vector> Simplex = new List<Vector>()
      {
        p0
      , p1
      , p2
      , p3
      , p4
      };


    List<Vector> S = new List<Vector>(Simplex)
      {
        Vector.LinearCombination(p1, 0.3, p2, 0.2)
      , Vector.LinearCombination(p1, 0.4, p2, 0.1)
      , Vector.LinearCombination(p1, 0.4, p3, 0.1)
      , Vector.LinearCombination(p1, 0.4, p3, 0.1)
      , Vector.LinearCombination(p1, 0.4, p4, 0.1)
      , Vector.LinearCombination(p1, 0.4, p4, 0.1)
      };

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(Simplex), "The set of vertices must be equal.");
  }
#endregion

#region AllCubes Генераторы "плохих" тестов для кубов
  [Test]
  public void AllCubes3D_TestRND() {
    const int nPoints = 5000;

    List<List<int>> fIDs = Enumerable.Range(1, 3).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Cube01(3, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 3
       , nPoints
       , fID
       , 6
       , 27
        );
    }
  }

  [Test]
  public void AllCubes4D_TestRND() {
    const int nPoints = 2000;

    List<List<int>> fIDs = Enumerable.Range(1, 4).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Cube01(4, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 4
       , nPoints
       , fID
       , 8
       , 81
        );
    }
  }

  [Test]
  public void AllCubes5D_TestRND() {
    const int nPoints = 5;

    List<List<int>> fIDs = Enumerable.Range(1, 5).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Cube01(5, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 5
       , nPoints
       , fID
       , 10
       , 243
        );
    }
  }

  [Test]
  public void AllCubes6D_TestRND() {
    const int nPoints = 2;

    List<List<int>> fIDs = Enumerable.Range(1, 6).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Cube01(6, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 6
       , nPoints
       , fID
       , 12
       , 729
        );
    }
  }
#endregion

#region AllSimplices Генераторы "плохих" тестов для симплексов полученных из базисных орт
  [Test]
  public void AllSimplices3D_TestRND() {
    const int nPoints = 2000;

    List<List<int>> fIDs = Enumerable.Range(1, 3).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Simplex(3, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 3
       , nPoints
       , fID
       , 4
       , 15
        );
    }
  }

  [Test]
  public void AllSimplices4D_TestRND() {
    const int nPoints = 2000;

    List<List<int>> fIDs = Enumerable.Range(1, 4).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Simplex(4, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 4
       , nPoints
       , fID
       , 5
       , 31
        );
    }
  }

  [Test]
  public void AllSimplices5D_TestRND() {
    const int nPoints = 1000;

    List<List<int>> fIDs = Enumerable.Range(1, 5).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Simplex(5, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 5
       , nPoints
       , fID
       , 6
       , 63
        );
    }
  }

  [Test]
  public void AllSimplices6D_TestRND() {
    const int nPoints = 5;

    List<List<int>> fIDs = Enumerable.Range(1, 6).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Simplex(6, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 6
       , nPoints
       , fID
       , 7
       , 127
        );
    }
  }

  [Test]
  public void AllSimplices7D_TestRND() {
    const int nPoints = 2;

    List<List<int>> fIDs = Enumerable.Range(1, 7).ToList().AllSubsets();

    foreach (List<int> fID in fIDs) {
      uint saveSeed = _random.Seed;

      List<Vector> S = Simplex(7, out List<Vector> P, fID, nPoints);

      Check
        (
         S
       , P
       , saveSeed
       , 7
       , nPoints
       , fID
       , 8
       , 255
        );
    }
  }
#endregion

#region AllSimplicesRND Генераторы "плохих" тестов для произвольных симплексов
  [Test]
  public void AllSimplicesRND_3D_TestRND() {
    const int nPoints    = 10;
    const int simplexDim = 3;

    List<List<int>> fIDs = Enumerable.Range(1, simplexDim).ToList().AllSubsets();

    for (int i = 0; i < 1e4; i++) {
      foreach (List<int> fID in fIDs) {
        uint saveSeed = _random.Seed;

        List<Vector> S = SimplexRND(simplexDim, out List<Vector> P, fID, nPoints);
        Check
          (
           S
         , P
         , saveSeed
         , simplexDim
         , nPoints
         , fID
         , 4
         , 15
          );
      }
    }
  }

  [Test]
  public void AllSimplicesRND_4D_TestRND() {
    const int nPoints    = 10;
    const int simplexDim = 4;

    List<List<int>> fIDs = Enumerable.Range(1, simplexDim).ToList().AllSubsets();

    for (int i = 0; i < 1e3; i++) {
      foreach (List<int> fID in fIDs) {
        uint saveSeed = _random.Seed;

        List<Vector> S = SimplexRND(simplexDim, out List<Vector> P, fID, nPoints);
        Check
          (
           S
         , P
         , saveSeed
         , simplexDim
         , nPoints
         , fID
         , 5
         , 31
          );
      }
    }
  }

  [Test]
  public void AllSimplicesRND_5D_TestRND() {
    const int nPoints    = 10;
    const int simplexDim = 5;

    List<List<int>> fIDs = Enumerable.Range(1, simplexDim).ToList().AllSubsets();

    for (int i = 0; i < 1e2; i++) {
      foreach (List<int> fID in fIDs) {
        uint saveSeed = _random.Seed;

        List<Vector> S = SimplexRND(simplexDim, out List<Vector> P, fID, nPoints);
        Check
          (
           S
         , P
         , saveSeed
         , simplexDim
         , nPoints
         , fID
         , 6
         , 63
          );
      }
    }
  }
#endregion


#region Other tests
  /// <summary>
  /// Пример из какой-то статьи
  /// </summary>
  [Test]
  public void SomePolytop_3D() {
    List<Vector> S = new List<Vector>()
      {
        new Vector(new ddouble[] { 1, 0, 1 })
      , new Vector(new ddouble[] { 1, 0, -1 })
      , new Vector(new ddouble[] { 1.25, -1, 1 })
      , new Vector(new ddouble[] { 1.25, -1, -1 })
      , new Vector(new ddouble[] { 0.25, -1, 1 })
      , new Vector(new ddouble[] { 0.25, -1, -1 })
      , new Vector(new ddouble[] { -1, 0, 1 })
      , new Vector(new ddouble[] { -1, 0, -1 })
      , new Vector(new ddouble[] { -1.25, 1, 1 })
      , new Vector(new ddouble[] { -1.25, 1, -1 })
      , new Vector(new ddouble[] { -0.25, 1, 1 })
      , new Vector(new ddouble[] { -0.25, 1, -1 })
      };

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(S), "The set of vertices must be equal.");
  }

  /// <summary>
  /// Параллелепипед расположенный в первом квадранте
  /// </summary>
  [Test]
  public void SomeParallelogram() {
    Vector origin = new Vector(3);
    Vector v1     = new Vector(new ddouble[] { 0.5, 1, 1 });
    Vector v2     = new Vector(new ddouble[] { 1, 0.5, 1 });
    Vector v3     = new Vector(new ddouble[] { 1, 1, 0.5 });

    List<Vector> S = new List<Vector>()
      {
        origin
      , origin + v1
      , origin + v2
      , origin + v3
      , origin + v1 + v2
      , origin + v1 + v3
      , origin + v2 + v3
      , origin + v1 + v2 + v3
      };

    SwarmShuffleAndCheckVertices(S, "SomeParallelogram", 6, 27);
  }
#endregion

  /// <summary>
  /// Procedure checks the answer obtained by Gift Wrapping algorithm and the answer given by user.
  /// </summary>
  /// <param name="S">The swarm to convexify.</param>
  /// <param name="Answer">The final list of points.</param>
  /// <param name="seed"></param>
  /// <param name="PDim"></param>
  /// <param name="nPoints"></param>
  /// <param name="fID"></param>
  /// <param name="numberHRep"></param>
  /// <param name="numberFVec"></param>
  private static void Check(
      List<Vector> S
    , List<Vector> Answer
    , uint         seed
    , int          PDim
    , int          nPoints
    , List<int>    fID
    , int          numberHRep
    , int          numberFVec
    ) {
    ShiftAndRotate(PDim, ref Answer, ref S);
    S.Shuffle(_random);

    ConvexPolytop P;
    try { P = ConvexPolytop.CreateFromPoints(S, true); }
    catch (Exception e) {
      GenTest
        (
         seed
       , PDim
       , nPoints
       , fID
       , numberHRep
       , numberFVec
        );

      throw new ArgumentException($"Error in gift wrapping!\n{e.Message}");
    }

    try {
      Assert.That(P.Vrep.SetEquals(Answer));
    }
    catch (Exception e) {
      // Console.WriteLine("Gift wrapping success. But sets of vertices do not equal!");
      GenTest
        (
         seed
       , PDim
       , nPoints
       , fID
       , numberHRep
       , numberFVec
        );

      Console.WriteLine(e.Message);

      throw new ArgumentException("The set of vertices must be equal.");
    }

    try {
      Assert.That(P.Hrep, Has.Count.EqualTo(numberHRep));
    }
    catch (Exception e) {
      // Console.WriteLine("Gift wrapping success. But half-spaces number is not equal!");
      GenTest
        (
         seed
       , PDim
       , nPoints
       , fID
       , numberHRep
       , numberFVec
        );

      Console.WriteLine(e.Message);

      throw new ArgumentException($"The half-spaces number must be equal to {numberHRep}.");
    }

    try {
      Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(numberFVec));
    }
    catch (Exception e) {
      // Console.WriteLine("Gift wrapping success. But half-spaces number is not equal!");
      GenTest
        (
         seed
       , PDim
       , nPoints
       , fID
       , numberHRep
       , numberFVec
        );

      Console.WriteLine(e.Message);

      throw new ArgumentException($"The f-vector number must be equal to {numberFVec}.");
    }
  }

  private static void GenTest(
      uint      seed
    , int       PDim
    , int       nPoints
    , List<int> fID
    , int       numberHRep
    , int       numberFVec
    ) {
    Console.WriteLine();
    Console.WriteLine($"[Test]");
    Console.WriteLine("public void Aux() {");
    Console.WriteLine($"const int PDim    = {PDim};");
    Console.WriteLine($"const int nPoints = {nPoints};");
    Console.WriteLine($"List<int> fID     = new List<int>() {{ {string.Join(", ", fID)} }};");

    Console.WriteLine($"GRandomLC random = new GRandomLC({seed});");


    Console.WriteLine();
    Console.WriteLine("List<Vector> S = ИМЯ_ФУНКЦИИ_ГЕНЕРАТОРА(PDim, out List<Vector> polytop, fID, nPoints, random);");
    Console.WriteLine($"ShiftAndRotate({PDim}, ref polytop, ref S, random);");
    Console.WriteLine("S.Shuffle(random);");

    Console.WriteLine();
    Console.WriteLine("ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);");
    Console.WriteLine($"Assert.That(P.Vrep.SetEquals(polytop), \"The set of vertices must be equal.\");");
    Console.WriteLine
      (
       $"Assert.That(P.Hrep, Has.Count.EqualTo({numberHRep}), $\"The number of facets of the cube must be equal to {numberHRep}.\");"
      );
    Console.WriteLine
      (
       $"Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo({numberFVec}), $\"The number of faces of the cube must be equal to {numberFVec}.\");"
      );
    Console.WriteLine("}");
    Console.WriteLine();
  }

}
