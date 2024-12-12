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

    Geometry<TNum, TConv>.Vector        point   = Geometry<TNum, TConv>.Vector.Zero(1);
    Geometry<TNum, TConv>.ConvexPolytop polytope = Geometry<TNum, TConv>.ConvexPolytop.Zero();

    switch (epiType) {
      case EpigraphType.DistToPoint:    point   = pr.ReadVector("Point"); break;
      case EpigraphType.DistToPolytope: polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Name"), dh); break;
    }


    TNum k = pr.ReadNumber<TNum>("Constant");
    if (Geometry<TNum, TConv>.Tools.LE(k)) {
      throw new ArgumentException($"Constant must be greater than zero. Found Constant = {k}");
    }

    string ballTypeStr = pr.ReadString("BallType");
    Geometry<TNum, TConv>.BallType ballType =
      ballTypeStr switch
        {
          "Ball_1"  => Geometry<TNum, TConv>.BallType.Ball_1
        , "Ball_2"  => Geometry<TNum, TConv>.BallType.Ball_2
        , "Ball_oo" => Geometry<TNum, TConv>.BallType.Ball_oo
        , _ => throw new ArgumentException
                 (
                  $"Unsupported BallType: '{ballTypeStr}'.\nIn file {pr.filePath}\n" +
                  $" Please refer to the documentation for supported types."
                 )
        };
    int theta = 0, phi = 0;
    if (ballType == Geometry<TNum, TConv>.BallType.Ball_2) {
      theta = pr.ReadNumber<int>("Polar");
      phi   = pr.ReadNumber<int>("Azimuth");
    }
    switch (epiType) {
      case EpigraphType.DistToPoint: {
        yield return
          ballType switch
            {
              Geometry<TNum, TConv>.BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPointBall_1(point, k)
            , Geometry<TNum, TConv>.BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPointBall_2(point, theta, phi, k)
            , Geometry<TNum, TConv>.BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPointBall_oo(point, k)
            };

        break;
      }

      // Множество в виде расстояния до заданного выпуклого многогранника в Rd
      case EpigraphType.DistToPolytope: {
        yield return
          ballType switch
            {
              Geometry<TNum, TConv>.BallType.Ball_1  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_1(polytope, k)
            , Geometry<TNum, TConv>.BallType.Ball_2  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_2(polytope, theta, phi, k)
            , Geometry<TNum, TConv>.BallType.Ball_oo => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_oo(polytope, k)
            };
        break;
      }
    }
  }

  public EpigraphTS(ref Geometry<TNum, TConv>.GameData gd) {
    // Расширяем систему, если решаем задачу с надграфиком функции цены
    gd.projDim++;
    gd.projInd = new List<int>(gd.projInd)
      {
        gd.n
      }.ToArray(); // новая координата всегда в ответе
    gd.n++;        // размерность стала на 1 больше
    gd.A            = Geometry<TNum, TConv>.Matrix.vcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(1, gd.n - 1));
    gd.A            = Geometry<TNum, TConv>.Matrix.hcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(gd.n, 1))!;
    gd.B            = Geometry<TNum, TConv>.Matrix.vcat(gd.B, Geometry<TNum, TConv>.Matrix.Zero(1, gd.pDim));
    gd.C            = Geometry<TNum, TConv>.Matrix.vcat(gd.C, Geometry<TNum, TConv>.Matrix.Zero(1, gd.qDim));
    gd.CauchyMatrix = new Geometry<TNum, TConv>.CauchyMatrix(gd.A, gd.T, gd.dt);
  }

}
