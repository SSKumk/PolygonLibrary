using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolygonLibrary
{
  #region Open types of different elements of polygon figures
  /// <summary>
  /// Class describing a closed polyline
  /// </summary>
  public class Contour
  {
    #region Data
    /// <summary>
    /// List of vertices of the polyline
    /// </summary>
    public List<Vector2D> Vertices { get; private set; }

    /// <summary>
    /// List of edges of the polyline
    /// </summary>
    public List<Segment> Edges { get; private set; }

    /// <summary>
    /// Flag showing whether the is finalized
    /// </summary>
    public bool IsReady { get; private set; }

    /// <summary>
    /// Orientation of a closed contour: +1 - counterclockwise, -1 - clockwise, 0 - the line isn't closed yet
    /// </summary>
    public int Orientation { get; private set; }
    #endregion

    #region Constructors
    /// <summary>
    /// The default constructor
    /// </summary>
    public Contour ()
    {
      Vertices = new List<Vector2D> ();
      Edges = new List<Segment> ();
      IsReady = false;
      Orientation = 0;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="vs">The set of points to be added</param>
    /// <param name="toFinalize">Flas showing whether to finalize the polyline</param>
    public Contour (IEnumerable<Vector2D> vs, bool toFinalize = true)
      : this ()
    {
#if DEBUG
      if (vs.Count () < 3)
        throw new ArgumentException ();
#endif
      bool first = true;
      Vector2D firstVert = null, lastVert = null;
      foreach (Vector2D v in vs)
      {
        if (first)
        {
          firstVert = v;
          first = false;
        }
        if (lastVert != null)
        {
          Vertices.Add (v);
          Edges.Add (new Segment (lastVert, v));
        }
        lastVert = v;
      }
      if (toFinalize)
      {
        Edges.Add (new Segment (lastVert, firstVert));
        IsReady = true;
        ComputeOrientation ();
      }
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="c">The contour to be copied</param>
    public Contour (Contour c)
    {
      Vertices = new List<Vector2D> (c.Vertices);
      Edges = new List<Segment> (c.Edges);
      IsReady = c.IsReady;
    }
    #endregion

    #region Operations
    /// <summary>
    /// Adding a point to the polyline
    /// </summary>
    /// <param name="p">The point to be added</param>
    /// <returns>true, if the point has been added; false, otherwise (it coincides with the lst vertex)</returns>
    public bool AddPoint (Vector2D p)
    {
      int k = Vertices.Count;
      if (k > 0 && p == Vertices[k - 1])
        return false;

      Vertices.Add (p);
      if (k > 0)
        Edges.Add (new Segment (Vertices[k - 1], p));
      return true;
    }

    /// <summary>
    /// Closing the line
    /// </summary>
    public void CloseLine ()
    {
      int k = Vertices.Count;
      if (k < 3)
        throw new Exception ("Cannot close the line: too few vertices!");
      Edges.Add (new Segment (Vertices[k - 1], Vertices[0]));
      IsReady = true;
      ComputeOrientation ();
    }

    /// <summary>
    /// Cyclic increment for an index inside the contour
    /// </summary>
    /// <param name="k">The value to be increased</param>
    /// <param name="d">The amount for increasing</param>
    /// <returns>The new value</returns>
    public int CInc (int k, int d = 1)
    {
      int res = (k + d) % Vertices.Count;
      if (res < 0)
        res += Vertices.Count;
      return res;
    }

    /// <summary>
    /// Cyclic decrement for an index inside the contour
    /// </summary>
    /// <param name="k">The value to be decreased</param>
    /// <param name="d">The amount for decreasing</param>
    /// <returns>The new value</returns>
    public int CDec (int k, int d = 1)
    {
      int res = (k - d) % Vertices.Count;
      if (res < 0)
        res += Vertices.Count;
      return res;
    }

    /// <summary>
    /// Compute signed area of the polygon bounded by the contour
    /// </summary>
    /// <returns>Returns the signed area of the polygon (positive value means that the contour 
    /// is oriented counterclockwise, negative means that the orientation is clockwise)</returns>
    public double ComputeArea()
    {
      if (!IsReady)
        throw new Exception("Try to compute area of a non-closed line!");
      double s = 0;
      int i, j;
      for (i = 0, j = 1; i < Vertices.Count; i = j, j = CInc(j))
        s += Vertices[i] ^ Vertices[j];
      return s;
    }

    /// <summary>
    /// Compute orientation of the contour
    /// </summary>
    private void ComputeOrientation ()
    {
      Orientation = Tools.Sign (ComputeArea());
      if (Orientation == 0)
        throw new Exception ("Contour with zero area!");
    }


    /// <summary>
    /// Checks whether the given point is inside this contour of is located at its boundary
    /// </summary>
    /// <param name="v">The point to be checked</param>
    /// <returns>true, if the point belongs to the contour; false, otherwise</returns>
    public bool isInside(Vector2D v)
    {
      if (!IsReady)
        throw new Exception("Try to check belonging the point to inner space of a non-closed line!");
      double s = 0;
      int i, j;
      for (i = 0, j = 1; i < Vertices.Count; i = j, j = CInc(j))
        s += Vector2D.Angle (Vertices[i], Vertices[j]);
      return Tools.GT(Math.Abs(s));
    }
    #endregion
  }

  /// <summary>
  /// Class for a simple polygonal area
  /// </summary>
  public class Domain : List<Contour>
  {
/*
    #region Constructors
    /// <summary>
    /// An auxiliary constructor to produce a region which border contains all intersection points from a given set
    /// </summary>
    /// <param name="r">The original region which border should be added by some points</param>
    /// <param name="c">The set of crossing points</param>
    public Domain (Domain d, SegmentCrosser c)
      : base ()
    {
      // Turn over all contours of the initial domain, process them, and add the results to the new domain
      foreach (Contour cont in d)
        Add (new Contour (cont, c));
    }
    #endregion
*/ 
  }

  /// <summary>
  /// Class of polygonal figure (region)
  /// </summary>
  public class Region : List<Domain>
  {
/*
    #region Constructors
    /// <summary>
    /// An auxiliary constructor to produce a region which border contains all intersection points from a given set
    /// </summary>
    /// <param name="r">The original region which border should be added by some points</param>
    /// <param name="c">The set of crossing points</param>
    public Region (Region r, SegmentCrosser c)
      : base ()
    {
      // Turn over all domains of the initial domain, process them, and add the results to the new region
      foreach (Domain d in r)
        Add (new Domain (d, c));
    }
    #endregion

    /// <summary>
    /// Preparing temproray regions whose borders contain all intersection points as vertices
    /// </summary>
    /// <param name="A">The original first region</param>
    /// <param name="B">The original second region</param>
    /// <param name="An">The modified first region</param>
    /// <param name="Bn">The modified second region</param>
    private static void PrepareTempRegions (Region A, Region B, out Region An, out Region Bn)
    {
      // Getting the system of segments for both regions
      SegmentCollection segColl = new SegmentCollection ();
      segColl.AddRegion (A);
      segColl.AddRegion (B);

      // Cross these segements
      SegmentCrosser c = new SegmentCrosser (segColl, segColl.incidents);

      // Getting the new regions
      An = new Region (A, c);
      Bn = new Region (B, c);
    }
*/
  }
  #endregion

/*
  #region Internal types for making bool operations on polygon figures
  /// <summary>
  /// Class accumulating segments and information about their incidency from various sources
  /// </summary>
  class SegmentCollection : List<Segment>
  {
    /// <summary>
    /// Information about incidency
    /// </summary>
    public SortedSet<SegmentPair> incidents;

    /// <summary>
    /// The only default constructor
    /// </summary>
    public SegmentCollection ()
      : base ()
    {
      incidents = new SortedSet<SegmentPair> ();
    }

    /// <summary>
    /// Just add a segment
    /// </summary>
    /// <param name="s">The segment to be added</param>
    public void AddSegment (Segment s)
    {
      Add (s);
    }

    /// <summary>
    /// Add two incident segments
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    public void AddTwoIncidentSegments (Segment s1, Segment s2)
    {
      AddSegment (s1);
      AddSegment (s2);
      incidents.Add (new SegmentPair (s1, s2));
    }

    /// <summary>
    /// Adding all segments of a contour 
    /// </summary>
    /// <param name="c">The contour to be added</param>
    public void AddContour (Contour c)
    {
      bool first = true;
      Segment firstSeg = null, lastSeg = null;
      foreach (Segment s in c.Edges)
      {
        if (first)
        {
          first = false;
          firstSeg = s;
        }
        if (lastSeg != null)
          incidents.Add (new SegmentPair (lastSeg, s));
        AddSegment (s);
        lastSeg = s;
      }
      incidents.Add (new SegmentPair (lastSeg, firstSeg));
    }

    /// <summary>
    /// Adding all segments of a domain border
    /// </summary>
    /// <param name="d">The domain whose border should be added</param>
    public void AddDomain (Domain d)
    {
      foreach (Contour c in d)
        AddContour (c);
    }

    /// <summary>
    /// Adding all segments of a region border
    /// </summary>
    /// <param name="r">The region whose border should be added</param>
    public void AddRegion (Region r)
    {
      foreach (Domain d in r)
        AddDomain (d);
    }
  }

  /// <summary>
  /// Information about a vertex
  /// </summary>
  class VertexInfo
  {
    /// <summary>
    /// Whether the vertex is obtained due to crossing two edges
    /// </summary>
    public bool IsCrossVertex;

    /// <summary>
    /// The default constructor
    /// </summary>
    public VertexInfo ()
    {
      IsCrossVertex = false;
    }
  }

  /// <summary>
  /// Marks of edges
  /// </summary>
  enum EdgeMark
  {
    /// <summary>
    /// The edge is in the border of the domain used for marking and is directed
    /// according the border orientation
    /// </summary>
    SHARED1,

    /// <summary>
    /// The edge is in the border of the domain used for marking and is directed
    /// oppositely the border orientation
    /// </summary>
    SHARED2,

    /// <summary>
    /// The edge is inside the domain used for marking
    /// </summary>
    INSIDE,

    /// <summary>
    /// The edge is outside the domain used for marking
    /// </summary>
    OUTSIDE
  }

  /// <summary>
  /// Marks of contours
  /// </summary>
  enum ContourMark  
  {
    /// <summary>
    /// The contour crosses the domain used for marking
    /// </summary>
    ISECTED,

    /// <summary>
    /// The contour is inside the domain used for marking
    /// </summary>
    INSIDE,

    /// <summary>
    /// The contour is outside the domain used for marking
    /// </summary>
    OUTSIDE 
  }

  /// <summary>
  /// Information about an edge
  /// </summary>
  class EdgeInfo
  {
    /// <summary>
    /// Mark of the edge
    /// </summary>
    public EdgeMark mark;

    /// <summary>
    /// The default constructor
    /// </summary>
    public EdgeInfo ()
    {
      mark = EdgeMark.INSIDE;
    }
  }

  /// <summary>
  /// Internal class of contour containing additional information about vertices and edges
  /// </summary>
  class InternalContour : Contour
  {
    #region Data
    /// <summary>
    /// Data on vertices
    /// </summary>
    public Dictionary<Vector2D, VertexInfo> VertexData;

    /// <summary>
    /// Data on vertices
    /// </summary>
    public Dictionary<Segment, EdgeInfo> EdgeData;

    /// <summary>
    /// Mark of the contour
    /// </summary>
    public ContourMark mark;
    #endregion

    #region Consructors
    /// <summary>
    /// Constructor to produce a contour which border contains all intersection points from a given set
    /// </summary>
    /// <param name="cont">The original contour which border should be added by some points</param>
    /// <param name="c">The set of crossing points</param>
    public InternalContour (Contour cont, SegmentCrosser c)
      : base ()
    {
      VertexData = new Dictionary<Vector2D, VertexInfo> ();
      EdgeData = new Dictionary<Segment, EdgeInfo> ();

      mark = ContourMark.INSIDE;

      int i;
      // Loop on vertices and the edges coming from them
      for (i = 0; i < cont.Vertices.Count; i++)
      {
        // Adding the vertex to the new contour 
        AddPoint (cont.Vertices[i]);
        VertexData[cont.Vertices[i]] = new VertexInfo ();

        EdgeData[cont.Edges[cont.Edges.Count - 1]] = new EdgeInfo ();

        // Check whether there are some intersections in the edge
        Segment curEdge = cont.Edges[i];
        if (c.ContainsKey (curEdge))
        {
          if (Tools.EQ(c[curEdge].Min.Part))
            VertexData[cont.Vertices[i]].IsCrossVertex = true;

          // If yes, turn over them and add to the new contour
          foreach (CrossData crData in c[curEdge])
          {
            if (!curEdge.IsEndPoint (crData.p))
              AddPoint (crData.p);
          }
        }
      }

      // Finally, close the line
      CloseLine ();
    }
    #endregion
  }
  #endregion
*/

}
