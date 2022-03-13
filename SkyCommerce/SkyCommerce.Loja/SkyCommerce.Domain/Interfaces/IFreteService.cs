﻿using SkyCommerce.Models;
using SkyCommerce.ViewObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkyCommerce.Interfaces
{
    public interface IFreteService
    {
        Task<IEnumerable<Frete>> CalcularFrete(Embalagem embalagem, GeoCoordinate posicao);
        Task<IEnumerable<Frete>> CalcularCarrinho(Carrinho carrinho, GeoCoordinate posicao);
        Task<IEnumerable<DetalhesFrete>> ObterModalidades(GeoCoordinate geoCoordinate);
    }
}