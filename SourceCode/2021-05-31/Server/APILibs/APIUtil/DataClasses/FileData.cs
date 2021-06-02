

using System;

namespace ApiUtil.DataClasses
{
    public class FileData
    {
        public FileData()
        {

        }

        public string FileName { get; set; }
        public string FileExt { get; set; }
        public int FileSize { get; set; }
        public DateTime FileDate { get; set; }
        public string FileHash { get; set; }
    }
}
