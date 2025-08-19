
using XLight;

//XLightCecero.XLight xLightCecero = new();
//if (xLightCecero.OpenCom())
//{
//    var ver = await xLightCecero.GetVersion();
//    Console.WriteLine(ver);
//    Console.ReadLine();
//}

CiceroCrestSpin ceceroCrestSpin = new CiceroCrestSpin();
if (ceceroCrestSpin.Init())
{
    var ver=await ceceroCrestSpin.Version;
    Console.WriteLine(ver);
    Console.ReadLine();
}

//Cecero.XLight xLight = new();
//if (xLight.OpenCom())
//{
//    Console.WriteLine(await xLight.SetResponseActivation(1));
//    Console.WriteLine(await xLight.GetVersion());

//    var ver = await xLight.GetVersion();
//    Console.WriteLine(ver);

//    var state = await xLight.GetIndividualDeviceState('B');
//    Console.WriteLine(state.Item2);

//    var states = await xLight.GetAllDevicesState();
//    Console.WriteLine(string.Join(" ", states.Values));

//    Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.Home());

//    var states2 = await xLight.GetAllDevicesState();
//    Console.WriteLine(string.Join(" ", states2.Values));

//    //Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetDichroic(2));
//    Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetDisk(1));
//    Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetEmission(5));
//    //Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetSpining(2));

//    var states3 = await xLight.GetAllDevicesState();
//    Console.WriteLine(string.Join(" ", states3.Values));

//    Console.WriteLine(await xLight.SetResponseActivation(0));
//    Console.WriteLine(await xLight.GetVersion());

//    Console.WriteLine(await xLight.SetResponseActivation(1));
//    Console.WriteLine(await xLight.GetVersion());

//    Console.ReadLine();
//    xLight.Dispose();
//}

//V2.XLight xLight=new V2.XLight();
//if (xLight.OpenCom())
//{
//    var ver = await xLight.GetVersion();
//    Console.WriteLine(ver);

//    var state1 = await xLight.GetIndividualDeviceState('A');
//    Console.WriteLine(state1.Item2);
//    var state2 = await xLight.GetIndividualDeviceState('B');
//    Console.WriteLine(state2.Item2);
//    var state3 = await xLight.GetIndividualDeviceState('C');
//    Console.WriteLine(state3.Item2);
//    var state4 = await xLight.GetIndividualDeviceState('D');
//    Console.WriteLine(state4.Item2);
//    var state5 = await xLight.GetIndividualDeviceState('N');
//    Console.WriteLine(state5.Item2);

//    //var states = await xLight.GetAllDevicesState();
//    //Console.WriteLine(string.Join(" ", states.Values));

//    //Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.Home());
//    //var states0 = await xLight.GetAllDevicesState();
//    //Console.WriteLine(string.Join(" ", states0.Values));

//    //Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetSpining(1));
//    //Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetDichroic(2));
//    //Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetDisk(1));
//    //Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetEmission(5));
//    //Console.WriteLine($"{DateTime.Now:HH-mm-ss-fff}_" + await xLight.SetExcitation(4));

//    //var states2 = await xLight.GetAllDevicesState();
//    //Console.WriteLine(string.Join(" ", states2.Values));

//    //var state = await xLight.GetIndividualDeviceState('A');
//    //Console.WriteLine(state.Item2);

//    Console.ReadLine();
//}