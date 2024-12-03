using System.Numerics;
using CGLibrary;

namespace Trajectories;

public class PlayerControl<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public enum ControlType { Optimal, Constant }

  public readonly Geometry<TNum, TConv>.GameData gd;

  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> D;
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> E;
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> W;

  protected ControlType controlType;
  public string controlTypeInfo = "";


  public List<Geometry<TNum, TConv>.Vector> AimPoints = new List<Geometry<TNum, TConv>.Vector>();

#region Поля под разные виды управлений
  public Geometry<TNum, TConv>.Vector constantControl;
#endregion


  public PlayerControl(
      SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>        D
    , SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>        E
    , SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> W
    , Geometry<TNum, TConv>.GameData                              gameData
    ) {
    gd     = gameData;
    this.D = D;
    this.E = E;
    this.W = W;
  }

  public virtual (Geometry<TNum, TConv>.Vector Control, Geometry<TNum, TConv>.Vector AimPoint) Constant(Geometry<TNum, TConv>.Vector x) {
    return (Geometry<TNum, TConv>.Vector.Zero(1), Geometry<TNum, TConv>.Vector.Zero(1));
  }

  public virtual (Geometry<TNum, TConv>.Vector Control, Geometry<TNum, TConv>.Vector AimPoint) Optimal(TNum t, Geometry<TNum, TConv>.Vector x) {
    return (Geometry<TNum, TConv>.Vector.Zero(1), Geometry<TNum, TConv>.Vector.Zero(1));
  }

  public Geometry<TNum, TConv>.Vector Control(TNum t, Geometry<TNum, TConv>.Vector x) {
    (Geometry<TNum, TConv>.Vector control, Geometry<TNum, TConv>.Vector aimPoint) =
      controlType switch
        {
          ControlType.Optimal  => Optimal(t, x)
        , ControlType.Constant => Constant(x)
        };
    AimPoints.Add(aimPoint);
    return control;
  }

  //todo: char player переделать на enum Player и сделать метод string GetPlayerPrefix(Player) => "P" or "Q";
  public ControlType ReadControlType(Geometry<TNum, TConv>.ParamReader pr, char player) {
    ControlType typeInfo =
      pr.ReadString($"{player}Control") switch
        {
          "Optimal"  => ControlType.Optimal
        , "Constant" => ControlType.Constant
        , _          => throw new ArgumentException("!!!")
        };

    controlTypeInfo += $"{player}";
    controlTypeInfo += typeInfo;

    return typeInfo;
  }

  public void ReadControl(Geometry<TNum, TConv>.ParamReader pr, char player) {
    switch (controlType) {
      case ControlType.Optimal: break;
      case ControlType.Constant: {
        constantControl =  pr.ReadVector("Value"); //todo: проверять корректность вх. данных (dim B, dim C)
        controlTypeInfo += $"_{constantControl}_";

        break;
      }
      default: throw new ArgumentOutOfRangeException();
    }
  }

}
