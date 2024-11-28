using System.Numerics;
using CGLibrary;

namespace Bridges;

public class TerminalSet_Explicit<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly Geometry<TNum, TConv>.ConvexPolytop terminalSet;
  public readonly string                              terminalSetInfo;

  public TerminalSet_Explicit(Geometry<TNum, TConv>.ParamReader pr, Geometry<TNum, TConv>.GameData gameData) : base
    (pr, gameData) {
    terminalSet     =  Geometry<TNum, TConv>.GameData.ReadExplicitSet(pr, 'M', gd.projDim, out terminalSetInfo);
    terminalSetInfo += "_Explicit_";
  }

  public override void DoSolve(string baseWorkDir) {
    Geometry<TNum, TConv>.SolverLDG sl = new Geometry<TNum, TConv>.SolverLDG(baseWorkDir, gd, terminalSet, $"{terminalSetInfo}");
    sl.Solve();
  }

}
