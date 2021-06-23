using Relatus.Graphics;

namespace Relatus
{
    /// <summary>
    /// An abstract class that represents a collection of data, logic, and content for a given environment.
    /// </summary>
    public abstract class Scene
    {
        public string Name { get; private set; }
        public Transition EnterTransition { get; set; }
        public Transition ExitTransition { get; set; }

        /// <summary>
        /// Creates an abstract class that represents a collection of data, logic, and content for a given environment.
        /// </summary>
        /// <param name="name">The name that this scene will be referred to in the future.</param>
        public Scene(string name)
        {
            Name = name;
            EnterTransition = null;
            ExitTransition = null;
        }

        /// <summary>
        /// Called right after the scene is loaded.
        /// </summary>
        protected internal virtual void OnEnter() { }

        /// <summary>
        /// Called right before the scene is unloaded.
        /// </summary>
        protected internal virtual void OnExit() { }

        /// <summary>
        /// Updates everything and anything related to the scene.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Renders everything and anything related to the scene. (Make sure to draw everything within a <see cref="Sketch"/>).
        /// </summary>
        public abstract void Draw();
    }
}
