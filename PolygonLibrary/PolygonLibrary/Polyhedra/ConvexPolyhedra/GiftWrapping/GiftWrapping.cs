namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents the gift wrapping algorithm for constructing convex polytops from Vrep*.
  /// </summary>
  public class GiftWrapping {

    /// <summary>
    /// The convex polytop obtained as a result of gift wrapping algorithm.
    /// </summary>
    private readonly BaseSubCP BuiltPolytop;

    /// <summary>
    /// Constructs the face lattice from the built convex polytop.
    /// </summary>
    /// <returns>The constructed face lattice.</returns>
    public FaceLattice ConstructFL() {
      if (BuiltPolytop.PolytopDim == 0) {
        return new FaceLattice(BuiltPolytop.OriginalVertices.First());
      }
      int d = BuiltPolytop.PolytopDim;

      List<SortedSet<BaseSubCP>> lattice = new List<SortedSet<BaseSubCP>>();
      for (int i = 0; i <= BuiltPolytop.PolytopDim; i++) {
        lattice.Add(new SortedSet<BaseSubCP>());
      }

      // добавили себя
      lattice[d].Add(BuiltPolytop);

      // собираем низ
      SortedSet<BaseSubCP> prevCP = BuiltPolytop.Faces!.ToSortedSet();
      for (int i = d - 1; i >= 0; i--) {
        lattice[i] = prevCP;
        prevCP     = prevCP.SelectMany(node => node.Faces is null ? new List<BaseSubCP>() : node.Faces!).ToSortedSet();
      }

      return FaceLattice.ConstructFromBaseSubCP(lattice);
    }

    /// <summary>
    /// Auxiliary function. It constructs the face lattice node based on given BaseSubCP.
    /// </summary>
    /// <param name="BSP">The BaseSubCP used to construct the node.</param>
    /// <param name="lattice">The list of sorted sets that represents the face lattice being constructed.</param>
    /// <returns>The constructed face lattice node, including all sub-nodes.</returns>
    private FLNode ConstructFLN(BaseSubCP BSP, ref List<SortedSet<FLNode>> lattice) {
      if (BSP is SubTwoDimensionalEdge) {
        List<FLNode> sub = new List<FLNode>();
        foreach (FLNode vertex in BSP.OriginalVertices.Select(p => new FLNode(p))) {
          if (lattice[0].TryGetValue(vertex, out FLNode? vertexInLattice)) {
            sub.Add(vertexInLattice);
          }
          else {
            lattice[0].Add(vertex);
            sub.Add(vertex);
          }
        }
        FLNode seg = new FLNode(sub);
        lattice[1].Add(seg);

        return seg;
      }
      else {
        List<FLNode> sub = new List<FLNode>();

        foreach (BaseSubCP subF in BSP.Faces!) {
          sub.Add
            (
             lattice[subF.PolytopDim].TryGetValue(new FLNode(subF.OriginalVertices), out FLNode? polytopInLattice)
               ? polytopInLattice
               : ConstructFLN(subF, ref lattice)
            );
        }
        FLNode node = new FLNode(sub);
        lattice[node.PolytopDim].Add(node);

        return node;
      }
    }

    /// <summary>
    /// Wraps the convex polytop of the given swarm and produce its face lattice.
    /// </summary>
    /// <param name="S">The swarm of points to construct the face lattice from.</param>
    /// <returns>The constructed face lattice.</returns>
    public static FaceLattice WrapFaceLattice(IReadOnlyCollection<Vector> S) => new GiftWrapping(S).ConstructFL();

    /// <summary>
    /// Wraps the given swarm and returns the Vrep.
    /// </summary>
    /// <param name="S">The swarm to thin convexify.</param>
    /// <returns>The Vrep of wrapped swarm.</returns>
    public static SortedSet<Vector> WrapVRep(IReadOnlyCollection<Vector> S) => new GiftWrapping(S).BuiltPolytop.OriginalVertices;

    /// <summary>
    /// Constructs a convex hull of the given swarm of points during initialization.
    /// </summary>
    /// <param name="Swarm">The swarm of points to convexify.</param>
    public GiftWrapping(IReadOnlyCollection<Vector> Swarm) {
      switch (Swarm.Count) {
        // Пока не будет SubBaseCP размерности 0.
        case 0: throw new ArgumentException("GW: At least one point must be in Swarm for convexification.");
        case 1:
          BuiltPolytop = new SubZeroDimensional(new SubPoint(Swarm.First(), null));

          break;
        default: {
          // Переводим рой точек на SubPoints чтобы мы могли возвращаться из-подпространств.
          IEnumerable<SubPoint> S       = Swarm.Select(s => new SubPoint(s, null));
          AffineBasis           AffineS = new AffineBasis(S);
          if (AffineS.SubSpaceDim < AffineS.SpaceDim) {
            // Если рой точек образует подпространство размерности меньшей чем размерность самих точек, то
            // уходим в подпространство и там овыпукляем.
            S = S.Select(s => s.ProjectTo(AffineS));
          }

          GiftWrappingMain gwSwarm = new GiftWrappingMain(S.ToSortedSet());
          BuiltPolytop = gwSwarm.BuiltPolytop;

          break;
        }
      }
    }

    /// <summary>
    /// Performs the gift wrapping algorithm. It holds the necessary fields to construct a d-polytop.
    /// </summary>
    private sealed class GiftWrappingMain {

#region Internal fields for GW algorithm
      /// <summary>
      /// The set of points in the swarm used for convexification.
      /// </summary>
      private readonly SortedSet<SubPoint> S; // Рой точек

      /// <summary>
      /// The dimension of the space in which convexification occurs.
      /// </summary>
      private readonly int spaceDim; // Размерность пространства, в котором происходит овыпукление

      /// <summary>
      /// The initial face from which the algorithm starts to explore other faces.
      /// </summary>
      private BaseSubCP? initFace; // Грань, с которой будем перекатываться в поисках других граней.

      /// <summary>
      /// The set of faces of the current dimension being constructed.
      /// </summary>
      private readonly List<BaseSubCP> buildFaces = new List<BaseSubCP>(); // Копим грани текущей размерности.

      /// <summary>
      /// The set of points used for constructing the d-polytop.
      /// </summary>
      private readonly SortedSet<SubPoint> buildPoints = new SortedSet<SubPoint>(); // Копим точки для создаваемого многогранника

      /// <summary>
      /// Temporary information about edge incidences between faces.
      /// </summary>
      private readonly TempIncidenceInfo
        buildIncidence = new TempIncidenceInfo(); // ребро --> (F1, F2) которые соседствуют через это ребро

      /// <summary>
      /// The resulted d-polytop in d-space. It holds information about face incidence, vertex-to-face incidence,
      /// (d-1)-faces, vertices, and all k-faces, where 0 &lt; k &lt; d - 1.
      /// </summary>
      public readonly BaseSubCP BuiltPolytop;
#endregion

#region Constructors
      /// <summary>
      /// Performs the gift wrapping algorithm to construct a convex polytop from the given swarm of points.
      /// </summary>
      /// <param name="Swarm">The set of points used for constructing the convex polytop.</param>
      /// <param name="initFace">The initial facet to start the gift wrapping algorithm. If null, the algorithm constructs it.</param>
      public GiftWrappingMain(SortedSet<SubPoint> Swarm, BaseSubCP? initFace = null) {
        S             = Swarm;
        spaceDim      = S.First().SpaceDim;
        this.initFace = initFace;

        Debug.Assert
          (
           new AffineBasis(Swarm).SubSpaceDim == spaceDim
         , $"GiftWrappingMain: span(Swarm) does not form a d-polytop in d-space!"
          );

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
          return new SubTwoDimensional(S);
        }
        if (S.Count == spaceDim + 1) { // Отдельно обработали случай симплекса.
          return new SubSimplex(S);
        }

        // Создаём начальную грань. (Либо берём, если она передана).
        if (initFace is null) {
          initFace        = BuildFace(BuildInitialPlane(out Vector normal));
          initFace.Normal = normal;
        }
        Debug.Assert(initFace.Vertices.Count >= spaceDim);
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
            BaseSubCP nextFace = RollOverEdge(face, edge);

            Debug.Assert(nextFace.Vertices.Count >= spaceDim);
            buildFaces.Add(nextFace);
            buildPoints.UnionWith(nextFace.Vertices);

            // У всех рёбер новой грани отмечаем соседей. Если это ребро уже было, то тогда добавляем вторую грань
            // иначе создаём новую запись с этим ребром и это гранью.
            bool hasFreeEdges = false;
            foreach (BaseSubCP newEdge in nextFace.Faces!) {
              if (buildIncidence.TryGetValue(newEdge, out (BaseSubCP F1, BaseSubCP? F2) E)) {
                buildIncidence[newEdge] = E.F1 <= nextFace ? (E.F1, nextFace) : (nextFace, E.F1);
              }
              else {
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
        // SubIncidenceInfo incidence = new SubIncidenceInfo();
        // foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in buildIncidence) {
        //   incidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
        // }

        return new SubPolytop(buildFaces, buildPoints);
      }

      /// <summary>
      /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
      /// and all other points lies for a one side from it.
      /// </summary>
      /// <param name="normal">The outward normal to the initial plane.</param>
      /// <returns>
      /// Affine basis of the plane, the dimension of the basis is less than d, the dimension of the vectors is d.
      /// </returns>
      private AffineBasis BuildInitialPlane(out Vector normal) {
        Debug.Assert(S.Count != 0, $"BuildInitialPlaneSwart (dim = {spaceDim}): The swarm must has at least one point!");

        // Для построения начальной плоскости найдём точку самую малую в лексикографическом порядке. (левее неё уже точек нет)
        SubPoint    origin = S.Min()!;
        AffineBasis FinalV = new AffineBasis(origin);

        // нормаль к плоскости начальной
        Vector n = -Vector.MakeOrth(spaceDim, 1);

        while (FinalV.SubSpaceDim < spaceDim - 1) {
          Vector e = new LinearBasis(FinalV.LinBasis, new LinearBasis(n)).FindOrthonormalVector(); //todo norm?

          Vector?     r      = null; // нужен для процедуры Сварта (ниже)
          TNum        minCos = Tools.Two;
          SubPoint?   sExtr  = null;
          LinearBasis lb     = new LinearBasis(new Vector[] { e, n }, false);


          foreach (SubPoint s in S) {
            // вычисляем "кандидата" проецируя в плоскость (e,n)
            // Vector u = (s - origin).ProjectToPlane(e, n);
            Vector u = lb.GetProjectionToSubSpace(s - origin);

            if (!u.IsZero) {
              TNum cos = e * u / u.Length;
              // Кандидата с самым большим углом запоминаем
              if (cos < minCos) {
                minCos = cos;
                sExtr  = s;
                r      = u;
              }
            }
          }
          bool isAdded = FinalV.AddVector(sExtr! - origin);

          Debug.Assert
            (
             isAdded
           , $"BuildInitialPlaneSwart (dim = {spaceDim}): The new vector of FinalV is linear combination of FinalV vectors!"
            );

          // НАЧАЛЬНАЯ Нормаль Наша (точная)
          // Temp / Final как-то через это надо

          //НАЧАЛЬНАЯ Нормаль по Сварту
          r = r!.Normalize();
          n = (r * n) * e - (r * e) * n;
          n = n.Normalize();

          OrientNormal(ref n, origin);

#if DEBUG
          HyperPlane hpDebug = new HyperPlane(n, origin, false);
          if (S.All(s => hpDebug.Contains(s))) {
            throw new ArgumentException
              (
               $"GiftWrapping.BuildInitialPlane: (dim = {spaceDim}): All points from S lies in initial plane! There are no convex hull of full dimension."
              );
          }
#endif
        }

        normal = n;

        return FinalV;
      }

      /// <summary>
      /// Builds the next facet of the polytop based on the given basis and normal.
      /// </summary>
      /// <param name="FaceBasis">The basis of the (d-1)-dimensional subspace in terms of d-space.</param>
      /// <param name="initEdge">The (d-2)-dimensional edge in terms of d-space, used as the initial facet in the subspace.</param>
      /// <returns>
      /// The BaseSubCP: (d-1)-dimensional polytop complex expressed in terms of d-dimensional points.
      /// </returns>
      private BaseSubCP BuildFace(AffineBasis FaceBasis, BaseSubCP? initEdge = null) {
        Debug.Assert
          (
           FaceBasis.SubSpaceDim == spaceDim - 1
         , $"BuildFace (dim = {spaceDim}): The basis must lie in (d-1)-dimensional space!"
          );

        BaseSubCP newFace;

        // Нужно выбрать точки лежащие в плоскости и спроектировать их в подпространство этой плоскости
        // SortedSet<SubPoint> inPlane = S.Where(FaceBasis.Contains).Select(s => s.ProjectTo(FaceBasis)).ToSortedSet();
        IEnumerable<SubPoint> inPlane = S.Where(FaceBasis.Contains);
        Debug.Assert(inPlane.Count() >= spaceDim, $"BuildFace (dim = {spaceDim}): In plane must be at least d points!");

        if (inPlane.Count() == spaceDim) { // Случай симплекса обрабатываем без ухода в подпространство
          newFace = new SubSimplex(inPlane);
        }
        else {
          inPlane = inPlane.Select(s => s.ProjectTo(FaceBasis));
          // Если нам передали ребро, то в подпространстве оно будет начальной гранью.
          // Его нормаль будет (0,0,...,1)
          BaseSubCP? prj_initFace = initEdge?.ProjectTo(FaceBasis);
          if (prj_initFace is not null) {
            prj_initFace.Normal = Vector.MakeOrth(FaceBasis.SubSpaceDim, FaceBasis.SubSpaceDim);
          }

          // Овыпукляем в подпространстве
          newFace = new GiftWrappingMain(inPlane.ToSortedSet(), prj_initFace).BuiltPolytop.ToPreviousSpace();

          // Из роя убираем точки, которые не попали в выпуклую оболочку под-граней
          SortedSet<SubPoint> toRemove = inPlane.Select(s => s.Parent).ToSortedSet()!;
          toRemove.ExceptWith(newFace.Vertices);
          S.ExceptWith(toRemove);

          // todo Может быть, что если после удаления точек их стало d+1, то создать симплекс и перестать овыпукляться?
        }

        return newFace;
      }

      /// <summary>
      /// Rolls through a given edge from a given facet to a new facet and returns its normal vector.
      /// </summary>
      /// <param name="face">(d-1)-dimensional face in d-dimensional space.</param>
      /// <param name="edge">(d-2)-dimensional edge in d-dimensional space.</param>
      /// <returns>(d-1)-dimensional face in d-dimensional space which incident to the face by the edge.</returns>
      private BaseSubCP RollOverEdge(BaseSubCP face, BaseSubCP edge) {
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
        Debug.Assert
          (Tools.EQ(face.Normal.Length, Tools.One), $"RollOverEdge (dim = {spaceDim}): The face has the non normalize normal!");
        Debug.Assert(!face.Normal.IsZero, $"RollOverEdge (dim = {spaceDim}): face.Normal has zero length");

        // v вектор перпендикулярный ребру и лежащий в текущей плоскости
        AffineBasis edgeAffBasis = new AffineBasis(edge.Vertices);
        SubPoint    f            = face.Vertices.First(p => !edge.Vertices.Contains(p));
        Vector      v            = edgeAffBasis.LinBasis.Orthonormalize(f - edgeAffBasis.Origin);

        Debug.Assert(Tools.GT((f - edgeAffBasis.Origin) * v));

        Vector?   r      = null;
        SubPoint? sStar  = null;
        TNum      minCos = Tools.Two;

        // ищем точку s, не лежащую в ребре, такую, что ее проекция на плоскость (v,N) дает угол, наибольший в сравнении с другими точками (дает наименьший косинус)
        foreach (SubPoint s in S) {
          if (!edgeAffBasis.Contains(s)) {
            Vector u   = s.ProjectTo2DAffineSpace(edgeAffBasis.Origin, v, face.Normal);
            TNum   cos = Vector.CosAngle(v, u);

            if (cos < minCos) {
              minCos = cos;
              r      = u;
              sStar  = s;
            }
          }
        }

        Debug.Assert(r is not null, "GiftWrapping.RollOverEdge: A new vector 'r' is null!");

        // TODO: А точно нужен новый базис?
        // AffineBasis newF_aBasis = new AffineBasis(edgeAffBasis);
        // newF_aBasis.AddVector(r);

        edgeAffBasis.AddVector(r); // newF_aBasis.AddVector(r)

        Debug.Assert
          (
           edgeAffBasis.SubSpaceDim == face.PolytopDim
         , $"RollOverEdge (dim = {spaceDim}): The dimension of the basis of new F' must equals to F dimension!"
          );

        //ПЕРЕКАТ точно. ЛУЧШЕ точно но в DOUBLE
        // n = CalcOuterNormal(newF_aBasis);

        //ПЕРЕКАТ Сварт. ЧЕМ "быстро" но в DDOUBLE!!!
        // n = (r! * face.Normal) * v - (r! * v) * face.Normal;
        // Debug.Assert(Tools.EQ(n.Length, Tools.One), $"GW.RollOverEdge (dim = {spaceDim}): New normal is not of length 1.");

        BaseSubCP newFace = BuildFace(edgeAffBasis, edge);
        newFace.Normal = CalcOuterNormal(edgeAffBasis);

        return newFace;
      }
#endregion

#region Auxiliary functions
      /// <summary>
      /// Calculates the outer normal vector of a given set of points.
      /// </summary>
      /// <param name="planeBasis">The basis of the plane.</param>
      /// <returns>The outer normal vector.</returns>
      private Vector CalcOuterNormal(AffineBasis planeBasis) {
        Vector n = planeBasis.LinBasis.FindOrthonormalVector();
        OrientNormal(ref n, planeBasis.Origin);

#if DEBUG
        HyperPlane hp = new HyperPlane(planeBasis, false);
        if (S.All(s => hp.Contains(s))) {
          throw new ArgumentException
            ("GiftWrappingMain.CalcOuterNormal: All points from S lies in face! There are no convex hull of full dimension.");
        }
#endif

        return n;
      }

      /// <summary>
      /// Orients the normal outward to the given swarm.
      /// </summary>
      /// <param name="normal">The normal to orient.</param>
      /// <param name="origin">A point from S.</param>
      private void OrientNormal(ref Vector normal, Vector origin) {
        Debug.Assert(Tools.EQ(normal.Length, Tools.One), "OrientNormal: normal is not unite!");

        foreach (SubPoint s in S) {
          TNum dot = Vector.AffMul(s, origin, normal); // (s - origin) * normal;

          if (Tools.LT(dot)) {
            break;
          }

          if (Tools.GT(dot)) {
            normal = -normal;

            break;
          }
        }

#if DEBUG //todo !важная проверка!
        HyperPlane hp = new HyperPlane(normal, origin, false);
        if (!S.All(s => hp.ContainsNegativeNonStrict(s))) {
          throw new ArgumentException("GiftWrapping.OrientNormal: The normal does not form a facet!");
        }
#endif
      }
#endregion

    }

  }

}
