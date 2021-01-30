using Oraculo.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oraculo
{
    class Program
    {
        static IManipuladorDeMensagens mensagens;
        static Chatbot _chatbot = new Chatbot();

        static void Main(string[] args)
        {
            //mensagens = new ManipuladorDeMensagens("localhost");
            mensagens = new ManipuladorDeMensagens("191.232.234.20");
            
            mensagens.MensagemRecebida += Mensagens_MensagemRecebida;
            Console.WriteLine($"Oraculo iniciado às {DateTime.Now}");
            Console.ReadKey();
            mensagens.Dispose();
        }

        private static void Mensagens_MensagemRecebida(Mensagem mensagem)
        {
            Mensagem _msg = mensagem;
            Console.WriteLine($"Mensagem recebida: Id={_msg.Id}, texto={_msg.Texto}");

            var resposta = _chatbot.Pergunta(_msg.Texto);

            mensagens.EnviaMensagem(new Mensagem() { Id = _msg.Id, Texto = resposta });
        }
    }
}
