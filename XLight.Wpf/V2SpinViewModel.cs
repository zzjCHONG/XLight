using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.IO.Ports;
using System.Timers;
using System.Windows;

//todo，当我使用对应方法实现业务逻辑的services接口，界面状态要如何实现同步？

namespace XLight.Wpf
{
    public partial class V2SpinViewModel: ObservableObject
    {
        private readonly ISpin _spin;
        private readonly System.Timers.Timer? _timerComs;
        private static string? currentPortname;

        public V2SpinViewModel()
        {
            _spin = new V2CrestSpin();

            SerialComs?.AddRange(SerialPort.GetPortNames());
            if (_timerComs == null)
            {
                _timerComs = new System.Timers.Timer(1000);
                _timerComs.Elapsed += OnTimedComsEvent!;
                _timerComs.AutoReset = true;
                _timerComs.Enabled = true;
            }
        }

        private void OnTimedComsEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                var com = SerialPort.GetPortNames();

                bool areEqual = SerialComs?.Count == com.Length
                    && !SerialComs.Except(com).Any() && !com.Except(SerialComs).Any();
                if (!areEqual)
                {
                    SerialComs = new();
                    SerialComs.AddRange(com);
                    if (SerialComs.Count != 0)
                    {
                        if (!string.IsNullOrEmpty(currentPortname) && IsConnected)
                        {
                            int index = SerialComs.IndexOf(currentPortname);
                            SerialIndex = index;
                        }
                        else
                        {
                            SerialIndex = SerialComs.Count - 1;
                        }
                    }

                    if (!SerialComs.Contains(currentPortname!) && !string.IsNullOrEmpty(currentPortname))
                        IsConnected = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnTimedComsEvent" + ex.ToString());
            }
        }

        [ObservableProperty]
        private List<string>? _serialComs = new();

        [ObservableProperty]
        public int _serialIndex = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsOperable))]
        private bool _isConnected = false;

        public bool IsOperable => !IsConnected;

        [RelayCommand]
        async Task Init()
        {
            await Task.Run(() =>
            {
                IsConnected = _spin.Init();
            });

            if (IsConnected)
            {
                Application.Current?.Dispatcher.Invoke(async () =>
                {
                    var ver = await _spin.Version;
                    MessageBox.Show(Application.Current.MainWindow, $"转盘{ver}\r\n连接成功！", "连接提示", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            else
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(Application.Current.MainWindow, $"转盘连接失败！", "连接提示", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        [RelayCommand]
        async Task InitManual()
        {
            var com = SerialComs![SerialIndex];
            await Task.Run(() =>
            {
                IsConnected = _spin.Init(com);
            });

            if (IsConnected)
            {
                currentPortname = com;
                Application.Current?.Dispatcher.Invoke(async () =>
                {
                    var ver = await _spin.Version;
                    MessageBox.Show(Application.Current.MainWindow, $"转盘{ver}\r\n连接{com}成功！", "连接提示", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            else
            {
                currentPortname = string.Empty;
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(Application.Current.MainWindow, $"转盘连接{com}失败！", "连接提示", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }
    }

    partial class V2SpinViewModel
    {
        [ObservableProperty]
        private bool _isEmissionEnable = true;

        public List<string> EmissionCollection => new() { "1", "2", "3", "4", "5", "6", "7", "8" };

        [ObservableProperty]
        private uint _emissionIndex = 0;

        async partial void OnEmissionIndexChanged(uint value)
        {
            if (isHoming) return;

            try
            {
                IsEmissionEnable = false;
                var pos = value + 1;
                if (!await _spin.SetEmission(pos))
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show(Application.Current.MainWindow, $"Emission切换失败！",
                            "切换提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsEmissionEnable = true;
            }

        }

        [ObservableProperty]
        private bool _isSpiningEnable=true;

        public List<string> SpiningCollection => new() { "0", "1", "2" };

        [ObservableProperty]
        private uint _spiningIndex = 0;

        async partial void OnSpiningIndexChanged(uint value)
        {
            if (isHoming) return;
            try
            {
                IsSpiningEnable = false;
                if (!await _spin.SetSpining(value))
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, $"SetSpining切换失败！",
                            "切换提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsSpiningEnable=true;
            }

        }

        [ObservableProperty]
        private bool _isDichroicEnable = true;

        public List<string> DichroicCollection => new(){"1","2","3","4","5"};

        [ObservableProperty]
        private uint _dichroicIndex = 0;

        async partial void OnDichroicIndexChanged(uint value)
        {
            if (isHoming) return;
            try
            {
                IsDichroicEnable = false;
                var pos = value + 1;
                if (!await _spin.SetDichroic(pos))
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, $"SetDichroic切换失败！",
                            "切换提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                IsDichroicEnable = true;
            }

        }

        [ObservableProperty]
        private bool _isExcitationEnable = true;

        public List<string> ExcitationCollection => new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8" };

        [ObservableProperty]
        private uint _excitationIndex = 0;

        async partial void OnExcitationIndexChanged(uint value)
        {
            if (isHoming) return;
            try
            {
                IsExcitationEnable = false;
                var pos = value + 1;
                if (!await _spin.SetExcitation(pos))
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(Application.Current.MainWindow, $"SetExcitation切换失败！",
                            "切换提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                IsExcitationEnable = true;
            }
        }

        private bool isHoming;

        [RelayCommand]
        async Task Home()
        {
            try
            {
                isHoming = true;

                DichroicIndex = 0;
                EmissionIndex = 0;
                ExcitationIndex = 0;
                SpiningIndex = 0;
                IsSetSpin = false;

                await _spin.Reset();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                isHoming = false;
            }

        }

        [ObservableProperty]
        private bool _isSetSpin;

        [ObservableProperty]
        private bool _isSetSpinEnable = true;

        [RelayCommand]
        async Task SetSpin()
        {
            try
            {
                IsSetSpinEnable = false;

                await _spin.SetDisk(IsSetSpin ? (uint)1 : 0);

                await Task.Delay(1000); //需额外增加转盘稳定延时，待测试确认
               
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                IsSetSpinEnable = true;
            }


        }
    }
}
