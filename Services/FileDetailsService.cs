using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Repositories;
using RepositoryContracts;
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
        IFileRepository _fileRepository;
        IFolderRepository _folderRepository;
        //List<FileDetails> _fileDetailsList = new List<FileDetails>();
        //List<VirtualFolder> _virtualFolderList = new List<VirtualFolder>();
        public FileDetailsService(IFileRepository fileRepository, IFolderRepository folderRepository)
        {
            _fileRepository = fileRepository;
            _folderRepository = folderRepository;
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
            FileResponse fileDetails = (await _fileRepository.GetFileDetails(fileId.Value)).ToFileResponse();

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

            return await _fileRepository.DeleteFile(fileId.Value);
        }

        async Task<FileResponse?> GetFileDetails(Guid? fileId)
        {
            if (fileId == null || fileId == Guid.Empty)
            {
                throw new ArgumentException("File ID cannot be empty or null.", nameof(fileId));
            }
            FileDetails? fileDetails = await _fileRepository.GetFileDetails(fileId.Value);
            return fileDetails?.ToFileResponse() ?? null;
        }

        async Task<List<FileResponse>> ListFilesAsync()
        {
            var files = await _fileRepository.ListFiles();
            List<FileResponse> fileResponses = files.Select(f => f.ToFileResponse()).ToList();
            for (int i = 0; i < fileResponses.Count; i++)
            {
                string virtualFoderPath = "";
                VirtualFolder? virtualFolder = await _folderRepository.GetFolder(fileResponses[i].VirualFolderId ?? Guid.Empty) ?? null;
                virtualFoderPath = virtualFolder?.FolderName ?? "";
                if (virtualFolder != null && virtualFolder.ParentFolderId != null)
                {
                    VirtualFolder? parentFolder = await _folderRepository.GetFolder(virtualFolder.ParentFolderId.Value);
                    while (parentFolder != null)
                    {
                        virtualFoderPath = Path.Combine(parentFolder.FolderName, virtualFoderPath);
                        parentFolder = await _folderRepository.GetFolder(parentFolder.ParentFolderId ?? Guid.Empty);
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
            //ValidationHelper.ModelValidation(fileRenameRequest);
            FileDetails? fileDetails = await _fileRepository.GetFileDetails(fileRenameRequest.Id);
            if (fileDetails == null)
            {
                throw new KeyNotFoundException($"File with ID {fileRenameRequest.Id} not found.");
            }
            if (await _fileRepository.Contains(f => f.FileName.Equals(fileRenameRequest.NewFileName) && f.VirualFolderId.Equals(fileDetails.VirualFolderId)))
            {
                for (int i = 0; i < 100; i++)
                {
                    string newFileName = $"{fileRenameRequest.NewFileName.Split('.')[0]} ({i}).{fileRenameRequest.NewFileName.Split('.')[1]}";
                    if (!await _fileRepository.Contains(f => f.FileName.Equals(newFileName) && f.VirualFolderId.Equals(fileDetails.VirualFolderId)))
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
            await _fileRepository.UpdateFile(fileDetails);
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
            if (await _fileRepository.Contains(f => f.FileName == fileAddRequest.FileName && f.VirualFolderId == fileAddRequest.VirualFolderId))
            {
                for (int i = 0; i < 100; i++)
                {
                    string newFileName = $"{fileAddRequest.FileName.Split('.')[0]} ({i}).{fileAddRequest.FileName.Split('.')[1]}";
                    if (!await _fileRepository.Contains(f => f.FileName == newFileName && f.VirualFolderId == fileAddRequest.VirualFolderId))
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
            await _fileRepository.AddFile(fileDetails);
            return fileDetails.ToFileResponse();
        }

        async Task<FileResponse> IFileService.AddFile(FileAddRequest fileAddRequest)
        {
            return await AddFile(fileAddRequest);
        }

        async Task<FileResponse> IFileService.RenameFile(FileRenameRequest fileRenameRequest)
        {
            return await RenameFile(fileRenameRequest);
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

        async Task<FileResponse?> IFileService.GetFileDetails(Guid? fileId)
        {
            return await GetFileDetails(fileId);
        }

        public async Task<FileResponse> MoveToFolder(FileToFolderRequest moveToFolderRequest)
        {
            if (moveToFolderRequest == null)
            {
                throw new ArgumentNullException(nameof(moveToFolderRequest), "FileToFolderRequest cannot be null.");
            }
            ValidationHelper.ModelValidation(moveToFolderRequest);
            FileDetails? fileDetails = await _fileRepository.GetFileDetails(moveToFolderRequest.Id);
            if (fileDetails == null)
            {
                throw new KeyNotFoundException($"File with ID {moveToFolderRequest.Id} not found.");
            }
            else if (fileDetails.VirualFolderId == moveToFolderRequest.VirualFolderId)
            {
                throw new InvalidOperationException("File is already in the specified folder.");
            }
            if (await _fileRepository.Contains(f => f.FileName == fileDetails.FileName && f.VirualFolderId == moveToFolderRequest.VirualFolderId))
            {
                for (int i = 0; i < 100; i++)
                {
                    string newFileName = $"{fileDetails.FileName.Split('.')[0]} ({i}).{fileDetails.FileName.Split('.')[1]}";
                    if (!await _fileRepository.Contains(f => f.FileName == newFileName && f.VirualFolderId == moveToFolderRequest.VirualFolderId))
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
            await _fileRepository.UpdateFile(fileDetails);
            return fileDetails.ToFileResponse();
        }
    }
}
