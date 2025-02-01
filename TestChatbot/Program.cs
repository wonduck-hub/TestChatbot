// Import packages
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using System.ClientModel;
using System;
using System.Text;
using OpenAI.Chat;
using OpenAI;

var endpoint = new Uri("https://models.inference.ai.azure.com"); // github models를 사용하는 경우 항상 이 uri를 사용하는 듯
var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN"); // 깃헙에서 발급받은 토큰 환경변수
var modelId = "gpt-4o";
var apiKeyCredential = new ApiKeyCredential(credential);


var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = endpoint
};

var client = new OpenAIClient(apiKeyCredential, openAIOptions);

var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(modelId, client);
Kernel kernel = builder.Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory(); // 프롬프트로 보임
history.AddSystemMessage(
    """
    넌 도우미 쳇봇이야
    """
    );

while (true)
{
    Console.Write("Q: ");
    var userQ = Console.ReadLine();
    if (string.IsNullOrEmpty(userQ))
    {
        break;
    }
    history.AddUserMessage(userQ);

    var sb = new StringBuilder();
    var result = chat.GetStreamingChatMessageContentsAsync(history);
    Console.Write("AI: ");
    await foreach (var item in result)
    {
        sb.Append(item);
        Console.Write(item.Content);
    }
    Console.WriteLine();

    history.AddAssistantMessage(sb.ToString());
}