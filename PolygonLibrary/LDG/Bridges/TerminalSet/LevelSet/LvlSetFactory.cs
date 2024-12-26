namespace LDG;

/// <summary>
/// A factory class for creating level set types used in constructing terminal sets.
/// </summary>
public abstract class LvlSetFactory<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads the level set configuration and creates an appropriate level set type.
  /// </summary>
  /// <param name="pr">The parameter reader used to extract the level set configuration from the terminal set file.</param>
  /// <param name="ph">
  /// Provides access to files describing polytopes. This is required only for level sets 
  /// of type "DistToPolytope".
  /// </param>
  /// <returns>
  /// An instance of <see cref="ILvlSetType{TNum, TConv}"/> configured based on the provided parameters.
  /// </returns>
  /// <exception cref="ArgumentException">
  /// Thrown when an unsupported level set type is specified in the parameters.
  /// </exception>
  public static ILvlSetType<TNum, TConv> Read(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
    string lvlSet = pr.ReadString("Type");
    ILvlSetType<TNum, TConv> lvlSetType =
      lvlSet switch
        {
          "DistToPoint"    => new LvlSetTypes<TNum, TConv>.DistToPoint()
        , "DistToPolytope" => new LvlSetTypes<TNum, TConv>.DistToPolytope()
        , _ => throw new ArgumentException
                 (
                  $"Unsupported level set type: '{lvlSet}'.\nIn file {pr.filePath}\n" +
                  $"Please refer to the documentation for supported types."
                 )
        };

    lvlSetType.ReadParameters(pr, ph);

    return lvlSetType;
  }

}
