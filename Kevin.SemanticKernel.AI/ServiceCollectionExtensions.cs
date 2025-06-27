
using DocumentFormat.OpenXml.Wordprocessing;
using Kevin.AI.SemanticKernel.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
namespace Kevin.AI.SemanticKernel
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 初始化SK
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <param name="_kernel"></param>
        public static void AddAIClient(this IServiceCollection services, Action<SKAISetting> action, Kernel _kernel = null)
        {
            services.Configure(action);
            var SKAISetting = new SKAISetting();
            action.Invoke(SKAISetting);
            services.AddTransient<Kernel>((serviceProvider) =>
            {
                if (_kernel == null)
                {
                    _kernel = Kernel.CreateBuilder()
                                    .AddOpenAIChatCompletion(SKAISetting.modelId,
                                    new Uri(SKAISetting.endpoint),
                                    SKAISetting.apiKey).Build();
                    //// 3.添加企业组件
                    //builder.Services.AddLogging(services => services.AddDebug().SetMinimumLevel(LogLevel.Trace));
                }

                //导入插件
                if (!_kernel.Plugins.Any(p => p.Name == "text2sql"))
                {
                    var pluginPatth = Path.Combine(SamplePluginsPath(), "kevinai");
                    Console.WriteLine($"pluginPatth:{pluginPatth}");
                    _kernel.ImportPluginFromPromptDirectory(pluginPatth);
                }
                // 5.添加一个插件（LightsPlugin类定义如下）
                //_kernel.Plugins.AddFromType<LightsPlugin>("Lights");

                // 6.开启规划 自动调用函数
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                };
                return _kernel;
            });
            services.AddTransient<IKevinAIClient, KevinAIClient>();
        }

        /// <summary>
        /// Scan the local folders from the repo, looking for "samples/plugins" folder.
        /// </summary>
        /// <returns>The full path to samples/plugins</returns>
        public static string SamplePluginsPath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string folderName = "kevinPlugins";

            string FindPluginsDirectory(string startDir, string targetFolder)
            {
                string currDir = Path.GetFullPath(startDir);
                const int maxAttempts = 10;

                for (int i = 0; i < maxAttempts; i++)
                {
                    string potentialPath = Path.Combine(currDir, targetFolder);
                    if (Directory.Exists(potentialPath))
                    {
                        return potentialPath;
                    }

                    currDir = Path.GetFullPath(Path.Combine(currDir, ".."));
                }

                return null; // Not found after max attempts.
            }

            // Check in the BaseDirectory and its parent directories
            string path = FindPluginsDirectory(baseDirectory, folderName)
                        ?? FindPluginsDirectory(baseDirectory + Path.DirectorySeparatorChar + folderName, folderName);

            //if (string.IsNullOrEmpty(path))
            //{
            //    throw new AppException("Plugins directory not found. The app needs the plugins from the repo to work.");
            //}
            return path;
        }
    }
}
