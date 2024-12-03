using System.Numerics;
using CGLibrary;

namespace Trajectories;

public class FirstPlayerControl<TNum, TConv> : PlayerControl<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public FirstPlayerControl(Geometry<TNum, TConv>.ParamReader pr, PlayerControl<TNum, TConv> game) : base
    (game.D, game.E, game.W, game.gd) {
    controlType = ReadControlType(pr, 'P');
    ReadControl(pr, 'P');

    if (controlType == ControlType.Constant) {
      if (!gd.P.Contains(constantControl)) {
        throw new ArgumentException("The constant control should lie within polytope P!");
      }
    }
  }

  public override (Geometry<TNum, TConv>.Vector Control, Geometry<TNum, TConv>.Vector AimPoint) Optimal(
      TNum                         t
    , Geometry<TNum, TConv>.Vector x
    ) {
    Geometry<TNum, TConv>.Vector fpControl = Geometry<TNum, TConv>.Vector.Zero(1);

    Geometry<TNum, TConv>.Vector aimPoint = W[t].NearestPoint(x, out bool isInside); // Нашли ближайшую точку на сечении моста
    if (isInside) {
      aimPoint = W[t].Vrep.Aggregate((acc, v) => acc + v) / TConv.FromInt(W[t].Vrep.Count);
    }

    Geometry<TNum, TConv>.Vector l       = aimPoint - x;
    TNum                         extrVal = Geometry<TNum, TConv>.Tools.NegativeInfinity;
    foreach (Geometry<TNum, TConv>.Vector pVert in gd.P.Vrep) {
      TNum val = gd.dt * D[t] * pVert * l;
      if (val > extrVal) {
        extrVal   = val;
        fpControl = pVert;
      }
    }

    return (fpControl, aimPoint);
  }

  public override (Geometry<TNum, TConv>.Vector Control, Geometry<TNum, TConv>.Vector AimPoint) Constant(
      TNum                         t
    , Geometry<TNum, TConv>.Vector proj_x
    )
    => (constantControl, (proj_x + gd.Xstar(t) * constantControl).Normalize());

}
