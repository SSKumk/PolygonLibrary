namespace LDG;

public class FirstPlayerConstantControl<TNum, TConv> : IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly Geometry<TNum, TConv>.Vector constantControl;

  public FirstPlayerConstantControl(Geometry<TNum, TConv>.ParamReader pr) { constantControl = pr.ReadVector("Constant"); }

  public Geometry<TNum, TConv>.Vector Control(
      TNum                             t
    , Geometry<TNum, TConv>.Vector     x
    , out Geometry<TNum, TConv>.Vector aimPoint
    , GameData<TNum, TConv>            gd
    ) {
    aimPoint = x + (gd.D[t] * constantControl).NormalizeZero();

    return constantControl;
  }

}

public class SecondPlayerConstantControl<TNum, TConv> : IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly Geometry<TNum, TConv>.Vector constantControl;

  public SecondPlayerConstantControl(Geometry<TNum, TConv>.ParamReader pr) { constantControl = pr.ReadVector("Constant"); }

  public Geometry<TNum, TConv>.Vector Control(
      TNum                             t
    , Geometry<TNum, TConv>.Vector     x
    , out Geometry<TNum, TConv>.Vector aimPoint
    , GameData<TNum, TConv>            gd
    ) {
    aimPoint = x + (gd.E[t] * constantControl).NormalizeZero();

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
    , out Geometry<TNum, TConv>.Vector aimPoint
    , GameData<TNum, TConv>            gd
    ) {
    Geometry<TNum, TConv>.Vector fpControl = Geometry<TNum, TConv>.Vector.Zero(1);

    // маленький -- первый мост (по обратному включению) вне которого лежит 'x'. Если такого нет, то таковым объявляется самый малый по объёму
    Geometry<TNum, TConv>.ConvexPolytop? smallBr = null;
    for (int i = bridges.Count - 1; i >= 0; i--) {
      if (bridges[i][t].ContainsComplement(x)) {
        smallBr = bridges[i][t];

        break;
      }
    }
    smallBr ??= bridges.First()[t];

    Geometry<TNum, TConv>.Vector h = smallBr.NearestPoint(x, out int position);
    aimPoint =
      position switch
        {
          -1 => h - smallBr.Hrep.Find(hp => hp.Contains(h))!.Normal // если строго внутри, то целимся в противоположную сторону от ближайшей грани
        , 0  => smallBr.Vrep.Aggregate((acc, v) => acc + v) / TConv.FromInt(smallBr.Vrep.Count) // если на границе, то целимся "во-внутрь"
        , 1  => h // если снаружи, то на ближайшую точку на мосте
        , _ => throw new ArgumentException
                 (
                  $"Controls.FPOptimalControl: Invalid value of position it should be -1 (inside), 0 (on the border), and 1 (outside). Found {position}"
                 )
        };

    if (position <= 0) { }
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

  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> bridges;

  public SecondPlayerOptimalControl(List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws) { bridges = Ws; }

  public Geometry<TNum, TConv>.Vector Control(
      TNum                             t
    , Geometry<TNum, TConv>.Vector     x
    , out Geometry<TNum, TConv>.Vector aimPoint
    , GameData<TNum, TConv>            gd
    ) {
    Geometry<TNum, TConv>.Vector spControl = Geometry<TNum, TConv>.Vector.Zero(1);

    // большой -- первый мост (по прямому включению) который строго содержит 'x', если такого нет, то самый большой по объёму
    Geometry<TNum, TConv>.ConvexPolytop? bigBr = null;

    for (int i = 0; i < bridges.Count; i++) {
      if (bridges[i][t].ContainsStrict(x)) {
        bigBr = bridges[i][t];

        break;
      }
    }
    bigBr ??= bridges.Last()[t];

    Geometry<TNum, TConv>.Vector h = bigBr.NearestPoint(x, out int position);
    aimPoint =
      position switch
        {
          -1 => // если внутри, то целимся "на" мост
            h
        , 0 => // если на границе, то целимся по любой нормали к гиперграням, которые формируют данную k-грань, где находится x
            h + bigBr.Hrep.Find(hp => hp.Contains(h))!.Normal
        , 1 => // если снаружи, то целимся "от" моста
            (x - h).Normalize() + x
        , _ => throw new ArgumentException
                 (
                  $"Controls.SPOptimalControl: Invalid value of position it should be -1 (inside), 0 (on the border), and 1 (outside). Found {position}"
                 )
        };

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
