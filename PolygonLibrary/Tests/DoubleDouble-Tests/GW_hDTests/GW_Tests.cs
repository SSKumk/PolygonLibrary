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
    const uint seed    = 0;
    const int  PDim    = 4;
    const int  nPoints = 2000;
    List<int>  fID     = new List<int>() { };
    GRandomLC  random  = new GRandomLC(0);

    List<Vector> S = Simplex(PDim, out List<Vector> polytop, fID, nPoints, random);
    ShiftAndRotate(4, ref polytop, ref S, random);
    S.Shuffle(random);

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(polytop), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(5), $"The number of facets of the cube must be equal to 5.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(31), $"The number of faces of the cube must be equal to 31.");
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
  public void Cube3D_Rotated_Shifted() {
    SortedSet<Vector> S = new SortedSet<Vector>()
      {
        new Vector(new ddouble[] { 4.989650328990457, 18.100255093909855, 14.491501515962065 })
      , new Vector(new ddouble[] { 5.66967171757855, 18.495941030863165, 15.108756366376465 })
      , new Vector(new ddouble[] { 4.518403603114471, 17.69118126989747, 15.272900984773663 })
      , new Vector(new ddouble[] { 5.198424991702564, 18.08686720685078, 15.890155835188063 })
      , new Vector(new ddouble[] { 4.427958745989709, 18.92250277302197, 14.58321476353759 })
      , new Vector(new ddouble[] { 5.107980134577802, 19.318188709975285, 15.20046961395199 })
      , new Vector(new ddouble[] { 3.9567120201137236, 18.513428949009587, 15.364614232349188 })
      , new Vector(new ddouble[] { 4.636733408701816, 18.909114885962897, 15.981869082763588 })
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
    for (int i = 0; i < 10 * S.Count; i++) {
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
         , 6
         , 63
          );
      }
    }
  }
#endregion


#region Other tests
  /// <summary>
  /// Вершины:
  ///[0] +2.573  A
  ///[1] -0.433     X
  ///[2] -3.942  B
  ///[3] -2.231  C
  ///[4] +0.457  D
  ///
  ///  Доп.точки:
  ///[5] -0.715  0,1,2
  ///[6] -1.250  0,2,3,4
  ///[7] +2.524  0,1,2,3,4  F
  ///
  ///0-2-3-4-6-7
  ///
  /// Точка F очень близка к вершине A так, что с точностью 1e-8 принадлежит гиперплоскостям всех гиперграней, проходящих через эту вершину.
  /// При загрублении точности до такого значения получается не гипер-тетраэдр.
  /// </summary>
  [Test]
  public void Simplex4D_PointCloseToVertex() {
    List<Vector> S = new List<Vector>()
      {
        new Vector(new ddouble[] { 2.573083673504434, 4.459384730891181, -0.27379963436950927, -3.9775508290570114 })
      , new Vector(new ddouble[] { -0.4334526451848103, -0.4053162935667942, 3.2553497814236554, 3.4524045601609177 })
      , new Vector(new ddouble[] { -3.942094170242104, -1.9384525033967692, -0.29372328782773627, 2.603184338100996 })
      , new Vector(new ddouble[] { -2.231889343176011, -3.249343109375179, -0.4791314609998676, -3.9361931497548226 })
      , new Vector(new ddouble[] { 0.4576718028303406, -1.483829232511071, 1.5060715392478907, -3.912975119639415 })
      , new Vector(new ddouble[] { -0.715863786767021, 1.1248964198021802, -0.08999099950936182, -0.41480693316225237 })
      , new Vector(new ddouble[] { -1.2504502355127234, -2.0382520989901907, 0.055536783449397165, -3.4765790886104364 })
      , new Vector(new ddouble[] { 2.5244380627935232, 4.399463020994712, -0.2616425805524087, -3.92127411448588 })
      };

    var hpABCD = new HyperPlane
      (
       new List<Vector>()
         {
           S[0]
         , S[2]
         , S[3]
         , S[4]
         }
      );
    var distABCD = S.Select(s => hpABCD.Eval(s));

    AffineBasis ABDbasis = new AffineBasis(new List<Vector>() { S[2], S[0], S[4] });

    Vector BC = LinearBasis.OrthonormalizeAgainstBasis(S[3] - S[2], ABDbasis.LinBasis);
    Vector BX = LinearBasis.OrthonormalizeAgainstBasis(S[1] - S[2], ABDbasis.LinBasis);
    Vector BF = LinearBasis.OrthonormalizeAgainstBasis(S[7] - S[2], ABDbasis.LinBasis);

    ddouble angleCBX = ddouble.Acos(BC * BX);
    ddouble angleCBF = ddouble.Acos(BC * BF);


    var hpABDX = new HyperPlane
      (
       new AffineBasis
         (
          new List<Vector>()
            {
              S[0]
            , S[2]
            , S[1]
            , S[4]
            }
         )
      );
    var distABDX = S.Select(s => hpABDX.Eval(s));

    var x = GiftWrapping.WrapVRep(S);
    var y = S.GetRange(0, 5).ToSortedSet();

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(S.GetRange(0, 5)), "The set of vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(5), "The number of facets of the 4D-simplex must be equal to 5.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(31), "The number of faces of the  4D-simplex must be equal to 31.");
  }

  /// <summary>
  /// Вершины:
  /// [0] +0.836      -->;  [2]
  /// [1] -2.291      -->;  [7]
  /// [2] +2.140      -->;  [5]
  /// [3] -1.128      -->;  [6]
  /// [4] +3.068      -->;  [8]
  ///
  /// Доп. точки:               номера точек на ребре между которыми лежат данные точки
  /// [5] +2.175 0,4  -->;  [1] 2,8
  /// [6] -1.531 1,4  -->;  [4] 7,8
  /// [7] +0.908 0,2  -->;  [3] 2,5
  /// [8] -2.290 1,4  -->;  [0] 7,8
  /// </summary>
  [Test]
  public void Simplex4D_InnerPointsIn_1D() {
    List<Vector> Simplex = new List<Vector>()
      {
        new Vector(new ddouble[] { 0.8364793532147252, 3.1538275299020646, -2.8732700734104193, 2.4909120326607748 })
      , new Vector(new ddouble[] { -2.2910587157334805, -2.149176399025409, 4.5139871187307845, -3.2342020813921 })
      , new Vector(new ddouble[] { 2.140466204644289, 1.8671979608170686, 0.043747361061103884, 0.9348952481371575 })
      , new Vector(new ddouble[] { -1.128714852065014, -1.7299541148194004, 0.4864426528770571, -1.6846663706667409 })
      , new Vector(new ddouble[] { 3.0687608076419592, 1.4408928939236543, 4.602441817895146, 1.823890199145276 })
      };

    List<Vector> S = new List<Vector>(Simplex)
      {
        new Vector(new ddouble[] { 2.175818488745113, 2.126089567011522, 1.6120574743850615, 2.0907078182196503 })
      , new Vector(new ddouble[] { -1.5310856984393355, -1.6401376560306955, 4.526529179955908, -2.517011206694256 })
      , new Vector(new ddouble[] { 0.9089342229083861, 3.08233710216511, -2.7111885939253577, 2.4044533438785916 })
      , new Vector(new ddouble[] { -2.290970227496747, -2.149117128577943, 4.51398857907853, -3.2341185745379626 })
      };

    List<Vector> S_shuffled = new List<Vector>()
      {
        new Vector(new ddouble[] { 2.175818488745113, 2.126089567011522, 1.6120574743850615, 2.0907078182196503 })
      , new Vector(new ddouble[] { -1.128714852065014, -1.7299541148194004, 0.4864426528770571, -1.6846663706667409 })
      , new Vector(new ddouble[] { 0.8364793532147252, 3.1538275299020646, -2.8732700734104193, 2.4909120326607748 })
      , new Vector(new ddouble[] { 2.140466204644289, 1.8671979608170686, 0.043747361061103884, 0.9348952481371575 })
      , new Vector(new ddouble[] { -2.2910587157334805, -2.149176399025409, 4.5139871187307845, -3.2342020813921 })
      , new Vector(new ddouble[] { -2.290970227496747, -2.149117128577943, 4.51398857907853, -3.2341185745379626 })
      , new Vector(new ddouble[] { 3.0687608076419592, 1.4408928939236543, 4.602441817895146, 1.823890199145276 })
      , new Vector(new ddouble[] { -1.5310856984393355, -1.6401376560306955, 4.526529179955908, -2.517011206694256 })
      , new Vector(new ddouble[] { 0.9089342229083861, 3.08233710216511, -2.7111885939253577, 2.4044533438785916 })
      };

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(S, true);
    Assert.That(P.Vrep.SetEquals(Simplex), "The set of vertices must be equal.");
    P = ConvexPolytop.CreateFromPoints(S_shuffled, true);
    Assert.That(P.Vrep.SetEquals(Simplex), "The set of shuffled vertices must be equal.");
    Assert.That(P.Hrep, Has.Count.EqualTo(5), "The number of facets of the 4D-simplex must be equal to 5.");
    Assert.That(P.FLrep.NumberOfKFaces, Is.EqualTo(31), "The number of faces of the  4D-simplex must be equal to 31.");
  }

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
    Console.WriteLine($"const uint seed   = {seed};");
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
