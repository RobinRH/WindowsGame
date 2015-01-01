// Copyright Robin A. Reynolds-Haertle, 2015



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourRivers
{

    public enum Direction
    {
        Horizontal, Vertical
    };


    /// <summary>
    /// The Path object represents the horizontal or vertical path that connects the two tiles. If Path is vertical, then each Tile first moves
    /// vertically to get to the Path. If Path is horizontal, then each Tile moves horizontally to get to the Path.
    /// </summary>
    public struct Path
    {
        public int RowOrColumn;
        public Direction Direction;
        public Path(Direction direction, int rowOrColumn)
        {
            RowOrColumn = rowOrColumn;
            Direction = direction;
        }

    }
}
