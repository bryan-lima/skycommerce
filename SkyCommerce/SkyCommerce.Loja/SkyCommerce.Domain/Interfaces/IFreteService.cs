using SkyCommerce.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkyCommerce.Interfaces
{
    public interface IFreteService
    {
        IEnumerable<Frete> CalcularFrete(Embalagem embalagem, GeoCoordinate posicao);
        Task<IEnumerable<Frete>> CalcularCarrinho(Carrinho carrinho, GeoCoordinate posicao);
    }
}