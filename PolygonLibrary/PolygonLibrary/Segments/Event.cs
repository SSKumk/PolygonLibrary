using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVLUtils;

namespace PolygonLibrary.Segments
{
  // Part of the SegmentCrosserBasic class containing the definition of a subsidiary
  // class for storing information about a point where the sweeping structure
  // can change
  public abstract partial class SegmentCrosserBasic
  {
    /// <summary>
    /// A subsidiary class for storing information about a point where the sweeping
    /// structure can change. Contains segments starting, ending at this point and
    /// the ones passing through this point
    /// </summary>
    private class Event : IEnumerable<InnerSegment>
    {
#region Storages
      /// <summary>
      /// Subsidiary type for storing groups of segments in each category (starting, ending, passing).
      /// Groups are sorted acceding by their polar angle
      /// </summary>
      public class GroupDictionary : AVLDictionary<double, SegmentGroup>
      {
#region Miscellaneous procedures        
        /// <summary>
        /// Properly adding a segment
        /// </summary>
        /// <param name="s">The segment to be added</param>
        /// <returns>true, if the segment has been added successfully; false, otherwise</returns>
        public bool Add(InnerSegment s)
        {
          if (!ContainsKey(s.polarAngle))
            this[s.polarAngle] = new SegmentGroup();
          return this[s.polarAngle].Add(s);
        }

        /// <summary>
        /// Checks whether the structure contains the given segment
        /// </summary>
        /// <param name="s">The given segment</param>
        /// <returns>true, if the segment is in the structure; false, otherwise</returns>
        public bool Contains(InnerSegment s) => ContainsKey(s.polarAngle) && this[s.polarAngle].Contains(s);
        #endregion
        
#region Iterating over segments
        /// <summary>
        /// Getting all segments in the structure (despite their angle)
        /// </summary>
        /// <returns>A collection of all segments in the structure</returns>
        public IEnumerable<InnerSegment> GetSegments()
        {
          return Keys.SelectMany(angle => this[angle]);
        }
#endregion        
      }

      /// <summary>
      /// Set of segments having here their start points ("left" ends)
      /// </summary>
      public GroupDictionary L = new GroupDictionary();

      /// <summary>
      /// Set of segments having here their end points ("right" ends)
      /// </summary>
      public GroupDictionary R = new GroupDictionary();

      /// <summary>
      /// Set of segments passing through the point
      /// </summary>
      public GroupDictionary I = new GroupDictionary();
#endregion

#region Iterator over all groups
      /// <summary>
      /// Enumerator for the segments hidden in an event
      /// </summary>
      private class EventSegmentIterator : IEnumerator<InnerSegment>
      {
        /// <summary>
        /// State of the enumerator
        /// </summary>
        private enum State
        {
          Before,
          InL,
          InR,
          InI,
          After
        };

        /// <summary>
        /// The current state of the enumerator
        /// </summary>
        private State state;

        /// <summary>
        /// The event to which this enumerator is connected
        /// </summary>
        private Event myEvent;
        
        /// <summary>
        /// Enumerator of a group the current storage
        /// </summary>
        private IEnumerator<KeyValuePair<double, SegmentGroup>> curGroupEnumerator;
        
        /// <summary>
        /// Enumerator in the current storage
        /// </summary>
        private IEnumerator<InnerSegment> curSegEnumerator;

        /// <summary>
        /// Constructor setting the enumerator to the beginning of the segment collection
        /// associated with the given event
        /// </summary>
        /// <param name="parent">The event to which the new enumerator will be connected</param>
        public EventSegmentIterator(Event parent)
        {
          myEvent = parent;
          state = State.Before;
          curSegEnumerator = null;
          curGroupEnumerator = null;
        }

        /// <summary>
        /// Constructor setting the enumerator to the first occurence of the given segment 
        /// in the segment collection associated with the given event. If there is no such a segment
        /// in the collection, then the enumerator is set to the beginning of the collection
        /// </summary>
        /// <param name="parent">The event to which the new enumerator will be connected</param>
        /// <param name="s">The segment to which the enumerator should be set</param>
        public EventSegmentIterator(Event parent, InnerSegment s)
        {
          myEvent = parent;

          if (myEvent.L.ContainsKey(s.polarAngle))
          {
            curGroupEnumerator = myEvent.L.GetEnumerator(s.polarAngle);
            curSegEnumerator = curGroupEnumerator.Current.Value.GetEnumerator(s);
            try
            {
              if (curSegEnumerator.Current.Equals(s))
              {
                state = State.InL;
                return;
              }
            }
            catch (InvalidOperationException)
            {
            }
          }

          if (myEvent.R.ContainsKey(s.polarAngle))
          {
            curGroupEnumerator = myEvent.R.GetEnumerator(s.polarAngle);
            curSegEnumerator = curGroupEnumerator.Current.Value.GetEnumerator(s);
            try
            {
              if (curSegEnumerator.Current.Equals(s))
              {
                state = State.InR;
                return;
              }
            }
            catch (InvalidOperationException)
            {
            }
          }

          if (myEvent.I.ContainsKey(s.polarAngle))
          {
            curGroupEnumerator = myEvent.I.GetEnumerator(s.polarAngle);
            curSegEnumerator = curGroupEnumerator.Current.Value.GetEnumerator(s);
            try
            {
              if (curSegEnumerator.Current.Equals(s))
              {
                state = State.InI;
                return;
              }
            }
            catch (InvalidOperationException)
            {
            }
          }

          state = State.Before;
          curGroupEnumerator = null;
          curSegEnumerator = null;
        }

        /// <summary>
        /// Getting property showing whether the iterator has a valid value
        /// </summary>
        public bool IsValid
        {
          get => state != State.Before && state != State.After;
        }

        /// <summary>
        /// Getting property of the current value
        /// </summary>
        public InnerSegment Current
        {
          get
          {
#if DEBUG            
            if (!IsValid)
              throw new InvalidOperationException();
#endif            
            return curSegEnumerator.Current;
          }
        }

        /// <summary>
        /// Getting property of non-generic interface
        /// </summary>
        object IEnumerator.Current
        {
          get => Current;
        }

        /// <summary>
        /// Dispose method (for the aim of compatability)
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Setting up the enumerator
        /// </summary>
        public void Reset()
        {
          state = State.Before;
          curGroupEnumerator = null;
          curSegEnumerator = null;
        }

        /// <summary>
        /// Step further
        /// </summary>
        /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
        public bool MoveNext()
        {
          if (state == State.Before)
          {
            curGroupEnumerator = myEvent.L.GetEnumerator();
            curSegEnumerator = curGroupEnumerator.Current.Value.GetEnumerator();
            state = State.InL;
          }

          if (state == State.InL)
          {
            if (curSegEnumerator.MoveNext())
              return true;
            
            curGroupEnumerator = myEvent.R.GetEnumerator();
            curSegEnumerator = curGroupEnumerator.Current.Value.GetEnumerator();
            state = State.InR;
          }
          
          if (state == State.InR)
          {
            if (curSegEnumerator.MoveNext())
              return true;
            
            curGroupEnumerator = myEvent.I.GetEnumerator();
            curSegEnumerator = curGroupEnumerator.Current.Value.GetEnumerator();
            state = State.InR;
          }

          if (state == State.InI)
          {
            if (curSegEnumerator.MoveNext())
              return true;
          
            state = State.After;
          }

          return false;
        }
      }

      /// <summary>
      /// Getting iterator over all segments put into the event
      /// </summary>
      /// <returns>The iterator set to the beginning of the collection</returns>
      public IEnumerator<InnerSegment> GetEnumerator() => new EventSegmentIterator(this);

      /// <summary>
      /// Getting a non-generic enumerator over all segments put into the event
      /// </summary>
      /// <returns>A non-generic enumerator set to the beginning of the collection</returns>
      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      /// <summary>
      /// Get enumerator set to the first occurence of the given segment in the collection 
      /// of segments associated with the event. If the given segment is not associated with the event,
      /// then the method returns an enumerator set to the beginning of the collection
      /// </summary>
      /// <param name="s">A segment to which the enumerator should be set to</param>
      /// <returns>An enumerator set to the given segment, if the segment is in the collection;
      /// or to the beginning of the collection, otherwise</returns>
      public IEnumerator<InnerSegment> GetEnumerator(InnerSegment s) => new EventSegmentIterator(this, s);
      #endregion
    }
  }
}