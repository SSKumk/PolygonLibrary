using System.Numerics;
using CGLibrary;

namespace BridgeCreator;

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

  public Geometry<TNum, TConv>.ConvexPolytop Process_DistToPolytope(
      Geometry<TNum, TConv>.ParamReader pr
    , BallType                          ballType
    , TNum                              cMax
    ) {
    Geometry<TNum, TConv>.ConvexPolytop polytop = Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(pr);

    int theta = 10, phi = 10;
    if (ballType == BallType.Ball_2) {
      theta = pr.ReadNumber<int>("MTheta");
      phi   = pr.ReadNumber<int>("MPhi");
    }

    return ballType switch
             {
               BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_1(polytop, cMax)
             , BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_2(polytop, theta, phi, cMax)
             , BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_oo(polytop, cMax)
             , _                => throw new ArgumentException($"Wrong type of the ball! Found {ballType}")
             };
  }

  public Geometry<TNum, TConv>.ConvexPolytop Process_DistToOrigin(
      Geometry<TNum, TConv>.ParamReader pr
    , int                               projDim
    , BallType                          ballType
    , TNum                              cMax
    , ref string                        tsInfo
    ) {
    int theta = 10, phi = 10;
    if (ballType == BallType.Ball_2) {
      theta = pr.ReadNumber<int>("MTheta");
      phi   = pr.ReadNumber<int>("MPhi");

      tsInfo += $"-T{theta}-P{phi}_";
    }

    return ballType switch
             {
               BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_1(projDim, cMax)
             , BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_2(projDim, theta, phi, cMax)
             , BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_oo(projDim, cMax)
             , _                => throw new ArgumentOutOfRangeException($"Wrong type of the ball! Found {ballType}")
             };
  }

}
