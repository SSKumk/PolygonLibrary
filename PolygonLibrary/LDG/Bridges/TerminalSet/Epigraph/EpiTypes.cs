namespace LDG;

/// <summary>
/// Represents different types of epigraphs and provides mechanisms to read their parameters according to the documentation.
/// </summary>
public static class EpiTypes<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents an epigraph type that is the distance to the given point defined in specific norm.
  /// </summary>
  public class DistToPointByUnitBall : EpiTypeBase<TNum, TConv> {

    /// <summary>
    /// Gets the point to which the distance is calculated.
    /// </summary>
    private Geometry<TNum, TConv>.Vector Point { get; }

    /// <summary>
    /// The (d-dim)-polytope which is used to determine the norm in the (d-dim) space from which the distance to the point is calculated.
    /// </summary>
    private Geometry<TNum, TConv>.ConvexPolytop UnitBall { get; }

    /// <summary>
    /// Reads the parameters required to configure the epigraph type for distance-to-point calculations.
    /// </summary>
    /// <param name="pr">The parameter reader used to extract data from the terminal set configuration file.</param>
    /// <param name="ph">Provides access to files describing polytopes. Required only for distance-to-polytope calculations.</param>
    public DistToPointByUnitBall(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) : base(pr) {
      Point         = pr.ReadVector("Point");
      UnitBall      = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("UnitBall"), ph);
    }

    public override Geometry<TNum, TConv>.ConvexPolytop BuildEpigraph() {
      return Geometry<TNum, TConv>.ConvexPolytop.BuildDistanceEpigraph(UnitBall, Point, ScaleFactor);
    }

  }

  /// <summary>
  /// Represents an epigraph for the distance to a point, measured using a standard norm (1, 2 or infinity).
  /// </summary>
  public class DistToPointByNorm : EpiTypeBase<TNum, TConv> {

    /// <summary>
    /// Gets the point to which the distance is calculated.
    /// </summary>
    private Geometry<TNum, TConv>.Vector Point { get; }

    private IBall<TNum, TConv> BallType { get; }

    /// <summary>
    /// Reads the parameters required to configure the epigraph type for distance-to-point calculations.
    /// </summary>
    /// <param name="pr">The parameter reader used to extract data from the terminal set configuration file.</param>
    public DistToPointByNorm(Geometry<TNum, TConv>.ParamReader pr) : base(pr) {
      Point    = pr.ReadVector("Point");
      BallType = BallFactory<TNum, TConv>.Read(pr);
    }

    public override Geometry<TNum, TConv>.ConvexPolytop BuildEpigraph() {
      return BallType switch
               {
                 Ball_1<TNum, TConv> => Geometry<TNum, TConv>.ConvexPolytop.BuildDistanceEpigraph_L1(Point, ScaleFactor)
               , Ball_2<TNum, TConv> b2 => Geometry<TNum, TConv>.ConvexPolytop.BuildDistanceEpigraph_L2
                   (Point, b2.AzimuthsDivisions, b2.PolarDivision, ScaleFactor)
               , Ball_oo<TNum, TConv> => Geometry<TNum, TConv>.ConvexPolytop.BuildDistanceEpigraph_Linf(Point, ScaleFactor)
               , _                    => throw new ArgumentOutOfRangeException()
               };
    }

  }

  /// <summary>
  /// Represents an epigraph for the distance to a convex polytope, measured using a standard norm (1, 2 or infinity).
  /// </summary>
  public class DistToPolytope : EpiTypeBase<TNum, TConv> {

    /// <summary>
    /// Gets the convex polytope used for constructing terminal sets.
    /// </summary>
    private Geometry<TNum, TConv>.ConvexPolytop Polytope { get; }

    private IBall<TNum, TConv> BallType { get; }

    /// <summary>
    /// Reads the parameters required to configure the epigraph type for distance-to-polytope calculations.
    /// </summary>
    /// <param name="pr">The parameter reader used to extract data from the terminal set configuration file.</param>
    /// <param name="ph">Provides access to files describing polytopes.</param>
    public DistToPolytope(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) : base(pr) {
      Polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Polytope"), ph);
      BallType = BallFactory<TNum, TConv>.Read(pr);
    }

    public override Geometry<TNum, TConv>.ConvexPolytop BuildEpigraph() {
      return BallType switch
               {
                 Ball_1<TNum, TConv> => Geometry<TNum, TConv>.ConvexPolytop.BuildDistanceEpigraph_ToPolytope_L1(Polytope, ScaleFactor)
               , Ball_2<TNum, TConv> b2 => Geometry<TNum, TConv>.ConvexPolytop.BuildDistanceEpigraph_ToPolytope_L2
                   (Polytope, ScaleFactor, b2.AzimuthsDivisions, b2.PolarDivision)
               , Ball_oo<TNum, TConv> => Geometry<TNum, TConv>.ConvexPolytop.BuildDistanceEpigraph_ToPolytope_Linf(Polytope, ScaleFactor)
               , _                    => throw new ArgumentOutOfRangeException()
               };
    }

  }

}
