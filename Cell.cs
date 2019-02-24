using System.Collections.Generic;

namespace LargeLaser
{
    // contains state of one location in maze.
    public class Cell
    {
        // what this cell looks like.
        public enum Type
        {
            None,
            Corridor,
            Corner,
            Tee,
            Cross,
            End
        }

        // some gamey state we can apply to this location.
        public enum Action
        {
            None,
            Encounter,
            Chest,
            Exit
        }

        // defines where we can move to from this cell.
        public List<Point> Links;

        // what this cell looks like.
        public Type CellType;

        // gamey state, what happens when we enter this cell.
        public Action ActionType;

        // orientation of this cell.
        public float Angle;

        // gamey state, have we been here before.
        public int Visited;

        public Cell()
        {
            Links = new List<Point>();
            CellType = Type.None;
            ActionType = Action.None;
        }

        public Cell(int x, int y, List<Link> links)
        {
            Links = new List<Point>();
            CellType = Type.None;
            ActionType = Action.None;

            foreach (Link link in links)
            {
                if (link.P1.x == x && link.P1.y == y)
                {
                    Links.Add(new Point(link.P2.x, link.P2.y));
                }
                else if (link.P2.x == x && link.P2.y == y)
                {
                    Links.Add(new Point(link.P1.x, link.P1.y));
                }
            }
        }

        public void makeExit(int x, int y)
        {
            ActionType = Action.Exit;
            Links.Add(new Point(x, y - 1));
        }

        // for querying if we can move to the position in args, from this cell.
        public bool hasLink(int x, int y)
        {
            foreach (Point p in Links)
            {
                if (p.x == x && p.y == y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
