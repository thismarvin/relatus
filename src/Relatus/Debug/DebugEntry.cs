using Microsoft.Xna.Framework;
using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
            entryText = new Text(position.X, position.Y, "", AssetManager.GetFont("Relatus_Probity"));
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
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    entryText.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DebugEntry()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
