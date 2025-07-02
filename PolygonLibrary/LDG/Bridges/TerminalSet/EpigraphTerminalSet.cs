namespace LDG;

/// <summary>
/// Represents the terminal set reader for the "Epigraph" type, which builds terminal sets based on the epigraph of a function.
/// It modifies the game data when constructing the terminal sets.
///</summary>
public class EpigraphTerminalSet<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Builds the terminal sets based on the provided configuration.
  /// </summary>
  /// <param name="pr">The parameter reader used to extract parameters.</param>
  /// <param name="ph">The path holder providing access to polytope files. Used by epigraph types that rely on explicit polytopes.</param>
  /// <returns>A collection of terminal sets -- ConvexPolytop.</returns>
  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> BuildTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>        ph
    ) {
    EpiTypeBase<TNum, TConv> epiTypeBase = EpiTypeFactory<TNum, TConv>.Read(pr, ph);

    yield return epiTypeBase.BuildEpigraph();
  }

  /// <summary>
  /// Extends the <paramref name="gd"/> (game data) system when the terminal set is an epigraph of a function.
  /// </summary>
  /// <param name="gd">The game data to be modified. Its dimension 'n' will be incremented,
  /// and system matrices will be augmented accordingly.</param>
  public EpigraphTerminalSet(ref GameData<TNum, TConv> gd) {
    // Расширяем систему, если решаем задачу с надграфиком функции цены
    gd.ProjDim++;
    gd.ProjInd = new List<int>(gd.ProjInd) { gd.n }.ToArray(); // новая координата всегда в ответе
    gd.n++;                                                    // размерность стала на 1 больше
    gd.A            = Geometry<TNum, TConv>.Matrix.vcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(1, gd.n - 1));
    gd.A            = Geometry<TNum, TConv>.Matrix.hcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(gd.n, 1))!;
    gd.B            = Geometry<TNum, TConv>.Matrix.vcat(gd.B, Geometry<TNum, TConv>.Matrix.Zero(1, gd.pDim));
    gd.C            = Geometry<TNum, TConv>.Matrix.vcat(gd.C, Geometry<TNum, TConv>.Matrix.Zero(1, gd.qDim));
    gd.CauchyMatrix = new Geometry<TNum, TConv>.CauchyMatrix(gd.A, gd.T, gd.dt);


    // Setting up the projection matrix
    TNum[,] projMatrixArr = new TNum[gd.ProjDim, gd.n];
    for (int i = 0; i < gd.ProjDim; i++) {
      projMatrixArr[i, gd.ProjInd[i]] = Geometry<TNum, TConv>.Tools.One;
    }
    gd.ProjMatrix = new Geometry<TNum, TConv>.Matrix(projMatrixArr);

    gd._Xstar = new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>();

    TNum t = gd.t0;
    do {
      gd.D[t] = gd.Xstar(t) * gd.B;
      gd.E[t] = gd.Xstar(t) * gd.C;

      t += gd.dt;
    } while (Geometry<TNum, TConv>.Tools.LE(t, gd.T));
  }

}
