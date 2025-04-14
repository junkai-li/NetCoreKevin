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
    public static class ESLStaticClient
    {
        public static Socket _socket = default;
        public static string HOST;
        public static int PORT;
        public static string PassWord;
        public static ConcurrentDictionary<string, Action<string>> callbacks = new ConcurrentDictionary<string, Action<string>>();
        public static ConcurrentQueue<string> Data = new ConcurrentQueue<string>();
        static Thread thread;
        static object lockObject = new object();
        public static void RegisterCallback(string id, Action<string> callback)
        {
            if (!callbacks.ContainsKey(id))
            {
                callbacks.TryAdd(id, callback);
            }
        }
        public static void DeleteCallBack(string id)
        {
            //延迟删除？
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                if (callbacks.ContainsKey(id))
                {
                    callbacks.TryRemove(id, out _);
                }
            });
        }

        public static void ESLStaticClientConfig(string host, int port, string password)
        {
            if (_socket == default)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 设置接收超时时间 1000秒
                _socket.ReceiveTimeout = 1000000;
                HOST = host;
                PORT = port;
                PassWord = password;
                _socket.Connect(new IPEndPoint(IPAddress.Parse(HOST), PORT));
                Authenticate(PassWord);
                GetAllMsg();
                thread = new Thread(() =>
                {
                    ESLEvent();
                });
                //实时分发数据
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (thread.ThreadState == ThreadState.Stopped)
                        {
                            Console.WriteLine("ESLEvent线程已完成。");
                            thread = new Thread(() =>
                            {
                                // 模拟线程执行一些任务，这里睡眠3秒 
                                Thread.Sleep(1000);
                                ESLEvent();
                            });
                            thread.Start();
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                });
                thread.Start();
            }
        }

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
                                if (callbacks.Count > 0)
                                {
                                    if (IsValidJson(value))
                                    {
                                        Parallel.ForEach(callbacks, new ParallelOptions
                                        {
                                            MaxDegreeOfParallelism = callbacks.Count,
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
                                                Parallel.ForEach(callbacks, new ParallelOptions
                                                {
                                                    MaxDegreeOfParallelism = callbacks.Count,
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

        public static void EslData()
        {
            Task.Run(() =>
            {
                try
                {
                    while (true && _socket != default)
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

        public static void Dispose()
        {
            lock (lockObject)
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
        }
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
        public static string Authenticate(string password)
        {
            SendData($"auth {password}");

            string authResponse = RecolectResponse();
            return authResponse;
        }
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
                if ((_socket != default && !_socket.Connected) || _socket == default)
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // 设置接收超时时间 1000秒
                    _socket.ReceiveTimeout = 100000;
                    _socket.Connect(new IPEndPoint(IPAddress.Parse(HOST), PORT));
                    Authenticate(PassWord);
                    GetAllMsg();
                }
            }
        }

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

        private static void SendData(string data, DateTime dateTime = default)
        {
            try
            {
                byte[] msg = Encoding.UTF8.GetBytes($"{data}\n\n");
                socketConti();
                _socket.Send(msg);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private static string RecolectResponse()
        {
            string responseValue = "";
            string dataBuffer = "";
            while (!(dataBuffer.EndsWith("\n\n") || dataBuffer.EndsWith("\r\n") || (IsValidJson(dataBuffer))) && _socket.Connected)
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
                    while (!(responseValue.EndsWith("}")) && _socket.Connected)
                    {
                        responseValue += ReceiveData(contentLenght - responseValue.Length);
                    }
                }
            }
            return responseValue;
        }

        private static string ReceiveData(int contentLength = 1, bool isstringutf = true)
        {

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
                    int bytesReceived = _socket.Receive(
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
