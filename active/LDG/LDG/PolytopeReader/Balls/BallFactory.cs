namespace LDG;

/// <summary>
/// Factory that reads ball parameters from the given parameter reader based on the specified ball type.
/// </summary>
public abstract class BallFactory<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads the ball type from the provided parameter reader and creates an instance of the corresponding ball class.
  /// </summary>
  /// <param name="pr">The parameter reader that contains the ball type and its parameters.</param>
  /// <returns>
  /// An instance of a class implementing <see cref="IBall{TNum, TConv}"/> corresponding to the specified ball type.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// Thrown if an unsupported ball type is encountered in the parameter reader.
  /// </exception>
  public static IBall<TNum, TConv> Read(Geometry<TNum, TConv>.ParamReader pr) {
    string ballType = pr.ReadString("BallType");

    IBall<TNum, TConv> ball =
      ballType switch
        {
          "Ball_1"  => new Ball_1<TNum, TConv>()
        , "Ball_2"  => new Ball_2<TNum, TConv>()
        , "Ball_oo" => new Ball_oo<TNum, TConv>()
        , _ => throw new ArgumentException
                 (
                  $"Unsupported BallType: '{ballType}'.\nIn file {pr.filePath}\n" +
                  $" Please refer to the documentation for supported types."
                 )
        };

    ball.ReadParameters(pr);

    return ball;
  }
}
