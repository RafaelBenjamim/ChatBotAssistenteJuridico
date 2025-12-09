
using ChatBotAssistenteJuridico.Controller;
using ChatBotAssistenteJuridico.Domain.Interfaces;
using ChatBotAssistenteJuridico.Infrastructure.Interface;
using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatBotAssistenteJuridico.Domain.Application
{
    public class SendRequestApplication : ISendRequestApplication
    {
        private readonly ILogger<SendRequestController> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly GenerativeModel _genAi;
        private readonly ISendRequestRepository _requestRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHistoryRepository _historyRepository;

        public SendRequestApplication(
            ILogger<SendRequestController> logger, 
            ITelegramBotClient botClient, 
            GenerativeModel genAi, 
            ISendRequestRepository requestRepository, 
            ICategoryRepository categoryRepository, 
            IHistoryRepository historyRepository
            )
        {
            _logger = logger;
            _botClient = botClient;
            _genAi = genAi;
            _requestRepository = requestRepository;
            _categoryRepository = categoryRepository;
            _historyRepository = historyRepository;
        }

        public const string Prompt = @"Você é um assistente jurídico senior especialista em legislação Brasileira prestativo e educado.
                                Seu objetivo é simplificar textos jurídicos ('juridiquês') para uma linguagem fácil de entender e tirar dúvidas sobre questões legais frequentes, 
                                especialmente em Direito Trabalhista (férias, acertos, FGTS) e Direito Civil. Baseie suas respostas estritamente na Constituição Federal de 1988 (CF/88), Consolidação das Leis do Trabalho (CLT) e Código Civil Brasileiro.
                                REGRAS:
                                0.  Responda a dúvida do usuario e ALEM disso classifique a duvida em exatamente uma dessas categorias: [{0}].Importante: Responda no seguinte formato estrito: Categoria| Resposta.Exemplo: Direito Trabalhista|O FGTS é o Fundo de Garantia...
                                1.  Se a mensagem do usuário parecer um texto jurídico ou uma pergunta sobre leis, sua tarefa é reescrever ou explicar em linguagem simples, clara e direta.
                                2.  Se a mensagem do usuário parecer uma saudação ou uma conversa casual (como 'olá', 'boa noite', 'tudo bem?', 'obrigado'), apenas responda educadamente como um assistente prestativo. Não tente 'traduzir' a saudação.
                                3.  No final de qualquer explicação jurídica, adicione o aviso: '(Esta é uma interpretação simplificada e não substitui um aconselhamento profissional.)'
                                4.  Se baseie em bases de dados de leis e jurisprudências para ter maior precisão.
                                Mensagem do usuário:
                                '{1}'";

        public async Task SendRequest(Update update)
        {
            if (update.Message is null || update.Message.Text is null) return;

            var messageText = update.Message.Text;
            var chatId = update.Message.Chat.Id;
            var userName = update.Message.From?.FirstName ?? "Usuário";

            _logger.LogInformation("Mensagem recebida: '{messageText}' no chat Id {chatId}.", messageText, chatId);

            await _historyRepository.AddMessage(chatId, "user", messageText);

            try
            {
                if (await Commands(chatId, messageText, userName))
                    return;

                var askHash = GenerateHash(messageText);

                if (await ValidateCache(chatId, askHash))
                    return;

                await _botClient.SendChatActionAsync(chatId, ChatAction.Typing);

                var aiResponse = await GetContext(chatId, messageText);

                await _botClient.SendTextMessageAsync(chatId, aiResponse.cleanText);

                await _requestRepository.saveAsk(askHash, messageText, aiResponse.cleanText, aiResponse.categoryId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem do chat {chatId}", chatId);
                await _botClient.SendTextMessageAsync(chatId, "Desculpe, tive um erro interno. Tente novamente em instantes.");
            }

        }

        public async Task<bool> Commands(long chatId, string text, string userName)
        {
            if(text.Trim().StartsWith("/start", StringComparison.OrdinalIgnoreCase))
            {
                var welcome = $"Olá, {userName}! 👋\n\nEu sou o Assistente Jurídico. Estou aqui para simplificar textos de lei e tirar suas dúvidas.\n\n Basta me enviar sua pergunta!";
                await _botClient.SendTextMessageAsync(chatId, welcome);
                return true;
            }
            return false;
        }

        public async Task<bool> ValidateCache(long chatId, string hash)
        {
            var cacheResponse = await _requestRepository.GetResponseHash(hash);
            
            if(cacheResponse != null)
            {
                _logger.LogInformation("Resposta encontrada no cache");
                await _botClient.SendTextMessageAsync(chatId, cacheResponse.Reposta);
                return true;
            }

            return false;
        }

        public async Task<(string cleanText, int? categoryId)> GetContext(long chatId, string userMessage)
        {
            var history = await _historyRepository.GetHistory(chatId);

            var chatHistory = history.Where(x => x.Message != userMessage).Select(x => new Content
            {
                Role = x.Role == "user" ? "user" : "model",
                Parts = new List<Part> { new Part { Text = x.Message } }
            }).ToList();

            var category = await _categoryRepository.GetAllCategoryNames();

            var categoryString = category.Any() ? string.Join(",", category) : "Trabalhista, Civil, Penal";

            var chatSession = _genAi.StartChat(history: chatHistory);
            string FinalPrompt = string.Format(Prompt, categoryString, userMessage);

            var response = await chatSession.GenerateContentAsync(FinalPrompt);

            return await SeparateQuestion(response.Text);
        }

        public string GenerateHash(string Text)
        {
            if (string.IsNullOrEmpty(Text)) return string.Empty;
            var textNormalized = Regex.Replace(Text.ToLowerInvariant().Trim(), @"[^\w\s]", "");
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(textNormalized));
            return Convert.ToHexString(bytes).ToLower();
        }

        public async Task<(string cleanText, int? categoryId)> SeparateQuestion(string text)
        {
            var cleanResponse = text;
            int? idcategory = null;

            if (text.Contains("|"))
            {         
            var partes = text.Split("|", 2);

            var categoryName = partes[0].Trim();

            var responseText = partes[1].Trim();

            cleanResponse = responseText;

            var category = await _categoryRepository.GetCategoryId(categoryName);

            idcategory = category?.Id;
            
            }

            return (cleanResponse, idcategory);
        }
    }
}
