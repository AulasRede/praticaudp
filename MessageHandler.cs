using System;
using UdpMulticastChat.Models;

namespace UdpMulticastChat
{
    /// <summary>
    /// Gerencia o processamento e exibição de mensagens
    /// </summary>
    public class MessageHandler
    {
        private readonly string _currentUsername;
        
        public MessageHandler(string username)
        {
            _currentUsername = username;
        }
        
        /// <summary>
        /// Processa e exibe uma mensagem recebida
        /// </summary>
        public void ProcessReceivedMessage(ChatMessage message)
        {
            if (message == null) return;
            
            // Não exibe mensagens enviadas por si mesmo
            if (message.Username == _currentUsername)
                return;
            
            // Define a cor baseada no tipo de mensagem
            ConsoleColor originalColor = Console.ForegroundColor;
            
            switch (message.Type)
            {
                case MessageType.UserJoined:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                
                case MessageType.UserLeft:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                
                case MessageType.Message:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }
            
            Console.WriteLine(message.FormatForDisplay());
            Console.ForegroundColor = originalColor;
        }
        
        /// <summary>
        /// Cria uma mensagem de chat normal
        /// </summary>
        public ChatMessage CreateMessage(string content)
        {
            return new ChatMessage
            {
                Username = _currentUsername,
                Content = content,
                Type = MessageType.Message
            };
        }
        
        /// <summary>
        /// Cria uma mensagem de entrada no chat
        /// </summary>
        public ChatMessage CreateJoinMessage()
        {
            return new ChatMessage
            {
                Username = _currentUsername,
                Content = string.Empty,
                Type = MessageType.UserJoined
            };
        }
        
        /// <summary>
        /// Cria uma mensagem de saída do chat
        /// </summary>
        public ChatMessage CreateLeaveMessage()
        {
            return new ChatMessage
            {
                Username = _currentUsername,
                Content = string.Empty,
                Type = MessageType.UserLeft
            };
        }
    }
}
