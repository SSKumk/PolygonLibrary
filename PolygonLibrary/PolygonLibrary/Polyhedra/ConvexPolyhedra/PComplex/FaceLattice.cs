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

    // public HashSet<Point> Points { get; init; }

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

    internal FaceLattice(FLNode Maximum, List<HashSet<FLNode>> lattice) {
      Top = Maximum;
      Lattice = lattice;
    }

    // public FaceLattice ProjectTo(AffineBasis aBasis) {



    // }

    public FaceLattice TranslateToOriginal(AffineBasis aBasis) {
      return new FaceLattice(Top.TranslateToOriginal(aBasis));
    }


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
        if (isEqual is false) {
          break;
        }
        isEqual = isEqual && this.Lattice[i].SetEquals(other.Lattice[i]);
        // ? Возможно надо что-то ещё проверять чтобы хорошо их сравнивать
        // ? Sub и Super надо проверять! (Но как?)
        System.Console.WriteLine($"Level i = {i}. Lattice are not equal:");
        System.Console.WriteLine($"this: {string.Join(' ', this.Lattice[i])}");
        System.Console.WriteLine($"other: {string.Join(' ', other.Lattice[i])}");
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
    public int Dim { get; }

    /// <summary>
    /// Reference to associated polytop to this node.
    /// </summary>
    public VPolytop Polytop { get; protected set; }

    public void ResetPolytop() { // todo Это противоречит нашей идеи о неизменности объектов! И нужна как вспомогательная вещь!
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
    // private Point? _innerPoint = null;

    // point(x) = 1/2*(point(y) + point(y')), where y,y' \in Faces, y != y'.
    public Point InnerPoint { get; }

    /// <summary>
    /// Gets the list of d-dimensional points which forms the affine space (not a Affine basis) corresponding to the this polytop.
    /// </summary>
    public AffineBasis Affine { get; }

    /// <summary>
    /// The list of the super-nodes, which Dim = this.Dim + 1.
    /// </summary>
    public HashSet<FLNode>? Super { get; protected set; }

    private HashSet<FLNode>? _allSuper = null;

    private HashSet<FLNode> AllNonStrictSuper {
      get
      {
        if (Super is null) {
          return new HashSet<FLNode>() { this };
        }

        return new HashSet<FLNode>(Super.SelectMany(sup => sup.AllNonStrictSuper)) { this };
      }
    }
    //! Надо как-то по-разному назвать Sub и Super, а то я путаюсь!
    public HashSet<FLNode> GetAllNonStrictSuper() => AllNonStrictSuper;

    public HashSet<FLNode> GetAllSuper() {
      HashSet<FLNode> allSuper = new HashSet<FLNode>(AllNonStrictSuper);
      allSuper.Remove(this);
      return allSuper;
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
      HashSet<FLNode> res = bottom.GetAllNonStrictSuper();
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

    public void AddSuper(FLNode node) {
      Super ??= new HashSet<FLNode>();
      Super.Add(node);
    }

    public FLNode(Point vertex) {
      Dim = 0;
      Polytop = new VPolytop(new List<Point>() { vertex });
      InnerPoint = vertex;
      Affine = new AffineBasis(vertex);
    }

    public FLNode(int dim, IEnumerable<Point> Vs, Point innerPoint, AffineBasis aBasis) {
      Dim = dim;
      Polytop = new VPolytop(Vs);
      InnerPoint = innerPoint;
      Affine = aBasis;
    }

    public FLNode(int dim, IEnumerable<Point> Vs, IEnumerable<FLNode> sub) {
      Dim = dim;
      Polytop = new VPolytop(Vs);
      Sub = new HashSet<FLNode>(sub);

      foreach (FLNode subNode in sub) {
        subNode.AddSuper(this);
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
      return new FLNode(Dim, sub.SelectMany(n => n.Vertices), sub);
    }

    public override int GetHashCode() => Polytop.GetHashCode();


    //todo Какой GetHashCode и Equals выбрать для FaceLatticeNode?
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      FLNode other = (FLNode)obj;


      // if (!this.Polytop.Equals(other.Polytop)) {
      //   System.Console.WriteLine($"this = {string.Join('\n', this.Polytop.Vertices)}");
      //   System.Console.WriteLine();
      //   System.Console.WriteLine($"other = {string.Join('\n', other.Polytop.Vertices)}");
      //   System.Console.WriteLine("NOT EQUAL!");

      // }

      return this.Polytop.Equals(other.Polytop);
    }


  }

}
