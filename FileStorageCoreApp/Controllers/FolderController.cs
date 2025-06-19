using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System.Threading.Tasks;

namespace FileStorageCoreApp.Controllers
{
    [Route("Folder")]
    public class FolderController : Controller
    {
        IVirtualFolderService _virtualFolderService;
        public FolderController(IVirtualFolderService folderService)
        {
            _virtualFolderService = folderService;
        }
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            List<FolderResponse> folders = await _virtualFolderService.GetAllFolders();
            return View(folders);
        }

        [Route("AddFolder")]
        [HttpGet]
        public IActionResult AddFolder()
        {

            return View();
        }

        [Route("AddFolder")]
        [HttpPost]
        public async Task<IActionResult> AddFolder(FolderAddRequest folderAddRequest)
        {
            if (folderAddRequest == null)
            {
                return BadRequest("FileAddRequest cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            FolderResponse fileResponse = await _virtualFolderService.AddFolder(folderAddRequest);
            return RedirectToAction("Index", "Folder");
        }

        [Route("Delete/{folderID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid folderID)
        {
            if (folderID == Guid.Empty)
            {
                return BadRequest("Invalid folder ID.");
            }
            FolderResponse folder = await _virtualFolderService.GetFolderById(folderID);
            if (folder == null)
            {
                return NotFound("Folder not found.");
            }
            return View(folder);
        }

        [Route("Delete/{folderID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(FolderResponse folderResponse)
        {
            if (folderResponse == null)
            {
                return BadRequest("Invalid folder.");
            }
            bool isDeleted = await _virtualFolderService.DeleteFolder(folderResponse.Id);
            if (isDeleted)
            {
                return RedirectToAction("Index", "Folder");
            }
            return NotFound("Folder not found.");
        }

        [Route("UpdateFolder/{folderID}")]
        [HttpGet]
        public async Task<IActionResult> UpdateFolder(Guid folderID)
        {
            if (folderID == Guid.Empty )
            {
                return BadRequest("folder id must not be empty");
            }
            FolderUpdateRequest folderUpdateRequest = (await _virtualFolderService.GetFolderById(folderID)).ToFolderUpdateRequest();
            return View(folderUpdateRequest);
        }

        [Route("UpdateFolder/{folderID}")]
        [HttpPost]
        public async Task<IActionResult> UpdateFolder(FolderUpdateRequest folderUpdateRequest)
        {
            if (folderUpdateRequest == null)
            {
                return BadRequest("FileAddRequest cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            FolderResponse fileResponse = await _virtualFolderService.UpdateFolder(folderUpdateRequest);
            return RedirectToAction("Index", "Folder");
        }

    }
}
