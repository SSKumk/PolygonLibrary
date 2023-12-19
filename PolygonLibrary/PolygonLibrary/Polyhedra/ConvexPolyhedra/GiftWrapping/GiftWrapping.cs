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

  //todo Сделать статические методы (вершины, многогранник, комплекс, совокупность всех гиперграней заданной размерности)

  /// <summary>
  /// The GiftWrapping class represents a gift wrapping algorithm for convex polytop.
  /// </summary>
  public class GiftWrapping {

    /// <summary>
    /// The convex polytop obtained as a result of gift wrapping algorithm.
    /// </summary>
    internal readonly BaseSubCP BuiltPolytop;

    /// <summary>
    /// The original set of points.
    /// </summary>
    private HashSet<Point> SOrig;

    /// <summary>
    /// The convex polytop. It describes by vertices, faces and edges.
    /// </summary>
    private ConvexPolytop? _polytop;

    /// <summary>
    /// Gets the polytop. In ConvexPolytop form.
    /// </summary>
    public ConvexPolytop CPolytop => _polytop ??= GetCPolytop();

    /// <summary>
    /// Builds ConvexPolytop from BuiltPolytop.
    /// </summary>
    /// <returns>The ConvexPolytop.</returns>
    private ConvexPolytop GetCPolytop() {
      if (BuiltPolytop.PolytopDim <= 2) {
        throw new NotImplementedException();
      }
      Debug.Assert(BuiltPolytop is not null, "GiftWrapping.GetPolytop(): built polytop is null!");
      HashSet<Face> Fs = new HashSet<Face>(BuiltPolytop.Faces!.Select(F => new Face(F.OriginalVertices, F.Normal!)));
      HashSet<Edge> Es = new HashSet<Edge>();

      foreach (BaseSubCP face in BuiltPolytop.Faces!) {
        Es.UnionWith(face.Faces!.Select(F => new Edge(F.OriginalVertices)));
      }

      return new ConvexPolytop(BuiltPolytop.OriginalVertices, BuiltPolytop.PolytopDim, Fs, Es);
    }

    /// <summary>
    /// Wraps a given swarm of points to the convex polytop.
    /// </summary>
    /// <param name="S">The swarm of points.</param>
    /// <returns>The convex polytop.</returns>
    public static ConvexPolytop WrapPolytop(IEnumerable<Point> S) => new GiftWrapping(S).CPolytop;

    /// <summary>
    /// Wraps a given swarm of points to the face lattice.
    /// </summary>
    /// <param name="S">The swarm of points.</param>
    /// <returns>The face lattice, which represents the convex polytop.</returns>
    public static FaceLattice WrapFaceLattice(IEnumerable<Point> S) => new GiftWrapping(S).FaceLattice;

    /// <summary>
    /// Polytop as face lattice. I.e. complex.
    /// </summary>
    private FaceLattice? _faceLattice;

    /// <summary>
    /// Gets the face lattice.
    /// </summary>
    /// <returns>The face lattice.</returns>
    public FaceLattice FaceLattice => _faceLattice ??= ConstructFL();

    /// <summary>
    /// Construct face lattice from BuiltPolytop.
    /// </summary>
    /// <returns>The face lattice.</returns>
    private FaceLattice ConstructFL() {
      Dictionary<int, FLNode> allNodes = new Dictionary<int, FLNode>();
      List<HashSet<FLNode>>   lattice  = new List<HashSet<FLNode>>();
      for (int i = 0; i < BuiltPolytop.PolytopDim + 1; i++) {
        lattice.Add(new HashSet<FLNode>());
      }

      ConstructFLN(BuiltPolytop, ref allNodes, ref lattice);

      return new FaceLattice(lattice);
    }

    /// <summary>
    /// Auxiliary function. It constructs the face lattice node based on given BaseSubCP.
    /// </summary>
    /// <param name="BSP">The sub polytop on which this node will be constructed.</param>
    /// <param name="allNodes">The additional structure. It holds (hash_of_node, node).</param>
    /// <param name="lattice">The lattice that builds.</param>
    /// <returns>The node, with all sub-nodes.</returns>
    private FLNode ConstructFLN(BaseSubCP BSP, ref Dictionary<int, FLNode> allNodes, ref List<HashSet<FLNode>> lattice) {
      if (BSP is SubTwoDimensionalEdge) {
        List<FLNode> sub = new List<FLNode>();
        foreach (Point p in BSP.OriginalVertices) {
          if (!allNodes.ContainsKey(p.GetHashCode())) {
            FLNode vertex = new FLNode(p);
            allNodes.Add(p.GetHashCode(), vertex);
            lattice[0].Add(vertex);
          }
          sub.Add(allNodes[p.GetHashCode()]);
        }
        FLNode seg = new FLNode(sub);
        allNodes.Add(seg.GetHashCode(), seg);
        lattice[1].Add(seg);

        return seg;
      } else {
        List<FLNode> sub = new List<FLNode>();

        foreach (BaseSubCP subF in BSP.Faces!) {
          int hash = new VPolytop(subF.OriginalVertices).GetHashCode();
          if (!allNodes.ContainsKey(hash)) {
            ConstructFLN(subF, ref allNodes, ref lattice);
          }
          sub.Add(allNodes[hash]);
        }
        FLNode node = new FLNode(sub);
        allNodes.Add(node.GetHashCode(), node);
        lattice[node.Dim].Add(node);

        return node;
      }
    }

    /// <summary>
    /// The vertices of the polytop.
    /// </summary>
    public HashSet<Point> Vertices => VPolytop.Vertices;

    /// <summary>
    /// Polytop as intersection of negative half-space of hyperplanes.
    /// </summary>
    private List<HyperPlane>? _HRepr;

    /// <summary>
    /// Get the polytop as a hyperplane representation. Its normals are oriented outwards.
    /// <returns>The list of hyperplanes.</returns>
    public List<HyperPlane> HRepresentation {
      get
        {
          if (_HRepr is null) {
            List<HyperPlane> res = new List<HyperPlane>();
            foreach (Face face in CPolytop.Faces) {
              res.Add(new HyperPlane(face.Vertices.First(), face.Normal));
            }
            _HRepr = res;
          }

          return _HRepr;
        }
    }

    /// <summary>
    /// Polytop as list of its vertices.
    /// </summary>
    public VPolytop VPolytop { get; init; }

    /// <summary>
    /// Gets the polytop as a list of its vertices.
    /// </summary>
    /// <returns>The polytop as a list of its vertices.</returns>
    public List<Point> VerticesList => VPolytop.Vertices.ToList();

    /// <summary>
    /// The class constructs a convex hull of the given swarm of a points during its initialization.
    /// </summary>
    /// <param name="Swarm">The swarm of points to convexify.</param>
    public GiftWrapping(IEnumerable<Point> Swarm) {
      // Пока не будет SubBaseCP размерности 0.
      if (Swarm.Count() == 1) {
        throw new ArgumentException("GW: At least 2 points must be in Swarm for convexification.");
      }

      SOrig = new HashSet<Point>(Swarm);
      // Переводим рой точек на SubPoints чтобы мы могли возвращаться из-подпространств.
      HashSet<SubPoint> S       = Swarm.Select(s => new SubPoint(s, null, s)).ToHashSet();
      AffineBasis       AffineS = new AffineBasis(S);
      if (AffineS.SpaceDim < AffineS.VecDim) {
        // Если рой точек образует подпространство размерности меньшей чем размерность самх точек, то
        // уходим в подпространство и там овыпукляем.
        S = S.Select(s => s.ProjectTo(AffineS)).ToHashSet();
      }


      GiftWrappingMain x = new GiftWrappingMain(S);
      BuiltPolytop = x.BuiltPolytop;
      VPolytop     = new VPolytop(x.BuiltPolytop.OriginalVertices);
    }

    /// <summary>
    /// Perform an gift wrapping algorithm. It holds a necessary fields to construct a d-polytop.
    /// </summary>
    private sealed class GiftWrappingMain {

#region Internal fields for GW algorithm
      private readonly HashSet<SubPoint> S;        // Рой точек
      private readonly int               spaceDim; // Размерность пространства, в котором происходит овыпукление
      private          BaseSubCP?        initFace; // Грань, с которой будем перекатываться в поисках других граней.

      private readonly HashSet<BaseSubCP> buildFaces  = new HashSet<BaseSubCP>(); // Копим грани текущей размерности.
      private readonly HashSet<SubPoint>  buildPoints = new HashSet<SubPoint>();  // Копим точки для создаваемого многогранника

      private readonly TempIncidenceInfo
        buildIncidence = new TempIncidenceInfo(); // ребро --> (F1, F2) которые соседствуют через это ребро

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
        if (spaceDim == 1) {
          return new SubTwoDimensionalEdge(S.Min()!, S.Max()!);
        }
        if (spaceDim == 2) {
          // Если d == 2, то пользуемся плоскостным алгоритмом овыпукления.
          // Для этого проецируем точки, а так как наши "плоские" алгоритмы не создают новые точки, то мы можем спокойно приводить типы.
          List<Point2D> convexPolygon2D = Convexification.GrahamHull(S.Select(s => new SubPoint2D(s)));

          return new SubTwoDimensional(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
        }
        // if (S.Count == spaceDim + 1) {
        //   // Отдельно обработали случай симплекса.
        //   return new SubSimplex(S);
        // }

        // Создаём начальную грань. (Либо берём, если она передана).
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

        // Будем складывать сюда грани, с которых потенциально можно перекатиться
        Queue<BaseSubCP> toTreat = new Queue<BaseSubCP>();
        toTreat.Enqueue(initFace);                    // Начинаем с начальной грани
        foreach (BaseSubCP edge in initFace.Faces!) { // Отмечаем, что с каждого ребра начальной грани мы можем перекатиться
          buildIncidence.Add(edge, (initFace, null));
        }

        do {
          BaseSubCP face = toTreat.Dequeue(); // Берём очередную грань на обработку

          // С каждого ребра, с которого можно перекатываться -- мы перекатываемся.
          foreach (BaseSubCP edge in face.Faces!.Where(edge => buildIncidence[edge].F2 is null)) {
            BaseSubCP nextFace = RollOverEdge(face, edge, out Vector n);
            nextFace.Normal = n;

            buildFaces.Add(nextFace);
            buildPoints.UnionWith(nextFace.Vertices);

            // У всех рёбер новой грани отмечаем соседей. Если это ребро уже было, то тогда добавляем вторую грань
            // иначе создаём новую запись с этим ребром и это гранью.
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
              // Если у построенной грани есть рёбра, у которых нет второго соседа, то эту грань добавляем в очередь
              toTreat.Enqueue(nextFace);
            }
          }
        } while (toTreat.Count != 0);

        // Подготавливаем и собираем многогранник из накопленных граней
        SubIncidenceInfo incidence = new SubIncidenceInfo();
        foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in buildIncidence) {
          incidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
        }
        // if (buildFaces.Count == spaceDim + 1 && buildFaces.All(F => F.Type == SubCPType.Simplex)) { //todo нужна ли вторая часть???
        if (buildFaces.Count == spaceDim + 1) {
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
        Debug.Assert(S.Count != 0, $"BuildInitialPlaneSwart (dim = {spaceDim}): The swarm must has at least one point!");

        // Для построения начальной плоскости найдём точку самую малую в лексикографическом порядке. (левее неё уже точек нет)
        SubPoint    origin = S.Min()!;
        AffineBasis FinalV = new AffineBasis(origin);

        // нормаль к плоскости начальной
        Vector n = -Vector.CreateOrth(spaceDim, 1);

        while (FinalV.SpaceDim < spaceDim - 1) {
          // TNum      maxAngle = -Tools.Six; // "Большое отрицательное число."
          double    maxAngle = -10.0;
          SubPoint? sExtr    = null;

          Vector   e;
          int      i      = 0;
          Vector[] nBasis = { n };
          do {
            i++;
            e = Vector.OrthonormalizeAgainstBasis(Vector.CreateOrth(spaceDim, i), FinalV.Basis, nBasis);
          } while (e.IsZero && i <= spaceDim);
          Debug.Assert
            (i <= spaceDim, $"BuildInitialPlaneSwart (dim = {spaceDim}): Can't find vector e! That orthogonal to FinalV and n.");

          // Vector? r = null; нужен для процедуры Сварта (ниже)
          foreach (SubPoint s in S) {
            // вычисляем "кандидата" проецируя в плоскость (e,n)
            Vector u = (s - origin).ProjectToPlane(e, n);

            if (!u.IsZero) {
              // TNum   angle = Vector.Angle(e, u);
              double angle = Vector.AngleDouble(e, u); // в ddouble Acos очень медленная операция. Поэтому считаем в double-ах.
              // Кандидата с самым большим углом запоминаем
              // if (Tools.GT(angle, maxAngle)) {
                if (angle - maxAngle > Tools.EpsDouble) {
                maxAngle = angle;
                sExtr    = s;
                // r = u.Normalize();
              }
            }
          }
          bool isAdded = FinalV.AddVectorToBasis(sExtr! - origin);

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


        // Нужно выбрать точки лежащие в плоскости и спроектировать их в подпространство этой плоскости
        HyperPlane hp = new HyperPlane(FaceBasis.Origin, n);
        // HyperPlane hp = new HyperPlane(FaceBasis, (FaceBasis.Origin + n, true)); хз что лучше (думаю, что нормаль)
        HashSet<SubPoint> inPlane = S.Where(s => hp.Contains(s)).Select(s => s.ProjectTo(FaceBasis)).ToHashSet();

        Debug.Assert(inPlane.Count >= spaceDim, $"BuildFace (dim = {spaceDim}): In plane must be at least d points!");

        // Если нам передали ребро, то в подпространстве оно будет начальной гранью.
        // Его нормаль будет (1,0,...,0)
        BaseSubCP? prj_initFace = initEdge?.ProjectTo(FaceBasis);
        if (prj_initFace is not null) {
          prj_initFace.Normal = Vector.CreateOrth(FaceBasis.SpaceDim, FaceBasis.SpaceDim);
        }

        // Овыпукляем в подпространстве
        BaseSubCP buildedFace = new GiftWrappingMain(inPlane, prj_initFace).BuiltPolytop.ToPreviousSpace();
        buildedFace.Normal = n;

        // Из роя убираем точки, которые не попали в выпуклую оболочку под-граней
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

        // v вектор перпендикулярный ребру и лежащий в текущей плоскости
        AffineBasis edgeBasis = new AffineBasis(edge.Vertices);
        SubPoint    f         = face.Vertices.First(p => !edge.Vertices.Contains(p));
        Vector      v         = Vector.OrthonormalizeAgainstBasis(f - edgeBasis.Origin, edgeBasis.Basis);

        Vector?   r        = null;
        SubPoint? sStar    = null;
        // TNum      maxAngle = -Tools.Six; // Something small
        double    maxAngle = -10.0;

        // ищем вектор u такой, что в плоскости (v,N) угол между ним и v наибольший, где N нормаль к текущей плоскости
        foreach (SubPoint s in S) {
          Vector u = (s - edgeBasis.Origin).ProjectToPlane(v, face.Normal);
          if (!u.IsZero) {
            // TNum   angle = Vector.Angle(v, u);
            double angle = Vector.AngleDouble(v, u);

            // if (Tools.GT(angle, maxAngle)) {
              if (angle - maxAngle > Tools.EpsDouble) {
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

        //ПЕРЕКАТ точно. ЛУЧШЕ точно но в DOUBLE
        n = CalcOuterNormal(newF_aBasis);

        //ПЕРЕКАТ Сварт. ЧЕМ "быстро" но в DDOUBLE!!!
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
