using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;


namespace CGLibrary;

// todo Разобраться с Equals(), GetHashCode() и IEquatable<T> ! повсюду !

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class FaceLattice {

    public HashSet<Point> Points => Top.Vertices;

    public FLNode Top { get; init; }

    public List<HashSet<FLNode>> Lattice { get; init; }

    // public HashSet<FLNode> Взять i-ю размерность

    private List<HashSet<FLNode>> ConstructLattice() {
      List<HashSet<FLNode>> FL = new List<HashSet<FLNode>>();
      for (int i = 0; i <= Top.Dim; i++) {
        FL.Add(new HashSet<FLNode>());
      }
      FL[Top.Dim].Add(Top);

      HashSet<FLNode> prevNodes = new HashSet<FLNode>() { Top };
      for (int i = Top.Dim - 1; i > -1; i--) {
        foreach (FLNode node in prevNodes) {
          FL[i].UnionWith(node.Sub!);
        }
        prevNodes = FL[i];
      }
      return FL;
    }

    public FaceLattice(Point point) {
      Top = new FLNode(point);
      Lattice = new List<HashSet<FLNode>>() { new HashSet<FLNode>() { Top } };
    }

    public FaceLattice(FLNode Maximum) {
      Top = Maximum;
      Lattice = ConstructLattice();
    }

    public FaceLattice(List<HashSet<FLNode>> lattice) {
      Top = lattice[^1].First();
      Lattice = lattice;
    }

    public FaceLattice ProjectTo(AffineBasis aBasis) {
      List<HashSet<FLNode>> newFL = new List<HashSet<FLNode>>();
      for (int i = 0; i < Top.Dim + 1; i++) {
        newFL.Add(new HashSet<FLNode>());
      }

      Dictionary<FLNode, FLNode> oldToNew = new Dictionary<FLNode, FLNode>();

      //Отдельно обрабатываем случай d == 0
      foreach (FLNode vertex in Lattice[0]) {
        FLNode newVertex = new FLNode(aBasis.ProjectPoint(vertex.Vertices.First()));
        oldToNew.Add(vertex, newVertex);
        newFL[0].Add(newVertex);
      }

      for (int i = 1; i < newFL.Count; i++) {
        foreach (FLNode node in Lattice[i]) {
          List<FLNode> newSub = node.Sub!.Select(n => oldToNew[n]).ToList();

          FLNode newNode = new FLNode(newSub);
          oldToNew.Add(node, newNode);
          newFL[i].Add(newNode);
        }
      }
      return new FaceLattice(newFL);
    }

    public FaceLattice TranslateToOriginal(AffineBasis aBasis) {
      return new FaceLattice(Top.TranslateToOriginal(aBasis));
    }

    public ConvexPolytop ToConvexPolytop() {
      var Fs = Lattice[^2].Select(n => new Face(n.Vertices
      , new HyperPlane(new AffineBasis(n.AffBasis), (Top.InnerPoint, false)).Normal));
      var Es = Lattice[^3].Select(n => new Edge(n.Vertices));
      return new ConvexPolytop(Top.Vertices, Top.AffBasis.SpaceDim, Fs, Es);
    }

    public void WriteTXT(string filePath) => this.ToConvexPolytop().WriteTXT(filePath);

    public override bool Equals(object? obj) {
      if (obj == null || GetType() != obj.GetType()) {
        return false;
      }

      FaceLattice other = (FaceLattice)obj;

      if (this.Lattice.Count != other.Lattice.Count) {
        return false;
      }

      bool isEqual = true;
      for (int i = this.Top.Dim; i > -1; i--) {
        var otherDict = new Dictionary<int, FLNode>();
        foreach (var otherNode in other.Lattice[i]) {
          otherDict.Add(otherNode.GetHashCode(), otherNode);
        }

        foreach (var thisNode in this.Lattice[i]) {
          otherDict.TryGetValue(thisNode.GetHashCode(), out FLNode? otherNode);
          if (otherNode is null) {
            isEqual = false;
          }
          isEqual = isEqual && (thisNode.Equals(otherNode));
        }
        if (!isEqual) {
          break;
        }


        // isEqual = isEqual && this.Lattice[i].SetEquals(other.Lattice[i]);
        System.Console.WriteLine($"Lattice are not equal: level i = {i}.");
      }

      return isEqual;
    }
  }

  /// <summary>
  /// The node of the face lattice. It stores references to the super-nodes and sub-nodes. 
  /// In addition it maintains a set of vertices representing the face.
  /// </summary>
  public class FLNode {

    /// <summary>
    /// The dimensional of the associated polytop.
    /// </summary>
    public int Dim => AffBasis.SpaceDim;

    /// <summary>
    /// Reference to the associated polytop to this node.
    /// </summary>
    public VPolytop Polytop { get; protected set; }

    /// <summary>
    /// In assumption that all nodes of a lower dimension are correct, it cerates a new Polytop based on vertices of the subs.
    /// </summary>
    public void ReconstructPolytop() {
      if (Dim != 0) {
        Polytop = new VPolytop(Sub!.SelectMany(s => s.Vertices));
      }
    }

    /// <summary>
    /// The vertices of the polytop.
    /// </summary>
    public HashSet<Point> Vertices => Polytop.Vertices;

    /// <summary>
    /// Gets the d-dimensional point 'p' which lies within P and does not lie on any faces of P.
    /// </summary>
    public Point InnerPoint { get; }

    /// <summary>
    /// Gets the list of d-dimensional points which forms the affine space (not a Affine basis) corresponding to the this polytop.
    /// </summary>
    public AffineBasis AffBasis { get; }

    /// <summary>
    /// The list of the super-nodes, whose Dim = this.Dim + 1.
    /// </summary>
    public HashSet<FLNode> Super { get; protected set; } = new HashSet<FLNode>();

    /// <summary>
    /// The list of the sub-nodes, which Dim = this.Dim - 1.
    /// </summary>
    public HashSet<FLNode> Sub { get; protected set; } = new HashSet<FLNode>();


    /// <summary>
    /// Maps the dimension to the set of nodes within this dimension.
    /// </summary>
    private Dictionary<int, HashSet<FLNode>> _levelNodes = new Dictionary<int, HashSet<FLNode>>();

    public Dictionary<int, HashSet<FLNode>> LevelNodes {
      get
      {
        if (_levelNodes.Count == 0) {
          ConstructLevelNodes();
        }
        return _levelNodes;
      }
    }

    /// <summary>
    /// Gets the requested level in the node structure. If there is no key = dim, then an empty set is produced.
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried.</returns>
    internal HashSet<FLNode> GetLevel(int dim) {
      if (LevelNodes.TryGetValue(dim, out HashSet<FLNode>? level)) {
        return LevelNodes[dim];
      }
      return new HashSet<FLNode>();
    }

    /// <summary>
    /// Gets the requested level in the node structure, that lies below this node.
    /// Otherwise returns empty set.
    /// </summary>
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried. If it lies above this node, it returns empty set.</returns>
    internal HashSet<FLNode> GetLevelBelowNonStrict(int dim) {
      if (dim > Dim) {
        return new HashSet<FLNode>();
      }
      return GetLevel(dim);
    }

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
    /// Construct the mapping that takes the dimension and maps it onto the set of nodes in that dimension,
    /// which are either sub-nodes or super-nodes of it.
    /// </summary>
    private void ConstructLevelNodes() {
      // добавили себя
      _levelNodes.Add(Dim, new HashSet<FLNode>() { this });

      // собираем верх
      HashSet<FLNode> superNodes = Super;
      int d = Dim;
      while (superNodes.Count != 0) {
        d++;
        _levelNodes.Add(d, superNodes);
        superNodes = superNodes.SelectMany(node => node.Super).ToHashSet();
      }

      // собираем низ
      HashSet<FLNode> prevNodes = Sub;
      d = Dim;
      while (prevNodes.Count != 0) {
        d--;
        _levelNodes.Add(d, prevNodes);
        prevNodes = prevNodes.SelectMany(node => node.Sub).ToHashSet();
      }
    }

    /// <summary>
    /// Retrieves all sub-faces of the node, including the node itself.
    /// </summary>
    /// <returns>The collection that contains all non strict sub-faces of the node.</returns>
    public IEnumerable<FLNode> AllNonStrictSub => LevelNodes
      .Where(ln => ln.Key <= Dim)
      .SelectMany(ln => ln.Value);

    // private HashSet<FLNode>? _allSuper = null;

    // // ! Не сваливать всё в кучу, а в Dictionary<dim, HashSet<FLNode>, тогда можно будет проще пересекать части решёток
    // // Пока не понимаю как это сделать ...
    // private HashSet<FLNode> AllNonStrictSuper {
    //   get
    //   {
    //     if (Super is null) {
    //       return new HashSet<FLNode>() { this };
    //     }

    //     return new HashSet<FLNode>(Super.SelectMany(sup => sup.AllNonStrictSuper)) { this };
    //   }
    // }
    // public HashSet<FLNode> GetAllNonStrictSuper() => AllNonStrictSuper;

    // public HashSet<FLNode> GetAllSuper() {
    //   HashSet<FLNode> allAbove = new HashSet<FLNode>(AllNonStrictSuper);
    //   allAbove.Remove(this);
    //   return allAbove;
    // }



    // /// <summary>
    // /// Get the non strict super facets of the current node.
    // /// </summary>
    // /// <returns>The set of supers with current one.</returns>
    // public HashSet<FLNode> GetNonStrictSuper() => new HashSet<FLNode>(GetSuper()) { this };

    // /// <summary>
    // /// Get the strict super facets of the current node.
    // /// </summary>
    // /// <returns>The set of supers with current one.</returns>
    // public HashSet<FLNode> GetSuper() {
    //   if (Super is null) {
    //     return new HashSet<FLNode>();
    //   } else {
    //     return new HashSet<FLNode>(Super);
    //   }
    // }

    // private HashSet<FLNode>? _allNonStrictSub = null;

    // public HashSet<FLNode> AllNonStrictSub {
    //   get
    //   {
    //     if (_allNonStrictSub is null) {
    //       if (Dim == 0) {
    //         _allNonStrictSub = new HashSet<FLNode>() { this };
    //       } else {
    //         _allNonStrictSub = new HashSet<FLNode>(Sub!.SelectMany(sub => sub.AllNonStrictSub!)) { this };
    //       }
    //     }
    //     return _allNonStrictSub;
    //   }
    // }

    // public HashSet<FLNode> 

    // // ToDo: Избавиться от функций Get...Sub/Sup и переделать на свойства
    // public HashSet<FLNode> GetAllNonStrictSub() => AllNonStrictSub;

    // public HashSet<FLNode> GetAllSub() {
    //   HashSet<FLNode> allSub = GetAllNonStrictSub();
    //   allSub.Remove(this);
    //   return allSub;
    // }

    // /// <summary>
    // /// "Ромбик"
    // /// </summary>
    // /// <param name="bottom"></param>
    // /// <param name="top"></param>
    // /// <param name="excludeBottom"></param>
    // /// <param name="excludeTop"></param>
    // /// <returns></returns>
    // public static HashSet<FLNode> GetFromBottomToTop(FLNode bottom, FLNode top, bool excludeBottom = false, bool excludeTop = false) {
    //   HashSet<FLNode> res = bottom.GetAllNonStrictSuper();
    //   res.IntersectWith(top.GetAllNonStrictSub());

    //   if (excludeBottom) {
    //     res.Remove(bottom);
    //   }

    //   if (excludeTop) {
    //     res.Remove(top);
    //   }

    //   return res;

    // }

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
    /// Constructs an instance of FLNode as a vertex.
    /// </summary>
    /// <param name="vertex">Vertex on which this instance will be created.</param>
    public FLNode(Point vertex) {
      Polytop = new VPolytop(new List<Point>() { vertex });
      InnerPoint = vertex;
      AffBasis = new AffineBasis(vertex);
    }

    /// <summary>
    /// Constructs an instance of FLNode as an face. (Aux for MinkSumSDas).
    /// </summary>
    /// <param name="Vs">The vertices of the face.</param>
    /// <param name="innerPoint">Inner point of the face.</param>
    /// <param name="aBasis">The affine basis of the face.</param>
    internal FLNode(IEnumerable<Point> Vs, Point innerPoint, AffineBasis aBasis) {
      Polytop = new VPolytop(Vs);
      InnerPoint = innerPoint;
      this.AffBasis = aBasis;
    }

    /// <summary>
    /// Constructs a node based on its sub-nodes.
    /// </summary>
    /// <param name="sub">The set of sub-nodes which is the set of sub-nodes of the node to be created.</param>
    public FLNode(IEnumerable<FLNode> sub) {
      Polytop = new VPolytop(sub.SelectMany(s => s.Vertices));
      Sub = new HashSet<FLNode>(sub);

      foreach (FLNode subNode in sub) {
        subNode.AddSuper(this);
      }

      InnerPoint = new Point
          (
           (new Vector(Sub!.First().InnerPoint) + new Vector(Sub!.Last().InnerPoint)) / Tools.Two
          );

      FLNode subF = Sub!.First();
      AffineBasis affine = new AffineBasis(subF.AffBasis);
      affine.AddVectorToBasis(InnerPoint - subF.InnerPoint);
      AffBasis = affine;
    }

    /// <summary>
    /// Creates the node with saving all its structure in terms of the space from which it was projected to the current space.
    /// </summary>
    /// <param name="aBasis">The affine basis from which it was projected.</param>
    /// <returns>The node in original space.</returns>
    public FLNode TranslateToOriginal(AffineBasis aBasis) {
      if (Dim == 0) {
        Point v_trans = aBasis.TranslateToOriginal(Vertices.First());
        return new FLNode(v_trans);
      }
      List<FLNode> sub = new List<FLNode>();
      foreach (FLNode subNode in Sub!) {
        sub.Add(subNode.TranslateToOriginal(aBasis));
      }
      return new FLNode(sub);
    }

    /// <summary>
    /// Checks if the polytop of this node is equal to the polytop of other node.
    /// </summary>
    /// <param name="other">Node to compare with.</param>
    /// <returns><c>True</c> if they are equal, else <c>False</c>.</returns>
    public bool PolytopEq(FLNode other) => this.Polytop.Equals(other.Polytop);

    /// <summary>
    /// The hash of the node is hash of the associated polytop.
    /// </summary>
    /// <returns>The hash of the polytop.</returns>
    public override int GetHashCode() => Polytop.GetHashCode();

    /// <summary>
    /// ! НАДО ДОБАВИТЬ СРАВНЕНИЕ НА EQUALS, а не только по хешам.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      FLNode other = (FLNode)obj;

      // 1) this.Above == other.Above
      if (this.Super is null && other.Super is not null) { return false; }
      if (this.Super is not null && other.Super is null) { return false; }

      bool isEqual = true;
      var thisAbovePHash = this.Super?.Select(a => a.Polytop.GetHashCode()).ToHashSet();
      var otherAbovePHash = other.Super?.Select(a => a.Polytop.GetHashCode()).ToHashSet();

      // ? Подумать о том, чтобы ещё сравнивать на Equals
      if (thisAbovePHash is not null && otherAbovePHash is not null) {
        isEqual = isEqual && thisAbovePHash.SetEquals(otherAbovePHash);
      }
      if (!isEqual) {
        Console.WriteLine("Above!");
        Console.WriteLine($"This:  {string.Join('\n', this.Super!.Select(n => n.Polytop))}");
        Console.WriteLine($"Other: {string.Join('\n', other.Super!.Select(n => n.Polytop))}");
        return false;
      }

      // 2) this.Sub == other.Sub
      if (this.Sub is null && other.Sub is not null) { return false; }
      if (this.Sub is not null && other.Sub is null) { return false; }

      var thisSubPHash = this.Sub?.Select(s => s.Polytop.GetHashCode()).ToHashSet();
      var otherSubPHash = other.Sub?.Select(s => s.Polytop.GetHashCode()).ToHashSet();

      if (thisSubPHash is not null && otherSubPHash is not null) {
        isEqual = isEqual && thisSubPHash.SetEquals(otherSubPHash);
      }
      if (!isEqual) {
        Console.WriteLine(value: "Below!");
        Console.WriteLine($"This:  {string.Join('\n', this.Sub!.Select(n => n.Polytop))}");
        Console.WriteLine($"Other: {string.Join('\n', other.Sub!.Select(n => n.Polytop))}");
        return false;
      }

      // 3) this == other
      return this.Polytop.Equals(other.Polytop);
    }


  }

}
