namespace StateMachineMapper.Constants;

public class AppSettings
{
    public RabbitMqSettings RabbitMq { get; set; }


    public class RabbitMqSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string VHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
