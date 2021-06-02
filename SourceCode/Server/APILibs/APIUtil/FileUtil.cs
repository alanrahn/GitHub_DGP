﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ApiUtil.DataClasses;

namespace ApiUtil
{
    public class FileUtil
    {
        int segmentSize = 45000;

        public FileUtil()
        {

        }

        public int GetFileSegCount(long fileSize, int segmentSize)
        {
            long segments = fileSize / segmentSize;
            if (fileSize % segmentSize != 0)
            {
                segments++;
            }

            return (int)segments;
        }

        /// <summary>
        /// builds a directory structure based on a file date value
        /// </summary>
        public string CreateFilePath(string basePath, DateTime fileDate)
        {
            string dirPath =  basePath;

            dirPath += "\\" + fileDate.Year.ToString();
            dirPath += "\\" + fileDate.Month.ToString();
            dirPath += "\\" + fileDate.Day.ToString();
            dirPath += "\\" + fileDate.Hour.ToString();
            dirPath += "\\" + fileDate.Minute.ToString();
            dirPath += "\\" + fileDate.Second.ToString();
            //dirPath += "\\" + fileDate.Millisecond.ToString();

            CheckDirPath(dirPath);

            return dirPath;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckDirPath(string dirPath)
        {
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
        }

        /// <summary>
        /// 
        /// </summary>
        public FileData GetFileData(string fullPath)
        {
            FileData fileData = new FileData();

            if (File.Exists(fullPath))
            {
                FileInfo fi = new FileInfo(fullPath);
                fileData.FileName = Path.GetFileNameWithoutExtension(fi.Name);
                fileData.FileExt = fi.Extension;
                fileData.FileSize = Convert.ToInt32(fi.Length);
                fileData.FileDate = File.GetLastWriteTime(fullPath);

                using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    HashAlgorithm hashalg = HashAlgorithm.Create();
                    byte[] hashbytes = hashalg.ComputeHash(fs);
                    fileData.FileHash = Convert.ToBase64String(hashbytes);
                }
            }

            return fileData;
        }

        /// <summary>
        /// used to read a segment of a local file for file upload or download API methods
        /// </summary>
        public byte[] ReadFileSegment(string fullPath, long offset)
        {
            byte[] segment = new byte[segmentSize];
            int bytesread = 0;

            if (File.Exists(fullPath) && offset >= 0)
            {
                using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.Seek(offset, SeekOrigin.Begin);
                    segment = new byte[segmentSize];
                    bytesread = fs.Read(segment, 0, segmentSize);
                }

                // a segment smaller than the max segment size must be trimmed
                if (bytesread < segmentSize)
                {
                    byte[] trimmedSegment = new byte[bytesread];
                    Array.Copy(segment, trimmedSegment, bytesread);
                    segment = trimmedSegment;
                }
            }

            return segment;
        }

        /// <summary>
        /// used to append a file segment to a local temporary file for file upload or download API methods
        /// </summary>
        public bool AppendFileSegment(string fullPath, byte[] segment)
        {
            bool appended = false;

            using (FileStream fs = new FileStream(fullPath, FileMode.Append))
            {
                fs.Write(segment, 0, segment.Length);
                appended = true;
            }

            return appended;
        }

        /// <summary>
        /// encKey must be 32 bytes
        /// </summary>
        public bool EncryptFileCopy(string originFilePath, string encKey, string encFilePath)
        {
            bool stored = false;

            // copy and encrypt local file into temp file directory
            if (File.Exists(originFilePath))
            {
                if (encKey.Length == 32)
                {
                    string salt = encKey.Substring(0, 16);
                    Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(encKey, Encoding.Unicode.GetBytes(salt));

                    using (RijndaelManaged rm = new RijndaelManaged())
                    {
                        rm.KeySize = 256;
                        rm.BlockSize = 128;
                        rm.Key = rfc.GetBytes(rm.KeySize / 8);
                        rm.IV = rfc.GetBytes(rm.BlockSize / 8);

                        using (FileStream encryptfile = new FileStream(encFilePath, FileMode.Create))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(new FileStream(originFilePath, FileMode.Open), rm.CreateEncryptor(), CryptoStreamMode.Read))
                            {
                                cryptoStream.CopyTo(encryptfile);
                                cryptoStream.Close();
                                stored = true;
                            }
                        }
                    }
                }
            }

            return stored;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DecryptFileCopy(string encFilePath, string encKey, string decryptFilePath)
        {
            bool retrieved = false;

            // copy local temp file to storage
            if (File.Exists(encFilePath))
            {
                if (encKey.Length == 32)
                {
                    string salt = encKey.Substring(0, 16);
                    Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(encKey, Encoding.Unicode.GetBytes(salt));

                    using (RijndaelManaged rm = new RijndaelManaged())
                    {
                        rm.KeySize = 256;
                        rm.BlockSize = 128;
                        rm.Key = rfc.GetBytes(rm.KeySize / 8);
                        rm.IV = rfc.GetBytes(rm.BlockSize / 8);

                        using (FileStream decryptfile = new FileStream(decryptFilePath, FileMode.Create))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(new FileStream(encFilePath, FileMode.Open), rm.CreateDecryptor(), CryptoStreamMode.Read))
                            {
                                cryptoStream.CopyTo(decryptfile);
                                cryptoStream.Close();
                                retrieved = true;
                            }
                        }
                    }
                }
            }

            return retrieved;
        }

        /// <summary>
        /// copies encrypted file being replicated from one location to another into local storage
        /// </summary>
        public bool CopyFile(string sourcePath, string destPath)
        {
            bool stored = false;

            // write local encrypted temp file into storage
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, destPath, true);
                stored = true;
            }

            return stored;
        }

        /// <summary>
        /// compare file length and file hash from original file to uploaded file
        /// </summary>
        public bool CheckFile(string checkFilePath, string originalHash)
        {
            bool samefile = false;

            FileData fileData = GetFileData(checkFilePath);

            if (fileData.FileHash == originalHash) samefile = true;

            return samefile;
        }
    }
}
