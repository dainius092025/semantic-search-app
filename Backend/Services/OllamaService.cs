//this class implemets IOllamaSrvice, It is responsible for communicating with Ollama to generate embedding and summaries.

using OllamaSharp.Models;
using OllamaSharp;
using Backend.Services.Interfaces;



namespace Backend.Services;
                            //this class must implement IOllamaService contract
public class OllamaService : IOllamaService
{
    //this is a viarable that hods our connection to the Ollama API. We will use it to send requests to Ollama and get responses back
            //readonly = can only be assigned once, in the constructor. This is good for things like API clients that should not change after they are created.
                    //a class provided by the OllamaSharp library that makes it easy to send requests to the Ollama API and handle responses.
                                    //the name of the connection
    private readonly OllamaApiClient _ollama;

    public OllamaService()//constructor, a special method that runs automatically when we create a new instance of the OllamaService class. We use it to initialize the _ollama variable and establish a connection to the Ollama API.
    {
        //here we create a new instance of the OllamaApiClient and pass in the URL of our Ollama API. This sets up the connection so we can start sending requests to generate embeddings and summaries.
        _ollama = new OllamaApiClient("http://ollama:11434");
    }


    // this is my helper method hat retries an operation if it fails, in here <T> measn that this method can return any type of data.
    private async Task<T> RetryAsync<T>(Func<Task<T>> action, int maxRetries = 3)


    {   
        //delays will control how long we wait between retries, we start with 1 second and then double it for each retry (1s, 2s, 4s) to give the system more time to recover if there is a temporary issue. We will retry a maximum of 3 times before giving up and letting the exception happen.
        var delayMs = 1000;


        //we try the operation ultiple times 
        for (int attempt = 1; attempt <= maxRetries; attempt++)

        {   
            //try to execute the operation (for example calling Ollama) if it succeeds, return the result immeddiately
            try
            {
                return await action();
            }

            //if error happens catch it, retry happens only if "when" we still have attempts left
            catch when (attempt < maxRetries)
            {
                //this bit waits before trying again, everytime waiting time doubles
                await Task.Delay(delayMs);

                delayMs *= 2; // backoff timer: double the delay for the next retry
            }
        }

        return await action();// if we get to here it means we have retried the maximum number of times and we will just try one last time and if it fails we will let the exception happen and handle it in the controller.
    }


    //method bellow sends text to Ollama and gets back a vector that represents the meaning of the text. We use this to convert stories and search queries into vectors so we can find stories that match the meaning of a search.
            //async- this method runs asynchronously(app wont freeze)
                //returns a list of decimal numbers that represent the embedding vector
                    //the name of the method
                        //accepts plain text as input
    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        //sends the text to Ollama and asks for an embedding
                    //"await" means to wait for ollama to respond before continuing
                                                            //here we tell ollama what we want
        var result = await RetryAsync(() => _ollama.EmbedAsync(new EmbedRequest
            {
                //name of the AI model we are using to generate the menedding of the text. This is a model provided by Ollama that is specifically designed for converting text into embedding vectors.
                Model = "nomic-embed-text",

                //the text we want to  convertto vector
                Input = new List<string> { text }
            })
        );

        //Ollama returns doubles (decimal numbers), we convert them to float because what our interface expects. We use LINQ to select each number in the result and convert it to a float, then we convert the whole thing to an array.
        return result.Embeddings[0].Select(d => (float)d).ToArray();
    }

    //this method sends text to alllama and gets back the a generated summary 
    public async Task<string> GenerateSummaryAsync(string text)
    {
        return await RetryAsync(async () =>
        {        
            //we create a prompt that we will send to Ollama. A prompt is just the text we send to the AI to tell it what we want. In this case, we are asking it to summarize the story in 1-2 sentences. We include the story text in the prompt so Ollama knows what to summarize.
            var prompt = $"Summarize this short story in 1-2 sentences: {text}";

            //empty string to hold the response from Ollama as it comes in chunks
            var response = "";

            //Add each chunkto our response as it comes in. We use "await foreach" because the response from Ollama is a stream of data that comes in chunks, and we want to process each chunk as it arrives without waiting for the entire response to be finished.
            await foreach (var chunk in _ollama.GenerateAsync(prompt))
            {
                                //we add each chunk of the response to our response variable. The "?"" means that if the chunk is null, we will just add an empty string instead of throwing an error.
                response += chunk?.Response ?? "";
            }
            
            //once we have received the entire response from Ollama, we trim any extra whitespace from the beginning and end of the response and return it as the summary.
            return response.Trim();
        });


    }
}