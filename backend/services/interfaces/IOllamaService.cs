/* This file defines the "contract" for the Ollama service. Any class that implements this interface MUST have these two methods. This allows ither parts of the BE to use Ollama. */

namespace Backend.Services.Interfaces;

public interface IOllamaService
{
    /* NOTE FOR ME SO I GET IT:
        Semantic search works by comparing meaning, not ords. To comparemeaning, we convert text into a list of numbers called an "embedding vector". similar texts produce similar vectors. it sends the text to Ollama and returns a float array(vector) that represents the "meaning of the text numericaly. we used for converting stories and search queries into vectors so we can find that match the meaning of a search. */

    // Task        = asynchronous operation (app won't freeze while waiting)
    // float[]     = returns a list of decimal numbers (the embedding vector)
    // Generate    = creates something new
    // Embedding   = a vector representing the meaning of the text
    // Async       = naming convention for asynchronous methods
    // string text = accepts plain text as input

    Task<float[]> GenerateEmbeddingAsync(string text);


    /* Users need a short sumamry to decide if a story is releveant without reading the full text. We use Ollama to generate this automaticaly instead of writting it manualy. It sends the story to Olama and returns a 1-2 sentence summary that describe the meaning and theme of the story, so later we can use it to display a short description under each search result. */

    // string      = returns plain text (the generated summary)
    // Summary     = a 1-2 sentence description of the story

    Task<string> GenerateSummaryAsync(string text);
}
