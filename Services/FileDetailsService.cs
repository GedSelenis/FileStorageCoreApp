using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Services
{
    public class FileDetailsService : IFileService
    {
        ApplicationDbContext _db;
        //List<FileDetails> _fileDetailsList = new List<FileDetails>();
        //List<VirtualFolder> _virtualFolderList = new List<VirtualFolder>();
        public FileDetailsService(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        List<VirtualFolder> ReadStoredFoldersXmlFile()
        {
            List<VirtualFolder> fileDetails = new List<VirtualFolder>();
            StreamReader file = new StreamReader("StoredFolders.xml");
            string xmlContent = file.ReadToEnd();
            file.Close();
            XmlSerializer serializer = new XmlSerializer(typeof(List<VirtualFolder>));
            using (TextReader reader = new StringReader(xmlContent))
            {
                fileDetails = (List<VirtualFolder>)serializer.Deserialize(reader);
            }
            return fileDetails;
        }

        List<FileDetails> ReadXmlFile()
        {
            List<FileDetails> fileDetails = new List<FileDetails>();
            StreamReader file = new StreamReader("StoredFiles.xml");
            string xmlContent = file.ReadToEnd();
            file.Close();
            XmlSerializer serializer = new XmlSerializer(typeof(List<FileDetails>));
            using (TextReader reader = new StringReader(xmlContent))
            {
                fileDetails = (List<FileDetails>)serializer.Deserialize(reader);
            }
            return fileDetails;
        }

        void WriteXmlFile(List<FileDetails> fileDetails)
        {
            StreamWriter file1 = new StreamWriter("StoredFiles.xml");
            XmlSerializer x = new XmlSerializer(fileDetails.GetType());
            x.Serialize(file1, fileDetails);
            file1.Close();
        }

        async Task<FileResponse> AddTextToFileAsync(FileAddTextToFileRequest fileAddTextToFileRequest)
        {
            if (fileAddTextToFileRequest == null)
            {
                throw new ArgumentNullException(nameof(fileAddTextToFileRequest), "FileAddTextToFileRequest cannot be null.");
            }
            ValidationHelper.ModelValidation(fileAddTextToFileRequest);
            using (var streamWriter = new StreamWriter(Path.Combine(fileAddTextToFileRequest.FilePath, fileAddTextToFileRequest.FileName), true))
            {
                await streamWriter.WriteLineAsync(fileAddTextToFileRequest.TextToAdd);
            }


            FileDetails fileDetails = fileAddTextToFileRequest.ToFileDetails();
            return fileDetails.ToFileResponse();
        }

        async Task<bool> DeleteFile(Guid? fileId)
        {
            if (fileId == null || fileId == Guid.Empty)
            {
                throw new ArgumentException("File ID cannot be empty or null.", nameof(fileId));
            }
            FileResponse fileDetails = GetFileDetails(fileId);

            try
            {
                if (File.Exists(Path.Combine(fileDetails.FilePath, fileDetails.FileName)))
                {
                    File.Delete(Path.Combine(fileDetails.FilePath, fileDetails.FileName));
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Error deleting file: {ex.Message}", ex);
            }

            _db.FileDetails.RemoveRange(_db.FileDetails.Where(f => f.Id == fileId));
            await _db.SaveChangesAsync();

            return true;
        }

        FileResponse? GetFileDetails(Guid? fileId)
        {
            if (fileId == null || fileId == Guid.Empty)
            {
                throw new ArgumentException("File ID cannot be empty or null.", nameof(fileId));
            }
            FileDetails? fileDetails = _db.FileDetails.Where(f => f.Id == fileId).FirstOrDefault();
            return fileDetails?.ToFileResponse() ?? null;
        }

        async Task<List<FileResponse>> ListFilesAsync()
        {
            var files = await _db.FileDetails.Include("VirtualFolder").ToListAsync();
            List<FileResponse> fileResponses = files.Select(f => f.ToFileResponse()).ToList();
            for (int i = 0; i < fileResponses.Count; i++)
            {
                string virtualFoderPath = "";
                VirtualFolder? virtualFolder = _db.VirtualFolders.FirstOrDefault(f => f.Id == fileResponses[i].VirualFolderId);
                virtualFoderPath = virtualFolder?.FolderName ?? "";
                if (virtualFolder != null && virtualFolder.ParentFolderId != null)
                {
                    VirtualFolder? parentFolder = _db.VirtualFolders.FirstOrDefault(f => f.Id == virtualFolder.ParentFolderId);
                    while (parentFolder != null)
                    {
                        virtualFoderPath = Path.Combine(parentFolder.FolderName, virtualFoderPath);
                        parentFolder = _db.VirtualFolders.FirstOrDefault(f => f.Id == parentFolder.ParentFolderId);
                    }
                }

                fileResponses[i].VirualFolderName = virtualFoderPath;
            }
            return fileResponses;
        }

        async Task<FileResponse> RenameFile(FileRenameRequest fileRenameRequest)
        {
            if (fileRenameRequest == null)
            {
                throw new ArgumentNullException(nameof(fileRenameRequest), "FileRenameRequest cannot be null.");
            }
            ValidationHelper.ModelValidation(fileRenameRequest);
            FileDetails? fileDetails = _db.FileDetails.Where(f => f.Id == fileRenameRequest.Id).FirstOrDefault();
            if (fileDetails == null)
            {
                throw new KeyNotFoundException($"File with ID {fileRenameRequest.Id} not found.");
            }
            if (_db.FileDetails.Any(f => f.FileName == fileRenameRequest.NewFileName && f.VirualFolderId == fileDetails.VirualFolderId))
            {
                for (int i = 0; i < 100; i++)
                {
                    string newFileName = $"{fileRenameRequest.NewFileName.Split('.')[0]} ({i}).{fileRenameRequest.NewFileName.Split('.')[1]}";
                    if (!_db.FileDetails.Any(f => f.FileName == newFileName && f.VirualFolderId == fileDetails.VirualFolderId))
                    {
                        fileRenameRequest.NewFileName = newFileName;
                        break;
                    }
                    else if (i == 99)
                    {
                        throw new InvalidOperationException("Unable to generate a unique file name after 100 attempts.");
                    }
                }
            }
            string oldFilePath = Path.Combine(fileDetails.FilePath, fileDetails.FileName);
            string newFilePath = Path.Combine(fileDetails.FilePath, fileRenameRequest.NewFileName);
            File.Move(oldFilePath, newFilePath);
            fileDetails.FileName = fileRenameRequest.NewFileName;
            fileDetails.FilePath = fileDetails.FilePath;
            await _db.SaveChangesAsync();
            return fileDetails.ToFileResponse();
        }

        async Task<FileResponse> AddFile(FileAddRequest fileAddRequest)
        {
            if (fileAddRequest == null)
            {
                throw new ArgumentNullException(nameof(fileAddRequest), "FileAddRequest cannot be null.");
            }
            ValidationHelper.ModelValidation(fileAddRequest);
            FileDetails fileDetails = fileAddRequest.ToFileDetails();
            if (_db.FileDetails.Any(f => f.FileName == fileAddRequest.FileName && f.VirualFolderId == fileAddRequest.VirualFolderId))
            {
                for (int i = 0; i < 100; i++)
                {
                    string newFileName = $"{fileAddRequest.FileName.Split('.')[0]} ({i}).{fileAddRequest.FileName.Split('.')[1]}";
                    if (!_db.FileDetails.Any(f => f.FileName == newFileName && f.VirualFolderId == fileAddRequest.VirualFolderId))
                    {
                        fileDetails.FileName = newFileName;
                        break;
                    }
                    else if (i == 99)
                    {
                        throw new InvalidOperationException("Unable to generate a unique file name after 100 attempts.");
                    }
                }
            }
            if (!File.Exists(Path.Combine(fileDetails.FilePath, fileDetails.FileName)))
            {
                File.Create(Path.Combine(fileDetails.FilePath, fileDetails.FileName)).Dispose();
            }
            _db.FileDetails.Add(fileDetails);
            await _db.SaveChangesAsync();
            return fileDetails.ToFileResponse();
        }

        async Task<FileResponse> IFileService.AddFile(FileAddRequest fileAddRequest)
        {
            return await AddFile(fileAddRequest);
        }

        Task<FileResponse> IFileService.RenameFile(FileRenameRequest fileRenameRequest)
        {
            return RenameFile(fileRenameRequest);
        }

        Task<FileResponse> IFileService.AddTextToFileAsync(FileAddTextToFileRequest fileAddTextToFileRequest)
        {
            return AddTextToFileAsync(fileAddTextToFileRequest);
        }

        async Task<bool> IFileService.DeleteFile(Guid? fileId)
        {
            return await DeleteFile(fileId);
        }

        async Task<List<FileResponse>> IFileService.ListFiles()
        {
            return await ListFilesAsync();
        }

        FileResponse? IFileService.GetFileDetails(Guid? fileId)
        {
            return GetFileDetails(fileId);
        }

        public async Task<FileResponse> MoveToFolder(FileToFolderRequest moveToFolderRequest)
        {
            if (moveToFolderRequest == null)
            {
                throw new ArgumentNullException(nameof(moveToFolderRequest), "FileToFolderRequest cannot be null.");
            }
            ValidationHelper.ModelValidation(moveToFolderRequest);
            FileDetails? fileDetails = _db.FileDetails.FirstOrDefault(f => f.Id == moveToFolderRequest.Id);
            if (fileDetails == null)
            {
                throw new KeyNotFoundException($"File with ID {moveToFolderRequest.Id} not found.");
            }
            else if (fileDetails.VirualFolderId == moveToFolderRequest.VirualFolderId)
            {
                throw new InvalidOperationException("File is already in the specified folder.");
            }
            if (_db.FileDetails.Any(f => f.FileName == fileDetails.FileName && f.VirualFolderId == moveToFolderRequest.VirualFolderId))
            {
                for (int i = 0; i < 100; i++)
                {
                    string newFileName = $"{fileDetails.FileName.Split('.')[0]} ({i}).{fileDetails.FileName.Split('.')[1]}";
                    if (!_db.FileDetails.Any(f => f.FileName == newFileName && f.VirualFolderId == moveToFolderRequest.VirualFolderId))
                    {
                        fileDetails.FileName = newFileName;
                        break;
                    }
                    else if (i == 99)
                    {
                        throw new InvalidOperationException("Unable to generate a unique file name after 100 attempts.");
                    }
                }
            }
            fileDetails.VirualFolderId = moveToFolderRequest.VirualFolderId;
            await _db.SaveChangesAsync();
            return fileDetails.ToFileResponse();
        }
    }
}
