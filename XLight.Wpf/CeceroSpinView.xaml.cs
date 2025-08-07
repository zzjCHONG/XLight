using System.Windows.Controls;

namespace XLight.Wpf
{
    /// <summary>
    /// CeceroSpinView.xaml 的交互逻辑
    /// </summary>
    public partial class CeceroSpinView : UserControl
    {
        public CeceroSpinView()
        {
            InitializeComponent();
            this.DataContext=new CeceroSpinViewModel();
        }
    }
}
