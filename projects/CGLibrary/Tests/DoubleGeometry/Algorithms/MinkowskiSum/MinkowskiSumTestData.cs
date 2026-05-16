using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

internal static class MinkowskiSumTestData {

  public static readonly Vector U00 = new(new double[] { 0, 0 });
  public static readonly Vector U10 = new(new double[] { 1, 0 });
  public static readonly Vector U01 = new(new double[] { 0, 1 });
  public static readonly Vector U11 = new(new double[] { 1, 1 });
  public static readonly Vector U21 = new(new double[] { 2, 1 });
  public static readonly Vector U12 = new(new double[] { 1, 2 });
  public static readonly Vector U22 = new(new double[] { 2, 2 });
  public static readonly Vector P05_0 = new(new double[] { 0.5, 0 });
  public static readonly Vector P10_05 = new(new double[] { 1, 0.5 });
  public static readonly Vector P05_1 = new(new double[] { 0.5, 1 });
  public static readonly Vector P0_05 = new(new double[] { 0, 0.5 });

  public static ConvexPolytop Point11 => ConvexPolytop.CreateFromPoints([U11]);
  public static ConvexPolytop OrthSegment => ConvexPolytop.CreateFromPoints([U00, U10]);
  public static ConvexPolytop VerticalSegment => ConvexPolytop.CreateFromPoints([U00, U01]);
  public static ConvexPolytop DiagonalSegment => ConvexPolytop.CreateFromPoints([U00, U11]);
  public static ConvexPolytop ShiftedDiagonalSegment => ConvexPolytop.CreateFromPoints([U11, U22]);
  public static ConvexPolytop TripleShiftedDiagonalSegment => ConvexPolytop.CreateFromPoints([3 * U11, 3 * U22]);
  public static ConvexPolytop ShiftedHorizontalSegment => ConvexPolytop.CreateFromPoints([U11, U21]);
  public static ConvexPolytop ShiftedVerticalSegment => ConvexPolytop.CreateFromPoints([U11, U12]);
  public static ConvexPolytop Square => ConvexPolytop.CreateFromPoints([U00, U10, U01, U11]);
  public static ConvexPolytop ShiftedSquare => ConvexPolytop.CreateFromPoints([U11, U21, U12, U22]);
  public static ConvexPolytop RotatedSquare => ConvexPolytop.CreateFromPoints([P05_0, P10_05, P05_1, P0_05]);

}
