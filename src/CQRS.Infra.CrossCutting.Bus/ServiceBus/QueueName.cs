namespace CQRS.Infra.CrossCutting.Bus.ServiceBus
{
    public static class QueueName
    {
        public const string Ocorrencia = "ocorrencia";
      
    }


    public class Funcionario {
        public string Nome { get; set; }
        public int Matricula { get; set; }
        public string Status { get; set; }

    }
}


