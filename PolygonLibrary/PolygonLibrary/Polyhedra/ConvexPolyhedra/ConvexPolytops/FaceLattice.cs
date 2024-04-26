using System;
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
    public List<HashSet<FLNode>> Lattice { get; init; }

    /// <summary>
    /// Set of vertices that form convex polytop.
    /// </summary>
    public VectorHashSet Vertices => Top.Vertices;

    /// <summary>
    /// Gets the total number of all k-faces in the lattice, except 0-faces.
    /// </summary>
    /// <value>A number of nonzero k-faces in the lattice.</value>
    public int NonZeroKFacesAmount => Lattice.Sum(lvl => lvl.Count) - Lattice[0].Count;
#endregion

#region Constructors
    /// <summary>
    /// The vertex forms a one-element lattice.
    /// </summary>
    /// <param name="point">The point at which a face lattice is formed.</param>
    public FaceLattice(Vector point) {
      Top     = new FLNode(point);
      Lattice = new List<HashSet<FLNode>>() { new HashSet<FLNode>() { Top } };
    }

    /// <summary>
    /// Construct a face lattice based on given lattice.
    /// </summary>
    /// <param name="lattice">The lattice.</param>
    public FaceLattice(List<HashSet<FLNode>> lattice) {
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
      List<HashSet<FLNode>> newFL = new List<HashSet<FLNode>>();
      for (int i = 0; i <= Top.PolytopDim; i++) {
        newFL.Add(new HashSet<FLNode>());
      }

      Dictionary<FLNode, FLNode> oldToNew = new Dictionary<FLNode, FLNode>();

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

#region Hash-Eq
    // !!! При "наивном" Equals у FLNode, "потеря" одного элемента из Sub, если при этом множество вершин граней не уменьшилось
    // НЕ ВЕДЁТ к тому, что объекты считаются разными !!!
    public override bool Equals(object? obj) {
      if (obj == null || GetType() != obj.GetType()) {
        return false;
      }

      FaceLattice other = (FaceLattice)obj;

      if (this.Lattice.Count != other.Lattice.Count) {
        return false;
      }

      bool isEqual = true;
      for (int i = this.Top.PolytopDim; i > -1; i--) {
        var otherDict = new Dictionary<int, FLNode>();
        foreach (var otherNode in other.Lattice[i]) {
          otherDict.Add(otherNode.GetHashCode(), otherNode);
        }
        var thisDict = new Dictionary<int, FLNode>();
        foreach (var thisNode in this.Lattice[i]) {
          thisDict.Add(thisNode.GetHashCode(), thisNode);
        }

        if (thisDict.Count != otherDict.Count) {
          isEqual = false;
          Console.WriteLine($"Lattice are not equal: level i = {i}.");

          break;
        }


        foreach (var thisNode in this.Lattice[i]) {
          otherDict.TryGetValue(thisNode.GetHashCode(), out FLNode? otherNode);
          if (otherNode is null) {
            isEqual = false;
          }
          isEqual = isEqual && thisNode.Equals(otherNode);
        }
        if (!isEqual) {
          Console.WriteLine($"Lattice are not equal: level i = {i}.");

          break;
        }
        foreach (var otherNode in other.Lattice[i]) {
          thisDict.TryGetValue(otherNode.GetHashCode(), out FLNode? thisNode);
          if (thisNode is null) {
            isEqual = false;
          }
          isEqual = isEqual && otherNode.Equals(thisNode);
        }
        if (!isEqual) {
          Console.WriteLine($"Lattice are not equal: level i = {i}.");

          break;
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
  public class FLNode {

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
    public HashSet<FLNode> Super { get; } = new HashSet<FLNode>();

    /// <summary>
    /// The list of the subnodes, which Dim = this.Dim - 1.
    /// </summary>
    public HashSet<FLNode> Sub { get; } = new HashSet<FLNode>();

    /// <summary>
    /// Reference to the associated polytop to this node.
    /// </summary>
    private VectorHashSet Polytop { get; set; }

    /// <summary>
    /// Maps the dimension to the set of nodes within this dimension.
    /// </summary>
    private readonly Dictionary<int, HashSet<FLNode>> _levelNodes = new Dictionary<int, HashSet<FLNode>>();

    private Dictionary<int, HashSet<FLNode>> LevelNodes {
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
    public VectorHashSet Vertices => Polytop;

    /// <summary>
    /// The dimension of the associated polytop.
    /// </summary>
    public int PolytopDim => AffBasis.SpaceDim;
#endregion

#region Constructors
    /// <summary>
    /// Constructs an instance of FLNode as a vertex.
    /// </summary>
    /// <param name="vertex">Vertex on which this instance will be created.</param>
    public FLNode(Vector vertex) {
      Polytop    = new VectorHashSet(new VectorHashSet() { vertex });
      InnerPoint = vertex;
      AffBasis   = new AffineBasis(vertex);
    }

    /// <summary>
    /// Constructs a node based on its sub-nodes.
    /// </summary>
    /// <param name="sub">The set of sub-nodes which is the set of sub-nodes of the node to be created.</param>
    public FLNode(List<FLNode> sub) {
      Polytop = new VectorHashSet(sub.SelectMany(s => s.Vertices).ToHashSet());
      Sub     = new HashSet<FLNode>(sub);

      foreach (FLNode subNode in sub) {
        subNode.AddSuper(this);
      }

      InnerPoint = new Vector((new Vector(Sub.First().InnerPoint) + new Vector(Sub.Last().InnerPoint)) / Tools.Two);

      FLNode      subF   = Sub.First();
      AffineBasis affine = new AffineBasis(subF.AffBasis);
      affine.AddVectorToBasis(InnerPoint - subF.InnerPoint);
      AffBasis = affine;
    }

    /// <summary>
    /// Constructs an instance of FLNode as an face. (Aux for MinkSumSDas).
    /// </summary>
    /// <param name="Vs">The vertices of the face.</param>
    /// <param name="innerPoint">Inner point of the face.</param>
    /// <param name="aBasis">The affine basis of the face.</param>
    internal FLNode(VectorHashSet Vs, Vector innerPoint, AffineBasis aBasis) {
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
    private HashSet<FLNode> GetLevel(int dim)
      => LevelNodes.TryGetValue(dim, out HashSet<FLNode>? value) ? value : new HashSet<FLNode>();

    /// <summary>
    /// Gets the requested level in the node structure, that lies below this node.
    /// Otherwise returns empty set.
    /// </summary>
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried. If it lies above this node, it returns the empty set.</returns>
    internal IEnumerable<FLNode> GetLevelBelowNonStrict(int dim) => dim > PolytopDim ? new HashSet<FLNode>() : GetLevel(dim);

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
      _levelNodes.Add(PolytopDim, new HashSet<FLNode>() { this });

      // собираем верх
      HashSet<FLNode> superNodes = Super;
      int             d          = PolytopDim;
      while (superNodes.Count != 0) {
        d++;
        _levelNodes.Add(d, superNodes);
        superNodes = superNodes.SelectMany(node => node.Super).ToHashSet();
      }

      // собираем низ
      HashSet<FLNode> prevNodes = Sub;
      d = PolytopDim;
      while (prevNodes.Count != 0) {
        d--;
        _levelNodes.Add(d, prevNodes);
        prevNodes = prevNodes.SelectMany(node => node.Sub).ToHashSet();
      }
    }
#endregion

#region Functions
    /// <summary>
    /// Gets the entire levelNodes structure.
    /// </summary>
    /// <returns>Returns the entire levelNodes structure.</returns>
    public List<HashSet<FLNode>> GetAllLevels() {
      List<HashSet<FLNode>> allLevels = new List<HashSet<FLNode>>();
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
        Polytop = new VectorHashSet(Sub.SelectMany(s => s.Vertices));
      }
      _hash = null;
    }

#region Hash-Eq
    private int? _hash = null;

    /// <summary>
    /// The hash of the node is hash of the associated polytop.
    /// </summary>
    /// <returns>The hash of the polytop.</returns>
    public override int GetHashCode() {
      if (_hash is null) {
        IOrderedEnumerable<Vector> sortedVs = Vertices.Order();

        int hash = 0;
        foreach (Vector v in sortedVs) {
          hash = HashCode.Combine(hash, v.GetHashCode());
        }
        _hash = hash;
      }

      return _hash.Value;
    }

    /// <summary>
    /// This is the equality function for FLNode.
    /// </summary>
    /// <param name="obj">Compare this FLNode with another object.</param>
    /// <returns>Two FLNodes are considered equal if obj is an FLNode and:
    /// 1) polytopes corresponding to the nodes are equal.
    /// 2) All polytopes belonging to super nodes are equal.
    /// 3) All polytopes belonging to sub nodes are equal.
    /// </returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      FLNode other = (FLNode)obj;
      // 1) this == other
      bool isEqual = this.Polytop.SetEquals(other.Polytop);
      // вообще говоря, это не делает 2 узла равными:
      // Квадрат. У одного нет одного ребра-узла, у другого.
      // Множество вершин одинаковое, а узлы различные.

      // 2) Немного наивно, но хоть что-то
      isEqual &= this.Sub.Count == other.Sub.Count;

      isEqual &= this.Super.Count == other.Super.Count;

      // Полное сравнение решёток -- ОЧЕНЬ сложная (с вычислительной точки зрения) задача!

      return isEqual;
    }
#endregion

  }

}
