using System.Collections.Generic;

namespace AVLUtils
{
  public partial class AVLTree<TValue>
  {
    /// <summary>
    /// Remove a set of values from the tree
    /// </summary>
    /// <param name="collection">The collection to be removed</param>
    public void RemoveRange (IEnumerable<TValue> collection)
    {
      foreach (TValue v in collection) {
        Remove(v);
      }
    }

    /// <summary>
    /// Removing given value from the tree
    /// </summary>
    /// <param name="remVal">The value to be removed from the tree</param>
    /// <returns>true, if the value has been removed successfully; 
    /// false, otherwise (if the value is absent in the tree)</returns>
    public bool Remove (TValue remVal)
    {
      bool removed, hChanged = true;
      RemoveIter (ref _top, remVal, out removed, ref hChanged);
      return removed;
    }

    /// <summary>
    /// Auxiliary iterative function for removing
    /// </summary>
    /// <param name="curNode">reference to the current node; can change due to rebalancing</param>
    /// <param name="remVal">value to be removed</param>
    /// <param name="removed">flag showing whether the value has been removed</param>
    /// <param name="hChanged">true, if the height of the subtree grows; false, otherwise</param>
    /// <returns>change of the height of the subtree</returns>
    private void RemoveIter (ref AVLNode? curNode, TValue remVal, out bool removed, ref bool hChanged)
    {
      if (curNode == null)
      {
        removed = false;
        hChanged = false;
      }
      else
      {
        int res = comparer.Compare (remVal, curNode.val);
        AVLNode? p;

        if (res < 0)
        {
          p = curNode.left;
          RemoveIter (ref p, remVal, out removed, ref hChanged);
          curNode.left = p;
          if (removed) {
            curNode.subtreeQnt--;
          }

          if (hChanged) {
            balance1 (ref curNode, ref hChanged);
          }
        }
        else if (res > 0)
        {
          p = curNode.right;
          RemoveIter (ref p, remVal, out removed, ref hChanged);
          curNode.right = p;
          if (removed) {
            curNode.subtreeQnt--;
          }

          if (hChanged) {
            balance2 (ref curNode, ref hChanged);
          }
        }
        else
        {
          removed = true;

          if (curNode.right == null)
          {
            curNode = curNode.left;
            hChanged = true;
          }
          else if (curNode.left == null)
          {
            curNode = curNode.right;
            hChanged = true;
          }
          else
          {
            AVLNode q;
            AVLNode temp = curNode.left;
            del (ref temp, out q, ref hChanged);

            q.left = temp;
            q.right = curNode.right;
            q.balance = curNode.balance;
            curNode = q;

            curNode.SetQnt ();

            if (hChanged) {
              balance1 (ref curNode, ref hChanged);
            }
          }
        }
      }
    }

    /// <summary>
    /// Auxiliary procedure for the case when we should remove a node with two children.
    /// It seeks for the rightmost node in the given subtree (the left subtree if the node to be deleted),
    /// takes it off the tree, rebalances the rest if necessary and returns the _top of the new subtree
    /// and the detached node (to replace the removed node by it)
    /// </summary>
    /// <param name="r">The _top of the left subtree</param>
    /// <param name="q">The detached node</param>
    /// <param name="hChanged">flag showing that the height of the left subtree has been changed</param>
    private void del (ref AVLNode r, out AVLNode q, ref bool hChanged)
    {
      if (r.right != null)
      {
        AVLNode r1 = r.right;
        del (ref r1, out q, ref hChanged);
        r.right = r1;
        r.SetQnt ();
        if (hChanged) {
          balance2 (ref r, ref hChanged);
        }
      }
      else
      {
        q        = r;
        r        = r.left!;
        hChanged = true;
      }
    }

    /// <summary>
    /// An auxiliary procedure for balancing left subtree
    /// </summary>
    /// <param name="p">the _top node of the left subtree</param>
    /// <param name="hChanged">flag showing that the balance has been changed</param>
    private void balance1 (ref AVLNode p, ref bool hChanged)
    {
      AVLNode p1, p2;
      int b1, b2;

      switch (p.balance)
      {
        case -1:
          p.balance = 0;
          break;

        case 0:
          p.balance = +1;
          hChanged = false;
          break;

        case +1:
          p1 = p.right!;
          b1 = p1.balance;
          if (b1 >= 0)
          {
            p.right = p1.left;
            p1.left = p;
            if (b1 == 0)
            {
              p.balance = +1;
              p1.balance = -1;
              hChanged = false;
            }
            else
            {
              p.balance = 0;
              p1.balance = 0;
            }

            p.SetQnt ();
            p1.SetQnt ();

            p = p1;
          }
          else
          {
            p2       = p1.left!;
            b2       = p2.balance;
            p1.left  = p2.right;
            p2.right = p1;
            p.right  = p2.left;
            p2.left  = p;
            if (b2 == +1) {
              p.balance = -1;
            } else {
              p.balance = 0;
            }

            if (b2 == -1) {
              p1.balance = +1;
            } else {
              p1.balance = 0;
            }

            p.SetQnt ();
            p1.SetQnt ();
            p2.SetQnt ();

            p = p2;
            p2.balance = 0;
          }
          break;
      }
    }

    /// <summary>
    /// An auxiliary procedure for balancing right subtree
    /// </summary>
    /// <param name="p">the _top node of the right subtree</param>
    /// <param name="hChanged">flag showing that the balance has been changed</param>
    private void balance2 (ref AVLNode p, ref bool hChanged)
    {
      AVLNode p1, p2;
      int b1, b2;

      switch (p.balance)
      {
        case +1:
          p.balance = 0;
          break;

        case 0:
          p.balance = -1;
          hChanged = false;
          break;

        case -1:
          p1 = p.left!;
          b1 = p1.balance;
          if (b1 <= 0)
          {
            p.left = p1.right;
            p1.right = p;
            if (b1 == 0)
            {
              p.balance = -1;
              p1.balance = +1;
              hChanged = false;
            }
            else
            {
              p.balance = 0;
              p1.balance = 0;
            }

            p.SetQnt ();
            p1.SetQnt ();

            p = p1;
          }
          else
          {
            p2       = p1.right!;
            b2       = p2.balance;
            p1.right = p2.left;
            p2.left  = p1;
            p.left   = p2.right;
            p2.right = p;
            if (b2 == -1) {
              p.balance = +1;
            } else {
              p.balance = 0;
            }

            if (b2 == +1) {
              p1.balance = -1;
            } else {
              p1.balance = 0;
            }

            p.SetQnt ();
            p1.SetQnt ();
            p2.SetQnt ();

            p = p2;
            p2.balance = 0;
          }
          break;
      }
    }



  }
}