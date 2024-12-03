using System.Numerics;
using CGLibrary;

namespace Trajectories;

public class SecondPlayerControl<TNum, TConv> : PlayerControl<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public SecondPlayerControl(Geometry<TNum, TConv>.ParamReader pr, PlayerControl<TNum, TConv> game) : base
    (game.D, game.E, game.W, game.gd) {
    controlType = ReadControlType(pr, 'Q');
    ReadControl(pr, 'Q');

    if (controlType == ControlType.Constant) {
      gd.Q = gd.Q.GetInFLrep(); //todo Убрать, когда по Фукуде сделается
      if (!gd.Q.Contains(constantControl)) {
        throw new ArgumentException("The constant control should lie within polytope Q!");
      }
    }
  }

  public override (Geometry<TNum, TConv>.Vector Control, Geometry<TNum, TConv>.Vector AimPoint) Optimal(
      TNum                         t
    , Geometry<TNum, TConv>.Vector x
    ) {
    Geometry<TNum, TConv>.Vector spControl = Geometry<TNum, TConv>.Vector.Zero(1);

    Geometry<TNum, TConv>.Vector h = W[t].NearestPoint(x, out bool isInside); // Нашли ближайшую точку на сечении моста
    Geometry<TNum, TConv>.Vector aimPoint;
    if (isInside) {
      aimPoint = h;
    }
    else {
      aimPoint = (x - h).Normalize() + x;
    }
    Geometry<TNum, TConv>.Vector l       = aimPoint - x;
    TNum                         extrVal = Geometry<TNum, TConv>.Tools.NegativeInfinity;
    foreach (Geometry<TNum, TConv>.Vector qVert in gd.Q.Vrep) {
      TNum val = gd.dt * E[t] * qVert * l;
      if (val > extrVal) {
        extrVal   = val;
        spControl = qVert;
      }
    }


    return (spControl, aimPoint);
  }

  public override (Geometry<TNum, TConv>.Vector Control, Geometry<TNum, TConv>.Vector AimPoint) Constant(
      TNum                         t
    , Geometry<TNum, TConv>.Vector proj_x
    )
    => (constantControl, (proj_x + gd.Xstar(t) * constantControl).Normalize());

}
