using Lamar;

namespace EventstoreConsumer
{
    internal class ConsumerRegistry : ServiceRegistry
    {
        public ConsumerRegistry()
        {
            Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();
            });
        }
    }
}