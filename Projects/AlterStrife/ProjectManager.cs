using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace StorybrewScripts
{
    public class ProjectManager : StoryboardObjectGenerator
    {
        public enum Destination
        {
            Mapset,
            AssetsLibrary
        }

        [Configurable]
        public string ExcludeDirectories = "f, font";

        [Configurable]
        public string ExcludeFiles = "";

        [Configurable]
        public Destination ExportDestination = Destination.Mapset;
 
        [Configurable]
        public bool CleanDestinationDirectory = false;

        [Configurable]
        public bool Reload = true;

        public override void Generate()
        {
            string sourcePath = ExportDestination == Destination.Mapset ? $"{ProjectPath}/assetlibrary/sb" : $"{MapsetPath}/sb";
            string destinationPath = ExportDestination == Destination.Mapset ? $"{MapsetPath}/sb" : $"{ProjectPath}/assetlibrary/sb";

            EnsureDirectory(sourcePath);
            EnsureDirectory(destinationPath);

            DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
            DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);

            if (CleanDestinationDirectory)
                DeleteDirectoriesAndFiles(destinationDirectory);

            string[] excludeDirectories = GetExcludePatterns(ExcludeDirectories);
            string[] excludeFiles = GetExcludePatterns(ExcludeFiles);
            CopyDirectoriesAndFiles(sourceDirectory, destinationDirectory, excludeDirectories, excludeFiles);
        }
        
        private void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void DeleteDirectoriesAndFiles(DirectoryInfo destinationDirectory)
        {
            foreach (DirectoryInfo directory in destinationDirectory.EnumerateDirectories())
            {
                directory.Delete(true);
            }

            foreach (FileInfo file in destinationDirectory.EnumerateFiles())
            {
                file.Delete();
            }
        }

        private void CopyDirectoriesAndFiles(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory, string[] excludeDirectories, string[] excludeFiles)
        {
            foreach (DirectoryInfo directory in sourceDirectory.EnumerateDirectories())
            {
                if (!CheckIsExclude(directory.Name, excludeDirectories))
                {
                    CopyDirectoriesAndFiles(directory, destinationDirectory.CreateSubdirectory(directory.Name),
                        excludeDirectories, excludeFiles);
                }
            }

            foreach (FileInfo file in sourceDirectory.EnumerateFiles())
            {
                if (!CheckIsExclude(file.Name, excludeFiles))
                {
                    file.CopyTo(Path.Combine(destinationDirectory.FullName, file.Name), true);
                }
            }
        }

        private bool CheckIsExclude(string name, string[] exclude)
        {
            if (exclude.Length == 1 && exclude.First() == "")
                return false;

            foreach (string pattern in exclude)
            {
                if (pattern.ToLower() == name.ToLower())
                {
                    return true;
                }
            }

            return false;
        }
        
        private string[] GetExcludePatterns(string pattern)
            => Regex.Split(pattern, @",\s*");
    }
}
