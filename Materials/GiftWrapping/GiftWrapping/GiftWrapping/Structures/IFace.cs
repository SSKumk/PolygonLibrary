using System.Collections.Generic;

namespace GiftWrapping.Structures
{
    public interface IFace:ICell
    {
        List<ICell> AdjacentCells { get; }
        List<ICell> InnerCells { get; }
        void AddAdjacentCell(ICell cell);
    }
}