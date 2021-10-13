using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW9_10
{
    public class User
    {
        public long UserId { get; set; }
        public string RepoPath { get; set; }
        public List<string> FolderList { get; set; }

        public User() 
        { 
        }
        public User(long userId)
        {
            UserId = userId;
            RepoPath = Path.Combine(Directory.GetCurrentDirectory(), "Repository", this.UserId.ToString());
            FolderList = GetFolderName();
        }

        private List<string> GetFolderName()
        {
            List<string> folders = new List<string>();
            var tempPath = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "Repository", this.UserId.ToString()));
            foreach (var item in tempPath)
            {
                folders.Add(new DirectoryInfo(item).Name);
            }
            return folders;
        }

        
    }
}
