namespace SkyCommerce.Fretes.ViewModels
{
    public class FreteViewModel
    {
        public bool Ativo { get; set; }
        public string Modalidade { get; set; }
        public string Descricao { get; set; }
        public decimal ValorMinimo { get; set; }
        public decimal Multiplicador { get; set; }
    }
}