using Relatus.ECS;
using Relatus.Graphics;
using Relatus.Graphics.Transitions;
using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// An abstract class that represents a collection of data, logic, and content for a given environment.
    /// </summary>
    public abstract class Scene
    {
        public Transition EnterTransition { get; set; }
        public Transition ExitTransition { get; set; }
        public string Name { get; private set; }
        public RectangleF SceneBounds { get; set; }

        /// <summary>
        /// Creates an abstract class that represents a collection of data, logic, and content for a given environment.
        /// </summary>
        /// <param name="name">The name that this scene will be referred to in the future.</param>
        public Scene(string name)
        {
            Name = name;

            EnterTransition = new Pinhole(TransitionType.Enter);
            ExitTransition = new Fade(TransitionType.Exit);
        }

        /// <summary>
        /// Performs logic related to entering a scene. (Automatically called right after the previous scene was unloaded).
        /// </summary>
        public abstract void LoadScene();

        /// <summary>
        /// Performs logic related to leaving a scene. (Automatically called right before the next scene is loaded).
        /// </summary>
        public abstract void UnloadScene();

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
