

namespace XLight
{
    public class CiceroCrestSpin : ISpin
    {
        private readonly XLightCicero.XLight _cecero;

        public CiceroCrestSpin()
        {
            _cecero = new XLightCicero.XLight();
        }

        public Task<string> Version
            => _cecero.GetVersion();

        public bool DisInit() => _cecero.DisConnect();

        public bool Init(string com = "") => _cecero.OpenCom(com);

        public async Task<bool> Reset() => await _cecero.Home();

        public async Task<bool> SetDisk(uint value) => await _cecero.SetDisk(value);

        public async Task<bool> SetEmission(uint value, bool isExtraction = false) => await _cecero.SetEmission(value, isExtraction);

        public Task<bool> SetExcitation(uint value, bool isExtraction = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetDichroic(uint value, bool isExtraction = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetFilterPositions(uint? excitationPos = null, uint? emissionPos = null, uint? dichroicPos = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetSpining(uint value)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<char, int>> GetAllDevicesState() => _cecero.GetAllDevicesState();
    }
}
