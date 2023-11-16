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

    private List<HashSet<FLNode>> ConstructLattice() {
      List<HashSet<FLNode>> FL = new List<HashSet<FLNode>>();
      for (int i = 0; i < Top.Dim + 1; i++) {
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
      , new HyperPlane(new AffineBasis(n.Affine), (Top.InnerPoint, false)).Normal));
      var Es = Lattice[^3].Select(n => new Edge(n.Vertices));
      return new ConvexPolytop(Top.Vertices, Top.Affine.SpaceDim, Fs, Es);
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
        if (isEqual == false) {
          break;
        }


        // isEqual = isEqual && this.Lattice[i].SetEquals(other.Lattice[i]);
        System.Console.WriteLine($"Lattice are not equal: level i = {i}.");
      }

      return isEqual;
    }
  }

  /// <summary>
  /// The node of the face lattice.
  /// </summary>
  public class FLNode {

    /// <summary>
    /// The dimensional of the associated polytop.
    /// </summary>
    public int Dim => Affine.SpaceDim;

    /// <summary>
    /// Reference to associated polytop to this node.
    /// </summary>
    public VPolytop Polytop { get; protected set; }

    // ? Это противоречит нашей идеи о неизменности объектов! И нужна как вспомогательная вещь!
    // ? Но с другой стороны она не вредит, так как получившийся рой точек является именно тем, чем должен
    public void ResetPolytop() {
      Polytop = new VPolytop(GetAllNonStrictSub().Where(n => n.Dim == 0)
                                          .Select(n => n.InnerPoint));
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
    public AffineBasis Affine { get; }

    /// <summary>
    /// The list of the super-nodes, which Dim = this.Dim + 1.
    /// </summary>
    public HashSet<FLNode>? Above { get; protected set; }

    private HashSet<FLNode>? _allAbove = null;

    private HashSet<FLNode> AllNonStrictAbove {
      get
      {
        if (Above is null) {
          return new HashSet<FLNode>() { this };
        }

        return new HashSet<FLNode>(Above.SelectMany(sup => sup.AllNonStrictAbove)) { this };
      }
    }
    public HashSet<FLNode> GetAllNonStrictAbove() => AllNonStrictAbove;

    public HashSet<FLNode> GetAllAbove() {
      HashSet<FLNode> allAbove = new HashSet<FLNode>(AllNonStrictAbove);
      allAbove.Remove(this);
      return allAbove;
    }



    /// <summary>
    /// The list of the sub-nodes, which Dim = this.Dim - 1.
    /// </summary>
    public HashSet<FLNode>? Sub { get; protected set; }

    /// <summary>
    /// Get the non strict sub facets of the current node.
    /// </summary>
    /// <returns>The set of subs with current one.</returns>
    public HashSet<FLNode> GetImmediateNonStrictSub() { //? Она точно нужна ?
      if (Sub is null) {
        return new HashSet<FLNode>() { this };
      } else {
        return new HashSet<FLNode>(Sub) { this };
      }
    }



    private HashSet<FLNode>? _allNonStrictSub = null;

    private HashSet<FLNode> AllNonStrictSub {
      get
      {
        if (_allNonStrictSub is null) {
          if (Dim == 0) {
            _allNonStrictSub = new HashSet<FLNode>() { this };
          } else {
            _allNonStrictSub = new HashSet<FLNode>(Sub!.SelectMany(sub => sub.AllNonStrictSub!)) { this };
          }
        }
        return _allNonStrictSub;
      }
    }

    public HashSet<FLNode> GetAllNonStrictSub() => AllNonStrictSub;

    public HashSet<FLNode> GetAllSub() {
      HashSet<FLNode> allSub = new HashSet<FLNode>(AllNonStrictSub);
      allSub.Remove(this);
      return allSub;
    }

    public static HashSet<FLNode> GetFromBottomToTop(FLNode bottom, FLNode top, bool excludeBottom = false, bool excludeTop = false) {
      HashSet<FLNode> res = bottom.GetAllNonStrictAbove();
      res.IntersectWith(top.GetAllNonStrictSub());

      if (excludeBottom) {
        res.Remove(bottom);
      }

      if (excludeTop) {
        res.Remove(top);
      }

      return res;

    }

    public void AddSub(FLNode node) {
      Sub ??= new HashSet<FLNode>();
      Sub.Add(node);
    }

    public void AddAbove(FLNode node) {
      Above ??= new HashSet<FLNode>();
      Above.Add(node);
    }

    public FLNode(Point vertex) {
      Polytop = new VPolytop(new List<Point>() { vertex });
      InnerPoint = vertex;
      Affine = new AffineBasis(vertex);
    }

    public FLNode(IEnumerable<Point> Vs, Point innerPoint, AffineBasis aBasis) {
      Polytop = new VPolytop(Vs);
      InnerPoint = innerPoint;
      Affine = aBasis;
    }

    public FLNode(IEnumerable<FLNode> sub) {
      Polytop = new VPolytop(sub.SelectMany(s => s.Vertices));
      Sub = new HashSet<FLNode>(sub);

      foreach (FLNode subNode in sub) {
        subNode.AddAbove(this);
      }

      InnerPoint = new Point
          (
           (new Vector(Sub!.First().InnerPoint) + new Vector(Sub!.Last().InnerPoint)) / Tools.Two
          );

      FLNode subF = Sub!.First();
      AffineBasis affine = new AffineBasis(subF.Affine);
      affine.AddVectorToBasis(InnerPoint - subF.InnerPoint);
      Affine = affine;
    }

    //! Эта хрень НЕверна!
    // public FLNode ProjectTo(AffineBasis aBasis) {
    //   if (Dim == 0) {
    //     Point v_proj = aBasis.ProjectPoint(Vertices.First());
    //     return new FLNode(v_proj);
    //   }
    //   List<FLNode> sub = new List<FLNode>();
    //   foreach (FLNode subNode in Sub!) {
    //     sub.Add(subNode.ProjectTo(aBasis));
    //   }
    //   return new FLNode(Dim, sub.SelectMany(n => n.Vertices), sub);
    // }

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

    public bool PolytopEq(FLNode other) => this.Polytop.Equals(other.Polytop);

    public override int GetHashCode() => Polytop.GetHashCode();


    //todo Какой GetHashCode и Equals выбрать для FaceLatticeNode?
    // ! Верно ли, что Polytop.GetHashCode() == Polytop.Equals()?
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      FLNode other = (FLNode)obj;

      // 1) this.Above == other.Above
      if (this.Above is null && other.Above is not null) { return false; }
      if (this.Above is not null && other.Above is null) { return false; }

      bool isEqual = true;
      var thisAbovePHash = this.Above?.Select(a => a.Polytop.GetHashCode()).ToHashSet();
      var otherAbovePHash = other.Above?.Select(a => a.Polytop.GetHashCode()).ToHashSet();


      if (thisAbovePHash is not null && otherAbovePHash is not null) {
        isEqual = isEqual && thisAbovePHash.SetEquals(otherAbovePHash);
      }
      if (!isEqual) {
        Console.WriteLine("Above!");
        Console.WriteLine($"This:  {string.Join('\n', this.Above!.Select(n => n.Polytop))}");
        Console.WriteLine($"Other: {string.Join('\n', other.Above!.Select(n => n.Polytop))}");
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

      // if (!this.Polytop.Equals(other.Polytop)) {
      //   System.Console.WriteLine($"this = {string.Join('\n', this.Polytop.Vertices)}");
      //   System.Console.WriteLine();
      //   System.Console.WriteLine($"other = {string.Join('\n', other.Polytop.Vertices)}");
      //   System.Console.WriteLine("NOT EQUAL!");

      // }

      // 3) this == other
      return this.Polytop.Equals(other.Polytop);
    }


  }

}
