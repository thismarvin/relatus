namespace Relatus.Graphics
{
    public class SpriteAtlasEntry
    {
        public string Name { get; private set; }
        public ImageRegion ImageRegion { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }
        public string Page { get; private set; }

        internal SpriteAtlasEntry(string name, int x, int y, int width, int height, int offsetX, int offsetY, string page)
        {
            Name = name;
            ImageRegion = new ImageRegion(x, y, width, height);
            OffsetX = offsetX;
            OffsetY = offsetY;
            Page = page;
        }
    }
}
