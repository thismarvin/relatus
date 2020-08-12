namespace Relatus.Industry
{
    public class Worker : Entity
    {
        public int SSN { get; private set; }

        public Worker(int ssn)
        {
            SSN = ssn;
        }
    }
}
