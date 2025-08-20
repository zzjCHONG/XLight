using System.IO.Ports;
using System.Text;
using System.Threading;

namespace XLightV2
{
    public partial class XLight
    {
        private readonly SerialPort? _serialPort;
        private string? _portName;
        private readonly ManualResetEventSlim _dataReceivedEvent = new(false);
        private string _receivedDataforValid = string.Empty;
        private readonly int _validTimeout = 1500;

        public XLight()
        {
            _serialPort = new SerialPort()
            {
                BaudRate = 9600,
                StopBits = StopBits.One,
                DataBits = 8,
                Parity = Parity.None,
            };
        }

        public bool OpenCom(string com = "")
        {
            if (Valid(com))
            {
                Console.WriteLine(_portName);
                _serialPort!.NewLine = "\r";
                _serialPort!.Open();

                _serialPort.DataReceived += SerialPort_DataReceived;
                return true;
            }
            return false;
        }

        private bool Valid(string com)
        {
            try
            {
                bool isAutoMode = com == "";

                if (isAutoMode)
                {
                    string[] portNames = SerialPort.GetPortNames();
                    foreach (string portName in portNames)
                    {
                        if (!CheckPort(portName)) continue;

                        if (_serialPort!.IsOpen) _serialPort.Close();

                        _serialPort.PortName = portName;
                        _serialPort.DataReceived -= SerialPort_DataReceived_Valid;
                        _serialPort.DataReceived += SerialPort_DataReceived_Valid;

                        _dataReceivedEvent.Reset();
                        _receivedDataforValid = string.Empty;

                        _serialPort.Open();
                        _serialPort.Write("v\r"); // 发送验证命令

                        if (_dataReceivedEvent.Wait(_validTimeout))
                        {
                            Console.WriteLine("未超时:" + _receivedDataforValid);
                            if (!string.IsNullOrEmpty(_receivedDataforValid) && _receivedDataforValid.Contains("Crest"))
                            {
                                _portName = portName;
                                _serialPort.Close();
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("超时:" + _validTimeout);
                        }

                        _serialPort.Close();
                    }

                    return !string.IsNullOrEmpty(_portName);
                }
                else
                {
                    if (!CheckPort(com)) return false;

                    if (_serialPort!.IsOpen) _serialPort.Close();

                    _serialPort.PortName = com;
                    _serialPort.DataReceived -= SerialPort_DataReceived_Valid;
                    _serialPort.DataReceived += SerialPort_DataReceived_Valid;

                    _dataReceivedEvent.Reset();
                    _receivedDataforValid = string.Empty;

                    _serialPort.Open();
                    _serialPort.Write("v\r"); // 发送验证命令

                    if (_dataReceivedEvent.Wait(_validTimeout))
                    {
                        Console.WriteLine("未超时:"+_receivedDataforValid);
                        if (!string.IsNullOrEmpty(_receivedDataforValid) && _receivedDataforValid.Contains("Crest"))
                        {
                            _portName = com;
                            _serialPort.Close();
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("超时:" + _validTimeout);
                    }

                    _serialPort.Close();

                    return !string.IsNullOrEmpty(_portName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Valid_" + ex.Message);
                return false;
            }
            finally
            {
                _serialPort!.DataReceived -= SerialPort_DataReceived_Valid;
            }
        }

        private void SerialPort_DataReceived_Valid(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _serialPort!.ReadExisting();

                if (!string.IsNullOrEmpty(data))
                {
                    _receivedDataforValid += data;

                    while (_receivedDataforValid.Contains('\r'))
                    {
                        _dataReceivedEvent.Set(); // 通知有数据
                        break;
                    }         
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SerialPort_DataReceived_Valid_" + ex.Message);
            }
        }

        private static bool CheckPort(string portName)
        {
            SerialPort port = new SerialPort(portName);
            try
            {
                port.Open();
                Console.WriteLine($"串口 {portName} 未被占用");
                if (port.IsOpen) port.Close();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"串口 {portName} 已被占用");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"打开串口 {portName} 发生错误: {ex.Message}");
                return true;
            }
        }

        public bool DisConnect()
        {
            _portName = string.Empty;
            if (_serialPort!.IsOpen)
                _serialPort.Close();
            return true;
        }

        public void Dispose()
        {
            DisConnect();
            _serialPort!.DataReceived -= SerialPort_DataReceived;
            _serialPort!.Dispose();
        }

        ~XLight() => Dispose();
    }

    public partial class XLight
    {
        //大写命令：执行后会回显相同字符串
        //小写命令：查询类，会返回数据

        //正确返回：命令会原样返回（如 C2<CR>）
        //设备不存在：返回<设备ID>0<CR>   示例：rA<CR> → rA0<CR>（激发轮不存在）
        // 错误命令：返回<CR>（空响应）

        /// <summary>
        /// Excitation FW
        /// 激发滤光轮 
        /// Home位置：0
        /// 1-8
        /// </summary>
        public static uint FlagA { get; set; } = 0;

        /// <summary>
        /// Emission FW
        /// 发射滤光轮 
        /// 1 - 8 (Emission)
        /// Home位置：位置1
        /// </summary>
        public static uint FlagB { get; set; } = 0;

        /// <summary>
        /// Dichroic FW
        /// 二向色轮 
        /// 1 - 5 (Dichroic)
        /// Home位置：位置1
        /// </summary>
        public static uint FlagC { get; set; } = 0;

        /// <summary>
        /// Spinning Disk position
        /// 共聚焦盘位置
        /// 0 - 2 (Spining)
        /// 0=盘移出，1=pos1μm，2=pos2μm
        /// Home位置：0
        /// </summary>
        public static uint FlagD { get; set; } = 0;

        /// <summary>
        /// Confocal Disk Motor
        /// 共焦盘电机
        /// 0 - 1 (Disk)
        /// 0=停转，1=转动
        /// Home位置：0
        /// </summary>
        public static uint FlagN { get; set; } = 0;

        /// <summary>
        /// Read Firmware Version
        /// 查询固件版本
        /// Examples v<CR> → Ver. 6.0.0<CR>
        /// </summary>
        public static uint Flagv { get; set; } = 0;

        /// <summary>
        /// Rehome All Devices
        /// 回零
        /// H<CR> → HB1C1D0N0<CR>
        /// </summary>
        public static uint FlagH { get; set; } = 0;

        /// <summary>
        /// Lockout Touch-pad for Computer Control
        /// Valid for X-Light V1 series only. Firmware version 1.x.x - 2.x.x
        /// </summary>
        public static uint FlagM { get; set; } = 0;

        /// <summary>
        /// Query State of All Devices
        /// 查询全部状态
        /// q<CR> → B1C1D0N0A4
        /// </summary>
        public static uint Flagq { get; set; } = 0;

        /// <summary>
        /// Read Current Position of Individual Device
        /// 查询单个设备位置
        /// rB<CR> → rB2<CR> Emission wheel position 2
        /// </summary>
        public static uint Flagr { get; set; } = 0;

        //批量移动（Macro）
        //可同时控制多个设备
        //B2C3<CR> → B2C3<CR>

        //m，维护位置。？m 位置（维护位置）只有固件 ≥ v2.5 才支持

        /// <summary>
        /// 多位置设置（ABC，DN不支持）
        /// </summary>
        /// <param name="excitationPos">A</param>
        /// <param name="emissionPos">B</param>
        /// <param name="dichroicPos">C</param>
        /// <returns></returns>
        public async Task<bool> SetFilterPositions(uint? excitationPos = null, uint? emissionPos = null, uint? dichroicPos = null)
        {
            try
            {
                // 构建命令
                var commandBuilder = new StringBuilder();

                if (excitationPos.HasValue)
                {
                    uint pos = excitationPos.Value is >= 1 and <= 8 ? excitationPos.Value : 1;
                    commandBuilder.Append($"A{pos}");
                }

                if (emissionPos.HasValue)
                {
                    uint pos = emissionPos.Value is >= 1 and <= 8 ? emissionPos.Value : 1;
                    commandBuilder.Append($"B{pos}");
                }

                if (dichroicPos.HasValue)
                {
                    uint pos = dichroicPos.Value is >= 1 and <= 5 ? dichroicPos.Value : 1;
                    commandBuilder.Append($"C{pos}");
                }

                // 没有要设置的内容
                if (commandBuilder.Length == 0)
                {
                    Console.WriteLine("[ERR] 未指定任何设备位置");
                    return false;
                }

                var command = commandBuilder.ToString();
                var (ok, resp) = await SendCommandAsync(command, 10000);

                if (ok)
                {
                    if (!CheckReturnMsg(command, resp)) return false;

                    Console.WriteLine($"[OK] 设置成功: {command}");
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERR] SetFilterPositions Failed: " + e.Message);
                return false;
            }
        }

        public async Task<bool> SetSpining(uint value)
        {
            try
            {
                FlagD = value is >= 0 and <= 2 ? value : 0;
                var command = $"D{FlagD}";
                var (ok, resp) = await SendCommandAsync(command, 10000);
                if (ok)
                {
                    if (!CheckReturnMsg(command, resp)) return false;

                    Console.WriteLine("[XXX] Spining Write Success");
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[XXX] Spining Write Failed:" + e.Message);
                return false;
            }
        }

        public async Task<bool> SetDisk(uint value)
        {
            try
            {
                FlagN = value <= 1 ? value : 0;
                var command = $"N{FlagN}";
                var (ok, resp) = await SendCommandAsync(command, 10000);
                if (ok)
                {
                    if (!CheckReturnMsg(command, resp)) return false;

                    Console.WriteLine("[XXX] SetDisk Success");
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[XXX] SetDisk Failed:" + e.Message);
                return false;
            }
        }

        public async Task<bool> SetDichroic(uint value, bool isExtraction = false)
        {
            try
            {
                var isExtractionMode = isExtraction ? "m" : "";
                FlagC = value is >= 1 and <= 5 ? value : 1;
                var command = $"C{FlagC}{isExtractionMode}";
                var (ok, resp) = await SendCommandAsync(command, 10000);
                if (ok)
                {
                    if (!CheckReturnMsg(command, resp)) return false;

                    Console.WriteLine("[XXX] SetDichroic Success");
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[XXX] SetDichroic Failed:" + e.Message);
                return false;
            }
        }

        public async Task<bool> SetEmission(uint value, bool isExtraction = false)
        {
            try
            {
                var isExtractionMode = isExtraction ? "m" : "";
                FlagB = value is >= 1 and <= 8 ? value : 1;
                var command = $"B{FlagB}{isExtractionMode}";
                var (ok, resp) = await SendCommandAsync(command, 10000);
                if (ok)
                {
                    if (!CheckReturnMsg(command, resp)) return false;

                    Console.WriteLine("[XXX] SetEmission Success");
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[XXX] SetEmission Failed:" + e.Message);
                return false;
            }
        }

        public async Task<bool> SetExcitation(uint value,bool isExtraction=false)
        {
            try
            {
                var isExtractionMode = isExtraction ? "m" : "";
                FlagA = value is >= 1 and <= 8 ? value : 1;
                var command = $"A{FlagA}{isExtractionMode}";
                var (ok, resp) = await SendCommandAsync(command, 10000);
                if (ok)
                {
                    if (!CheckReturnMsg(command, resp)) return false;

                    Console.WriteLine("[XXX] SetExcitation Success");

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[XXX] SetExcitation Failed:" + e.Message);
                return false;
            }
        }

        public async Task<bool> Home()
        {
            try
            {
                var command = "H";
                var (ok, resp) = await SendCommandAsync(command,15000);
                if (ok)
                {
                    Console.WriteLine("[XXX] Home Success");
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("[XXX] Home Failed:" + e.Message);
                return false;
            }
        }

        public async Task<string> GetVersion()
        {
            try
            {
                var command = "v";
                var (ok, resp) = await SendCommandAsync(command);
                if (ok)
                {
                    Console.WriteLine($"[XXX] GetVersion {resp}");
                    return resp;
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine("[XXX] GetVersion Failed:" + e.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// 原厂返回有错误
        /// A随B位置同步，而不是实际位置
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<char, int>> GetAllDevicesState()
        {
            var states = new Dictionary<char, int>();
            try
            {
                var ares = await GetIndividualDeviceState('a');

                var command = "q";
                var (ok, resp) = await SendCommandAsync(command);
                if (!ok || string.IsNullOrWhiteSpace(resp))
                {
                    Console.WriteLine("[ERR] GetAllDevicesState 无响应");
                    return states;
                }

                // 去掉 CR LF
                resp = resp.Trim('\r', '\n', ' ');

                // 确保返回以 q 开头
                if (!resp.StartsWith("q", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[ERR] 返回格式错误: {resp}");
                    return states;
                }

                // 解析 qB1C1D0N0A4 这种格式
                // 每个设备格式固定：字母 + 数字
                for (int i = 1; i < resp.Length - 1; i += 2)
                {
                    char deviceId = resp[i];
                    if (i + 1 >= resp.Length) break;

                    if (int.TryParse(resp[i + 1].ToString(), out int pos))
                    {
                        // 校验位置合法性
                        if (IsValidPosition(deviceId, pos))
                        {
                            if (deviceId == 'A')
                            {
                                states[deviceId] = ares.Item2;
                            }
                            else
                            {
                                states[deviceId] = pos;
                            }

                        }
                        else
                        {
                            Console.WriteLine($"[WARN] 设备 {deviceId} 返回非法位置 {pos}");
                            states[deviceId] = -1; // 标记非法
                        }
                    }
                }

                Console.WriteLine($"[OK] GetAllDevicesState: {string.Join(", ", states.Select(kv => $"{kv.Key}={kv.Value}"))}");

               

                return states;
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERR] GetAllDevicesState Failed: " + e.Message);
                return states;
            }
        }

        public async Task<(bool, int)> GetIndividualDeviceState(char deviceId)
        {
            try
            {
                // 检查是否是有效设备 ID
                deviceId = char.ToUpper(deviceId);
                var validDevices = new[] {'A', 'B', 'C', 'D', 'N' };
                if (!validDevices.Contains(deviceId))
                {
                    Console.WriteLine($"[ERR] 无效的设备ID: {deviceId}");
                    return (false, -1);
                }

                // 构造命令
                var command = $"r{deviceId}";
                var (ok, resp) = await SendCommandAsync(command);
                if (!ok || string.IsNullOrWhiteSpace(resp))
                {
                    Console.WriteLine($"[ERR] 设备 {deviceId} 无响应");
                    return (false, -1);
                }

                // 去掉 CR LF
                resp = resp.Trim('\r', '\n', ' ');

                // 设备不存在 (返回 rX0)
                if (resp.Length >= 3 && resp[2] == '0')
                {
                    Console.WriteLine($"[WARN] 设备 {deviceId} 不存在");
                    return (false, 0);
                }

                // 解析位置
                if (resp.Length >= 3 && int.TryParse(resp.Substring(2), out int pos))
                {
                    // 检查是否在合法范围
                    if (IsValidPosition(deviceId, pos))
                    {
                        Console.WriteLine($"[OK] 设备 {deviceId} 位置 {pos}");
                        return (true, pos);
                    }
                    else
                    {
                        Console.WriteLine($"[ERR] 设备 {deviceId} 返回非法位置: {pos}");
                        return (false, pos);
                    }
                }

                Console.WriteLine($"[ERR] 无法解析返回: {resp}");
                return (false, -1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERR] GetIndividualDeviceState Failed: {e.Message}");
                return (false, -1);
            }
        }

        private static bool IsValidPosition(char deviceId, int position)
        {
            return deviceId switch
            {
                'A' => position >= 1 && position <= 8,  // 激发轮
                'B' => position >= 1 && position <= 8,  // 发射轮
                'C' => position >= 1 && position <= 5,  // 二向色轮
                'D' => position >= 0 && position <= 2,  // 盘位置
                'N' => position == 0 || position == 1,  // 盘电机
                _ => false
            };
        }

        private static bool CheckReturnMsg(string command, string response)
        {
            //返回空（""）→ 协议错误或超时
            //返回 <设备ID>0 → 设备不存在或执行失败
            //返回其他不匹配 → 校验失败

            if (string.IsNullOrWhiteSpace(response))
            {
                Console.WriteLine($"[ERR] 命令 {command} 返回为空");
                return false;
            }

            // 去掉 CR LF
            response = response.Trim('\r', '\n', ' ');
            var commit = command.Replace("m", "");//使用处理后的command做校验

            // 正常应该和命令一致
            if (string.Equals(commit, response, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[OK] 命令 {command} 校验通过，返回: {response}");
                return true;
            }
            else
            {
                Console.WriteLine($"[ERR] 命令 {command} 返回 {response} 与期望不一致");
                return false;
            }
        }

    }

    public partial class XLight
    {
        private TaskCompletionSource<string>? _commandTcs;
        private string _receiveBuffer = string.Empty;

        private string? _lastResponse;
        private readonly object _syncRoot = new();
        private readonly ManualResetEventSlim _waitHandle = new(false);

        private async Task<(bool, string)> SendCommandAsync(string command, int timeoutMs = 1500)
        {
            if (!_serialPort!.IsOpen)
                throw new InvalidOperationException("串口未打开");

            // 准备 TaskCompletionSource 等待返回
            _commandTcs = new TaskCompletionSource<string>();

            // 清空缓冲
            _receiveBuffer = string.Empty;

            // 发送命令
            Console.WriteLine($"[SEND] {command}");
            _serialPort.Write(command + "\r");

            // 等待返回或超时
            var completedTask = await Task.WhenAny(_commandTcs.Task, Task.Delay(timeoutMs));
            if (completedTask == _commandTcs.Task)
            {
                string response = _commandTcs.Task.Result;
                return (true, response);
            }
            else
            {
                Console.WriteLine($"[TIMEOUT] {command} 超时未收到返回");
                return (false, string.Empty);
            }
        }

        // 接收处理
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _serialPort!.ReadExisting();
                _receiveBuffer += data;

                // 协议以 \r 结束，收到完整消息时返回
                if (_receiveBuffer.Contains("\r"))
                {
                    string fullResponse = _receiveBuffer.Trim('\r', '\n');
                    _lastResponse = fullResponse;

                    _commandTcs?.TrySetResult(fullResponse);
                    _waitHandle.Set();

                    // 清空缓冲以便下次
                    _receiveBuffer = string.Empty;
                }

            }
            catch (Exception ex)
            {
                _commandTcs?.TrySetException(ex);
            }
        }

        public bool SendCommand(string command, out string respond, int timeoutMs = 2000)
        {
            if (!_serialPort!.IsOpen)
                throw new InvalidOperationException("串口未打开");

            respond = string.Empty;
            lock (_syncRoot)
            {
                _lastResponse = string.Empty;
                _receiveBuffer = string.Empty;
                _waitHandle.Reset();
            }

            Console.WriteLine($"[SEND] {command}");
            _serialPort.Write(command);

            if (_waitHandle.Wait(timeoutMs))
            {
                lock (_syncRoot)
                    respond = _lastResponse;
                return true;
            }
            else
            {
                Console.WriteLine($"[TIMEOUT] {command} 超时未收到返回");
                return false;
            }
        }
    }
}
