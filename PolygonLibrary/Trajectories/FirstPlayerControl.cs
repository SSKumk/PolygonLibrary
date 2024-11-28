using System.Numerics;
using CGLibrary;

namespace Trajectories;

public class FirstPlayerControl<TNum, TConv> : PlayerControl<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public FirstPlayerControl(
      Geometry<TNum, TConv>.Vector        x
    , Geometry<TNum, TConv>.Matrix        Dt
    , Geometry<TNum, TConv>.Matrix        Et
    , Geometry<TNum, TConv>.ConvexPolytop Wt
    , Geometry<TNum, TConv>.GameData      gameData
    , TrajMain<TNum, TConv>.ControlType   controlType
    ) : base
    (
     x
   , Dt
   , Et
   , Wt
   , gameData
   , controlType
    ) { }

  public override Geometry<TNum, TConv>.Vector Optimal() {
    Geometry<TNum, TConv>.Vector fpControl = Geometry<TNum, TConv>.Vector.Zero(1);

    AimPoint = Wt.NearestPoint(x, out bool isInside); // Нашли ближайшую точку на сечении моста
    if (isInside) {
      AimPoint = Wt.Vrep.Aggregate((acc, v) => acc + v) / TConv.FromInt(Wt.Vrep.Count);
    }

    Geometry<TNum, TConv>.Vector l       = AimPoint - x;
    TNum                         extrVal = Geometry<TNum, TConv>.Tools.NegativeInfinity;
    foreach (Geometry<TNum, TConv>.Vector pVert in gd.P.Vrep) {
      TNum val = gd.dt * Dt * pVert * l;
      if (val > extrVal) {
        extrVal   = val;
        fpControl = pVert;
      }
    }

    return fpControl;
  }

  public override Geometry<TNum, TConv>.Vector Constant() {
    if (!gd.P.Contains(constControl)) {
      throw new ArgumentException("The constant control should lie within polytope P!");
    }
    AimPoint = (x + constControl).Normalize();

    return constControl;
  }

}
