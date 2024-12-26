using System.Diagnostics;

namespace LDG;

public class Tools<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Converts the given number to a string representation with two decimal places. For example, 1.23.
  /// The number is converted to a double using the specified converter type TConv.
  /// </summary>
  /// <param name="t">The numeric value to convert.</param>
  /// <returns>A string representation of the number with two decimal places.</returns>
  public static string ToPrintTNum(TNum t) => $"{TConv.ToDouble(t):F2}";

}
