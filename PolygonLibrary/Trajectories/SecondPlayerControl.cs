using System.Numerics;
using CGLibrary;

namespace Trajectories;

public class SecondPlayerControl<TNum, TConv> : PlayerControl<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public SecondPlayerControl(Geometry<TNum, TConv>.ParamReader pr, PlayerControl<TNum, TConv> game) : base(game.D, game.E, game.W, game.gd) {
    controlType = ReadControlType(pr, 'Q');
    ReadControl(pr, 'Q');
  }

  public override Geometry<TNum, TConv>.Vector Optimal(TNum t) {
    Geometry<TNum, TConv>.Vector spControl = Geometry<TNum, TConv>.Vector.Zero(1);

    Geometry<TNum, TConv>.Vector h = Wt.NearestPoint(x, out bool isInside); // Нашли ближайшую точку на сечении моста
    if (isInside) {
      AimPoint = h;
    }
    else {
      AimPoint = (x - h).Normalize() + x;
    }
    Geometry<TNum, TConv>.Vector l       = AimPoint - x;
    TNum                         extrVal = Geometry<TNum, TConv>.Tools.NegativeInfinity;
    foreach (Geometry<TNum, TConv>.Vector qVert in gd.Q.Vrep) {
      TNum val = gd.dt * Et * qVert * l;
      if (val > extrVal) {
        extrVal   = val;
        spControl = qVert;
      }
    }


    return spControl;
  }

  public override Geometry<TNum, TConv>.Vector Constant() {
    gd.Q = gd.Q.GetInFLrep(); //todo Убрать, когда по Фукуде сделается
    if (!gd.Q.Contains(constControl)) {
      throw new ArgumentException("The constant control should lie within polytope Q!");
    }
    AimPoint = (x + constControl).Normalize();

    return constControl;
  }

}
