using System.Numerics;
using CGLibrary;

namespace BridgeCreator;

public class TerminalSet_LevelSet<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public enum LevelSetType { DistToOrigin, DistToPolytope }

  public readonly string terminalSetInfo = "_LevelSet_";

  public readonly TNum[] cs; // коэффициенты

  // public readonly List<Geometry<TNum, TConv>.ConvexPolytop> levelSets = new List<Geometry<TNum, TConv>.ConvexPolytop>();
  public readonly LevelSetType levelSetType;
  public readonly BallType     ballType;

  private Geometry<TNum, TConv>.ParamReader _pr;

  public TerminalSet_LevelSet(Geometry<TNum, TConv>.ParamReader pr) : base(pr) {
    int qnt = pr.ReadNumber<int>("CQnt");
    cs = pr.Read1DArray<TNum>("Constants", qnt);

    levelSetType =
      pr.ReadString("MType") switch
        {
          "DistToOrigin"   => LevelSetType.DistToOrigin
        , "DistToPolytope" => LevelSetType.DistToPolytope
        , _                => throw new ArgumentOutOfRangeException()
        };
    terminalSetInfo += levelSetType;

    ballType =
      pr.ReadString("MBallType") switch
        {
          "Ball_1"  => BallType.Ball_1
        , "Ball_2"  => BallType.Ball_2
        , "Ball_oo" => BallType.Ball_oo
        , _         => throw new ArgumentOutOfRangeException()
        };
    terminalSetInfo += ballType;

    _pr = pr;
  }

  public override void Solve(
      string                         baseWorkDir
    , Geometry<TNum, TConv>.GameData gameData
    , int                            projDim
    , int[]                          projInd
    , string                         gameInfoNoTerminalInfo
    , string                         PsInfo
    , string                         QsInfo
    ) {
    foreach (TNum num in cs) {
      Geometry<TNum, TConv>.ConvexPolytop terminalSet;

      string tsInfo = $"_{cs}_";
      switch (levelSetType) {
        case LevelSetType.DistToOrigin: {
          terminalSet = Process_DistToOrigin(_pr, projDim - 1, ballType, num, ref tsInfo);
          break;
        }
        case LevelSetType.DistToPolytope: {
          terminalSet = Process_DistToPolytope(_pr, ballType, num);

          break;
        }
        default:
          throw new ArgumentException("TerminalSet_LevelSet: Другие варианты непредусмотрены!");

          tsInfo += $""; //todo: какую инфу по многограннику выдавать-то? М.б. он сам должен это делать?
      }

      Geometry<TNum, TConv>.SolverLDG sl =
        new Geometry<TNum, TConv>.SolverLDG
          (
           Path.Combine(baseWorkDir, TConv.ToDouble(num).ToString("G"), NumericalType, Eps)
         , gameData
         , projDim
         , projInd
         , terminalSet
         , $"{gameInfoNoTerminalInfo}{terminalSetInfo}{tsInfo}"
         , PsInfo
         , QsInfo
          );
      sl.Solve();
    }
  }

}
