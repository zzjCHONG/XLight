namespace XLight
{
    public class V2CrestSpin : ISpin
    {
        private readonly XLightV2.XLight _v2;

        public V2CrestSpin()
        {
            _v2 = new XLightV2.XLight();
        }

        public Task<string> Version => _v2.GetVersion();

        public bool DisInit() => _v2.DisConnect();

        public bool Init(string com = "") => _v2.OpenCom(com);

        public async Task<bool> Reset() => await _v2.Home();

        public async Task<bool> SetDichroic(uint value, bool isExtraction = false) => await _v2.SetDichroic(value, isExtraction);

        public async Task<bool> SetDisk(uint value) => await _v2.SetDisk(value);

        public async Task<bool> SetEmission(uint value, bool isExtraction = false) => await _v2.SetEmission(value, isExtraction);

        public async Task<bool> SetExcitation(uint value, bool isExtraction = false) => await _v2.SetExcitation(value, isExtraction);

        public async Task<bool> SetFilterPositions(uint? excitationPos = null, uint? emissionPos = null, uint? dichroicPos = null) => await _v2.SetFilterPositions(excitationPos, emissionPos, dichroicPos);

        public async Task<bool> SetSpining(uint value) => await _v2.SetSpining(value);

    }
}
