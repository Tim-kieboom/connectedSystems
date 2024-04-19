namespace backend.Controllers
{
    public record Position
    {
        public Position() { }
        public Position(int x, int y)
        {
            Set(x, y);
        }

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
