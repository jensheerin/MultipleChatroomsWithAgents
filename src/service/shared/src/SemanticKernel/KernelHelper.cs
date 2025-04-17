// Import necessary namespaces for document handling, Azure services, dependency injection, and Semantic Kernel connectivity.
 
using Azure;
using Azure.Identity;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using MultiAgents.SemanticKernel.VectorStore.Documents;


namespace MultiAgents.SemanticKernel
{
    /// <summary>
    /// Helper class for configuring and building a Semantic Kernel.
    /// This class manages the initialization of the kernel with various services including:
    /// - Azure OpenAI services (for chat completion and text embedding generation)
    /// - Ollama services (for chat completion and text embedding generation)
    /// - Pinecone and Qdrant vector stores for storing embeddings.
    /// - Azure Cognitive Search vector store.
    /// </summary>
    public class KernelHelper
    {
        public static int EmbeddingDimension { get; set; }

        /// <summary>
        /// Configures the kernel with Azure OpenAI services and, optionally, Azure Cognitive Search services.
        /// </summary>
        /// <param name="kernelBuilder">The kernel builder used to add services.</param>
        /// <param name="configuration">The configuration containing API keys, endpoints, and deployment names.</param>
        /// <exception cref="InvalidOperationException">Thrown if any required Azure OpenAI environment variables are not set.</exception>
        static public void SetupAzure(IKernelBuilder kernelBuilder, IConfiguration configuration)
        {
            EmbeddingDimension = 1536;
            // Retrieve Azure OpenAI configuration settings
            var endpoint = configuration["AZURE_OPENAI_ENDPOINT"];
            var deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT"];

            // Validate required settings
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(deploymentName))
            {
                throw new InvalidOperationException("Azure OpenAI endpoint or deployment name is not set.");
            }

            // Use managed identity for authentication
            var endpointUri = new Uri(endpoint);
            var credential = new DefaultAzureCredential();

            // Register Azure OpenAI services using API key and endpoint strings
            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                apiKey: configuration["AZURE_OPENAI_API_KEY"],
                endpoint: configuration["AZURE_OPENAI_ENDPOINT"]
            );
#pragma warning disable SKEXP0010
            kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
                deploymentName: deploymentName,
                apiKey: configuration["AZURE_OPENAI_API_KEY"],
                endpoint: configuration["AZURE_OPENAI_ENDPOINT"]
            );
#pragma warning restore SKEXP0010

            // Optional: configure Azure Cognitive Search if endpoint provided
            var searchEndpoint = configuration["AZURE_SEARCH_ENDPOINT"];
            if (!string.IsNullOrEmpty(searchEndpoint))
            {
                var searchUri = new Uri(searchEndpoint);
                var searchCredential = new DefaultAzureCredential();
                kernelBuilder.AddAzureAISearchVectorStore(searchUri, searchCredential);
            }
        }

        /// <summary>
        /// Configures the kernel with Ollama services, including chat completion and text embedding generation.
        /// </summary>
        /// <param name="kernelBuilder">The kernel builder used to add services.</param>
        /// <param name="configuration">The configuration containing the Ollama endpoint and model details.</param>
        //static public void SetupOllama(IKernelBuilder kernelBuilder, IConfiguration configuration)
        //{
            //this is the embedding size for and we can use this to make other lives easier
          //  EmbeddingDimension = 3584;

            // Retrieve Ollama endpoint and model details from the configuration.
            // If not provided, default to a local endpoint and a default model.
            // var ollamaEndpoint = configuration["OLLAMA_ENDPOINT"] ?? "http://localhost:11434";
            //var modelId = configuration["OLLAMA_MODEL"] ?? "deepseek-r1";

            // Create a URI from the provided Ollama endpoint.
            //var ollamaUri = new Uri(ollamaEndpoint);

            // Add the Ollama Chat Completion service to the kernel.
            // The extension method is expected to integrate with the Ollama API.
//#pragma warning disable SKEXP0070
            //kernelBuilder.AddOllamaChatCompletion(modelId, ollamaUri);

            // Add the Ollama Text Embedding Generation service to the kernel.
            // This enables the kernel to generate text embeddings using the Ollama service.
           // kernelBuilder.AddOllamaTextEmbeddingGeneration(modelId, ollamaUri);
//#pragma warning restore SKEXP0070
        //}

            /// <summary>
            /// Configures the kernel to use the Pinecone vector store.
            /// </summary>
            /// <param name="builder">The kernel builder used to add services.</param>
            /// <param name="configuration">The configuration containing the Pinecone API key.</param>
            //static public void SetupPinecone(IKernelBuilder builder, IConfiguration configuration)
        //{
            // Retrieve the Pinecone API key from the configuration or use a placeholder.
            //var pineconeApiKey = configuration["PINECONE_API_KEY"] ?? "your-pinecone-api-key";

            // Add Pinecone vector store to the kernel using the provided API key.
            //builder.AddPineconeVectorStore(pineconeApiKey);

            // Register the DataUploader as a singleton service, which is used for uploading data to the vector store.
            //builder.Services.AddSingleton<DataUploader>();
        //}

        /// <summary>
        /// Configures the kernel with Qdrant as the vector store.
        /// </summary>
        /// <param name="builder">The kernel builder used to add services.</param>
        /// <param name="configuration">The configuration containing Qdrant settings.</param>
        //public static void SetupQdrant(IKernelBuilder builder, IConfiguration configuration)
        //{
            // Retrieve the Qdrant endpoint from the configuration or use a default value.
            //var qdrantEndpoint = configuration["QDRANT_ENDPOINT"] ?? "https://localhost:6334";

            // Parse the Qdrant endpoint URI to extract the host and port information.
            //var uri = new Uri(qdrantEndpoint);
            //string host = uri.Host;
            //int port = uri.Port;

            // Add the Qdrant vector store to the kernel using the extracted host and port.
            //builder.AddQdrantVectorStore(host, port);

            // Register the DataUploader as a singleton service for uploading data to the vector store.
            //builder.Services.AddSingleton<DataUploader>();
        //}

        /// <summary>
        /// Configures the kernel to use the Azure Cognitive Search vector store.
        /// </summary>
        /// <param name="builder">The kernel builder used to add services.</param>
        /// <param name="configuration">The configuration containing Azure Cognitive Search settings.</param>
        public static void SetupAzureSearch(IKernelBuilder builder, IConfiguration configuration)
        {
            // Retrieve the Azure Cognitive Search endpoint, key, and index name from the configuration.
            var azureSearchEndpoint = configuration["AZURE_SEARCH_ENDPOINT"] ?? "AZURE_SEARCH_ENDPOINT";
            var azureSearchKey = configuration["AZURE_SEARCH_KEY"] ?? "AZURE_SEARCH_KEY";
            var indexName = configuration["AZURE_SEARCH_INDEX"] ?? "pdf-docs";
            var endpoint = new Uri(azureSearchEndpoint);
            var cred = new AzureKeyCredential(azureSearchKey);
            builder.AddAzureAISearchVectorStore(endpoint, cred);

            // Create an AzureKeyCredential using the search key.
            var tokenCredential = new AzureKeyCredential(azureSearchKey);

            // Add the Azure Cognitive Search vector store to the kernel.
            builder.AddAzureAISearchVectorStore(new Uri(azureSearchEndpoint), tokenCredential);

            // Register the DataUploader as a singleton service for uploading data to the vector store.
            builder.Services.AddSingleton<DataUploader>();
        }
    }
}
