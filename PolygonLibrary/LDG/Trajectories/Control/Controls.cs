namespace LDG;

public class ConstantControl<TNum, TConv> : IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly Geometry<TNum, TConv>.Vector constantControl;

  public ConstantControl(Geometry<TNum, TConv>.ParamReader pr) { constantControl = pr.ReadVector("Constant"); }

  public Geometry<TNum, TConv>.Vector Control(
      TNum                             t
    , Geometry<TNum, TConv>.Vector     x
    , out Geometry<TNum, TConv>.Vector aimPoint
    , GameData<TNum, TConv>            gd
    ) {
    aimPoint = (x + gd.Xstar(t) * constantControl).Normalize();

    return constantControl;
  }

}

public class FirstPlayerOptimalControl<TNum, TConv> : IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // маленький -- первый мост (по обратному включению) вне которого лежит 'x'. Если такого нет, то таковым объявляется самый малый по объёму
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> smallBr;

  public FirstPlayerOptimalControl(SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> W) { smallBr = W; }

  public Geometry<TNum, TConv>.Vector Control(
      TNum                             t
    , Geometry<TNum, TConv>.Vector     x
    , out Geometry<TNum, TConv>.Vector aimPoint
    , GameData<TNum, TConv>            gd
    ) {
    Geometry<TNum, TConv>.Vector fpControl = Geometry<TNum, TConv>.Vector.Zero(1);

    aimPoint = smallBr[t].NearestPoint(x, out bool isInside);
    if (isInside) {
      aimPoint = smallBr[t].Vrep.Aggregate((acc, v) => acc + v) / TConv.FromInt(smallBr[t].Vrep.Count);
    }
    Geometry<TNum, TConv>.Vector l       = aimPoint - x;
    TNum                         extrVal = Geometry<TNum, TConv>.Tools.NegativeInfinity;
    foreach (Geometry<TNum, TConv>.Vector pVert in gd.P.Vrep) {
      TNum val = gd.dt * gd.D[t] * pVert * l;
      if (val > extrVal) {
        extrVal   = val;
        fpControl = pVert;
      }
    }

    return fpControl;
  }

}

public class SecondPlayerOptimalControl<TNum, TConv> : IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // большой -- первый мост (по прямому включению) который содержит 'x', если такого нет, то самый большой по объёму
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> bigBr;

  public SecondPlayerOptimalControl(SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> W) { bigBr = W; }

  public Geometry<TNum, TConv>.Vector Control(
      TNum                             t
    , Geometry<TNum, TConv>.Vector     x
    , out Geometry<TNum, TConv>.Vector aimPoint
    , GameData<TNum, TConv>            gd
    ) {
    Geometry<TNum, TConv>.Vector spControl = Geometry<TNum, TConv>.Vector.Zero(1);

    Geometry<TNum, TConv>.Vector h = bigBr[t].NearestPoint(x, out bool isInside);
    if (isInside) {
      aimPoint = h;
    }
    else {
      aimPoint = (x - h).Normalize() + x;
    }
    Geometry<TNum, TConv>.Vector l       = aimPoint - x;
    TNum                         extrVal = Geometry<TNum, TConv>.Tools.NegativeInfinity;
    foreach (Geometry<TNum, TConv>.Vector qVert in gd.Q.Vrep) {
      TNum val = gd.dt * gd.E[t] * qVert * l;
      if (val > extrVal) {
        extrVal   = val;
        spControl = qVert;
      }
    }

    return spControl;
  }

}
