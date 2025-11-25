using System;
using System.IO;
using System.Windows;
using MissionTelemetry.Core.Services;  // JsonDictionaryLoader, DataDrivenAlarmEvaluator
// using MissionTelemetry.Core.Models; // falls du Model-Typen brauchst

namespace MissionTelemetry.Wpf
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            // mission_dict.json aus dem Ausgabeverzeichnis laden
            var path = Path.Combine(AppContext.BaseDirectory, "mission_dict.json");
            var dict = new JsonDictionaryLoader().LoadFromFile(path);

            // Data-driven Evaluator erzeugen (ersetzt: new AlarmEvaluator())
            IAlarmEvaluator evaluator = new DataDrivenAlarmEvaluator(dict);

            // TODO: Wenn dein MainWindow ein ViewModel braucht, hier setzen:
            var window = new MainWindow();
            // Beispiel:
            // window.DataContext = new TelemetryViewModel(evaluator, /* weitere deps */);

            window.Show();
        }
    }
}


