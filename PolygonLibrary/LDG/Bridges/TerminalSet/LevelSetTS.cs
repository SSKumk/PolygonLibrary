namespace LDG;

public class LevelSetTS<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop>
    ReadTerminalSets(Geometry<TNum, TConv>.ParamReader pr
                   , LDGPathHolder<TNum, TConv>        ph) {

    ILvlSetType<TNum, TConv> lvlSetType = LvlSetFactory<TNum, TConv>.Read(pr, ph);

    TNum[] ks = pr.ReadVector("Constants").GetAsArray();
    if (ks.Any(Geometry<TNum, TConv>.Tools.LE)) {
      throw new ArgumentException($"Bridges...ReadTerminalSets.LevelSetTS: " +
                                  $"All 'Constants' must be greater than zero.");
    }

    IBall<TNum, TConv> ballType = BallFactory<TNum, TConv>.Read(pr);

    foreach (TNum k in ks) {
      switch (lvlSetType) {
        case LvlSetTypes<TNum, TConv>.DistToPoint d: {
          yield return
            ballType switch
              {
                Ball_1<TNum, TConv>    => Geometry<TNum, TConv>.ConvexPolytop.Ball_1(d.Point, k)
              , Ball_2<TNum, TConv> b2 => Geometry<TNum, TConv>.ConvexPolytop.Sphere(d.Point, k, b2.PolarDivision, b2.AzimuthsDivisions)
              , Ball_oo<TNum, TConv>   => Geometry<TNum, TConv>.ConvexPolytop.Ball_oo(d.Point, k)
              };

          break;
        }

        case LvlSetTypes<TNum, TConv>.DistToPolytope d: {
          Console.WriteLine($"LevelSetTS.DistToPolytope: Some naive approach here. Perhaps there are exist a better way!");
          yield return
            ballType switch
              {
                Ball_1<TNum, TConv> => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k, Geometry<TNum, TConv>.ConvexPolytop.Ball_1)
              , Ball_2<TNum, TConv> b2 => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k
                                                                                                     , Geometry<TNum, TConv>.ConvexPolytop
                                                                                                        .Ball_2FuncCreator(b2.PolarDivision, b2.AzimuthsDivisions))
              , Ball_oo<TNum, TConv> => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k, Geometry<TNum, TConv>.ConvexPolytop.Ball_oo)
              };

          break;
        }
      }

    }


  }
}
