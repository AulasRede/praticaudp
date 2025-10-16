using System.Net;

namespace UdpMulticastChat
{
    /// <summary>
    /// Configurações de rede para o chat multicast UDP
    /// </summary>
    public static class NetworkConfig
    {
        // Endereço IP multicast (faixa 239.255.x.x é para uso local)
        public static readonly IPAddress MulticastAddress = IPAddress.Parse("239.255.0.1");
        
        // Porta UDP para comunicação
        public const int Port = 5000;
        
        // Tamanho máximo do buffer de recepção
        public const int BufferSize = 8192;
        
        // Tempo limite para operações de rede (em milissegundos)
        public const int SocketTimeout = 5000;
        
        // TTL (Time To Live) para pacotes multicast
        // 1 = mesma sub-rede, 32 = mesma região, 255 = global
        public const int MulticastTTL = 32;
    }
}
