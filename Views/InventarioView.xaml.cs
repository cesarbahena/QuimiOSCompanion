using System.Windows.Controls;
using QuimiOSCompanion.ViewModels;

namespace QuimiOSCompanion.Views
{
    public partial class InventarioView : UserControl
    {
        public InventarioView()
        {
            InitializeComponent();
            DataContext = new InventarioViewModel();
        }
    }
}
