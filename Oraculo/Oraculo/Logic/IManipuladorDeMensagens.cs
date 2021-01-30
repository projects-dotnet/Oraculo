using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oraculo.Logic
{
    public delegate void MensagemRecebidaDelegate(Mensagem mensagem);

    interface IManipuladorDeMensagens
    {
        void EnviaMensagem(Mensagem mensagem);
        event MensagemRecebidaDelegate MensagemRecebida;
        void Dispose();
    }
}
