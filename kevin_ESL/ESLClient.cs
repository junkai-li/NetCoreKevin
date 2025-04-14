using kevin_ESL.Const;
using kevin_ESL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace kevin_ESL
{
    public class ESLClient : IDisposable, IESLClient
    {
        private Socket _socket;
        private string HOST;
        private int PORT;
        private string PassWord;
        public ESLClient(string host, int port, string password)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 设置接收超时时间 100秒
            _socket.ReceiveTimeout = 100000;
            HOST = host;
            PORT = port;
            this.PassWord = password;
        }
        public void Connect()
        {
            _socket.Connect(HOST, PORT);
        }

        public string Authenticate(string password)
        {
            SendData($"auth {password}");

            string authResponse = RecolectResponse();
            return authResponse;
        }
        public string ApiCommand(string args, string pand = "OK")
        {
            try
            {
                SendData($"api {args}");
                string Response = RecolectResponse();
                if (Response.Contains(pand))
                {
                    return RecolectResponse();
                }
                return Response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string ApiCommandReslut2(string args)
        {
            try
            {
                SendData($"api {args}");
                string Response = RecolectResponse();
                return Response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string ApiCommandReslut(string args)
        {
            try
            {
                SendData($"api {args}");
                var data = RecolectResponse();
                Thread.Sleep(500);
                var data2 = RecolectResponse();
                return data2;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string Command(string args, string pand = "OK")
        {
            try
            {
                SendData($"api {args}");
                string Response = RecolectResponse();
                if (Response.Contains(pand))
                {
                    return RecolectResponse();
                }
                return Response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetAllMsg()
        {
            SendData($"event json  {EventNameConst.CHANNEL_CALLSTATE}  {EventNameConst.CHANNEL_DESTROY} {EventNameConst.CHANNEL_EXECUTE_COMPLETE} {EventNameConst.CHANNEL_HANGUP_COMPLETE} {EventNameConst.CHANNEL_HANGUP} {EventNameConst.CHANNEL_ANSWER} {EventNameConst.DTMF} {EventNameConst.CHANNEL_STATE}  \r\n\r\n");
            //SendData($"event json ALL \r\n\r\n");

        }
        public void GetAllincomingcallMsg()
        {
            SendData($"event json  {EventNameConst.CHANNEL_CALLSTATE}  {EventNameConst.CHANNEL_DESTROY}  {EventNameConst.CHANNEL_HANGUP_COMPLETE} {EventNameConst.CHANNEL_HANGUP} {EventNameConst.CHANNEL_ANSWER} {EventNameConst.DTMF} {EventNameConst.CHANNEL_STATE}  \r\n\r\n");
            //SendData($"event json ALL \r\n\r\n");

        }
        public void GetAllMsgLog()
        {
            //SendData($"event json  {EventNameConst.CHANNEL_CALLSTATE}  {EventNameConst.CHANNEL_DESTROY} {EventNameConst.CHANNEL_EXECUTE_COMPLETE} {EventNameConst.CHANNEL_HANGUP_COMPLETE} {EventNameConst.CHANNEL_HANGUP} {EventNameConst.CHANNEL_ANSWER} {EventNameConst.DTMF} {EventNameConst.CHANNEL_STATE}  \r\n\r\n");
            SendData($"event json ALL \r\n\r\n");

        }

        public void SendData(string data)
        {
            try
            {
                byte[] msg = Encoding.UTF8.GetBytes($"{data}\n\n");
                _socket.Send(msg);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public string RecolectResponse()
        {
            string responseValue = "";
            string dataBuffer = "";
            var api_isRunning = true;
            while (!(dataBuffer.EndsWith("\n\n") || dataBuffer.EndsWith("\r\n") || (dataBuffer.StartsWith("{") && dataBuffer.EndsWith("}"))) && api_isRunning && _socket.Connected)
            {
                dataBuffer += ReceiveData();
            }
            if (dataBuffer == "C")
            {
                throw new Exception($"服务器异常:服务器断开链接");
            }
            responseValue = DpuParser.GetLineValueFromKey(dataBuffer, "Reply-Text");
            if (!string.IsNullOrEmpty(responseValue))
            {
                return responseValue;
            }
            if (dataBuffer.StartsWith("{") && dataBuffer.EndsWith("}"))
            {
                return dataBuffer;
            }
            var contentLength = DpuParser.GetLineValueFromKey(dataBuffer, "Content-Length");
            var ContentType = DpuParser.GetLineValueFromKey(dataBuffer, "Content-Type");
            if (!string.IsNullOrEmpty(contentLength) && !string.IsNullOrEmpty(ContentType))
            {
                if (ContentType == "api/response")
                {
                    int contentLenght = int.Parse(contentLength);
                    responseValue += ReceiveData(contentLenght);
                }
                else
                {

                    int contentLenght = int.Parse(contentLength);
                    responseValue = "";
                    responseValue += ReceiveData(contentLenght);
                    while (!(responseValue.EndsWith("}")) && api_isRunning && _socket.Connected)
                    {
                        responseValue += ReceiveData(contentLenght - responseValue.Length);
                    }
                }
            }
            return responseValue;
        }

        public string ReceiveData(int contentLenght = 1, bool isstringutf = true)
        {

            try
            {
                if (contentLenght < 1)
                {
                    contentLenght = 1;
                }
                byte[] buffer = new byte[contentLenght];
                int bytesReceived = _socket.Receive(buffer);
                string data = "";
                if (isstringutf)
                {
                    data = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                }
                else
                {
                    data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                }

                return data;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    if (_socket != default)
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                        _socket.Close();
                    }
                    throw new Exception($"接收数据超时");
                }
                else
                {
                    throw new Exception($"Socket异常");
                }
            }
        }

        public void Dispose()
        {
            try
            {
                if (_socket != default)
                {
                    _socket?.Shutdown(SocketShutdown.Both); // 优雅关闭
                    _socket?.Close();                       // 关闭并释放资源（内部调用 Dispose）
                }

            }
            finally
            {
                _socket?.Dispose(); // 显式释放（非必需，但可确保释放）
            }
        }

        private Action<string> EsleventCallback = default;
        /// <summary>
        /// 开启监控
        /// </summary>
        public void StartEnableMonitoring(Action<string> eventCallback)
        {
            EsleventCallback = eventCallback;
            EnableMonitoring(EsleventCallback);
        }
        public bool EnableMonitoringisRunning = true;
        public void EnableMonitoring(Action<string> eventCallback)
        {
            socketConti();
            GetAllincomingcallMsg();
            while (EnableMonitoringisRunning)
            {
                string data = "";
                data = RecolectResponse();
                if (data.Contains("The system is being shut down"))
                {
                    EnableMonitoringisRunning = false; 
                    Task.Run(() =>
                    {
                        Dispose();
                    }); 
                    throw new Exception($"断开链接");
                }
                if (data.Contains("SERVER_DISCONNECTED"))
                {
                    EnableMonitoringisRunning = false; 
                    Task.Run(() =>
                    {
                        Dispose();
                    });
                    throw new Exception($"断开链接");
                }
                if (!string.IsNullOrEmpty(data))
                {
                    eventCallback(data);
                    Thread.Sleep(500);
                }
                else
                {
                    Console.WriteLine("未读取到数据");
                    throw new Exception($"读取数据异常");
                }
            }
            if (!EnableMonitoringisRunning)
            { 
                throw new Exception($"结束");
            }

        }

        /// <summary>
        /// 重连
        /// </summary>
        public void socketConti()
        {
            // 确保Socket资源被释放
            if ((_socket != default && !_socket.Connected) || _socket == default)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 设置接收超时时间 100秒
                _socket.ReceiveTimeout = 100000;
                _socket.Connect(new IPEndPoint(IPAddress.Parse(HOST), PORT));
                Authenticate(PassWord);
            }
        }

        /// <summary>
        /// 获取通道是否可用
        /// </summary>
        public bool GetChannelsIsOk(string seo)
        {

            socketConti();
            try
            {
                var sip = seo.Split('/')[2].ToString();
                var data = ApiCommandReslut2($"sofia_contact {sip}");//是否在线
                if (data.Contains("error"))
                {
                    //不在线返回
                    return false;
                }
            }
            catch
            {
            }
            return !(Command("show channels").Contains(seo));
        }

        /// <summary>
        /// 获取通道数据
        /// </summary>
        public string GetChannelsData()
        {
            socketConti();
            return Command("show channels");
        }
    }
}
