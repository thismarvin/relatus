using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Relatus.Graphics
{
    public class SpriteAtlas
    {
        public string Name { get; private set; }

        private readonly ResourceHandler<SpriteAtlasEntry> entries;
        private readonly ResourceHandler<Texture2D> images;

        public SpriteAtlas()
        {
            entries = new ResourceHandler<SpriteAtlasEntry>();
            images = new ResourceHandler<Texture2D>();
        }

        internal void ParseMeta(string meta)
        {
            string[] data = meta.Split(',');
            Name = data[0];
        }

        internal void AddEntry(string entry)
        {
            string[] data = entry.Split(',');

            SpriteAtlasEntry spriteAtlasEntry = new SpriteAtlasEntry(data[0], int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4]), int.Parse(data[5]), int.Parse(data[6]), Name);
            entries.Register(spriteAtlasEntry.Name, spriteAtlasEntry);
        }

        internal SpriteAtlas LoadSpriteSheet(string path)
        {
            // The SpriteAtlas' texture should be in the same directory as the .damp file.
            // The path parameter is referencing said file, but in order to load the texture we need the path to the directory.
            // This following code is probably excessive, but it should get the path that we are looking for.
            string[] splitPath = path.Split('/');
            StringBuilder pathBuilder = new StringBuilder();
            for (int i = 0; i < splitPath.Length - 1; i++)
            {
                pathBuilder.Append($"{splitPath[i]}/");
            }
            string directory = pathBuilder.ToString();

            string concatedPath = $"{directory}{Name}";
            images.Register(Name, Engine.Instance.Content.Load<Texture2D>(concatedPath));

            return this;
        }

        public Texture2D GetPage(string page)
        {
            return images.Get(page);
        }

        public SpriteAtlasEntry GetEntry(string name)
        {
            return entries.Get(name);
        }
    }
}
