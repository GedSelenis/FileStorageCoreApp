using Entities;
using Microsoft.EntityFrameworkCore;
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
    public class VirtualFolderService : IVirtualFolderService
    {
        IFolderRepository _folderRepository;
        //List<VirtualFolder> _virtualFolderList = new List<VirtualFolder>();

        public VirtualFolderService(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
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
            if (await _folderRepository.ContainsName(folderAddRequest.FolderName))
            {
                throw new ArgumentException("Folder with the same name already exists.");
            }

            VirtualFolder virtualFolder = folderAddRequest.ToVirtualFolder();
            virtualFolder.Id = Guid.NewGuid();
            await _folderRepository.AddFolder(virtualFolder);

            return virtualFolder.ToFolderResponse();
        }

        public async Task<bool> DeleteFolder(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Folder ID cannot be empty.");
            }
            VirtualFolder? folderToDelete = await _folderRepository.GetFolder(id);
            if (folderToDelete == null)
            {
                return false;
            }
            return await _folderRepository.DeleteFolder(id);
        }

        public async Task<List<FolderResponse>> GetAllFolders()
        {
            var folders = await _folderRepository.ListFolders();
            List<FolderResponse> virtualFolderResponses = folders.Select(folder => folder.ToFolderResponse()).ToList();

            for (int i = 0; i < virtualFolderResponses.Count; i++)
            {
                string virtualFoderPath = "";
                virtualFoderPath = virtualFolderResponses[i]?.FolderName ?? "";
                if (virtualFolderResponses[i] != null && virtualFolderResponses[i].ParentFolderId != null)
                {
                    VirtualFolder? parentFolder = await _folderRepository.GetFolder(virtualFolderResponses[i].ParentFolderId ?? Guid.Empty);
                    while (parentFolder != null)
                    {
                        virtualFoderPath = Path.Combine(parentFolder.FolderName, virtualFoderPath);
                        parentFolder = await _folderRepository.GetFolder(parentFolder.ParentFolderId ?? Guid.Empty);
                    }
                }

                virtualFolderResponses[i].VirtualPath = virtualFoderPath;
            }

            return virtualFolderResponses;
        }

        public async Task<FolderResponse> GetFolderById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Folder ID cannot be empty.");
            }
            VirtualFolder? folder = await _folderRepository.GetFolder(id);
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
            VirtualFolder? folderToUpdate = await _folderRepository.GetFolder(folderUpdateRequest.Id);
            if (folderToUpdate == null)
            {
                throw new KeyNotFoundException("Folder not found.");
            }
            if (await _folderRepository.ContainsName(folderUpdateRequest.FolderName))
            {
                throw new ArgumentException("Folder with the same name already exists.");
            }
            folderToUpdate.FolderName = folderUpdateRequest.FolderName;
            await _folderRepository.UpdateFolder(folderToUpdate);

            return folderToUpdate.ToFolderResponse();
        }

        public async Task<FolderResponse> MoveToFolder(FolderToFolderRequest folderToFolderRequest)
        {
            if (folderToFolderRequest == null)
            {
                throw new ArgumentException("Invalid folder move request.");
            }
            ValidationHelper.ModelValidation(folderToFolderRequest);
            VirtualFolder? sourceFolder = await _folderRepository.GetFolder(folderToFolderRequest.Id);
            VirtualFolder? destinationFolder = await _folderRepository.GetFolder(folderToFolderRequest.ParentFolderId ?? Guid.Empty);
            if (sourceFolder == null)
            {
                throw new KeyNotFoundException("Source or destination folder not found.");
            }
            else if (destinationFolder != null && sourceFolder.Id == destinationFolder.Id)
            {
                throw new ArgumentException("Source and destination folders cannot be the same.");
            }

            // Check if any of the children are the same as the source folder
            while (destinationFolder != null && destinationFolder.ParentFolderId != Guid.Empty)
            {
                if (destinationFolder.ParentFolderId == sourceFolder.Id)
                {
                    throw new ArgumentException("Cannot move folder into its own subfolder.");
                }
                destinationFolder = await _folderRepository.GetFolder(destinationFolder.ParentFolderId ?? Guid.Empty);
            }

            sourceFolder.ParentFolderId = folderToFolderRequest.ParentFolderId;
            await _folderRepository.UpdateFolder(sourceFolder);
            return sourceFolder.ToFolderResponse();
        }
    }
}
