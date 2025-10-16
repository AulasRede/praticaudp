using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UdpMulticastChat.Models;

namespace UdpMulticastChat
{
    /// <summary>
    /// Cliente de chat usando UDP Multicast
    /// </summary>
    public class ChatClient : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _multicastEndpoint;
        private readonly MessageHandler _messageHandler;
        private Thread _receiveThread;
        private bool _isRunning;
        
        public string Username { get; private set; }
        
        public ChatClient(string username)
        {
            Username = username;
            _messageHandler = new MessageHandler(username);
            
            // Cria o endpoint multicast
            _multicastEndpoint = new IPEndPoint(
                NetworkConfig.MulticastAddress, 
                NetworkConfig.Port
            );
            
            // Configura o cliente UDP
            _udpClient = new UdpClient();
            _udpClient.ExclusiveAddressUse = false;
            
            // Permite reutilização de endereço (importante para multicast)
            _udpClient.Client.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true
            );
            
            // Vincula à porta local
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, NetworkConfig.Port));
            
            // Entra no grupo multicast
            _udpClient.JoinMulticastGroup(
                NetworkConfig.MulticastAddress, 
                NetworkConfig.MulticastTTL
            );
        }
        
        /// <summary>
        /// Inicia o cliente de chat
        /// </summary>
        public void Start()
        {
            _isRunning = true;
            
            // Inicia a thread de recepção
            _receiveThread = new Thread(ReceiveMessages)
            {
                IsBackground = true
            };
            _receiveThread.Start();
            
            // Anuncia entrada no chat
            SendMessage(_messageHandler.CreateJoinMessage());
            
            Console.WriteLine($"╔══════════════════════════════════════════╗");
            Console.WriteLine($"║   Bem-vindo ao Chat UDP Multicast!       ║");
            Console.WriteLine($"╚══════════════════════════════════════════╝");
            Console.WriteLine($"Usuário: {Username}");
            Console.WriteLine($"Grupo Multicast: {NetworkConfig.MulticastAddress}:{NetworkConfig.Port}");
            Console.WriteLine($"Digite suas mensagens (ou 'sair' para encerrar)\n");
        }
        
        /// <summary>
        /// Envia uma mensagem para o grupo multicast
        /// </summary>
        public void SendMessage(ChatMessage message)
        {
            try
            {
                byte[] data = message.ToBytes();
                _udpClient.Send(data, data.Length, _multicastEndpoint);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
                Console.ResetColor();
            }
        }
        
        /// <summary>
        /// Thread que recebe mensagens continuamente
        /// </summary>
        private void ReceiveMessages()
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            
            while (_isRunning)
            {
                try
                {
                    // Recebe dados do grupo multicast
                    byte[] receivedData = _udpClient.Receive(ref remoteEndpoint);
                    
                    // Converte para mensagem
                    ChatMessage message = ChatMessage.FromBytes(receivedData);
                    
                    // Processa e exibe a mensagem
                    _messageHandler.ProcessReceivedMessage(message);
                }
                catch (SocketException)
                {
                    // Socket foi fechado, encerra a thread
                    break;
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Erro ao receber mensagem: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
        }
        
        /// <summary>
        /// Para o cliente e libera recursos
        /// </summary>
        public void Stop()
        {
            if (!_isRunning) return;
            
            _isRunning = false;
            
            // Anuncia saída do chat
            SendMessage(_messageHandler.CreateLeaveMessage());
            
            // Aguarda um momento para a mensagem ser enviada
            Thread.Sleep(100);
            
            // Sai do grupo multicast
            try
            {
                _udpClient.DropMulticastGroup(NetworkConfig.MulticastAddress);
            }
            catch { }
            
            // Fecha o socket
            _udpClient.Close();
            
            // Aguarda a thread de recepção terminar
            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Join(1000);
            }
            
            Console.WriteLine("\nChat encerrado. Até logo!");
        }
        
        public void Dispose()
        {
            Stop();
            _udpClient?.Dispose();
        }
    }
}
