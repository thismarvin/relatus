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

        protected internal virtual void OnJumpstart()
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
