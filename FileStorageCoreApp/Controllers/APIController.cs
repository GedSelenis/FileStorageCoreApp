using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace FileStorageCoreApp.Controllers
{
    [Route("API")]
    public class APIController : Controller
    {

        IFileService _fileDetailsService;
        public APIController(IFileService fileDetailsService)
        {
            _fileDetailsService = fileDetailsService;
        }

        [Route("Index")]
        [HttpPost]
        public IActionResult Index()
        {
            List<FileResponse> files = _fileDetailsService.ListFiles();
            return Json(files);
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
                var Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(Errors);
            }
            FileResponse fileResponse = _fileDetailsService.AddFile(fileAddRequest);
            return Json(fileResponse);
        }

        [Route("delete/{fileID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid fileID)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileID);
            if (fileResponse == null)
            {
                return BadRequest("Could not find file");
            }
            var result = _fileDetailsService.DeleteFile(fileID);
            return Json(result);
        }

        [Route("AddText/{fileID}")]
        [HttpPost]
        public async Task<IActionResult> AddText(FileAddTextToFileRequest fileInputResponse,Guid fileID)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileID);
            if (fileResponse == null)
            {
                return BadRequest("Could not find file");
            }
            fileInputResponse.FilePath = fileResponse.FilePath;
            fileInputResponse.FileName = fileResponse.FileName;
            fileInputResponse.Id = fileID;
            FileResponse response = await _fileDetailsService.AddTextToFileAsync(fileInputResponse);
            return Json(response);
        }

        [Route("Rename/{fileID}")]
        [HttpPost]
        public async Task<IActionResult> Rename(FileRenameRequest fileInputResponse,Guid fileID)
        {
            FileResponse? fileResponse = _fileDetailsService.GetFileDetails(fileID);
            if (fileResponse == null)
            {
                return BadRequest("Could not find file");
            }
            fileInputResponse.FilePath = fileResponse.FilePath;
            fileInputResponse.FileName = fileResponse.FileName;
            fileInputResponse.Id = fileID;
            fileInputResponse.VirualFolderId = fileResponse.VirualFolderId;
            FileResponse response = _fileDetailsService.RenameFile(fileInputResponse);
            return Json(response);
        }
    }
}
