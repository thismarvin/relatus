namespace Relatus.Industry
{
    public class Trade : Behavior
    {
        public Factory Workplace { get; private set; }

        public Trade()
        {
        }

        public Trade AttachFactory(Factory factory)
        {
            Workplace = factory;

            return this;
        }
    }
}
