namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // /// <summary>
  // /// Represents a facet (or face) of a convex polytop.
  // /// </summary>
  // public class Facet {
  //
  //   /// <summary>
  //   /// Gets the vertices of the facet.
  //   /// </summary>
  //   /// <value>A list of <see cref="Vector"/> representing the vertices of the facet.</value>
  //   public List<Vector> Vertices { get; }
  //
  //   /// <summary>
  //   /// Gets the normal vector of the facet.
  //   /// </summary>
  //   /// <value>The <see cref="Vector"/> representing the outward normal vector of the facet.</value>
  //   public Vector Normal { get; }
  //
  //   /// <summary>
  //   /// Initializes a new instance of the <see cref="Facet"/> class.
  //   /// </summary>
  //   /// <param name="Vs">The vertices of the facet.</param>
  //   /// <param name="normal">The outward normal vector of the facet.</param>
  //   public Facet(IReadOnlyList<Vector> Vs, Vector normal) {
  //     Vertices = new List<Vector>(Vs);
  //     Normal   = normal;
  //   }
  //
  //   /// <summary>
  //   /// Determines whether the specified object is equal to the current facet.
  //   /// Two facets are considered equal if they have the same set of vertices and the same normal vector.
  //   /// </summary>
  //   /// <param name="obj">The object to compare with the current facet.</param>
  //   /// <returns>True if the specified object is equal to the current facet; otherwise, False.</returns>
  //   public override bool Equals(object? obj) {
  //     if (obj == null || GetType() != obj.GetType()) {
  //       return false;
  //     }
  //
  //     Facet other = (Facet)obj;
  //
  //     return Normal.Equals(other.Normal) && new SortedSet<Vector>(Vertices).SetEquals(other.Vertices);
  //   }
  //
  // }


  /// <summary>
  /// Represents a full-dimensional convex polytope in a d-dimensional space.
  /// </summary>
  public class ConvexPolytop {

    /// <summary>
    /// Specifies the representation format of a convex polytop.
    /// </summary>
    public enum Rep {

      /// <summary>
      /// Vertex representation or V-representation or Vrep.
      /// </summary>
      Vrep

     ,

      /// <summary>
      /// Half-space representation or H-representation or Hrep.
      /// </summary>
      Hrep

     ,

      /// <summary>
      /// Face lattice representation or FLrep .
      /// </summary>
      FLrep

    }

#region Fields and Properties
    /// <summary>
    /// Gets the dimension of the space in which the polytope exists.
    /// </summary>
    public int SpaceDim { get; }

    /// <summary>
    /// Gets the dimension of the polytop. If the face lattice (FLrep) is not available, it will be computed first.
    /// </summary>
    public int PolytopDim => FLrep.Top.PolytopDim;

    /// <summary>
    /// Returns the most informative representation of the polytope.
    /// If multiple representations are available, the priority is as follows:
    /// <list type="bullet">
    /// <item><description><c>FLrep</c>: Face lattice representation.</description></item>
    /// <item><description><c>Vrep</c>: Vertex representation.</description></item>
    /// <item><description><c>Hrep</c>: Half-space representation.</description></item>
    /// </list>
    /// </summary>
    public Rep WhichRep {
      get
        {
          if (IsFLrep) { return Rep.FLrep; }
          if (IsVrep) { return Rep.Vrep; }

          return Rep.Hrep;
        }
    }


    private Vector? _innerPoint = null;

    /// <summary>
    /// Strictly interior point of the polytope.
    /// </summary>
    public Vector InnerPoint {
      get
        {
          if (_innerPoint is not null) {
            return _innerPoint;
          }

          if (IsFLrep) {
            _innerPoint = FLrep.Top.InnerPoint;
          }
          if (IsVrep) {
            _innerPoint = Vrep.Aggregate((acc, v) => acc + v) / TConv.FromInt(Vrep.Count);
          }
          if (IsHrep) {
            /*
             * В случае Hrep найдём какую-нибудь вершину p, затем найдём все рёбра выходящие из неё
             *   после чего, возьмём направление v, как среднее арифметическое векторов-рёбер.
             * Будем искать наименьшее пересечение q вектора v с гиперплоскостями. Потом возьмём середину между p и q.
             */


            Vector? vertex = FindInitialVertex_Simplex(Hrep, out List<HyperPlane>? activeHPs);
            Debug.Assert(vertex is not null, $"ConvexPolytop.InnerPoint_Hrep: Current object is not bounded polytope!");

            // ищём вектор, направленный строго внутрь многогранника
            Combination  J          = new Combination(activeHPs.Count, SpaceDim - 1);
            List<Vector> directions = new List<Vector>();
            do // перебираем кандидатов в "рёбра"
            {
              List<HyperPlane> edge = new List<HyperPlane>(SpaceDim - 1);
              for (int j = 0; j < SpaceDim - 1; j++) {
                edge.Add(activeHPs[J[j]]);
              }
              LinearBasis coEdgeLinSpace = new LinearBasis(edge.Select(hp => hp.Normal));
              bool        isEdge         = true;

              Vector v = coEdgeLinSpace.FindOrthonormalVector(); // ищем направляющий вектор ребра

              // проверяем, что вектор v действительно определяет ребро
              bool firstNonZeroProduct = true;
              foreach (HyperPlane hp in activeHPs) {
                TNum dotProduct = v * hp.Normal;

                // Если скалярное произведение равно нулю, игнорируем
                if (!Tools.EQ(dotProduct)) {
                  // Первое ненулевое скалярное произведение определяет ориентацию вектора v
                  if (firstNonZeroProduct) {
                    if (Tools.GT(dotProduct)) { v = -v; }
                    firstNonZeroProduct = false;
                  }
                  else { // Для всех последующих не нулевых произведений требуется, чтобы они были отрицательные
                    if (Tools.GT(dotProduct)) {
                      isEdge = false;

                      break;
                    }
                  }
                }
              }
              if (isEdge) {
                directions.Add(v);
              }
            } while (J.Next());


            Vector directionIn = directions.Aggregate((acc, v) => acc + v).Normalize();

            TNum tMin  = Tools.Zero;
            bool isInf = true;
            foreach (HyperPlane hp in Hrep) {
              if (!hp.Contains(vertex)) {
                TNum ti = (hp.ConstantTerm - hp.Normal * vertex) / (hp.Normal * directionIn);
                if (Tools.GT(ti) && (isInf || Tools.LT(ti, tMin))) {
                  tMin  = ti;
                  isInf = false;
                }
              }
            }


            Debug.Assert(!isInf, $"ConvexPolytop.InnerPoint: The set of inequalities is unbounded!");


            _innerPoint = Vector.MulByNumAndAdd(directionIn, tMin / Tools.Two, vertex);
          }
          Debug.Assert(_innerPoint is not null, $"ConvexPolytop.InnerPoint: The inner point should not be null!");

          Debug.Assert
            (Hrep.All(hp => hp.ContainsNegative(_innerPoint)), $"ConvexPolytop.InnerPoint: This is not an inner point!");

          return _innerPoint;
        }
    }


    /// <summary>
    /// <c>true</c> if the polytope has constructed Vrep, <c>false</c> otherwise.
    /// </summary>
    public bool IsVrep => _Vrep is not null;

    private SortedSet<Vector>? _Vrep = null;

    /// <summary>
    /// Gets the vertex representation of the polytop.
    /// The V-representation (Vrep) is lazily initialized from the face lattice or if not available, from the H-representation.
    /// </summary>
    public SortedSet<Vector> Vrep {
      get
        {
          if (IsVrep) {
            return _Vrep!;
          }

          if (IsFLrep) {
            _Vrep = FLrep.Vertices;
          }
          else {
            _Vrep = HrepToVrep_Geometric(Hrep);
          }


          return _Vrep!;
        }
    }


    /// <summary>
    /// <c>true</c> if the polytope has constructed Hrep, <c>false</c> otherwise.
    /// </summary>
    public bool IsHrep => _Hrep is not null;

    private List<HyperPlane>? _Hrep = null;

    /// <summary>
    /// Gets the half-space representation of the polytop.
    /// The H-representation (Hrep) is lazily initialized from the face lattice (FLrep).
    /// </summary>
    public List<HyperPlane> Hrep {
      get
        {
          // todo: Возможно стоит реализовать Double Description Method  и/или  Reverse Search Fukud-ы
          _Hrep ??=
            new List<HyperPlane>
              (
               FLrep
                .Lattice[^2]
                .Select(n => new HyperPlane(new AffineBasis(n.AffBasis), false, (FLrep.Top.InnerPoint, false)))
                .ToList()
              );
          Debug.Assert(IsHrep, $"ConvexPolytop.Hrep: _Hrep is null after constructing. Something went wrong!");

          return _Hrep;
        }
    }

    /// <summary>
    /// <c>true</c> if the polytope has constructed FLrep, <c>false</c> otherwise.
    /// </summary>
    public bool IsFLrep => _FLrep is not null;

    private FaceLattice? _FLrep = null;

    /// <summary>
    /// Gets the face lattice representation of the polytop.
    /// The face lattice (FLrep) is lazily initialized from the vertex representation (Vrep).
    /// </summary>
    public FaceLattice FLrep {
      get
        {
          _FLrep ??= new GiftWrapping(Vrep).ConstructFL();
          Debug.Assert(IsFLrep, $"ConvexPolytop.FLrep: _FLrep is null after constructing. Something went wrong!");

          _Vrep = null;

          return _FLrep;
        }
    }

    /// <summary>
    /// Gets the number of faces at each dimension level of the face lattice.
    /// The f-vector represents the count of elements at each dimension level of the face lattice of the polytop.
    /// </summary>
    public int[] fVector => FLrep.Lattice.Select(lvl => lvl.Count).ToArray();
#endregion

#region Constructors
    /// <summary>
    /// Constructs a ConvexPolytop using a set of vertices (Vrep).
    /// Optionally convexifies the polytope and builds its face lattice.
    /// </summary>
    /// <param name="VP">The set of vertices defining the polytop.</param>
    /// <param name="toConvexify">If true, convexifies the polytope and constructs the face lattice.</param>
    private ConvexPolytop(SortedSet<Vector> VP, bool toConvexify) {
      SpaceDim = VP.First().SpaceDim;
      if (toConvexify) { // Если уж овыпукляем, то и решётку построим
        _FLrep = new GiftWrapping(VP).ConstructFL();
        var _ = Vrep; //Сразу инициировали
      }
      else {
        _Vrep = new SortedSet<Vector>(VP);
      }
    }

    /// <summary>
    /// Constructs a ConvexPolytop using a set of hyperplanes (Hrep).
    /// Optionally reduces redundant hyperplanes.
    /// </summary>
    /// <param name="HPs">The list of hyperplanes defining the polytop.</param>
    /// <param name="doHRedundancy">If true, performs redundancy reduction on the hyperplanes.</param>
    private ConvexPolytop(IEnumerable<HyperPlane> HPs, bool doHRedundancy) {
      SpaceDim = HPs.First().Normal.SpaceDim;
      if (doHRedundancy) {
        _Hrep = HRedundancyByGW(HPs);
      }
      else {
        _Hrep = new List<HyperPlane>(HPs);
      }
    }

    /// <summary>
    /// Constructs a ConvexPolytop using a face lattice (FLrep).
    /// </summary>
    /// <param name="fLrep">The face lattice defining the polytop.</param>
    /// <param name="updateIP">If true, updates inner points of a face lattice.</param>
    private ConvexPolytop(FaceLattice fLrep, bool updateIP) {
      SpaceDim = fLrep.Top.AffBasis.SpaceDim;
      _FLrep   = fLrep;

      if (updateIP) {
        _FLrep.Top.ReCalcAddInfo();
      }
    }
#endregion

#region Fabrics
    /// <summary>
    /// Gets or constructs the FLrep of the polytope, and based on it creates new polytope.
    /// </summary>
    /// <returns>The convex polytope in FLrep.</returns>
    public ConvexPolytop GetInFLrep() => CreateFromFaceLattice(FLrep, false);

    /// <summary>
    /// Gets or constructs the Vrep of the polytope, and based on it creates new polytope.
    /// </summary>
    /// <returns>The convex polytope in FLrep.</returns>
    public ConvexPolytop GetInVrep() => CreateFromPoints(Vrep);

    /// <summary>
    /// Gets or constructs the Hrep of the polytope, and based on it creates new polytope.
    /// </summary>
    /// <returns>The convex polytope in Hrep.</returns>
    public ConvexPolytop GetInHrep() => CreateFromHalfSpaces(Hrep);

    /// <summary>
    /// Constructs a convex polytope from a set of points.
    /// </summary>
    /// <param name="S">The set of points.</param>
    /// <param name="doGW">Specifies whether to apply the Gift Wrapping algorithm to convexify the set of points.</param>
    public static ConvexPolytop CreateFromPoints(IEnumerable<Vector> S, bool doGW = false)
      => new ConvexPolytop(S.ToSortedSet(), doGW);

    /// <summary>
    /// Constructs a convex polytope from a set of half-spaces.
    /// </summary>
    /// <param name="HPs">The list of hyperplanes representing the half-spaces.</param>
    /// <param name="doHRedundancy">Specifies whether to eliminate redundant half-spaces.</param>
    public static ConvexPolytop CreateFromHalfSpaces(IEnumerable<HyperPlane> HPs, bool doHRedundancy = false)
      => new ConvexPolytop(HPs, doHRedundancy);

    /// <summary>
    /// Constructs a convex polytope from a face lattice.
    /// </summary>
    /// <param name="faceLattice">The face lattice representing the polytop.</param>
    /// <param name="updateIP">If true, updates inner points of a face lattice.</param>
    public static ConvexPolytop CreateFromFaceLattice(FaceLattice faceLattice, bool updateIP = false)
      => new ConvexPolytop(faceLattice, updateIP);

    /// <summary>
    /// Represents the actions that can be performed on a built polytop.
    /// </summary>
    public enum PolytopAction { None, Convexify, HRedundancy }

    /// <summary>
    /// Constructs a convex polytope from a parameter reader.
    /// </summary>
    /// <param name="pr">The parameter reader.</param>
    /// <param name="action">Specifies the action to be performed on the polytope (e.g., convexify or eliminate redundancies).</param>
    /// <returns>The constructed convex polytop.</returns>
    public static ConvexPolytop CreateFromReader(ParamReader pr, PolytopAction action = PolytopAction.None) {
      string nameRep = pr.ReadString("Rep");

      switch (nameRep) {
        case "Vrep":  return CreateFromVRep_Reader(pr, action == PolytopAction.Convexify);
        case "Hrep":  return CreateFromHRep_Reader(pr, action == PolytopAction.HRedundancy);
        case "FLrep": return CreateFromFLrep_Reader(pr);
        default:      throw new ArgumentException($"Unsupported representation type: {nameRep}");
      }
    }

    /// <summary>
    /// Constructs a convex polytope from a Vrep representation.
    /// </summary>
    /// <param name="pr">The parameter reader.</param>
    /// <param name="toConvexify">Specifies whether to apply the Gift Wrapping algorithm to convexify the set of points.</param>
    /// <returns>The constructed convex polytop.</returns>
    private static ConvexPolytop CreateFromVRep_Reader(ParamReader pr, bool toConvexify) {
      List<Vector> S = pr.ReadVectors("Vs");

      Debug.Assert
        (
         S.All(v => v.SpaceDim == S.First().SpaceDim)
       , $"ConvexPolytop.FromReader: The dimension of all points must be the same at the path = {pr.filePath}"
        );

      return new ConvexPolytop(S.ToSortedSet(), toConvexify);
    }

    /// <summary>
    /// Constructs a convex polytope from an Hrep representation.
    /// </summary>
    /// <param name="pr">The parameter reader.</param>
    /// <param name="doHRedundancy">Specifies whether to eliminate redundant half-spaces.</param>
    /// <returns>The constructed convex polytop.</returns>
    private static ConvexPolytop CreateFromHRep_Reader(ParamReader pr, bool doHRedundancy) {
      List<HyperPlane> HPs = pr.ReadHyperPlanes("HPs");

      Debug.Assert
        (
         HPs.All(v => v.SpaceDim == HPs.First().SpaceDim)
       , $"ConvexPolytop.FromReader: The dimension of all points must be the same at the path = {pr.filePath}"
        );

      return new ConvexPolytop(HPs, doHRedundancy);
    }

    /// <summary>
    /// Constructs a convex polytope from a FLrep representation.
    /// </summary>
    /// <param name="pr">The parameter reader.</param>
    /// <returns>The constructed convex polytop.</returns>
    private static ConvexPolytop CreateFromFLrep_Reader(ParamReader pr) {
      int          PDim = pr.ReadNumber<int>("PDim");
      List<Vector> Vs   = pr.ReadVectors("Vs");

      List<List<FLNode>> lattice = new List<List<FLNode>>(PDim) { Vs.Select(v => new FLNode(v)).ToList() };
      for (int i = 1; i <= PDim; i++) {
        int             predI = i - 1;
        List<List<int>> fk    = pr.Read2DJaggedArray<int>($"f{i}");
        lattice.Add(new List<FLNode>(fk.Count));
        foreach (List<int> fPred in fk) {
          lattice[i].Add(new FLNode(fPred.Select(fPredInd => lattice[predI][fPredInd])));
        }
      }

      return CreateFromFaceLattice(new FaceLattice(lattice.Select(level => level.ToSortedSet()).ToList(), false), true);
    }
#endregion

#region Special polytopes
    /// <summary>
    /// Very simple polytope.
    /// </summary>
    /// <returns>One zero point polytope.</returns>
    public static ConvexPolytop Zero() => CreateFromPoints(new Vector[] { Vector.Zero(1) });

    /// <summary>
    /// Makes a full-dimension axis-parallel 0-1 cube of given dimension in the form of Vrep.
    /// </summary>
    /// <param name="dim">The dimension of the cube.</param>
    /// <returns>A convex polytope as Vrep representation of the hypercube.</returns>
    public static ConvexPolytop Cube01_VRep(int dim) => RectAxisParallel(Vector.Zero(dim), Vector.Ones(dim));

    /// <summary>
    /// Makes a full-dimension axis-parallel 0-1 cube of given dimension in the form of Hrep.
    /// </summary>
    /// <param name="dim">The dimension of the cube.</param>
    /// <returns>A convex polytope as Hrep representation of the hypercube.</returns>
    public static ConvexPolytop Cube01_HRep(int dim) {
      List<HyperPlane> cube = new List<HyperPlane>();

      for (int i = 0; i < dim; i++) {
        cube.Add(new HyperPlane(-Vector.MakeOrth(dim, i + 1), Tools.Zero));
        cube.Add(new HyperPlane(Vector.MakeOrth(dim, i + 1), Tools.One));
      }

      return CreateFromHalfSpaces(cube);
    }

    /// <summary>
    /// Makes a full-dimension axis-parallel rectangle based on two corners.
    /// </summary>
    /// <returns>A convex polytope as Vrep representing the hyper rectangle.</returns>
    public static ConvexPolytop RectAxisParallel(Vector left, Vector right) {
      Debug.Assert
        (
         left.SpaceDim == right.SpaceDim
       , $"ConvexPolytop.RectAxisParallel: The dimension of the points must be equal! Found left = {left}, right = {right}"
        );

      if (left.SpaceDim == 1) {
        return CreateFromPoints(new SortedSet<Vector>() { left, right });
      }

      List<List<TNum>> rect_prev = new List<List<TNum>>();
      List<List<TNum>> rect      = new List<List<TNum>>();
      rect_prev.Add(new List<TNum>() { left[0] });
      rect_prev.Add(new List<TNum>() { right[0] });

      for (int i = 1; i < left.SpaceDim; i++) {
        rect.Clear();

        foreach (List<TNum> coords in rect_prev) {
          rect.Add(new List<TNum>(coords) { left[i] });
          rect.Add(new List<TNum>(coords) { right[i] });
        }

        rect_prev = new List<List<TNum>>(rect);
      }

      SortedSet<Vector> Cube = new SortedSet<Vector>();

      foreach (List<TNum> v in rect) {
        Cube.Add(new Vector(v.ToArray(), false));
      }

      return CreateFromPoints(Cube);
    }

    /// <summary>
    /// Generates a d-simplex in d-space.
    /// </summary>
    /// <param name="simplexDim">The dimension of the simplex.</param>
    /// <param name="doFL">If the flag is true, then face lattice will be constructed, otherwise only Vrep.</param>
    /// <param name="rnd">Optional random number generator. If not provided, a default one will be used.</param>
    /// <returns>A convex polytope as representing the random simplex.</returns>
    public static ConvexPolytop SimplexRND(int simplexDim, bool doFL = false, GRandomLC? rnd = null) {
      GRandomLC random = rnd ?? Tools.Random;

      SortedSet<Vector> simplex = new SortedSet<Vector>();
      do {
        for (int i = 0; i < simplexDim + 1; i++) {
          simplex.Add(new Vector(Vector.GenVector(simplexDim, TConv.FromInt(0), TConv.FromInt(10), random)));
        }
      } while (!new AffineBasis(simplex).IsFullDim);


      return doFL ? CreateFromPoints(simplex, true) : CreateFromPoints(simplex);
    }

    public static ConvexPolytop Simplex3D_base()
      => CreateFromPoints
        (
         new List<Vector>()
           {
             new Vector(new TNum[] { Tools.One, Tools.One, Tools.One })
           , new Vector(new TNum[] { Tools.One, -Tools.One, -Tools.One })
           , new Vector(new TNum[] { -Tools.One, Tools.One, -Tools.One })
           , new Vector(new TNum[] { -Tools.One, -Tools.One, Tools.One })
           }
       , true
        );

    /// <summary>
    /// Makes the cyclic polytope in specified dimension with specified number of points.
    /// </summary>
    /// <param name="pDim">The dimension of the cyclic polytop.</param>
    /// <param name="amountOfPoints">The number of vertices in cyclic polytop.</param>
    /// <param name="step">The increment step for generating points on the moment curve. init = 1 + step.</param>
    /// <param name="doFL">If the flag is true, then face lattice will be constructed, otherwise only Vrep.</param>
    /// <returns>A convex polytope as Vrep representing the cyclic polytop.</returns>
    public static ConvexPolytop Cyclic(int pDim, int amountOfPoints, TNum step, bool doFL = false) {
      Debug.Assert
        (
         amountOfPoints > pDim
       , $"TestPolytopes.Cyclic: The amount of points must be greater than the dimension of the space. Dim = {pDim}, amount = {amountOfPoints}"
        );
      SortedSet<Vector> cycP      = new SortedSet<Vector>() { new Vector(pDim) };
      TNum              baseCoord = Tools.One + step;
      for (int i = 1; i < amountOfPoints; i++) {
        TNum[] point      = new TNum[pDim];
        TNum   coordinate = baseCoord;
        TNum   multiplyer = coordinate;
        for (int t = 0; t < pDim; t++) { // (i, i^2, i^3 , ... , i^d)
          point[t]   =  coordinate;
          coordinate *= multiplyer;
        }
        cycP.Add(new Vector(point, false));
        baseCoord += step;
      }

      return doFL ? CreateFromPoints(cycP, true) : CreateFromPoints(cycP);
    }

    /// <summary>
    /// Makes a hD-sphere as Vrep with given radius using the Euclidean norm.
    /// </summary>
    /// <param name="center">The center of a sphere.</param>
    /// <param name="radius">The radius of a sphere.</param>
    /// <param name="polarDivision">The number of partitions at a zenith angle. Theta in [0, Pi].
    ///   thetaPoints should be greater than 2 for proper calculation.</param>
    /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle. Phi in [0, 2*Pi).</param>
    /// <returns>A convex polytope as Vrep representing the sphere in hD.</returns>
    public static ConvexPolytop Sphere(Vector center, TNum radius, int polarDivision, int azimuthsDivisions)
      => Ellipsoid(polarDivision, azimuthsDivisions, center, Vector.Ones(center.SpaceDim) * radius);

    /// <summary>
    /// Makes a hD-ellipsoid as Vrep with given semi-axis.
    /// </summary>
    /// <param name="polarDivision">The number of partitions at a zenith angle.</param>
    /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle.</param>
    /// <param name="center">The center of an ellipsoid.</param>
    /// <param name="semiAxis">The vector where each coordinate represents the length of the corresponding semi-axis.</param>
    /// <returns>A convex polytope as Vrep representing the ellipsoid in hD.</returns>
    public static ConvexPolytop Ellipsoid(int polarDivision, int azimuthsDivisions, Vector center, Vector semiAxis) {
      Debug.Assert
        (
         semiAxis.SpaceDim == center.SpaceDim
       , $"ConvexPolytop.Ellipsoid: The dimension of the semiAxis-vector of an ellipsoid must be equal to dim = {center.SpaceDim}. Found semiAxis.SpaceDim = {semiAxis.SpaceDim}"
        );
      int dim = center.SpaceDim;
#if DEBUG
      for (int i = 0; i < dim; i++) {
        Debug.Assert
          (
           Tools.GT(semiAxis[i])
         , $"ConvexPolytop.Ellipsoid: The value of semi-axis must be greater than 0! Found i = {i}, val = {semiAxis[i]}"
          );
      }
#endif

      if (dim == 1) {
        return CreateFromPoints(new[] { center - semiAxis, center + semiAxis });
      }


      // Phi in [0, 2*Pi)
      // Theta in [0, Pi]
      SortedSet<Vector> Ps        = new SortedSet<Vector>();
      int               N         = dim - 2;
      TNum              thetaStep = Tools.PI / TConv.FromInt(polarDivision);
      TNum              phiStep   = Tools.PI2 / TConv.FromInt(azimuthsDivisions);

      List<TNum> thetaAll = new List<TNum>();
      for (int i = 0; i <= polarDivision; i++) {
        thetaAll.Add(thetaStep * TConv.FromInt(i));
      }

      // цикл по переменной [0, 2*Pi)
      for (int i = 0; i < azimuthsDivisions; i++) {
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
          (TNum sinPhi, TNum cosPhi) = TNum.SinCos(phi);
          point.Add(semiAxis[0] * cosPhi * sinsN);
          point.Add(semiAxis[1] * sinPhi * sinsN);


          //добавляем серединные координаты
          if (dim >= 4) { // Их нет для 2Д и 3Д сфер
            TNum sinsJ = Tools.One;
            for (int j = 2; j <= N; j++) {
              sinsJ *= TNum.Sin(s[j - 1]);
              point.Add(semiAxis[j] * TNum.Cos(s[j]) * sinsJ);
            }
          }

          // последнюю координату
          if (dim >= 3) { // У 2Д сферы её нет
            point.Add(semiAxis[dim - 1] * TNum.Cos(s[1]));
          }

          // точка готова, добавляем в наш массив
          Ps.Add(center + new Vector(point.ToArray(), false));
        }
      }

      return CreateFromPoints(Ps);
    }

    /// <summary>
    /// Makes the ball in 1-norm.
    /// </summary>
    /// <param name="center">The center of the ball.</param>
    /// <param name="radius">The radius of the ball.</param>
    /// <returns>The ball in 1-norm.</returns>
    public static ConvexPolytop Ball_1(Vector center, TNum radius) {
      SortedSet<Vector> ball = new SortedSet<Vector>();
      for (int i = 1; i <= center.SpaceDim; i++) {
        Vector e = radius * Vector.MakeOrth(center.SpaceDim, i) + center;
        ball.Add(e);
        ball.Add(-e);
      }

      return CreateFromPoints(ball);
    }

    /// <summary>
    /// Makes the ball in infinity norm.
    /// </summary>
    /// <param name="center">The center of the ball.</param>
    /// <param name="radius">The radius of the ball.</param>
    /// <returns>The ball in infinity norm.</returns>
    public static ConvexPolytop Ball_oo(Vector center, TNum radius) {
      Vector one = Vector.Ones(center.SpaceDim) * radius;

      return RectAxisParallel(-one + center, one + center);
    }

    // /// <summary>
    // /// Makes the convex polytope which represents the distance to the ball in dim-space.
    // /// </summary>
    // /// <param name="dim">The dimension of the sphere-ball. It is greater than 1.</param>
    // /// <param name="polarDivision">The number of partitions at a zenith angle. Theta in [0, Pi].</param>
    // /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle. Phi in [0, 2*Pi).</param>
    // /// <param name="radius0">The radius of an initial sphere.</param>
    // /// <param name="CMax">The value of the last coordinate of a final sphere in dim+1 space.</param>
    // /// <returns>The convex polytope which represents the distance to the ball in dim-space. </returns>
    // public static ConvexPolytop DistanceToBall_2(int dim, int polarDivision, int azimuthsDivisions, TNum radius0, TNum CMax) {
    //   ConvexPolytop ball0        = Sphere(polarDivision, azimuthsDivisions, Vector.Zero(dim), radius0);
    //   TNum          radiusF      = radius0 + CMax;
    //   ConvexPolytop ballF        = CreateFromPoints(ball0.Vrep.Select(v => v * radiusF).ToSortedSet());
    //   ConvexPolytop ball0_lifted = ball0.LiftUp(dim + 1, Tools.Zero);
    //   ConvexPolytop ballF_lifted = ballF.LiftUp(dim + 1, CMax);
    //   ball0_lifted.Vrep.UnionWith(ballF_lifted.Vrep); // Теперь тут лежат все точки
    //
    //   return CreateFromPoints(ball0_lifted.Vrep, true);
    // }

    /// <summary>
    /// Makes the convex polytope which represents the distance to the given convex polytope in dim-space.
    /// </summary>
    /// <param name="P">Polytop the distance to which is constructed.</param>
    /// <param name="k">The value of the last coordinate of P in dim+1 space.</param>
    /// <param name="ballCreator">Function that makes balls.</param>
    /// <returns>A polytope representing the distance to the ball.</returns>
    private static ConvexPolytop DistanceToPolytope(ConvexPolytop P, TNum k, Func<Vector, TNum, ConvexPolytop> ballCreator) {
      // R (+) Ball(0, k)
      ConvexPolytop bigP = DistTo_MakeBase(P, k, ballCreator);

      //{(R,0), (R (+) Ball(0, k),k)}
      ConvexPolytop toConv = bigP.LiftUp(P.SpaceDim + 1, k);
      toConv.Vrep.UnionWith(P.LiftUp(P.SpaceDim + 1, Tools.Zero).Vrep);

      //conv{...}
      return CreateFromPoints(toConv.Vrep, true);
    }

    //todo: xml
    public static ConvexPolytop DistTo_MakeBase(ConvexPolytop P, TNum k, Func<Vector, TNum, ConvexPolytop> ballCreator) {
      ConvexPolytop bigP = MinkowskiSum.BySandipDas(ballCreator(Vector.Zero(P.SpaceDim), k), P);

      return bigP;
    }

    /// <summary>
    ///  Makes the convex polytope representing the distance in "_1"-norm to the given convex polytope in dimensional space.
    /// </summary>
    /// <param name="P">The polytope to which the distance is constructed.</param>
    /// <param name="k">The value of the last coordinate in the (dim + 1)-dimensional space.</param>
    /// <returns>A polytope representing the distance to the polytope P in ball_1 norm.</returns>
    public static ConvexPolytop DistanceToPolytopeBall_1(ConvexPolytop P, TNum k) => DistanceToPolytope(P, k, Ball_1);

    /// <summary>
    /// Makes the convex polytope representing the distance in "infinity"-norm to the given convex polytope in dimensional space.
    /// </summary>
    /// <param name="P">The polytope to which the distance is constructed.</param>
    /// <param name="k">The value of the last coordinate in the (dim + 1)-dimensional space.</param>
    /// <returns>A polytope representing the distance to the polytope P in ball_oo norm.</returns>
    public static ConvexPolytop DistanceToPolytopeBall_oo(ConvexPolytop P, TNum k) => DistanceToPolytope(P, k, Ball_oo);

    /// <summary>
    /// Makes the convex polytope representing the distance in "euclidean"-norm to the given convex polytope in dimensional space.
    /// </summary>
    /// <param name="P">The polytope to which the distance is constructed.</param>
    /// <param name="k">The value of the last coordinate in the (dim + 1)-dimensional space.</param>
    /// <param name="polarDivision">The number of partitions at zenith angle.</param>
    /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle.</param>
    /// <returns>A polytope representing the distance to the polytope P in ball_2 norm.</returns>
    public static ConvexPolytop DistanceToPolytopeBall_2(ConvexPolytop P, TNum k, int polarDivision, int azimuthsDivisions)
      => DistanceToPolytope(P, k, (center, radius) => Sphere(center, radius, polarDivision, azimuthsDivisions));

    /// <summary>
    /// Makes the convex polytope representing the distance in "_1"-norm to the origin in dimensional space.
    /// </summary>
    /// <param name="point">The point distance to is computed.</param>
    /// <param name="k">The value of the last coordinate in the (dim + 1)-dimensional space.</param>
    /// <returns>A polytope representing the distance to the origin in ball_1 norm.</returns>
    public static ConvexPolytop DistanceToPointBall_1(Vector point, TNum k) => DistanceToPoint(point, k, Ball_1);

    /// <summary>
    /// Makes the convex polytope representing the distance in "_oo"-norm to the origin in dimensional space.
    /// </summary>
    /// <param name="point">The point distance to is computed.</param>
    /// <param name="k">The value of the last coordinate in the (dim + 1)-dimensional space.</param>
    /// <returns>A polytope representing the distance to the origin in ball_oo norm.</returns>
    public static ConvexPolytop DistanceToPointBall_oo(Vector point, TNum k) => DistanceToPoint(point, k, Ball_oo);

    /// <summary>
    /// Makes the convex polytope representing the distance in "_2"-norm to the origin in (dim)-dimensional space.
    /// </summary>
    /// <param name="point">The point distance to is computed.</param>
    /// <param name="polarDivision">The number of partitions at a zenith angle.</param>
    /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle.</param>
    /// <param name="k">The value of the last coordinate in the (dim + 1)-dimensional space.</param>
    /// <returns>A polytope representing the distance to the origin in ball_2 norm.</returns>
    public static ConvexPolytop DistanceToPointBall_2(Vector point, int polarDivision, int azimuthsDivisions, TNum k)
      => DistanceToPoint(point, k, Ball_2FuncCreator(polarDivision, azimuthsDivisions));

    //todo: xml
    public static Func<Vector, TNum, ConvexPolytop> Ball_2FuncCreator(int polarDivision, int azimuthsDivisions) {
      return (center, radius) => Sphere(center, radius, polarDivision, azimuthsDivisions);
    }

    /// <summary>
    /// Makes the convex polytope representing the distance to the point in (dim)-dimensional space.
    /// </summary>
    /// <param name="point">The point distance to is computed.</param>
    /// <param name="k">The value of the last coordinate in the (dim + 1)-dimensional space.</param>
    /// <param name="ballCreator">Function that creates balls.</param>
    /// <returns>A polytope representing the distance to the origin.</returns>
    private static ConvexPolytop DistanceToPoint(Vector point, TNum k, Func<Vector, TNum, ConvexPolytop> ballCreator) {
      ConvexPolytop ball   = ballCreator(point, k);
      ConvexPolytop toConv = ball.LiftUp(point.SpaceDim + 1, k);
      toConv.Vrep.Add(Vector.Zero(point.SpaceDim + 1));

      //conv{...}
      return CreateFromPoints(toConv.Vrep, true);
    }
#endregion

#region Get it specific forms
#endregion

#region Functions
    /// <summary>
    /// Finds the nearest point on the surface of the convex polytope to the given point and reports inside or outside it is.
    /// </summary>
    /// <param name="point">The point to find the nearest surface point to.</param>
    /// <param name="position">The indicator whenever the point lies outside the polytope or not.</param>
    /// <returns>The nearest point on the polytope.</returns>
    /// <exception cref="NotImplementedException">
    /// Thrown if the nearest point calculation from Vrep and Hrep, i.e. this is not implemented.
    /// </exception>
    public Vector NearestPoint(Vector point, out int position) {
      position = Contains(point);

      if (position == 0) { // если внутри, то возвращаем текущую точку
        return point;
      }

      if (position < 0) { // Если внутри многогранника, то ищем любую ближайшую точку на гранях
        if (IsHrep || IsFLrep) {
          // Перебираем только d-1 грани, в силу выпуклости!!!!
          Vector? min =
            Hrep
             .Select(hp => hp.AffBasis.ProjectPointToSubSpace_in_OrigSpace(point))
              // .Where(ContainsNonStrict) // todo: кажется, что можно не проверять!
             .MinBy(v => (point - v).Length);

          Debug.Assert(min is not null, "ConvexPolytope.NearestPoint: Can't find minimal vector");

          return min;
        }
      }
      else {           // если снаружи, то ближайшая точка единственна
        if (IsFLrep) { // visible для ускорения счёта, так как, очевидно, что искать нужно только среди видимых k-граней
          IEnumerable<FLNode> visible =
            FLrep[^2].Where(hnode => new HyperPlane(hnode.AffBasis, false, (InnerPoint, false)).ContainsPositive(point));
          SortedSet<FLNode> allKfaces = visible.SelectMany(node => node.AllNonStrictSub).ToSortedSet();
          Vector? min =
            allKfaces
             .Select(node => node.AffBasis.ProjectPointToSubSpace_in_OrigSpace(point))
             .Where(ContainsOnBorder)
             .MinBy(v => (point - v).Length);

          Debug.Assert(min is not null, "ConvexPolytope.NearestPoint: Can't find minimal vector");

          return min;
        }
      }


      throw new NotImplementedException("Из Vrep и Hrep пока не умею.");
    }

    /// <summary>
    /// Finds the nearest point on the surface of the convex polytope to the given point.
    /// </summary>
    /// <param name="point">The point to find the nearest surface point to.</param>
    /// <returns>The nearest point on the polytope.</returns>
    /// <exception cref="NotImplementedException">
    /// Thrown if the nearest point calculation from Vrep and Hrep, i.e. this is not implemented.
    /// </exception>
    public Vector NearestPoint(Vector point) => NearestPoint(point, position: out _);

    /// <summary>
    /// Determines the position of a given point relative to the polytope: inside, on the border, or outside.
    /// </summary>
    /// <param name="v">The point to check.</param>
    /// <returns>
    /// <c>-1</c> if the point is inside the polytope;
    /// <c>0</c> if the point is on the border of the polytope;
    /// <c>1</c> if the point is outside the polytope.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when the method is called for a polytope in V-representation.
    /// </exception>
    public int Contains(Vector v) {
      int inP = 0;
      if (IsFLrep || IsHrep) {
        foreach (HyperPlane hp in Hrep) {
          int w = Tools.CMP(hp.Eval(v));
          if (w == 1) { // если точка оказалась снаружи хотя бы одной гиперграни, значит она вне многогранника
            return 1;
          }
          inP += w; // иначе складываем -1 и 0. Если всё -1, то внутри, если не всё, то на границе.
        }

        if (inP == -Hrep.Count) {
          return -1; // точка внутри
        }

        return 0; // точка на границе
      }

      throw new NotImplementedException
        ("ConvexPolytop.ContainsNonStrict: Не умеем в Vrep! Тут надо решить несколько LP задач. Смотри Фукуду.");
    }

    /// <summary>
    /// Checks whether the given vector is contained on its boundary.
    /// </summary>
    /// <param name="v">The vector to check.</param>
    /// <returns>
    /// <c>true</c> if the vector is contained on the polytope boundary; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsOnBorder(Vector v) => Contains(v) == 0;

    /// <summary>
    /// Checks whether the given vector is contained within the polytope, including its boundary.
    /// </summary>
    /// <param name="v">The vector to check.</param>
    /// <returns>
    /// <c>true</c> if the vector is contained within the polytope (including the boundary); otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsNonStrict(Vector v) => Contains(v) <= 0;


    /// <summary>
    /// Checks whether the given vector is strictly contained within the polytope, excluding its boundary.
    /// </summary>
    /// <param name="v">The vector to check.</param>
    /// <returns>
    /// <c>true</c> if the vector is strictly contained within the polytope (excluding the boundary); otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsStrict(Vector v) => Contains(v) < 0;


    /// <summary>
    /// Determines whether a given point lies outside the polytope.
    /// </summary>
    /// <param name="v">The point to check.</param>
    /// <returns>
    /// <c>true</c> if the point lies outside the polytope; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsComplement(Vector v) => Contains(v) > 0;


    /// <summary>
    /// Shifts an origin into a polytope and makes a dual polytope to this.
    /// </summary>
    /// <returns>The dual polytope.</returns>
    public ConvexPolytop Polar(out Vector shiftToOrigin, bool doUnRedundancy = false) {
      ConvexPolytop atOrigin = ShiftToOrigin(out Vector toOrigin);

      shiftToOrigin = toOrigin;

      return atOrigin.Polar(doUnRedundancy);
    }

    /// <summary>
    /// Makes a dual polytope to this.
    /// </summary>
    /// <returns>The dual polytope.</returns>
    public ConvexPolytop Polar(bool doUnRedundancy = false) { // Начало координат внутри многогранника, важно!
      if (IsFLrep) {
        List<SortedSet<FLNode>>          newFL    = new List<SortedSet<FLNode>>() { new SortedSet<FLNode>() };
        SortedDictionary<FLNode, FLNode> oldToNew = new SortedDictionary<FLNode, FLNode>();

        //Уровень вершин создаём отдельно
        foreach (FLNode oldNode in FLrep[^2]) {
          HyperPlane hp        = new HyperPlane(oldNode.AffBasis, true, (InnerPoint, false));
          FLNode     newVertex = new FLNode(hp.Normal / hp.ConstantTerm);
          newFL[0].Add(newVertex);
          oldToNew.Add(oldNode, newVertex);
        }
        int dOld = PolytopDim - 2;
        int dNew = 1;
        for (; dOld >= 0; dOld--, dNew++) {
          newFL.Add(new SortedSet<FLNode>());
          foreach (FLNode oldNode in FLrep[dOld]) {
            FLNode reverseNode = new FLNode(oldNode.Super.Select(n => oldToNew[n]));
            newFL[dNew].Add(reverseNode);
            oldToNew.Add(oldNode, reverseNode);
          }
        }
        newFL.Add(new SortedSet<FLNode>() { new FLNode(newFL.Last()) });

        return CreateFromFaceLattice(new FaceLattice(newFL, false), false);
      }

      if (IsVrep) {
        List<HyperPlane> hrepDual = new List<HyperPlane>();
        foreach (Vector v in Vrep) {
          hrepDual.Add(new HyperPlane(v, Tools.One));
        }

        return CreateFromHalfSpaces(hrepDual);
      }

      // if IsHrep
      List<Vector> vrepDual = new List<Vector>();
      foreach (HyperPlane hp in Hrep) {
        vrepDual.Add(hp.Normal / hp.ConstantTerm);
      }

      return CreateFromPoints(vrepDual, doUnRedundancy);
    }

    /// <summary>
    /// Writes the convex polytope representation to the specified <see cref="ParamWriter"/> in the chosen format.
    /// </summary>
    /// <param name="pw">The <see cref="ParamWriter"/> instance where the convex polytope will be written.</param>
    /// <param name="rep">
    /// The representation format of the convex polytop.
    /// This determines whether the polytope will be written as a Vrep, Hrep, or FLrep.
    /// Defaults to <see cref="Rep.FLrep"/>.
    /// </param>
    public void WriteIn(ParamWriter pw, Rep rep) {
      switch (rep) {
        case Rep.Vrep:  WriteAsVRep(pw, this); break;
        case Rep.Hrep:  WriteAsHRep(pw, this); break;
        case Rep.FLrep: WriteAsFLRep(pw, this); break;
      }
      pw.Flush();
    }

    /// <summary>
    /// Lifts the given convex polytope to a higher dimension by extending all its vertices with the specified value.
    /// </summary>
    /// <param name="d">The target dimension to expand to. Must be greater than the current dimension of the vector.</param>
    /// <param name="val">The value used in expansion.</param>
    /// <returns>The polytope in higher dimension.</returns>
    public ConvexPolytop LiftUp(int d, TNum val) => CreateFromPoints(Vrep.Select(v => v.LiftUp(d, val)).ToSortedSet());

    /// <summary>
    /// Creates a new convex polytope in d-dimensional space by intersecting the given d-dimensional convex polytope P with a specified hyperplane.
    /// </summary>
    /// <param name="hp">The hyperplane to section P.</param>
    /// <returns>The section of the polytope P.</returns>
    public ConvexPolytop SectionByHyperPlane(HyperPlane hp) {
      HyperPlane       hp_   = new HyperPlane(-hp.Normal, -hp.ConstantTerm);
      List<HyperPlane> xList = new List<HyperPlane> { hp };
      xList.AddRange(Hrep);
      SortedSet<Vector> x = HrepToVrep_Geometric(xList);

      List<HyperPlane> yList = new List<HyperPlane> { hp_ };
      yList.AddRange(Hrep);
      SortedSet<Vector> y = HrepToVrep_Geometric(yList);

      x.IntersectWith(y);

      return CreateFromPoints(x);
    }

    /// <summary>
    /// Shifts the convex polytop by the specified vector.
    /// </summary>
    /// <param name="s">The vector by which to shift the polytop.</param>
    /// <returns>A new convex polytop shifted by the given vector.</returns>
    public ConvexPolytop Shift(Vector s) {
      if (IsFLrep) {
        return CreateFromFaceLattice(FLrep.VertexTransform(v => v + s), false);
      }
      if (IsVrep) {
        return CreateFromPoints(Vrep.Select(v => v + s));
      }

      return CreateFromHalfSpaces(Hrep.Select(hp => new HyperPlane(hp.Normal, hp.ConstantTerm + hp.Normal * s)));
    }

    /// <summary>
    /// Rotates the convex polytop by the specified matrix.
    /// </summary>
    /// <param name="rotate">The matrix by which to rotate the polytop.</param>
    /// <returns>A new convex polytop rotated by the given matrix.</returns>
    public ConvexPolytop Rotate(Matrix rotate) {
      if (IsFLrep) {
        return CreateFromFaceLattice(FLrep.VertexTransform(v => v * rotate), false);
      }
      if (_Vrep is not null) {
        return CreateFromPoints(Vrep.Select(v => v * rotate));
      }

      return CreateFromHalfSpaces(Hrep.Select(hp => new HyperPlane(hp.Normal * rotate, hp.ConstantTerm)));
    }

    /// <summary>
    /// Rotates the polytope by a randomly generated orthonormal matrix.
    /// </summary>
    /// <param name="doFL">If true, the face lattice will be constructed; otherwise, only the Vrep.</param>
    /// <param name="rnd">The random engine to be used. If null, the default random engine will be used. (Tools.Random)</param>
    /// <returns>The rotated polytope.</returns>
    public ConvexPolytop RotateRND(bool doFL = false, GRandomLC? rnd = null) {
      GRandomLC random = rnd ?? Tools.Random;
      Matrix    rotate = Matrix.GenONMatrix(SpaceDim, random);

      return doFL ? Rotate(rotate).GetInFLrep() : Rotate(rotate);
    }

    /// <summary>
    /// Shifts the convex polytop to the origin.
    /// </summary>
    /// <returns>A new convex polytop with the selected point at the origin.</returns>
    public ConvexPolytop ShiftToOrigin(out Vector innerPoint) {
      innerPoint = InnerPoint;

      return Shift(-InnerPoint);
    }


    /// <summary>
    /// Creates a scaled version of the current polytope with respect to a given origin.
    /// The polytope is scaled by the given factor <paramref name="k"/>.
    /// </summary>
    /// <param name="k">The scaling factor. Must be non-negative.</param>
    /// <param name="origin">The origin point for scaling.</param>
    /// <returns>
    /// A new <see cref="ConvexPolytop"/> that is a scaled version of the current polytope.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when the scaling factor <paramref name="k"/> is negative, as this case is not supported for now.
    /// </exception>
    public ConvexPolytop Scale(TNum k, Vector origin) {
      if (Tools.LT(k)) {
        throw new NotImplementedException("ConvexPolytop.Scale: Пока не умеем с отрицательным 'k'");
      }

      if (IsFLrep) {
        return CreateFromFaceLattice(FLrep.VertexTransform(v => (v - origin) * k), true);
      }
      if (IsVrep) {
        return CreateFromPoints(Vrep.Select(v => (v - origin) * k));
      }

      return CreateFromHalfSpaces(Hrep.Select(hp => new HyperPlane(hp.Normal, hp.ConstantTerm * k)));
    }

    /// <summary>
    /// Computes the minimum distance between any pair of vertices in the Vrep.
    /// </summary>
    /// <returns>The minimum distance between any pair of vertices.</returns>
    public TNum MinDistBtwVs() => MinimalDiameter(Vrep);

    /// <summary>
    /// Converts the most informative representation of the polytope to its string equivalent.
    /// </summary>
    /// <returns>
    /// A string representing the polytope's current representation:
    /// <list type="bullet">
    /// <item><description><c>Vrep</c> for vertex representation.</description></item>
    /// <item><description><c>Hrep</c> for half-space representation.</description></item>
    /// <item><description><c>FLrep</c> for face lattice representation.</description></item>
    /// </list>
    /// </returns>
    public string WhichRepToString() {
      switch (WhichRep) {
        case Rep.Vrep:  return "Vrep";
        case Rep.Hrep:  return "Hrep";
        case Rep.FLrep: return "FLrep";
      }

      throw new ArgumentException("ConvexPolytop.WhichRepToString: Unexpected representation type.");
    }

    /// <summary>
    /// Computes the minimal pairwise distance among the given points.
    /// </summary>
    /// <param name="S">The set of points.</param>
    /// <returns>The minimal pairwise distance.</returns>
    public static TNum MinimalDiameter(IEnumerable<Vector> S) {
      List<Vector> points = S.ToList();
      Debug.Assert
        (
         points.Count > 1
       , $"ConvexPolytop.MinimalDiameter: The number of points in the collection should be greater than one. Found {points.Count}"
        );

      TNum minD = (points[0] - points[1]).Length;
      for (int i = 0; i < points.Count - 1; i++) {
        for (int j = i + 1; j < points.Count; j++) {
          TNum d = (points[i] - points[j]).Length;
          if (Tools.LT(d, minD)) {
            minD = d;
          }
        }
      }

      return minD;
    }
#endregion

#region Write out
    /// <summary>
    /// Writes the convex polytope as Vrep to the specified <see cref="ParamWriter"/>.
    /// </summary>
    private static void WriteAsVRep(ParamWriter pr, ConvexPolytop P) {
      pr.WriteString("Rep", "Vrep");
      pr.WriteVectors("Vs", P.Vrep);
    }

    /// <summary>
    /// Writes the convex polytope as Hrep to the specified <see cref="ParamWriter"/>.
    /// </summary>
    private static void WriteAsHRep(ParamWriter pr, ConvexPolytop P) {
      pr.WriteString("Rep", "Hrep");
      pr.WriteHyperPlanes("HPs", P.Hrep);
    }

    /// <summary>
    /// Writes the convex polytope as FLrep to the specified <see cref="ParamWriter"/>.
    /// </summary>
    private static void WriteAsFLRep(ParamWriter pr, ConvexPolytop P) {
      pr.WriteString("Rep", "FLrep");
      pr.WriteNumber("PDim", P.PolytopDim);
      pr.WriteVectors("Vs", P.Vrep);

      for (int i = 1; i < P.FLrep.Lattice.Count; i++) {
        List<FLNode>    levelPred = P.FLrep.Lattice[i - 1].ToList();
        List<FLNode>    level     = P.FLrep.Lattice[i].ToList();
        List<List<int>> f_k       = new List<List<int>>(level.Count);
        foreach (FLNode subFace in level) {
          f_k.Add(subFace.Sub.Select(pred => levelPred.IndexOf(pred)).ToList());
        }
        pr.Write2DArray($"f{i}", f_k);
      }
    }
#endregion


#region Overrides
    public override int GetHashCode() => throw new InvalidOperationException(); //HashCode.Combine(SpaceDim);

    //!!! Вообще говоря, сравнивать многогранники тяжело!!! Тут "какая-то наивная" реализация сравнения
    private bool FLEquals(ConvexPolytop other) => this.FLrep.Equals(other.FLrep);

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;

      return FLEquals((ConvexPolytop)obj);
    }
#endregion


    /// <summary>
    /// Converts the polytope to a convex polygon in 2D space using the specified affine basis.
    /// </summary>
    /// <param name="basis">Affine basis for the conversion.</param>
    /// <returns>A convex polygon representing the polytope in 2D space.</returns>
    public ConvexPolygon ToConvexPolygon(AffineBasis basis) {
      if (PolytopDim != 2) {
        throw new ArgumentException($"The dimension of the polygon must equal to 2. Found = {PolytopDim}.");
      }

      return new ConvexPolygon(basis.ProjectPoints(Vrep));
    }

    /// <summary>
    /// Converts the Hrep of a convex polytope to Vrep using a naive approach by checking all possible d-tuples of the hyperplanes.
    /// </summary>
    /// <param name="HPs">List of hyperplanes defining the Hrep.</param>
    /// <returns>The Vrep of the convex polytop.</returns>
    public static SortedSet<Vector> HrepToVrep_Naive(List<HyperPlane> HPs) {
      int m = HPs.Count;
      int d = HPs.First().Normal.SpaceDim;

      SortedSet<Vector>    Vs          = new SortedSet<Vector>();
      Combination          combination = new Combination(m, d);
      Func<int, int, TNum> AFunc       = (r, l) => HPs[combination[r]].Normal[l];
      Func<int, TNum>      bFunc       = r => HPs[combination[r]].ConstantTerm;
      bool                 belongs;
      GaussSLE             gaussSLE = new GaussSLE(d, d);
      do { // Перебираем все сочетания из d элементов из набора гиперплоскостей
        gaussSLE.SetSystem(AFunc, bFunc, d, d, GaussSLE.GaussChoice.RowWise);
        gaussSLE.Solve();
        if (gaussSLE.GetSolution(out Vector point)) { // Ищем точку пересечения
          belongs = true;
          foreach (HyperPlane hp in HPs) {
            if (hp.ContainsPositive(point)) {
              belongs = false;

              break;
            }
          }
          if (belongs) {
            Vs.Add(point);
          }
        }
      } while (combination.Next());

      return new SortedSet<Vector>(Vs);
    }

    // todo: научиться работать не с полноразмернымми многогранниками
    /// <summary>
    /// Converts the Hrep of a convex polytope to Vrep using a geometric-inspired algorithm. Full-dimensional only!
    /// </summary>
    /// <param name="HPs">List of hyperplanes defining the Hrep.</param>
    /// <returns>The Vrep of the convex polytop.</returns>
    public static SortedSet<Vector>? HrepToVrep_Geometric(List<HyperPlane> HPs) {
      SortedSet<Vector> Vs = new SortedSet<Vector>();
      int               d  = HPs.First().Normal.SpaceDim;
      // Этап 1. Поиск какой-либо вершины и определение гиперплоскостей, которым она принадлежит

      Vector? firstPoint = FindInitialVertex_Simplex(HPs, out List<HyperPlane>? activeHPs);
      if (firstPoint is null || activeHPs is null) { return null; }


      Vs.Add(new Vector(firstPoint)); // первую вершину нашли


      // Этап 2. Поиск всех остальных вершин
      Queue<(Vector, List<HyperPlane>)> process = new Queue<(Vector, List<HyperPlane>)>();
      process.Enqueue((Vs.First(), activeHPs));
      // process.Enqueue((Vs.First(), HPs.Where(hp => hp.Contains(Vs.First())).ToList()));

      HyperPlane[] edge = new HyperPlane[d - 1];
      // Обход в ширину
      while (process.TryDequeue(out (Vector, List<HyperPlane>) elem)) {
        (Vector z, List<HyperPlane> Hz) = elem;

        // Debug.Assert(Hz.Count >= d, $"ConvexPolytop.HrepToVrep_Geometric: The number of planes should be at least {d}. Found {Hz.Count}");

        Combination J = new Combination(Hz.Count, d - 1);
        do // перебираем кандидатов в "рёбра"
        {
          for (int j = 0; j < d - 1; j++) {
            edge[j] = Hz[J[j]];
          }
          LinearBasis coEdgeLinSpace = new LinearBasis(edge.Select(hp => hp.Normal));
          if (coEdgeLinSpace.SubSpaceDim != d - 1) {
            continue;
          } // Несколько гиперплоскостей "наложились", ребро не получим


          // ищем направляющий вектор ребра
          Vector v = coEdgeLinSpace.FindOrthonormalVector();

          // проверяем вектор v
          bool isEdge              = true;
          bool firstNonZeroProduct = true;
          foreach (HyperPlane hp in Hz) {
            TNum dotProduct = v * hp.Normal;

            // Если скалярное произведение равно нулю, игнорируем
            if (!Tools.EQ(dotProduct)) {
              // Первое ненулевое скалярное произведение определяет ориентацию вектора v
              if (firstNonZeroProduct) {
                if (Tools.GT(dotProduct)) { v = -v; }
                firstNonZeroProduct = false;
              }
              else { // Для всех последующих не нулевых произведений требуется, чтобы они были отрицательные
                if (Tools.GT(dotProduct)) {
                  isEdge = false;

                  break;
                }
              }
            }
          }
          if (isEdge) {
            // Теперь v определяет луч, на котором лежит ребро
            List<HyperPlane> zNewHPs       = new List<HyperPlane>();
            List<HyperPlane> orthToEdgeHPs = new List<HyperPlane>();
            TNum             tMin          = Tools.Zero;
            bool             isInf         = true;
            Vector           zNew          = Vector.Zero(d);
            bool             foundPrev     = false;

            foreach (HyperPlane hp in HPs) {
              TNum denominator = hp.Normal * v;

              if (Tools.EQ(denominator)) { // Точка лежит на гиперплоскости
                if (hp.Contains(z)) {
                  orthToEdgeHPs.Add(hp); // мы собираем все грани
                }
              }
              else {
                TNum ti = (hp.ConstantTerm - hp.Normal * z) / denominator;

                if (Tools.GT(ti)) {                  // учитываем только положительные ti
                  if (isInf || Tools.LT(ti, tMin)) { // первый кандидат или найден новый минимум
                    tMin  = ti;
                    isInf = false;
                    zNewHPs.Clear();
                    zNewHPs.Add(hp);
                    zNew = Vector.MulByNumAndAdd(v, tMin, z); // вычисляем новую точку: v*tMin + z
                    if (Vs.Contains(zNew)) {
                      foundPrev = true;

                      break;
                    }
                  }
                  else if (Tools.EQ(ti, tMin)) { // если ti равен текущему минимуму, добавляем гиперплоскость
                    zNewHPs.Add(hp);
                  }
                }
              }
            }

            Debug.Assert(!isInf, $"ConvexPolytop.HrepToVrep_Geometric: The set of inequalities is unbounded!");

            if (!foundPrev) { // если точку ранее не нашли
              Vs.Add(zNew);
              orthToEdgeHPs.AddRange(zNewHPs);
              process.Enqueue((zNew, orthToEdgeHPs));
              // Console.WriteLine($"{zNew}");
            }
          }
        } while (J.Next());
      }

      return Vs;
    }


    /// <summary>
    /// Finds an initial vertex of the convex polytope using a naive approach by checking combinations of hyperplanes.
    /// </summary>
    /// <param name="HPs">List of hyperplanes defining the Hrep.</param>
    /// <param name="d">Dimension of the polytop.</param>
    /// <returns>A vertex of the polytop, or null if no vertex is found.</returns>
    public static Vector FindInitialVertex_Naive(List<HyperPlane> HPs, int d) {
      Combination          combination = new Combination(HPs.Count, d);
      Func<int, int, TNum> AFunc       = (r, l) => HPs[combination[r]].Normal[l];
      Func<int, TNum>      bFunc       = r => HPs[combination[r]].ConstantTerm;
      GaussSLE             gaussSLE    = new GaussSLE(d, d);
      bool                 belongs;
      bool                 goNext     = true;
      Vector?              firstPoint = null;
      do { // Перебираем все сочетания из d элементов из набора гиперплоскостей в поиске любой вершины.
        gaussSLE.SetSystem(AFunc, bFunc, d, d, GaussSLE.GaussChoice.All);
        gaussSLE.Solve();
        if (gaussSLE.GetSolution(out Vector point)) { // Ищем точку пересечения
          belongs = true;
          foreach (HyperPlane hp in HPs) {
            if (hp.ContainsPositive(point)) {
              belongs = false;

              break;
            }
          }
          if (belongs) {
            firstPoint = point;
            goNext     = false;
          }
        }
      } while (goNext && combination.Next());

      Debug.Assert(firstPoint is not null, $"ConvexPolytop.FindInitialVertex_Naive: Can't find a solution of a given system!");

      Debug.Assert
        (
         HPs.Select(hp => hp.Contains(firstPoint)).Count(b => b) >= firstPoint.SpaceDim
       , $"ConvexPolytop.FindInitialVertex_Naive: Wrong vertex found!"
        );

      return firstPoint;
    }

    /// <summary>
    /// Finds the initial vertex of a convex polytope using the simplex method.
    /// </summary>
    /// <param name="HPs">The list of hyperplanes that defines the system of inequalities.</param>
    /// <param name="activeHPs">The indices of the hyperplanes whose intersection forms the vertex.</param>
    /// //todo -- !!!
    /// <returns>The initial vertex.</returns>
    public static Vector? FindInitialVertex_Simplex(List<HyperPlane> HPs, out List<HyperPlane>? activeHPs) {
      SimplexMethod.SimplexMethodResult x = SimplexMethod.Solve(HPs, _ => Tools.One);

      if (x.Solution is null) {
        activeHPs = null;

        return null;
      }

      // Vector sol = new Vector(x.Solution);
      // Debug.Assert(HPs.Select(hp => hp.Contains(sol)).Count(b => b) >= sol.SpaceDim, $"ConvexPolytop.FindInitialVertex_Simplex: Wrong vertex found!");

      activeHPs = new List<HyperPlane>();
      foreach (int i in x.ActiveInequalitiesID) {
        activeHPs.Add(HPs[i]);
      }

      List<HyperPlane> forLambdaHPs = activeHPs;
      bool solExist =
        GaussSLE.Solve
          (
           (i, j) => forLambdaHPs[i].Normal[j]
         , i => forLambdaHPs[i].ConstantTerm
         , x.Solution.Length
         , GaussSLE.GaussChoice.All
         , out TNum[]? res
          );

      if (!solExist) {
        throw new ArgumentException
          (
           $"ConvexPolytop.HrepToVrep_Geometric: Gauss-Jordan elimination failed to find a solution for the system of equations derived from the active hyperplanes. This indicates an inconsistency."
          );
      }

      return new Vector(res, false);
    }

    /// <summary>
    /// Eliminates all redundant inequalities from a linear system.
    /// </summary>
    /// <param name="HPs">The given system of inequalities.</param>
    /// <returns>The not redundant system.</returns>
    public static List<HyperPlane> HRedundancyByGW(IEnumerable<HyperPlane> HPs) {
      ConvexPolytop init = CreateFromHalfSpaces(HPs);

      return init.ShiftToOrigin(out Vector innerPoint).Polar(true).Polar().Shift(innerPoint).Hrep;
    }

  }

}
