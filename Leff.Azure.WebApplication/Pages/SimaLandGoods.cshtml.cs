using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amphasis.SimaLand.Models;
using Leff.Azure.WebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Leff.Azure.WebApplication.Pages
{
    public class SimaLandGoodsModel : PageModel
    {
        [FromQuery(Name = "pageIndex")]
        public int PageIndex { get; set; }

        public IEnumerable<ItemResponse> Goods { get; private set; }

        private readonly SimaLandService _simaLandService;

        public SimaLandGoodsModel(SimaLandService simaLandService)
        {
            _simaLandService = simaLandService;
        }

        public async Task<IActionResult> OnGet()
        {
            if (PageIndex < 1) return RedirectToPage(routeValues: new {pageIndex = 1});

            var itemList = await _simaLandService.GetItemsAsync(PageIndex);
            Goods = itemList.OrderBy(x => x.Id);

            return Page();
        }
    }
}
