﻿using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IFileService
    {
        public Task<FileResponse> AddFile(FileAddRequest fileAddRequest);
        public Task<FileResponse> RenameFile(FileRenameRequest fileRenameRequest);
        public Task<FileResponse> AddTextToFileAsync(FileAddTextToFileRequest fileAddTextToFileRequest);
        public Task<bool> DeleteFile(Guid? fileId);
        public Task<List<FileResponse>> ListFiles();
        public Task<FileResponse?> GetFileDetails(Guid? fileId);
        public Task<FileResponse> MoveToFolder(FileToFolderRequest moveToFolderRequest);
    }
}
