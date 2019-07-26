using System;
using System.Collections.Generic;
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
            List<Field> fields = new List<Field>();
            var fx = new Field
            {
                Name = "<<a>>",
                FieldId = Guid.NewGuid(),
                Script = null,
                Type = ScriptType.Number
            };
            var fy = new Field
            {
                Name = "<<y>>",
                FieldId = Guid.NewGuid(),
                Script = "<<a>> + 2",
                Type = ScriptType.Number
            };

            fields.Add(fx);
            fields.Add(fy);

            var dataList = new List<Dictionary<Guid, object>>();
            var data1 = new Dictionary<Guid, object>();
            data1[fx.FieldId] = 1;
            data1[fy.FieldId] = "one";
            var data2 = new Dictionary<Guid, object>();
            data2[fx.FieldId] = 2;
            data2[fy.FieldId] = "two";

            dataList.Add(data1);
            dataList.Add(data2);

            string script = File.ReadAllText(path);
            var engine = new Engine();
            //var compiled = engine.Compile(fx, fields);
            engine.Calculate(fields, dataList);
            Console.WriteLine(dataList[0][fy.FieldId]);
        }
    }
}
