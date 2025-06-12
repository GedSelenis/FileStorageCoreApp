using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace FileStorageCoreApp.Controllers
{
    [Route("File")]
    public class FileController : Controller
    {
        IFileService _fileDetailsService;
        public FileController(IFileService fileDetailsService)
        {
            _fileDetailsService = fileDetailsService;
        }
        [Route("Index")]
        [Route("/")]
        public IActionResult Index()
        {
            List<FileResponse> files = _fileDetailsService.ListFiles();
            return View(files);
        }

        [Route("AddFile")]
        [HttpGet]
        public IActionResult AddFile()
        {
            return View();
        }

        [Route("AddFile")]
        [HttpPost]
        public IActionResult AddFile(FileAddRequest fileAddRequest)
        {
            if (fileAddRequest == null)
            {
                return BadRequest("FileAddRequest cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            FileResponse fileResponse = _fileDetailsService.AddFile(fileAddRequest);
            return RedirectToAction("Index","File");
        }

        [Route("delete/{fileID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid fileID)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileID);
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }

            return View(fileResponse);
        }

        [Route("delete/{fileID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(FileResponse fileInputResponse)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileInputResponse.Id);
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }
            _fileDetailsService.DeleteFile(fileResponse.Id);
            return RedirectToAction("Index", "File");
        }

        [Route("AddText/{fileID}")]
        [HttpGet]
        public async Task<IActionResult> AddText(Guid fileID)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileID);
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }
            FileAddTextToFileRequest fileAddTextToFileRequest = new FileAddTextToFileRequest
            {
                Id = fileResponse.Id,
                FileName = fileResponse.FileName,
                FilePath = fileResponse.FilePath
            };

            return View(fileAddTextToFileRequest);
        }

        [Route("AddText/{fileID}")]
        [HttpPost]
        public async Task<IActionResult> AddText(FileAddTextToFileRequest fileInputResponse)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileInputResponse.Id);
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }
            await _fileDetailsService.AddTextToFileAsync(fileInputResponse);
            return RedirectToAction("Index", "File");
        }

        [Route("Rename/{fileID}")]
        [HttpGet]
        public async Task<IActionResult> Rename(Guid fileID)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileID);
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }
            FileRenameRequest fileRenameRequest = new FileRenameRequest
            {
                Id = fileResponse.Id,
                FileName = fileResponse.FileName,
                FilePath = fileResponse.FilePath
            };

            return View(fileRenameRequest);
        }

        [Route("Rename/{fileID}")]
        [HttpPost]
        public async Task<IActionResult> Rename(FileRenameRequest fileInputResponse)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileInputResponse.Id);
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }
            _fileDetailsService.RenameFile(fileInputResponse);
            return RedirectToAction("Index", "File");
        }
    }
}
