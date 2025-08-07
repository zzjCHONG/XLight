using System.Windows.Controls;

namespace XLight.Wpf
{
    /// <summary>
    /// V2SpinView.xaml 的交互逻辑
    /// </summary>
    public partial class V2SpinView : UserControl
    {
        public V2SpinView()
        {
            InitializeComponent();
            this.DataContext = new V2SpinViewModel();
        }
    }
}
