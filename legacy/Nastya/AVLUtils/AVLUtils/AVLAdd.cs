using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVLUtils
{
  public partial class AVLTree<TValue>
  {
    /// <summary>
    /// Adding a collection to the tree
    /// </summary>
    /// <param name="collection">The collection to be added</param>
    public void AddRange (IEnumerable<TValue> collection)
    {
      foreach (TValue v in collection)
        Add (v);
    }

    /// <summary>
    /// Procedure for adding a value to the tree
    /// </summary>
    /// <param name="newVal">The value to be added</param>
    public void Add (TValue newVal)
    {
      if (_top == null)
      {
        _top = new AVLNode (newVal);
        return;
      }

      bool added, hChanged = true;
      AddIter (ref _top, newVal, out added, ref hChanged);
      return;
    }

    /// <summary>
    /// A subsidiary recursive function, which searches place to add the element 
    /// and rebalances the tree, if necessary
    /// </summary>
    /// <param name="curNode">The node where the algorithm is</param>
    /// <param name="newVal">The value to be added</param>
    /// <param name="added">Flag showing whether the element has been added</param>
    /// <param name="hChanged">true, if the height of the subtree grows; false, otherwise</param>
    private void AddIter (ref AVLNode curNode, TValue newVal, out bool added, ref bool hChanged)
    {
      // If we left the existing tree, just create a new node and return it as the _top of the new subtree.
      // Thus, the element has been added. And height of corresponding subtree is increased
      if (curNode == null)
      {
        curNode = new AVLNode (newVal);
        added = true;
        hChanged = true;
        return;
      }

      // Otherwise check in what subtree the new element should be placed
      int res = comparer.Compare (newVal, curNode.val);

      AVLNode p1, p2;

      // In the left one
      if (res < 0)
      {
        // Add the element, get the change of the height of the left subtree
        p1 = curNode.left;
        AddIter (ref p1, newVal, out added, ref hChanged);
        curNode.left = p1;
        if (added)
          curNode.subtreeQnt++;

        // If the balance is changed, check and rebalance the tree if necessary
        if (hChanged)
        {
          switch (curNode.balance)
          {
            case 1:
              curNode.balance = 0;
              hChanged = false;
              break;

            case 0:
              curNode.balance = -1;
              break;

            case -1:
              p1 = curNode.left;
              if (p1.balance == -1)
              {
                curNode.left = p1.right;
                p1.right = curNode;
                curNode.balance = 0;

                curNode.SetQnt ();
                curNode = p1;
                curNode.SetQnt ();
              }
              else
              {
                p2 = p1.right;
                p1.right = p2.left;
                p2.left = p1;
                curNode.left = p2.right;
                p2.right = curNode;
                if (p2.balance == -1)
                  curNode.balance = +1;
                else
                  curNode.balance = 0;
                if (p2.balance == +1)
                  p1.balance = -1;
                else
                  p1.balance = 0;

                p1.SetQnt ();
                curNode.SetQnt ();
                curNode = p2;
                curNode.SetQnt ();
              }
              curNode.balance = 0;
              hChanged = false;
              break;
          }
        }
      }
      else if (res > 0)
      {
        // Add the element, get the change of the height of the left subtree
        p1 = curNode.right;
        AddIter (ref p1, newVal, out added, ref hChanged);
        curNode.right = p1;
        if (added)
          curNode.subtreeQnt++;

        if (hChanged)
        {
          switch (curNode.balance)
          {
            case -1:
              curNode.balance = 0;
              hChanged = false;
              break;

            case 0:
              curNode.balance = +1;
              break;

            case +1:
              p1 = curNode.right;
              if (p1.balance == +1)
              {
                curNode.right = p1.left;
                p1.left = curNode;
                curNode.balance = 0;

                curNode.SetQnt ();
                curNode = p1;
                curNode.SetQnt ();
              }
              else
              {
                p2 = p1.left;
                p1.left = p2.right;
                p2.right = p1;
                curNode.right = p2.left;
                p2.left = curNode;
                if (p2.balance == +1)
                  curNode.balance = -1;
                else
                  curNode.balance = 0;
                if (p2.balance == -1)
                  p1.balance = +1;
                else
                  p1.balance = 0;

                p1.SetQnt ();
                curNode.SetQnt ();
                curNode = p2;
                curNode.SetQnt ();
              }
              curNode.balance = 0;
              hChanged = false;
              break;
          }
        }
      }
      else
      {
        added = false;
        hChanged = false;
      }
    }
  }
}