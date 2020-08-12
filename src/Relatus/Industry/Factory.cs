namespace Relatus.Industry {
    public class Factory {
        public int Capacity { get; private set; }

        private readonly Worker[] workers;

        public Factory(int capacity) {
            Capacity = capacity;
            workers = new Worker[Capacity];
        }
    }
}
