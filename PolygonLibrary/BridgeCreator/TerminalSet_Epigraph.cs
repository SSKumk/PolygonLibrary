using System.Numerics;
using CGLibrary;

namespace BridgeCreator;

public class TerminalSet_Epigraph<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  enum EpigraphType { DistToOrigin, DistToPolytope }

  public Geometry<TNum, TConv>.ConvexPolytop terminalSet;
  public string                              terminalSetInfo = "_Epigraph_";


  public TerminalSet_Epigraph(Geometry<TNum, TConv>.ParamReader pr, int projDim) : base(pr) {
    EpigraphType epiType =
      pr.ReadString("MType") switch
        {
          "DistToOrigin"   => EpigraphType.DistToOrigin
        , "DistToPolytope" => EpigraphType.DistToPolytope
        , _                => throw new ArgumentOutOfRangeException()
        };
    terminalSetInfo += epiType;

    TNum cMax = pr.ReadNumber<TNum>("MCMax");
    terminalSetInfo += $"-CMax{cMax}";

    BallType ballType =
      pr.ReadString("MBallType") switch
        {
          "Ball_1"  => BallType.Ball_1
        , "Ball_2"  => BallType.Ball_2
        , "Ball_oo" => BallType.Ball_oo
        , _         => throw new ArgumentOutOfRangeException()
        };
    terminalSetInfo += ballType;

    int theta = 0, phi = 0;
    if (ballType == BallType.Ball_2) {
      (theta, phi) = ReadBall2Params(pr, ref terminalSetInfo);
    }

    switch (epiType) {
      // Множество в виде расстояния до начала координат
      case EpigraphType.DistToOrigin: {
        terminalSet =
          ballType switch
            {
              BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_1(projDim, cMax)
            , BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_2(projDim, theta, phi, cMax)
            , BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_oo(projDim, cMax)
            , _                => throw new ArgumentOutOfRangeException($"Wrong type of the ball! Found {ballType}")
            };

        break;
      }

      // Множество в виде расстояния до заданного выпуклого многогранника в Rd
      case EpigraphType.DistToPolytope: {
        Geometry<TNum, TConv>.ConvexPolytop polytop = Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(pr);
        terminalSet =
          ballType switch
            {
              BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_1(polytop, cMax)
            , BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_2(polytop, theta, phi, cMax)
            , BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_oo(polytop, cMax)
            , _                => throw new ArgumentException($"Wrong type of the ball! Found {ballType}")
            };

        break;
      }


      // Другие варианты функции платы
      // case :

      default: throw new ArgumentException("TerminalSet_Epigraph: Другие варианты непредусмотрены!");
    }
    terminalSetInfo += $""; //todo: какую инфу по многограннику выдавать-то? М.б. он сам должен это делать?
  }


  public override void Solve(
      string                         baseWorkDir
    , Geometry<TNum, TConv>.GameData gameData
    , int                            projDim
    , int[]                          projInd
    , string                         gameInfoNoTerminalInfo
    , string                         PsInfo
    , string                         QsInfo
    ) {
    // Расширяем систему, если решаем задачу с надграфиком функции цены
    projDim++;
    projInd = new List<int>(projInd) { gameData.n }.ToArray(); // новая координата всегда в ответе
    gameData.n++;                                              // размерность стала на 1 больше
    gameData.A            = Geometry<TNum, TConv>.Matrix.vcat(gameData.A, Geometry<TNum, TConv>.Matrix.Zero(1, gameData.n - 1));
    gameData.A            = Geometry<TNum, TConv>.Matrix.hcat(gameData.A, Geometry<TNum, TConv>.Matrix.Zero(gameData.n, 1))!;
    gameData.B            = Geometry<TNum, TConv>.Matrix.vcat(gameData.B, Geometry<TNum, TConv>.Matrix.Zero(1, gameData.pDim));
    gameData.C            = Geometry<TNum, TConv>.Matrix.vcat(gameData.C, Geometry<TNum, TConv>.Matrix.Zero(1, gameData.qDim));
    gameData.cauchyMatrix = new Geometry<TNum, TConv>.CauchyMatrix(gameData.A, gameData.T, gameData.dt);


    Geometry<TNum, TConv>.SolverLDG sl =
      new Geometry<TNum, TConv>.SolverLDG
      (
       Path.Combine(baseWorkDir, NumericalType, Eps)
     , gameData
     , projDim
     , projInd
     , terminalSet
     , $"{gameInfoNoTerminalInfo}{terminalSetInfo}Epigraph"
     , PsInfo
     , QsInfo
      );
    sl.Solve();
  }

}
