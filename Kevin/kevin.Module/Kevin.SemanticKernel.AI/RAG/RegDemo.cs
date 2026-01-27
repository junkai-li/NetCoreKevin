using Kevin.AI.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OpenAI;
using System.ClientModel;
using System.Text;
using static Kevin.AI.Dto.ContentDto;

namespace Kevin.AI.SemanticKernel.RAG
{
    public class RegDemo
    {
        public static async Task RegStart()
        {
            var modelId = "deepseek-v3";
            var endpoint = "https://dashscope.aliyuncs.com/compatible-mode/v1/";
            var apiKey = "";

            // 2. 创建一个OpenAI聊天完成的内核
            var builder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelId,
                new Uri(endpoint),
                apiKey);

            builder.Services.AddInMemoryVectorStore().AddQdrantVectorStore("localhost", https: false);
            // 4.构建内核
            Kernel kernel = builder.Build();
            var vectorStore = kernel.GetRequiredService<QdrantVectorStore>();
            var collectionName = "kProd";
            // Load the data.
            var Products = new ContentDto()
            {
                Nodes = new List<NodeDto>()
            };
            Products.Nodes.Add(new NodeDto
            {
                Key = Guid.NewGuid(),
                Name = "罗技鼠标",
                Description = "我是罗技鼠标，只要21块钱"
            });
            Products.Nodes.Add(new NodeDto
            {
                Key = Guid.NewGuid(),
                Name = "凯文鼠标",
                Description = "我是凯文鼠标，只要43块钱"
            });
            Products.Nodes.Add(new NodeDto
            {
                Key = Guid.NewGuid(),
                Name = "哇哈哈儿童书包",
                Description = "我是一个哇哈哈儿童书包只要28块钱"
            });
            Products.Nodes.Add(new NodeDto
            {
                Key = Guid.NewGuid(),
                Name = "七匹狼成人背包",
                Description = "我是一个七匹狼成人背包只要21块钱"
            });
            var collection = vectorStore.GetCollection<Guid, NodeDto>(collectionName);

            await collection.EnsureCollectionExistsAsync();
            //var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>(); 
            foreach (var paragraph in Products.Nodes)
            {
                // Generate the text embedding.
                Console.WriteLine($"Generating embedding for paragraph: {paragraph.Key}");
                paragraph.DescriptionEmbedding = await LLMUtils.GetEmbedding(paragraph.Description);

                await collection.UpsertAsync(paragraph);
                Console.WriteLine();
            }
            // Inspect the returned hotel.

            // 2. 定义提示
            var prompt = """
                        你是一个产品推荐官。 
                        请根据以下数据中相似度大小进行相关产品推荐：{{$data}} 
                        只需要返回你觉得最适合客户询问的内容”{{$msg}}“中最适合推荐的产品Id和产品名称和描述
                       
                        """;
            var explainFunction = kernel.CreateFunctionFromPrompt(prompt);
            // 8.发起一次多轮对话聊天服务
            string? userInput;
            do
            {
                Console.Write("User > ");
                userInput = Console.ReadLine();
                var vectorSearchOptions = new VectorSearchOptions<NodeDto>
                {
                    VectorProperty = r => r.DescriptionEmbedding,
                    //IncludeVectors = true,
                    //Filter= r => r.Name == "书包",
                    //Skip = 2
                };
                var vector = await LLMUtils.GetEmbedding(userInput ?? "");
                // Do the search, passing an options object with a Top value to limit results to the single top match.
                var searchResult = collection.SearchAsync(vector, top: 10, vectorSearchOptions);
                StringBuilder data = new StringBuilder();
                await foreach (var record in searchResult)
                {
                    data.Append("产品Id: " + record.Record.Key);
                    data.Append("产品名称: " + record.Record.Name);
                    data.Append("产品描述: " + record.Record.Description);
                    data.Append("相似度: " + record.Score.ToString());
                    data.Append('\n');
                }
                var arguments = new KernelArguments { { "data", data.ToString() }, { "msg", userInput } };
                var Functionresult = await kernel.InvokeAsync(explainFunction, arguments);
                Console.WriteLine("Assistant > " + Functionresult);
            } while (userInput is not null);
        }
    }
}
