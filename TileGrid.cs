// Copyright Robin A. Reynolds-Haertle, 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourRivers
{
    public struct VerticalRange
    {
        public int Upper;
        public int Lower;
        public VerticalRange(int upper, int lower)
        {
            Upper = upper;
            Lower = lower;
        }
    }

    public struct HorizontalRange
    {
        public int Left;
        public int Right;
        public HorizontalRange(int left, int right)
        {
            Left = left;
            Right = right;
        }
    }
  
    public class TileGrid
    {
        private Tile[] m_tiles = new Tile[144];

        public int HowManySelected(Tile[] tiles)
        {
            int count = 0;
            for (int i = 0; i < 144; i++)
            {
                if (m_tiles[i].Selected)
                {
                    tiles[count] = m_tiles[i];
                    count++;
                }
            }
            return count;
        }

        public void RemoveAll()
        {
            for (int i = 0; i < 144; i++)
            {
                m_tiles[i] = null;
            }
        }

        public bool IsGameWon()
        {
            for (int i = 0; i < 144; i++)
            {
                if (!m_tiles[i].Clear)
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearAll()
        {
            foreach (Tile aTile in m_tiles)
            {
                aTile.Clear = true;
            }
        }

        public void UnSelectAll()
        {
            foreach (Tile tile in m_tiles)
            {
                tile.Selected = false;
            }
        }

        public Tile this[int row, int column]
        {
            get
            {
                int index = (row * 12) + column;
                return m_tiles[index];
            }
            set
            {
                int index = (row * 12) + column;
                m_tiles[index] = value;
            }
        }

        public Tile WhichTileClicked(int xmouse, int ymouse)
        {
            return null;
        }

        public bool IsWinnable()
        {
            Tile tileA = null;
            Tile tileB = null;

            while (this.AnyPairsLeft(ref tileA, ref tileB))
            {
                tileA.Clear = true;
                tileB.Clear = true;
            }

            bool isWinnable = (this.HowManyTilesLeft() == 0);
            foreach (Tile tile in m_tiles)
            {
                tile.Clear = false;
            }
            return isWinnable;

        }



        public int HowManyTilesLeft()
        {
            int total = 0;
            foreach (Tile tile in m_tiles)
            {
                if (!tile.Clear)
                {
                    total++;
                }
            }
            return total;
        }

        public bool AnyPairsLeft(ref Tile atile, ref Tile btile)
        {
            Tile tile1, tile2;
            Path path = new Path();
            for (int try1 = 0; try1 < 144; try1++)
            {
                for (int try2 = try1 + 1; try2 < 144; try2++)
                {
                    tile1 = m_tiles[try1];
                    tile2 = m_tiles[try2];
                    if ((tile1 != tile2) &&
                        (!tile1.Clear) && (!tile2.Clear) &&
                        CanRemove(tile1, tile2, ref path))
                    {
                        atile = tile1;
                        btile = tile2;
                        return true;
                    }
                }
            }
            atile = null;
            btile = null;
            return false;
        }

        public bool CanRemove(Tile atile, Tile btile, ref Path path)
        {
            int pathRowColumn = -2;
            //if ((atile.Color == btile.Color) && (atile.Number == btile.Number))
            if ((atile.Number == btile.Number))
                {
                if (HorizontalSweep(atile, btile, ref pathRowColumn))
                {
                    path.Direction = Direction.Horizontal;
                    path.RowOrColumn = pathRowColumn;
                    return true;
                }
                else
                {
                    if (VerticalSweep(atile, btile, ref pathRowColumn))
                    {
                        path.Direction = Direction.Vertical;
                        path.RowOrColumn = pathRowColumn;
                        return true;
                    }
                    else
                        return false;
                }
            }
            return false;
        }

        // VerticalSweep: Find out how far up and down each tile can travel. If there is any overlap
        // in the vertical sweeps, then find out if there is a clear horizontal path running
        // between the two vertical sweeps.
        // HorizontalSweep: Find out how far left and right each tile can travel. If there is any
        // overlap in the horizontal sweeps, then find out if there is a clear vertical path
        // running between the two horizontal sweeps.
        private bool VerticalSweep(Tile tile1, Tile tile2, ref int pathRow)
        {
            Tile left, right;
            if (tile1.Column < tile2.Column)
            {
                left = tile1;
                right = tile2;
            }
            else
            {
                left = tile2;
                right = tile1;
            }

            VerticalRange leftRange = this.GetVerticalRange(left, right);
            VerticalRange rightRange = this.GetVerticalRange(right, left);

            int lower = Math.Max(leftRange.Upper, rightRange.Upper);
            int upper = Math.Min(leftRange.Lower, rightRange.Lower);

            bool foundPathOnRow, foundPath;

            foundPath = false;
            for (int vertical = lower; vertical <= upper; vertical++)
            {
                foundPathOnRow = true;
                // try to pass through each cell
                for (int horizontal = left.Column; horizontal <= right.Column; horizontal++)
                {
                    if (!CanPass(vertical, horizontal, left, right))
                    {
                        foundPathOnRow = false;
                    }
                }
                if (foundPathOnRow)
                {
                    pathRow = vertical;
                    foundPath = true;
                    return foundPath;
                }
            }

            return foundPath;
        }

        private bool HorizontalSweep(Tile tile1, Tile tile2, ref int pathColumn)
        {
            Tile lower;
            Tile upper;
            if (tile1.Row < tile2.Row)
            {
                lower = tile1;
                upper = tile2;
            }
            else
            {
                lower = tile2;
                upper = tile1;
            }

            HorizontalRange lowerRange = GetHorizontalRange(lower, upper);
            HorizontalRange upperRange = GetHorizontalRange(upper, lower);

            int left = Math.Max(lowerRange.Left, upperRange.Left);
            int right = Math.Min(lowerRange.Right, upperRange.Right);

            bool foundPathInColumn, foundPath;

            foundPath = false;
            for (int horizontal = left; horizontal <= right; horizontal++)
            {
                foundPathInColumn = true;
                for (int vertical = lower.Row; vertical <= upper.Row; vertical++)
                {
                    if (!CanPass(vertical, horizontal, lower, upper))
                    {
                        foundPathInColumn = false;
                    }
                }
                if (foundPathInColumn)
                {
                    foundPath = true;
                    pathColumn = horizontal;
                    return foundPath;
                }
            }
            return foundPath;

        }

        private HorizontalRange GetHorizontalRange(Tile atile, Tile btile)
        {
            int left = atile.Column;
            while (CanPass(atile.Row, left - 1, atile, btile))
            {
                left--;
            }

            int right = atile.Column;
            while (CanPass(atile.Row, right + 1, atile, btile))
            {
                right++;
            }
            return new HorizontalRange(left, right);
        }

        private bool CanPass(int row, int column, Tile atile, Tile btile)
        {
            Tile t = null;
            if ((row >= 0) && (row < 12) && (column >= 0) && (column < 12))
            {
                t = this[row, column];
            }

            if ((column < -1) || (row < -1) || (column > 12) || (row > 12))
            {
                return false;
            }
            else if ((column == -1) || (row == -1))
            {
                return true;
            }
            else if ((column == 12) || (row == 12))
            {
                return true;
            }
            else if (t.Clear)
            {
                return true;
            }
            else if ((atile.Row == row) && (atile.Column == column))
            {
                return true;
            }
            else if ((btile.Row == row) && (btile.Column == column))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private VerticalRange GetVerticalRange(Tile atile, Tile btile)
        {
            int lower = atile.Row;
            while (CanPass(lower - 1, atile.Column, atile, btile))
            {
                lower--;
            }

            int upper = atile.Row;
            while (CanPass(upper + 1, atile.Column, atile, btile))
            {
                upper++;
            }

            return new VerticalRange(lower, upper);
        }



    }
}
