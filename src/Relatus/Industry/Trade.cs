namespace Relatus.Industry
{
    public abstract class Trade : IBehavior
    {
        public Worker Worker { get; private set; }
        public Factory Workplace { get; private set; }

        public Trade()
        {
        }

        protected virtual void OnAttach()
        {

        }

        internal Trade Attach(Worker worker, Factory workplace)
        {
            Worker = worker;
            Workplace = workplace;

            OnAttach();

            return this;
        }
    }
}
