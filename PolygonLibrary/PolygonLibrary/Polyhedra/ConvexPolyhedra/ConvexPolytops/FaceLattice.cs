namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Representation of a Convex Polytop as a Face Lattice:
  /// A face lattice is a lattice where the maximum is the polytop itself.
  /// The nodes of the lattice correspond to the faces of the polytop in their respective dimensions.
  /// </summary>
  public class FaceLattice {

#region Fields and properties
    /// <summary>
    /// The maximum element in the lattice.
    /// </summary>
    public FLNode Top { get; init; }

    /// <summary>
    /// The lattice is represented level by level.
    /// At each level, there is a set that contains all nodes (faces of polytop) of that level's dimension.
    /// </summary>
    public List<SortedSet<FLNode>> Lattice { get; init; }

    /// <summary>
    /// Set of vertices that form convex polytop.
    /// </summary>
    public SortedSet<Vector> Vertices => Top.Vertices;

    /// <summary>
    /// Gets the total number of all k-faces in the lattice, except 0-faces.
    /// </summary>
    /// <value>A number of nonzero k-faces in the lattice.</value>
    public int NumberOfNonZeroKFaces => NumberOfKFaces - Lattice[0].Count;

    /// <summary>
    /// Gets the total number of all k-faces in the lattice, except 0-faces.
    /// </summary>
    /// <value>A number of nonzero k-faces in the lattice.</value>
    public int NumberOfKFaces => Lattice.Sum(lvl => lvl.Count);
#endregion

#region Constructors
    /// <summary>
    /// The vertex forms a one-element lattice.
    /// </summary>
    /// <param name="point">The point at which a face lattice is formed.</param>
    public FaceLattice(Vector point) {
      Top     = new FLNode(point);
      Lattice = new List<SortedSet<FLNode>>() { new SortedSet<FLNode>() { Top } };
    }

    /// <summary>
    /// Construct a face lattice based on given lattice.
    /// </summary>
    /// <param name="lattice">The lattice.</param>
    public FaceLattice(List<SortedSet<FLNode>> lattice) {
      Top     = lattice[^1].First();
      Lattice = lattice;
    }

    /// <summary>
    /// Construct a face lattice based on a maximum element.
    /// </summary>
    /// <param name="Maximum">The maximum node.</param>
    public FaceLattice(FLNode Maximum) {
      Top     = Maximum;
      Lattice = Maximum.GetAllLevels();
    }
#endregion

#region Functions
    /// <summary>
    /// Transforms the lattice by applying a given function to each vertex of the lattice.
    /// </summary>
    /// <param name="transformFunc">The function to be applied to each vertex. This function takes a vertex of the lattice and returns a new point.</param>
    /// <returns>A new FaceLattice where each vertex has been transformed by the given function.</returns>
    public FaceLattice LinearVertexTransform(Func<Vector, Vector> transformFunc) {
      List<SortedSet<FLNode>> newFL = new List<SortedSet<FLNode>>();
      for (int i = 0; i <= Top.PolytopDim; i++) {
        newFL.Add(new SortedSet<FLNode>());
      }

      SortedDictionary<FLNode, FLNode> oldToNew = new SortedDictionary<FLNode, FLNode>();

      //Отдельно обрабатываем случай d == 0
      foreach (FLNode vertex in Lattice[0]) {
        FLNode newVertex = new FLNode(transformFunc(vertex.Vertices.First()));
        oldToNew.Add(vertex, newVertex);
        newFL[0].Add(newVertex);
      }

      for (int i = 1; i < newFL.Count; i++) {
        foreach (FLNode node in Lattice[i]) {
          List<FLNode> newSub = node.Sub.Select(n => oldToNew[n]).ToList();

          FLNode newNode = new FLNode(newSub);
          oldToNew.Add(node, newNode);
          newFL[i].Add(newNode);
        }
      }

      return new FaceLattice(newFL);
    }
#endregion

#region Compares
    public override bool Equals(object? obj) {
      if (obj == null || GetType() != obj.GetType()) {
        return false;
      }

      FaceLattice other = (FaceLattice)obj;

      if (this.Lattice.Count != other.Lattice.Count) {
        return false;
      }

      bool isEqual = true;
      for (int i = this.Top.PolytopDim; isEqual && i > -1; i--) {
        foreach (var thisNode in this.Lattice[i]) {
          if (!other.Lattice[i].TryGetValue(thisNode, out FLNode? otherNode)) {
            isEqual = false;
          }
          else {
            isEqual = isEqual && thisNode.Sub.SetEquals(otherNode.Sub);
            isEqual = isEqual && thisNode.Super.SetEquals(otherNode.Super);
          }
        }
        if (isEqual) {
          foreach (var otherNode in other.Lattice[i]) {
            if (!this.Lattice[i].TryGetValue(otherNode, out FLNode? thisNode)) {
              isEqual = false;
            }
            else {
              isEqual = isEqual && otherNode.Sub.SetEquals(thisNode.Sub);
              isEqual = isEqual && otherNode.Super.SetEquals(thisNode.Super);
            }
          }
        }
      }

      return isEqual;
    }
#endregion

  }

  /// <summary>
  /// The node of the face lattice. It stores references to the supernodes and sub-nodes.
  /// In addition, it maintains a set of vertices representing the face.
  /// </summary>
  public class FLNode : IComparable<FLNode> {

#region Data and properties
    /// <summary>
    /// Gets the d-dimensional point 'p' which lies within P and does not lie on any faces of P.
    /// </summary>
    public Vector InnerPoint { get; }

    /// <summary>
    /// Gets the list of d-dimensional points which forms the affine space (not a Affine basis) corresponding to this polytop.
    /// </summary>
    public AffineBasis AffBasis { get; }

    /// <summary>
    /// The list of the supernodes, whose Dim = this.Dim + 1.
    /// </summary>
    public SortedSet<FLNode> Super { get; } = new SortedSet<FLNode>();

    /// <summary>
    /// The list of the subnodes, which Dim = this.Dim - 1.
    /// </summary>
    public SortedSet<FLNode> Sub { get; } = new SortedSet<FLNode>();

    /// <summary>
    /// Reference to the associated polytop to this node.
    /// </summary>
    private SortedSet<Vector> Polytop { get; set; }

    /// <summary>
    /// Maps the dimension to the set of nodes within this dimension.
    /// </summary>
    private readonly Dictionary<int, SortedSet<FLNode>> _levelNodes = new Dictionary<int, SortedSet<FLNode>>();

    private Dictionary<int, SortedSet<FLNode>> LevelNodes {
      get
        {
          if (_levelNodes.Count == 0) {
            ConstructLevelNodes();
          }

          return _levelNodes;
        }
    }

    /// <summary>
    /// The vertices of the polytop.
    /// </summary>
    public SortedSet<Vector> Vertices => Polytop;

    /// <summary>
    /// The dimension of the associated polytop.
    /// </summary>
    public int PolytopDim => AffBasis.SubSpaceDim;
#endregion

#region Constructors
    /// <summary>
    /// Constructs an instance of FLNode as a vertex.
    /// </summary>
    /// <param name="vertex">Vertex on which this instance will be created.</param>
    public FLNode(Vector vertex) {
      Polytop    = new SortedSet<Vector>() { vertex };
      InnerPoint = vertex;
      AffBasis   = new AffineBasis(vertex);
    }

    /// <summary>
    /// AUX. For Constructing FaceLattice in GW procedure.
    /// </summary>
    /// <param name="Vs">The points of the Polytop.</param>
    internal FLNode(IEnumerable<Vector> Vs) {
      Polytop    = new SortedSet<Vector>(Vs); // todo мб тут можно и не копировать
      InnerPoint = Vector.Zero(1);
      AffBasis   = new AffineBasis(Vs.First());
    }

    /// <summary>
    /// Constructs a node based on its sub-nodes.
    /// </summary>
    /// <param name="sub">The set of sub-nodes which is the set of sub-nodes of the node to be created.</param>
    public FLNode(List<FLNode> sub) {
      Polytop = new SortedSet<Vector>(sub.SelectMany(s => s.Vertices).ToSortedSet());
      Sub     = new SortedSet<FLNode>(sub);

      foreach (FLNode subNode in sub) {
        subNode.AddSuper(this);
      }

      InnerPoint = new Vector((new Vector(Sub.First().InnerPoint) + new Vector(Sub.Last().InnerPoint)) / Tools.Two);

      FLNode      subF   = Sub.First();
      AffineBasis affine = new AffineBasis(subF.AffBasis);
      affine.AddVector(InnerPoint - subF.InnerPoint);
      AffBasis = affine;
    }

    /// <summary>
    /// Constructs an instance of FLNode as an face. (Aux for MinkSumSDas).
    /// </summary>
    /// <param name="Vs">The vertices of the face.</param>
    /// <param name="innerPoint">Inner point of the face.</param>
    /// <param name="aBasis">The affine basis of the face.</param>
    internal FLNode(SortedSet<Vector> Vs, Vector innerPoint, AffineBasis aBasis) {
      Polytop       = Vs;
      InnerPoint    = innerPoint;
      this.AffBasis = aBasis;
    }
#endregion

#region Internal methods
    /// <summary>
    /// Gets the requested level in the node structure. If there is no key = dim, then an empty set is produced.
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried.</returns>
    /// </summary>
    private SortedSet<FLNode> GetLevel(int dim)
      => LevelNodes.TryGetValue(dim, out SortedSet<FLNode>? value) ? value : new SortedSet<FLNode>();

    /// <summary>
    /// Gets the requested level in the node structure, that lies below this node.
    /// Otherwise returns empty set.
    /// </summary>
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried. If it lies above this node, it returns the empty set.</returns>
    internal IEnumerable<FLNode> GetLevelBelowNonStrict(int dim) => dim > PolytopDim ? new SortedSet<FLNode>() : GetLevel(dim);

    /// <summary>
    /// Adds a given node to the set of super nodes for this node.
    /// </summary>
    /// <param name="node">The node to be added.</param>
    internal void AddSub(FLNode node) => Sub.Add(node);

    /// <summary>
    /// Adds a given node to the set of sub nodes for this node.
    /// </summary>
    /// <param name="node">The node to be added.</param>
    internal void AddSuper(FLNode node) => Super.Add(node);


    /// <summary>
    /// Construct the mapping that takes the dimension and maps it onto the set of nodes in that dimension,
    /// which are either sub-nodes or supernodes of it.
    /// </summary>
    private void ConstructLevelNodes() {
      // добавили себя
      _levelNodes.Add(PolytopDim, new SortedSet<FLNode>() { this });

      // собираем верх
      SortedSet<FLNode> superNodes = Super;
      int               d          = PolytopDim;
      while (superNodes.Count != 0) {
        d++;
        _levelNodes.Add(d, superNodes);
        superNodes = superNodes.SelectMany(node => node.Super).ToSortedSet();
      }

      // собираем низ
      SortedSet<FLNode> prevNodes = Sub;
      d = PolytopDim;
      while (prevNodes.Count != 0) {
        d--;
        _levelNodes.Add(d, prevNodes);
        prevNodes = prevNodes.SelectMany(node => node.Sub).ToSortedSet();
      }
    }
#endregion

#region Functions
    /// <summary>
    /// Gets the entire levelNodes structure.
    /// </summary>
    /// <returns>Returns the entire levelNodes structure.</returns>
    public List<SortedSet<FLNode>> GetAllLevels() {
      List<SortedSet<FLNode>> allLevels = new List<SortedSet<FLNode>>();
      for (int i = 0; i < LevelNodes.Count; i++) {
        allLevels.Add(LevelNodes[i]);
      }

      return allLevels;
    }

    /// <summary>
    /// Retrieves all sub-faces of the node, including the node itself.
    /// </summary>
    /// <returns>The collection that contains all non-strict sub-faces of the node.</returns>
    public IEnumerable<FLNode> AllNonStrictSub => LevelNodes.Where(ln => ln.Key <= PolytopDim).SelectMany(ln => ln.Value);
#endregion

    /// <summary>
    /// In assumption that all nodes of a lower dimension are correct, it creates a new Polytop based on vertices of the subs.
    /// </summary>
    public void ReconstructPolytop() {
      if (PolytopDim != 0) {
        Polytop = new SortedSet<Vector>(Sub.SelectMany(s => s.Vertices));
      }
    }

#region Compares
    /// <summary>
    /// The equality function for FLNode. It checks only the Node itself, not its neighbors.
    /// </summary>
    /// <param name="obj">Compare this FLNode with another object.</param>
    /// <returns>Two FLNodes are considered equal if obj is a FLNode:
    /// 1) polytopes corresponding to the nodes are equal.
    /// </returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      FLNode other = (FLNode)obj;

      return this.Polytop.SetEquals(other.Polytop);
    }

    /// <summary>
    /// Compares two FLNodes.
    /// </summary>
    /// <param name="other">The FLNode object to compare with.</param>
    /// <returns>
    /// Returns '-1' if the number of vertices in 'this' is less than that of 'other'.
    /// Returns '+1' if the number of vertices in 'this' is greater than that of 'other'.
    /// Otherwise, it returns the result based on a lexicographical comparison of their elements.
    /// If all corresponding elements are equal, then the sets are considered equal.
    /// </returns>
    public int CompareTo(FLNode? other) {
      if (other is null) { return 1; } // null < this (always)

      if (this.Vertices.Count < other.Vertices.Count) { // this < other
        return -1;
      }

      if (this.Vertices.Count > other.Vertices.Count) { // this > other
        return 1;
      }

      SortedSet<Vector>.Enumerator e1       = this.Vertices.GetEnumerator();
      SortedSet<Vector>.Enumerator e2       = other.Vertices.GetEnumerator();
      Comparer<Vector>             comparer = Comparer<Vector>.Default;

      while (e1.MoveNext() && e2.MoveNext()) {
        int compare = comparer.Compare(e1.Current, e2.Current);
        switch (compare) {
          case < 0: // this < other
            return -1;
          case > 0: // this > other
            return 1;
        }
      }

      return 0;
    }
#endregion

  }

}
