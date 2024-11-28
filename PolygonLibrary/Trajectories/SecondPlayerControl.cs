using System.Numerics;
using CGLibrary;

namespace Trajectories;

public class SecondPlayerControl<TNum, TConv> : PlayerControl<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public SecondPlayerControl(
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
    if (!gd.Q.Contains(constControl)) {
      throw new ArgumentException("The constant control should lie within polytope Q!");
    }
    AimPoint = (x + constControl).Normalize();

    return constControl;
  }

}
