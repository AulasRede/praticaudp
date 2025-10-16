using System;
using System.Text;
using System.Text.Json;

namespace UdpMulticastChat.Models
{
    /// <summary>
    /// Representa uma mensagem no chat
    /// </summary>
    public class ChatMessage
    {
        public string Username { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageType Type { get; set; }
        
        public ChatMessage()
        {
            Timestamp = DateTime.Now;
        }
        
        /// <summary>
        /// Converte a mensagem para JSON e depois para bytes
        /// </summary>
        public byte[] ToBytes()
        {
            string json = JsonSerializer.Serialize(this);
            return Encoding.UTF8.GetBytes(json);
        }
        
        /// <summary>
        /// Cria uma mensagem a partir de bytes
        /// </summary>
        public static ChatMessage FromBytes(byte[] data)
        {
            try
            {
                string json = Encoding.UTF8.GetString(data);
                return JsonSerializer.Deserialize<ChatMessage>(json);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Formata a mensagem para exibição
        /// </summary>
        public string FormatForDisplay()
        {
            string timeStr = Timestamp.ToString("HH:mm:ss");
            
            switch (Type)
            {
                case MessageType.UserJoined:
                    return $"[{timeStr}] *** {Username} entrou no chat ***";
                
                case MessageType.UserLeft:
                    return $"[{timeStr}] *** {Username} saiu do chat ***";
                
                case MessageType.Message:
                    return $"[{timeStr}] {Username}: {Content}";
                
                default:
                    return $"[{timeStr}] {Content}";
            }
        }
    }
    
    /// <summary>
    /// Tipos de mensagens do chat
    /// </summary>
    public enum MessageType
    {
        Message,      // Mensagem normal
        UserJoined,   // Usuário entrou
        UserLeft      // Usuário saiu
    }
}
