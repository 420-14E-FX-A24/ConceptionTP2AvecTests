using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Automate.Interfaces
{
    public interface IFileSystem
    {
        Stream OpenRead(string path);

    }
}
