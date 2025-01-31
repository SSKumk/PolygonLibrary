namespace LDG;

/// <summary>
/// Class for keeping game parameter data.
/// </summary>
public class GameData<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {
#region Input data
#region Data defining the dynamics of the game
  /// <summary>
  /// Dimension of the phase vector.
  /// </summary>
  public int n;

  /// <summary>
  /// The main matrix that defines the dynamics of the game.
  /// </summary>
  public Geometry<TNum, TConv>.Matrix A;

  /// <summary>
  /// Dimension of the useful control.
  /// </summary>
  public readonly int pDim;

  /// <summary>
  /// The useful control matrix.
  /// </summary>
  public Geometry<TNum, TConv>.Matrix B;

  /// <summary>
  /// Dimension of the disturbance.
  /// </summary>
  public readonly int qDim;

  /// <summary>
  /// The disturbance matrix.
  /// </summary>
  public Geometry<TNum, TConv>.Matrix C;

  /// <summary>
  /// The initial instant (starting time).
  /// </summary>
  public readonly TNum t0;

  /// <summary>
  /// The final instant (ending time).
  /// </summary>
  public readonly TNum T;

  /// <summary>
  /// The time step for the integration.
  /// </summary>
  public readonly TNum dt;

  /// <summary>
  /// The dimension of the selected projection coordinates.
  /// </summary>
  public int ProjDim;

  /// <summary>
  /// Indices of the selected projection coordinates.
  /// </summary>
  public int[] ProjInd;
#endregion


#region Control constraints
  /// <summary>
  /// The convex polytope representing the control set for the first player.
  /// </summary>
  public readonly Geometry<TNum, TConv>.ConvexPolytop P;

  /// <summary>
  /// The convex polytope representing the control set for the second player.
  /// </summary>
  public readonly Geometry<TNum, TConv>.ConvexPolytop Q;
#endregion
#endregion

#region Matrices related to the system
  /// <summary>
  /// Collection of matrices D for the instants from the time grid.
  /// </summary>
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> D =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>(Geometry<TNum, TConv>.Tools.TComp);

  /// <summary>
  /// Collection of matrices E for the instants from the time grid.
  /// </summary>
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> E =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>(Geometry<TNum, TConv>.Tools.TComp);

  /// <summary>
  /// The fundamental Cauchy matrix of the corresponding system.
  /// </summary>
  public Geometry<TNum, TConv>.CauchyMatrix CauchyMatrix;

  /// <summary>
  /// Projection matrix, which extracts the necessary rows of the Cauchy matrix.
  /// </summary>
  public Geometry<TNum, TConv>.Matrix ProjMatrix;

  /// <summary>
  /// Returns the Xstar matrix for a given time <paramref name="t"/>.
  /// </summary>
  /// <param name="t">The time at which to compute the matrix.</param>
  /// <returns>The computed Xstar matrix.</returns>
  public Geometry<TNum, TConv>.Matrix Xstar(TNum t) {
    if (_Xstar.TryGetValue(t, out Geometry<TNum, TConv>.Matrix? matrix)) {
      return matrix;
    }

    Geometry<TNum, TConv>.Matrix projMatrix = ProjMatrix * CauchyMatrix[t];
    _Xstar.Add(t, projMatrix);

    return projMatrix;
  }
  /// <summary>
  /// A sorted dictionary that stores computed projection matrices for different time instants (t).
  /// The key is the time instant, and the value is the corresponding projection matrix for that time.
  /// </summary>
  internal SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> _Xstar =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>(Geometry<TNum, TConv>.Tools.TComp);
#endregion

#region Constructor
  /// <summary>
  /// Initializes a new instance of the <see cref="GameData{TNum,TConv}"/> class by reading and setting all required parameters.
  /// The order of reading is critical for correct initialization.
  /// </summary>
  /// <param name="prDyn">The reader for dynamic parameters.</param>
  /// <param name="prP">The reader for the first player's control parameters.</param>
  /// <param name="prQ">The reader for the second player's control parameters.</param>
  /// <param name="trP">The reader that reads the transformation for the first player's control set <paramref name="P"/>.</param>
  /// <param name="trQ">The reader that reads the transformation for the second player's control set <paramref name="Q"/>.</param>
  public GameData(
      Geometry<TNum, TConv>.ParamReader      prDyn
    , Geometry<TNum, TConv>.ParamReader      prP
    , Geometry<TNum, TConv>.ParamReader      prQ
    , TransformReader<TNum, TConv>.Transform trP
    , TransformReader<TNum, TConv>.Transform trQ
    ) {
    // Reading dynamics
    n    = prDyn.ReadNumber<int>("Dim");
    A    = new Geometry<TNum, TConv>.Matrix(prDyn.Read2DArray<TNum>("A", n, n));
    pDim = prDyn.ReadNumber<int>("PDim");
    B    = new Geometry<TNum, TConv>.Matrix(prDyn.Read2DArray<TNum>("B", n, pDim));
    qDim = prDyn.ReadNumber<int>("QDim");
    C    = new Geometry<TNum, TConv>.Matrix(prDyn.Read2DArray<TNum>("C", n, qDim));

    t0 = prDyn.ReadNumber<TNum>("t0");
    T  = prDyn.ReadNumber<TNum>("T");
    dt = prDyn.ReadNumber<TNum>("dt");

    // Reading projection information
    ProjDim = prDyn.ReadNumber<int>("ProjDim");
    ProjInd = prDyn.Read1DArray<int>("ProjInd", ProjDim);

    // Reading the first player's control data
    P = TransformReader<TNum, TConv>.DoTransform(PolytopeReader<TNum, TConv>.Read(prP), trP, pDim);
    // Reading the second player's control data
    Q = TransformReader<TNum, TConv>.DoTransform(PolytopeReader<TNum, TConv>.Read(prQ), trQ, qDim);

    // Computing the Cauchy matrix
    CauchyMatrix = new Geometry<TNum, TConv>.CauchyMatrix(A, T, dt);

    // Setting up the projection matrix
    TNum[,] projMatrixArr = new TNum[ProjDim, n];
    for (int i = 0; i < ProjDim; i++) {
      projMatrixArr[i, ProjInd[i]] = Geometry<TNum, TConv>.Tools.One;
    }
    ProjMatrix = new Geometry<TNum, TConv>.Matrix(projMatrixArr);


    TNum t = t0;
    do {
      D[t] = Xstar(t) * B;
      E[t] = Xstar(t) * C;

      t += dt;
    } while (Geometry<TNum, TConv>.Tools.LE(t, T));
  }
#endregion

}
