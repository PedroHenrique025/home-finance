namespace home_finance.API.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        /// <summary>
        /// Lista de transações associadas à pessoa.
        /// Aqui utilizei uma Colletion ao invés de List para maior flexibilidade.
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        /// <summary>
        /// booleano para verificar maioridade da pessoa.
        /// </summary>
        public bool IsMinor => Age < 18;
    }
}