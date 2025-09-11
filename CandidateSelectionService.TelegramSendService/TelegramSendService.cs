using CandidateSelectionService.Core.Models.Search;
using CandidateSelectionService.Core.Service.Interfaces;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace CandidateSelectionService.TelegramSendService
{
    public class TelegramSendService : ISendDataService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly long chatId;

        public TelegramSendService(string token, long chatId)
        {
            _botClient = new TelegramBotClient(token);
            this.chatId = chatId;
        }

        public async Task SendMessageAsync(SendData data, CancellationToken cancellationToken)
        {
            try
            {
                string message = GenerateMessage(data);

                var bot = await _botClient.GetMe(cancellationToken);

                var chat = await _botClient.GetChat(chatId, cancellationToken);

                if (message.Length < 4096)
                {
                    await SendSingleMessage(message, cancellationToken);
                }
                else
                {
                    await SendSplitMessage(message, cancellationToken);
                }
            }
            catch (ApiRequestException ex) when (ex.ErrorCode == 400)
            {
                throw new InvalidOperationException("Чат не существует или бот не имеет доступа", ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GenerateMessage(SendData data)
        {
            var message = new StringBuilder();

            message.AppendLine("Результаты поиска");
            message.AppendLine();

            message.AppendLine($"Пользователь: {data.UserName}");
            message.AppendLine($"Дата поиска: {data.Date}");
            message.AppendLine($"Критерии поиска: {data.SearchData}");
            message.AppendLine();

            if (data.Candidates == null && data.Employess == null)
            {
                message.AppendLine("Поиск не дал результата");
            }
            else
            {
                if (data.Candidates != null && data.Candidates.Any())
                {
                    message.AppendLine($"Кандидаты ({data.Candidates.Count()}):");
                    message.AppendLine();

                    foreach (var candidate in data.Candidates) // Ограничиваем для читаемости
                    {
                        message.AppendLine($"{candidate.Name}");
                        message.AppendLine($"• ID: {candidate.EntityId}");
                        message.AppendLine($"• Дата рождения: {candidate.DateBirth}");
                        message.AppendLine($"• Email: `{candidate.Email}`");
                        message.AppendLine($"• Телефон: `{candidate.Phone}`");
                        message.AppendLine($"• Страна: {candidate.Country}");
                        message.AppendLine($"• График работы: {candidate.WorkSchedule}");
                        message.AppendLine();
                    }
                }

                if (data.Employess != null && data.Employess.Any())
                {
                    message.AppendLine($"Сотрудники ({data.Employess.Count()}):");
                    message.AppendLine();

                    foreach (var employee in data.Employess.Take(5))
                    {
                        message.AppendLine($"{employee.Name}");
                        message.AppendLine($"• ID: {employee.EntityId}");
                        message.AppendLine($"• Дата рождения: {employee.DateBirth}");
                        message.AppendLine($"• Email: `{employee.Email}`");
                        message.AppendLine($"• Телефон: `{employee.Phone}`");
                        message.AppendLine($"• Страна: {employee.Country}");
                        message.AppendLine($"• График работы: {employee.WorkSchedule}");
                        message.AppendLine();
                    }
                }
            }

            return message.ToString();
        }

        private async Task SendSingleMessage(string message, CancellationToken cancellationToken)
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: message,
                cancellationToken: cancellationToken
            );
        }

        private async Task SendSplitMessage(string message, CancellationToken cancellationToken)
        {
            var parts = SplitMessageSmart(message, 4090);

            foreach(var part in parts)
            {
            
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: part,
                    cancellationToken: cancellationToken
                );

                await Task.Delay(200, cancellationToken); 
            }
        }

        private List<string> SplitMessageSmart(string message, int maxLength)
        {
            var parts = new List<string>();

            if (string.IsNullOrEmpty(message))
                return parts;

            var paragraphs = message.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var currentPart = new StringBuilder();

            foreach (var paragraph in paragraphs)
            {
                if (currentPart.Length + paragraph.Length + 2 > maxLength)
                {
                    if (currentPart.Length > 0)
                    {
                        parts.Add(currentPart.ToString());
                        currentPart.Clear();
                    }

                    if (paragraph.Length > maxLength)
                    {
                        var lines = paragraph.Split('\n');
                        foreach (var line in lines)
                        {
                            if (currentPart.Length + line.Length + 1 > maxLength)
                            {
                                if (currentPart.Length > 0)
                                {
                                    parts.Add(currentPart.ToString());
                                    currentPart.Clear();
                                }

                                if (line.Length > maxLength)
                                {
                                    var chunks = SplitLongLine(line, maxLength);
                                    parts.AddRange(chunks);
                                    continue;
                                }
                            }

                            if (currentPart.Length > 0)
                                currentPart.Append('\n');
                            currentPart.Append(line);
                        }
                        continue;
                    }
                }

                if (currentPart.Length > 0)
                    currentPart.Append("\n\n");
                currentPart.Append(paragraph);
            }

            if (currentPart.Length > 0)
                parts.Add(currentPart.ToString());

            return parts;
        }

        private List<string> SplitLongLine(string line, int maxLength)
        {
            var chunks = new List<string>();
            for (int i = 0; i < line.Length; i += maxLength)
            {
                chunks.Add(line.Substring(i, Math.Min(maxLength, line.Length - i)));
            }
            return chunks;
        }
    }
}
