using Entities;
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
        List<FileDetails> _fileDetailsList = new List<FileDetails>();
        public FileDetailsService()
        {
            _fileDetailsList = ReadXmlFile();
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

        bool DeleteFile(Guid? fileId)
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

            _fileDetailsList.RemoveAll(f => f.Id == fileId);
            WriteXmlFile(_fileDetailsList);

            return true;
        }

        FileResponse? GetFileDetails(Guid? fileId)
        {
            if (fileId == null || fileId == Guid.Empty)
            {
                throw new ArgumentException("File ID cannot be empty or null.", nameof(fileId));
            }
            FileDetails? fileDetails = _fileDetailsList.Where(f => f.Id == fileId).FirstOrDefault();
            return fileDetails?.ToFileResponse() ?? null;
        }

        List<FileResponse> ListFilesAsync()
        {
            _fileDetailsList = ReadXmlFile();
            List<FileResponse> fileResponses = _fileDetailsList.Select(f => f.ToFileResponse()).ToList();
            return fileResponses;
        }

        FileResponse RenameFile(FileRenameRequest fileRenameRequest)
        {
            if (fileRenameRequest == null)
            {
                throw new ArgumentNullException(nameof(fileRenameRequest), "FileRenameRequest cannot be null.");
            }
            ValidationHelper.ModelValidation(fileRenameRequest);
            FileDetails? fileDetails = _fileDetailsList.Where(f => f.Id == fileRenameRequest.Id).FirstOrDefault();
            if (fileDetails == null)
            {
                throw new KeyNotFoundException($"File with ID {fileRenameRequest.Id} not found.");
            }
            if (_fileDetailsList.Any(f => f.FileName == fileRenameRequest.NewFileName && f.VirtualFolder == fileRenameRequest.VirtualFolder))
            {
                for (int i = 0; i < 100; i++)
                {
                    string newFileName = $"{fileRenameRequest.NewFileName.Split('.')[0]} ({i}).{fileRenameRequest.NewFileName.Split('.')[1]}";
                    if (!_fileDetailsList.Any(f => f.FileName == newFileName && f.VirtualFolder == fileRenameRequest.VirtualFolder))
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
            _fileDetailsList.RemoveAll(file => file.Id == fileDetails.Id);
            fileDetails.FileName = fileRenameRequest.NewFileName;
            fileDetails.FilePath = fileDetails.FilePath;
            _fileDetailsList.Add(fileDetails);
            WriteXmlFile(_fileDetailsList);
            return fileDetails.ToFileResponse();
        }

        FileResponse AddFile(FileAddRequest fileAddRequest)
        {
            if (fileAddRequest == null)
            {
                throw new ArgumentNullException(nameof(fileAddRequest), "FileAddRequest cannot be null.");
            }
            ValidationHelper.ModelValidation(fileAddRequest);
            FileDetails fileDetails = fileAddRequest.ToFileDetails();
            if (_fileDetailsList.Any(f => f.FileName == fileAddRequest.FileName && f.VirtualFolder == fileAddRequest.VirtualFolder))
            {
                for (int i = 0; i < 100; i++)
                {
                    string newFileName = $"{fileAddRequest.FileName.Split('.')[0]} ({i}).{fileAddRequest.FileName.Split('.')[1]}";
                    if (!_fileDetailsList.Any(f => f.FileName == newFileName && f.VirtualFolder == fileAddRequest.VirtualFolder))
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
            _fileDetailsList.Add(fileDetails);
            WriteXmlFile(_fileDetailsList);
            return fileDetails.ToFileResponse();
        }

        FileResponse IFileService.AddFile(FileAddRequest fileAddRequest)
        {
            return AddFile(fileAddRequest);
        }

        FileResponse IFileService.RenameFile(FileRenameRequest fileRenameRequest)
        {
            return RenameFile(fileRenameRequest);
        }

        Task<FileResponse> IFileService.AddTextToFileAsync(FileAddTextToFileRequest fileAddTextToFileRequest)
        {
            return AddTextToFileAsync(fileAddTextToFileRequest);
        }

        bool IFileService.DeleteFile(Guid? fileId)
        {
            return DeleteFile(fileId);
        }

        List<FileResponse> IFileService.ListFiles()
        {
            return ListFilesAsync();
        }

        FileResponse? IFileService.GetFileDetails(Guid? fileId)
        {
            return GetFileDetails(fileId);
        }
    }
}
