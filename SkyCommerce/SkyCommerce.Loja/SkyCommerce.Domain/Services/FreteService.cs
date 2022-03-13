using Refit;
using SkyCommerce.Extensions;
using SkyCommerce.Interfaces;
using SkyCommerce.Models;
using SkyCommerce.ViewObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyCommerce.Services
{
    public class FreteService : IFreteService
    {
        private readonly IProdutoStore _produtoStore;

        public FreteService(IProdutoStore produtoStore)
        {
            _produtoStore = produtoStore;
        }


        public Task<IEnumerable<DetalhesFrete>> ObterModalidades(GeoCoordinate geo)
        {
            var freteApi = RestService.For<IFreteApi>("https://localhost:5007");
            return freteApi.Modalidades(PosicaoViewObject.FromGeoCoordinate(geo));
        }

        public Task<IEnumerable<Frete>> CalcularFrete(Embalagem embalagem, GeoCoordinate posicao)
        {
            var freteApi = RestService.For<IFreteApi>("https://localhost:5007");
            return freteApi.Calcular(posicao.Latitude, posicao.Longitude, embalagem);
        }

        public async Task<IEnumerable<Frete>> CalcularCarrinho(Carrinho carrinho, GeoCoordinate posicao)
        {
            var freteApi = RestService.For<IFreteApi>("https://localhost:5007");
            var fretes = (await freteApi.Modalidades(PosicaoViewObject.FromGeoCoordinate(posicao))).Select(Frete.FromViewModel).ToList();
            if (carrinho != null && posicao != null)
            {
                foreach (var carrinhoItem in carrinho.Items)
                {
                    var produto = await _produtoStore.ObterPorNome(carrinhoItem.NomeUnico);
                    var opcoesDeFrete = await freteApi.Calcular(posicao.Latitude, posicao.Longitude, produto.Embalagem);
                    foreach (var frete in fretes)
                    {
                        frete.AtualizarValor(opcoesDeFrete.Modalidade(frete.Modalidade));
                    }
                }
            }

            return fretes;
        }
    }
}
