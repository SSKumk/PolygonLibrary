using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

internal static class GiftWrappingTestData {

  public static readonly List<Vector> UnitSquareWithInnerPoints =
    [
      new(new double[] { 0, 0 }),
      new(new double[] { 1, 0 }),
      new(new double[] { 1, 1 }),
      new(new double[] { 0, 1 }),
      new(new double[] { 0.5, 0.5 }),
      new(new double[] { 0.25, 0.5 })
    ];

  public static readonly List<Vector> Cube3DWithInnerPoints =
    [
      new(new double[] { 0, 0, 0 }),
      new(new double[] { 1, 0, 0 }),
      new(new double[] { 0, 1, 0 }),
      new(new double[] { 0, 0, 1 }),
      new(new double[] { 1, 1, 0 }),
      new(new double[] { 0, 1, 1 }),
      new(new double[] { 1, 0, 1 }),
      new(new double[] { 1, 1, 1 }),
      new(new double[] { 0.5, 0.5, 0.5 }),
      new(new double[] { 0.5, 0.5, 0.0 }),
      new(new double[] { 0.5, 0.0, 0.5 })
    ];

  public static readonly SortedSet<Vector> Cube3DVertices =
    [
      new(new double[] { 0, 0, 0 }),
      new(new double[] { 1, 0, 0 }),
      new(new double[] { 0, 1, 0 }),
      new(new double[] { 0, 0, 1 }),
      new(new double[] { 1, 1, 0 }),
      new(new double[] { 0, 1, 1 }),
      new(new double[] { 1, 0, 1 }),
      new(new double[] { 1, 1, 1 })
    ];

  public static readonly List<Vector> Tetrahedron3D =
    [
      new(new double[] { 0, 0, 0 }),
      new(new double[] { 1, 0, 0 }),
      new(new double[] { 0, 1, 0 }),
      new(new double[] { 0, 0, 1 })
    ];

  public static readonly List<Vector> LineIn2DWithInnerPoints =
    [
      new(new double[] { -1, 2 }),
      new(new double[] { 0, 2 }),
      new(new double[] { 3, 2 }),
      new(new double[] { 1, 2 })
    ];

}
