namespace LargeLaser
{
    // simple library agnostic link between 2 positions.
    public class Link
    {
        public Point P1;
        public Point P2;

        public Link(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public bool hasLink(int x1, int y1, int x2, int y2)
        {
            if ((P1.x == x1 && P1.y == y1 && P2.x == x2 && P2.y == y2) ||
                (P2.x == x1 && P2.y == y1 && P1.x == x2 && P1.y == y2))
            {
                return true;
            }
            return false;
        }
    }
}