using System;

namespace FileScanner
{
    internal class IOObject
    {
        public DateTime CreationDate { get; set; }
        public bool IsDirectory { get; set; }
        public string FullPath { get; set; }
        public bool OnDisk { get; set; }
    }
}