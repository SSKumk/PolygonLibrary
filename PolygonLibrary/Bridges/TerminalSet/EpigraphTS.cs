namespace Bridges;

public class EpigraphTS<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  private enum EpigraphType { DistToPoint, DistToPolytope }


  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> ReadTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>        dh
    , Geometry<TNum, TConv>.GameData    gd
    ) {
    string type = pr.ReadString("Type");
    EpigraphType epiType =
      type switch
        {
          "DistToPoint"    => EpigraphType.DistToPoint
        , "DistToPolytope" => EpigraphType.DistToPolytope
        , _ => throw new ArgumentException
                 (
                  $"Unsupported epigraph type: '{type}'.\nIn file {pr.filePath}\n" +
                  $"Please refer to the documentation for supported types."
                 )
        };

    Geometry<TNum, TConv>.Vector        point;
    Geometry<TNum, TConv>.ConvexPolytop polytop;

    switch (epiType) {
      case EpigraphType.DistToPoint:    point   = pr.ReadVector("Point"); break;
      case EpigraphType.DistToPolytope: polytop = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Name"), dh); break;
    }


    TNum k = pr.ReadNumber<TNum>("Constant");
    if (Geometry<TNum, TConv>.Tools.LE(k)) {
      throw new ArgumentException($"Constant must be greater than zero. Found Constant = {k}");
    }

    string ballTypeStr = pr.ReadString("BallType");
    ITerminalSetReader<TNum, TConv>.BallType ballType =
      ballTypeStr switch
        {
          "Ball_1"  => ITerminalSetReader<TNum, TConv>.BallType.Ball_1
        , "Ball_2"  => ITerminalSetReader<TNum, TConv>.BallType.Ball_2
        , "Ball_oo" => ITerminalSetReader<TNum, TConv>.BallType.Ball_oo
        , _ => throw new ArgumentException
                 (
                  $"Unsupported BallType: '{ballTypeStr}'.\nIn file {pr.filePath}\n" +
                  $" Please refer to the documentation for supported types."
                 )
        };
    int theta = 0, phi = 0;
    if (ballType == ITerminalSetReader<TNum, TConv>.BallType.Ball_2) {
      theta = pr.ReadNumber<int>("Polar");
      phi   = pr.ReadNumber<int>("Azimuth");
    }
    switch (epiType) {
      case EpigraphType.DistToPoint: {
        yield return ballType switch
                       {
                         ITerminalSetReader<TNum, TConv>.BallType.Ball_1 => Geometry<TNum, TConv>.ConvexPolytop
                                                                                                 .DistanceToPointBall_1
                                                                                                    (TODO, k)
                       , ITerminalSetReader<TNum, TConv>.BallType.Ball_2 => Geometry<TNum, TConv>.ConvexPolytop
                                                                                                 .DistanceToOriginBall_2
                                                                                                    (gd.projDim, theta, phi, k)
                       , ITerminalSetReader<TNum, TConv>.BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop
                                                                                                  .DistanceToOriginBall_oo
                                                                                                     (gd.projDim, k)
                       , _ => throw new ArgumentOutOfRangeException($"Wrong type of the ball! Found {ballType}")
                       };

        break;
      }

      // Множество в виде расстояния до заданного выпуклого многогранника в Rd
      case EpigraphType.DistToPolytope: {
        Geometry<TNum, TConv>.ConvexPolytop polytop = Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(pr);
        terminalSet =
          ballType switch
            {
              ITerminalSetReader<TNum, TConv>.BallType.Ball_1 => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_1
                (polytop, k)
            , ITerminalSetReader<TNum, TConv>.BallType.Ball_2 => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_2
                (polytop, theta, phi, k)
            , ITerminalSetReader<TNum, TConv>.BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_oo
                (polytop, k)
            , _ => throw new ArgumentException($"Wrong type of the ball! Found {ballType}")
            };

        //todo: какую инфу по многограннику выдавать-то? М.б. он сам должен это делать?
        break;
      }


      // Другие варианты функции платы
      // case :

      default: throw new ArgumentException("TerminalSet_Epigraph: Другие варианты непредусмотрены!");
    }
  }

  public EpigraphTS(ref Geometry<TNum, TConv>.GameData gd) {
    // Расширяем систему, если решаем задачу с надграфиком функции цены
    gd.projDim++;
    gd.projInd = new List<int>(gd.projInd) { gd.n }.ToArray(); // новая координата всегда в ответе
    gd.n++;                                                    // размерность стала на 1 больше
    gd.A            = Geometry<TNum, TConv>.Matrix.vcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(1, gd.n - 1));
    gd.A            = Geometry<TNum, TConv>.Matrix.hcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(gd.n, 1))!;
    gd.B            = Geometry<TNum, TConv>.Matrix.vcat(gd.B, Geometry<TNum, TConv>.Matrix.Zero(1, gd.pDim));
    gd.C            = Geometry<TNum, TConv>.Matrix.vcat(gd.C, Geometry<TNum, TConv>.Matrix.Zero(1, gd.qDim));
    gd.CauchyMatrix = new Geometry<TNum, TConv>.CauchyMatrix(gd.A, gd.T, gd.dt);
  }

}
