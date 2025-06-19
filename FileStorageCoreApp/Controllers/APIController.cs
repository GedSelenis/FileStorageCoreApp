using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace FileStorageCoreApp.Controllers
{
    [Route("API")]
    public class APIController : Controller
    {

        IFileService _fileDetailsService;
        IVirtualFolderService _virtualFolderService;
        public APIController(IFileService fileDetailsService, IVirtualFolderService virtualFolderService)
        {
            _fileDetailsService = fileDetailsService;
            _virtualFolderService = virtualFolderService;
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

        [Route("MoveToFolder")]
        [HttpPost]
        public async Task<IActionResult> MoveToFolder([FromForm] FileToFolderRequest fileToFolderRequest)
        {
            if (fileToFolderRequest == null)
            {
                throw new ArgumentNullException(nameof(fileToFolderRequest), "FileToFolderRequest cannot be null.");
            }
            FileResponse? fileResponse = _fileDetailsService.MoveToFolder(fileToFolderRequest);
            return Json(fileResponse);
        }

        // Folder related APIs can be added here as needed

        [Route("FolderIndex")]
        public async Task<IActionResult> FolderIndex()
        {
            List<FolderResponse> folders = await _virtualFolderService.GetAllFolders();
            return Json(folders);
        }

        [Route("AddFolder")]
        [HttpPost]
        public async Task<IActionResult> AddFolder([FromForm] FolderAddRequest folderAddRequest)
        {
            if (folderAddRequest == null)
            {
                return BadRequest("FileAddRequest cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(Errors);
            }
            FolderResponse fileResponse = await _virtualFolderService.AddFolder(folderAddRequest);
            return Json(fileResponse);
        }

        [Route("DeleteFolder/{folderID}")]
        [HttpPost]
        public async Task<IActionResult> DeleteFolder(Guid folderID)
        {
            if (folderID == Guid.Empty)
            {
                return BadRequest("Invalid folder.");
            }
            bool isDeleted = await _virtualFolderService.DeleteFolder(folderID);
            if (isDeleted)
            {
                return Json(isDeleted);
            }
            return NotFound("Folder not found.");
        }

        [Route("UpdateFolder")]
        [HttpPost]
        public async Task<IActionResult> UpdateFolder([FromForm] FolderUpdateRequest folderUpdateRequest)
        {
            if (folderUpdateRequest == null)
            {
                return BadRequest("FolderUpdateRequest cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(Errors);
            }
            FolderResponse fileResponse = await _virtualFolderService.UpdateFolder(folderUpdateRequest);
            return Json(fileResponse);
        }

        [Route("MoveFolderToFolder")]
        [HttpPost]
        public async Task<IActionResult> MoveFolderToFolder([FromForm] FolderToFolderRequest folderToFolderRequest)
        {
            if (folderToFolderRequest == null)
            {
                return BadRequest("FolderToFolderRequest cannot be null.");
            }
            if (!ModelState.IsValid)
            {
                var Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(Errors);
            }
            FolderResponse fileResponse = await _virtualFolderService.MoveToFolder(folderToFolderRequest);
            return Json(fileResponse);
        }
    }
}
