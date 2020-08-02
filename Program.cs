using System;
using System.IO;
using ManagedLzma.SevenZip.FileModel;

namespace RomSorter
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Drag a folder with archived romes on this program exe to run!");
                Console.ReadKey();

                return;
            }

            string directory = args[0];
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Directory {directory} not found!");
                Console.ReadKey();
                
                return;
            }
            
            string[] filesList = Directory.GetFiles(directory);

            foreach (string fileName in filesList)
            {
                Console.WriteLine("Processing: " + fileName);
                
                if (!fileName.EndsWith(".7z")) continue;

                string ext = AnalyzeArchive(fileName);

                MoveToSubfolder(fileName, ext);
            }

            Console.WriteLine("DONE!");
            Console.ReadKey();
            
            
            void MoveToSubfolder(string originalPath, string subfolder)
            {
                string fileName = System.IO.Path.GetFileName(originalPath);
                string targetPath = $"{directory}/{subfolder}";
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }
                File.Move(originalPath, $"{targetPath}/{fileName}");
            }
        }
        
        private static string AnalyzeArchive(string archiveFileName)
        {
            if (!File.Exists(archiveFileName))
            {
                throw new FileNotFoundException("Archive not found.", archiveFileName);
            }

            using FileStream archiveStream = new FileStream(archiveFileName,
                FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
            
            ArchiveFileModelMetadataReader archiveMetadataReader = new ArchiveFileModelMetadataReader();
            ArchiveFileModel archiveFileModel = archiveMetadataReader.ReadMetadata(archiveStream);

            string romName = archiveFileModel?.RootFolder?.Items[0]?.Name;
            string res = System.IO.Path.GetExtension(romName);

            archiveStream.DisposeAsync();

            return res;
        }
    }
}