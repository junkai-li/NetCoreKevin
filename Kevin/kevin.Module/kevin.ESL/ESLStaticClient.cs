using kevin_ESL.Const;
using kevin_ESL.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kevin_ESL
{
    /// <summary>
    /// 用于链接ESL服务
    /// </summary>
    public static class ESLStaticClient
    {
        public static Socket? _Socket;
        public static string HOST = "";
        public static int PORT;
        public static string PassWord = "";
        public static ConcurrentDictionary<string, Action<string>> Callbacks = new ConcurrentDictionary<string, Action<string>>();
        public static ConcurrentQueue<string> Data = new ConcurrentQueue<string>();
        static Thread? Thread;
        static object lockObject = new();

        /// <summary>
        /// 注册启动事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public static void RegisterCallback(string id, Action<string> callback)
        {
            if (!Callbacks.ContainsKey(id))
            {
                Callbacks.TryAdd(id, callback);
            }
        }
        /// <summary>
        /// 删除回调事件
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteCallBack(string id)
        {
            //延迟删除？
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                if (Callbacks.ContainsKey(id))
                {
                    Callbacks.TryRemove(id, out _);
                }
            });
        }
        /// <summary>
        /// 配置ESL
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public static void ESLStaticClientConfig(string host, int port, string password)
        {
            if (_Socket == default)
            {
                _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 设置接收超时时间 1000秒
                _Socket.ReceiveTimeout = 1000000;
                HOST = host;
                PORT = port;
                PassWord = password;
                _Socket.Connect(new IPEndPoint(IPAddress.Parse(HOST), PORT));
                Authenticate(PassWord);
                GetAllMsg();
                Thread = new Thread(() =>
                {
                    ESLEvent();
                });
                //实时分发数据
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (Thread.ThreadState == ThreadState.Stopped)
                        {
                            Console.WriteLine("ESLEvent线程已完成。");
                            Thread = new Thread(() =>
                            {
                                // 模拟线程执行一些任务，这里睡眠3秒 
                                Thread.Sleep(1000);
                                ESLEvent();
                            });
                            Thread.Start();
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                });
                Thread.Start();
            }
        }
        /// <summary>
        /// 事件分发
        /// </summary>
        public static void ESLEvent()
        {
            Console.WriteLine("开启ESLEvent线程");
            while (true)
            {
                try
                {
                    if (Data.Count > 0)
                    {
                        foreach (var i in Data)
                        {
                            if (Data.TryDequeue(out string value))
                            {
                                if (Callbacks.Count > 0)
                                {
                                    if (IsValidJson(value))
                                    {
                                        Parallel.ForEach(Callbacks, new ParallelOptions
                                        {
                                            MaxDegreeOfParallelism = Callbacks.Count,
                                        }, item =>
                                        {
                                            Task.Run(() =>
                                            {
                                                item.Value(value);
                                            });

                                        });
                                    }
                                    else
                                    {
                                        string pattern = @"\{.*?\}";
                                        MatchCollection matches = Regex.Matches(value, pattern);
                                        if (matches.Count > 0)
                                        {
                                            foreach (Match match in matches)
                                            {
                                                Parallel.ForEach(Callbacks, new ParallelOptions
                                                {
                                                    MaxDegreeOfParallelism = Callbacks.Count,
                                                }, item =>
                                                {
                                                    Task.Run(() =>
                                                    {
                                                        item.Value(match.Value);
                                                    });
                                                });
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }
                    Thread.Sleep(100);
                }
                catch (Exception)
                {

                    Thread.Sleep(500);
                }

            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public static void EslData()
        {
            Task.Run(() =>
            {
                try
                {
                    while (true && _Socket != default)
                    {
                        socketConti();
                        var item = RecolectResponse();
                        if (item.Contains("The system is being shut down"))
                        {
                            Task.Run(() =>
                            {
                                Dispose();
                                socketConti();
                            });
                            throw new Exception($"服务器异常:服务器断开链接");
                        }
                        if (item.Contains("SERVER_DISCONNECTED"))
                        {
                            Task.Run(() =>
                            {
                                Dispose();
                                socketConti();
                            });
                            throw new Exception($"服务器异常:断开链接");

                        }
                        //线路占用
                        if ((item.Contains("UNALLOCATED_NUMBER") || item.Contains("NO_USER_RESPONSE")))
                        {
                            throw new Exception($"系统异常:线路占用||线路忙;" + item);
                        }
                        Data.Enqueue(item);
                    }
                }
                catch (Exception ex)
                {
                    Data.Enqueue("异常:" + ex.Message);
                    Thread.Sleep(500);
                    EslData();
                }
            });
        }

        /// <summary>
        /// 关闭链接
        /// </summary>
        public static void Dispose()
        {
            lock (lockObject)
            {
                try
                {
                    if (_Socket != default)
                    {
                        _Socket?.Shutdown(SocketShutdown.Both); // 优雅关闭
                        _Socket?.Close();                       // 关闭并释放资源（内部调用 Dispose）
                    }

                }
                finally
                {
                    _Socket?.Dispose(); // 显式释放（非必需，但可确保释放）
                }
            }
        }
        /// <summary>
        /// 验证json
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidJson(string input)
        {
            try
            {
                JsonDocument.Parse(input);
                return true;
            }
            catch (System.Text.Json.JsonException)
            {
                return false;
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Authenticate(string password)
        {
            SendData($"auth {password}");

            string authResponse = RecolectResponse();
            return authResponse;
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="args"></param>
        /// <param name="pand"></param>
        /// <returns></returns>
        public static string Command(string args, string pand = "OK")
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
        /// <summary>
        /// 重连
        /// </summary>
        public static void socketConti()
        {
            lock (lockObject)
            {
                // 确保Socket资源被释放
                if ((_Socket != default && !_Socket.Connected) || _Socket == default)
                {
                    _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // 设置接收超时时间 1000秒
                    _Socket.ReceiveTimeout = 100000;
                    _Socket.Connect(new IPEndPoint(IPAddress.Parse(HOST), PORT));
                    Authenticate(PassWord);
                    GetAllMsg();
                }
            }
        }
        /// <summary>
        /// 获取通道所有消息
        /// </summary>
        /// <returns></returns>
        public static string GetAllMsg()
        {
            SendData($"event json  {EventNameConst.CHANNEL_ANSWER}  {EventNameConst.CHANNEL_DESTROY} {EventNameConst.CHANNEL_EXECUTE_COMPLETE} \r\n\r\n");
            //SendData($"event json ALL \r\n\r\n");
            string Response = RecolectResponse();
            return Response;
        }

        //public static string GetAllMsgLog()
        //{
        //    //SendData($"event json  {EventNameConst.CHANNEL_CALLSTATE}  {EventNameConst.CHANNEL_DESTROY} {EventNameConst.CHANNEL_EXECUTE_COMPLETE} {EventNameConst.CHANNEL_HANGUP_COMPLETE} {EventNameConst.CHANNEL_HANGUP} {EventNameConst.CHANNEL_ANSWER} {EventNameConst.DTMF} {EventNameConst.CHANNEL_STATE}  \r\n\r\n");
        //    SendData($"event json ALL \r\n\r\n");
        //    string Response = RecolectResponse();
        //    return Response;
        //}
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dateTime"></param>
        private static void SendData(string data, DateTime dateTime = default)
        {
            try
            {
                if (_Socket == default)
                {
                    return;
                }
                byte[] msg = Encoding.UTF8.GetBytes($"{data}\n\n");
                socketConti();
                _Socket.Send(msg);
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 返回数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string RecolectResponse()
        {
            string responseValue = "";
            string dataBuffer = "";
            while (!(dataBuffer.EndsWith("\n\n") || dataBuffer.EndsWith("\r\n") || (IsValidJson(dataBuffer))) && (_Socket?.Connected ?? false))
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
            if (IsValidJson(dataBuffer))
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
                    while (!(responseValue.EndsWith("}")) && _Socket.Connected)
                    {
                        responseValue += ReceiveData(contentLenght - responseValue.Length);
                    }
                }
            }
            return responseValue;
        }
        /// <summary>
        /// 返回字节数据
        /// </summary>
        /// <param name="contentLength"></param>
        /// <param name="isstringutf"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string ReceiveData(int contentLength = 1, bool isstringutf = true)
        {
            if (_Socket == default)
            {
                return "";
            }
            try
            {
                if (contentLength < 1)
                {
                    contentLength = 1;
                }
                byte[] buffer = new byte[contentLength];
                int totalBytesReceived = 0;
                while (totalBytesReceived < contentLength)
                {
                    int bytesReceived = _Socket.Receive(
                        buffer,
                        totalBytesReceived,
                        contentLength - totalBytesReceived,
                        SocketFlags.None
                    );
                    if (bytesReceived == 0)
                        throw new SocketException((int)SocketError.ConnectionReset);
                    totalBytesReceived += bytesReceived;
                }
                string data = "";
                if (isstringutf)
                {
                    data = Encoding.UTF8.GetString(buffer, 0, totalBytesReceived);
                }
                else
                {
                    data = Encoding.ASCII.GetString(buffer, 0, totalBytesReceived);
                }

                return data;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    socketConti();
                    throw new Exception($"服务器异常:服务器接收数据超时");
                }
                else
                {
                    throw new Exception($"服务器异常:Socket");
                }

            }
        }

        /// <summary>
        /// 获取通道是否占用
        /// </summary>
        public static bool GetChannelsIsOk(string seo)
        {
            socketConti();
            return !(Command("show channels").Contains(seo));
        }
    }
}
