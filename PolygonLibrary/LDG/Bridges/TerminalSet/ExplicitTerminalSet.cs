namespace LDG;

/// <summary>
/// Represents a terminal set reader for the "Explicit" type, which builds terminal sets based on explicitly provided polytopes.
/// </summary>
public class ExplicitTerminalSet<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Builds terminal sets based on explicitly defined polytopes by reading the polytope names from the parameter reader.
  /// </summary>
  /// <param name="pr">The parameter reader used to extract the polytope names from the terminal set configuration file..</param>
  /// <param name="ph">The path holder used to access polytope files.</param>
  /// <returns>A collection of terminal sets based on the explicitly defined polytopes.</returns>
  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> BuildTerminalSets(
    Geometry<TNum, TConv>.ParamReader pr
  , LDGPathHolder<TNum, TConv>        ph) {
    int      k     = pr.ReadNumber<int>("Qnt");
    string[] names = pr.Read1DArray<string>("Polytopes", k);


    //todo: СДЕЛАТЬ ТАК, ЧТОБЫ В ОТВЕТ МНОЖЕСТВА ШЛИ ПО ВКЛЮЧЕНИЮ!

    return names.Select(name => ITerminalSetReader<TNum, TConv>.DoPolytope(name, ph));
  }
}
