namespace Bridges;

public class TerminalSet_MinkowskiFunctional<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly TNum[]                                    cs; // коэффициенты в преобразовании Минковского
  public readonly List<Geometry<TNum, TConv>.ConvexPolytop> minkFuncSets = new List<Geometry<TNum, TConv>.ConvexPolytop>();

  public TerminalSet_MinkowskiFunctional(
      Geometry<TNum, TConv>.ParamReader pr
    , Geometry<TNum, TConv>.GameData    gameData
    , string                            tsInfo
    ) : base(pr, gameData, tsInfo) {
    int cQnt = pr.ReadNumber<int>("CQnt");
    cs = pr.Read1DArray<TNum>("Constants", cQnt);

    Geometry<TNum, TConv>.ConvexPolytop basePolytop =
      Geometry<TNum, TConv>.GameData.ReadExplicitSet(pr, 'M', gd.projDim, out string tsInfo2).GetInFLrep();
    terminalSetInfo += tsInfo2;


    if (!basePolytop.ContainsStrict(Geometry<TNum, TConv>.Vector.Zero(gd.projDim))) {
      throw new ArgumentException("TerminalSet_MinkowskiFunctional.Ctor: The origin should lie within the polytope!");
    }

    foreach (TNum num in cs) {
      minkFuncSets.Add(basePolytop.Scale(num));
    }
  }

  public override void DoSolve(string baseWorkDir) {
    for (int i = 0; i < cs.Length; i++) {
      Geometry<TNum, TConv>.SolverLDG sl =
        new Geometry<TNum, TConv>.SolverLDG
          (Path.Combine(baseWorkDir, TConv.ToDouble(cs[i]).ToString("G")), gd, minkFuncSets[i], terminalSetInfo);
      sl.Solve();
    }
  }

}
