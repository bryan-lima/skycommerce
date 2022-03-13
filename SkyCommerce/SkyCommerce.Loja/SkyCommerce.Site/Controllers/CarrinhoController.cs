using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyCommerce.Extensions;
using SkyCommerce.Interfaces;
using SkyCommerce.Site.Models;
using SkyCommerce.Site.Service;
using System.Threading.Tasks;

namespace SkyCommerce.Site.Controllers
{
    [Route("carrinho"), Authorize]
    public class CarrinhoController : Controller
    {
        private readonly ICarrinhoService _carrinhoService;
        private readonly ICarrinhoStore _carrinhoStore;
        private readonly IFreteService _freteService;
        private readonly IGeoposicaoService _geoposicaoService;
        private readonly IProdutoStore _produtoStore;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarrinhoController(
            ICarrinhoService carrinhoService,
            ICarrinhoStore carrinhoStore,
            IFreteService freteService,
            IGeoposicaoService geoposicaoService,
            IProdutoStore produtoStore,
            IHttpContextAccessor httpContextAccessor)
        {
            _carrinhoService = carrinhoService;
            _carrinhoStore = carrinhoStore;
            _freteService = freteService;
            _geoposicaoService = geoposicaoService;
            _produtoStore = produtoStore;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var carrinho = await _carrinhoStore.ObterCarrinho(User.Identity.Name);
            var fretes = await _freteService.CalcularCarrinho(carrinho, await _geoposicaoService.GeolocalizarUsuario());
            return View(new CarrinhoViewModel()
            {
                Carrinho = carrinho,
                Fretes = fretes
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("cupom")]
        public async Task<IActionResult> Cupom(string returnurl, DescontoCarrinhoViewModel model)
        {
            await _carrinhoService.AplicarCupom(model.Cupom, User.Identity.Name);

            if (returnurl.IsPresent())
                return Redirect(returnurl);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("quantidade")]
        public async Task<IActionResult> Quantidade(AtualizarQuantidadeCarrinhoViewModel model)
        {
            await _carrinhoService.AtualizarQuantidadeProduto(User.Identity.Name, model.NomeUnico, model.Quantidade);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("remover")]
        public async Task<IActionResult> Remover(RemoverProdutoCarrinhoViewModel model)
        {
            await _carrinhoService.Remover(model.NomeUnico, User.Identity.Name);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("adicionar")]
        public async Task<IActionResult> Adicionar(AdicionarProdutoCarrinhoViewModel model)
        {
            var produto = await _produtoStore.ObterPorNome(model.NomeUnico);
            await _carrinhoService.AdicionarProduto(User.Identity.Name, produto, 1);

            return RedirectToAction(nameof(HomeController.Index), "Carrinho");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("selecionar-frete")]
        public async Task<IActionResult> SelecionarFrete(string returnurl, [FromForm] SelecionarFreteViewModel model)
        {
            await _carrinhoService.SelecionarFrete(User.Identity.Name, model.Modalidade, await _geoposicaoService.GeolocalizarUsuario());

            if (returnurl.IsPresent())
                return Redirect(returnurl);
            return RedirectToAction(nameof(HomeController.Index), "Carrinho");
        }
    }
}
