using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class ConvexPolytopesFabrics {

    /// <summary>
    /// Generates a full-dimension hypercube in the specified dimension.
    /// </summary>
    /// <param name="cubeDim">The dimension of the hypercube.</param>
    /// <param name="d">The value of non-zero coordinates.</param>
    /// <returns>A convex polytop as VRep representing the hypercube.</returns>
    public static ConvexPolytop Cube(int cubeDim, TNum d) {
      if (cubeDim == 1) {
        HashSet<Vector> oneDimCube = new HashSet<Vector>() { new Vector(new TNum[] { TNum.Zero }), new Vector(new TNum[] { d }) };

        return ConvexPolytop.AsVPolytop(oneDimCube);
      }

      List<List<TNum>> cube_prev = new List<List<TNum>>();
      List<List<TNum>> cube      = new List<List<TNum>>();
      cube_prev.Add(new List<TNum>() { TNum.Zero });
      cube_prev.Add(new List<TNum>() { d });

      for (int i = 1; i < cubeDim; i++) {
        cube.Clear();

        foreach (List<TNum> coords in cube_prev) {
          cube.Add(new List<TNum>(coords) { TNum.Zero });
          cube.Add(new List<TNum>(coords) { d });
        }

        cube_prev = new List<List<TNum>>(cube);
      }

      HashSet<Vector> Cube = new HashSet<Vector>();

      foreach (List<TNum> v in cube) {
        Cube.Add(new Vector(v.ToArray()));
      }

      return ConvexPolytop.AsVPolytop(Cube);
    }

    /// <summary>
    /// Generates a d-simplex in d-space.
    /// </summary>
    /// <param name="simplexDim">The dimension of the simplex.</param>
    /// <returns>A convex polytop as VRep representing the random simplex.</returns>
    public static ConvexPolytop SimplexRND(int simplexDim) {
      GRandomLC random = new GRandomLC();

      HashSet<Vector> simplex = new HashSet<Vector>();
      do {
        for (int i = 0; i < simplexDim + 1; i++) {
          simplex.Add(new Vector(Vector.GenVector(simplexDim, TConv.FromInt(0), TConv.FromInt(10), random)));
        }
      } while (!new AffineBasis(simplex).IsFullDim);

      return ConvexPolytop.AsVPolytop(simplex);
    }

    /// <summary>
    /// Makes the cyclic polytop in specified dimension with specified amount of points.
    /// </summary>
    /// <param name="pDim">The dimension of the cyclic polytop.</param>
    /// <param name="amountOfPoints">The amount of vertices in cyclic polytop.</param>
    /// <param name="step">The step of increasing the moment on the moments curve. init = 1 + step.</param>
    /// <returns>A convex polytop as VRep representing the cyclic polytop.</returns>
    public static ConvexPolytop CyclicPolytop(int pDim, int amountOfPoints, TNum step) {
      Debug.Assert
        (
         amountOfPoints > pDim
       , $"TestPolytopes.CyclicPolytop: The amount of points must be greater than the dimension of the space. Dim = {pDim}, amount = {amountOfPoints}"
        );
      HashSet<Vector> cycP      = new HashSet<Vector>() { new Vector(pDim) };
      TNum         baseCoord = Tools.One + step;
      for (int i = 1; i < amountOfPoints; i++) {
        TNum[] point      = new TNum[pDim];
        TNum   coordinate = baseCoord;
        TNum   multiplyer = coordinate;
        for (int t = 0; t < pDim; t++) { // (i, i^2, i^3 , ... , i^d)
          point[t]   =  coordinate;
          coordinate *= multiplyer;
        }
        cycP.Add(new Vector(point));
        baseCoord += step;
      }

      return ConvexPolytop.AsVPolytop(cycP);
    }

    /// <summary>
    /// Generates a list of Cartesian coordinates for points on a hD-sphere.
    /// </summary>
    /// <param name="dim">The dimension of the sphere. It is greater than 1.</param>
    /// <param name="thetaPoints">The number of points at each zenith angle. Theta in [0, Pi].
    ///  thetaPoints should be greater than 2 for proper calculation.</param>
    /// <param name="phiPoints">The number of points by azimuthal angle. Phi in [0, 2*Pi).</param>
    /// <param name="radius">The radius of a sphere.</param>
    /// <returns>A convex polytop as VRep representing the sphere in hD.</returns>
    public static ConvexPolytop Sphere(int dim, int thetaPoints, int phiPoints, TNum radius) {
      Debug.Assert(dim > 1, "The dimension of a sphere must be 2 or greater.");
      // Phi in [0, 2*Pi)
      // Theta in [0, Pi]
      HashSet<Vector> Ps        = new HashSet<Vector>();
      int             N         = dim - 2;
      TNum            thetaStep = Tools.PI / TConv.FromInt(thetaPoints);
      TNum            phiStep   = Tools.PI2 / TConv.FromInt(phiPoints);

      List<TNum> thetaAll = new List<TNum>();
      for (int i = 0; i <= thetaPoints; i++) {
        thetaAll.Add(thetaStep * TConv.FromInt(i));
      }

      // цикл по переменной [0, 2*Pi)
      for (int i = 0; i < phiPoints; i++) {
        TNum phi = phiStep * TConv.FromInt(i);

        // соберём все наборы углов вида [Phi, t1, t2, t3, ..., t(n-2)]
        // где t_i принимают все возможные свои значения из theta_all
        List<List<TNum>> thetaAngles_prev = new List<List<TNum>>() { new List<TNum>() { phi } };
        List<List<TNum>> thetaAngles      = new List<List<TNum>>() { new List<TNum>() { phi } };
        // сколько раз нужно углы добавлять
        for (int k = 0; k < N; k++) {
          thetaAngles.Clear();
          // формируем наборы добавляя к каждому текущему набору всевозможные углы из theta all
          foreach (List<TNum> angle in thetaAngles_prev) {
            foreach (TNum theta in thetaAll) {
              thetaAngles.Add(new List<TNum>(angle) { theta });
            }
          }
          thetaAngles_prev = new List<List<TNum>>(thetaAngles);
        }

        foreach (List<TNum> s in thetaAngles) {
          List<TNum> point = new List<TNum>();
          // собрали 1 и 2 координаты
          TNum sinsN = Tools.One;
          for (int k = 1; k <= N; k++) { sinsN *= TNum.Sin(s[k]); }
          point.Add(radius * TNum.Cos(phi) * sinsN);
          point.Add(radius * TNum.Sin(phi) * sinsN);


          //добавляем серединные координаты
          if (dim >= 4) { // Их нет для 2Д и 3Д сфер
            TNum sinsJ = Tools.One;
            for (int j = 2; j <= N; j++) {
              sinsJ *= TNum.Sin(s[j - 1]);
              point.Add(radius * TNum.Cos(s[j]) * sinsJ);
            }
          }

          // последнюю координату
          if (dim >= 3) { // У 2Д сферы её нет
            point.Add(radius * TNum.Cos(s[1]));
          }

          // точка готова, добавляем в наш массив
          Ps.Add(new Vector(point.ToArray()));
        }
      }

      return ConvexPolytop.AsVPolytop(Ps);
    }

  }

}
