using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        float TemperatureReelle {  get; set; }
        float LuminositeReelle { get; set; }
        float HumiditeReelle { get; set; }

        string FileName { get; set; }

        void LoadFile(string filePath);

        void ReadMeteoData();

        void AddLineToFileData(int index, string[] line);
        Dictionary<int, string[]> FileData { get; set; }
        int MeteoChangeDelay { get; set; }
        bool MeteoDataIsRead { get; set; }

    }
}