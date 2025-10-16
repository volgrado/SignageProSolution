using SignagePro.Main.ViewModels;
// Al usar System.Windows.Controls, nos aseguramos de que UserControl se refiere a la versión de WPF
using System.Windows.Controls;

namespace SignagePro.Main.UI
{
    /// <summary>
    /// Interaction logic for SignagePalette.xaml
    /// </summary>
    public partial class SignagePalette : UserControl // CORRECCIÓN: La clase base es UserControl, no Window
    {
        public SignagePalette(SignageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

