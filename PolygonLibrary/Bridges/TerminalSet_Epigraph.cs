using System.Numerics;
using CGLibrary;

namespace Bridges;

public class TerminalSet_Epigraph<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  enum EpigraphType { DistToOrigin, DistToPolytope }

  public Geometry<TNum, TConv>.ConvexPolytop terminalSet;
  public string                              terminalSetInfo = "_Epigraph_";


  public TerminalSet_Epigraph(Geometry<TNum, TConv>.ParamReader pr, Geometry<TNum, TConv>.GameData gameData) : base
    (pr, gameData) {
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
              BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_1(gd.projDim, cMax)
            , BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_2(gd.projDim, theta, phi, cMax)
            , BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_oo(gd.projDim, cMax)
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


  public override void DoSolve(string baseWorkDir) {
    // Расширяем систему, если решаем задачу с надграфиком функции цены
    gd.projDim++;
    gd.projInd = new List<int>(gd.projInd) { gd.n }.ToArray(); // новая координата всегда в ответе
    gd.n++;                                                    // размерность стала на 1 больше
    gd.A            = Geometry<TNum, TConv>.Matrix.vcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(1, gd.n - 1));
    gd.A            = Geometry<TNum, TConv>.Matrix.hcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(gd.n, 1))!;
    gd.B            = Geometry<TNum, TConv>.Matrix.vcat(gd.B, Geometry<TNum, TConv>.Matrix.Zero(1, gd.pDim));
    gd.C            = Geometry<TNum, TConv>.Matrix.vcat(gd.C, Geometry<TNum, TConv>.Matrix.Zero(1, gd.qDim));
    gd.CauchyMatrix = new Geometry<TNum, TConv>.CauchyMatrix(gd.A, gd.T, gd.dt);


    Geometry<TNum, TConv>.SolverLDG sl = new Geometry<TNum, TConv>.SolverLDG(baseWorkDir, gd, terminalSet, $"{terminalSetInfo}");
    sl.Solve();
  }

}
