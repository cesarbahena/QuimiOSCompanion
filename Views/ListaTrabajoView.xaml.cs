using System.Windows.Controls;
using QuimiOSCompanion.ViewModels;

namespace QuimiOSCompanion.Views
{
    public partial class ListaTrabajoView : UserControl
    {
        public ListaTrabajoView()
        {
            InitializeComponent();
            DataContext = new ListaTrabajoViewModel();
        }
    }
}
