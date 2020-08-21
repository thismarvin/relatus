using Microsoft.Xna.Framework;
using Relatus.Graphics;
using System;
using System.Globalization;

namespace Relatus.Debug
{
    public class DebugEntry : IDisposable
    {
        #region Properties
        /// <summary>
        /// A unique name used for retrieval.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A composite format string.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The information that will be used during formatting.
        /// </summary>
        public string[] Information { get; private set; }

        /// <summary>
        /// The final text after formatting the information.
        /// </summary>
        public string Content { get { return entryText.Content; } }
        #endregion

        #region Fields
        private readonly Text entryText;
        #endregion

        /// <summary>
        /// Creates a useful text entry that can be added to the <see cref="DebugManager"/> and displayed during in-game debugging.
        /// </summary>
        /// <param name="name">A unique name used for retrieval.</param>
        /// <param name="format">A composite format string.</param>
        public DebugEntry(string name, string format)
        {
            Name = name;
            Format = format;
            Information = new string[] { "" };

            Vector2 position = DebugManager.NextDebugEntryPosition();
            entryText = new Text("", AssetManager.GetFont("Relatus_Probity"))
                .SetPosition(position.X, position.Y, 0)
                .ApplyChanges();
        }

        /// <summary>
        /// Sets the entries information, formats the new information, and updates the display.
        /// </summary>
        /// <param name="information"></param>
        /// <exception cref="FormatException"></exception>
        public void SetInformation(params object[] information)
        {
            string[] informationAsString = new string[information.Length];
            for (int i = 0; i < information.Length; i++)
            {
                informationAsString[i] = information[i].ToString();
            }

            try
            {
                string.Format(CultureInfo.InvariantCulture, Format, informationAsString);
            }
            catch (FormatException e)
            {
                throw new RelatusException("The amount of information provided is incompatible with the current format string.", e);
            }

            Information = informationAsString;

            FormatEntry();
        }

        private void FormatEntry()
        {
            entryText.SetContent(string.Format(CultureInfo.InvariantCulture, Format, Information));
        }

        public void Draw(Camera camera)
        {
            entryText.Draw(camera);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    entryText.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
