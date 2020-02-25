using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Alantoo.AvaloniaDeadlockRepro
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}