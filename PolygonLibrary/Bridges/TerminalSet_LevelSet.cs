namespace Bridges;

public class TerminalSet_LevelSet<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public enum LevelSetType { DistToOrigin, DistToPolytope }

  public          Geometry<TNum, TConv>.ConvexPolytop terminalSet;

  public readonly TNum[] cs; // коэффициенты

  // public readonly List<Geometry<TNum, TConv>.ConvexPolytop> levelSets = new List<Geometry<TNum, TConv>.ConvexPolytop>();
  public readonly LevelSetType levelSetType;
  public readonly BallType     ballType;


  private readonly int                                 _theta = 0;
  private readonly int                                 _phi   = 0;
  private readonly Geometry<TNum, TConv>.ConvexPolytop _polytop;

  public TerminalSet_LevelSet(Geometry<TNum, TConv>.ParamReader pr, Geometry<TNum, TConv>.GameData gameData, string tsInfo) : base
    (pr, gameData,tsInfo) {
    int qnt = pr.ReadNumber<int>("CQnt");
    cs = pr.Read1DArray<TNum>("Constants", qnt);

    levelSetType =
      pr.ReadString("MType") switch
        {
          "DistToOrigin"   => LevelSetType.DistToOrigin
        , "DistToPolytope" => LevelSetType.DistToPolytope
        , _                => throw new ArgumentOutOfRangeException()
        };
    tsInfo += levelSetType;

    ballType =
      pr.ReadString("MBallType") switch
        {
          "Ball_1"  => BallType.Ball_1
        , "Ball_2"  => BallType.Ball_2
        , "Ball_oo" => BallType.Ball_oo
        , _         => throw new ArgumentOutOfRangeException()
        };
    tsInfo += ballType;

    if (ballType == BallType.Ball_2) {
      (_theta, _phi) = ReadBall2Params(pr, ref tsInfo);
    }

    terminalSetInfo += tsInfo;

    if (levelSetType == LevelSetType.DistToPolytope) {
      _polytop = Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(pr);
    }
  }

  public override void DoSolve(string baseWorkDir) {
    foreach (TNum num in cs) {
      string tsInfo = $"_{cs}_";

      switch (levelSetType) {
        // Множество в виде расстояния до начала координат
        case LevelSetType.DistToOrigin: {
          terminalSet =
            ballType switch
              {
                BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_1(gd.projDim - 1, num)
              , BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_2(gd.projDim - 1, _theta, _phi, num)
              , BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_oo(gd.projDim - 1, num)
              , _                => throw new ArgumentOutOfRangeException($"Wrong type of the ball! Found {ballType}")
              };

          break;
        }

        // Множество в виде расстояния до заданного выпуклого многогранника в Rd
        case LevelSetType.DistToPolytope: {
          terminalSet =
            ballType switch
              {
                BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_1(_polytop, num)
              , BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_2(_polytop, _theta, _phi, num)
              , BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_oo(_polytop, num)
              , _                => throw new ArgumentException($"Wrong type of the ball! Found {ballType}")
              };

          break;
        }


        // Другие варианты функции платы
        // case :

        default: throw new ArgumentException("TerminalSet_Epigraph: Другие варианты непредусмотрены!");
      }
      tsInfo += $""; //todo: какую инфу по многограннику выдавать-то? М.б. он сам должен это делать?


      Geometry<TNum, TConv>.SolverLDG sl =
        new Geometry<TNum, TConv>.SolverLDG
          (
           Path.Combine(baseWorkDir, TConv.ToDouble(num).ToString("G"))
         , gd
         , terminalSet
         , $"{terminalSetInfo}{tsInfo}"
          );
      sl.Solve();
    }
  }

}
