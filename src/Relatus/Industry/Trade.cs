namespace Relatus.Industry
{
    public class Trade : IBehavior
    {
        public Factory Workplace { get; private set; }

        public Trade()
        {
        }

        internal Trade AttachFactory(Factory factory)
        {
            Workplace = factory;

            return this;
        }
    }
}
