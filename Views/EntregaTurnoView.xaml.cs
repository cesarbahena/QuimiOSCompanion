using System.Windows.Controls;
using QuimiOSCompanion.ViewModels;

namespace QuimiOSCompanion.Views
{
    public partial class EntregaTurnoView : UserControl
    {
        public EntregaTurnoView()
        {
            InitializeComponent();
            DataContext = new EntregaTurnoViewModel();
        }
    }
}
