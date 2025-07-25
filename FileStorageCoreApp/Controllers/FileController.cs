﻿using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System.Threading.Tasks;

namespace FileStorageCoreApp.Controllers
{
    [Route("File")]
    [Authorize]
    public class FileController : Controller
    {
        IFileService _fileDetailsService;
        IVirtualFolderService _virtualFolderService;
        private readonly UserManager<IdentityUser> _userManager;
        public FileController(IFileService fileDetailsService, IVirtualFolderService virtualFolderService,UserManager<IdentityUser> userManager)
        {
            _fileDetailsService = fileDetailsService;
            _virtualFolderService = virtualFolderService;
            _userManager = userManager;
        }
        [Route("Index")]
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            List<FileResponse> files = await _fileDetailsService.ListFiles();
            ViewBag.VirtualFolders = await _virtualFolderService.GetAllFolders();
            return View(files);
        }

        [Route("AddFile")]
        [HttpGet]
        public async Task<IActionResult> AddFile()
        {
            ViewBag.VirtualFolders = (await _virtualFolderService.GetAllFolders()).Select(temp => new SelectListItem() { Text = temp.FolderName, Value = temp.Id.ToString() });
            return View();
        }

        [Route("AddFile")]
        [HttpPost]
        public async Task<IActionResult> AddFile(FileAddRequest fileAddRequest)
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
            fileAddRequest.AddedBy = _userManager.GetUserName(HttpContext.User) ?? string.Empty;
            FileResponse fileResponse = await _fileDetailsService.AddFile(fileAddRequest);
            return RedirectToAction("Index","File");
        }

        [Route("delete/{fileID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid fileID)
        {
            FileResponse? fileResponse = await _fileDetailsService.GetFileDetails(fileID);
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
            FileResponse? fileResponse = await _fileDetailsService.GetFileDetails(fileInputResponse.Id);
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }
            await _fileDetailsService.DeleteFile(fileResponse.Id);
            return RedirectToAction("Index", "File");
        }

        [Route("AddText/{fileID}")]
        [HttpGet]
        public async Task<IActionResult> AddText(Guid fileID)
        {
            FileResponse? fileResponse = await _fileDetailsService.GetFileDetails(fileID);
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
            FileResponse? fileResponse = await _fileDetailsService.GetFileDetails(fileInputResponse.Id);
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
            FileResponse? fileResponse = await _fileDetailsService.GetFileDetails(fileID);
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
            FileResponse? fileResponse = await _fileDetailsService.GetFileDetails(fileInputResponse.Id);
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }
            var res = await _fileDetailsService.RenameFile(fileInputResponse);
            return RedirectToAction("Index", "File");
        }

        [Route("MoveToFolder/{fileID}")]
        [HttpGet]
        public async Task<IActionResult> MoveToFolder(Guid fileID)
        {
            FileResponse? fileResponse = await _fileDetailsService.GetFileDetails(fileID);
            ViewBag.VirtualFolders = (await _virtualFolderService.GetAllFolders()).Select(temp => new SelectListItem() { Text = temp.FolderName, Value = temp.Id.ToString() });
            if (fileResponse == null)
            {
                return RedirectToAction("Index", "File");
            }
            FileToFolderRequest fileToFolderRequest = new FileToFolderRequest
            {
                Id = fileResponse.Id,
                VirualFolderId = fileResponse.VirualFolderId
            };
            return View(fileToFolderRequest);
        }

        [Route("MoveToFolder/{fileID}")]
        [HttpPost]
        public async Task<IActionResult> MoveToFolder(FileToFolderRequest fileToFolderRequest)
        {
            if (fileToFolderRequest == null)
            {
                return RedirectToAction("Index", "File");
            }
            FileResponse? fileResponse = await _fileDetailsService.MoveToFolder(fileToFolderRequest);
            return RedirectToAction("Index", "File");
        }
    }
}
