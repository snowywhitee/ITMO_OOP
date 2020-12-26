using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath1 = @"C:\Users\нр\Desktop\save\test1.txt";
            string filePath2 = @"C:\Users\нр\Desktop\save\test2.txt";
            string filePath3 = @"C:\Users\нр\Desktop\save\test3.txt";
            string filePath4 = @"C:\Users\нр\Desktop\save\picture.jpg";
            string storagePath = @"C:\Users\нр\Desktop\backups";
            string archivePath = @"C:\Users\нр\Desktop\archive.zip";

            ////Case 1
            //Backup backup = new Backup(StorageType.Separate, @"C:\Users\нр\Desktop\backups");
            //backup.AddFiles(filePath1, filePath2);
            //backup.MakePoint(BackupType.Full, StorageType.Separate, @"C:\Users\нр\Desktop\backups\backup1");
            //backup.MakePoint(BackupType.Full, StorageType.Separate, @"C:\Users\нр\Desktop\backups\backup2");
            //Console.WriteLine($"Files in the backup: {backup.Files.Count}");
            //backup.MakePoint(BackupType.Full, StorageType.Separate, @"C:\Users\нр\Desktop\backups\backup3");
            //backup.PointLimit = 1;
            //Console.WriteLine($"Points in the backup: {backup.List.Count}");

            ////Case 2
            //Backup backup = new Backup(StorageType.Separate, @"C:\Users\нр\Desktop\backups");
            //backup.AddFiles(filePath1, filePath2);
            //backup.MakePoint(BackupType.Full, StorageType.Separate, @"C:\Users\нр\Desktop\backups\backup1");
            //backup.MakePoint(BackupType.Full, StorageType.Separate, @"C:\Users\нр\Desktop\backups\backup2");
            //Console.WriteLine($"Points in the backup: {backup.List.Count}");
            //Console.WriteLine($"Backup total size: {backup.TotalSize}");
            //backup.SizeLimit = 30;
            //Console.WriteLine($"Points in the backup: {backup.List.Count}");

            ////United storagetype
            //Backup backup = new Backup(StorageType.United, archivePath);
            //backup.AddFiles(filePath1, filePath2, filePath3, filePath4);
            //backup.MakePoint(BackupType.Full, StorageType.United, archivePath);

            //Case 3
            Backup backup = new SeparateStorageBackup(@"C:\Users\нр\Desktop\backups");
            backup.AddFiles(filePath1, filePath2, filePath2, filePath3, filePath4);

            //backup.PointLimit = 2;
            ////backup.SizeLimit = 253339;
            //backup.SizeLimit = 126686;
            //backup.TimeLimit = DateTime.Now;

            backup.MakePoint(BackupType.Full, "backup1");
            ChangeText(filePath1);
            backup.MakePoint(BackupType.Incremental, "backup2");
            ChangeText(filePath2);
            backup.MakePoint(BackupType.Incremental, "backup3");
            Console.WriteLine($"Points in the backup: {backup.List.Count}");
        }
        public static void ChangeText(string path)
        {
            File.WriteAllText(path, "CHANGED!");
        }
    }
    public enum BackupType
    {
        Full,
        Incremental
    }
    public enum StorageType
    {
        Separate,
        United
    }
    public class BackupException : Exception
    {
        private string msg;
        public BackupException(string msg)
        {
            this.msg = msg;
        }
        public override string Message => msg;
    }
}

