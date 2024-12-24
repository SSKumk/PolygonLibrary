namespace LDG;

/// <summary>
/// Represents the unit ball for the first norm in a finite-dimensional space.
/// This norm uses the sum of absolute values of coordinates (Manhattan distance).
/// </summary>
/// <remarks>
/// This norm does not require any additional parameters for its configuration.
/// </remarks>
public class Ball_1<TNum, TConv> : IBall<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads no additional parameters for the unit ball of the first norm.
  /// </summary>
  /// <param name="pr">The parameter reader used to read data from the configuration source.</param>
  public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr) { } // Ball_1 has no parameters

}

/// <summary>
/// Represents the unit ball for the second norm (Euclidean norm) in a finite-dimensional space.
/// This norm uses the square root of the sum of squared coordinates (Euclidean distance).
/// </summary>
/// <remarks>
/// This norm requires two parameters:
/// - AzimuthsDivisions: The number of divisions in each of azimuthal directions.
/// - PolarDivision: The number of divisions in the polar direction.
/// </remarks>
public class Ball_2<TNum, TConv> : IBall<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public int PolarDivision     { get; private set; }
  public int AzimuthsDivisions { get; private set; }

  /// <summary>
  /// Reads the parameters for the unit ball of the second norm.
  /// Specifically, it reads:
  /// - AzimuthsDivisions: The number of divisions in each of azimuthal directions.
  /// - PolarDivision: The number of divisions in the polar direction.
  /// </summary>
  /// <param name="pr">The parameter reader used to read the parameters for the norm.</param>
  public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr) {
    AzimuthsDivisions = pr.ReadNumber<int>("AzimuthsDivisions");
    PolarDivision     = pr.ReadNumber<int>("PolarDivision");
  }

}

/// <summary>
/// Represents the unit ball for the infinity norm (maximum norm) in a finite-dimensional space.
/// This norm uses the maximum absolute value of coordinates (Chebyshev distance).
/// </summary>
/// <remarks>
/// This norm does not require any additional parameters for its configuration.
/// </remarks>
public class Ball_oo<TNum, TConv> : IBall<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads no additional parameters for the unit ball of the infinity norm.
  /// </summary>
  /// <param name="pr">The parameter reader used to read data from the configuration source.</param>
  public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr) { } // Ball_oo has no parameters

}
