using SignagePro.Main.ViewModels;
// CORRECCIÓN DEFINITIVA: Se crea un alias para el UserControl de WPF.
// Esto elimina cualquier posible ambigüedad de forma explícita.
using WpfUserControl = System.Windows.Controls.UserControl;

namespace SignagePro.Main.UI
{
    /// <summary>
    /// Interaction logic for SignagePalette.xaml
    /// </summary>
    // Se utiliza el alias 'WpfUserControl' en lugar del nombre genérico.
    public partial class SignagePalette : WpfUserControl
    {
        public SignagePalette(SignageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

