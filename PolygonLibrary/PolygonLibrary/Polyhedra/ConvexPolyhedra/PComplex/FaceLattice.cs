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

  public class FaceLattice {

    public HashSet<Point> Points { get; init; }

    // public List<FaceLatticeNode> Vs {get; init;}

    public FLNode Top { get; init; }

    public Dictionary<int, FLNode> Lattice { get; init; }

    public FaceLattice(HashSet<Point> Ps, FLNode Maximum) {
      Points = Ps;
      Top = Maximum;
    }

    // public FaceLattice(HashSet<Point> Ps, FLNode Maximum, Dictionary<int, FLNode> lattice) {
    //   Points = Ps;
    //   Top = Maximum;
    //   Lattice = lattice;
    // }


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
    public VPolytop? Polytop { get; protected set; }

    /// <summary>
    /// The vertices of the polytop.
    /// </summary>
    public HashSet<Point> Vertices => Polytop is null ? new HashSet<Point>() : Polytop.Vertices;

    /// <summary>
    /// Gets the d-dimensional point 'p' which lies within P and does not lie on any faces of P.
    /// </summary>
    private Point? _innerPoint = null;

    // point(x) = 1/2*(point(y) + point(y')), where y,y' \in Faces, y != y'.
    public Point InnerPoint => _innerPoint
     ??= new Point
         (
          (new Vector(Sub!.First().InnerPoint) + new Vector(Sub!.Last().InnerPoint)) / Tools.Two
         );


    /// <summary>
    /// The list represents the affine space of the polytop.
    /// </summary>
    private AffineBasis? _affine = null;


    /// <summary>
    /// Gets the list of d-dimensional points which forms the affine space (not a Affine basis) corresponding to the this polytop.
    /// </summary>
    public AffineBasis Affine {
      get
      {
        if (_affine is null) {
          FLNode subF = Sub!.First();
          AffineBasis affine = new AffineBasis(subF.Affine);
          affine.AddPointToBasis(new Point(subF.InnerPoint - InnerPoint));
          _affine = affine;

          // Console.WriteLine("Affine!");
        }

        return _affine;
      }
    }

    /// <summary>
    /// The list of the super-nodes, which Dim = this.Dim + 1.
    /// </summary>
    public HashSet<FLNode>? Super { get; protected set; }

    /// <summary>
    /// The list of the sub-nodes, which Dim = this.Dim - 1.
    /// </summary>
    public HashSet<FLNode>? Sub { get; protected set; }

    private HashSet<FLNode>? _allSub = null;

    public HashSet<FLNode>? AllSub {
      get
      {
        if (_allSub is null) {
          if (Dim == 0) {
            _allSub = new HashSet<FLNode>() { this };
          } else {
            _allSub = new HashSet<FLNode>(Sub!.SelectMany(sub => sub.AllSub!)) { this };
          }
        }
        return _allSub;
      }
    }


    public void AddSub(FLNode node) {
      Sub ??= new HashSet<FLNode>();
      Sub.Add(node);
    }

    public void AddSuper(FLNode node) {
      Super ??= new HashSet<FLNode>();
      Super.Add(node);
    }

    public FLNode(int dim) {
      Dim = dim;
    }

    public FLNode(int dim, Point innerPoint, AffineBasis aBasis) : this(dim) {
      _innerPoint = innerPoint;
      _affine = aBasis;
    }

    public FLNode(int dim, HashSet<Point> Vs) : this(dim) {
      Polytop = new VPolytop(Vs);
    }

    public FLNode(int dim, HashSet<Point> Vs, Point innerPoint, AffineBasis aBasis) : this(dim, innerPoint, aBasis) {
      Polytop = new VPolytop(Vs);
    }

    public override int GetHashCode() => InnerPoint.GetHashCode();

    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      FLNode other = (FLNode)obj;

      return this.InnerPoint == other.InnerPoint;
    }

    //todo Какой GetHashCode и Equals выбрать для FaceLatticeNode?

  }

}
