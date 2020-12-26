using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Backup
{
    public class BackupPoint
    {
        //General Info
        public int Id { get; private set; }
        private static int idCount = 0;
        public BackupType Type { get; protected set; }
        public StorageType StorageType { get; protected set; }
        public DateTime Time { get; protected set; }
        public long Size { get; protected set; }
        public string StoragePath { get; protected set; }

        //Members
        private Dictionary<string, FileInfo> files = new Dictionary<string, FileInfo>();
        public Dictionary<string, FileInfo> Files { get => files; }

        //Methods
        public BackupPoint(BackupType type, StorageType storageType, string storagePath)
        {
            Id = idCount;
            idCount++;
            Type = type;
            StorageType = storageType;
            Time = DateTime.Now;
            StoragePath = storagePath;
        }
        public virtual void AddFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new BackupException($"Invalid filePath: {filePath}");
            }
            if (files.ContainsKey(filePath))
            {
                return;
            }
            if (StorageType == StorageType.Separate)
            {
                AddFileToDir(filePath);
            }
            else if (StorageType == StorageType.United)
            {
                AddFileToZip(filePath);
            }
        }
        public virtual void Clear()
        {
            Size = 0;
            if (StorageType == StorageType.Separate)
            {
                DirectoryInfo directory = new DirectoryInfo(StoragePath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
            }
            else if (StorageType == StorageType.United)
            {
                using (var zipArchive = ZipFile.Open(StoragePath, ZipArchiveMode.Update))
                {
                    var entries = zipArchive.Entries;
                    foreach (var item in entries)
                    {
                        item.Delete();
                    }
                }
            }
            
        }

        //Private methods
        private void AddFileToZip(string filePath)
        {
            using (var zipArchive = ZipFile.Open(StoragePath, ZipArchiveMode.Update))
            {
                var fileInfo = new FileInfo(filePath);
                zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                Size += fileInfo.Length;
            }
        }
        private void AddFileToDir(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            FileInfo DestinationFileInfo = new FileInfo(Path.Combine(StoragePath, fileInfo.Name));
            File.Copy(fileInfo.FullName, DestinationFileInfo.FullName, true);
            files.Add(filePath, DestinationFileInfo);
            Size += DestinationFileInfo.Length;
        }

        public virtual DateTime GetLastChangeTime(string filePath)
        {
            if (files.ContainsKey(filePath))
            {
                return Time;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
    public class IncrementalPoint : BackupPoint
    {
        protected Dictionary<string, DateTime> lastChangeTime = new Dictionary<string, DateTime>();
        public BackupPoint Parent { get; private set; }
        public IncrementalPoint(BackupType type, StorageType storageType, string storagePath, BackupPoint parent) : base(type, storageType, storagePath)
        {
            Parent = parent;
        }
        public override DateTime GetLastChangeTime(string filePath)
        {
            if (lastChangeTime.ContainsKey(filePath))
            {
                return lastChangeTime[filePath];
            }
            else
            {
                return Parent.GetLastChangeTime(filePath);
            }
        }
        public override void AddFile(string filePath)
        {
            base.AddFile(filePath);
            if (lastChangeTime.ContainsKey(filePath))
            {
                lastChangeTime[filePath] = Time;
            }
            else
            {
                lastChangeTime.Add(filePath, Time);
            }
        }
        public override void Clear()
        {
            base.Clear();
            lastChangeTime.Clear();
        }
    }
}

