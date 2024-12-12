namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {
  public class BallFactory {
    
    /// <summary>
    /// Reads the ball type from a parameter reader and creates an instance of the corresponding ball class.
    /// </summary>
    /// <param name="pr">The parameter reader.</param>
    /// <returns>
    /// An instance of a class that implements <see cref="IBall"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the ball type string is not supported.
    /// </exception>
    public static IBall Read(ParamReader pr) {
      string ballType = pr.ReadString("BallType");

      IBall ball = ballType switch
                     {
                       "Ball_1" => new Ball_1(), "Ball_2" => new Ball_2(), "Ball_oo" => new Ball_oo(), _ => throw new ArgumentException(
                                                                                                               $"Unsupported BallType: '{ballType}'.\nIn file {pr.filePath}\n" +
                                                                                                               $" Please refer to the documentation for supported types."
                                                                                                              )
                     };

      ball.ReadParameters(pr);
      return ball;
    }
  }
}
