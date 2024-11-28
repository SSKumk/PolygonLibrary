using System.Numerics;
using CGLibrary;

namespace Bridges;

public class TerminalSet_MinkowskiFunctional<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string terminalSetInfo;

  public readonly TNum[]                                    cs; // коэффициенты в преобразовании Минковского
  public readonly List<Geometry<TNum, TConv>.ConvexPolytop> minkFuncSets = new List<Geometry<TNum, TConv>.ConvexPolytop>();

  public TerminalSet_MinkowskiFunctional(Geometry<TNum, TConv>.ParamReader pr, Geometry<TNum, TConv>.GameData gameData) : base
    (pr, gameData) {
    int cQnt = pr.ReadNumber<int>("CQnt");
    cs = pr.Read1DArray<TNum>("Constants", cQnt);

    Geometry<TNum, TConv>.ConvexPolytop basePolytop =
      Geometry<TNum, TConv>.GameData.ReadExplicitSet(pr, 'M', gd.projDim, out terminalSetInfo).GetInFLrep();
    terminalSetInfo += "_MinkowskiFunctional_";


    if (!basePolytop.ContainsStrict(Geometry<TNum, TConv>.Vector.Zero(gd.projDim))) {
      throw new ArgumentException("TerminalSet_MinkowskiFunctional.Ctor: The origin should lie within the polytope!");
    }

    foreach (TNum num in cs) {
      minkFuncSets.Add(basePolytop.Homothety(num));
    }
  }

  public override void DoSolve(
      string                         baseWorkDir
    ) {
    for (int i = 0; i < cs.Length; i++) {
      Geometry<TNum, TConv>.SolverLDG sl =
        new Geometry<TNum, TConv>.SolverLDG
          (
           Path.Combine(baseWorkDir, TConv.ToDouble(cs[i]).ToString("G"))
         , gd
         , minkFuncSets[i]
         , terminalSetInfo
          );
      sl.Solve();
    }
  }

}