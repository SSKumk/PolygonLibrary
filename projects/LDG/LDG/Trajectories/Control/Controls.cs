using System.Diagnostics;

namespace LDG;

public class FirstPlayerConstantControl<TNum, TConv> : ControlData<TNum, TConv>, IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly Geometry<TNum, TConv>.Vector constantControl;

  public FirstPlayerConstantControl(Geometry<TNum, TConv>.ParamReader pr, GameData<TNum, TConv> gd) : base(gd) {
    constantControl = pr.ReadVector("Constant");
    Debug.Assert
      (
       gd.P.GetInFLrep().ContainsNonStrict(constantControl)
     , $"FirstPlayerConstantControl: The control vector doesn't belong to P. Found {constantControl}"
      );
  }

  public Geometry<TNum, TConv>.Vector Control(TNum t, Geometry<TNum, TConv>.Vector x, out Geometry<TNum, TConv>.Vector aimPoint) {
    aimPoint = x + (gd.D[t] * constantControl).NormalizeZero();

    return constantControl;
  }

}

public class SecondPlayerConstantControl<TNum, TConv> : ControlData<TNum, TConv>, IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly Geometry<TNum, TConv>.Vector constantControl;

  public SecondPlayerConstantControl(Geometry<TNum, TConv>.ParamReader pr, GameData<TNum, TConv> gd) : base(gd) {
    constantControl = pr.ReadVector("Constant");
    Debug.Assert
      (
       gd.Q.GetInFLrep().ContainsNonStrict(constantControl)
     , $"FirstPlayerConstantControl: The control vector doesn't belong to Q. Found {constantControl}"
      );
  }

  public Geometry<TNum, TConv>.Vector Control(TNum t, Geometry<TNum, TConv>.Vector x, out Geometry<TNum, TConv>.Vector aimPoint) {
    aimPoint = x + (gd.E[t] * constantControl).NormalizeZero();

    return constantControl;
  }

}

public class FirstPlayerOptimalControl<TNum, TConv> : ControlData<TNum, TConv>, IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> bridges;

  public FirstPlayerOptimalControl(List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws, GameData<TNum, TConv> gd)
    : base(gd) {
    bridges = Ws;
  }

  public Geometry<TNum, TConv>.Vector Control(TNum t, Geometry<TNum, TConv>.Vector x, out Geometry<TNum, TConv>.Vector aimPoint) {
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
          -1 => h - smallBr.Hrep.Find
                  (hp => hp.Contains(h))!.Normal // если строго внутри, то целимся в противоположную сторону от ближайшей грани
        , 0 => smallBr.Vrep.Aggregate((acc, v) => acc + v) / TConv.FromInt
                 (smallBr.Vrep.Count) // если на границе, то целимся "во-внутрь"
        , 1 => h                      // если снаружи, то на ближайшую точку на мосте
        , _ => throw new ArgumentException
                 (
                  $"Controls.FPOptimalControl: Invalid value of position it should be -1 (inside), 0 (on the border), and 1 (outside). Found {position}"
                 )
        };

    if (position <= 0) { }
    Geometry<TNum, TConv>.Vector l       = aimPoint - x;
    TNum                         extrVal = gd.dt * gd.D[t] * gd.P.Vrep.First() * l;
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

public class SecondPlayerOptimalControl<TNum, TConv> : ControlData<TNum, TConv>, IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> bridges;

  public SecondPlayerOptimalControl(
      List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws
    , GameData<TNum, TConv>                                             gd
    ) : base(gd) {
    bridges = Ws;
  }

  public Geometry<TNum, TConv>.Vector Control(TNum t, Geometry<TNum, TConv>.Vector x, out Geometry<TNum, TConv>.Vector aimPoint) {
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
    TNum                         extrVal = gd.dt * gd.E[t] * gd.Q.Vrep.First() * l;
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

public class FirstPlayerRandomControl_RectParallel<TNum, TConv> : ControlData<TNum, TConv>, IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly uint                            seed;
  public readonly Geometry<TNum, TConv>.GRandomLC rnd;
  public readonly Geometry<TNum, TConv>.Vector    min;
  public readonly Geometry<TNum, TConv>.Vector    max;

  public FirstPlayerRandomControl_RectParallel(Geometry<TNum, TConv>.ParamReader pr, GameData<TNum, TConv> gd) : base(gd) {
    seed = pr.ReadNumber<uint>("Seed");
    rnd  = new Geometry<TNum, TConv>.GRandomLC(seed);

    min = gd.P.Vrep.Min!;
    max = gd.P.Vrep.Max!;
  }

  public Geometry<TNum, TConv>.Vector Control(TNum t, Geometry<TNum, TConv>.Vector x, out Geometry<TNum, TConv>.Vector aimPoint) {
    TNum[] rndControl = new TNum[gd.pDim];
    for (int i = 0; i < rndControl.Length; i++) {
      rndControl[i] = rnd.NextPrecise(min[i], max[i]);
    }

    Geometry<TNum, TConv>.Vector control = new Geometry<TNum, TConv>.Vector(rndControl);
    aimPoint = x + (gd.D[t] * control).NormalizeZero();

    return control;
  }

}

public class SecondPlayerRandomControl_RectParallel<TNum, TConv> : ControlData<TNum, TConv>, IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly uint                            seed;
  public readonly Geometry<TNum, TConv>.GRandomLC rnd;
  public readonly Geometry<TNum, TConv>.Vector    min;
  public readonly Geometry<TNum, TConv>.Vector    max;

  public SecondPlayerRandomControl_RectParallel(Geometry<TNum, TConv>.ParamReader pr, GameData<TNum, TConv> gd) : base(gd) {
    seed = pr.ReadNumber<uint>("Seed");
    rnd  = new Geometry<TNum, TConv>.GRandomLC(seed);

    min = gd.Q.Vrep.Min!;
    max = gd.Q.Vrep.Max!;
  }

  public Geometry<TNum, TConv>.Vector Control(TNum t, Geometry<TNum, TConv>.Vector x, out Geometry<TNum, TConv>.Vector aimPoint) {
    TNum[] rndControl = new TNum[gd.qDim];
    for (int i = 0; i < rndControl.Length; i++) {
      rndControl[i] = rnd.NextPrecise(min[i], max[i]);
    }

    Geometry<TNum, TConv>.Vector control = new Geometry<TNum, TConv>.Vector(rndControl);
    aimPoint = x + (gd.E[t] * control).NormalizeZero();

    return control;
  }

}

public class ControlData<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public GameData<TNum, TConv> gd;
  public ControlData(GameData<TNum, TConv> gd) { this.gd = gd; }

}
