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
    , out Geometry<TNum, TConv>.Vector aim
    , GameData<TNum, TConv>            gd
    ) {
    aim = (x + gd.Xstar(t) * constantControl).Normalize();

    return constantControl;
  }

}

public class FirstPlayerOptimalControl<TNum, TConv> : IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> bridges;

  public FirstPlayerOptimalControl(List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws) { bridges = Ws; }

  public Geometry<TNum, TConv>.Vector Control(
      TNum                             t
    , Geometry<TNum, TConv>.Vector     x
    , out Geometry<TNum, TConv>.Vector aim
    , GameData<TNum, TConv>            gd
    ) {
    //todo: КАК-ТО нашли Sm маленький мост
    // маленький -- первый мост (по обратному включению) вне которого лежит 'x'. Если такого нет, то таковым объявляется самый малый по объёму

    var                          Sm        = bridges[100];
    Geometry<TNum, TConv>.Vector fpControl = Geometry<TNum, TConv>.Vector.Zero(1);

    Geometry<TNum, TConv>.Vector aimPoint = Sm[t].NearestPoint(x, out bool isInside);
    if (isInside) {
      aimPoint = Sm[t].Vrep.Aggregate((acc, v) => acc + v) / TConv.FromInt(Sm[t].Vrep.Count);
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
  }

}

public class SecondPlayerOptimalControl<TNum, TConv> : IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {


  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> bridges;

  public SecondPlayerOptimalControl(List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws) { bridges = Ws; }

  public Geometry<TNum, TConv>.Vector Control(
      TNum                             t
    , Geometry<TNum, TConv>.Vector     x
    , out Geometry<TNum, TConv>.Vector aim
    , GameData<TNum, TConv>            gd
    ) {
    //todo: КАК-ТО нашли Bg большой мост
    // большой -- первый мост (по прямому включению) который содержит 'x'ч

    var Bg = bridges[1000];

    throw new NotImplementedException();
  }

}
