namespace WebApiAuthors.Filters
{
    public class WriteInFile : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string fileName = "file1.txt";
        private Timer timer;

        public WriteInFile(IWebHostEnvironment env)
        {
            this.env = env;
        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            //Write($"process start {DateTime.Now}");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Write($@"process stop {DateTime.Now}");
            return Task.CompletedTask;

        }
        private void DoWork(object state)
        {
            Write($"process work {DateTime.Now}");
        }
        private void Write(string message)
        {
            var route = $@"{env.ContentRootPath}\wwwroot\{fileName}";
            using (StreamWriter writer = new StreamWriter(route, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
