using Google.GenAI;

namespace VH_Burguer.Applications.ContentSafety
{
    public class ContentSafetyService : IContentSafetyRepository
    {
        // chave da api do Gemini
        private readonly string _apiKey;

        public ContentSafetyService(IConfiguration configuration)
        {
            // validações:
            // verifica na appsettings.json se o ApiKey existe
            // verificar se existe na variavel de ambiente
            // erro!
            _apiKey = configuration["Gemini:ApiKey"] ??
                Environment.GetEnvironmentVariable("GEMINI_API_KEY") ??
                // caso vc tenha configurado a variavel de ambiente na sua maquina:
                // Environment.GetEnvironmentVariable("GEMINI_API_KEY") ??
                throw new Exception("API key não configurada.");
        }

        public async Task<(bool aprovado, string msg)> ValidarConteudo(string texto)
        {
            // verfifica se a chave veio vazia
            // x-bacon
            // gemini -> texto -> valido???? -> true / false
            if(string.IsNullOrEmpty(_apiKey))
            {
                return(false, "API Key não configurada");
            }

            try
            {
                // cliente responsavel pela comunicacao com o Gemini
                Client client = new Client(apiKey: _apiKey);

                // definir o prompt
                string prompt = $@"Você é um moderador de conteúdo extremamente rigoroso para uma plataforma pública.

                    Analise o TEXTO abaixo considerando as regras:

                    - NÃO é permitido:
                      - palavrões, xingamentos ou linguagem vulgar (ex: ""caralho"", ""porra"", ""merda"", etc.)
                      - conteúdo ofensivo, agressivo ou desrespeitoso
                      - conteúdo com duplo sentido ou conotação sexual
                      - qualquer linguagem inadequada para ambiente profissional ou educacional
                      - conteúdo ilegal (drogas, armas, etc.)

                    - Mesmo que esteja em tom informal ou ""brincadeira"", ainda deve ser considerado INSEGURO.

                    - Seja extremamente conservador: na dúvida, classifique como INSEGURO.

                    Responda APENAS com:

                    SEGURO ou INSEGURO: [breve motivo em português]

                    TEXTO:{texto}";

                // envia o texto para analise de ia
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash-lite",
                    contents: prompt
                );

                // obter a resposta gerada pela ia
                string result = response.Text?.Trim().ToUpper() ?? "";
                // INSEGURO: é um palavrão TEXTO: vai tomar la
                if(result.StartsWith("INSEGURO"))
                {
                    return (false, result);
                }

                return (true, "Textos seguros! ^_^ ");
            }
            catch (Exception ex)
            {
                return (false, "Erro na IA" + ex.Message);
            }
            //throw new NotImplementedException();
        }
    }
}
