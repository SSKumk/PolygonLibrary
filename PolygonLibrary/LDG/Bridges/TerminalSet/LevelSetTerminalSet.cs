namespace LDG;

/// <summary>
/// Represents a terminal set reader for the "LevelSet" type, which builds terminal sets based on the level set of specific functions.
/// </summary>
public class LevelSetTerminalSet<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Builds terminal sets based on a level set.
  /// </summary>
  /// <param name="pr">The parameter reader used to extract the level set parameters from the terminal set configuration file.</param>
  /// <param name="ph">The path holder used to access polytope files.</param>
  /// <returns>A collection of terminal sets based on the level set function.</returns>
  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop>
    BuildTerminalSets(Geometry<TNum, TConv>.ParamReader pr
                    , LDGPathHolder<TNum, TConv>        ph) {

    ILvlSetType<TNum, TConv> lvlSetType = LvlSetFactory<TNum, TConv>.Read(pr, ph);

    TNum[] ks = pr.ReadVector("Constants").GetCopyAsArray();
    if (ks.Any(Geometry<TNum, TConv>.Tools.LE)) {
      throw new ArgumentException($"Bridges...BuildTerminalSets.LevelSetTerminalSet: " +
                                  $"All 'Constants' must be greater than zero.");
    }
    Array.Sort(ks);

    IBall<TNum, TConv> ballType = BallFactory<TNum, TConv>.Read(pr);

    foreach (TNum k in ks) {
      switch (lvlSetType) {
        case LvlSetTypes<TNum, TConv>.DistToPoint d: {
          yield return
            ballType switch
              {
                Ball_1<TNum, TConv>    => Geometry<TNum, TConv>.ConvexPolytop.Ball_1(d.Point, k)
              , Ball_2<TNum, TConv> b2 => Geometry<TNum, TConv>.ConvexPolytop.Sphere(d.Point, k, b2.AzimuthsDivisions, b2.PolarDivision)
              , Ball_oo<TNum, TConv>   => Geometry<TNum, TConv>.ConvexPolytop.Ball_oo(d.Point, k)
              };

          break;
        }

        case LvlSetTypes<TNum, TConv>.DistToPolytope d: {
          Console.WriteLine($"LevelSetTerminalSet.DistToPolytope: Some naive approach here. Perhaps there are exist a better way!");
          yield return
            ballType switch
              {
                Ball_1<TNum, TConv> => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k, Geometry<TNum, TConv>.ConvexPolytop.Ball_1)
              , Ball_2<TNum, TConv> b2 => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k
                                                                                            , Geometry<TNum, TConv>.ConvexPolytop
                                                                                               .Ball_2FuncCreator(b2.AzimuthsDivisions, b2.PolarDivision))
              , Ball_oo<TNum, TConv> => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k, Geometry<TNum, TConv>.ConvexPolytop.Ball_oo)
              };

          break;
        }
      }
    }
  }
}
