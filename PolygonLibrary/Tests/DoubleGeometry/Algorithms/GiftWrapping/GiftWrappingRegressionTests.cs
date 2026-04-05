using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsBase<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsPolytopes<double, Tests.DConvertor>;
using Tests.DoubleGeometry.Polyhedra;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class GiftWrappingRegressionTests {

  private static readonly SortedSet<Vector> Cube3DRotated =
    [
      new(new double[] { 0, 0, 0 }),
      new(new double[] { 0.6800213885880926, 0.3956859369533106, 0.6172548504143999 }),
      new(new double[] { -0.47124672587598565, -0.40907382401238557, 0.7813994688115978 }),
      new(new double[] { 0.20877466271210693, -0.013387887059074954, 1.3986543192259977 }),
      new(new double[] { -0.561691583000748, 0.822247679112118, 0.0917132475755244 }),
      new(new double[] { 0.11832980558734463, 1.2179336160654286, 0.7089680979899242 }),
      new(new double[] { -1.0329383088767337, 0.4131738550997325, 0.8731127163871222 }),
      new(new double[] { -0.3529169202886411, 0.8088597920530431, 1.4903675668015222 })
    ];

  private static readonly SortedSet<Vector> Cube3DShifted =
    [
      new(new double[] { -10.029417029821644, -8.414457472370579, 12.142282885765258 }),
      new(new double[] { -10.029417029821644, -8.414457472370579, 13.142282885765258 }),
      new(new double[] { -10.029417029821644, -7.414457472370579, 12.142282885765258 }),
      new(new double[] { -10.029417029821644, -7.414457472370579, 13.142282885765258 }),
      new(new double[] { -9.029417029821644, -8.414457472370579, 12.142282885765258 }),
      new(new double[] { -9.029417029821644, -8.414457472370579, 13.142282885765258 }),
      new(new double[] { -9.029417029821644, -7.414457472370579, 12.142282885765258 }),
      new(new double[] { -9.029417029821644, -7.414457472370579, 13.142282885765258 })
    ];

  private static readonly SortedSet<Vector> Cube3DRotatedShifted =
    [
      new(new double[] { 4.989650328990457, 18.100255093909855, 14.491501515962065 }),
      new(new double[] { 5.66967171757855, 18.495941030863165, 15.108756366376465 }),
      new(new double[] { 4.518403603114471, 17.69118126989747, 15.272900984773663 }),
      new(new double[] { 5.198424991702564, 18.08686720685078, 15.890155835188063 }),
      new(new double[] { 4.427958745989709, 18.92250277302197, 14.58321476353759 }),
      new(new double[] { 5.107980134577802, 19.318188709975285, 15.20046961395199 }),
      new(new double[] { 3.9567120201137236, 18.513428949009587, 15.364614232349188 }),
      new(new double[] { 4.636733408701816, 18.909114885962897, 15.981869082763588 })
    ];

  [OneTimeSetUp]
  public void SetUp() => Tools.Eps = 1e-8;

  private static void AssertWrappedVerticesEqual(IEnumerable<Vector> swarm, IEnumerable<Vector> expected, string message) {
    ConvexPolytop polytop = ConvexPolytop.CreateFromPoints(swarm.ToList(), true);

    ConvexPolytopAssert.AssertVertexSetEquals(polytop.Vrep, expected, message);
  }

  private static void AssertShuffleInvariant(List<Vector> source, string testName) {
    CGLibrary.RandomLC random = new(0u);
    SortedSet<Vector> expected = new(source);

    for (int i = 0; i < 10 * source.Count; i++) {
      uint seed = random.Seed;
      List<Vector> shuffled = source.ToList();
      shuffled.Shuffle(random);

      AssertWrappedVerticesEqual(
        shuffled,
        expected,
        $"{testName}: The set of vertices must be equal.\nSeed: {seed}"
      );
    }
  }

  [Test]
  public void CreateFromPoints_ForCube3D_PreservesVertexSet() {
    List<Vector> swarm = Cube01(3, out List<Vector> cube);

    AssertWrappedVerticesEqual(swarm, cube, "Cube3D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DRotatedZ45_PreservesVertexSet() {
    List<Vector> swarm = Cube01(3, out List<Vector> _);
    List<Vector> rotated = Rotate(swarm, rotate3D_45XY);

    AssertWrappedVerticesEqual(rotated, rotated, "Cube3D_Rotated_Z45: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DRotated_PreservesVertexSet() {
    AssertWrappedVerticesEqual(Cube3DRotated, Cube3DRotated, "Cube3D_Rotated: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DShifted_PreservesVertexSet() {
    AssertWrappedVerticesEqual(Cube3DShifted, Cube3DShifted, "Cube3D_Shifted: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DRotatedShifted_PreservesVertexSet() {
    AssertWrappedVerticesEqual(Cube3DRotatedShifted, Cube3DRotatedShifted, "Cube3D_Rotated_Shifted: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DWithInnerPointsOn1D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(3, out List<Vector> cube, [1], 1, new GRandomLC(131));

    AssertWrappedVerticesEqual(swarm, cube, "Cube3D_withInnerPoints_On_1D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DWithInnerPointsOn2D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(3, out List<Vector> cube, [2], 1, new GRandomLC(132));

    AssertWrappedVerticesEqual(swarm, cube, "Cube3D_withInnerPoints_On_2D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DWithInnerPointsOn3D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(3, out List<Vector> cube, [3], 1, new GRandomLC(133));

    AssertWrappedVerticesEqual(swarm, cube, "Cube3D_withInnerPoints_On_3D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DWithInnerPointsOn1DAnd2D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(3, out List<Vector> cube, [1, 2], 1, new GRandomLC(1312));

    AssertWrappedVerticesEqual(swarm, cube, "Cube3D_withInnerPoints_On_1D_2D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DWithInnerPointsOn2DAnd3D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(3, out List<Vector> cube, [2, 3], 1, new GRandomLC(1323));

    AssertWrappedVerticesEqual(swarm, cube, "Cube3D_withInnerPoints_On_2D_3D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3DWithInnerPointsOn1D2DAnd3D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(3, out List<Vector> cube, [1, 2, 3], 1, new GRandomLC(13123));

    AssertWrappedVerticesEqual(swarm, cube, "Cube3D_withInnerPoints_On_1D_2D_3D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube3D_IsShuffleInvariant() {
    List<Vector> swarm = Cube01(3, out List<Vector> _);

    AssertShuffleInvariant(swarm, "Cube3D_Shuffled");
  }

  [Test]
  public void CreateFromPoints_ForCube4D_IsShuffleInvariant() {
    List<Vector> swarm = Cube01(4, out List<Vector> _);

    AssertShuffleInvariant(swarm, "Cube4D_Shuffled");
  }

  [Test]
  public void CreateFromPoints_ForSimplex3D_IsShuffleInvariant() {
    List<Vector> swarm = Simplex(3, out List<Vector> _);

    AssertShuffleInvariant(swarm, "Simplex3D_Shuffled");
  }

  [Test]
  public void CreateFromPoints_ForSimplex4D_IsShuffleInvariant() {
    List<Vector> swarm = Simplex(4, out List<Vector> _);

    AssertShuffleInvariant(swarm, "Simplex4D_Shuffled");
  }

  [Test]
  public void CreateFromPoints_ForCube4DWithInnerPointsOn1D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(4, out List<Vector> cube, [1], 1, new GRandomLC(141));

    AssertWrappedVerticesEqual(swarm, cube, "Cube4D_withInnerPoints_On_1D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube4DWithInnerPointsOn2D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(4, out List<Vector> cube, [2], 1, new GRandomLC(142));

    AssertWrappedVerticesEqual(swarm, cube, "Cube4D_withInnerPoints_On_2D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube4DWithInnerPointsOn3D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(4, out List<Vector> cube, [3], 1, new GRandomLC(143));

    AssertWrappedVerticesEqual(swarm, cube, "Cube4D_withInnerPoints_On_3D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube4DWithInnerPointsOn1DAnd2D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(4, out List<Vector> cube, [1, 2], 1, new GRandomLC(1412));

    AssertWrappedVerticesEqual(swarm, cube, "Cube4D_withInnerPoints_On_1D_2D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube4DWithInnerPointsOn2DAnd3D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(4, out List<Vector> cube, [2, 3], 1, new GRandomLC(1423));

    AssertWrappedVerticesEqual(swarm, cube, "Cube4D_withInnerPoints_On_2D_3D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube4DWithInnerPointsOn1D2DAnd3D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(4, out List<Vector> cube, [1, 2, 3], 1, new GRandomLC(14123));

    AssertWrappedVerticesEqual(swarm, cube, "Cube4D_withInnerPoints_On_1D_2D_3D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForCube4DWithInnerPointsOn1D2D3DAnd4D_PreservesHullVertices() {
    List<Vector> swarm = Cube01(4, out List<Vector> cube, [1, 2, 3, 4], 1, new GRandomLC(141234));

    AssertWrappedVerticesEqual(swarm, cube, "Cube4D_withInnerPoints_On_1D_2D_3D_4D: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForSimplex4DWithEdgeAndNeighborPoints_PreservesVertexSet() {
    Vector p0 = new(new double[] { 0, 0, 0, 0 });
    Vector p1 = new(new double[] { 1, 0, 0, 0 });
    Vector p2 = new(new double[] { 0, 1, 0, 0 });
    Vector p3 = new(new double[] { 0.1, 0, 1, 0 });
    Vector p4 = new(new double[] { 0.1, 0, 0, 1 });

    List<Vector> simplex =
      [
        p0,
        p1,
        p2,
        p3,
        p4
      ];

    List<Vector> swarm = new(simplex)
      {
        Vector.LinearCombination(p1, 0.3, p2, 0.2),
        Vector.LinearCombination(p1, 0.4, p2, 0.1),
        Vector.LinearCombination(p1, 0.4, p3, 0.1),
        Vector.LinearCombination(p1, 0.4, p3, 0.1),
        Vector.LinearCombination(p1, 0.4, p4, 0.1),
        Vector.LinearCombination(p1, 0.4, p4, 0.1)
      };

    AssertWrappedVerticesEqual(swarm, simplex, "Simplex4D_1DEdge_2DNeighborsPointsTest: The set of vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForSimplex4DWithInnerPointsOn1D_PreservesVertexSetEvenAfterShuffle() {
    List<Vector> simplex =
      [
        new(new double[] { 0.8364793532147252, 3.1538275299020646, -2.8732700734104193, 2.4909120326607748 }),
        new(new double[] { -2.2910587157334805, -2.149176399025409, 4.5139871187307845, -3.2342020813921 }),
        new(new double[] { 2.140466204644289, 1.8671979608170686, 0.043747361061103884, 0.9348952481371575 }),
        new(new double[] { -1.128714852065014, -1.7299541148194004, 0.4864426528770571, -1.6846663706667409 }),
        new(new double[] { 3.0687608076419592, 1.4408928939236543, 4.602441817895146, 1.823890199145276 })
      ];

    List<Vector> swarm =
      [
        ..simplex,
        new(new double[] { 2.175818488745113, 2.126089567011522, 1.6120574743850615, 2.0907078182196503 }),
        new(new double[] { -1.5310856984393355, -1.6401376560306955, 4.526529179955908, -2.517011206694256 }),
        new(new double[] { 0.9089342229083861, 3.08233710216511, -2.7111885939253577, 2.4044533438785916 }),
        new(new double[] { -2.290970227496747, -2.149117128577943, 4.51398857907853, -3.2341185745379626 })
      ];

    List<Vector> shuffled =
      [
        new(new double[] { 2.175818488745113, 2.126089567011522, 1.6120574743850615, 2.0907078182196503 }),
        new(new double[] { -1.128714852065014, -1.7299541148194004, 0.4864426528770571, -1.6846663706667409 }),
        new(new double[] { 0.8364793532147252, 3.1538275299020646, -2.8732700734104193, 2.4909120326607748 }),
        new(new double[] { 2.140466204644289, 1.8671979608170686, 0.043747361061103884, 0.9348952481371575 }),
        new(new double[] { -2.2910587157334805, -2.149176399025409, 4.5139871187307845, -3.2342020813921 }),
        new(new double[] { -2.290970227496747, -2.149117128577943, 4.51398857907853, -3.2341185745379626 }),
        new(new double[] { 3.0687608076419592, 1.4408928939236543, 4.602441817895146, 1.823890199145276 }),
        new(new double[] { -1.5310856984393355, -1.6401376560306955, 4.526529179955908, -2.517011206694256 }),
        new(new double[] { 0.9089342229083861, 3.08233710216511, -2.7111885939253577, 2.4044533438785916 })
      ];

    AssertWrappedVerticesEqual(swarm, simplex, "Simplex4D_InnerPointsIn_1D: The set of vertices must be equal.");
    AssertWrappedVerticesEqual(shuffled, simplex, "Simplex4D_InnerPointsIn_1D: The set of shuffled vertices must be equal.");
  }

  [Test]
  public void CreateFromPoints_ForSkewParallelepiped3D_IsShuffleInvariant() {
    Vector origin = new(3);
    Vector v1 = new(new double[] { 0.5, 1, 1 });
    Vector v2 = new(new double[] { 1, 0.5, 1 });
    Vector v3 = new(new double[] { 1, 1, 0.5 });

    List<Vector> swarm =
      [
        origin,
        origin + v1,
        origin + v2,
        origin + v3,
        origin + v1 + v2,
        origin + v1 + v3,
        origin + v2 + v3,
        origin + v1 + v2 + v3
      ];

    AssertShuffleInvariant(swarm, "SomeParallelogram");
  }

}
