using System.Collections.Generic;

namespace Relatus.Graphics
{
    public class BatchCollection
    {
        public List<Batch> Batches { get; set; }

        public BatchCollection()
        {
            Batches = new List<Batch>();
        }

        public BatchCollection Draw()
        {
            for (int i = 0; i < Batches.Count; i++)
            {
                Batches[i].Draw();
            }

            return this;
        }
    }
}
