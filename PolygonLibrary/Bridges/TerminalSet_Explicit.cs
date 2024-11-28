using System.Numerics;
using CGLibrary;

namespace Bridges;

public class TerminalSet_Explicit<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly Geometry<TNum, TConv>.ConvexPolytop terminalSet;
  public readonly string                              terminalSetInfo;

  public TerminalSet_Explicit(Geometry<TNum, TConv>.ParamReader pr, int projDim) : base(pr) {
    terminalSet     =  Geometry<TNum, TConv>.GameData.ReadExplicitSet(pr, 'M', projDim, out terminalSetInfo);
    terminalSetInfo += "_Explicit_";
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
    Geometry<TNum, TConv>.SolverLDG sl = new Geometry<TNum, TConv>.SolverLDG
    (
     Path.Combine(baseWorkDir, NumericalType, Eps)
   , gameData
   , projDim
   , projInd
   , terminalSet
   , $"{gameInfoNoTerminalInfo}{terminalSetInfo}"
   , PsInfo
   , QsInfo
    );
    sl.Solve();
  }

}
