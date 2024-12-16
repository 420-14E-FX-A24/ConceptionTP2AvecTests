using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Automate.Models;

namespace Automate.Interfaces
{
    public interface IWindowService
    {
        DateTime DateSelection { get; set; }
        ObservableCollection<Task> Tasks { get; set; }
		bool IsAdmin { get; set; }
        void Close();
        void Logout();

        int TemperatureReelle {  get; set; }
        int LuminositeReelle { get; set; }
        int HumiditeReelle { get; set; }

        string FileName { get; set; }
        Stream? OpenRead(string path);

        void LoadFile(string filePath);

        void ReadMeteoData();

        void AddLineToFileData(int index, string[] line);
        Dictionary<int, string[]> FileData { get; set; }
        int MeteoChangeDelay { get; set; }
        bool MeteoDataIsRead { get; set; }

        bool FileIsLoading { get; set; }

    }
}