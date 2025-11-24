using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MissionTelemetry.Core.Infrastructure
{
    public abstract class ObservableObject : INotifyPropertyChanged // Interface: meldet der UI, wenn Properties sich ändert
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null )
        {
            if (Equals(field, value)) return false;
            field = value;                                                        // CMN füllt automatisch den Propertynamen
           
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
