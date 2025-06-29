using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;       // dla SQLiteConnection, SQLiteCommand
using CarRentalSystem.Models;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Utils;
using System.Windows.Forms;

namespace CarRentalSystem.Interfaces
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception ex = null);
    }
}