using System;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Segments
{
  // Part of the SegmentCrosserBasic class containing the definition of a subsidiary
  // class for storing segments with some additional information
  public abstract partial class SegmentCrosserBasic
  {
    /// <summary>
    /// Subsidiary class for storing a segment with some additional data
    /// </summary>
    protected internal class InnerSegment : Segment, IComparable<InnerSegment>
    {
#region Global counters and IDs
      /// <summary>
      /// The counter of the added segments 
      /// </summary>
      private static int curID;

      /// <summary>
      /// Number of the segment in the entire collection
      /// </summary>
      protected readonly int myID;

      /// <summary>
      /// Reading property for the ID
      /// </summary>
      public int ID
      {
        get => myID;
      }
#endregion

#region Additional data
      /// <summary>
      /// Reference to the parent object
      /// </summary>
      public readonly Segment Origin;
#endregion

#region Constructors
      /// <summary>
      /// Default constructor for compatability
      /// </summary>
      public InnerSegment()
      {
        myID = curID;
        curID++;

        Origin = null;
      }

      /// <summary>
      /// Constructing a segment with two given ends without the original segment
      /// </summary>
      /// <param name="x1">The abscissa of one end</param>
      /// <param name="y1">The ordinate of one end</param>
      /// <param name="x2">The abscissa of another end</param>
      /// <param name="y2">The ordinate of another end</param>
      public InnerSegment(double x1, double y1, double x2, double y2)
        : this (new Vector2D(x1, y1), new Vector2D(x2,y2)) 
      {
      }

      /// <summary>
      /// Constructing a segment with two given ends without the original segment
      /// </summary>
      /// <param name="p1">The point of one end</param>
      /// <param name="p2">The point of another end</param>
      public InnerSegment(Vector2D p1, Vector2D p2)
      {
        myID = curID;
        curID++;

        Origin = null;  
        
        if (p1 < p2)
        {
          base.p1 = p1;
          base.p2 = p2;
        }
        else
        {
          base.p1 = p2;
          base.p2 = p1;
        }
      }

      /// <summary>
      /// The only constructor on the basis of a segment
      /// </summary>
      /// <param name="s">The original segment</param>
      /// <param name="newID">Possible ID of the new segment; 
      /// if no new ID is given, then ID is taken from the global counter</param>
      public InnerSegment(Segment s, int newID = -1)
      {
        if (newID == -1)
        {
          myID = curID;
          curID++;
        }
        else
        {
          myID = newID;
          curID = Math.Max(curID, newID) + 1;
        }

        Origin = s;

        // Proper orientation of the segment
        if (s.p1 < s.p2)
        {
          p1 = s.p1;
          p2 = s.p2;
        }
        else
        {
          p1 = s.p2;
          p2 = s.p1;
        }

        ComputeParameters();
      }

      /// <summary>
      /// Static constructor to initialize the global counter of segments
      /// </summary>
      static InnerSegment() => curID = 0;
      #endregion

#region Overrided procedures
      /// <summary>
      /// String representation of the inner segment
      /// </summary>
      /// <returns>The string representation</returns>
      public override string ToString() => base.ToString() + ", ID = " + myID;

      /// <summary>
      /// Comparison with other inner segment. 
      /// At first, we compare the objects as segments 
      /// (taking into account that the first end is less than the second one). 
      /// In the case, of equality, compare IDs
      /// </summary>
      /// <param name="other">The inner segment to be comapared with</param>
      /// <returns>-1, if this object is less than the other; +1, if this object is greater; 0, if they are equal</returns>
      public int CompareTo(InnerSegment other)
      {
        int res = base.CompareTo(other);
        if (res != 0)
          return res;
        else
          return myID.CompareTo(other.myID);
      }
#endregion
    }
  }
}  