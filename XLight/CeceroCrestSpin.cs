
namespace XLight
{
    public class CeceroCrestSpin : ISpin
    {
        private readonly XLightCecero.XLight _cecero;

        public CeceroCrestSpin()
        {
            _cecero=new XLightCecero.XLight();
        }

        public Task<string> Version 
            => _cecero.GetVersion();

        public bool DisInit()=> _cecero.DisConnect();

        public bool Init(string com = "")=> _cecero.OpenCom(com);

        public async Task<bool> Reset()=>await _cecero.Home();

        public async Task<bool> SetDisk(uint value)=> await _cecero.SetDisk(value);

        public async Task<bool> SetEmission(uint value) => await _cecero.SetEmission(value);

        public Task<bool> SetExcitation(uint value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetDichroic(uint value)
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
    }
}
