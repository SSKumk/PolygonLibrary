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

    public FaceLatticeNode Maximum { get; init; }


    public FaceLattice(HashSet<Point> Ps, FaceLatticeNode Max) {
      Points = Ps;
      Maximum = Max;
    }


  }

  /// <summary>
  /// The node of the face lattice.
  /// </summary>
  public class FaceLatticeNode {

    /// <summary>
    /// The dimensional of the associated polytop.
    /// </summary>
    public int Dim { get; }

    /// <summary>
    /// Reference to associated polytop to this node.
    /// </summary>
    public VPolytop Polytop { get; init; }

    /// <summary>
    /// The vertices of the polytop.
    /// </summary>
    public HashSet<Point> Vertices => Polytop.Vertices;

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
    private List<Point>? _affine = null;


    /// <summary>
    /// Gets the list of d-dimensional points which forms the affine space (not a Affine basis) corresponding to the this polytop.
    /// </summary>
    public virtual List<Point> Affine {
      get
      {
        if (_affine is null) {
          FaceLatticeNode subF = Sub!.First();
          List<Point> affine = new List<Point>(subF.Affine) { new Point(subF.InnerPoint - InnerPoint) };
          _affine = affine;

          // Console.WriteLine("Affine!");
        }

        return _affine;
      }
    }

    /// <summary>
    /// The list of the super-nodes, which Dim = this.Dim + 1.
    /// </summary>
    public List<FaceLatticeNode>? Super { get; protected set; }

    /// <summary>
    /// The list of the sub-nodes, which Dim = this.Dim - 1.
    /// </summary>
    public List<FaceLatticeNode>? Sub { get; protected set; }

    public void AddSub(FaceLatticeNode node) {
      Sub ??= new List<FaceLatticeNode>();
      Sub.Add(node);
    }

    public void AddSuper(FaceLatticeNode node) {
      Super ??= new List<FaceLatticeNode>();
      Super.Add(node);
    }

    public FaceLatticeNode(int dim, VPolytop VP) {
      Dim = dim;
      Polytop = VP;
    }

    public FaceLatticeNode(int dim, HashSet<Point> Vs) {
      Dim = dim;
      Polytop = new VPolytop(Vs);
    }

    public FaceLatticeNode(int dim, HashSet<Point> Vs, Point innerPoint, List<Point> affine) : this(dim, Vs) {
      _innerPoint = innerPoint;
      _affine = affine;
    }

    public override int GetHashCode() {
      return Polytop.GetHashCode();
    }


    //todo Какой GetHashCode выбрать для FaceLatticeNode?

  }

}
