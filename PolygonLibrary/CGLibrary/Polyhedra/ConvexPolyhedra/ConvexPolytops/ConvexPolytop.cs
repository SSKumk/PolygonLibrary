using System.Globalization;
using System.IO;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

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
            Debug.Assert(vertex is not null, "ConvexPolytop.InnerPoint_Hrep: Current object is not bounded polytope!");
            Debug.Assert(activeHPs is not null, "ConvexPolytop.InnerPoint_Hrep: Current object is not bounded polytope!");

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

              Vector v = coEdgeLinSpace.OrthonormalVector(); // ищем направляющий вектор ребра

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


            Debug.Assert(!isInf, "ConvexPolytop.InnerPoint: The set of inequalities is unbounded!");


            _innerPoint = Vector.MulByNumAndAdd(directionIn, tMin / Tools.Two, vertex);
          }
          Debug.Assert(_innerPoint is not null, "ConvexPolytop.InnerPoint: The inner point should not be null!");

          Debug.Assert(Hrep.All(hp => hp.ContainsNegative(_innerPoint)), "ConvexPolytop.InnerPoint: This is not an inner point!");

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
            new List<HyperPlane>(FLrep.Lattice[^2].Select(n => new HyperPlane(n.AffBasis, (FLrep.Top.InnerPoint, false))).ToList());
          Debug.Assert(IsHrep, "ConvexPolytop.Hrep: _Hrep is null after constructing. Something went wrong!");
          _Hrep.Sort();

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
          Debug.Assert(IsFLrep, "ConvexPolytop.FLrep: _FLrep is null after constructing. Something went wrong!");

          _Vrep = null;

          return _FLrep;
        }
    }

    /// <summary>
    /// Gets the f-vector of the polytope, which contains the number of faces for each dimension.
    /// </summary>
    /// <value>
    /// An integer array where the element at index <c>k</c> is the number of <c>k</c>-dimensional faces.
    /// For example, <c>fVector[0]</c> is the number of vertices, <c>fVector[1]</c> is the number of edges, and so on.
    /// </value>
    public Vector fVector => new Vector(FLrep.Lattice.Select(lvl => lvl.Count));
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
        _      = Vrep; //Сразу инициировали (достали из решётки)
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
      _Hrep.Sort();
    }

    /// <summary>
    /// Constructs a ConvexPolytop using a face lattice (FLrep).
    /// </summary>
    /// <param name="fLrep">The face lattice defining the polytop.</param>
    private ConvexPolytop(FaceLattice fLrep) {
      SpaceDim = fLrep.Top.AffBasis.SpaceDim;
      _FLrep   = fLrep;
    }
#endregion

#region Fabrics
    /// <summary>
    /// Gets or constructs the FLrep of the polytope, and based on it creates new polytope.
    /// </summary>
    /// <returns>The convex polytope in FLrep.</returns>
    public ConvexPolytop GetInFLrep() => CreateFromFaceLattice(FLrep);

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
    public static ConvexPolytop CreateFromFaceLattice(FaceLattice faceLattice) => new ConvexPolytop(faceLattice);

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

      return CreateFromFaceLattice(new FaceLattice(lattice.Select(level => level.ToSortedSet()).ToList()));
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
      } while (!new AffineBasis(simplex).FullDim);


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
    /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle. Phi in [0, 2*Pi).</param>
    /// <param name="polarDivision">The number of partitions at a zenith angle. Theta in [0, Pi].
    ///   This parameter affects the geometry starting from 3D; in 2D it is ignored.</param>
    /// <returns>A convex polytope as Vrep representing the sphere in hD.</returns>
    public static ConvexPolytop Sphere(Vector center, TNum radius, int azimuthsDivisions, int polarDivision)
      => Ellipsoid(azimuthsDivisions, polarDivision, center, Vector.Ones(center.SpaceDim) * radius);

    /// <summary>
    /// Makes a hD-ellipsoid as Vrep with given semi-axis.
    /// </summary>
    /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle.</param>
    /// <param name="polarDivision">The number of partitions at a zenith angle. In 2D this parameter is ignored.</param>
    /// <param name="center">The center of an ellipsoid.</param>
    /// <param name="semiAxis">The vector where each coordinate represents the length of the corresponding semi-axis.</param>
    /// <returns>A convex polytope as Vrep representing the ellipsoid in hD.</returns>
    public static ConvexPolytop Ellipsoid(int azimuthsDivisions, int polarDivision, Vector center, Vector semiAxis) {
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
      Debug.Assert(azimuthsDivisions >= 3, "ConvexPolytop.Ellipsoid: The azimuthsDivisions should be greater than 2.");

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
        Vector e = radius * Vector.MakeOrth(center.SpaceDim, i);
        ball.Add(center + e);
        ball.Add(center - e);
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


    /// <summary>
    /// Creates a cone-like polytope in (d+1) dimensions representing a distance function.
    /// The distance metric is defined by the shape of the 'unitBall' polytope (Minkowski distance).
    /// The resulting cone has its apex at 'targetPoint' (at height 0) and its base at height 'scaleFactor'.
    /// </summary>
    /// <param name="unitBall">The convex polytope that acts as the unit ball for the distance norm.</param>
    /// <param name="targetPoint">The point to which distance is measured; becomes the apex of the cone.</param>
    /// <param name="scaleFactor">The scaling factor for the 'unitBall' and the height of the cone's base.</param>
    public static ConvexPolytop BuildDistanceEpigraph(ConvexPolytop unitBall, Vector targetPoint, TNum scaleFactor) {
      int newDim = unitBall.PolytopDim + 1;
      ConvexPolytop lifted =
        unitBall
         .Scale(scaleFactor, Vector.Zero(unitBall.PolytopDim))
         .Shift(targetPoint)
         .LiftUp(newDim, scaleFactor);
      SortedSet<Vector> toConv = new SortedSet<Vector>(lifted.Vrep) { targetPoint.LiftUp(newDim, Tools.Zero) };

      return CreateFromPoints(toConv, true);
    }

    /// <summary>
    /// Constructs the epigraph of the distance function from a given polytope <paramref name="P"/>,
    /// measured using a norm defined by the <paramref name="ballCreator"/>.
    /// </summary>
    /// <remarks>
    /// The resulting epigraph is the convex hull of two sets:
    /// 1. The input polytope <paramref name="P"/> lifted to height 0.
    /// 2. The Minkowski sum of <paramref name="P"/> and a ball of radius <paramref name="k"/>, lifted to height <paramref name="k"/>.
    /// This creates a skewed "cone" over <paramref name="P"/>.
    /// </remarks>
    /// <param name="P">The polytope to which the distance is measured.</param>
    /// <param name="k">The radius of the norm ball and the height of the epigraph's upper base.</param>
    /// <param name="ballCreator">A factory function that creates a norm ball of a given radius centered at the origin.</param>
    /// <returns>A new polytope representing the distance epigraph.</returns>
    private static ConvexPolytop DistanceToPolytope(ConvexPolytop P, TNum k, Func<Vector, TNum, ConvexPolytop> ballCreator) {
      ConvexPolytop upperBase = DistTo_MakeBase(P, k, ballCreator).LiftUp(P.SpaceDim + 1, k);
      ConvexPolytop lowerBase = P.LiftUp(P.SpaceDim + 1, Tools.Zero);

      SortedSet<Vector> allVertices = new SortedSet<Vector>(upperBase.Vrep);
      allVertices.UnionWith(lowerBase.Vrep);

      return CreateFromPoints(allVertices, true);
    }

    /// <summary>
    /// Computes the Minkowski sum of a polytope <paramref name="P"/> and a norm ball.
    /// This forms the upper base of a distance epigraph.
    /// </summary>
    /// <param name="P">The input polytope.</param>
    /// <param name="radius">The radius of the norm ball.</param>
    /// <param name="ballCreator">A factory function that creates the norm ball.</param>
    /// <returns>The resulting polytope from the Minkowski sum.</returns>
    public static ConvexPolytop DistTo_MakeBase(ConvexPolytop P, TNum radius, Func<Vector, TNum, ConvexPolytop> ballCreator) {
      ConvexPolytop bigP = MinkowskiSum.BySandipDas(ballCreator(Vector.Zero(P.SpaceDim), radius), P);

      return bigP;
    }

    /// <summary>
    /// Builds an epigraph for the distance to a polytope <paramref name="P"/>, measured using the L1 (Manhattan) norm.
    /// </summary>
    /// <remarks>
    /// Following common convention, this method uses 'L1' to refer to the l_1 vector norm.
    /// </remarks>
    /// <param name="P">The polytope to which the distance is measured.</param>
    /// <param name="scaleFactor">The radius of the L1-ball and the height of the epigraph's upper base.</param>
    /// <returns>A new polytope representing the distance epigraph.</returns>
    public static ConvexPolytop BuildDistanceEpigraph_ToPolytope_L1(ConvexPolytop P, TNum scaleFactor)
      => DistanceToPolytope(P, scaleFactor, Ball_1);

    /// <summary>
    /// Builds an epigraph for the distance to a polytope <paramref name="P"/>, measured using the Linf (Chebyshev) norm.
    /// </summary>
    /// <remarks>
    /// Following common convention, this method uses 'Linf' to refer to the l_inf vector norm.
    /// </remarks>
    /// <param name="P">The polytope to which the distance is measured.</param>
    /// <param name="scaleFactor">The radius of the Linf-ball and the height of the epigraph's upper base.</param>
    /// <returns>A new polytope representing the distance epigraph.</returns>
    public static ConvexPolytop BuildDistanceEpigraph_ToPolytope_Linf(ConvexPolytop P, TNum scaleFactor)
      => DistanceToPolytope(P, scaleFactor, Ball_oo);

    /// <summary>
    /// Builds an epigraph for the distance to a polytope <paramref name="P"/>, measured using the L2 (Euclidean) norm.
    /// </summary>
    /// <remarks>
    /// Following common convention, this method uses 'L2' to refer to the l_2 vector norm.
    /// </remarks>
    /// <param name="P">The polytope to which the distance is measured.</param>
    /// <param name="scaleFactor">The radius of the Linf-ball and the height of the epigraph's upper base.</param>
    /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle.</param>
    /// <param name="polarDivision">The number of partitions at the zenith angle.</param>
    /// <returns>A polytope representing the distance to the polytope P in ball_2 norm.</returns>
    public static ConvexPolytop BuildDistanceEpigraph_ToPolytope_L2(
        ConvexPolytop P
      , TNum          scaleFactor
      , int           azimuthsDivisions
      , int           polarDivision
      )
      => DistanceToPolytope
        (
         P
       , scaleFactor
       , (center, radius) => Sphere(center, radius, azimuthsDivisions, polarDivision)
        );

    /// <summary>
    /// Builds an epigraph for the distance to a point, measured using the L1 (Manhattan) norm.
    /// The result is a cone with its apex at the origin of the (d+1) space and its base being an L1-ball centered at <paramref name="point"/> and lifted to height <paramref name="scaleFactor"/>.
    /// </summary>
    /// <remarks>
    /// Following common convention, this method uses 'L1' to refer to the l_1 vector norm.
    /// </remarks>
    /// <param name="point">The center of the distance function; the base of the cone will be centered here.</param>
    /// <param name="scaleFactor">The radius of the L1-ball and the height of the cone's base.</param>
    /// <returns>A new polytope representing the distance epigraph.</returns>
    public static ConvexPolytop BuildDistanceEpigraph_L1(Vector point, TNum scaleFactor)
      => DistanceToPoint(point, scaleFactor, Ball_1);

    /// <summary>
    /// Builds an epigraph for the distance to a point, measured using the Linf (Chebyshev) norm.
    /// The result is a cone with its apex at the origin of the (d+1) space and its base being a Linf-ball centered at <paramref name="point"/> and lifted to height <paramref name="scaleFactor"/>.
    /// </summary>
    /// <remarks>
    /// Following common convention, this method uses 'Linf' to refer to the l_inf vector norm.
    /// </remarks>
    /// <param name="point">The center of the distance function; the base of the cone will be centered here.</param>
    /// <param name="scaleFactor">The radius of the Linf-ball and the height of the cone's base.</param>
    /// <returns>A new polytope representing the distance epigraph.</returns>
    public static ConvexPolytop BuildDistanceEpigraph_Linf(Vector point, TNum scaleFactor)
      => DistanceToPoint(point, scaleFactor, Ball_oo);

    /// <summary>
    /// Builds an epigraph for the distance to a point, measured using the L2 (Euclidean) norm.
    /// The result is a cone with its apex at the origin of the (d+1) space and its base being a L2-ball centered at <paramref name="point"/> and lifted to height <paramref name="scaleFactor"/>.
    /// </summary>
    /// <remarks>
    /// Following common convention, this method uses 'L2' to refer to the l_2 vector norm.
    /// </remarks>
    /// <param name="point">The point distance to is computed.</param>
    /// <param name="azimuthsDivisions">The number of partitions at each azimuthal angle.</param>
    /// <param name="polarDivision">The number of partitions at a zenith angle.</param>
    /// <param name="scaleFactor">The radius of the L2-ball and the height of the cone's base.</param>
    /// <returns>A new polytope representing the distance epigraph.</returns>
    public static ConvexPolytop BuildDistanceEpigraph_L2(Vector point, int azimuthsDivisions, int polarDivision, TNum scaleFactor)
      => DistanceToPoint(point, scaleFactor, Ball_2FuncCreator(azimuthsDivisions, polarDivision));

    /// <summary>
    /// Creates a factory function for generating L2-balls (spheres) with a fixed mesh resolution.
    /// </summary>
    /// <param name="azimuthsDivisions">The number of subdivisions for the azimuthal angles (longitude).</param>
    /// <param name="polarDivision">The number of subdivisions for the polar angle (latitude).</param>
    /// <returns>A function that takes a center and radius and returns a sphere polytope.</returns>
    public static Func<Vector, TNum, ConvexPolytop> Ball_2FuncCreator(int azimuthsDivisions, int polarDivision) {
      return (center, radius) => Sphere(center, radius, azimuthsDivisions, polarDivision);
    }

    /// <summary>
    /// Makes the convex polytope representing the distance to the point in (dim)-dimensional space.
    /// </summary>
    /// <param name="point">The point distance to is computed.</param>
    /// <param name="k">The value of the last coordinate in the (dim + 1)-dimensional space.</param>
    /// <param name="ballCreator">Function that creates balls.</param>
    /// <returns>A polytope representing the distance to the specified <paramref name="point"/>.</returns>
    private static ConvexPolytop DistanceToPoint(Vector point, TNum k, Func<Vector, TNum, ConvexPolytop> ballCreator) {
      ConvexPolytop ball   = ballCreator(point, k);
      ConvexPolytop toConv = ball.LiftUp(point.SpaceDim + 1, k);
      Vector        apex   = point.LiftUp(point.SpaceDim + 1, Tools.Zero);

      toConv.Vrep.Add(apex);

      //conv{...}
      return CreateFromPoints(toConv.Vrep, true);
    }
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
            FLrep[^2].Where(hnode => new HyperPlane(hnode.AffBasis, (InnerPoint, false)).ContainsPositive(point));
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
          HyperPlane hp        = new HyperPlane(oldNode.AffBasis, (InnerPoint, false));
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

        return CreateFromFaceLattice(new FaceLattice(newFL));
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
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
      switch (rep) {
        case Rep.Vrep:  WriteAsVRep(pw, this); break;
        case Rep.Hrep:  WriteAsHRep(pw, this); break;
        case Rep.FLrep: WriteAsFLRep(pw, this); break;
      }
      pw.Flush();
    }

    /// <summary>
    /// Writes the convex polytope to a file with the specified name and format.
    /// </summary>
    /// <param name="path">The directory path where the file will be written.</param>
    /// <param name="name">The name of the file without extension.</param>
    /// <param name="rep">
    /// The representation format of the convex polytope.
    /// This determines whether the polytope will be written as a Vrep, Hrep, or FLrep.
    /// </param>
    public void WriteIn(string path, string name, Rep rep) {
      string            fullPath = Path.Combine(path, name + ".cpolytope");
      using ParamWriter pw       = new ParamWriter(fullPath);
      WriteIn(pw, rep);
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
      SortedSet<Vector> x =
        HrepToVrep_Geometric(xList)
     ?? throw new InvalidOperationException("ConvexPolytop.SectionByHyperPlane: Set is unbounded!");

      List<HyperPlane> yList = new List<HyperPlane> { hp_ };
      yList.AddRange(Hrep);
      SortedSet<Vector> y =
        HrepToVrep_Geometric(yList)
     ?? throw new InvalidOperationException("ConvexPolytop.SectionByHyperPlane: Set is unbounded!");

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
        return CreateFromFaceLattice(FLrep.VertexTransform(v => v + s));
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
        return CreateFromFaceLattice(FLrep.VertexTransform(v => Matrix.MultRowVectorByMatrix(v, rotate)));
      }
      if (_Vrep is not null) {
        return CreateFromPoints(Vrep.Select(v => Matrix.MultRowVectorByMatrix(v, rotate)));
      }

      return CreateFromHalfSpaces
        (Hrep.Select(hp => new HyperPlane(Matrix.MultRowVectorByMatrix(hp.Normal, rotate), hp.ConstantTerm)));
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
    /// <param name="k">The scaling factor. Can be any real number.</param>
    /// <param name="origin">The origin point for scaling.</param>
    /// <returns>
    /// A new <see cref="ConvexPolytop"/> that is a scaled version of the current polytope.
    /// </returns>
    public ConvexPolytop Scale(TNum k, Vector origin) {
      if (IsFLrep) {
        return CreateFromFaceLattice(FLrep.VertexTransform(v => origin + (v - origin) * k));
      }
      if (IsVrep) {
        return CreateFromPoints(Vrep.Select(v => origin + (v - origin) * k));
      }

      if (Tools.GE(k)) {
        return CreateFromHalfSpaces
          (Hrep.Select(hp => new HyperPlane(hp.Normal, (Tools.One - k) * origin * hp.Normal + hp.ConstantTerm * k)));
      }

      // For k < 0 the affine map reverses the inequality direction, so the normal must be flipped.
      return CreateFromHalfSpaces
        (
         Hrep.Select
           (
            hp => new HyperPlane
              (
               -hp.Normal
             , -hp.ConstantTerm * k - (Tools.One - k) * origin * hp.Normal
              )
           )
        );
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
      for (int i = 0; i < points.Count; i++) {
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

      ConvexPolytop other = (ConvexPolytop)obj;
      if (this.IsFLrep && other.IsFLrep) {
        return FLEquals(other);
      }

      if (this.IsVrep && other.IsVrep) {
        return this.Vrep.SetEquals(other.Vrep);
      }

      if (this.IsHrep && other.IsHrep) {
        return this.Hrep.SequenceEqual(other.Hrep);
      }

      // produce FLrep and compare
      return FLEquals(other);
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

      List<Vector>         Vs          = new List<Vector>();
      Combination          combination = new Combination(m, d);
      Func<int, int, TNum> AFunc       = (r, l) => HPs[combination[r]].Normal[l];
      Func<int, TNum>      bFunc       = r => HPs[combination[r]].ConstantTerm;
      bool                 belongs;
      GaussSLE             gaussSLE = new GaussSLE(d, d);
      do { // Перебираем все сочетания из d элементов из набора гиперплоскостей
        gaussSLE.SetSystem(AFunc, bFunc, d, d);
        gaussSLE.Solve();
        if (gaussSLE.GetSolution(out Vector? point)) { // Ищем точку пересечения
          belongs = true;
          foreach (HyperPlane hp in HPs) {
            var x = hp.Eval(point);
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


    /// <summary>
    /// Сравнивает два SortedSet по содержимому.
    /// </summary>
    public class SortedIntSetComparer : IEqualityComparer<SortedSet<int>> {

      public bool Equals(SortedSet<int>? x, SortedSet<int>? y) {
        if (ReferenceEquals(x, y))
          return true;
        if (x is null || y is null)
          return false;

        return x.SetEquals(y);
      }

      public int GetHashCode(SortedSet<int>? obj) {
        int hashCode = 0;
        if (obj is not null) {
          foreach (int item in obj) {
            hashCode = HashCode.Combine(hashCode, item);
          }
        }

        return hashCode;
      }

    }

    // todo: научиться работать не с полноразмернымми многогранниками
    /// <summary>
    /// Converts the Hrep of a convex polytope to Vrep using a geometric-inspired algorithm. Full-dimensional only!
    /// </summary>
    /// <param name="HPs">List of hyperplanes defining the Hrep.</param>
    /// <returns>The Vrep of the convex polytop.</returns>
    public static SortedSet<Vector>? HrepToVrep_Geometric(List<HyperPlane> HPs) {
// #if DEBUG
      // int[] bins = new int[28];
// #endif

      SortedSet<Vector> Vs       = new SortedSet<Vector>(); // копим точки результата
      SortedSet<Vector> toRemove = new SortedSet<Vector>(); // метим точки к удалению
      int               d        = HPs.First().Normal.SpaceDim;

      // словарь гиперплоскость --> её индекс в списке HPs
      SortedDictionary<HyperPlane, int> HP2ind = new SortedDictionary<HyperPlane, int>();
      for (int i = 0; i < HPs.Count; i++) {
        HP2ind[HPs[i]] = i;
      }
      // Для быстрого сравнения двух наборов индексов
      HashSet<SortedSet<int>> HPsings = new HashSet<SortedSet<int>>(new SortedIntSetComparer());

      // Этап 1. Поиск какой-либо вершины и определение гиперплоскостей, которым она принадлежит
      Vector? firstPoint = FindInitialVertex_Simplex(HPs, out List<HyperPlane>? activeHPs);
      if (firstPoint is null || activeHPs is null) { return null; }


      Vs.Add(new Vector(firstPoint)); // нашли первую вершину
      HPsings.Add(new SortedSet<int>(activeHPs.Select(hp => HP2ind[hp])));


      // Этап 2. Поиск всех остальных вершин
      Queue<(Vector, List<HyperPlane>)> process = new Queue<(Vector, List<HyperPlane>)>();
      process.Enqueue((Vs.First(), activeHPs));

      HyperPlane[] edge = new HyperPlane[d - 1];
      // Обходим по рёбрам
      while (process.TryDequeue(out (Vector, List<HyperPlane>) elem)) {
        (Vector z, List<HyperPlane> Hz) = elem;

        Debug.Assert
          (Hz.Count >= d, $"ConvexPolytop.HrepToVrep_Geometric: The number of planes should be at least {d}. Found {Hz.Count}");

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
          Vector v = coEdgeLinSpace.OrthonormalVector();

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
            List<HyperPlane> zNewDefiningHPs = new List<HyperPlane>();
            List<HyperPlane> zNewActiveHPs   = new List<HyperPlane>();
            TNum             tMin            = Tools.Zero;
            bool             isInf           = true;
            Vector           zNew            = Vector.Zero(d);
            Vector           zEdge           = Vector.Zero(d);

            foreach (HyperPlane hp in HPs) {
              TNum denominator = hp.Normal * v;

              if (Tools.EQ(denominator)) { // Точка лежит на гиперплоскости
                if (hp.Contains(z)) {
                  zNewActiveHPs.Add(hp); // мы собираем все грани
                }
              }
              else {
                TNum ti = (hp.ConstantTerm - hp.Normal * z) / denominator;

                if (Tools.GT(ti)) {                  // учитываем только положительные ti
                  if (isInf || Tools.LT(ti, tMin)) { // первый кандидат или найден новый минимум
                    tMin  = ti;
                    isInf = false;
                    zNewDefiningHPs.Clear();
                    zNewDefiningHPs.Add(hp);
                  }
                  else if (Tools.EQ(ti, tMin)) { // если ti равен текущему минимуму, добавляем гиперплоскость
                    zNewDefiningHPs.Add(hp);
                  }
                }
              }
            }

            Debug.Assert(!isInf, "ConvexPolytop.HrepToVrep_Geometric: The set of inequalities is unbounded!");


            zEdge = v * tMin;
            zNew  = z + zEdge;
            // zNew  = Vector.MulByNumAndAdd(v, tMin, z);
            zNewActiveHPs.AddRange(zNewDefiningHPs);
            Debug.Assert
              (
               zNewDefiningHPs.All(hp => hp.Contains(zNew))
             , $"ConvexPolytop.Hre2Vrep_Geometric: A new point doesn't belong to the some hyper plane!"
              );

            if (Tools.LT(zEdge.Length, Tools.EpsG)) { // если очередное ребро "короткое"
              toRemove.Add(z);                        // пометим точку, из которой вышли
              toRemove.Add(zNew);                     // пометим точку, в которую пришли
            }

// #if DEBUG
            // Vector check = zNew - z;
            // if (Tools.GT(check.Length, Tools.One)) {
            //   bins[0]++;
            // }
            // else if (Tools.LT(check.Length, TNum.Pow(Tools.Two, TConv.FromInt(-26)))) {
            //   bins[27]++;
            // }
            // else {
            //   double len = TConv.ToDouble(check.Length);
            //   int    num = -(int)Math.Ceiling(Math.Log2(len));
            //   bins[num]++;
            // }
// #endif


            // встречалась ли эта точка ранее (определяем набором гиперплоскостей)
            if (HPsings.Add(new SortedSet<int>(zNewActiveHPs.Select(hp => HP2ind[hp])))) {
              Vs.Add(zNew);
              process.Enqueue((zNew, zNewActiveHPs));
            }
          }
        } while (J.Next());
      }

// #if DEBUG
      // using (ParamWriter pr =
      //        new ParamWriter
      //          (
      //           "F:\\Works\\IMM\\Аспирантура\\LDG\\_Out\\Oscillator-triangle\\Br\\0\\DoubleDouble.ddouble\\1E-15\\.distribution"
      //           // "F:\\Works\\IMM\\Аспирантура\\LDG\\_Out\\Oscillator-triangle\\Br\\0\\System.Double\\1E-08\\.distribution"
      //           // "F:\\Works\\IMM\\Аспирантура\\LDG\\_Out\\Oscillator-circle30\\Br\\0\\DoubleDouble.ddouble\\1E-15\\.distribution"
      //         , true
      //          )) {
      //   pr.Write($"[{string.Join(',', bins)}]");
      //   pr.WriteLine();
      // }
// #endif

      if (toRemove.Count > 0) {
        var x = FindClosePairs_Naive(Vs, Tools.EpsG);
        Vs.ExceptWith(toRemove);
        var y = FindClosePairs_Naive(Vs, Tools.EpsG);
// #if DEBUG
//         TNum minDiamPruned = MinimalDiameter(Vs);
//         Debug.Assert
//           (
//            minDiamPruned > Tools.EpsG
//          , $"ConvexPolytop.Hrep2Vrep_Geometric: A close points presents in the pruned set of vertices! Found: {minDiamPruned}"
//           );
// #endif

        Vs.UnionWith(MergePoints(toRemove)); // вернули "склеенные" точки в рой

// #if DEBUG
//         TNum minDiamUnion = MinimalDiameter(Vs);
//         Debug.Assert
//           (
//            minDiamUnion > Tools.EpsG
//          , $"ConvexPolytop.Hrep2Vrep_Geometric: A close points presents in the pruned set of vertices! Found: {minDiamUnion}"
//           );
// #endif

        Console.WriteLine($"h2v.toRem: {toRemove.Count}");
      }

      return Vs;
    }

    /// <summary>
    /// Находит все пары точек, расстояние между которыми меньше заданного порога.
    /// Реализовано с помощью простого перебора всех пар (сложность O(N²)).
    /// </summary>
    /// <param name="points">Входной набор ("рой") точек.</param>
    /// <param name="epsilon">Пороговое расстояние.</param>
    /// <returns>
    /// Словарь, где ключ - это точка, имеющая слишком близких соседей,
    /// а значение - список этих соседей.
    /// </returns>
    public static SortedDictionary<Vector, List<Vector>> FindClosePairs_Naive(
        IEnumerable<Vector> points
      , TNum                epsilon
      ) {
      var result    = new SortedDictionary<Vector, List<Vector>>();
      var pointList = points.ToList(); // Нужен доступ по индексу для эффективности

      if (pointList.Count < 2) {
        return result; // Не может быть пар
      }

      for (int i = 0; i < pointList.Count; i++) {
        for (int j = i + 1; j < pointList.Count; j++) {
          Vector p1 = pointList[i];
          Vector p2 = pointList[j];

          TNum distance = (p1 - p2).Length;

          if (Tools.LT(distance, epsilon)) {
            // Нашли "плохую" пару (p1, p2)

            // Добавляем p2 в список для p1
            if (!result.ContainsKey(p1)) {
              result[p1] = new List<Vector>();
            }
            result[p1].Add(p2);

            // Добавляем p1 в список для p2
            if (!result.ContainsKey(p2)) {
              result[p2] = new List<Vector>();
            }
            result[p2].Add(p1);
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Merges clusters of nearby points into their arithmetic mean.
    /// </summary>
    /// <param name="Ps">The initial set of points.</param>
    /// <returns>A set of points where each cluster of close points has been replaced by its average.</returns>
    /// <remarks>
    /// Two or more points are considered a cluster if the distance between them is less than <c>Tools.EpsG</c>.
    /// Clusters are detected and merged sequentially: after merging a cluster, the process restarts on the updated set.
    /// </remarks>
    public static SortedSet<Vector> MergePoints(IEnumerable<Vector> Ps) {
      List<Vector> points = Ps.ToList();

      if (points.Count < 2) {
        return points.ToSortedSet();
      }

      bool changed;
      do {
        changed = false;

        for (int i = 0; i < points.Count; i++) {
          SortedSet<Vector> cluster = new() { points[i] };

          for (int j = i + 1; j < points.Count; j++) {
            if (Tools.LT((points[i] - points[j]).Length, Tools.EpsG)) {
              cluster.Add(points[j]);
            }
          }

          if (cluster.Count > 1) {
            Vector average = Vector.Sum(cluster) / TConv.FromInt(cluster.Count);
            points.RemoveAll(p => cluster.Contains(p));
            points.Add(average);
            changed = true;

            break; // начинаем заново после замены
          }
        }
      } while (changed);

#if DEBUG
      if (points.Count > 1) {
        Debug.Assert
          (
           MinimalDiameter(points) > Tools.EpsG
         , $"ConvexPolytop.MergePoints: The merging process went wrong! Found: {MinimalDiameter(points)}"
          );
      }
#endif

      return points.ToSortedSet();
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
        gaussSLE.SetSystem(AFunc, bFunc, d, d);
        gaussSLE.Solve();
        if (gaussSLE.GetSolution(out Vector? point)) { // Ищем точку пересечения
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

      Debug.Assert(firstPoint is not null, "ConvexPolytop.FindInitialVertex_Naive: Can't find a solution of a given system!");

      Debug.Assert
        (
         HPs.Select(hp => hp.Contains(firstPoint)).Count(b => b) >= firstPoint.SpaceDim
       , "ConvexPolytop.FindInitialVertex_Naive: Wrong vertex found!"
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

      activeHPs = new List<HyperPlane>();
      foreach (int i in x.ActiveInequalitiesID) {
        activeHPs.Add(HPs[i]);
      }

      bool solExist = GaussSLE.Solve(activeHPs, GaussSLE.GaussChoice.All, out TNum[]? res);

      if (!solExist) {
        throw new ArgumentException
          (
           "ConvexPolytop.HrepToVrep_Geometric: Gauss-Jordan elimination failed to find a solution for the system of equations derived from the active hyperplanes. This indicates an inconsistency."
          );
      }

      Vector initVertex = new Vector(res!, false);
      activeHPs = HPs.Where(hp => hp.Contains(initVertex)).ToList();

      return initVertex;
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
