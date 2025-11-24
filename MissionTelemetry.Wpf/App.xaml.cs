using System.Configuration;
using System.Data;
using System.Windows;
using MissionTelemetry.Core.Services;
using MissionTelemetry.Core.ViewModels;

namespace MissionTelemetry.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Core-Services instanzieren (DI-light)
        var hk = new SimulatedTelemetrySource(1.0);
        var eval = new AlarmEvaluator();
        var prox = new SimulatedProximitySource(1.0);
        var mgr = new AlarmManager();

        var vm = new TelemetryViewModel(hk, eval, prox, mgr);

        var win = new MainWindow { DataContext = vm };
        win.Show();
    }

}

