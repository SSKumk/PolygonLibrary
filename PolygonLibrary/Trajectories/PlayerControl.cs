using System.Numerics;
using CGLibrary;

namespace Trajectories;

public class PlayerControl<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public TrajMain<TNum, TConv>.ControlType controlType;

  public Geometry<TNum, TConv>.Vector AimPoint;

  public Geometry<TNum, TConv>.GameData gd;

  public Geometry<TNum, TConv>.Vector constControl; // для случая Constant

  public Geometry<TNum, TConv>.Vector x;

  /// <summary>
  /// Collection of matrices D for the instants from the time grid
  /// </summary>
  public readonly Geometry<TNum, TConv>.Matrix Dt;

  /// <summary>
  /// Collection of matrices E for the instants from the time grid
  /// </summary>
  public readonly Geometry<TNum, TConv>.Matrix Et;


  /// <summary>
  /// The bridge of the game
  /// </summary>
  public readonly Geometry<TNum, TConv>.ConvexPolytop Wt;

  public PlayerControl(
      Geometry<TNum, TConv>.Vector        x
    , Geometry<TNum, TConv>.Matrix        Dt
    , Geometry<TNum, TConv>.Matrix        Et
    , Geometry<TNum, TConv>.ConvexPolytop Wt
    , Geometry<TNum, TConv>.GameData      gameData
    , TrajMain<TNum, TConv>.ControlType   controlType
    ) {
    gd               = gameData;
    this.Dt          = Dt;
    this.Et          = Et;
    this.Wt          = Wt;
    this.x           = x;
    this.controlType = controlType;
  }

  public virtual Geometry<TNum, TConv>.Vector Constant() { return Geometry<TNum, TConv>.Vector.Zero(1); }
  public virtual Geometry<TNum, TConv>.Vector Optimal()  { return Geometry<TNum, TConv>.Vector.Zero(1); }

  public Geometry<TNum, TConv>.Vector Control() {
    return controlType switch
             {
               TrajMain<TNum, TConv>.ControlType.Optimal  => Optimal()
             , TrajMain<TNum, TConv>.ControlType.Constant => Constant()
             };
  }

}
