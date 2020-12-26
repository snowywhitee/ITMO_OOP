using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Backup
{
    public class Backup
    {
        //General info
        public int Id { get; protected set; }
        private static int idCount = 0;
        public string Name { get; set; }
        public BackupType Type { get; protected set; }
        public StorageType StorageType { get; protected set; }
        public string StoragePath { get; protected set; }
        public long TotalSize { get; protected set; }

        //Points & Files
        protected List<BackupPoint> list = new List<BackupPoint>();
        protected HashSet<string> filePaths = new HashSet<string>();
        protected Dictionary<string, FileInfo> files = new Dictionary<string, FileInfo>();
        public Dictionary<string, FileInfo> Files { get => files; }
        public HashSet<string> FilePaths { get => filePaths; }
        public List<BackupPoint> List { get => list; }
        //for incremental points
        protected HashSet<int> lockedPointIds = new HashSet<int>();

        //Limits
        private bool pointLimit = false;
        private bool timeLimit = false;
        private bool sizeLimit = false;
        private int pointLimitValue;
        private DateTime timeLimitValue;
        private long sizeLimitValue;
        public int PointLimit
        {
            get { return pointLimitValue; }
            set
            {
                if (value < 0)
                {
                    throw new BackupException($"PointLimit can't be negative");
                }
                pointLimitValue = value;
                pointLimit = true;
                FitLimits();
            }
        }
        public DateTime TimeLimit
        {
            get { return timeLimitValue; }
            set
            {
                timeLimitValue = value;
                timeLimit = true;
                FitLimits();
            }
        }
        public long SizeLimit
        {
            get { return sizeLimitValue; }
            set
            {
                if (value < 0)
                {
                    throw new BackupException($"Invalid SetSizeLimit value: {value}");
                }
                sizeLimitValue = value;
                sizeLimit = true;
                FitLimits();
            }
        }

        private bool limitMode = true; //true - only if all limits passed, false - at least one
        public bool LimitMode { get => limitMode; set => limitMode = value; }

        //Methods
        public Backup(StorageType storageType, string storagePath)
        {
            Id = idCount;
            idCount++;
            StorageType = storageType;
            StoragePath = storagePath;
        }
        public void AddFiles(params string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (!File.Exists(paths[i]))
                {
                    throw new BackupException($"Invalid filePath: {paths[i]}");
                }
                filePaths.Add(paths[i]);
            }
        }
        public void RemoveFiles(params string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (filePaths.Contains(paths[i]))
                {
                    filePaths.Remove(paths[i]);
                }
            }
        }
        public virtual void MakePoint(BackupType type, string storagePath)
        {
            if (type == BackupType.Full)
            {
                FitLimits();
                BackupPoint point = new BackupPoint(BackupType.Full, StorageType, storagePath);
                foreach (string path in filePaths)
                {
                    point.AddFile(path);
                }
                CheckLimits(point);
                AddPoint(point);
            }
            else if (type == BackupType.Incremental)
            {
                if (list.Count == 0)
                {
                    throw new BackupException($"New incremental point can't be initialized - there is no parents to gain changes from");
                }
                FitLimits();
                IncrementalPoint point = new IncrementalPoint(BackupType.Incremental, StorageType, storagePath, list.Last());
                foreach (string path in filePaths)
                {
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.LastWriteTime > point.GetLastChangeTime(path))
                    {
                        //add delta
                        point.AddFile(path);
                    }
                }
                CheckLimits(point);
                lockedPointIds.Add(point.Parent.Id);
                AddPoint(point);
            }

        }

        //Private methods
        protected void FitLimits()
        {
            int failCount = 0;
            int limitCount = 0;
            if (pointLimit)
            {
                limitCount++;
                if (list.Count > PointLimit)
                {
                    if (PointLimit == 0)
                    {
                        ClearBackup();
                    }
                    if (PointLimit == 1)
                    {
                        if (list.Last().Type != BackupType.Full)
                        {
                            failCount++;
                            //throw new BackupException($"Can't fit PointLimit, because the last backuppoint is incremental");
                        }
                    }
                    while(list.Count > PointLimit)
                    {
                        try
                        {
                            RemovePoint();
                        }
                        catch (Exception)
                        {
                            failCount++;
                            //throw new BackupException($"Can't fit the PointLimit {PointLimit}");
                        }
                    }

                }
            }
            if (timeLimit)
            {
                limitCount++;
                if (list.Count != 0)
                {
                    BackupPoint point = list[0];
                    while (point.Time < TimeLimit)
                    {
                        if (list.Count == 0)
                        {
                            break;
                        }
                        try
                        {
                            RemovePoint();
                        }
                        catch (Exception)
                        {
                            failCount++;
                        }
                        if (list.Count != 0)
                        {
                            point = list[0];
                        }
                    }
                }
            }
            if (sizeLimit)
            {
                limitCount++;
                if (TotalSize > SizeLimit)
                {
                    while (TotalSize > SizeLimit)
                    {
                        try
                        {
                            RemovePoint();
                        }
                        catch (Exception)
                        {
                            failCount++;
                            //throw new BackupException($"Can't fit the SizeLimit {SizeLimit}");
                        }
                    }
                }
            }

            if (limitMode)
            {
                if (failCount != 0)
                {
                    throw new BackupException($"Unable to fit limits, limitMode \"All\"");
                }
            }
            else
            {
                if (limitCount - failCount < 1)
                {
                    throw new BackupException($"Unable to fit limits, limitMode \"Any\"");
                }
            }
        }
        protected void CheckLimits(BackupPoint point)
        {
            int failCount = 0;
            int limitCount = 0;
            if (sizeLimit)
            {
                limitCount++;
                if (TotalSize + point.Size > SizeLimit)
                {
                    if (point.Size > SizeLimit)
                    {
                        failCount++;
                        //throw new BackupException($"Can't add a BackupPoint, SizeLimit exceeded: {SizeLimit}");
                    }
                    else
                    {
                        while (TotalSize + point.Size > SizeLimit)
                        {
                            try
                            {
                                RemovePoint();
                            }
                            catch (Exception)
                            {
                                failCount++;
                                //throw new BackupException($"Unable to create new point. Can't fit the SizeLimit {SizeLimit}");
                            }
                        }
                    }
                }
            }
            if (pointLimit)
            {
                limitCount++;
                if (list.Count + 1 > PointLimit)
                {
                    if (PointLimit == 0)
                    {
                        failCount++;
                        //throw new BackupException($"Can't add a BackupPoint, PointLimit exceeded: {PointLimit}");
                    }
                    else
                    {
                        while (list.Count + 1 > PointLimit)
                        {
                            try
                            {
                                RemovePoint();
                            }
                            catch (Exception)
                            {
                                failCount++;
                                //throw new BackupException($"Unable to create new point. Can't fit the PointLimit {PointLimit}");
                            }
                        }
                    }
                }
            }
            if (timeLimit)
            {
                if (point.Time < TimeLimit)
                {
                    failCount++;
                    //throw new BackupException($"Unable to create new point. Can't add a BackupPoint, TimeLimit exceeded: {TimeLimit}");
                }
            }
            if (limitMode)
            {
                if (failCount != 0)
                {
                    throw new BackupException($"Unable to fit limits, limitMode \"All\"");
                }
            }
            else
            {
                if (limitCount - failCount < 1)
                {
                    throw new BackupException($"Unable to fit limits, limitMode \"Any\"");
                }
            }
        }
        private void ClearBackup()
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Clear();
            }
            TotalSize = 0;
            list.Clear();
            lockedPointIds.Clear();
            files.Clear();
        }
        private void UpdateFiles()
        {
            files.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                foreach (var item in list[i].Files)
                {
                    if (! files.ContainsKey(item.Key))
                    {
                        files.Add(item.Key, item.Value);
                    }
                    
                }
            }
        }
        private void RemovePoint()
        {
            //Check removing
            if (list.Count != 0)
            {
                if (lockedPointIds.Contains(list[0].Id))
                {
                    throw new Exception($"Unable to remove BackupPoint with id: {list[0].Id} because there are IncrementalPoints derived from it");
                }
                TotalSize -= list[0].Size;
                if (lockedPointIds.Contains(list[0].Id))
                {
                    lockedPointIds.Remove(list[0].Id);
                }
                list[0].Clear();
                list.RemoveAt(0);
                UpdateFiles();
            }
        }
        protected void AddPoint(BackupPoint point)
        {
            TotalSize += point.Size;
            list.Add(point);
            foreach (var item in point.Files)
            {
                if (! files.ContainsKey(item.Key))
                {
                    files.Add(item.Key, item.Value);
                    //lastChangeTime[item.Key] = point.Time;
                }
                else
                {
                    //lastChangeTime[item.Key] = point.Time;
                }
            }
        }
    }

    public class SeparateStorageBackup : Backup
    {
        public SeparateStorageBackup(string storagePath) : base(StorageType.Separate, storagePath)
        {
            
        }
        public override void MakePoint(BackupType type, string dirName)
        {
            DirectoryInfo directory = new DirectoryInfo(StoragePath);
            if (! Directory.Exists(Path.Combine(StoragePath, dirName)))
            {
                directory = Directory.CreateDirectory(Path.Combine(StoragePath, dirName));
            }
            else
            {
                directory = new DirectoryInfo(Path.Combine(StoragePath, dirName));
            }

            base.MakePoint(type, directory.FullName);
        }
    }
    public class UnitedStorageBackup : Backup
    {
        public UnitedStorageBackup(string storagePath) : base(StorageType.United, storagePath)
        {

        }

        public override void MakePoint(BackupType type, string storagePath)
        {
            if (! IsZipValid(storagePath))
            {
                throw new BackupException($"Invalid zip file path: {storagePath}");
            }

            base.MakePoint(type, storagePath);
        }
        private bool IsZipValid(string path)
        {
            try
            {
                using (var zipFile = ZipFile.OpenRead(path))
                {
                    var entries = zipFile.Entries;
                    return true;
                }
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }
    }
}

