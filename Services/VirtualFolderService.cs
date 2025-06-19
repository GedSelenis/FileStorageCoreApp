using Entities;
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
    public class VirtualFolderService : IVirtualFolderService
    {
        List<VirtualFolder> _virtualFolderList = new List<VirtualFolder>();

        public VirtualFolderService()
        {
            _virtualFolderList = ReadXmlFile();
        }

        List<VirtualFolder> ReadXmlFile()
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

        void WriteXmlFile(List<VirtualFolder> fileDetails)
        {
            StreamWriter file1 = new StreamWriter("StoredFolders.xml");
            XmlSerializer x = new XmlSerializer(fileDetails.GetType());
            x.Serialize(file1, fileDetails);
            file1.Close();
        }
        public async Task<FolderResponse> AddFolder(FolderAddRequest folderAddRequest)
        {
            if (folderAddRequest == null)
            {
                throw new ArgumentException("Folder name cannot be null.");
            }
            ValidationHelper.ModelValidation(folderAddRequest);
            if (_virtualFolderList.Any(x => x.FolderName == folderAddRequest.FolderName))
            {
                throw new ArgumentException("Folder with the same name already exists.");
            }

            VirtualFolder virtualFolder = folderAddRequest.ToVirtualFolder();
            virtualFolder.Id = Guid.NewGuid();
            _virtualFolderList.Add(virtualFolder);
            WriteXmlFile(_virtualFolderList);

            return virtualFolder.ToFolderResponse();
        }

        public async Task<bool> DeleteFolder(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Folder ID cannot be empty.");
            }
            VirtualFolder folderToDelete = _virtualFolderList.FirstOrDefault(x => x.Id == id);
            if (folderToDelete == null)
            {
                return false;
            }
            _virtualFolderList.RemoveAll(f => f.Id == id);
            WriteXmlFile(_virtualFolderList);
            return true;

        }

        public async Task<List<FolderResponse>> GetAllFolders()
        {
            return _virtualFolderList.Select(folder => folder.ToFolderResponse()).ToList();
        }

        public async Task<FolderResponse> GetFolderById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Folder ID cannot be empty.");
            }
            VirtualFolder? folder = _virtualFolderList.FirstOrDefault(x => x.Id == id);
            if (folder == null)
            {
                throw new KeyNotFoundException("Folder not found.");
            }
            return folder.ToFolderResponse();
        }

        public async Task<FolderResponse> UpdateFolder(FolderUpdateRequest folderUpdateRequest)
        {
            if (folderUpdateRequest == null)
            {
                throw new ArgumentException("Invalid folder update request.");
            }
            ValidationHelper.ModelValidation(folderUpdateRequest);
            VirtualFolder? folderToUpdate = _virtualFolderList.FirstOrDefault(x => x.Id == folderUpdateRequest.Id);
            if (folderToUpdate == null)
            {
                throw new KeyNotFoundException("Folder not found.");
            }
            if (_virtualFolderList.Any(x => x.FolderName == folderUpdateRequest.FolderName))
            {
                throw new ArgumentException("Folder with the same name already exists.");
            }
            folderToUpdate.FolderName = folderUpdateRequest.FolderName;
            WriteXmlFile(_virtualFolderList);

            return folderToUpdate.ToFolderResponse();
        }
    }
}
