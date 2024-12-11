namespace Bridges;

public abstract class TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {


  public readonly string                         terminalSetName; // имя терминального множества в выходном файле
  public          string                         terminalSetInfo;
  public          Geometry<TNum, TConv>.GameData gd;


  protected TerminalSetBase(Geometry<TNum, TConv>.ParamReader pr, Geometry<TNum, TConv>.GameData gameData, string tsInfo) {
    terminalSetName = pr.ReadString("Name");
    gd              = gameData;
    terminalSetInfo = tsInfo;
  }


  public abstract void DoSolve(string baseWorkDir);


  protected (int theta, int phi) ReadBall2Params(Geometry<TNum, TConv>.ParamReader pr) {
    int theta = pr.ReadNumber<int>("MTheta");
    int phi   = pr.ReadNumber<int>("MPhi");

    return (theta, phi);
  }

}
