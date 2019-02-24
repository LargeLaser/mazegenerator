using System.Collections.Generic;

namespace LargeLaser
{
    public class Maze
    {
        Cell[,] m_cells;
        int m_width;
        int m_length;

        const int UNKNOWN = 0;
        const int VISITED = 1;

        public Cell InvalidCell;

        public int Width { get { return m_width; } }
        public int Length { get { return m_length; } }
        public Cell[,] Cells { get { return m_cells; } }

        public Maze()
        {
            InvalidCell = new Cell();
        }

        public void create(int seed, int width, int length)
        {
            System.Random rand = new System.Random(seed);

            m_width = width;
            m_length = length;
            m_cells = new Cell[m_width, m_length];

            generate(rand);
        }

        // get the cell at this location.
        public Cell cell(int x, int y)
        {
            if (valid(x, y))
            {
                return m_cells[x, y];
            }
            return InvalidCell;
        }

        // bounds checking.
        public bool valid(int x, int y)
        {
            return x >= 0 && x < m_width && y >= 0 && y < m_length;
        }

        // for serialisation.
        public void resize(int width, int length)
        {
            m_width = width;
            m_length = length;
            m_cells = new Cell[m_width, m_length];
        }

        void generate(System.Random rand)
        {
            // holds state of area as we traverse it.
            int[,] map = new int[m_width, m_length];

            // directions we will travel from any cell.
            Point[] dirs = new Point[]
            {
                new Point(1,0),
                new Point(-1,0),
                new Point(0,1),
                new Point(0,-1),
            };
            

            // holds valid movements between cells.
            List<Link> links = new List<Link>();

            // current active cells, where we can continue to travel.
            List<Point> ends = new List<Point>();

            // cells we will append to active list.
            List<Point> newEnds = new List<Point>();

            // some arbitrary position where we start from. middle/bottom.
            Point start = new Point(m_width / 2, m_length - 1);
            ends.Add(start);
            map[start.x, start.y] = VISITED;

            while (ends.Count > 0)
            {
                newEnds.Clear();

                // pick a random active cell what we will move from,
                // and add all valid possible cells we can move to.
                int c1 = rand.Next(ends.Count);
                if (addEnds(map, ends[c1], dirs, links, newEnds, rand))
                {
                    // no cells to move to, this active cell is dead.
                    ends.RemoveAt(c1);
                    --c1;
                }
                if (newEnds.Count > 0)
                {
                    // got some, add to the active list.
                    ends.AddRange(newEnds);
                }
            }

            createCells(links);
        }

        bool addEnds(int[,] map, Point end, Point[] dirs, List<Link> links, List<Point> ends, System.Random rand)
        {
            int nx;
            int ny;
            List<int> possibles = new List<int>(dirs.Length);
            int c1 = 0;
            foreach (Point dir in dirs)
            {
                nx = end.x + dir.x;
                ny = end.y + dir.y;
                //if (nx >= 0 && ny >= 0 && nx < m_width && ny < m_length)
                if(valid(nx, ny))
                {
                    if (map[nx, ny] == UNKNOWN)
                    {
                        // not looked at this cell yet, add it as a potential.
                        possibles.Add(c1);
                    }
                }
                ++c1;
            }

            if (possibles.Count == 0)
            {
                return true;
            }

            // pick a random destination from all the potential cells from this cell.
            int index = rand.Next(possibles.Count);
            index = possibles[index];

            // add to active list.
            nx = end.x + dirs[index].x;
            ny = end.y + dirs[index].y;
            Point p = new Point(nx, ny);
            ends.Add(p);

            // add link from this cell to our new active one, and mark as visited.
            links.Add(new Link(end, p));
            map[nx, ny] = VISITED;
            return false;
        }

        void createCells(List<Link> links)
        {
            for (int y = 0; y < m_length; ++y)
            {
                for (int x = 0; x < m_width; ++x)
                {
                    Cell cell = new Cell(x, y, links);
                    cell.CellType = getAt(x, y, links, ref cell.Angle);
                    m_cells[x, y] = cell;
                }
            }
        }

        Cell.Type getAt(int x, int y, List<Link> links, ref float angle)
        {
            // figure out what this cell looks like.
            bool up = hasLink(x, y, x, y + 1, links);
            bool down = hasLink(x, y, x, y - 1, links);
            bool left = hasLink(x, y, x - 1, y, links);
            bool right = hasLink(x, y, x + 1, y, links);

            // if it has 4 directions, it's a crossroads
            if (up && down && left && right)
            {
                return Cell.Type.Cross;
            }

            // 3 directions, t junction.
            else if (up && down && left)
            {
                return Cell.Type.Tee;
            }
            else if (left && right && up)
            {
                angle = 90;
                return Cell.Type.Tee;
            }
            else if (up && down && right)
            {
                angle = 180;
                return Cell.Type.Tee;
            }
            else if (left && right && down)
            {
                angle = 270;
                return Cell.Type.Tee;
            }

            // 2 directions 90 degrees to each other, it's a corner.
            else if (right && down)
            {
                return Cell.Type.Corner;
            }
            else if (left && down)
            {
                angle = 90;
                return Cell.Type.Corner;
            }
            else if (left && up)
            {
                angle = 180;
                return Cell.Type.Corner;
            }
            else if (right && up)
            {
                angle = 270;
                return Cell.Type.Corner;
            }

            // 2 directions 180 degrees to each other, it's a corridor.
            else if (left && right)
            {
                angle = 90;
                return Cell.Type.Corridor;
            }
            else if (up && down)
            {
                return Cell.Type.Corridor;
            }

            // only one direction, it's an end.
            else if (down)
            {
                return Cell.Type.End;
            }
            else if (left)
            {
                angle = 90;
                return Cell.Type.End;
            }
            else if (up)
            {
                angle = 180;
                return Cell.Type.End;
            }
            else if (right)
            {
                angle = 270;
                return Cell.Type.End;
            }

            return Cell.Type.None;
        }

        bool hasLink(int x1, int y1, int x2, int y2, List<Link> links)
        {
            foreach (Link link in links)
            {
                if(link.hasLink(x1, y1, x2, y2))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
