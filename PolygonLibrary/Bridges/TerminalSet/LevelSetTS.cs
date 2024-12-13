using Bridges.LevelSet;
namespace Bridges;

public class LevelSetTS<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop>
    ReadTerminalSets(Geometry<TNum, TConv>.ParamReader pr
                   , LDGPathHolder<TNum, TConv>        dh
                   , Geometry<TNum, TConv>.GameData    gd) {

    ILvlSetType<TNum, TConv> lvlSetType = LvlSetFactory<TNum, TConv>.Read(pr, dh);

    TNum[] ks = pr.ReadVector("Constants").GetAsArray();
    if (ks.Any(Geometry<TNum, TConv>.Tools.LE)) {
      throw new ArgumentException($"Bridges...ReadTerminalSets.LevelSetTS: " +
                                  $"All 'Constants' must be greater than zero.");
    }

    Geometry<TNum, TConv>.IBall ballType = Geometry<TNum, TConv>.BallFactory.Read(pr);

    foreach (TNum k in ks) {
      switch (lvlSetType) {
        case LvlSetTypes<TNum, TConv>.DistToPoint d: {
          yield return
            ballType switch
              {
                Geometry<TNum, TConv>.Ball_1    => Geometry<TNum, TConv>.ConvexPolytop.Ball_1(d.Point, k)
              , Geometry<TNum, TConv>.Ball_2 b2 => Geometry<TNum, TConv>.ConvexPolytop.Sphere(d.Point, k, b2.PolarDivision, b2.AzimuthsDivisions)
              , Geometry<TNum, TConv>.Ball_oo   => Geometry<TNum, TConv>.ConvexPolytop.Ball_oo(d.Point, k)
              };

          break;
        }

        case LvlSetTypes<TNum, TConv>.DistToPolytope d: {
          Console.WriteLine($"LevelSetTS.DistToPolytope: Some naive approach here. Perhaps there are exist a better way!");
          yield return
            ballType switch
              {
                Geometry<TNum, TConv>.Ball_1 => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k, Geometry<TNum, TConv>.ConvexPolytop.Ball_1)
              , Geometry<TNum, TConv>.Ball_2 b2 => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k
                                                                                                     , Geometry<TNum, TConv>.ConvexPolytop
                                                                                                        .Ball_2FuncCreator(b2.PolarDivision, b2.AzimuthsDivisions))
              , Geometry<TNum, TConv>.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistTo_MakeBase(d.Polytope, k, Geometry<TNum, TConv>.ConvexPolytop.Ball_oo)
              };

          break;
        }
      }

    }


  }
}
