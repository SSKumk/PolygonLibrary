using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// The GiftWrapping class represents a gift wrapping algorithm for convex polytopes.
  /// </summary>
  public class GiftWrapping {

    /// <summary>
    /// The convex polytope obtained as a result of gift wrapping algorithm.
    /// </summary>
    private readonly BaseSubCP BuiltPolytop;

    /// <summary>
    /// The original set of points.
    /// </summary>
    private HashSet<Point> SOrig;

    /// <summary>
    /// The convex polytope.
    /// </summary>
    private ConvexPolytop? _polytop;

    public ConvexPolytop ConvexPolytop => _polytop ??= GetPolytop();

    /// <summary>
    /// The vertices of the polytope.
    /// </summary>
    public HashSet<Point> Vertices => ConvexPolytop.Vertices;
    // public HashSet<Point> Vertices => BuiltPolytop.OriginalVertices;


    /// <summary>
    /// Polytop as intersection of negative half-space of hyperplanes.
    /// </summary>
    private List<HyperPlane>? _HRepr;

    public List<HyperPlane> HRepresentation {
      get
        {
          if (_HRepr is null) {
            List<HyperPlane> res = new List<HyperPlane>();
            foreach (Face facet in ConvexPolytop.Faces) {
              res.Add(new HyperPlane(facet.Vertices.First(), facet.Normal));
            }
            _HRepr = res;
          }

          return _HRepr;
        }
    }

    /// <summary>
    /// Polytop as list of its vertices.
    /// </summary>
    private List<Point>? _VRepr;

    public List<Point> VerticesList => _VRepr ??= BuiltPolytop.OriginalVertices.ToList();


    /// <summary>
    /// Initializes a new instance of the GiftWrapping class.
    /// The class constructs a convex hull of the swarm during initialization.
    /// </summary>
    /// <param name="Swarm">The swarm of points to convexify.</param>
    public GiftWrapping(IEnumerable<Point> Swarm) {
      SOrig = new HashSet<Point>(Swarm);
      GiftWrappingMain x = new GiftWrappingMain(new HashSet<SubPoint>(Swarm.Select(s => new SubPoint(s, null, s))));
      BuiltPolytop = x.BuiltPolytop;
    }

    /// <summary>
    /// Builds the convex polytop using 'builtPolytop' property.
    /// </summary>
    /// <returns>The convex polytop.</returns>
    private ConvexPolytop GetPolytop() {
      Debug.Assert(BuiltPolytop is not null, "GiftWrapping.GetPolytop(): built polytop is null!");
      HashSet<Face> Fs = new HashSet<Face>(BuiltPolytop.Faces!.Select(F => new Face(F.OriginalVertices, F.Normal!)));
      HashSet<Edge> Es = new HashSet<Edge>();

      foreach (BaseSubCP face in BuiltPolytop.Faces!) {
        Es.UnionWith(face.Faces!.Select(F => new Edge(F.OriginalVertices)));
      }

      ConvexPolyhedronType type;

      switch (BuiltPolytop.Type) {
        case SubCPType.Simplex:
          type = ConvexPolyhedronType.Simplex;

          break;
        case SubCPType.NonSimplex:
          type = ConvexPolyhedronType.NonSimplex;

          break;

        case SubCPType.TwoDimensional:
        case SubCPType.OneDimensional:
        default: throw new NotImplementedException("Can not build ConvexPolytop from One or Two dimensional!");
      }

      return new ConvexPolytop
        (
         BuiltPolytop.OriginalVertices
       , BuiltPolytop.PolytopDim
       , Fs
       , Es
       , type
       , new IncidenceInfo(BuiltPolytop.FaceIncidence!)
       , new FansInfo(BuiltPolytop.Faces!)
        );
    }


    /// <summary>
    /// Perform an gift wrapping algorithm. It holds a necessary fields to construct a d-polytop.
    /// </summary>
    private sealed class GiftWrappingMain {

#region Internal fields for GW algorithm
      private readonly HashSet<SubPoint> S;
      private readonly int               spaceDim;
      private          BaseSubCP?        initFace;

      private readonly HashSet<BaseSubCP> buildFaces     = new HashSet<BaseSubCP>();
      private readonly HashSet<SubPoint>  buildPoints    = new HashSet<SubPoint>();
      private readonly TempIncidenceInfo  buildIncidence = new TempIncidenceInfo();

      /// <summary>
      /// The resulted d-polytop in d-space. It holds information about face incidence, vertex -> face incidence,
      /// about (d-1)-faces, vertices and information about all k-faces, 0 &lt; k &lt; d - 1.
      /// </summary>
      public readonly BaseSubCP BuiltPolytop;
#endregion

#region Constructors
      public GiftWrappingMain(HashSet<SubPoint> Swarm, BaseSubCP? initFace = null) {
        S             = Swarm;
        spaceDim      = S.First().Dim;
        this.initFace = initFace;

        BuiltPolytop = GW();
      }
#endregion

#region Main functions
      /// <summary>
      /// Executes the gift wrapping algorithm.
      /// </summary>
      /// <returns>A built polytop.</returns>
      private BaseSubCP GW() {
        if (spaceDim == 2) {
          List<Point2D> convexPolygon2D = Convexification.GrahamHull(S.Select(s => new SubPoint2D(s)));

          if (convexPolygon2D.Count == 3) {
            return new SubSimplex(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
          }

          return new SubTwoDimensional(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
        }


        initFace ??= BuildFace(BuildInitialPlane(out Vector normal), normal);
        buildFaces.Add(initFace);
        buildPoints.UnionWith(initFace.Vertices);

#region Debug
        Debug.Assert(initFace is not null, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet is null!");
        Debug.Assert(initFace.Normal is not null, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet is null.");
        Debug.Assert(!initFace.Normal.IsZero, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet has zero length.");
        Debug.Assert
          (
           initFace.SpaceDim == spaceDim
         , $"GiftWrapping.GW (space dim = {spaceDim}): The initFace must lie in d-dimensional space!"
          );
        Debug.Assert
          (
           initFace.Faces!.All(F => F.SpaceDim == spaceDim)
         , $"GiftWrapping.GW (space dim = {spaceDim}): All edges of the initFace must lie in d-dimensional space!"
          );
        Debug.Assert
          (
           initFace.PolytopDim == spaceDim - 1
         , $"GiftWrapping.GW (space dim = {spaceDim}): The dimension of the initFace must equals to d-1!"
          );
        Debug.Assert
          (
           initFace.Faces!.All(F => F.PolytopDim == spaceDim - 2)
         , $"GiftWrapping.GW (space dim = {spaceDim}): The dimension of all edges of initFace must equal to d-2!"
          );
#endregion

        Queue<BaseSubCP> toTreat = new Queue<BaseSubCP>();
        toTreat.Enqueue(initFace);
        foreach (BaseSubCP edge in initFace.Faces!) {
          buildIncidence.Add(edge, (initFace, null));
        }

        do {
          BaseSubCP face = toTreat.Dequeue();

          foreach (BaseSubCP edge in face.Faces!.Where(edge => buildIncidence[edge].F2 is null)) {
            BaseSubCP nextFace = RollOverEdge(face, edge, out Vector n);
            nextFace.Normal = n;

            buildFaces.Add(nextFace);
            buildPoints.UnionWith(nextFace.Vertices);

            bool hasFreeEdges = false;
            foreach (BaseSubCP newEdge in nextFace.Faces!) {
              if (buildIncidence.TryGetValue(newEdge, out (BaseSubCP F1, BaseSubCP? F2) E)) {
                buildIncidence[newEdge] = E.F1.GetHashCode() <= nextFace.GetHashCode() ? (E.F1, nextFace) : (nextFace, E.F1);
              } else {
                buildIncidence.Add(newEdge, (nextFace, null));
                hasFreeEdges = true;
              }
            }
            if (hasFreeEdges) {
              toTreat.Enqueue(nextFace);
            }
          }
        } while (toTreat.Count != 0);

        // Подготавливаем и собираем многогранник из накопленных граней
        SubIncidenceInfo incidence = new SubIncidenceInfo();
        foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in buildIncidence) {
          incidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
        }
        if (buildFaces.Count == spaceDim + 1 && buildFaces.All(F => F.Type == SubCPType.Simplex)) {
          return new SubSimplex(buildPoints, buildFaces, incidence);
        }

        return new SubNonSimplex(buildFaces, incidence, buildPoints);
      }

      /// <summary>
      /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
      /// and all other points lies for a one side from it.
      /// </summary>
      /// <param name="normal">The outward normal to the initial plane.</param>
      /// <returns>
      /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
      /// </returns>
      private AffineBasis BuildInitialPlane(out Vector normal) {
        Debug.Assert(S.Any(), $"BuildInitialPlaneSwart (dim = {spaceDim}): The swarm must has at least one point!");

        SubPoint    origin = S.Min(p => p)!;
        AffineBasis FinalV = new AffineBasis(origin);

        Vector n = -Vector.CreateOrth(spaceDim, 1);

        while (FinalV.SpaceDim < spaceDim - 1) {
          TNum      maxAngle = -Tools.Six; // "Большое отрицательное число."
          SubPoint? sExtr    = null;

          Vector   e;
          int      i      = 0;
          Vector[] nBasis = new[] { n };
          do {
            i++;
            e = Vector.OrthonormalizeAgainstBasis(Vector.CreateOrth(spaceDim, i), FinalV.Basis, nBasis);
          } while (e.IsZero && i <= spaceDim);
          Debug.Assert
            (i <= spaceDim, $"BuildInitialPlaneSwart (dim = {spaceDim}): Can't find vector e! That orthogonal to FinalV and n.");

          Vector? r = null;
          foreach (SubPoint s in S) {
            Vector u = ((s - origin) * e) * e + ((s - origin) * n) * n;

            if (!u.IsZero) {
              TNum angle = Vector.Angle(e, u);

              if (Tools.GT(angle, maxAngle)) {
                maxAngle = angle;
                sExtr    = s;
                r        = u.Normalize();
              }
            }
          }

          if (sExtr is null) { // Что делать если выпуклая оболочка не полной размерности?
            throw new ArgumentException($"GW.BIP (dim = {spaceDim}): All points of S lies in initial plane!");

            // normal = n;

            // return FinalV;
          }

          bool isAdded = FinalV.AddVectorToBasis(sExtr - origin);

          Debug.Assert
            (
             isAdded
           , $"BuildInitialPlaneSwart (dim = {spaceDim}): The new vector of FinalV is linear combination of FinalV vectors!"
            );

          // НАЧАЛЬНАЯ Нормаль Наша (точная)
          i = 0;
          do {
            i++;
            n = Vector.OrthonormalizeAgainstBasis(Vector.CreateOrth(spaceDim, i), FinalV.Basis);
          } while (n.IsZero && i <= spaceDim);

          //НАЧАЛЬНАЯ Нормаль по Сварту
          // n = (r! * n) * e - (r! * e) * n;

          OrientNormal(ref n, origin);
          if (S.All(s => new HyperPlane(origin, n).Contains(s))) {
            throw new ArgumentException
              (
               $"BuildInitialPlaneSwart (dim = {spaceDim}): All points from S lies in initial plane! There are no convex hull of full dimension."
              );
          }

          Debug.Assert(!n.IsZero, $"BuildInitialPlaneSwart (dim = {spaceDim}): Normal is zero!");
        }

        normal = n;

        return FinalV;
      }

      /// <summary>
      /// Builds next face of a polytop.
      /// </summary>
      /// <param name="FaceBasis">The basis of (d-1)-dimensional subspace in terms of d-space.</param>
      /// <param name="n"></param>
      /// <param name="r"></param>
      /// <param name="initEdge">The (d-2)-dimensional edge in terms of d-space.</param>
      /// <returns>
      /// The BaseSubCP: (d-1)-dimensional polytop complex expressed in terms of d-dimensional points.
      /// </returns>
      private BaseSubCP BuildFace(AffineBasis FaceBasis, Vector n, Vector? r = null, BaseSubCP? initEdge = null) {
        Debug.Assert
          (FaceBasis.SpaceDim == spaceDim - 1, $"BuildFace (dim = {spaceDim}): The basis must lie in (d-1)-dimensional space!");


        if (initEdge is not null) {
          Debug.Assert
            (
             initEdge.PolytopDim == spaceDim - 2
           , $"BuildFace (dim = {S.First().Dim}): The dimension of the initial edge must equal to (d-2)!"
            );

          initEdge.Normal = r;
        }


        HyperPlane hp = new HyperPlane(FaceBasis.Origin, n);
        HashSet<SubPoint> inPlane = S.Where(s => hp.Contains(s))
                                     .Select(s => new SubPoint(s.ProjectTo(FaceBasis), s, s.Original))
                                     .ToHashSet();

        Debug.Assert(inPlane.Count >= spaceDim, $"BuildFace (dim = {spaceDim}): In plane must be at least d points!");

        if (inPlane.Count == FaceBasis.VecDim) {
          BaseSubCP buildedSimplex = new SubSimplex(inPlane.Select(p => p.Parent!));
          buildedSimplex.Normal = n;

          return buildedSimplex;
        }

        BaseSubCP? prj = initEdge?.ProjectTo(FaceBasis);
        if (prj is not null) { //
          prj.Normal = Vector.CreateOrth(FaceBasis.SpaceDim, FaceBasis.SpaceDim);
        }

        BaseSubCP buildedFace = new GiftWrappingMain(inPlane, prj).BuiltPolytop.ToPreviousSpace();
        buildedFace.Normal = n;

        HashSet<SubPoint> toRemove = new HashSet<SubPoint>(inPlane.Select(s => s.Parent).ToHashSet()!);
        toRemove.ExceptWith(buildedFace.Vertices);
        S.ExceptWith(toRemove);

        return buildedFace;
      }

      /// <summary>
      /// Rolls through a given edge from a given face to a new face and returns its normal vector.
      /// </summary>
      /// <param name="face">(d-1)-dimensional face in d-dimensional space.</param>
      /// <param name="edge">(d-2)-dimensional edge in d-dimensional space.</param>
      /// <param name="n"></param>
      /// <returns>(d-1)-dimensional face in d-dimensional space which incident to the face by the edge.</returns>
      private BaseSubCP RollOverEdge(BaseSubCP face, BaseSubCP edge, out Vector n) {
        Debug.Assert(face.SpaceDim == spaceDim, $"RollOverEdge (dim = {spaceDim}): The face must lie in d-dimensional space!");
        Debug.Assert
          (
           face.Faces!.All(F => F.SpaceDim == spaceDim)
         , $"RollOverEdge (dim = {spaceDim}): All edges of the face must lie in d-dimensional space!"
          );
        Debug.Assert
          (face.PolytopDim == spaceDim - 1, $"RollOverEdge (dim = {spaceDim}): The dimension of the face must equal to d-1!");
        Debug.Assert
          (edge.PolytopDim == spaceDim - 2, $"RollOverEdge (dim = {spaceDim}): The dimension of the edge must equal to d-2!");
        Debug.Assert(face.Normal is not null, $"RollOverEdge (dim = {spaceDim}): face.Normal is null");
        Debug.Assert(!face.Normal.IsZero, $"RollOverEdge (dim = {spaceDim}): face.Normal has zero length");

        AffineBasis edgeBasis = new AffineBasis(edge.Vertices);
        SubPoint    f         = face.Vertices.First(p => !edge.Vertices.Contains(p));
        Vector      v         = Vector.OrthonormalizeAgainstBasis(f - edgeBasis.Origin, edgeBasis.Basis);

        Vector?   r        = null;
        SubPoint? sStar    = null;
        TNum      maxAngle = -Tools.Six; // Something small

        foreach (SubPoint s in S) {
          Vector so = s - edgeBasis.Origin;
          Vector u  = (so * v) * v + (so * face.Normal) * face.Normal;

          if (!u.IsZero) {
            TNum angle = Vector.Angle(v, u);

            if (Tools.GT(angle, maxAngle)) {
              maxAngle = angle;
              r        = u.Normalize();
              sStar    = s;
            }
          }
        }

        AffineBasis newF_aBasis = new AffineBasis(edgeBasis);
        newF_aBasis.AddVectorToBasis(r!, false);

        Debug.Assert
          (
           newF_aBasis.SpaceDim == face.PolytopDim
         , $"RollOverEdge (dim = {spaceDim}): The dimension of the basis of new F' must equals to F dimension!"
          );


        List<SubPoint> newPlane = new List<SubPoint>(edge.Vertices) { sStar! };
        newF_aBasis = new AffineBasis(newPlane);

        //ПЕРЕКАТ точно ЛУЧШЕ точно но в DOUBLE
        n = CalcOuterNormal(newF_aBasis);

        //ПЕРЕКАТ Сварт ЧЕМ "быстро" но в DDOUBLE!!!
        // n = (r! * face.Normal) * v - (r! * v) * face.Normal;
        Debug.Assert(Tools.EQ(n.Length, Tools.One), $"GW.RollOverEdge (dim = {spaceDim}): New normal is not of length 1.");


        return BuildFace(newF_aBasis, n, r, edge);
      }
#endregion

#region Auxiliary functions
      /// <summary>
      /// Calculates the outer normal vector of a given set of points.
      /// </summary>
      /// <param name="planeBasis">The basis of the plane.</param>
      /// <returns>The outer normal vector.</returns>
      private Vector CalcOuterNormal(AffineBasis planeBasis) {
        HyperPlane hp = new HyperPlane(planeBasis);
        Vector     n  = hp.Normal;
        OrientNormal(ref n, planeBasis.Origin);

#if DEBUG
        if (S.All(s => hp.Contains(s))) {
          throw new ArgumentException
            ("CalcOuterNormal: All points from S lies in initial plane! There are no convex hull of full dimension.");
        }
#endif

        return n;
      }

      /// <summary>
      /// Orients the normal outward to the given swarm.
      /// </summary>
      /// <param name="normal">The normal to orient.</param>
      /// <param name="origin">A point from S.</param>
      private void OrientNormal(ref Vector normal, Point origin) {
        foreach (SubPoint s in S) {
          TNum dot = (s - origin) * normal;

          if (Tools.LT(dot)) {
            break;
          }

          if (Tools.GT(dot)) {
            normal = -normal;

            break;
          }
        }
      }
#endregion

    }

  }

}
