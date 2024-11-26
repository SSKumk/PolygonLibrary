using System.Numerics;
using CGLibrary;

namespace BridgeCreator;

public class TerminalSet_MinkowskiFunctional<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string terminalSetInfo;

  public readonly TNum[]                                    cs; // коэффициенты в преобразовании Минковского
  public readonly List<Geometry<TNum, TConv>.ConvexPolytop> minkFuncSets = new List<Geometry<TNum, TConv>.ConvexPolytop>();

  public TerminalSet_MinkowskiFunctional(Geometry<TNum, TConv>.ParamReader pr, int projDim) : base(pr) {
    int cQnt = pr.ReadNumber<int>("CQnt");
    cs = pr.Read1DArray<TNum>("Constants", cQnt);

    Geometry<TNum, TConv>.ConvexPolytop basePolytop =
      Geometry<TNum, TConv>.GameData.ReadExplicitSet(pr, 'M', projDim, out terminalSetInfo).GetInFLrep();
    terminalSetInfo += "_MinkowskiFunctional_";


    if (!basePolytop.ContainsStrict(Geometry<TNum, TConv>.Vector.Zero(projDim))) {
      throw new ArgumentException("TerminalSet_MinkowskiFunctional.Ctor: The origin should lie within the polytope!");
    }

    foreach (TNum num in cs) {
      minkFuncSets.Add(basePolytop.Homothety(num));
    }
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
    for (int i = 0; i < cs.Length; i++) {
      Geometry<TNum, TConv>.SolverLDG sl =
        new Geometry<TNum, TConv>.SolverLDG
          (
           Path.Combine(baseWorkDir, TConv.ToDouble(cs[i]).ToString("G"), NumericalType, Eps)
         , gameData
         , projDim
         , projInd
         , minkFuncSets[i]
         , $"{gameInfoNoTerminalInfo}{terminalSetInfo}"
         , PsInfo
         , QsInfo
          );
      sl.Solve();
    }
  }

}
