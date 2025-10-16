using System;

namespace UdpMulticastChat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Chat UDP Multicast";
            
            // Solicita o nome do usuário
            Console.Write("Digite seu nome de usuário: ");
            string username = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Nome inválido!");
                return;
            }
            
            // Cria e inicia o cliente de chat
            using (ChatClient client = new ChatClient(username))
            {
                client.Start();
                
                // Loop principal para enviar mensagens
                string input;
                while ((input = Console.ReadLine()) != null)
                {
                    if (input.Trim().ToLower() == "sair")
                    {
                        break;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        var message = new Models.ChatMessage
                        {
                            Username = client.Username,
                            Content = input,
                            Type = Models.MessageType.Message
                        };
                        
                        client.SendMessage(message);
                    }
                }
                
                client.Stop();
            }
        }
    }
}
