using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polygons.ConvexPolygons;

/// <summary>
/// Class of a support function defined as an array of pairs (normal;value)
/// ordered counterclockwise over the normals. It is supposed that
/// the first pair has the normal vector with the least polar angle among all normals 
/// </summary>
public class SupportFunction : List<GammaPair> {
  /// <summary>
  /// Constructor of the SupportFunction class, 
  /// which takes a list of pairs, filters non-zero vectors,
  /// normalizes vectors, sorts the pairs counterclockwise,
  /// and filters pairs with equal vectors
  /// </summary>
  /// <param name="gs">The list of pairs</param>
  /// <param name="ToSort">Flag showing whether the list should be sorted according the vector polar angle</param>
  public SupportFunction(IEnumerable<GammaPair> gs, bool ToSort = true) {
    List<GammaPair> gs1 = new List<GammaPair>();
    foreach (GammaPair pair in gs) {
      if (pair.Normal != Vector2D.Zero) {
        double l = pair.Normal.Length;
        gs1.Add(new GammaPair(pair.Normal / l, pair.Value / l));
      }
    }

    if (gs1.Count == 0) {
      throw new ArgumentException("Only pairs with zero normals in initialization of a support function");
    }

    if (ToSort) gs1.Sort();

    foreach (GammaPair pair in gs1) {
      if (Count == 0 || !this[^1].Normal.Equals(pair.Normal)) {
        Add(pair);
      }
    }
  }

  /// <summary>
  /// Constructor of the SupportFunction class on the basis of vertices 
  /// of the polygon ordered counterclockwise (or the collection of points,
  /// which convex hull is the polygon). If there are too few points
  /// in the collection (after convexification, if necessary) - 1 or 0,
  /// then an exception is thrown
  /// </summary>
  /// <param name="ps">The collection of points</param>
  /// <param name="ToConvexify">Flag showing whether the collection should be convexified in the beginning</param>
  public SupportFunction(List<Point2D> ps, bool ToConvexify = true) {
    List<Point2D> ps1 = ToConvexify ? Convexification.ArcHull2D(ps) : ps;

    switch (ps1.Count) {
      case > 2: {
        int i, j;
        for (i = 0, j = 1; i < ps1.Count; i++, j = (j + 1) % ps1.Count) {
          Vector2D v = (ps1[j] - ps1[i]).TurnCW().Normalize();
          this.Add(new GammaPair(v, v * (Vector2D)ps1[i]));
        }

        break;
      }

      case 2: {
        // Special generation of the support function of a segment
        Vector2D v = (ps1[1] - ps1[0]).Normalize();
        Vector2D v1 = v.TurnCW();
        this.Add(new GammaPair(v, v * (Vector2D)ps1[1]));
        this.Add(new GammaPair(-v, -v * (Vector2D)ps1[0]));
        this.Add(new GammaPair(v1, v1 * (Vector2D)ps1[0]));
        this.Add(new GammaPair(-v1, -v1 * (Vector2D)ps1[0]));
        break;
      }

      default: {
        Vector2D p = (Vector2D)ps1[0];
        this.Add(new GammaPair(Vector2D.E1, Vector2D.E1 * p));
        this.Add(new GammaPair(Vector2D.E2, Vector2D.E2 * p));
        this.Add(new GammaPair(-Vector2D.E1, -Vector2D.E1 * p));
        this.Add(new GammaPair(-Vector2D.E2, -Vector2D.E2 * p));
        break;
      }
    }

    this.Sort();
  }

  /// <summary>
  /// Procedure for finding cone that contains the given vector.
  /// If the vector coincides with a normal from the collection, 
  /// the cone will be returned, which has the vector as the counterclockwise boundary.
  /// The search is performed by dichotomy, therefore, it is of O(log n) complexity.
  /// If the collection is sub-definite (that is has one or zero pairs),
  /// an exception is thrown.
  /// </summary>
  /// <param name="v">The vector to be localized</param>
  /// <param name="i">Index of the clockwise boundary of the cone</param>
  /// <param name="j">Index of the counterclockwise boundary of the cone</param>
  public void FindCone(Vector2D v, out int i, out int j) {
#if DEBUG
    if (this.Count < 2) {
      throw new Exception("The function is defined by too few directions");
    }
#endif

    // If the vector belongs to the cone between the last and the first normals
    // or coincides with the last vector, then the cone is between the last and the first vectors
    if (Vector2D.AreCodirected(v, this[Count-1].Normal) || v.IsBetween(this[Count - 1].Normal, this[0].Normal)) {
      i = Count - 1;
      j = 0;
    }
    // If the vector coincides with the first vector, then the cone is between the first and second vectors
    else if (Vector2D.AreCodirected(v, this[0].Normal)) {
      i = 0;
      j = 1;
    }
    // Otherwise the vector is somewhere strictly between the first and last vectors, do binary search
    else {
      j = this.BinarySearchByPredicate(swElem => Tools.LT(v.PolarAngle, swElem.Normal.PolarAngle), 0, Count - 1);
      i = this.NormalizeIndex(j - 1);
    }
  }

  /// <summary>
  /// Computing coordinates of the given vector in the basis of two given normals
  /// from the collection. If the indices of the basis vectors are wrong (less than 0, 
  /// greater than Count-1, coincide), an exception is thrown
  /// </summary>
  /// <param name="v">The vector, whose coordinates should be computed</param>
  /// <param name="i">The index of the first basis vector</param>
  /// <param name="j">The index of the second basis vector</param>
  /// <param name="a">The first resultant coordinate</param>
  /// <param name="b">The second resultant coordinate</param>
  public void ConicCombination(Vector2D v, int i, int j, out double a, out double b) {
#if DEBUG
    if (i < 0 || i >= Count) {
      throw new IndexOutOfRangeException("ConicCombination: bad index i");
    }

    if (j < 0 || j >= Count) {
      throw new IndexOutOfRangeException("ConicCombination: bad index j");
    }

    if (i == j) {
      throw new ArgumentException("ConicCombination: indices i and j coincide");
    }
#endif

    // Computing coefficients of the conic combinations
    double
      // v = alpha * vi + beta * vj
      //  alpha * vi.x + beta * vj.x = v.x
      //  alpha * vi.y + beta * vj.y = v.y
      d = this[i].Normal.x * this[j].Normal.y - this[i].Normal.y * this[j].Normal.x
      , d1 = v.x * this[j].Normal.y - v.y * this[j].Normal.x
      , d2 = this[i].Normal.x * v.y - this[i].Normal.y * v.x;
    a = d1 / d;
    b = d2 / d;
  }

  /// <summary>
  /// Computation of the function value on a given vector
  /// </summary>
  /// <param name="v">The vector where the value should be computed</param>
  /// <param name="i">The supposed index of the clockwise boundary of the cone; 
  /// if -1, then the appropriate cone will be found</param>
  /// <param name="j">The supposed index of the counterclockwise boundary of the cone; 
  /// if -1, then the appropriate cone will be found</param>
  /// <returns>The support function</returns>
  public double FuncVal(Vector2D v, int i = -1, int j = -1) {
#if DEBUG
    if (this.Count <= 2) {
      throw new Exception("The function is defined by too few directions");
    }
#endif

    // Seeking the vector from the set, which is first equal or greater in the counterclockwise order
    if (i == -1 || j == -1) {
      FindCone(v, out i, out j);
    }

    // Computing coefficients of the conic combinations
    double alpha, beta;
    ConicCombination(v, i, j, out alpha, out beta);
    return alpha * this[i].Value + beta * this[j].Value;
  }

  /// <summary>
  /// Method that constructs linear combination of two support functions
  /// </summary>
  /// <param name="fa">The first function</param>
  /// <param name="fb">The second function</param>
  /// <param name="ca">The coefficient of the first function</param>
  /// <param name="cb">The coefficient of the second function</param>
  /// <param name="suspiciousIndices">Array of indices in the resultant collection, 
  /// where vectors from the second function occurs; 
  /// if null, then this information does not accumulated</param>
  /// <param name="suspiciousVectors">Array of vectors of the second function; 
  /// if null, then this information does not accumulated</param>
  /// <returns>The desired linear combination of the original functions</returns>
  public static SupportFunction CombineFunctions(SupportFunction fa, SupportFunction fb,
    double ca, double cb, List<int> suspiciousIndices = null, List<Vector2D> suspiciousVectors = null) {
    List<GammaPair> res = new List<GammaPair>();
    
    int
      ia = fa.Count - 1, ja = 0, ib = fb.Count - 1, jb = 0;
    double
      angleA = fa[0].Normal.PolarAngle, angleB = fb[0].Normal.PolarAngle, oldAngle;

    suspiciousIndices?.Clear();
    suspiciousVectors?.Clear();

    while (Tools.LE(angleA, Math.PI) || Tools.LE(angleB, Math.PI)) {
      if (Tools.LT(angleA, angleB)) {
        res.Add(new GammaPair(fa[ja].Normal, ca * fa[ja].Value + cb * fb.FuncVal(fa[ja].Normal, ib, jb)));

        ia = ja;
        ja = (ja + 1) % fa.Count;
        oldAngle = angleA;
        angleA = fa[ja].Normal.PolarAngle;
        if (Tools.LT(angleA, oldAngle)) angleA += Tools.PI2;
      } else if (Tools.GT(angleA, angleB)) {
        suspiciousIndices?.Add(res.Count);
        suspiciousVectors?.Add(fb[jb].Normal);

        res.Add(new GammaPair(fb[jb].Normal, ca * fa.FuncVal(fb[jb].Normal, ia, ja) + cb * fb[jb].Value));

        ib = jb;
        jb = (jb + 1) % fb.Count;
        oldAngle = angleB;
        angleB = fb[jb].Normal.PolarAngle;
        if (Tools.LT(angleB, oldAngle)) angleB += Tools.PI2;
      } else {
        suspiciousIndices?.Add(res.Count);
        suspiciousVectors?.Add(fb[jb].Normal);

        res.Add(new GammaPair(fa[ja].Normal, ca * fa[ja].Value + cb * fb[jb].Value));

        oldAngle = angleA;
        
        ia = ja;
        ja = (ja + 1) % fa.Count;
        angleA = fa[ja].Normal.PolarAngle;
        if (Tools.LT(angleA, oldAngle)) angleA += Tools.PI2;

        ib = jb;
        jb = (jb + 1) % fb.Count;
        angleB = fb[jb].Normal.PolarAngle;
        if (Tools.LT(angleB, oldAngle)) angleB += Tools.PI2;
      }
    }

    return new SupportFunction(res);    
  }

  /// <summary>
  /// Auxiliary type: element of double-linked list describing collection 
  /// of pairs passed the check procedure up to the current instant
  /// </summary>
  private struct PairInfo {
    /// <summary>
    /// Flag showing whether the element passed the check procedure
    /// </summary>
    public bool isValid;

    /// <summary>
    /// Index of the next valid element in the cyclic list 
    /// </summary>
    public int next;

    /// <summary>
    /// Index of the previous valid element in the cyclic list 
    /// </summary>
    public int prev;
  }

  /// <summary>
  /// Method supplement for the convexification procedure, which checks a triple of pairs
  /// for validity of the central element of the triple with respect to the extreme ones.
  /// It is assumed that normals of extreme elements of the triple are not parallel
  /// </summary>
  /// <param name="lm">Index of the clockwise neighbor</param>
  /// <param name="lc">Index of the pair to be checked</param>
  /// <param name="lp">Index of the counterclockwise neighbor</param>
  /// <returns>true, if the central pair is valid; false, otherwise</returns>
  protected bool CheckTriple(int lm, int lc, int lp) => SupportFunction.CheckTriple(this[lm], this[lc], this[lp]);

  /// <summary>
  /// Method supplement for the convexification procedure, which checks a triple of pairs
  /// for validity of the central element of the triple with respect to the extreme ones.
  /// </summary>
  /// <param name="pm">The clockwise neighbor</param>
  /// <param name="pc">The pair to be checked</param>
  /// <param name="pp">The counterclockwise neighbor</param>
  /// <returns>true, if the central pair is valid; false, otherwise</returns>
  /// <remarks>
  /// The algorithm is the following:
  ///  - if the normals of the extreme elements of the triple are parallel,
  ///    that is they cannot define any vertex, then the middle element of the triple
  ///    is valid if the extreme elements are not compatible;
  ///  - otherwise:
  ///  - find the point of intersection of lines defined by the first and second
  ///    elements of the triple (they are not parallel - by definition of the support function)
  ///  - to allow to the middle element to be valid, the point should strictly belong
  ///    to the halfplane defined by the third element of the triple
  ///</remarks>
  protected static bool CheckTriple(GammaPair pm, GammaPair pc, GammaPair pp) {
    if (Tools.EQ(pm.Normal.x, -pp.Normal.x) && Tools.EQ(pm.Normal.y, -pp.Normal.y)) {
      return Tools.GE(pm.Value, -pp.Value);
    }

    Point2D p = GammaPair.CrossPairs(pm, pc);
    return Tools.LT(pp.Normal * (Vector2D)p, pp.Value);
  }

  /// <summary>
  /// Method that convexifies the current piecewise-linear positively homogeneous function
  /// taking into account information about places of possible violation of convexity
  /// (given as the subtrahend support function)
  /// </summary>
  /// <param name="suspiciousIndices">Indices of the pair, around which normals the convexity can be violated</param>
  /// <returns>The resultant convex function; if the convex hull is improper function,
  /// the result equals null</returns>
  public SupportFunction ConvexifyFunctionWithInfo(List<int> suspiciousIndices) {
    // If there is no suspicious elements, do nothing
    if (suspiciousIndices.Count == 0) {
      return this;
    }

    int i;

    // Creating the double linked list
    PairInfo[] list = new PairInfo[Count];
    list[0].next = 1;
    list[0].prev = Count - 1;
    list[0].isValid = true;
    for (i = 1; i < Count - 1; i++) {
      list[i].next = i + 1;
      list[i].prev = i - 1;
      list[i].isValid = true;
    }

    list[Count - 1].next = 0;
    list[Count - 1].prev = Count - 2;
    list[Count - 1].isValid = true;

    // Stack of suspicious indices
    Stack<int> suspicious = new Stack<int>(suspiciousIndices);
    double da;

    while (suspicious.Count > 0) {
      i = suspicious.Pop();
      if (list[i].isValid && !CheckTriple(list[i].prev, i, list[i].next)) {
        // Mark the i-th element as invalid
        list[i].isValid = false;

        // Exclude the i-th element from the list (by rearranging links)
        list[list[i].next].prev = list[i].prev;
        list[list[i].prev].next = list[i].next;

        // Check the angle between the new neighbors
        if (Tools.LT(this[list[i].prev].Normal.PolarAngle, this[list[i].next].Normal.PolarAngle)) {
          da = this[list[i].next].Normal.PolarAngle - this[list[i].prev].Normal.PolarAngle;
        } else {
          da = 2 * Math.PI + this[list[i].next].Normal.PolarAngle - this[list[i].prev].Normal.PolarAngle;
        }

        if (Tools.GE(da, Math.PI)) {
          return null;
        }

        // If the angle is OK, add the neighbors to the suspicious collection
        suspicious.Push(list[i].prev);
        suspicious.Push(list[i].next);
      }
    }

    // If we reach this point, the convexification is successfully finished
    // The remained pairs are marked in the list
    // Put them to the resultant function
    List<GammaPair> res = new List<GammaPair>();
    for (i = 0; i < Count; i++) {
      if (list[i].isValid) {
        res.Add(this[i]);
      }
    }

    return new SupportFunction(res);
  }
}
