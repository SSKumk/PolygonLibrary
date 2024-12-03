namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class FourierMotzkin {

    public List<HyperPlane> HPs { get; }

    public FourierMotzkin(List<HyperPlane> hPs) { HPs = hPs; }

    public FourierMotzkin EliminateVariableNaive(int variableNum) {
      int variableIndex = variableNum - 1;

      List<HyperPlane> upperBounds = new List<HyperPlane>(); // неравенства с положительным коэффициентом
      List<HyperPlane> lowerBounds = new List<HyperPlane>(); // неравенства с отрицательным коэффициентом
      List<HyperPlane> neutral     = new List<HyperPlane>(); // неравенства без переменной (нулевой коэффициент)

      foreach (HyperPlane hp in HPs) {
        TNum coefficient = hp.Normal[variableIndex];

        if (Tools.GT(coefficient)) {
          upperBounds.Add(hp);
        }
        else if (Tools.LT(coefficient)) {
          lowerBounds.Add(hp);
        }
        else {
          neutral.Add(hp);
        }
      }


      int              spaceDim        = HPs[0].SpaceDim;
      List<HyperPlane> newInequalities = new List<HyperPlane>(neutral);
      foreach (HyperPlane upper in upperBounds) { // Генерация новых неравенств путём комбинирования верхних и нижних
        foreach (HyperPlane lower in lowerBounds) {
          TNum[] newRow = new TNum[spaceDim];
          for (int j = 0; j < spaceDim; j++) {
            if (j == variableIndex) { continue; }

            newRow[j] = lower.Normal[j] / -lower.Normal[variableIndex] + upper.Normal[j] / upper.Normal[variableIndex];
          }
          TNum constant = lower.ConstantTerm / -lower.Normal[variableIndex] + upper.ConstantTerm / upper.Normal[variableIndex];

          Vector newIneq = new Vector(newRow);
          if (!newIneq.IsZero) {
            newInequalities.Add(new HyperPlane(newIneq, constant));
          }
        }
      }

      return new FourierMotzkin(newInequalities);
    }

  }

}
