using System;
using System.Collections.Generic;

namespace Relatus.Industry
{
    public class Worker : Entity, IDisposable
    {
        public uint SSN { get; private set; }

        public Worker(uint ssn)
        {
            SSN = ssn;
        }

        #region IDisposable Support
        private bool disposedValue;

        public void Dispose()
        {
            if (!disposedValue)
            {
                List<IBehavior> behaviors = Behaviors;
                for (int i = 0; i < behaviors.Count; i++)
                {
                    if (behaviors[i] is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                disposedValue = true;
            }
        }
        #endregion
    }
}
