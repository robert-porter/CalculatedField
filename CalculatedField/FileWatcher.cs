using System;
using System.IO;
using System.Security.Permissions;

namespace CalculatedField
{
    class FileWatcher
    {
        string Path;
        string Filter;

        public FileWatcher(string path, string filter)
        {
            Path = path;
            Filter = filter;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void StartWatch()
        {
            RunScript(Path + Filter);
            // Create a new FileSystemWatcher and set its properties.
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = Path;

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                // Only watch text files.
                watcher.Filter = Filter;

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;

                // Begin watching.
                watcher.EnableRaisingEvents = true;
                while (true) ;
            }
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            var watcher = (FileSystemWatcher)source;
            // Specify what is done when a file is changed, created, or deleted.
            RunScript(watcher.Path + watcher.Filter);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            var watcher = (FileSystemWatcher)source;
            // Specify what is done when a file is changed, created, or deleted.
            RunScript(watcher.Path + watcher.Filter);
        }
        static void RunScript(string path)
        {
            string script = File.ReadAllText(path);
            var engine = new Engine(script);
            if(engine.CompilerErrors.Count > 0)
            {
                foreach(var error in engine.CompilerErrors)
                {
                    Console.WriteLine(error.Message);
                }
            }
            else
            {
                var value = engine.Run(null);
                Console.WriteLine(value.Value);
            }
        }
    }
}
