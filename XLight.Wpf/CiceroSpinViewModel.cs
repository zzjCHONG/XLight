using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.IO.Ports;
using System.Timers;
using System.Windows;

namespace XLight.Wpf
{
    public partial class CiceroSpinViewModel : ObservableObject
    {
        private readonly ISpin _spin;
        private readonly System.Timers.Timer? _timerComs;
        private static string? currentPortname;

        public CiceroSpinViewModel()
        {
            _spin = new CiceroCrestSpin();

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
                await InitSetting();
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
                await InitSetting();
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

    partial class CiceroSpinViewModel
    {
        async Task InitSetting()
        {
            await _spin.GetAllDevicesState();

            await _spin.Reset();

            await _spin.GetAllDevicesState();
        }

        [ObservableProperty]
        private bool _isControlEnable = true;

        public static List<string> EmissionCollection => new() { "1", "2", "3", "4", "5", };

        [ObservableProperty]
        private uint _emissionIndex = 0;

        [ObservableProperty]
        private bool _isEmissionExtraction;

        async partial void OnIsEmissionExtractionChanged(bool value)
        {
            if (IsHoming) return;
            try
            {
                IsControlEnable = false;
                if (!await _spin.SetEmission(EmissionIndex + 1, IsEmissionExtraction))
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show(Application.Current.MainWindow, $"Emission-Extraction切换失败！",
                            "切换提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnIsEmissionExtractionChanged Failed:" + ex.Message);
            }
            finally
            {
                IsControlEnable = true;
            }
        }

        async partial void OnEmissionIndexChanged(uint value)
        {
            if (IsHoming) return;
            try
            {
                IsControlEnable = false;
                var pos = value + 1;
                if (!await _spin.SetEmission(pos, IsEmissionExtraction))
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        System.Windows.MessageBox.Show(Application.Current.MainWindow, $"Emission切换失败！",
                            "切换提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnEmissionIndexChanged Failed:" + ex.Message);
            }
            finally
            {
                IsControlEnable = true;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsHomingOperable))]
        private bool _isHoming;

        public bool IsHomingOperable => !IsHoming;

        [RelayCommand]
        async Task Home()
        {
            try
            {
                IsHoming = true;
                IsControlEnable = false;

                EmissionIndex = 0;
                IsSetSpin = false;
                IsEmissionExtraction = false;

                await _spin.Reset();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Home Failed:" + ex.Message);
            }
            finally
            {
                IsHoming = false;
                IsControlEnable = true;
            }
        }

        [ObservableProperty]
        private bool _isSetSpin;

        [RelayCommand]
        async Task SetSpin()
        {
            try
            {
                IsControlEnable = false;

                await _spin.SetDisk(IsSetSpin ? (uint)1 : 0);

                await Task.Delay(1000); //需额外增加转盘稳定延时，待测试确认

            }
            catch (Exception ex)
            {
                Console.WriteLine("SetSpin Failed:" + ex.Message);
            }
            finally
            {
                IsControlEnable = true;
            }
        }
    }
}
