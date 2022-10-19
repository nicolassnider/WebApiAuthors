namespace WebApiAuthors.Services
{
    public interface IService
    {
        void DoTask();
        Guid getTransient();
        Guid getScoped();
        Guid getSingleton();
    }
    public class ServiceA : IService
    {
        private readonly ILogger<ServiceA> logger;
        private readonly ServiceTransient serviceTransient;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;

        public ServiceA(ILogger<ServiceA> logger, ServiceTransient serviceTransient, ServiceScoped serviceScoped, ServiceSingleton serviceSingleton)
        {
            this.logger = logger;
            this.serviceTransient = serviceTransient;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
        }    
        public Guid getTransient() {
            return serviceTransient.guid;
        }
        public Guid getScoped()
        {
            return serviceScoped.guid;
        }
        public Guid getSingleton()
        {
            return serviceSingleton.guid;
        }

        public void DoTask()
        {
            throw new NotImplementedException();
        }
    }
    public class ServiceB : IService
    {
        public void DoTask()
        {
            Console.WriteLine("Service B");
        }

        public Guid getScoped()
        {
            throw new NotImplementedException();
        }

        public Guid getSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid getTransient()
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceTransient {
        public Guid guid = Guid.NewGuid();
    }
    public class ServiceScoped
    {
        public Guid guid = Guid.NewGuid();
    }
    public class ServiceSingleton
    {
        public Guid guid = Guid.NewGuid();
    }

}
