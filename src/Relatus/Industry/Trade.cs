namespace Relatus.Industry
{
    public class Trade : IBehavior
    {
        public Worker Worker { get; private set; }
        public Factory Workplace { get; private set; }

        public Trade()
        {
        }

        internal Trade Attach(Worker worker, Factory workplace)
        {
            Worker = worker;
            Workplace = workplace;

            return this;
        }
    }
}
