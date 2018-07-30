using Microsoft.AspNetCore.SignalR;
using Modbus.ASCII;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KVM.WEB.Hubs
{
  public class PLCHub : Hub
  {
    private static ModbusSocket Z = null;
    private static ModbusSocket C = null;
    private static IHubCallerClients _clients = null;

    public PLCHub()
    {
      TaskFactory tf = new TaskFactory();
      tf.StartNew(() => Console.WriteLine(Thread.CurrentThread.ManagedThreadId));

      if (Z == null)
      {
        Z = new ModbusSocket("192.168.181.101", 502, "主站", true);
        Z.ModbusLinkSuccess += ModbusLinkSuccess;
        Z.ModbusLinkError += ModbusLinkError;
      }
      if (C == null)
      {
        C = new ModbusSocket("192.168.181.102", 502, "从站", false);
        C.ModbusLinkSuccess += ModbusLinkSuccess;
        C.ModbusLinkError += ModbusLinkError;
      }
    }
    public void Init()
    {
      _clients = Clients;
    }
    public void F05(InPLC data)
    {

        if (data.Id == 1)
        {
          Z.F05(data.Address, data.F05, null);
        }
        else
        {
          C.F05(data.Address, data.F05, null);
        }
    }
    public void F06(InPLC data)
    {

      if (data.Id == 1)
      {
        Z.F06(data.Address, data.F06, null);
      }
      else
      {
        C.F06(data.Address, data.F06, null);
      }
    }
    public void F01(InPLC data)
    {
        if (data.Id == 1)
        {
          Z.F01(data.Address, data.F01, (rData) => {
            _clients.All.SendAsync("noe", rData);
          });
        }
        else
        {
          C.F01(data.Address, data.F01, (rData) => {
            _clients.All.SendAsync("noe", rData);
          });
        }
    }
    /// <summary>
    /// 通信状态事件
    /// </summary>
    /// <param name="id">主从Id</param>
    /// <param name="message">通信状态</param>
    public void ModbusLinkError(string id, string message)
    {
      if (_clients != null)
      {
        _clients.All.SendAsync("Send", new { Id = id, Message = message });
      }
    }
    public void ModbusLinkSuccess(string id, string message)
    {
      var device = Z;
      if (id == "从站")
      {
        device = C;
      }
      //GetDeviceParameter(device);
      // 心跳包保证链接
      Task.Run(() =>
      {
        while (device.Client != null && device.Client.Connected && device.IsSuccess)
        {
          device.F05(PLCSite.M(0), true, null);
          device.F03(PLCSite.D(0), 8, (data) => {
            _clients.All.SendAsync("LiveData", new { name = device.Name, data = ReceiveData.F03(data, 8) } );
          });
          Thread.Sleep(10);
        }
      });
      //if (_clients != null)
      //{
      //  _clients.All.SendAsync("Send", new { Id = id, Message = message });
      //}
    }
    /// <summary>
    /// 获取设备参数
    /// </summary>
    public void GetDeviceParameter()
    {
      ModbusSocket device = Z;
      device.F03(PLCSite.D(500), 17, (data) => {
        _clients.All.SendAsync("DeviceParameter", new { name = device.Name, data = ReceiveData.F03(data, 17) });
      });
    }
    public Boolean SetDeviceParameter(SetDeviceParameterData data)
    {
      try
      {
        Z.F06(PLCSite.D(data.Address), data.Value, null);
        C.F06(PLCSite.D(data.Address), data.Value, null);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
  public class InPLC
  {
    public int Id { get; set; }
    public int Address { get; set; }
    public Boolean F05 { get; set; }
    public int F01 { get; set; }
    public int F06 { get; set; }
  }
  public class SetDeviceParameterData
  {
    public int Address { get; set; }
    public int Value { get; set; }
  }
}
