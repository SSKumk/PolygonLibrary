using System.Numerics;
using CGLibrary;

namespace Bridges;

public abstract class TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public enum BallType { Ball_1, Ball_2, Ball_oo }

  public string                         TerminalSetName; // имя терминального множества в выходном файле
  public Geometry<TNum, TConv>.GameData gd;


  protected TerminalSetBase(Geometry<TNum, TConv>.ParamReader pr, Geometry<TNum, TConv>.GameData gamedata) {
    TerminalSetName = pr.ReadString("Name");
    gd              = gamedata;
  }


  public abstract void DoSolve(string baseWorkDir);


  protected (int theta, int phi) ReadBall2Params(Geometry<TNum, TConv>.ParamReader pr, ref string tsInfo) {
    int theta = pr.ReadNumber<int>("MTheta");
    int phi   = pr.ReadNumber<int>("MPhi");
    tsInfo += $"-T{theta}-P{phi}_";

    return (theta, phi);
  }

}
