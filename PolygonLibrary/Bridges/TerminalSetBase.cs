using System.Numerics;
using CGLibrary;

namespace Bridges;

public abstract class TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public enum BallType { Ball_1, Ball_2, Ball_oo }

  public string TerminalSetName; // имя терминального множества в выходном файле


  protected TerminalSetBase(Geometry<TNum, TConv>.ParamReader pr) { TerminalSetName = pr.ReadString("Name"); }

  public static readonly string NumericalType = typeof(TNum).ToString(); // текущий используемый числовой тип
  public static readonly string Eps = $"{TConv.ToDouble(Geometry<TNum, TConv>.Tools.Eps):e0}"; // текущая точность в библиотеке


  public abstract void Solve(
      string                         baseWorkDir
    , Geometry<TNum, TConv>.GameData gameData
    , int                            projDim
    , int[]                          projInd
    , string                         gameInfoNoTerminalInfo
    , string                         PsInfo
    , string                         QsInfo
    );


  protected (int theta, int phi) ReadBall2Params(Geometry<TNum, TConv>.ParamReader pr, ref string tsInfo) {
    int theta = pr.ReadNumber<int>("MTheta");
    int phi   = pr.ReadNumber<int>("MPhi");
    tsInfo += $"-T{theta}-P{phi}_";

    return (theta, phi);
  }

}
