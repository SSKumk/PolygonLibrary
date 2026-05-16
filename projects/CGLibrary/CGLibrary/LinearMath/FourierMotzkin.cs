namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Naive Fourier-Motzkin elimination for systems of linear inequalities represented by <see cref="HyperPlane"/>.
  /// </summary>
  /// <remarks>
  /// The implementation keeps the original ambient dimension in the resulting inequalities and simply zeroes
  /// the eliminated variable coefficient. No redundancy removal or post-processing is performed.
  /// </remarks>
  public class FourierMotzkin {

    /// <summary>
    /// Gets the current list of inequalities.
    /// </summary>
    public List<HyperPlane> HPs { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FourierMotzkin"/> class from a list of inequalities.
    /// </summary>
    /// <param name="hPs">The inequalities to work with.</param>
    public FourierMotzkin(List<HyperPlane> hPs) { HPs = hPs; }

    /// <summary>
    /// Eliminates one variable using the naive Fourier-Motzkin combination rule.
    /// </summary>
    /// <param name="variableNum">One-based number of the variable to eliminate.</param>
    /// <returns>
    /// A new <see cref="FourierMotzkin"/> instance containing:
    /// neutral inequalities copied as-is and all pairwise combinations of upper and lower bounds.
    /// Zero inequalities produced by exact cancellation are skipped.
    /// </returns>
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
