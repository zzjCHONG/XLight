using System.Windows.Controls;

namespace XLight.Wpf
{
    /// <summary>
    /// CeceroSpinView.xaml 的交互逻辑
    /// </summary>
    public partial class CiceroSpinView : UserControl
    {
        public CiceroSpinView()
        {
            InitializeComponent();
            this.DataContext=new CiceroSpinViewModel();
        }
    }
}
