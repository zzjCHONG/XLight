namespace XLight
{
    public interface ISpin
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="com">填入com口则手动连接对应串口，无则默认为自动连接</param>
        /// <returns></returns>
        public bool Init(string com="");

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool DisInit();

        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>
        public Task<bool> Reset();

        /// <summary>
        /// Confocal Disk Motor
        /// 共焦盘电机
        /// 0 - 1 (Disk)
        /// 0=停转，1=转动
        /// Home位置：0
        /// N
        /// </summary>
        /// <returns></returns>
        public Task<bool> SetDisk(uint value);

        /// <summary>
        /// Spinning Disk position
        /// 共聚焦盘位置
        /// 0 - 2 (Spining)
        /// 0=盘移出光路，1=pos1μm，2=pos2μm
        /// Home位置：0
        /// D
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> SetSpining(uint value);

        /// <summary>
        /// Dichroic FW
        /// 二向色轮 
        /// 1 - 5 (Dichroic)
        /// Home位置：位置1
        /// C
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> SetDichroic(uint value, bool isExtraction = false);

        /// <summary>
        /// Emission FW
        /// 发射滤光轮 
        /// 1 - 8 (Emission)
        /// Home位置：位置1
        /// B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> SetEmission(uint value, bool isExtraction = false);

        /// <summary>
        /// Excitation FW
        /// 激发滤光轮 
        /// Home位置：0
        /// 1-8
        /// A
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<bool> SetExcitation(uint value, bool isExtraction = false);

        /// <summary>
        /// 多位置设置（ABC，DN不支持）
        /// </summary>
        /// <param name="excitationPos"></param>
        /// <param name="emissionPos"></param>
        /// <param name="dichroicPos"></param>
        /// <returns></returns>
        public Task<bool> SetFilterPositions(uint? excitationPos = null, uint? emissionPos = null, uint? dichroicPos = null);

        /// <summary>
        /// 版本号
        /// </summary>
        public Task<string> Version { get; }
    }
}
